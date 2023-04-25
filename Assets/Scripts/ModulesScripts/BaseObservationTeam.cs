using System;
using InterfaceProviders;
using UnityEngine;

namespace ModulesScripts
{
    public class BaseObservationTeam : ShipModule, IDetectionProvider
    {
        public event Action DetectionChanged = delegate { };
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
            string description = $"Observation Team. " +
                $"+{_detectionValue} detection.  " +
                $"{GetBaseDescription()} ";

            description += IsInOrder ? "" : "Out of order!";
            textMeshProDescription.text = description;
        }

        public int GetDetection()
        {
            return IsInOrder ? _detectionValue : 0;
        }

        public override void SetActive(bool value)
        {
            IsInOrder = value;
            DetectionChanged?.Invoke();
            UpdateDescription();
        }
    }

}