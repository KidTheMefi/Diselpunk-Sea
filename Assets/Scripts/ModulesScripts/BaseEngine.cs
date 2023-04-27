using System;
using DefaultNamespace;
using InterfaceProviders;
using UnityEngine;

namespace ModulesScripts
{
    public class BaseEngine : ShipModule, IManeuverabilityProvider, ISpeedProvider
    {
        public event Action ManeuverabilityChanged = delegate { };
        public event Action SpeedChanged = delegate { };

        [SerializeField]
        private int _maneuverabilityValue;
        [SerializeField]
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
        
        public int GetSpeed()
        {
            var speed = !IsInOrder ? 0 : hasCommanderOnDuty ? _speed * 2 : _speed;
            return speed;
        }
        public override void SetCommanderOnDuty(bool value)
        {
            base.SetCommanderOnDuty(value);
            SpeedChanged?.Invoke();
        }

        public override void SetActive(bool value)
        {
            IsInOrder = value;
            ManeuverabilityChanged?.Invoke();
            SpeedChanged?.Invoke();
            UpdateDescription();
        }
    }
}