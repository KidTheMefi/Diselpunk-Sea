using System.Collections.Generic;
using InterfaceProviders;
using TMPro;
using UnityEngine;

namespace ShipCharacteristics
{
    public class RepairSkillHandler : MonoBehaviour
    {
        private IntValue RepairSkillValue = new IntValue(10);
        private List<IRepairSkillProvider> _repairSkillProviders;
        private int _baseRepairSkill;

        [SerializeField]
        private TextMeshPro _textMeshPro;

        public int RepairSkill => RepairSkillValue.CurrentValue;

        private IAdvancedRepairProvider _advancedRepairProvider;
        public bool AdvancedRepair => _advancedRepairProvider?.AdvancedRepair() ?? false;

        public void Setup(int baseRepairSkill, List<IRepairSkillProvider> repairSkillProviders, IAdvancedRepairProvider advancedRepairProvider = null)
        {
            _advancedRepairProvider = advancedRepairProvider;
            _baseRepairSkill = baseRepairSkill;
            RepairSkillValue.SetValueTo(baseRepairSkill);
            _repairSkillProviders = repairSkillProviders;
            CalculateCurrentRepairSkill();

            foreach (var provider in _repairSkillProviders)
            {
                provider.RepairSkillChanged += CalculateCurrentRepairSkill;
            }
        }

        private void CalculateCurrentRepairSkill()
        {
            int newValue = _baseRepairSkill;
            foreach (var provider in _repairSkillProviders)
            {
                newValue += provider.GetRepairSkill();
            }
            RepairSkillValue.SetValueTo(newValue);
            _textMeshPro.text = $"Current repair skill: {newValue}";
        }

        private void OnDestroy()
        {
            foreach (var provider in _repairSkillProviders)
            {
                provider.RepairSkillChanged -= CalculateCurrentRepairSkill;
            }
        }
    }
}