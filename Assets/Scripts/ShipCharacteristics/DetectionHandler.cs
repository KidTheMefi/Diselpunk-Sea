using System.Collections.Generic;
using InterfaceProviders;
using TMPro;
using UnityEngine;

namespace ShipCharacteristics
{
    public class DetectionHandler : MonoBehaviour
    {
        private IntValue _detectionValue = new IntValue(30);
        private List<IDetectionProvider> _maneuverabilityProviders;
        private int _baseDetection;

        [SerializeField]
        private TextMeshPro _textMeshPro;

        public int Detection => _detectionValue.CurrentValue;

        public void Setup(int baseDetection, List<IDetectionProvider> maneuverabilityProviders)
        {
            _baseDetection = baseDetection;
            _detectionValue.SetValueTo(baseDetection);
            _maneuverabilityProviders = maneuverabilityProviders;
            CalculateCurrentManeuverability();

            foreach (var provider in _maneuverabilityProviders)
            {
                provider.DetectionChanged += CalculateCurrentManeuverability;
            }
        }

        private void CalculateCurrentManeuverability()
        {
            int newValue = _baseDetection;
            foreach (var provider in _maneuverabilityProviders)
            {
                newValue += provider.GetDetection();
            }
            _detectionValue.SetValueTo(newValue);
            _textMeshPro.text = $"Current detection: {newValue}";
        }

        private void OnDestroy()
        {
            foreach (var provider in _maneuverabilityProviders)
            {
                provider.DetectionChanged -= CalculateCurrentManeuverability;
            }
        }
    }
}