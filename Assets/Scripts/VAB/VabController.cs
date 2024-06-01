using System.Collections.Generic;
using Arkship.Parts;
using UnityEngine;
using UnityEngine.UIElements;

public class VabController : MonoBehaviour
{
    [SerializeField] private UIDocument _partPickerDoc;
    [SerializeField] private VisualTreeAsset _categoryTabTemplate;
    [SerializeField] private VisualTreeAsset _partPickerPartTemplate;
    
    void Start()
    {
        PartDictionary.Initialise();

        Dictionary<string, Tab> CategoryTabs = new Dictionary<string, Tab>();

        var categoriesTabView = _partPickerDoc.rootVisualElement.Q<TabView>("Categories");
        
        foreach (var part in PartDictionary.GetParts())
        {
            if (!CategoryTabs.ContainsKey(part.Category))
            {
                var categoryTab = _categoryTabTemplate.Instantiate().Q<Tab>();
                categoryTab.Q<Label>("unity-tab__header-label").text = part.Category;
                categoriesTabView.Add(categoryTab);
                CategoryTabs.Add(part.Category, categoryTab);
            }
            
            var tab = CategoryTabs[part.Category];
            
            var button = _partPickerPartTemplate.Instantiate().Q<Button>();
            button.clicked += () => { OnPartClicked(part); };
            button.text = part.Name;
            button.tooltip = part.Description;
            tab.Add(button);
        }
    }

    private void OnPartClicked(PartDefinition part)
    {
        PartDictionary.SpawnPart(part);
    }
}
