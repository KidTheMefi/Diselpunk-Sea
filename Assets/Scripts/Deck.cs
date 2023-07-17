using System;
using System.Collections.Generic;
using ModulesScripts.ModulesSetup;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class Deck : MonoBehaviour
    {
        [SerializeField]
        private ShipModulePlace _shipModulePlacePrefab;
        [SerializeField]
        private TextMeshPro _armorText;
        [SerializeField, Range(0, 10)]
        private int _armor;
        [SerializeField]
        private ModuleLocation _moduleLocation;
        [SerializeField, Range(1, 10)]
        private int _baseDurability;
        [SerializeField, Range(1, 10)]
        private int _baseCrew;

        [SerializeField]
        private List<BaseModuleSetup> _moduleSetups;

        public ShipModulePlace[] DeckPlaces { get; private set;}
        
        private void Awake()
        {
            List<ShipModulePlace> places = new List<ShipModulePlace>();

            foreach (var module in _moduleSetups)
            {
                ShipModulePlace shipModulePlace = Instantiate(_shipModulePlacePrefab, transform);
                if (module != null)
                {
                    shipModulePlace.SetModule(module.GetModule());
                }
                places.Add(shipModulePlace); 
            }
            DeckPlaces = places.ToArray();
            UpdatePlacePosition();
            //DeckPlaces = GetComponentsInChildren<ShipModulePlace>();
        }
        
        
        private void UpdatePlacePosition()
        {
            float i = 0;
            var placesCount = DeckPlaces.Length;
            var redundant = placesCount / 2;
            i = placesCount % 2 == 0 ? -redundant + 0.5f : -redundant;
            foreach (var modulePlace in DeckPlaces)
            {
                modulePlace.transform.position = transform.position + Vector3.right * i*2.75f;
                i++;
            }
        }
        
        public void SetupDeck(ShipPlaceSignal shipPlaceSignal)
        {
            SetArmorValueText(_armor);
            foreach (var place in DeckPlaces)
            {
                SetupPlace(place, shipPlaceSignal);
            }
        }
        
        private void SetArmorValueText(int value)
        {
            _armorText.text = value.ToString();
        }
        
        
        private void SetupPlace(ShipModulePlace shipModulePlace, ShipPlaceSignal shipPlaceSignal)
        {
            shipModulePlace.UpdateDurability(_baseDurability);
            shipModulePlace.UpdateCrew(_baseCrew);
            shipModulePlace.UpdateArmor(_armor);
            shipModulePlace.SetLocationSignal(_moduleLocation, shipPlaceSignal);
        }

        public void UpgradeDurability()
        {
            _baseDurability++;
            foreach (var place in DeckPlaces)
            {
                place.UpdateDurability(_baseDurability);
            }
        }
        
        public void UpgradeCrew()
        {
            _baseCrew++;
            foreach (var place in DeckPlaces)
            {
                place.UpdateCrew(_baseCrew);;
            }
        }

        public void UpgradeArmor()
        {
            _armor++;
            SetArmorValueText(_armor);
            foreach (var place in DeckPlaces)
            {
                place.UpdateArmor(_armor);
            }
        }
    }
}