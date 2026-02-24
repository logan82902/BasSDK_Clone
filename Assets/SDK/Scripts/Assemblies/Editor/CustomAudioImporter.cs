using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace ThunderRoad
{
    [CustomEditor(typeof(AudioImporter))]
    public class CustomAudioImporter : Editor
    {
        
        Editor _internalEditor;

        void OnEnable()
        {
           
            var inspectorType = Type.GetType(
                "UnityEditor.AudioImporterInspector, UnityEditor");

            _internalEditor = CreateEditor(targets, inspectorType);
        }

        void OnDisable()
        {
            if (_internalEditor != null)
                DestroyImmediate(_internalEditor);
        }

        public override void OnInspectorGUI()
        {
            _internalEditor.OnInspectorGUI();

            DrawClassificationUI();
        }
        void DrawClassificationUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Audio Classification", EditorStyles.boldLabel);
            
            AudioImporter importer = (AudioImporter)target;
            var labels = AssetDatabase.GetLabels(importer);

            bool diegetic = Array.Exists(labels, l => l == AudioContainer.Diegetic);
            
            EditorGUI.BeginChangeCheck();
            diegetic = EditorGUILayout.Toggle("Diegetic", diegetic);

            if (EditorGUI.EndChangeCheck())
            {
                AudioContainer.SetLabels(importer, diegetic);
            }
            
            if(GUILayout.Button("Reimport")) 
            {
                AudioContainer.SetLabels(importer, diegetic);
                AssetDatabase.ImportAsset(importer.assetPath);
            }
        }
    }

    class AudioImportProcessor : AssetPostprocessor
    {
        public static bool Enabled = true;
        const long SizeThresholdBytes = 200 * 1024; // 200 KB
        
        void OnPreprocessAudio()
        {
            if (!Enabled) return;
            var importer = (AudioImporter)assetImporter;
            var labels = AssetDatabase.GetLabels(importer);
            if(labels.Length == 0)
            {
                //if it has no labels, set default labels, default to Diegetic true
                AudioContainer.SetLabels(importer, true);
            }
            bool isDiegetic = labels.Contains(AudioContainer.Diegetic);
            bool nonDiegeticLabel = labels.Contains(AudioContainer.NonDiegetic);

            if(isDiegetic && nonDiegeticLabel)
            {
                Debug.LogWarning($"Audio clip '{importer.assetPath}' has both Diegetic and Non-Diegetic labels. Defaulting to Diegetic settings.");
            }
            bool isShort = labels.Contains(AudioContainer.Short);
            bool isLong = labels.Contains(AudioContainer.Long);
            if(isShort && isLong)
            {
                Debug.LogWarning($"Audio clip '{importer.assetPath}' has both Short and Long labels. Defaulting to Short settings.");
                isLong = false;
            }
            
            importer.ClearSampleSettingOverride("Standalone");
            importer.ClearSampleSettingOverride("Android");
            importer.ClearSampleSettingOverride("PS5");
            importer.ClearSampleSettingOverride("PS4");
            
            
            long fileSizeBytes = GetFileSize(importer.assetPath);
            
            var settings = importer.defaultSampleSettings;
            
            ApplyRules(importer, ref settings, fileSizeBytes,isDiegetic, isShort, isLong);
            

            importer.defaultSampleSettings = settings;
        }
        
        static long GetFileSize(string assetPath)
        {
            var fullPath = Path.GetFullPath(assetPath);
            return File.Exists(fullPath)
                ? new FileInfo(fullPath).Length
                : 0;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="importer"></param>
        /// <param name="settings"></param>
        /// <param name="fileSizeBytes"></param>
        /// <param name="isDiegetic">If its heard in world, by npcs, ambient environment noise, spells, footsteps etc, its diegetic</param>
        /// <param name="isShort">less than 1 second or so</param>
        /// <param name="isLong">longer than 10 seconds</param>
        static void ApplyRules(
            AudioImporter importer,
            ref AudioImporterSampleSettings settings,
            long fileSizeBytes, bool isDiegetic, bool isShort, bool isLong)
        {
            importer.forceToMono = isDiegetic;
            
            // Disable preloading. This means it will be loaded on demand, could cause a delay the first time it's played.
            importer.preloadAudioData = false;
            // ensure that it uses a background thread to load so it doesnt block the main thread
            importer.loadInBackground = true;

            // Load type
            // anything under 200KB should be decompressed on load because the buffer allocates 200KB anyway
            if (fileSizeBytes < SizeThresholdBytes)
            {
                settings.loadType = AudioClipLoadType.DecompressOnLoad;
            }
            else
            {
                //longer audio that is longer than 200KB should be streamed, anything else compressed in memory to save memory
                settings.loadType = isLong ? AudioClipLoadType.Streaming : AudioClipLoadType.CompressedInMemory;    
            }
            

            // Compression
            settings.compressionFormat = isShort ? AudioCompressionFormat.ADPCM : AudioCompressionFormat.Vorbis;

            // Frequency
            settings.sampleRateSetting = AudioSampleRateSetting.OptimizeSampleRate;
            
            //get the importer as a serialized object to disable the m_Normalize field
            SerializedObject serializedObject = new SerializedObject(importer);
            SerializedProperty m_Normalize = serializedObject.FindProperty("m_Normalize");
            if (m_Normalize != null)
            {
                m_Normalize.boolValue = false;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
