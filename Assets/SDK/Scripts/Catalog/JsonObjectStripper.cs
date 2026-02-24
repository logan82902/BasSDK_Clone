using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ThunderRoad
{
#if UNITY_EDITOR
    public class JsonObjectStripper
    {
        public static JObject StripDefaultValues<T>(T target) where T : new()
        {
            JObject targetObj = new JObject(target);
            JObject defaultObj = new JObject(new T());
            return StripMatchingRecursive(targetObj, defaultObj);
        }

        public static JObject StripMatchingRecursive(JObject targetObj, JObject baseObj)
        {
            if (targetObj == null) return null;
            if (baseObj == null) return targetObj;

            var typeCache = new Dictionary<string, Type>(StringComparer.Ordinal);
            var mergeKeyNameCache = new Dictionary<Type, HashSet<string>>();

            var dupMergeSettings = new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Replace,
                MergeNullValueHandling = MergeNullValueHandling.Merge
            };

            static bool IsTokenNull(JToken t) => t == null || t.Type == JTokenType.Null || t.Type == JTokenType.Undefined;

            static bool IsTokenEmptyString(JToken t) => t?.Type == JTokenType.String && t.Value<string>() == string.Empty;

            static bool TokensEquivalent(JToken a, JToken b)
            {
                if (JToken.DeepEquals(a, b)) return true;
                // if token A is null and token B is an empty string (or vice-versa), they're considered equivalent for the purposes of stripping
                return (IsTokenNull(a) && IsTokenEmptyString(b)) || (IsTokenNull(b) && IsTokenEmptyString(a));
            }

            static string ToCamelCase(string s)
            {
                if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0])) return s;
                return s.Length == 1 ? char.ToLowerInvariant(s[0]).ToString()
                                     : char.ToLowerInvariant(s[0]) + s.Substring(1);
            }

            static string NormalizeKey(JToken keyTok)
            {
                // Keep key normalization consistent with TokensEquivalent (null == "")
                if (IsTokenNull(keyTok) || IsTokenEmptyString(keyTok)) return "\u0000";

                if (keyTok is JValue v)
                    return v.Value.ToString().ToLowerInvariant() ?? "\u0000";

                return keyTok.ToString(Formatting.None);
            }

            Type ResolveObjectType(JObject obj)
            {
                var typeToken = obj?["$type"];
                if (typeToken?.Type != JTokenType.String) return null;

                var typeName = typeToken.Value<string>();
                if (string.IsNullOrWhiteSpace(typeName)) return null;

                if (typeCache.TryGetValue(typeName, out var cached)) return cached;

                var t = Type.GetType(typeName, throwOnError: false);
                if (t == null)
                {
                    var fullName = typeName.Split(',')[0].Trim();
                    foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        t = asm.GetType(fullName, throwOnError: false, ignoreCase: false);
                        if (t != null) break;
                    }
                }

                typeCache[typeName] = t;
                return t;
            }

            HashSet<string> GetMergeKeyJsonNames(JObject owner, JObject ownerBase)
            {
                var ownerType = ResolveObjectType(owner) ?? ResolveObjectType(ownerBase);
                if (ownerType == null) return null;

                if (mergeKeyNameCache.TryGetValue(ownerType, out var cached))
                    return cached;

                var names = new HashSet<string>(StringComparer.Ordinal);

                var members = ownerType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var member in members)
                {
                    if (member is PropertyInfo pi)
                    {
                        if (pi.GetIndexParameters().Length != 0) continue;
                    }
                    else if (member is not FieldInfo)
                    {
                        continue;
                    }

                    bool hasMergeKeyAttr = member
                        .GetCustomAttributes(inherit: true)
                        .Any(a => a != null && a.GetType().Name == "JsonMergeKeyAttribute");

                    if (!hasMergeKeyAttr) continue;

                    void Add(string n)
                    {
                        if (!string.IsNullOrWhiteSpace(n))
                            names.Add(n);
                    }

                    var jp = member.GetCustomAttributes(typeof(JsonPropertyAttribute), inherit: true)
                                   .FirstOrDefault() as JsonPropertyAttribute;
                    if (jp != null && !string.IsNullOrWhiteSpace(jp.PropertyName))
                        Add(jp.PropertyName);

                    Add(member.Name);
                    Add(ToCamelCase(member.Name));
                }

                mergeKeyNameCache[ownerType] = names;
                return names;
            }

            bool IsMergeKeyField(JObject owner, JObject ownerBase, string propName)
            {
                var set = GetMergeKeyJsonNames(owner, ownerBase);
                return set != null && set.Contains(propName);
            }

            bool IsProtectedField(JObject owner, JObject ownerBase, string propName, bool root)
            {
                // the "$type" field should never be stripped under any circumstances unless removed as part of an otherwise empty object
                if (propName == "$type") return true;
                if (root)
                {
                    // if this is the root object, we never strip the id or version
                    if (propName == "id") return true;
                    if (propName == "version") return true;
                }
                // if it's a merge key, we never strip this either
                if (IsMergeKeyField(owner, ownerBase, propName)) return true;
                return false;
            }

            bool TryGetArrayMergeKeyName(JArray tArr, JArray bArr, out string keyName)
            {
                keyName = null;

                if (tArr == null || bArr == null) return false;
                if (tArr.Count == 0 || bArr.Count == 0) return false;
                if (tArr.Any(x => x is not JObject) || bArr.Any(x => x is not JObject)) return false;

                HashSet<string> intersection = null;

                foreach (var obj in tArr.Cast<JObject>().Concat(bArr.Cast<JObject>()))
                {
                    var mergeNames = GetMergeKeyJsonNames(obj, ownerBase: null);
                    if (mergeNames == null || mergeNames.Count == 0) return false;

                    var present = new HashSet<string>(StringComparer.Ordinal);
                    foreach (var n in mergeNames)
                    {
                        if (obj.TryGetValue(n, StringComparison.Ordinal, out _))
                            present.Add(n);
                    }

                    if (present.Count == 0) return false;

                    intersection = intersection == null
                        ? present
                        : new HashSet<string>(intersection.Intersect(present), StringComparer.Ordinal);

                    if (intersection.Count == 0) return false;
                }

                keyName = intersection.OrderBy(n => n, StringComparer.Ordinal).First();
                return true;
            }

            void MergeDuplicatesInPlace(JArray arr, string keyName)
            {
                var firstByKey = new Dictionary<string, JObject>(StringComparer.Ordinal);

                for (int i = 0; i < arr.Count; i++)
                {
                    if (arr[i] is not JObject obj) continue;
                    if (!obj.TryGetValue(keyName, StringComparison.Ordinal, out var keyTok)) continue;

                    var k = NormalizeKey(keyTok);

                    if (!firstByKey.TryGetValue(k, out var first))
                    {
                        firstByKey[k] = obj;
                        continue;
                    }

                    first.Merge(obj, dupMergeSettings); // later wins
                    arr.RemoveAt(i);
                    i--;
                }
            }

            Dictionary<string, JObject> BuildMergedMap(JArray arr, string keyName)
            {
                var map = new Dictionary<string, JObject>(StringComparer.Ordinal);

                foreach (var tok in arr)
                {
                    if (tok is not JObject obj) continue;
                    if (!obj.TryGetValue(keyName, StringComparison.Ordinal, out var keyTok)) continue;

                    var k = NormalizeKey(keyTok);

                    if (!map.TryGetValue(k, out var merged))
                        map[k] = (JObject)obj.DeepClone();
                    else
                        merged.Merge(obj, dupMergeSettings); // later wins
                }

                return map;
            }

            // returns true if the object has no meaningful differences and can be stripped
            bool StripObject(JObject tObj, JObject bObj, bool isRoot)
            {
                if (tObj == null) return true;
                if (bObj == null) return false;

                foreach (var prop in tObj.Properties().ToList())
                {
                    var name = prop.Name;

                    // never strip a protected field
                    if (IsProtectedField(tObj, bObj, name, isRoot))
                        continue;

                    // if the base object is missing this field, skip it and don't strip it
                    if (!bObj.TryGetValue(name, StringComparison.Ordinal, out var bTok))
                        continue;

                    var tTok = prop.Value;

                    // if both tokens are object tokens, recurse to see if it should be kept or not
                    if (tTok is JObject tChildObj && bTok is JObject bChildObj)
                    {
                        bool childRemovable = StripObject(tChildObj, bChildObj, false);
                        if (childRemovable) prop.Remove();
                        continue;
                    }

                    // if both tokens are array tokens, check matching elements and strip, then optionally remove the array if no elements remain
                    if (tTok is JArray tArr && bTok is JArray bArr)
                    {
                        bool arrRemovable = StripArray(tArr, bArr);
                        if (arrRemovable) prop.Remove();
                        continue;
                    }

                    // otherwise for non-object and non-array tokens, check equivalence and strip if matching
                    if (TokensEquivalent(tTok, bTok))
                        prop.Remove();
                }

                if (isRoot) return false;

                // if this is a non-root object, check if there are any meaningful differences left
                bool hasMeaningfulDiffs = tObj.Properties().Any(p => !IsProtectedField(tObj, bObj, p.Name, false));
                // if there are any meanintful differences, we don't strip this
                if (hasMeaningfulDiffs)
                    return false;

                // otherwise, we remove all the properties in the object and strip
                foreach (var p in tObj.Properties().ToList())
                    p.Remove();

                return true;
            }

            bool WouldElementBeRemovableAgainst(JToken targetToken, JToken baseToken)
            {
                if (TokensEquivalent(targetToken, baseToken)) return true;

                if (targetToken is JObject tObj && baseToken is JObject bObj)
                {
                    var clone = (JObject)tObj.DeepClone();
                    return StripObject(clone, bObj, false);
                }

                if (targetToken is JArray tArr && baseToken is JArray bArr)
                {
                    var clone = (JArray)tArr.DeepClone();
                    return StripArray(clone, bArr);
                }

                return false;
            }

            // removes elements from targetArray which perfectly match any element(s) in baseArray
            bool StripArray(JArray targetArray, JArray baseArray)
            {
                if (targetArray == null) return true;
                if (baseArray == null) return false;

                if (TryGetArrayMergeKeyName(targetArray, baseArray, out var keyName))
                {
                    MergeDuplicatesInPlace(targetArray, keyName);

                    var baseMap = BuildMergedMap(baseArray, keyName);

                    for (int i = targetArray.Count - 1; i >= 0; i--)
                    {
                        if (targetArray[i] is not JObject tObjEl) continue;
                        if (!tObjEl.TryGetValue(keyName, StringComparison.Ordinal, out var tKeyTok)) continue;

                        var k = NormalizeKey(tKeyTok);

                        if (!baseMap.TryGetValue(k, out var bObjEl))
                            continue; // not in base => diff, keep

                        bool removable = StripObject(tObjEl, bObjEl, false);
                        if (removable)
                            targetArray.RemoveAt(i);
                    }

                    return targetArray.Count == 0;
                }

                var availableBase = new List<JToken>(baseArray);

                for (int i = targetArray.Count - 1; i >= 0; i--)
                {
                    var tEl = targetArray[i];

                    int matchIndex = -1;
                    for (int j = 0; j < availableBase.Count; j++)
                    {
                        if (WouldElementBeRemovableAgainst(tEl, availableBase[j]))
                        {
                            matchIndex = j;
                            break;
                        }
                    }

                    if (matchIndex >= 0)
                    {
                        targetArray.RemoveAt(i);
                        availableBase.RemoveAt(matchIndex); // consume one match (handles duplicates correctly)
                    }
                }

                return targetArray.Count == 0;
            }

            StripObject(targetObj, baseObj, true);
            return targetObj;
        }
    }
#endif
}
