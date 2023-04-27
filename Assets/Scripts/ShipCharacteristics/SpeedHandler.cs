using System;
using System.Collections.Generic;
using InterfaceProviders;
using TMPro;
using UnityEngine;

namespace ShipCharacteristics
{
    public class SpeedHandler : MonoBehaviour
    {
        public event Action SpeedChange = delegate { };
        
        private IntValue _speedValue = new IntValue(30);
        private List<ISpeedProvider> _speedProviders;

        [SerializeField]
        private TextMeshPro _textMeshPro;

        public int Speed => _speedValue.CurrentValue;
        
        
        public void Setup( List<ISpeedProvider> speedProviders)
        {
     
            _speedProviders = speedProviders;
            CalculateCurrentSpeed();

            foreach (var provider in _speedProviders)
            {
                provider.SpeedChanged += CalculateCurrentSpeed;
            }
        }
        
        private void CalculateCurrentSpeed()
        {
            int newValue = 0;
            foreach (var provider in _speedProviders)
            {
                newValue += provider.GetSpeed();
            }
            _speedValue.SetValueTo(newValue);
            _textMeshPro.text = $"Current speed: {newValue}";
            SpeedChange.Invoke();
        }

        private void OnDestroy()
        {
            foreach (var provider in _speedProviders)
            {
                provider.SpeedChanged -= CalculateCurrentSpeed;
            }
        }
    }
}