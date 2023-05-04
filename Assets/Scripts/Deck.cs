using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class Deck : MonoBehaviour
    {
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
        public ShipModulePlace[] DeckPlaces { get; private set;}
        
        private void Awake()
        {
            DeckPlaces = GetComponentsInChildren<ShipModulePlace>();
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
            shipModulePlace.Setup(_moduleLocation, shipPlaceSignal);
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