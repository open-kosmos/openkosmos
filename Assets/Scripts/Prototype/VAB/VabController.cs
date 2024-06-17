using System;
using System.IO;
using System.Threading.Tasks;
using GLTFast;
using GLTFast.Schema;
using Kosmos.Prototype.Parts;
using Unity.Entities;
using Unity.Entities.Graphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;
using Camera = UnityEngine.Camera;
using Material = UnityEngine.Material;
using Mesh = UnityEngine.Mesh;
using Scene = UnityEngine.SceneManagement.Scene;
using Kosmos.Prototype.Parts.Components;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;

namespace Kosmos.Prototype.Vab
{
    public class VabController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private PartPickerPanel _partPickerPanel;
        [SerializeField] private PartInfoPanel _partInfoPanel;
        [SerializeField] private CameraController _camController;
        
        [Header("Gizmos")]
        [SerializeField] private GizmoBase _moveGizmo;
        
        [Header("Input")]
        [SerializeField] private InputActionReference _mousePosition;
        [SerializeField] private InputActionReference _mouseDelta;
        [SerializeField] private InputActionReference _camFocus;
        [SerializeField] private InputActionReference _click;
        [SerializeField] private InputActionReference _partInfoFocus;
        [SerializeField] private InputActionReference _deletePress;
        [SerializeField] private InputActionReference _savePress;
        [SerializeField] private InputActionReference _loadPress;
        
        private const float SNAP_DIST = 0.02f;
        private PartCollection _vehicleRoot;
        private PartBase _movingPart;
        private float _movingPartInitialDist;
        private Camera _mainCam;

        private bool _controllComponentAdded = false;
        private Entity _playerControlledControlPod;
        private Dictionary<int, List<Entity>> _stages = new();

        private enum EControlState
        {
            None,
            MovingPart,
            DraggingGizmo,
        }

        private EControlState _controlState = EControlState.None;
        private GizmoBase _currentGizmo; 

        void Start()
        {
            _partPickerPanel.OnPartPicked += OnPartPickerClicked;
            _partPickerPanel.OnLaunchButtonClicked += async () => await OnLaunchButtonClicked();

            _camFocus.action.performed += OnCamFocusClick;
            _click.action.performed += OnScreenClick;
            _partInfoFocus.action.performed += OnPartInfoFocusClick;
            _deletePress.action.performed += OnDelete;
            _loadPress.action.performed += OnLoadPressed;
            _savePress.action.performed += OnSavePressed;

            //Temp disable gizmos
            //_currentGizmo = _moveGizmo;
            _moveGizmo.gameObject.SetActive(false);

            _vehicleRoot = new GameObject("VehicleRoot").AddComponent<PartCollection>();
            _mainCam = _camController.GetComponent<Camera>();
            
        }

        private void OnCamFocusClick(InputAction.CallbackContext context)
        {
            var clicked = GetPartUnderCursor();

            if (clicked != null)
            {
                _camController.SetCameraTarget(clicked.transform.position);
            }
        }

        private async Awaitable OnLaunchButtonClicked()
        {
            string flightControlScenceName = "Prototype_FlightControl";
            Scene currentScene = SceneManager.GetActiveScene();
            await SceneManager.LoadSceneAsync(flightControlScenceName, LoadSceneMode.Additive);

            //SceneManager.MoveGameObjectToScene(_vehicleRoot.gameObject, SceneManager.GetSceneByName(flightControlScenceName));

            await ConstructPlayableVehicle();

            await SceneManager.UnloadSceneAsync(currentScene);
        }
        private async Task ConstructPlayableVehicle()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var parts = _vehicleRoot.AllParts;


            foreach (var part in parts)
            {
                var entity = entityManager.CreateEntity();
                SetStage(part, entity);
                AddComponentsToEntity(part, entity, entityManager);                
                var modelPath = Path.Combine(Application.streamingAssetsPath, "Parts", $"{part.PartId}.glb");
                await CreateMeshForEntity(entity, entityManager, modelPath, part);
            }

