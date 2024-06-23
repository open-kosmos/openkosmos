using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Kosmos.Prototype.Parts.TraitComponents;
using Mono.Cecil;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using File = UnityEngine.Windows.File;

namespace Kosmos.Prototype.Parts.Serialization
{
    //Data structures that define the structure of a part in the game
    
    //TODO! Collision shapes are currently not serialized
    
    [System.Serializable]
    public class TransformData
    {
        public string Name;
        public Quaternion LocalRotation;
        public Vector3 LocalPosition;
        public Vector3 LocalScale;
        public int ParentTransform;
    }

    [System.Serializable]
    public class TraitData
    {
        public string Type;
        public string JsonString;
    }

    [System.Serializable]
    public class SocketData
    {
        public int TransformIndex;
    }

    [System.Serializable]
    public class ModelData
    {
        public string ModelPath;
        public List<RendererData> Instances;
    }

    [System.Serializable]
    public class RendererData
    {
        public string MeshName;
        public int TransformIndex;
    }
    
    [System.Serializable]
    public class PartPrefabData
    {
        public List<TransformData> Transforms;
        public List<TraitData> Traits;
        public List<SocketData> Sockets;
        public List<ModelData> Models;
        
#if UNITY_EDITOR
        //TODO!
        //Should be an editor panel for creating, loading and saving parts,
        //which makes it easy to create a part definition with a new GUID
        
        [MenuItem("Parts/ExportPart")]
        public static void ExportPart()
        {
            TraitDictionary.Initialize();
            
            string defPath = EditorUtility.OpenFilePanel("Select part definition", "Assets\\Resources\\Parts\\Definitions", "json");

            PartDefinition part = PartDefinition.Load(System.IO.File.ReadAllText(defPath));
            
            string path = System.IO.Path.Combine("Assets/Resources", part.Path + ".txt");
            
            var selected = Selection.activeObject;

            if (selected is GameObject partRoot)
            {
                PartPrefabData data = new();
                data.Transforms = new();

                Dictionary<Transform, int> transDict = new();
                List<Transform> objectTransList = partRoot.GetComponentsInChildren<Transform>().ToList();
                objectTransList.Remove(partRoot.transform);
                for (int i = 0; i < objectTransList.Count; i++)
                {
                    transDict.Add(objectTransList[i], i);
                }

                //Build transform list
                foreach (var origTrans in objectTransList)
                {
                    TransformData partTrans = new()
                    {
                        LocalPosition = origTrans.position,
                        LocalRotation = origTrans.localRotation,
                        LocalScale = origTrans.localScale,
                        Name = origTrans.name,
                        ParentTransform = GetTransformIndex(origTrans.parent, transDict, partRoot.transform)
                    };

                    data.Transforms.Add(partTrans);
                }

                //Build trait list
                var traits = partRoot.GetComponents<TraitMonoBase>();
                data.Traits = new();
                foreach (var trait in traits)
                {
                    string traitTypeName = trait.GetType().FullName;
                    TraitData traitData = new()
                    {
                        Type = traitTypeName,
                        JsonString = TraitDictionary.GetFactoryForTrait(traitTypeName)
                            .SerializeTrait(trait)
                    };
                    data.Traits.Add(traitData);
                }

                //Build socket list
                data.Sockets = new();
                var sockets = partRoot.GetComponentsInChildren<PartSocket>();
                foreach (var socket in sockets)
                {
                    var socketData = new SocketData();
                    socketData.TransformIndex = GetTransformIndex(socket.transform, transDict, partRoot.transform);
                    data.Sockets.Add(socketData);
                }
                
                //Build renderer list
                data.Models = new();
                var renderers = partRoot.GetComponentsInChildren<UnityEngine.MeshRenderer>();
                foreach (var rend in renderers)
                {
                    RendererData rendererData = new();
                    var mesh = rend.GetComponent<MeshFilter>().sharedMesh;
                    
                    rendererData.MeshName = mesh.name;
                    rendererData.TransformIndex = GetTransformIndex(rend.transform, transDict, partRoot.transform);

                    string meshPath = System.IO.Path.GetRelativePath("Assets/Resources", AssetDatabase.GetAssetPath(mesh));
                    ModelData parentModel = null;
                    foreach (var modelData in data.Models)
                    {
                        if (modelData.ModelPath == meshPath)
                        {
                            parentModel = modelData;
                            break;
                        }
                    }

                    if (parentModel == null)
                    {
                        parentModel = new();
                        parentModel.ModelPath = meshPath;
                        parentModel.Instances = new();
                        data.Models.Add(parentModel);
                    }
                    
                    parentModel.Instances.Add(rendererData);
                }
                
                Debug.Log(JsonUtility.ToJson(data, true));
                System.IO.File.WriteAllText(path, JsonUtility.ToJson(data));
            }
        }
        
