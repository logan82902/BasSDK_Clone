#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TriInspector;
using UnityEditor;
using UnityEngine;

namespace ThunderRoad
{
    [CreateAssetMenu(menuName = "ThunderRoad/Script Bridge")]
    [DeclareBoxGroup("Path", Title = "CsProj Path")]
    [DeclareHorizontalGroup("Path/Horiz", Sizes = new float[] { 0f, 120f })]
    [DeclareBoxGroup("Configs", HideTitle = true)]
    [DeclareVerticalGroup("Directories")]
    [DeclareBoxGroup("Directories/Buttons", HideTitle = true)]
    [DeclareHorizontalGroup("Directories/Buttons/Horiz")]
    [DeclareBoxGroup("Mod", Title = "Connected Mod")]
    [DeclareHorizontalGroup("Mod/Horiz")]
    public class ScriptBridge : ScriptableObject
    {
        private static readonly List<string> ignoredFolders = new List<string>() { "bin", "obj", "properties" };

        private static readonly List<string> assemblyIncludeMatchList = new List<string>() { "Unity", "NavMeshComponents", "ThunderRoad", "ThunderSpline", "TriInspector" };

        private static readonly List<string> cleanUpTargetFiles = new List<string>() { ".dll", ".pdb", ".xml" };

        private const string dllListPath = "Assets\\SDK\\Scripts\\Assemblies\\Editor\\GameAssemblies.txt";

        [Group("Path/Horiz")]
        [ReadOnly]
        [HideLabel]
        public string csProjPath;
        private bool hasCsProj => !csProjPath.IsNullOrEmptyOrWhitespace();

        [SerializeReference]
        [ShowIf(nameof(hasCsProj))]
        [InlineProperty]
        [HideReferencePicker]
        [HideLabel]
        [Group("Configs")]
        public CsProjInfo csProjInfo;

        [ShowIf(nameof(hasCsProj))]
        [Group("Directories")]
        [LabelText("Linkable Directories")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, Draggable = false, AlwaysExpanded = true)]
        public List<LinkableDir> linkableDirs = new List<LinkableDir>();

        [ShowInInspector]
        [Group("Mod")]
        [Dropdown(nameof(GetAllAssetBundleGroups))]
        [HideLabel]
        public AssetBundleGroup linkedMod
        {
            get => _linkedMod;
            set
            {
                if (_linkedMod != null && _linkedMod.scriptBridges.Contains(this)) _linkedMod.scriptBridges.Remove(this);
                _linkedMod = value;
                if (_linkedMod != null && !_linkedMod.scriptBridges.Contains(this)) _linkedMod.scriptBridges.Add(this);
            }
        }
        [SerializeReference]
        [HideInInspector]
        private AssetBundleGroup _linkedMod;

        public enum LinkState
        {
            Unlinked,
            Linked,
        }

        public enum BuildResult
        {
            Exception,
            Failure,
            Success,
            CleanSuccess,
        }

        [System.Serializable]
        [DeclareHorizontalGroup("Horiz", Sizes = new float[] { 0f, 0f, 160f})]
        public class CsProjInfo
        {
            [HideInInspector]
            public string path;
            [HideInInspector]
            public string assemblyName;
            [HideInInspector]
            public string rootNamespace;
            [SerializeReference]
            [HideInInspector]
            public List<BuildConfig> buildConfigs;

            [SerializeReference]
            [Group("Horiz")]
            [LabelText("PCVR Build Config")]
            [AutoLabelWidth(AddedWidth = 7f)]
            [Dropdown(nameof(GetBuildConfigOptions), Searchable = false)]
            public BuildConfig pcvrBuildConfig;
            [SerializeReference]
            [Group("Horiz")]
            [LabelText("Android Build Config")]
            [AutoLabelWidth(AddedWidth = 7f)]
            [Dropdown(nameof(GetBuildConfigOptions), Searchable = false)]
            public BuildConfig androidBuildConfig;

            public class BuildConfigDictionary : UnitySerializedDictionary<string, BuildConfig> { }

            [System.Serializable]
            [AutoLabelWidth(AddedWidth = 7f)]
            [DeclareHorizontalGroup("Info")]
            public class BuildConfig
            {
                public string name;
                public string defines;
                
                public enum Platform
                {
                    None,
                    PCVR,
                    Android,
                    Both,
                }

                public BuildConfig(string name, string defines)
                {
                    this.name = name;
                    this.defines = defines;
                }
            }

