using System;
using InterfaceProviders;
using ModulesScripts.ModulesSetup;
using UnityEngine;

namespace ModulesScripts
{
    public class BaseWaterPump : ShipModule, IPumpPowerProvider
    {
        public event Action PumpPowerChanged = delegate { };
        private int _pumpPower;
        
        
        public int GetPumpPower()
        {
            var pumpPower = IsInOrder ? _pumpPower : 0;
            pumpPower = hasCommanderOnDuty ? pumpPower * 2 : pumpPower;
            return pumpPower;
        }

        public override void SetCommanderOnDuty(bool value)
        {
            base.SetCommanderOnDuty(value);
            PumpPowerChanged.Invoke();
        }
        
        public void Setup(BaseWaterPumpSetup setup)
        {
            _pumpPower = setup.PumpPowerValue;
            BaseSetup(setup);
            UpdateDescription();
        }
        
        private void Start()
        {
            UpdateDescription();
        }

        private void UpdateDescription()
        {
            string description = $"Water pump. " +
                $"+{_pumpPower} pump power.  " +
                $"{GetBaseDescription()} ";
            moduleDescription.SetDescriptionText(description);
        }

        public override void SetActive(bool value)
        {
            IsInOrder = value;
            PumpPowerChanged?.Invoke();
            UpdateDescription();
        }
    }
}