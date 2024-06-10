using Kosmos.Prototype.Parts;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kosmos.Prototype.Vab
{
    public class PartInfoPanel : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDoc;
        
        private Label _partNameLabel;
        private Label _partDescriptionLabel;
        private Foldout _tweakablesFoldout;
        
        private void Awake()
        {
            _partNameLabel = _uiDoc.rootVisualElement.Q<Label>("PartName");
            _partDescriptionLabel = _uiDoc.rootVisualElement.Q<Label>("PartDescription");
            _tweakablesFoldout = _uiDoc.rootVisualElement.Q<Foldout>("Tweakables");
        }

        public void SetPart(PartBase part)
        {
            _tweakablesFoldout.Clear();
            
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
                
                foreach (var field in PartDictionary.GetPartTweakableFields(part))
                {
                    if (field.FieldType == typeof(float))
                    {
                        Slider slider = new Slider(field.Name, 0.0f, 1.0f);
                        slider.value = (float)field.GetValue(part);
                        slider.RegisterValueChangedCallback((evt) =>
                        {
                            field.SetValue(part, evt.newValue);
                        });
                        
                        _tweakablesFoldout.Add(slider);
                    }
                    else
                    {
                        _tweakablesFoldout.Add(new Label(field.Name + " (unsupported type)"));
                    }
                }
            }
        }
    }
}
