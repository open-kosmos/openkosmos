using Arkship.Parts;
using UnityEngine;
using UnityEngine.UIElements;

namespace Arkship.Vab
{
    public class PartInfoPanel : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDoc;
        
        private Label _partNameLabel;
        private Label _partDescriptionLabel;
        
        private void Start()
        {
            _partNameLabel = _uiDoc.rootVisualElement.Q<Label>("PartName");
            _partDescriptionLabel = _uiDoc.rootVisualElement.Q<Label>("PartDescription");
        }

        public void SetPart(PartBase part)
        {
            //TODO - Would be nice to turn the panel off if there's nothing selected,
            //but that completely breaks the UI:
            //https://forum.unity.com/threads/label-text-does-not-change-when-text-parameter-is-modified-by-a-script.985582/
            if (part == null)
            {
                _partNameLabel.text = "None";
                _partDescriptionLabel.text = "None";
            }
            else
            {
                _partNameLabel.text = part.GetDefinition().Name;
                _partDescriptionLabel.text = part.GetDefinition().Description;
            }
        }
    }
}
