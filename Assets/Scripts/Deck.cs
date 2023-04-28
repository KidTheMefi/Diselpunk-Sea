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
            
            shipModulePlace.Setup(_baseDurability, _baseCrew, _armor, _moduleLocation, shipPlaceSignal);
        }
    }
}