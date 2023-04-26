using System;
using DefaultNamespace;
using UnityEngine;

namespace ModulesScripts
{
    public class BaseEngine : ShipModule, IManeuverabilityProvider
    {
        public event Action ManeuverabilityChanged = delegate { };
        
        [SerializeField]
        private int _maneuverabilityValue;
        private int _speed;

        private void Start()
        {
            UpdateDescription();
        }

        /*public void Setup(int speed, int maneuverabilityValue)
        {
            _maneuverabilityValue = maneuverabilityValue;
            _speed = speed;
            UpdateDescription();
        }*/

        private void UpdateDescription()
        {
            string description = $"{gameObject.name}. " +
                $"+{_maneuverabilityValue} Maneuverability.  " +
                $"{_speed} speed. " +
                $"{GetBaseDescription()} ";

            description += IsInOrder ? "" : "Out of order!"; 
            textMeshProDescription.text = description;
        }
        
        public int GetManeuverability()
        {
            return IsInOrder ? _maneuverabilityValue : 0;
        }
        
        public override void SetActive(bool value)
        {
            IsInOrder = value;
            ManeuverabilityChanged?.Invoke();
            UpdateDescription();
        }
    }
}