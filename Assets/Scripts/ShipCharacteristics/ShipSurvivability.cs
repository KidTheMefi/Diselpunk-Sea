using System;
using System.Collections.Generic;
using InterfaceProviders;
using ShipModuleScripts.ModuleDurability;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ShipCharacteristics
{
    public class ShipSurvivability : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro _textMeshPro;
        [SerializeField]
        private Toggle _toggle;
        private DurabilityHandler[] _modulesDurabilityHandlers;
        [SerializeField]
        private int _recoverabilityValue;
        private int SurvivabilityValue;
        private int FullSurvivabilityValue;

        private DurabilitySignals _repairSignals;
        private RepairSkillHandler _repairSkillHandle;

        private DateTime _startTime;

        public void Setup(DurabilityHandler[] modulesDurability, RepairSkillHandler repairSkillHandler, List<IRecoverabilityProvider> recoverabilityProviders)
        {
            _startTime = DateTime.Now;

            _modulesDurabilityHandlers = modulesDurability;
            
            _repairSignals = new DurabilitySignals(repairSkillHandler);
            _repairSkillHandle = repairSkillHandler;
            foreach (var recoverabilityProvider in recoverabilityProviders)
            {
                _recoverabilityValue += recoverabilityProvider.GetAdditionalRecoverability();
            }
            _repairSignals.HaveRecoverabilityPoint(_recoverabilityValue > 0);

            SurvivabilityValue = 0;
            FullSurvivabilityValue = 0;
            foreach (var durabilityHandler in _modulesDurabilityHandlers)
            {
                FullSurvivabilityValue += durabilityHandler.DurabilityValue.MaxValue;
                SurvivabilityValue += durabilityHandler.DurabilityValue.CurrentValue;
                durabilityHandler.SetupSignal(_repairSignals);
            }

            _repairSignals.DurabilityDamaged += RepairSignalsOnDamaged;
            _repairSignals.RequestRepair += RepairSignalsOnRepair;

            _toggle.onValueChanged.AddListener(OnToggleValueChange);
            UpdateSurvivabilityText();

        }

        private void OnToggleValueChange(bool value)
        {
            foreach (var modules in _modulesDurabilityHandlers)
            {
                modules.ChangeToggle(value);
            }
        }

        private void RepairSignalsOnRepair(DurabilityHandler crewHandler, int value)
        {
            if (_recoverabilityValue < 1)
            {
                _repairSignals.HaveRecoverabilityPoint(false);
                return;
            }
            value = value > _recoverabilityValue ? _recoverabilityValue : value;
            _recoverabilityValue -= value;
            
            value = _repairSkillHandle.AdvancedRepair && AdvancedRepair() ? value*2: value;
            SurvivabilityValue += value;
            crewHandler.Repair(value);
            UpdateSurvivabilityText();
        }

        private bool AdvancedRepair()
        {
            var advancedRepair = _repairSkillHandle.RepairSkill *10;
            return Random.Range(0, 100) < advancedRepair;
        }

        private void RepairSignalsOnDamaged(int value)
        {
            SurvivabilityValue -= value;
            if (SurvivabilityValue < 0.3f * FullSurvivabilityValue)
            {
                ShipDestroyed();
            }
            UpdateSurvivabilityText();
        }

        private void UpdateSurvivabilityText()
        {
            _textMeshPro.text = $"Survivability: {SurvivabilityValue}/{FullSurvivabilityValue}" +
                $"\nRecoverability: {_recoverabilityValue}";
        }

        private void ShipDestroyed()
        {
            var time = DateTime.Now - _startTime;
            Debug.Log($"Ship begin sink at: {time.TotalSeconds}");
        }

        private void OnDestroy()
        {
            _repairSignals.DurabilityDamaged -= RepairSignalsOnDamaged;
            _repairSignals.RequestRepair -= RepairSignalsOnRepair;
        }
    }
}