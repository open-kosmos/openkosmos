using System;
using System.IO;
using Kosmos.Prototype.Parts;
using Kosmos.Prototype.Parts.Serialization;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Prototype.Parts.Editor
{
    public class PartEditorWindow : EditorWindow
    {
        private PartDefinition _partDef;
        private string _partDefPath;
        
        [MenuItem("Kosmos/Parts/Part Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<PartEditorWindow>();
            window.titleContent = new GUIContent("Part Editor");
            window.Show();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Create New"))
                {
                    CreateNewPart();
                }

                if (GUILayout.Button("Load"))
                {
                    LoadPart();
                }

                GUI.enabled = _partDef != null;
                if (GUILayout.Button("Save"))
                {
                    SavePart();
                }
                GUI.enabled = true;
            }

            if (_partDef != null)
            {
                DisplayPartDef();
            }
        }

        private void SavePart()
        {
            //Check there's only one GO in the scene root
            var roots = EditorSceneManager.GetActiveScene().GetRootGameObjects();
            if (roots.Length != 1)
            {
                Debug.LogError("Please ensure that there's exactly one root object");
                return;
            }
            
            PartDefinition.Save(_partDef, _partDefPath);
            PartPrefabData.ExportPart(_partDef, roots[0]);
        }

        private void LoadPart()
        {
            string defPath = EditorUtility.OpenFilePanel("Select part definition", "Assets\\Resources\\Parts\\Definitions", "json");

            if (!string.IsNullOrEmpty(defPath))
            {
                _partDef = PartDefinition.Load(System.IO.File.ReadAllText(defPath));
                _partDefPath = defPath;

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                    string path = System.IO.Path.Combine("Assets/Resources", _partDef.Path + ".txt");
            
                    string textPart = System.IO.File.ReadAllText(path);
                    PartPrefabData data = JsonUtility.FromJson<PartPrefabData>(textPart);
            
                    var part = data.CreateGoPart();
                    part.name = System.IO.Path.GetFileNameWithoutExtension(defPath);
                }
            }
        }

        private void CreateNewPart()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            string filePath = EditorUtility.SaveFilePanel("Select part definition", 
                "Assets\\Resources\\Parts\\Definitions", "",
                "json");

            if (!string.IsNullOrEmpty(filePath))
            {
                string partName = Path.GetFileNameWithoutExtension(filePath);
                _partDef = new();
                _partDef.Guid = Guid.NewGuid().ToString();
                _partDef.Name = partName;
                _partDef.Path = "Parts/Stock/" + partName;
                _partDefPath = filePath;
                
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            }
        }

        private void DisplayPartDef()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                GUI.enabled = false;
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Name");
                    GUILayout.TextField(_partDef.Name);
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Guid");
                    GUILayout.TextField(_partDef.Guid);
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Prefab Path");
                    GUILayout.TextField(_partDef.Path);
                }
                
                GUI.enabled = true;
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Category");
                    _partDef.Category = GUILayout.TextField(_partDef.Category);
                }
                
                GUILayout.Label("Description");
                _partDef.Description = GUILayout.TextArea(_partDef.Description);
            }
        }
    }
}