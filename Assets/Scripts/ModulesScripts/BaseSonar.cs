using System;
using InterfaceProviders;
using ModulesScripts.ModulesSetup;
using UnityEngine;

namespace ModulesScripts
{
    public class BaseSonar : ShipModule, ISonarValueProvider
    {
        public event Action SonarValueChanged = delegate { };
        [SerializeField]
        private int _sonarValue;
        
        
        public int GetSonarValue()
        {
            var sonarValue = IsInOrder ? _sonarValue : 0;
            sonarValue = hasCommanderOnDuty ? sonarValue * 2 : sonarValue;
            return sonarValue;
        }

        public override void SetCommanderOnDuty(bool value)
        {
            base.SetCommanderOnDuty(value);
            SonarValueChanged.Invoke();
        }
        
        public void Setup(BaseSonarSetup setup)
        {
            _sonarValue = setup.SonarValue;
            BaseSetup(setup);
            UpdateDescription();
        }
        
        private void Start()
        {
            UpdateDescription();
        }

        private void UpdateDescription()
        {
            string description = $"Sonar. " +
                $"+{_sonarValue} sonar detection.  " +
                $"{GetBaseDescription()} ";
            moduleDescription.SetDescriptionText(description);
        }

        public override void SetActive(bool value)
        {
            IsInOrder = value;
            SonarValueChanged?.Invoke();
            UpdateDescription();
        }
    }
}