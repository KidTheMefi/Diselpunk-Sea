using System.Collections.Generic;
using InterfaceProviders;
using TMPro;
using UnityEngine;

namespace ShipCharacteristics
{
    public class MedicineHandler : MonoBehaviour
    {
        private IntValue _medicineValue = new IntValue(10);
        private List<IMedicineProvider> _medicineProviders;
        private int _baseMedicine;

        [SerializeField]
        private TextMeshPro _textMeshPro;

        public int Medicine => _medicineValue.CurrentValue;
        private IQuickRecoveryProvider _quickRecoveryProvider;
        
        
        public void Setup(int baseMedicine, List<IMedicineProvider> medicineProviders, IQuickRecoveryProvider quickRecoveryProvider = null)
        {
            _quickRecoveryProvider = quickRecoveryProvider;
            _baseMedicine = baseMedicine;
            _medicineValue.SetValueTo(_baseMedicine);
            _medicineProviders = medicineProviders;
            CalculateCurrentMedicine();

            foreach (var provider in _medicineProviders)
            {
                provider.MedicineSkillChanged += CalculateCurrentMedicine;
            }
        }
        
        public bool QuickRecovery()
        {
            return _quickRecoveryProvider != null && _quickRecoveryProvider.HasQuickRecovery();
        }
        
        private void CalculateCurrentMedicine()
        {
            int newValue = _baseMedicine;
            foreach (var provider in _medicineProviders)
            {
                newValue += provider.GetMedicineSkill();
            }
            _medicineValue.SetValueTo(newValue);
            _textMeshPro.text = $"Current medicine: {newValue}";
        }

        private void OnDestroy()
        {
            foreach (var provider in _medicineProviders)
            {
                provider.MedicineSkillChanged -= CalculateCurrentMedicine;
            }
        }
        
    }
}