using System;
using InterfaceProviders;
using ModulesScripts.ModulesSetup;
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
        
        public void Setup(BaseMedicalDepartmentSetup setup)
        {
            _medicineSkill = setup.MedicineSkill;
            BaseSetup(setup);
            UpdateDescription();
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
            moduleDescription.SetDescriptionText(description);
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