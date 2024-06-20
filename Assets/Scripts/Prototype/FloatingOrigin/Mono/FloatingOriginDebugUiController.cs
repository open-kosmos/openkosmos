using System;
using Kosmos.FloatingOrigin;
using Kosmos.Prototype.Character;
using Kosmos.Prototype.OrbitalPhysics;
using Prototype.OrbitalPhysics.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

namespace Prototype.FloatingOrigin.Mono
{
    public class FloatingOriginDebugUiController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private UIDocument _uiDocument;
        
        private EntityManager _entityManager;
        private Entity _entity;
        
        private Label _speedLabel;
        private Label _scaleLabel;
        private Label _fpsLabel;
        private Button _affixButton;
        private Button _unaffixButton;
        private Button _spawnSphereButton;

        private NativeArray<Entity> _bodyEntities;
        private Label[] _namePlates;
        
        private bool _initialized;
        
        private const string NUMBER_FORMAT = "N1";
        
        private void Start()
        {
            _speedLabel = _uiDocument.rootVisualElement.Q<Label>("text_speed");
            _scaleLabel = _uiDocument.rootVisualElement.Q<Label>("text_scale");
            _fpsLabel = _uiDocument.rootVisualElement.Q<Label>("text_fps");
            _affixButton = _uiDocument.rootVisualElement.Q<Button>("btn_affix");
            _unaffixButton = _uiDocument.rootVisualElement.Q<Button>("btn_unaffix");
            _spawnSphereButton = _uiDocument.rootVisualElement.Q<Button>("btn_spawnsphere");
            
            _affixButton.clicked += OnAffixClicked;
            _unaffixButton.clicked += OnUnaffixClicked;
            _spawnSphereButton.clicked += OnSpawnSphereClicked;
            
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _entity = _entityManager.CreateEntity();
            _entityManager.AddComponentObject(_entity, this);
        }

        private void OnDestroy()
        {
            _affixButton.clicked -= OnAffixClicked;
            _unaffixButton.clicked -= OnUnaffixClicked;
            _spawnSphereButton.clicked -= OnSpawnSphereClicked;
        }

        private void Initialize()
        {
            var bodyEntityQuery = _entityManager.CreateEntityQuery(typeof(BodyId));
            var entityCount = bodyEntityQuery.CalculateEntityCount();
            
            if (entityCount == 0)
            {
                return;
            }
            
            _initialized = true;
            
            _bodyEntities = bodyEntityQuery.ToEntityArray(Allocator.Persistent);
            _namePlates = new Label[_bodyEntities.Length];

            for (int i = 0; i < _bodyEntities.Length; i++)
            {
                var label = new Label
                {
                    usageHints = UsageHints.DynamicTransform,
                    style =
                    {
                        color = Color.white,
                        position = Position.Absolute,
                        alignItems = Align.Center,
                        justifyContent = Justify.Center,
                        unityTextAlign = TextAnchor.MiddleCenter
                    },
                    text = _entityManager.GetComponentData<BodyId>(_bodyEntities[i]).Value.ToString()
                };

                _uiDocument.rootVisualElement.Add(label);
                
                _namePlates[i] = label;
            }
        }
        
        private void Update()
        {
            _fpsLabel.text = "FPS: " + (1.0f / Time.deltaTime).ToString(NUMBER_FORMAT);
            
            if (!_initialized)
            {
                Initialize();
                return;
            }
            
            UpdateLabels();
            
            var query = _entityManager.CreateEntityQuery(typeof(PlayableCharacterData));
            var scaleQuery = _entityManager.CreateEntityQuery(typeof(FloatingOriginData));

            var count = query.CalculateEntityCount();
            var scaleCount = scaleQuery.CalculateEntityCount();

            if (count == 0)
            {
                return;
            }
            
            var playerCharacterData = query.GetSingleton<PlayableCharacterData>();
            SetSpeedText(playerCharacterData.MoveSpeed);
            
            if (scaleCount == 0)
            {
                return;
            }
            
            var floatingOriginData = scaleQuery.GetSingleton<FloatingOriginData>();
            SetScaleText((float)floatingOriginData.Scale);
        }

        private void UpdateLabels()
        {
            for (int i = 0; i < _namePlates.Length; i++)
            {
                var entity = _bodyEntities[i];
                var entityTransform = _entityManager.GetComponentData<LocalTransform>(entity);
                
                var vecCameraTransform =  entityTransform.Position - (float3)_camera.transform.position;
                var cameraForward = _camera.transform.forward;
                
                if (math.dot(vecCameraTransform, cameraForward) < 0)
                {
                    _namePlates[i].style.display = DisplayStyle.None;
                    continue;
                }
                
                _namePlates[i].style.display = DisplayStyle.Flex;

                var screenPos = RuntimePanelUtils.CameraTransformWorldToPanel(
                    _uiDocument.rootVisualElement.panel,
                    entityTransform.Position,
                    _camera);
                
                _namePlates[i].style.translate = new StyleTranslate(new Translate(screenPos.x, screenPos.y));
            }
        }