            AddStageBuffer(parts, entityManager);
        }

        private void SetStage(PartBase part, Entity entity)
        {

            if (part is StageablePart)
            {
                var stageIndex = (part as StageablePart).GetStageIndex();
                if (!_stages.ContainsKey(stageIndex))
                {
                    _stages.Add(stageIndex, new());
                }

                _stages[stageIndex].Add(entity);
            }
        }

        private void AddStageBuffer(HashSet<PartBase> parts, EntityManager entityManager)
        {
            if (_playerControlledControlPod != Entity.Null)
            {
                var stagesBuffer = entityManager.AddBuffer<Stage>(_playerControlledControlPod);
                foreach (var stageParts in _stages.OrderBy(s => s.Key))
                {
                    NativeArray<StagePart> stage = new NativeArray<StagePart>(stageParts.Value.Count, Allocator.Persistent);
                    for (int i = 0; i < stageParts.Value.Count; i++)
                    {
                        stage[i] = new StagePart { Value = stageParts.Value[i] };
                    }

                    stagesBuffer.Add(new Stage { Parts = stage });
                }
            }
        }

        private void AddComponentsToEntity(PartBase part, Entity entity, EntityManager entityManager)
        {
            //TODO: Probably a better way to add components to certain parts.
            switch (part.PartId)
            {
                case "Engine_M":
                case "Engine_S":
                    entityManager.AddComponentData(entity, new Engine { MaxThrust = (part as RocketPart).GetMaxThrust() });
                    break;
                case "Capsule_M":
                case "Capsule_S":
                    var controlPod = new ControlPod { CurrentStageIndex = 0 };
                    entityManager.AddComponentData(entity, controlPod);

                    if (!_controllComponentAdded)
                    {
                        _playerControlledControlPod = entity;
                        entityManager.AddComponentData(entity, new PlayerControlledTag());
                        _controllComponentAdded = true;
                    }
                    break;
            }
        }

        private async Task CreateMeshForEntity(Entity entity, EntityManager entityManager, string modelPath, PartBase part)
        {
            var gltf = new GltfImport();
            var importSettings = new ImportSettings()
            {
                AnimationMethod = AnimationMethod.Mecanim,
                AnisotropicFilterLevel = 16,
                DefaultMagFilterMode = Sampler.MagFilterMode.Nearest,
                DefaultMinFilterMode = Sampler.MinFilterMode.Nearest,
                GenerateMipMaps = true,
                NodeNameMethod = NameImportMethod.Original
            };
            var bytes = await File.ReadAllBytesAsync(modelPath);
            await gltf.LoadGltfBinary(bytes, new Uri(modelPath), importSettings);

            var meshes = gltf.GetMeshes();
            var meshRefs = new UnityObjectRef<Mesh>[meshes.Length];
            for (int i = 0; i < meshes.Length; i++)
            {
                meshRefs[i] = new UnityObjectRef<Mesh>
                {
                    Value = meshes[i]
                };
            }

            var materials = gltf.GetMaterial();
            var materialRefs = new UnityObjectRef<Material>[1];
            materialRefs[0] = new UnityObjectRef<Material>
            {
                Value = materials
            };

            var materialMeshIndices = new MaterialMeshIndex[1]
            {
                    new MaterialMeshIndex()
            };

            var renderMeshDescription = new RenderMeshDescription()
            {
                FilterSettings = RenderFilterSettings.Default,
                LightProbeUsage = LightProbeUsage.Off
            };

            var renderMeshArray = new RenderMeshArray(
                new ReadOnlySpan<UnityObjectRef<Material>>(materialRefs),
                new ReadOnlySpan<UnityObjectRef<Mesh>>(meshRefs),
                new ReadOnlySpan<MaterialMeshIndex>(materialMeshIndices)
                );

            RenderMeshUtility.AddComponents(
                entity,
                entityManager,
                renderMeshDescription,
                renderMeshArray,
                MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

            entityManager.AddComponentData(entity, new LocalTransform()
            {
                Position = part.transform.position,
                Scale = 1,
            });
        }

        private void OnPartPickerClicked(PartDefinition part)
        {
            var newPart = _vehicleRoot.AddPart(part);
            _movingPart = newPart;
            _controlState = EControlState.MovingPart;

            Vector3 mousePos = _mousePosition.action.ReadValue<Vector2>();
            mousePos.z = _camController.GetCamDistance();
            _movingPartInitialDist = mousePos.z;
            
            newPart.transform.position = _mainCam.ScreenToWorldPoint(mousePos);
        }

        private void OnPartInfoFocusClick(InputAction.CallbackContext context)
        {
            var part = GetPartUnderCursor();
            if (part != null)
            {
                _partInfoPanel.SetPart(part);
            }
        }

        private void OnScreenClick(InputAction.CallbackContext context)
        {
            //Part selection
            if (_controlState == EControlState.MovingPart)
            {
                Debug.Assert(_movingPart != null);
                
                //Stop dragging
                //See if we should snap to something
                if (_vehicleRoot.GetClosestPotentialConnection(_movingPart, out var partLink, _mainCam, _mainCam.pixelWidth * SNAP_DIST))
                {
                    _vehicleRoot.LinkParts(partLink._parentPart, partLink._parentSocket, _movingPart, partLink._childSocket);
                }

                _movingPart = null;
                _controlState = EControlState.None;
            }
            else if (_controlState == EControlState.None)
            {
                var part = GetPartUnderCursor();

                if (part != null)
                {
                    _controlState = EControlState.MovingPart;
                    _movingPart = part;
                    _movingPartInitialDist =
                        Vector3.Distance(_mainCam.transform.position, _movingPart.transform.position); 
                    _vehicleRoot.DisconnectFromParents(_movingPart);
                }
            }
        }

        public PartBase GetPartUnderCursor()
        {
            Vector2 mousePos = _mousePosition.action.ReadValue<Vector2>();
            var ray = _mainCam.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var part = hit.collider.GetComponentInParent<PartBase>();
                if (part != null)
                {
                    return part;
                }
            }

            return null;
        }

        public void OnScreenDrag(InputAction.CallbackContext context)
        {
            //Gizmo handling
            // var mousePos = _mousePosition.action.ReadValue<Vector2>();
            // if (context.phase == InputActionPhase.Started && _controlState == EControlState.None)
            // {
            //     if (_currentGizmo.TestClick(mousePos, _mainCam))
            //     {
            //         _controlState = EControlState.DraggingGizmo;
            //         _currentGizmo.StartDrag(mousePos, _mainCam);
            //     }
            // }
            // else if (context.phase == InputActionPhase.Canceled && _controlState == EControlState.DraggingGizmo)
            // {
            //     _currentGizmo.EndDrag(mousePos, _mainCam);
            //     _controlState = EControlState.None;
            // }
        }
        
        private void OnDelete(InputAction.CallbackContext context)
        {
            if (_movingPart != null)
            {
                _vehicleRoot.RemovePart(_movingPart);
                _controlState = EControlState.None;
            }
        }
        
        public void Update()
        {
            switch (_controlState)
            {
                case EControlState.None:
                    break;
                // case EControlState.DraggingGizmo:
                //     _currentGizmo.UpdateDrag(_mousePosition.action.ReadValue<Vector2>(), _mouseDelta.action.ReadValue<Vector2>(), _mainCam);
                //     break;
                case EControlState.MovingPart:
                    UpdateMovingPart();
                    break;
            }
        }

        private void UpdateMovingPart()
        {
            var mousePos = _mousePosition.action.ReadValue<Vector2>();
                
            Vector3 newPos = _mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _movingPartInitialDist));
            
            Vector3 mouseMoveWorld = newPos - _movingPart.transform.position;
                
            _vehicleRoot.MovePart(_movingPart, mouseMoveWorld);

            //This is a bit clumsy. Rewrite it at some point
            
            if (_vehicleRoot.GetClosestPotentialConnection(_movingPart, out var partLink, _mainCam,
                    _mainCam.pixelWidth * SNAP_DIST))
            {
                Vector3 offset = partLink._parentSocket.transform.position - partLink._childSocket.transform.position;
                _vehicleRoot.MovePart(_movingPart, offset);
            }
        }
        
        private string GetSaveFolder()
        {
#if UNITY_EDITOR
            return Path.Combine(Application.streamingAssetsPath, "Vehicles");
#else
            return Path.Combine(Application.persistentDataPath, "Vehicles");
#endif
        }

        private void OnSavePressed(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
            {
                return;
            }
            
            string fileName = context.control.displayName;
            string folder = GetSaveFolder();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            
            string path = Path.Combine(GetSaveFolder(), $"{fileName}.veh");
            _vehicleRoot.Serialise(path);
        }

        private void OnLoadPressed(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
            {
                return;
            }
            
            string fileName = context.control.displayName;
            string path = Path.Combine(GetSaveFolder(), $"{fileName}.veh");
            if (!File.Exists(path))
            {
                Debug.Log($"Can't load {path} - file doesn't exist");
                return;
            }
            Debug.Log($"Loading from {path}");
            _vehicleRoot.Deserialise(path);

            _movingPart = null;
            _controlState = EControlState.None;
        }
    }
}