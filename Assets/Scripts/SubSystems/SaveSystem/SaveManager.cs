using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using Utils;

namespace SubSystems.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        // Path where save files will be stored (in Resources folder)
        private static string saveFilePath = "Assets/Resources/";
        
        private const string SavePrefix = "Roulette_"; // Prefix folder name for save files

        // Called when the application is quitting to save all data
        private void OnApplicationQuit()
        {
            SaveAll();
        }

        // Called when the object becomes enabled, triggers loading after short delay
        private void OnEnable()
        {
            Invoke(nameof(LoadAll), 1); // Start loading all saved data after 1 second delay
        }

        // Saves the state of all MonoBehaviours that implement ISaveable<T>
        private void SaveAll()
        {
            // Find all MonoBehaviours in the scene (including inactive)
            var monoBehaviours = FindObjectsOfType<MonoBehaviour>(true);

            foreach (var mono in monoBehaviours)
            {
                // Find interfaces of type ISaveable<T>
                var interfaces = mono.GetType()
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISaveable<>));

                foreach (var iface in interfaces)
                {
                    try
                    {
                        // Get SaveKey and CaptureState method from the interface
                        var saveKeyProp = iface.GetProperty("SaveKey");
                        var captureMethod = iface.GetMethod("CaptureState");

                        var saveKey = (string)saveKeyProp?.GetValue(mono);
                        var state = captureMethod?.Invoke(mono, null);
                        
                        // Serialize the saved state to JSON
                        string json = JsonUtility.ToJson(state);

                        // Create directory if it doesn't exist
                        string directoryPath = $"{saveFilePath}/{SavePrefix}";
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        // Write JSON to file named by saveKey
                        File.WriteAllText($"{directoryPath}/{saveKey}.txt", json);

#if UNITY_EDITOR
                        // Refresh AssetDatabase so the file appears in the editor immediately
                        UnityEditor.AssetDatabase.Refresh();
#endif
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Save data went wrong, stack: {e}");
                    }
                }
            }
        }

        // Loads the state of all ISaveable<T> MonoBehaviours from saved JSON files
        public void LoadAll()
        {
            var monoBehaviours = FindObjectsOfType<MonoBehaviour>(true);

            foreach (var mono in monoBehaviours)
            {
                var interfaces = mono.GetType()
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISaveable<>));

                foreach (var iface in interfaces)
                {
                    try
                    {
                        var saveKeyProp = iface.GetProperty("SaveKey");
                        var restoreMethod = iface.GetMethod("RestoreState");
                        var genericArg = iface.GetGenericArguments()[0];

                        var saveKey = (string)saveKeyProp.GetValue(mono);

                        // Load JSON text asset from Resources folder
                        TextAsset textAsset = Resources.Load<TextAsset>($"{SavePrefix}/{saveKey}");
                        if (textAsset == null) continue;

                        var json = textAsset.text;

                        // Deserialize JSON into the appropriate type
                        var state = JsonUtility.FromJson(json, genericArg);

                        // Restore the saved state by invoking RestoreState
                        restoreMethod?.Invoke(mono, new[] { state });
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Load data went wrong, stack: {e}");
                    }
                }
            }

            // Start the UI fade-out animation after loading
            SceneHideCG.Instance.FadeOut(2);
        }
    }
}
