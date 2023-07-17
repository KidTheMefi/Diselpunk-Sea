﻿using System;
using DefaultNamespace;
using InterfaceProviders;
using ModulesScripts.ModulesSetup;
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

        
        public void Setup(int speed, int maneuverabilityValue, BaseModuleSetup baseModuleSetup, Sprite moduleImage = null)
        {
            _maneuverabilityValue = maneuverabilityValue;
            _speed = speed;
            BaseSetup(baseModuleSetup);
            UpdateDescription();
        }

        private void UpdateDescription()
        {
            string description = $"{gameObject.name}. " +
                $"+{_maneuverabilityValue} Maneuverability.  " +
                $"{_speed} speed. " +
                $"{GetBaseDescription()} ";
            moduleDescription.SetDescriptionText(description);
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