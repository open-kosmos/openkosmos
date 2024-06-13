using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kosmos.UI.Debug
{
    public class DebugTimeUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        private const string TEXT_TIME = "text_time";

        private Label _timeLabel;

        private Entity _entity;
        private EntityManager _entityManager;

        private void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _entity = _entityManager.CreateEntity();
            _entityManager.AddComponentObject(_entity, this);
        }

        private void Start()
        {
            _timeLabel = _uiDocument.rootVisualElement.Q<Label>(TEXT_TIME);
        }
        
        public void SetText(string text)
        {
            _timeLabel.text = text;
        }
    }
}
