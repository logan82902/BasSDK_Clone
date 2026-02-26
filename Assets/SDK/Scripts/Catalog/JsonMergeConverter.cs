using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ThunderRoad
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class JsonMergeKeyAttribute : System.Attribute
    {
        public string MergeKeyValueProvider { get; set; }
    }

    public class KeyedListMergeConverter : JsonConverter
    {
        private readonly IContractResolver _contractResolver;
        private static readonly object ContractResolverLock = new object();
        public KeyedListMergeConverter(IContractResolver contractResolver)
        {
            if (contractResolver == null)
                throw new ArgumentNullException(nameof(contractResolver));
            this._contractResolver = contractResolver;
        }

        static bool CanConvert(IContractResolver contractResolver, Type objectType, out Type elementType, out JsonProperty keyProperty, out JsonMergeKeyAttribute keyAttribute)
        {
            elementType = objectType.GetListType();
            if (elementType == null)
            {
                keyProperty = null;
                keyAttribute = null;
                return false;
            }
            var contract = contractResolver.ResolveContract(elementType) as JsonObjectContract;
            if (contract == null)
            {
                keyProperty = null;
                keyAttribute = null;
                return false;
            }
            keyProperty = null; keyAttribute = null;
            for (var i = 0; i < contract.Properties.Count; i++)
            {
                var property = contract.Properties[i];
                if (property.AttributeProvider == null) continue;
                var attributes = property.AttributeProvider.GetAttributes(typeof(JsonMergeKeyAttribute), true);
                if (attributes.Count <= 0) continue;
                keyProperty = property;
                keyAttribute = attributes[0] as JsonMergeKeyAttribute;
                break;
            }
            return keyProperty != null;
        }

        public override bool CanConvert(Type objectType) => CanConvert(_contractResolver, objectType, out _, out _, out _);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (_contractResolver != serializer.ContractResolver) throw new InvalidOperationException("Inconsistent contract resolvers");
            if (!CanConvert(_contractResolver, objectType, out var elementType, out var keyProperty, out var keyAttribute)) throw new JsonSerializationException($"Invalid input type {objectType}");

            if (reader.TokenType == JsonToken.Null) return existingValue;

            var list = existingValue as IList;
            if (list == null || list.Count == 0)
            {
                try
                {
                    lock (ContractResolverLock) // Contract resolver is not thread safe when creating objects, so lock to prevent issues in multithreaded scenarios
                    {
                        var defaultCreator = _contractResolver.ResolveContract(objectType).DefaultCreator;
                        if (defaultCreator != null) list = (IList)defaultCreator();
                    }
                    if(list != null) serializer.Populate(reader, list);
                } 
                catch (Exception ex)
                {
                    Debug.LogError($"Exception when trying to read JSON into list of type {objectType}: {ex}");
                    // rethrow the exception
                    throw;
                }
            }
            else
            {
                var jArray = JArray.Load(reader);
                var comparer = new KeyedListMergeComparer();

                // Try to get the key not set handler method so we can run this and get a key for it when reading JSON if it's missing from the JSON, or is null/default
                MethodInfo keyNotSetHandlerMethod = null;
                if (!keyAttribute.MergeKeyValueProvider.IsNullOrEmptyOrWhitespace())
                {
                    if (keyProperty.DeclaringType != null) keyNotSetHandlerMethod = keyProperty.DeclaringType.GetMethod(keyAttribute.MergeKeyValueProvider, BindingFlags.Public | BindingFlags.Static);
                }

                // Custom change to throw error if JsonMergeKey do not exist in the JSON
                string propertyName = keyProperty.PropertyName;
                JToken jToken = jArray.FirstOrDefault(i => i[propertyName] != null);
                if (jToken == null)
                {
                    if (keyNotSetHandlerMethod != null)
                    {
                        object keyValue = keyNotSetHandlerMethod.Invoke(null, null);
                        jToken = new JProperty(propertyName, keyValue);
                        jArray.Children().Append(jToken);
                        Debug.LogWarning($"JSON loaded with missing JsonMergeKey [\"{propertyName}\"] but was automatically populated during read: {keyValue.ToString()}");
                    }
                    else
                    {
                        throw new InvalidOperationException($"JsonMergeKey [\"{propertyName}\"] is missing");
                    }
                }
                else if (jToken is JProperty jProperty && jProperty.Value.Type == JTokenType.Null)
                {
                    if (keyNotSetHandlerMethod != null)
                    {
                        object keyValue = keyNotSetHandlerMethod.Invoke(null, null);
                        jProperty.Value.Replace(JValue.FromObject(keyValue));
                        Debug.LogWarning($"JSON loaded with null JsonMergeKey [\"{propertyName}\"] but was automatically populated during read: {keyValue.ToString()}");
                    }
                    else
                    {
                        throw new InvalidOperationException($"JsonMergeKey [\"{propertyName}\"] is null");
                    }
                }

                var lookup = jArray.ToLookup(i => i[propertyName]?.ToObject(keyProperty.PropertyType, serializer), comparer);
                var done = new HashSet<JToken>();
                foreach (var item in list)
                {
                    var key = keyProperty.ValueProvider.GetValue(item);
                    var replacement = lookup[key].Where(v => !done.Contains(v)).FirstOrDefault();
                    if (replacement != null)
                    {
                        using (var subReader = replacement.CreateReader())
                            serializer.Populate(subReader, item);
                        done.Add(replacement);
                    }
                }
                // Populate the NEW items into the list.
                if (done.Count < jArray.Count)
                    foreach (var item in jArray.Where(i => !done.Contains(i)))
                    {
                        list.Add(item.ToObject(elementType, serializer));
                    }
            }
            return list;
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        class KeyedListMergeComparer : IEqualityComparer<object>
        {
            #region IEqualityComparer<object> Members

            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                if (x is string && y is string && StringComparer.OrdinalIgnoreCase.Equals(x, y)) return true; // Custom change to ignore case in string
                if (object.ReferenceEquals(x, y)) return true;
                else if (x == null || y == null) return false;
                return x.Equals(y);
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
                if (obj == null) return 0;
                if (obj is string) return StringComparer.OrdinalIgnoreCase.GetHashCode(obj); // Custom change to ignore case in string
                return obj.GetHashCode();
            }

            #endregion
        }
    }

    public static class TypeExtensions
    {
        public static Type GetListType(this Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                {
                    var genType = type.GetGenericTypeDefinition();
                    if (genType == typeof(List<>))
                        return type.GetGenericArguments()[0];
                }
                type = type.BaseType;
            }
            return null;
        }
    }

}
