using System;
using System.Collections.Generic;
using System.Linq;
using Kosmos.Prototype.Parts;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kosmos.Prototype.Vab
{
    public class StagingPanel : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDoc;
        private ListView _stageListView;


        private List<PartBase> _stagingEntries = new();
        
        private void Awake()
        {
            _stageListView = _uiDoc.rootVisualElement.Q<ListView>();
            
           
            Func<VisualElement> makeItem = () => new Label();
            Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = _stagingEntries[i].name;
            
            _stageListView.bindItem = bindItem;
            _stageListView.makeItem = makeItem;
            _stageListView.itemsSource = _stagingEntries;
            _stageListView.RefreshItems();
            
            _stageListView.itemIndexChanged += (i, i1) => { Debug.Log("Reordered!"); };
        }

        public void AddPart(PartBase part)
        {
            _stagingEntries.Add(part);
            _stageListView.RefreshItems();
        }

        public void RemovePart(PartBase part)
        {
            _stagingEntries.Remove(part);
            _stageListView.RefreshItems();
        }

        public int GetPartStageIndex(PartBase part)
        {
            return _stagingEntries.IndexOf(part);
        }

        public IReadOnlyList<PartBase> GetStageOrder()
        {
            return _stagingEntries;
        }

        public void SetStagingList(List<PartBase> stagingList)
        {
            _stagingEntries = stagingList;
            _stageListView.RefreshItems();
        }
    }
}