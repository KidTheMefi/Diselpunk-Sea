using System;
using InterfaceProviders;
using ModulesScripts.ModulesSetup;
using UnityEngine;

namespace ModulesScripts
{
    public class BaseEngineeringDepartment : ShipModule, IRepairSkillProvider, IRecoverabilityProvider, IAdvancedRepairProvider
    {
        public event Action RepairSkillChanged = delegate { };

        [SerializeField, Range(0,9)]
        private int _repairSkillValue;
        [SerializeField, Range(0,50)]
        private int _recoverability;

        private void Start()
        {
            UpdateDescription();
        }
        
        public void Setup(BaseEngineeringDepartmentSetup setup)
        {
            _recoverability = setup.Recoverability;
            _repairSkillValue = setup.RepairSkillValue;
            BaseSetup(setup);
            UpdateDescription();
        }

        private void UpdateDescription()
        {
            string description = $"Engineering Department. " +
                $"+{_repairSkillValue} repair.  " +
                $"{GetBaseDescription()} ";
            moduleDescription.SetDescriptionText(description);
        }

        public override void SetActive(bool value)
        {
            IsInOrder = value;
            RepairSkillChanged?.Invoke();
            UpdateDescription();
        }
        
        
        public int GetRepairSkill()
        {
            return IsInOrder ? _repairSkillValue : 0;
        }
        public int GetAdditionalRecoverability()
        {
            return _recoverability;
            
        }
        public bool AdvancedRepair()
        {
            return hasCommanderOnDuty && IsInOrder;
        }
    }
}