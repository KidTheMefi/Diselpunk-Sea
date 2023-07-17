using System;
using DefaultNamespace;
using InterfaceProviders;
using ModulesScripts.ModulesSetup;
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

        public void Setup(BaseCaptainsBridgeSetup baseCaptainsBridgeSetup)
        {
            _maneuverabilityValue = baseCaptainsBridgeSetup.ManeuverabilityValue;
            _detectionValue = baseCaptainsBridgeSetup.DetectionValue;
            BaseSetup(baseCaptainsBridgeSetup);
            UpdateDescription();
        }

        private void UpdateDescription()
        {
            string description = $"Captains Bridge. " +
                $"+{_maneuverabilityValue} Maneuverability.  " +
                $"+{_detectionValue} Detection.  " +
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