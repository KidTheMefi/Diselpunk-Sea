using System;
using InterfaceProviders;
using UnityEngine;

namespace ModulesScripts
{
    public class BaseEngineeringDepartment : ShipModule, IRepairSkillProvider, IRecoverabilityProvider, IAdvancedRepairProvider
    {
        public event Action RepairSkillChanged = delegate { };

        [SerializeField, Range(0,9)]
        private int _repaireSkillValue;
        [SerializeField, Range(0,50)]
        private int _recoverability;

        private void Start()
        {
            UpdateDescription();
        }

        private void UpdateDescription()
        {
            string description = $"Engineering Department. " +
                $"+{_repaireSkillValue} repair.  " +
                $"{GetBaseDescription()} ";

            description += IsInOrder ? "" : "Out of order!";
            textMeshProDescription.text = description;
        }

        public override void SetActive(bool value)
        {
            IsInOrder = value;
            RepairSkillChanged?.Invoke();
            UpdateDescription();
        }
        
        
        public int GetRepairSkill()
        {
            return IsInOrder ? _repaireSkillValue : 0;
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