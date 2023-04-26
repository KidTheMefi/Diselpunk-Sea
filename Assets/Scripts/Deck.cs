using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class Deck : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro _armorText;
        [SerializeField]
        private int _armor;
        [SerializeField]
        private ModuleLocation _moduleLocation;
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
            
            shipModulePlace.SetLocation(_moduleLocation);
            shipModulePlace.SetArmor(_armor);
            shipModulePlace.SetShipPlaceSignal(shipPlaceSignal);
        }
    }
}