        public void SetSpeedText(float speed)
        {
            _speedLabel.text = "Speed: " + speed.ToString("N1");
        }
        
        public void SetScaleText(float scale)
        {
            _scaleLabel.text = "Scale: " + scale.ToString("N1");
        }
        
        private void OnAffixClicked()
        {
            var floatingFocusQuery = _entityManager.CreateEntityQuery(typeof(FloatingFocusTag));
            var nameQuery = _entityManager.CreateEntityQuery(typeof(BodyId));
            
            var count = floatingFocusQuery.CalculateEntityCount();
            var nameCount = nameQuery.CalculateEntityCount();
            
            if (count == 0 || nameCount == 0)
            {
                Debug.Log("No focus entity or Proterra entity found.");
                return;
            }
            
            var focusEntity = floatingFocusQuery.GetSingletonEntity();
            var proterraEntity = Entity.Null;
            var nameQueryArray = nameQuery.ToEntityArray(Allocator.Temp);

            foreach (var entity in nameQueryArray)
            {
                var bodyId = _entityManager.GetComponentData<BodyId>(entity).Value;
                if (bodyId == "proterra")
                {
                    Debug.Log("Proterra entity found.");
                    proterraEntity = entity;
                    break;
                }
            }
            
            if (proterraEntity == Entity.Null)
            {
                Debug.Log("Proterra entity not found.");
                return;
            }

            var floatingFocusParent = new FloatingFocusParent()
            {
                ParentEntity = proterraEntity
            };
            
            if (_entityManager.HasComponent<FloatingFocusParent>(focusEntity))
            {
                _entityManager.SetComponentData(focusEntity, floatingFocusParent);
                return;
            }
            
            _entityManager.AddComponentData(focusEntity, floatingFocusParent);
            
            nameQueryArray.Dispose();
            
            Debug.Log("Affixed focus entity to Proterra.");
        }
        
        private void OnUnaffixClicked()
        {
            var floatingFocusQuery = _entityManager.CreateEntityQuery(typeof(FloatingFocusTag));
            var count = floatingFocusQuery.CalculateEntityCount();
            
            if (count == 0)
            {
                return;
            }
            
            var focusEntity = floatingFocusQuery.GetSingletonEntity();
            
            if (!_entityManager.HasComponent<FloatingFocusParent>(focusEntity))
            {
                return;
            }
            
            _entityManager.RemoveComponent<FloatingFocusParent>(focusEntity);
        }

        private void OnSpawnSphereClicked()
        {
            var currentFocusQuery = _entityManager.CreateEntityQuery(typeof(FloatingFocusTag));
            var count = currentFocusQuery.CalculateEntityCount();
            
            if (count == 0)
            {
                return;
            }
            
            var proterraQuery = _entityManager.CreateEntityQuery(typeof(BodyId));
            var proterraCount = proterraQuery.CalculateEntityCount();
            
            if (proterraCount == 0)
            {
                return;
            }

            var prefabQuery = _entityManager.CreateEntityQuery(typeof(BuildingPrefab));
            var prefabCount = prefabQuery.CalculateEntityCount();
            
            if (prefabCount == 0)
            {
                return;
            }
            
            var prefabSpawner = prefabQuery.GetSingletonEntity();
            var prefabEntity = _entityManager.GetComponentData<BuildingPrefab>(prefabSpawner).Prefab;
            
            var focusEntity = currentFocusQuery.GetSingletonEntity();
            var focusFloatingPosition = _entityManager.GetComponentData<FloatingPositionData>(focusEntity);
            
            var proterraEntity = Entity.Null;
            var proterraQueryArray = proterraQuery.ToEntityArray(Allocator.Temp);
            
            foreach (var entity in proterraQueryArray)
            {
                var bodyId = _entityManager.GetComponentData<BodyId>(entity).Value;
                if (bodyId == "proterra")
                {
                    proterraEntity = entity;
                    break;
                }
            }
            
            proterraQueryArray.Dispose();
            
            var prefabInstance = _entityManager.Instantiate(prefabEntity);
            
            _entityManager.SetComponentData(prefabInstance, new LocalTransform()
            {
                Scale = 1
            });
            
            var floatingScale = new FloatingScaleData()
            {
                Value = 1f
            };
            
            _entityManager.AddComponentData(prefabInstance, floatingScale);
            
            _entityManager.AddComponentData(prefabInstance, focusFloatingPosition);
            _entityManager.AddComponentData(prefabInstance, new FloatingFocusParent()
            {
                ParentEntity = proterraEntity
            });
        }
    }
}
