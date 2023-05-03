using System;
using DefaultNamespace;
using InterfaceProviders;
using UnityEngine;

namespace ModulesScripts
{
    public class BaseCaptainsBridge : ShipModule, IManeuverabilityProvider, IDetectionProvider, IEvasionProvider
    {
        public event Action ManeuverabilityChanged = delegate { };
        public event Action DetectionChanged = delegate { };
        
        [SerializeField]
        private int _maneuverabilityValue;
        [SerializeField]
        private int _detectionValue;

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
            string description = $"Captains Bridge. " +
                $"+{_maneuverabilityValue} Maneuverability.  " +
                $"{GetBaseDescription()} ";
            moduleDescription.SetDescriptionText(description);
        }
        
        public int GetManeuverability()
        {
            return IsInOrder ? _maneuverabilityValue : 0;
        }
        
        public override void SetActive(bool value)
        {
            IsInOrder = value;
            ManeuverabilityChanged?.Invoke();
            DetectionChanged?.Invoke();
            UpdateDescription();
        }
        
        public int GetDetection()
        {
            return IsInOrder ? _detectionValue : 0;
        }
        
        public bool HasEvasion()
        {
            return hasCommanderOnDuty && IsInOrder;
        }
    }
}