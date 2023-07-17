using System.Collections.Generic;
using InterfaceProviders;
using TMPro;
using UnityEngine;

namespace ShipCharacteristics
{
    public class SonarHandler: MonoBehaviour
    {
        [SerializeField, Range(0,2)]
        private int _baseSonarValue;
        private IntValue _sonarValue = new IntValue(30);
        private List<ISonarValueProvider> _isSonarValueProviders= new List<ISonarValueProvider>();

        [SerializeField]
        private TextMeshPro _textMeshPro;

        public int SonarDetection => _sonarValue.CurrentValue;

        public void Setup( List<ISonarValueProvider> sonarProviders)
        {
            _sonarValue.SetValueTo(_baseSonarValue);
            _isSonarValueProviders = sonarProviders;
            CalculateCurrentManeuverability();

            foreach (var provider in _isSonarValueProviders)
            {
                provider.SonarValueChanged += CalculateCurrentManeuverability;
            }
        }

        private void CalculateCurrentManeuverability()
        {
            int newValue = _baseSonarValue;
            foreach (var provider in _isSonarValueProviders)
            {
                newValue += provider.GetSonarValue();
            }
            _sonarValue.SetValueTo(newValue);
            _textMeshPro.text = $"Current sonar detection: {newValue}";
        }

        private void OnDestroy()
        {
            foreach (var provider in _isSonarValueProviders)
            {
                provider.SonarValueChanged -= CalculateCurrentManeuverability;
            }
        }
    }
}