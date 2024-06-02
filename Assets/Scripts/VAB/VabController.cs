using System.Collections.Generic;
using Arkship.Parts;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArkShip.Vab
{
    public class VabController : MonoBehaviour
    {
        [SerializeField] private PartPickerPanel _partPickerPanel;
        
        private GameObject Vehicleroot;

        void Start()
        {
            _partPickerPanel.OnPartPicked += OnPartPickerClicked;

            Vehicleroot = new GameObject("VehicleRoot");
        }

        private void OnPartPickerClicked(PartDefinition part)
        {
            var newPart = PartDictionary.SpawnPart(part);
            newPart.transform.SetParent(Vehicleroot.transform);
        }
    }
}