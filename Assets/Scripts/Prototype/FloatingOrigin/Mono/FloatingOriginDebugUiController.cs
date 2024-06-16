using System;
using Kosmos.FloatingOrigin;
using Kosmos.Prototype.Character;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Prototype.FloatingOrigin.Mono
{
    public class FloatingOriginDebugUiController : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        
        private EntityManager _entityManager;
        private Entity _entity;
        
        private Label _speedLabel;
        private Label _scaleLabel;
        
        private void Start()
        {
            _speedLabel = _uiDocument.rootVisualElement.Q<Label>("text_speed");
            _scaleLabel = _uiDocument.rootVisualElement.Q<Label>("text_scale");
            
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _entity = _entityManager.CreateEntity();
            _entityManager.AddComponentObject(_entity, this);
        }

        private void Update()
        {
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

        public void SetSpeedText(float speed)
        {
            _speedLabel.text = "Speed: " + speed.ToString("N1");
        }
        
        public void SetScaleText(float scale)
        {
            _scaleLabel.text = "Scale: " + scale.ToString("N1");
        }
    }
}