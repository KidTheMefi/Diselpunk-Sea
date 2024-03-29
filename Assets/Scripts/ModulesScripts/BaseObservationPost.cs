﻿using System;
using DefaultNamespace;
using InterfaceProviders;
using ModulesScripts.ModulesSetup;
using UnityEngine;

namespace ModulesScripts
{
    public class BaseObservationPost : ShipModule, IDetectionProvider, ITargetRequired
    {
        public event Action DetectionChanged = delegate{ };
        [SerializeField]
        private int _detectionValue;
        
        private BaseShip _targetShip;


        public void Setup(BaseObservationPostSetup setup)
        {
            _detectionValue = setup.DetectionValue;
            BaseSetup(setup);
            UpdateDescription();
        }
        
        public int GetDetection()
        {
            return IsInOrder ? _detectionValue : 0;
        }
        
        public void SetShipTarget(BaseShip targetShip)
        {
            _targetShip = targetShip;
        }
        
        private void UpdateDescription()
        {
            string description = $"{gameObject.name}. " +
                $"+{_detectionValue} Detection.  " +
                $"{GetBaseDescription()} ";
            moduleDescription.SetDescriptionText(description);
        }
       
        public override void SetCommanderOnDuty(bool value)
        {
            base.SetCommanderOnDuty(value);

            if (_targetShip != null)
            {
                _targetShip.Observation(hasCommanderOnDuty&&IsInOrder);
            }
            
        }
        
        public override void SetActive(bool value)
        {
            IsInOrder = value;
            
            if (_targetShip != null)
            {
                _targetShip.Observation(hasCommanderOnDuty&&IsInOrder);
            }
            DetectionChanged?.Invoke();
            UpdateDescription();
        }
    }
}