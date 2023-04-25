using System.Collections.Generic;
using DefaultNamespace;
using InterfaceProviders;
using TMPro;
using UnityEngine;

namespace ShipCharacteristics
{
    public class ManeuverabilityHandler : MonoBehaviour
    {
        private IntValue ManeuverabilityValue = new IntValue(30);
        private List<IManeuverabilityProvider> _maneuverabilityProviders;
        private int _baseManeuverability;

        [SerializeField]
        private TextMeshPro _textMeshPro;

        public int Maneuverability => ManeuverabilityValue.CurrentValue;
        private IEvasionProvider _evasionProvider;


        public bool CanEvade()
        {
            return _evasionProvider != null && _evasionProvider.HasEvasion();
        }
        
        public void Setup(int baseManeuverability, List<IManeuverabilityProvider> maneuverabilityProviders, IEvasionProvider evasionProvider)
        {
            _evasionProvider = evasionProvider;
            _baseManeuverability = baseManeuverability;
            ManeuverabilityValue.SetValueTo(baseManeuverability);
            _maneuverabilityProviders = maneuverabilityProviders;
            CalculateCurrentManeuverability();

            foreach (var provider in _maneuverabilityProviders)
            {
                provider.ManeuverabilityChanged += CalculateCurrentManeuverability;
            }
        }

        private void CalculateCurrentManeuverability()
        {
            int newValue = _baseManeuverability;
            foreach (var provider in _maneuverabilityProviders)
            {
                newValue += provider.GetManeuverability();
            }
            ManeuverabilityValue.SetValueTo(newValue);
            _textMeshPro.text = $"Current maneuverability: {newValue}";
        }

        private void OnDestroy()
        {
            foreach (var provider in _maneuverabilityProviders)
            {
                provider.ManeuverabilityChanged -= CalculateCurrentManeuverability;
            }
        }
    }
}