            public CsProjInfo(string csProjPath)
            {
                path = csProjPath;
                XDocument xmlDoc = XDocument.Load(csProjPath);
                XNamespace ns = xmlDoc.Root.GetDefaultNamespace();
                assemblyName = xmlDoc.Root.Element(ns + "PropertyGroup")?.Element(ns + "AssemblyName")?.Value;
                rootNamespace = xmlDoc.Root.Element(ns + "PropertyGroup")?.Element(ns + "RootNamespace")?.Value;
                CollectBuildConfigs(xmlDoc, ns);
            }


            [Button]
            [Group("Horiz")]
            public void RefreshBuildConfigs() => CollectBuildConfigs();

            private void CollectBuildConfigs(XDocument xmlDoc = null, XNamespace ns = null)
            {
                if (xmlDoc == null) xmlDoc = XDocument.Load(path);
                if (ns == null) ns = xmlDoc.Root.GetDefaultNamespace();
                buildConfigs = new List<BuildConfig>();
                var configRegex = new Regex(@"==\s*['""]([^'""|]+)");
                var groups = xmlDoc.Descendants(ns + "PropertyGroup").Where(pg => pg.Attribute("Condition") != null);
                foreach (var group in groups)
                {
                    var condition = group.Attribute("Condition").Value;
                    var match = configRegex.Match(condition);

                    if (match.Success)
                    {
                        string configName = match.Groups[1].Value;
                        var definesElement = group.Element(ns + "DefineConstants");
                        string defines = definesElement?.Value ?? "";
                        if (!string.IsNullOrEmpty(defines) && !defines.EndsWith(";"))
                        {
                            defines += ";";
                        }
                        BuildConfig buildConfig = new BuildConfig(configName, defines);
                        buildConfigs.Add(buildConfig);
                    }
                }
                string startPcvrBuildConfig = pcvrBuildConfig?.name ?? string.Empty;
                string startAndroidBuildConfig = androidBuildConfig?.name ?? string.Empty;
                pcvrBuildConfig = androidBuildConfig = buildConfigs[0];
                foreach (BuildConfig config in buildConfigs)
                {
                    if (config.name == startPcvrBuildConfig) pcvrBuildConfig = config;
                    if (config.name == startAndroidBuildConfig) androidBuildConfig = config;
                }
            }

            private TriDropdownList<BuildConfig> GetBuildConfigOptions()
            {
                TriDropdownList<BuildConfig> result = new TriDropdownList<BuildConfig>();
                foreach (BuildConfig buildConfig in buildConfigs)
                {
                    result.Add(buildConfig.name, buildConfig);
                }
                return result;
            }

            private const string AssemblyDefinitionTemplate = @"
            {
                ""name"": ""{AssemblyName}"",
                ""rootNamespace"": ""{RootNamespace}"",
                ""references"": {References},
                ""includePlatforms"": [],
                ""excludePlatforms"": [],
                ""allowUnsafeCode"": false,
                ""overrideReferences"": false,
                ""precompiledReferences"": [],
                ""autoReferenced"": false,
                ""defineConstraints"": [],
                ""versionDefines"": [],
                ""noEngineReferences"": false
            }";

            public string ToAssemDefText()
            {
                string references = "[";
                foreach (string assetGuid in AssetDatabase.FindAssets("t:AssemblyDefinitionAsset"))
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                    if (!AssemblyShouldBeIncluded(assetPath)) continue;
                    if (assetPath.Contains("Editor")) continue;
                    if (references[^1] != '[') references += ",";
                    references += $" \"GUID:{assetGuid}\"";
                }
                references += " ]";
                return AssemblyDefinitionTemplate.Replace("{AssemblyName}", assemblyName).Replace("{RootNamespace}", rootNamespace).Replace("{References}", references);
            }