        [MenuItem("Parts/Import Part")]
        public static void ImportPart()
        {
            string defPath = EditorUtility.OpenFilePanel("Select part definition", "Assets\\Resources\\Parts\\Definitions", "json");

            PartDefinition partDef = PartDefinition.Load(System.IO.File.ReadAllText(defPath));
            
            string path = System.IO.Path.Combine("Assets/Resources", partDef.Path + ".txt");
            
            string textPart = System.IO.File.ReadAllText(path);
            PartPrefabData data = JsonUtility.FromJson<PartPrefabData>(textPart);
            
            var part = data.CreateGoPart();
            part.name = System.IO.Path.GetFileNameWithoutExtension(defPath);
        }
#endif
        
        public GameObject CreateGoPart()
        {
            var root = new GameObject("NewPart");
            Dictionary<int, Transform> transDict = new();
            transDict[-1] = root.transform;

            //Two passes so we're not assuming anything about the order
            for (var transIdx = 0; transIdx < Transforms.Count; transIdx++)
            {
                var transData = Transforms[transIdx];
                var trans = new GameObject(transData.Name).transform;
                transDict[transIdx] = trans;
            }

            for (var transIdx = 0; transIdx < Transforms.Count; transIdx++)
            {
                var transData = Transforms[transIdx];
                var trans = transDict[transIdx];
                trans.SetParent(transDict[transData.ParentTransform]);
                trans.localRotation = transData.LocalRotation;
                trans.localPosition = transData.LocalPosition;
                trans.localScale = transData.LocalScale;
            }
            
            //Add Traits
            foreach (var traitData in Traits)
            {
                TraitDictionary.GetFactoryForTrait(traitData.Type).DeserializeGo(traitData.JsonString, root);
            }
            
            //Add Sockets
            foreach (var socketData in Sockets)
            {
                transDict[socketData.TransformIndex].gameObject.AddComponent<PartSocket>();
            }
            
            //Add renderers
            foreach (var modelData in Models)
            {
                foreach (var instance in modelData.Instances)
                {
                    MeshRenderer templateMeshRend = GetMeshReference(modelData.ModelPath, instance.MeshName);
                    MeshFilter templateFilter = templateMeshRend.GetComponent<MeshFilter>();

                    var rendGo = transDict[instance.TransformIndex].gameObject;
                    var filter = rendGo.AddComponent<MeshFilter>();
                    var rend = rendGo.AddComponent<MeshRenderer>();
                    filter.sharedMesh = templateFilter.sharedMesh;
                    rend.sharedMaterials = templateMeshRend.sharedMaterials;
                }
            }

            return root;
        }

        private static MeshRenderer GetMeshReference(string modelPath, string meshName)
        {
            string resourcePath = Path.ChangeExtension(modelPath, null);
            var meshResource = Resources.Load(resourcePath);
            var rends = (meshResource as GameObject).GetComponentsInChildren<MeshRenderer>();
            return rends.First((x) => x.GetComponent<MeshFilter>().sharedMesh.name == meshName);
        }

        private static int GetTransformIndex(Transform trans, Dictionary<Transform, int> transDict, Transform rootTrans)
        {
            if (trans == rootTrans)
            {
                return -1;
            }

            return transDict[trans];
        }
    }
}