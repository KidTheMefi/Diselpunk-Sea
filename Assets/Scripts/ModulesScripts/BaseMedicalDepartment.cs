using System;
using InterfaceProviders;
using UnityEngine;

namespace ModulesScripts
{
    public class BaseMedicalDepartment : ShipModule, IMedicineProvider, IQuickRecoveryProvider
    {
        public event Action MedicineSkillChanged = delegate { };

        [SerializeField]
        private int _medicineSkill;
        
        public int GetMedicineSkill()
        {
            return IsInOrder ? _medicineSkill : 0;
        }
        
        private void Start()
        {
            UpdateDescription();
        }

        private void UpdateDescription()
        {
            string description = $"Medical Department. " +
                $"+{_medicineSkill} medicine.  " +
                $"{GetBaseDescription()} ";
            textMeshProDescription.text = description;
        }

        public override void SetActive(bool value)
        {
            IsInOrder = value;
            MedicineSkillChanged?.Invoke();
            UpdateDescription();
        }
        public bool HasQuickRecovery()
        {
            return hasCommanderOnDuty && IsInOrder;
        }
    }
}