            private bool AssemblyShouldBeIncluded(string assemblyPath)
            {
                foreach (string includedAssembly in assemblyIncludeMatchList)
                {
                    if (assemblyPath.Contains(includedAssembly)) return true;
                }
                return false;
            }
        }

        [System.Serializable]
        [DeclareHorizontalGroup("Info", Sizes = new float[] { 0f, 130f })]
        public class LinkableDir
        {
            [ShowInInspector]
            [PropertyOrder(0)]
            [Group("Info")]
            [ReadOnly]
            [LabelText("Path")]
            [AutoLabelWidth(AddedWidth = 7f)]
            private string displayPath => $"(CsProj Folder)\\{partialPath}";

            [ShowInInspector]
            [PropertyOrder(1)]
            [Group("Info")]
            [HideLabel]
            [EnumToggleButtons]
            public LinkState stateDisplay
            {
                get
                {
                    return Directory.Exists(localPath) ? LinkState.Linked : LinkState.Unlinked;
                }
                set
                {
                    SetLinked(value, true);
                }
            }

            [SerializeField]
            [HideInInspector]
            public ScriptBridge owner;
            [SerializeField]
            [HideInInspector]
            public string fullPath;
            [SerializeField]
            [HideInInspector]
            private string linkedPath;

            private string partialPath => fullPath.Replace($"{Path.GetDirectoryName(owner.csProjPath)}\\", "");

            private string localPath => Path.Combine(owner.GetAssetFolder(), partialPath);

            public LinkableDir(ScriptBridge owner, string path)
            {
                this.owner = owner;
                this.fullPath = path;
            }

            public void SetLinked(LinkState state, bool refresh)
            {
                var startState = this.stateDisplay;
                if (state == startState) return;
                if (state == LinkState.Linked)
                {
                    CreateJunction(localPath, fullPath);
                }
                if (state == LinkState.Unlinked)
                {
                    ProcessRunner.RunProcessAsync("cmd.exe", $"/c rmdir \"{localPath}\"", logToConsole: false);
                    // delete the meta file after
                    File.Delete($"{localPath}.meta");
                }
                if (refresh) AssetDatabase.Refresh();
            }
        }

        public static void CreateJunction(string junctionPath, string targetPath)
        {
            if (!Directory.Exists(targetPath))
            {
                Debug.LogError("Target directory does not exist.");
                return;
            }
            // this passes in the mklink command with /J indicating that it creates a junction (like a symlink, but doesn't require admin/dev privileges)
            ProcessRunner.RunProcessAsync("cmd.exe", $"/c mklink /J \"{junctionPath}\" \"{targetPath}\"", logToConsole: false);
        }

        public static void TriggerDllBuild(CsProjInfo csProj, string outputPath = null, QualityLevel? platform = null, Action<BuildResult> callback = null)
        {
            if (platform == null) platform = Common.GetQualityLevel();
            if (!File.Exists(csProj.path))
            {
                Debug.LogError("Target csproj does not exist.");
                return;
            }
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            CsProjInfo.BuildConfig activeConfig = platform == QualityLevel.Windows ? csProj.pcvrBuildConfig : csProj.androidBuildConfig;
            bool customOutputPath = outputPath != null;
            string outputPathArgument = customOutputPath ? $"-p:OutputPath=\"{outputPath}\"" : "";
            ProcessRunner.RunProcessAsync("dotnet", $"build \"{csProj.path}\" -c {activeConfig.name} {outputPathArgument}", null, null, processResult =>
            {
                string consoleOutput = $"{processResult.Output}\n{processResult.Error}";
                bool buildFail = consoleOutput.Contains("Build FAILED.");
                if (!processResult.Success || buildFail)
                {
                    if (buildFail)
                    {
                        consoleOutput = consoleOutput.Substring(consoleOutput.IndexOf("Build FAILED.")).Trim().Replace("Build FAILED.", "[ScriptBridge] Mod scripts build failure. Expand for more info.");
                    }
                    Debug.LogError(consoleOutput);
                    callback?.Invoke(buildFail ? BuildResult.Failure : BuildResult.Exception);
                    return;
                }
                // if we didn't build to a custom output path, skip the cleanup step
                if (!customOutputPath)
                {
                    callback?.Invoke(BuildResult.Success);
                    return;
                }

                // clean up the output path; remove all unnecessary dlls, pdbs, and xmls
                List<string> allDllNames = new List<string>();
                allDllNames.AddRange(File.ReadAllLines(GetAllDllsPath()));
                foreach (string filePath in Directory.GetFiles(outputPath))
                {
                    string ext = Path.GetExtension(filePath).ToLowerInvariant();
                    if (!cleanUpTargetFiles.Contains(ext)) continue;
                    if (!allDllNames.Contains(Path.GetFileNameWithoutExtension(filePath))) continue;
                    File.Delete(filePath);
                }
                callback?.Invoke(BuildResult.CleanSuccess);
            }, true, false, false);
        }

        private static string GetAllDllsPath() => Path.Combine(Path.GetDirectoryName(Application.dataPath), dllListPath);

        private TriDropdownList<AssetBundleGroup> GetAllAssetBundleGroups()
        {
            TriDropdownList<AssetBundleGroup> results = new TriDropdownList<AssetBundleGroup> { { "None", null } };
            foreach (AssetBundleGroup assetBundleGroup in EditorCommon.GetAllProjectAssets<AssetBundleGroup>())
            {
                results.Add(assetBundleGroup.modName, assetBundleGroup);
            }
            return results;
        }

        private string GetAssetFolder() => Path.GetDirectoryName(Path.Combine(Path.GetDirectoryName(Application.dataPath), AssetDatabase.GetAssetPath(this)));

        [Group("Path/Horiz")]
        [Button("Select CsProj")]
        public void ChangePath()
        {
            string newPath = EditorUtility.OpenFilePanel("Select csproj file...", csProjPath.IsNullOrEmptyOrWhitespace() ? GetAssetFolder() : Path.GetDirectoryName(csProjPath), "csproj");
            if (newPath.IsNullOrEmptyOrWhitespace()) return;
            // if the user selects the same path, return
            if (newPath == csProjPath) return;
            DisconnectCurrentCsProj();
            csProjPath = newPath;
            csProjInfo = new CsProjInfo(csProjPath);
            GenerateAssemDef();
            CollectPotentialLinks();
        }

        [Button]
        [ShowIf(nameof(hasCsProj))]
        [Group("Directories/Buttons/Horiz")]
        public void RefreshLinkableDirectories() => CollectPotentialLinks();

        [Button]
        [ShowIf(nameof(hasCsProj))]
        [Group("Directories/Buttons/Horiz")]
        public void RemoveAllLinks() => RemoveAllLinks(true);

        private void RemoveAllLinks(bool refresh)
        {
            foreach (LinkableDir linkableDir in linkableDirs)
            {
                linkableDir.SetLinked(LinkState.Unlinked, false);
            }
            if (refresh) AssetDatabase.Refresh();
        }

        [Button]
        [ShowIf(nameof(hasCsProj))]
        [Group("Directories/Buttons/Horiz")]
        public void DisconnectCurrentCsProj()
        {
            RemoveAllLinks(false);
            linkableDirs = new List<LinkableDir>();
            string[] files = Directory.GetFiles(GetAssetFolder());
            for (int i = files.Length - 1; i >= 0; i--)
            {
                // remove all asmdef files in the same folder as the asset
                if (files[i].Contains(".asmdef"))
                {
                    File.Delete(files[i]);
                }
            }
            csProjPath = null;
            AssetDatabase.Refresh();
        }

        [DisableIf(nameof(linkedMod), null)]
        [Group("Mod/Horiz")]
        [Button("Build PCVR DLL(s) to BuildStaging")]
        public void BuildPCVRDllsToStaging() => BuildDllsToStaging(QualityLevel.Windows);

        [DisableIf(nameof(linkedMod), null)]
        [Group("Mod/Horiz")]
        [Button("Build Android DLL(s) to BuildStaging")]
        public void BuildAndroidDllsToStaging() => BuildDllsToStaging(QualityLevel.Android);

        [DisableIf(nameof(linkedMod), null)]
        [Group("Mod")]
        [Button("Build DLL(s) to BuildStaging (Both platforms)")]
        public void BuildDllsToStaging()
        {
            BuildDllsToStaging(QualityLevel.Windows);
            BuildDllsToStaging(QualityLevel.Android);
        }

        public string GetStagingPath(QualityLevel platform) => linkedMod.GetAssemblyStagingPath(platform);

        public void BuildDllsToStaging(QualityLevel platform, Action<BuildResult> callback = null) => BuildDlls(GetStagingPath(platform), platform, callback);

        public void BuildDlls(string outputPath, QualityLevel? platform = null, Action<BuildResult> callback = null)
        {
            if (!hasCsProj) return;
            TriggerDllBuild(csProjInfo, outputPath, platform, callback);
        }

        private void CollectPotentialLinks()
        {
            linkableDirs = new List<LinkableDir>();
            foreach (string filePath in Directory.GetDirectories(Path.GetDirectoryName(csProjPath)))
            {
                string folderName = new DirectoryInfo(filePath).Name;
                if (folderName[0] == '.' || ignoredFolders.Contains(folderName.ToLowerInvariant())) continue;
                linkableDirs.Add(new LinkableDir(this, filePath));
            }
        }

        private void GenerateAssemDef()
        {
            if (csProjInfo != null)
            {
                File.WriteAllText(Path.Combine(GetAssetFolder(), $"{csProjInfo.assemblyName}.asmdef"), csProjInfo.ToAssemDefText());
            }
            AssetDatabase.Refresh();
        }
    }
}
#endif