<<<<<<< HEAD
<<<<<<< Updated upstream
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace Kinetix.Internal
{
    internal class FreeAnimationsImportWindow : EditorWindow
    {

        private FreeAnimationManifest manifest;

        [MenuItem("Kinetix/Free Emotes importer")]
        public static void ShowMyEditor()
        {
            // This method is called when the user selects the menu item in the Editor
            EditorWindow wnd = GetWindow<FreeAnimationsImportWindow>();
            wnd.titleContent = new GUIContent("Free Emotes importer");
        }

        void OnGUI()
        {
            if (!Directory.Exists("Assets/StreamingAssets")) {
                Directory.CreateDirectory("Assets/StreamingAssets");
            }

            if (manifest == null) {
                manifest = FreeAnimationManifest.LoadFromPath(Path.Combine(Application.streamingAssetsPath, KinetixConstants.C_FreeAnimationsManifestPath));
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Import Kinetix base emotes")) {
                ImportKinetixTestEmotes();
            }

            if (GUILayout.Button("Remove Kinetix base emotes")) {
                RemoveKinetixTestEmotes();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Refresh Free emotes manifest")) {
                List<string> emotesNames = new List<string>();

                
                DirectoryInfo freeCustomAnimDir = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, KinetixConstants.C_FreeCustomAnimationsPath));

                if (freeCustomAnimDir.Exists) {
                    foreach (DirectoryInfo emotedir in freeCustomAnimDir.GetDirectories()) {
                        emotesNames.Add(emotedir.Name);
                        
                        if (!manifest.Contains(emotedir.Name, KinetixConstants.C_FreeCustomAnimationsPath)) {
                            manifest.AddEmote(KinetixConstants.C_FreeCustomAnimationsPath, emotedir.Name);
                        }
                    }
                }

                manifest.RemoveWhenNotInList(emotesNames, KinetixConstants.C_FreeCustomAnimationsPath);

                emotesNames.Clear();
                DirectoryInfo freeAnimDir = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, KinetixConstants.C_FreeAnimationsPath) + "/");

                if (freeAnimDir.Exists) {
                    foreach (DirectoryInfo emotedir in freeAnimDir.GetDirectories()) {
                        emotesNames.Add(emotedir.Name);

                        if (!manifest.Contains(emotedir.Name, KinetixConstants.C_FreeAnimationsPath)) {
                            manifest.AddEmote(KinetixConstants.C_FreeAnimationsPath, emotedir.Name);
                        }
                    }
                }

                manifest.RemoveWhenNotInList(emotesNames, KinetixConstants.C_FreeAnimationsPath);

                
                manifest.Save();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Import custom free emotes");
            EditorGUILayout.Space();

            
            try {
                if (GUILayout.Button("Select emote catalog folder")) {
                    string path = EditorUtility.OpenFolderPanel("Select emote catalog folder", "", "");

                    if (path.Length != 0) {
                        if (!Directory.Exists("Assets/StreamingAssets/Kinetix")) {
                            Directory.CreateDirectory("Assets/StreamingAssets/Kinetix");
                        }

                        DirectoryInfo freeAnimDir = new DirectoryInfo(path);
                        DirectoryInfo[] emotesDirectories = freeAnimDir.GetDirectories();

                        foreach (DirectoryInfo emotedir in emotesDirectories) {
                            manifest.AddEmote(KinetixConstants.C_FreeCustomAnimationsPath, emotedir.Name);
                        }

                        DirectoryHelper.Copy(path, Application.streamingAssetsPath + KinetixConstants.C_FreeCustomAnimationsPath);

                        manifest.Save();
                        AssetDatabase.Refresh();
                    }
                }
            #pragma warning disable CS0168 // Variable non utilisée
            } catch (ExitGUIException e) {
            #pragma warning restore CS0168
            }

            if (GUILayout.Button("Remove Custom Free emotes")) {
                RemoveKinetixCustomEmotes();
            }
        }

        


        private void ImportKinetixTestEmotes()
        {
            RemoveKinetixTestEmotes();

            if (!Directory.Exists("Assets/StreamingAssets/Kinetix")) {
                Directory.CreateDirectory("Assets/StreamingAssets/Kinetix");
            }

            bool success = AssetDatabase.CopyAsset(KinetixConstants.C_FreeAnimationsAssetPluginPath, KinetixConstants.C_FreeAnimationsAssetSAPath);

            DirectoryInfo freeAnimDir = new DirectoryInfo(KinetixConstants.C_FreeAnimationsAssetSAPath);
            DirectoryInfo[] emotesDirectories = freeAnimDir.GetDirectories();

            foreach (DirectoryInfo emotedir in emotesDirectories) {
                manifest.AddEmote(KinetixConstants.C_FreeAnimationsPath, emotedir.Name);
            }

            if (success) {
                Debug.Log("Imported Kinetix Base Free emotes");
            } else {
                Debug.LogWarning("Import Failed");
            }

            manifest.Save();

            AssetDatabase.Refresh();
        }

        private void RemoveKinetixTestEmotes()
        {
            DirectoryInfo freeAnimDir = new DirectoryInfo(KinetixConstants.C_FreeAnimationsAssetSAPath);

            if (!freeAnimDir.Exists) return;

            DirectoryInfo[] emotesDirectories = freeAnimDir.GetDirectories();

            foreach (DirectoryInfo emotedir in emotesDirectories) {
                manifest.RemoveEmote(KinetixConstants.C_FreeAnimationsPath, emotedir.Name);
            }

            bool success = AssetDatabase.DeleteAsset(KinetixConstants.C_FreeAnimationsAssetSAPath);

            Debug.Log("Removed existing Kinetix Base Free emotes");
            
            manifest.Save();
            
            AssetDatabase.Refresh();
        }

        private void RemoveKinetixCustomEmotes()
        {
            DirectoryInfo freeAnimDir = new DirectoryInfo(KinetixConstants.C_FreeCustomAnimationsAssetSAPath);

            if (!freeAnimDir.Exists) return;
            
            DirectoryInfo[] emotesDirectories = freeAnimDir.GetDirectories();

            foreach (DirectoryInfo emotedir in emotesDirectories) {
                manifest.RemoveEmote(KinetixConstants.C_FreeCustomAnimationsPath, emotedir.Name);
            }


            bool success = AssetDatabase.DeleteAsset(KinetixConstants.C_FreeCustomAnimationsAssetSAPath);

            Debug.Log("Removed existing Custom Free emotes");

            manifest.Save();
            
            AssetDatabase.Refresh();
        }


    }
}
=======
=======
>>>>>>> 06ddde23de98092f4c3f29a5edcf5fb88da8c702
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Kinetix.Internal
{
    internal class FreeAnimationsImportWindow : EditorWindow
    {

        private FreeAnimationManifest manifest;

        [MenuItem("Kinetix/Free Emotes importer")]
        public static void ShowMyEditor()
        {
            // This method is called when the user selects the menu item in the Editor
            EditorWindow wnd = GetWindow<FreeAnimationsImportWindow>();
            wnd.titleContent = new GUIContent("Free Emotes importer");
        }

        void OnGUI()
        {
            if (!Directory.Exists("Assets/StreamingAssets")) {
                Directory.CreateDirectory("Assets/StreamingAssets");
            }

            if (manifest == null) {
                manifest = FreeAnimationManifest.LoadFromPath(Path.Combine(Application.streamingAssetsPath, KinetixConstants.C_FreeAnimationsManifestPath));
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Import Kinetix base emotes")) {
                ImportKinetixTestEmotes();
            }

            if (GUILayout.Button("Remove Kinetix base emotes")) {
                RemoveKinetixTestEmotes();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Refresh Free emotes manifest")) {
                List<string> emotesNames = new List<string>();

                
                DirectoryInfo freeCustomAnimDir = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, KinetixConstants.C_FreeCustomAnimationsPath));

                if (freeCustomAnimDir.Exists) {
                    foreach (DirectoryInfo emotedir in freeCustomAnimDir.GetDirectories()) {
                        emotesNames.Add(emotedir.Name);
                        
                        if (!manifest.Contains(emotedir.Name, KinetixConstants.C_FreeCustomAnimationsPath)) {
                            manifest.AddEmote(KinetixConstants.C_FreeCustomAnimationsPath, emotedir.Name);
                        }
                    }
                }

                manifest.RemoveWhenNotInList(emotesNames, KinetixConstants.C_FreeCustomAnimationsPath);

                emotesNames.Clear();
                DirectoryInfo freeAnimDir = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, KinetixConstants.C_FreeAnimationsPath) + "/");

                if (freeAnimDir.Exists) {
                    foreach (DirectoryInfo emotedir in freeAnimDir.GetDirectories()) {
                        emotesNames.Add(emotedir.Name);

                        if (!manifest.Contains(emotedir.Name, KinetixConstants.C_FreeAnimationsPath)) {
                            manifest.AddEmote(KinetixConstants.C_FreeAnimationsPath, emotedir.Name);
                        }
                    }
                }

                manifest.RemoveWhenNotInList(emotesNames, KinetixConstants.C_FreeAnimationsPath);

                
                manifest.Save();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        


        private void ImportKinetixTestEmotes()
        {
            RemoveKinetixTestEmotes();

            if (!Directory.Exists("Assets/StreamingAssets/Kinetix")) {
                Directory.CreateDirectory("Assets/StreamingAssets/Kinetix");
            }

            if (!Directory.Exists("Assets/StreamingAssets/Kinetix/FreeAnimations")) {
                Directory.CreateDirectory("Assets/StreamingAssets/Kinetix/FreeAnimations");
            }


            bool success = AssetDatabase.CopyAsset(KinetixConstants.C_FreeAnimationsAssetPluginPath, KinetixConstants.C_FreeAnimationsAssetSAPath);
<<<<<<< HEAD
            
            if (!success) {
                success = true;

                foreach (DirectoryInfo emotedir in new DirectoryInfo(KinetixConstants.C_FreeAnimationsAssetPluginPath).GetDirectories()) {
                    success = success && AssetDatabase.CopyAsset(KinetixConstants.C_FreeAnimationsAssetPluginPath + "/" + emotedir.Name, KinetixConstants.C_FreeAnimationsAssetSAPath + "/" + emotedir.Name);
                }
            }
=======
>>>>>>> 06ddde23de98092f4c3f29a5edcf5fb88da8c702

            DirectoryInfo freeAnimDir = new DirectoryInfo(KinetixConstants.C_FreeAnimationsAssetSAPath);
            DirectoryInfo[] emotesDirectories = freeAnimDir.GetDirectories();

            foreach (DirectoryInfo emotedir in emotesDirectories) {
                manifest.AddEmote(KinetixConstants.C_FreeAnimationsPath, emotedir.Name);
            }

            if (success) {
                Debug.Log("Imported Kinetix Base Free emotes");
            } else {
                Debug.LogWarning("Import Failed");
            }

            manifest.Save();

            AssetDatabase.Refresh();
        }

        private void RemoveKinetixTestEmotes()
        {
            DirectoryInfo freeAnimDir = new DirectoryInfo(KinetixConstants.C_FreeAnimationsAssetSAPath);

            if (!freeAnimDir.Exists) return;

            DirectoryInfo[] emotesDirectories = freeAnimDir.GetDirectories();

            foreach (DirectoryInfo emotedir in emotesDirectories) {
                manifest.RemoveEmote(KinetixConstants.C_FreeAnimationsPath, emotedir.Name);
            }

            bool success = AssetDatabase.DeleteAsset(KinetixConstants.C_FreeAnimationsAssetSAPath);

            Debug.Log("Removed existing Kinetix Base Free emotes");
            
            manifest.Save();
            
            AssetDatabase.Refresh();
        }

        private void RemoveKinetixCustomEmotes()
        {
            DirectoryInfo freeAnimDir = new DirectoryInfo(KinetixConstants.C_FreeCustomAnimationsAssetSAPath);

            if (!freeAnimDir.Exists) return;
            
            DirectoryInfo[] emotesDirectories = freeAnimDir.GetDirectories();

            foreach (DirectoryInfo emotedir in emotesDirectories) {
                manifest.RemoveEmote(KinetixConstants.C_FreeCustomAnimationsPath, emotedir.Name);
            }


            bool success = AssetDatabase.DeleteAsset(KinetixConstants.C_FreeCustomAnimationsAssetSAPath);

            Debug.Log("Removed existing Custom Free emotes");

            manifest.Save();
            
            AssetDatabase.Refresh();
        }


    }
}
<<<<<<< HEAD
>>>>>>> Stashed changes
=======
>>>>>>> 06ddde23de98092f4c3f29a5edcf5fb88da8c702
