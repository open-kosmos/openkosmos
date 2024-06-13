using System.Collections.Generic;
using Kosmos.Prototype.Parts;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kosmos.Prototype.Vab
{
    public class PartPickerPanel : MonoBehaviour
    {
        [SerializeField] private UIDocument _partPickerDoc;
        [SerializeField] private VisualTreeAsset _categoryTabTemplate;
        [SerializeField] private VisualTreeAsset _partPickerPartTemplate;

        public event System.Action<PartDefinition> OnPartPicked;
        public event System.Action OnLaunchButtonClicked;

        void Start()
        {
            PartDictionary.Initialise();

            Dictionary<string, Tab> categoryTabs = new Dictionary<string, Tab>();

            var categoriesTabView = _partPickerDoc.rootVisualElement.Q<TabView>("Categories");

            foreach (var part in PartDictionary.GetParts())
            {
                Tab tab;
                if (!categoryTabs.ContainsKey(part.Category))
                {
                    var categoryTab = _categoryTabTemplate.Instantiate().Q<Tab>();
                    categoryTab.Q<Label>("unity-tab__header-label").text = part.Category;
                    categoriesTabView.Add(categoryTab);
                    categoryTabs.Add(part.Category, categoryTab);
                    tab = categoryTab;
                }
                else
                {
                    tab = categoryTabs[part.Category];
                }

                var button = _partPickerPartTemplate.Instantiate().Q<Button>();
                button.clicked += () => { OnPartPicked(part); };
                button.text = part.Name;
                button.tooltip = part.Description;
                tab.Add(button);
            }
            
            var launchButton = _partPickerPartTemplate.Instantiate().Q<Button>();
            launchButton.clicked += () => { OnLaunchButtonClicked(); };
            launchButton.text = "Launch";
            launchButton.tooltip = "Launch ship into control prototype scene.";
            categoriesTabView.Add(launchButton);

        }

    }
}
