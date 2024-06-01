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
            if (!CategoryTabs.ContainsKey(part.GetCategory()))
            {
                var categoryTab = _categoryTabTemplate.Instantiate().Q<Tab>();
                categoryTab.Q<Label>("unity-tab__header-label").text = part.GetCategory();
                categoriesTabView.Add(categoryTab);
                CategoryTabs.Add(part.GetCategory(), categoryTab);
            }
            
            var tab = CategoryTabs[part.GetCategory()];
            
            var button = _partPickerPartTemplate.Instantiate().Q<Button>();
            button.clicked += () => { OnPartClicked(part); };
            button.text = part.GetName();
            tab.Add(button);
        }
    }

    private void OnPartClicked(PartBase part)
    {
        GameObject.Instantiate(part);
    }
}
