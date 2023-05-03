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
        public event Action EndBattle;
        [SerializeField]
        private TextMeshPro _textMeshPro;
        [SerializeField]
        private Toggle _toggle;
        private DurabilityHandler[] _modulesDurabilityHandlers;
        [SerializeField]
        private int _recoverabilityValue;
        private int _maxRecoverability;
        public int SurvivabilityValue { get; private set; }
        public int FullSurvivabilityValue { get; private set; }

        private DurabilitySignals _durabilitySignals;
        private RepairSkillHandler _repairSkillHandle;

        private DateTime _startTime;



        public void Setup(DurabilityHandler[] modulesDurability,
            RepairSkillHandler repairSkillHandler,
            List<IRecoverabilityProvider> recoverabilityProviders,
            DurabilitySignals durabilitySignals,
            Action endBattle)
        {

            EndBattle = endBattle;
            _startTime = DateTime.Now;

            _modulesDurabilityHandlers = modulesDurability;

            _durabilitySignals = durabilitySignals;
            _repairSkillHandle = repairSkillHandler;
            foreach (var recoverabilityProvider in recoverabilityProviders)
            {
                _recoverabilityValue += recoverabilityProvider.GetAdditionalRecoverability();
            }
            _durabilitySignals.HaveRecoverabilityPoint(_recoverabilityValue > 0);
            _maxRecoverability = _recoverabilityValue;
            SurvivabilityValue = 0;
            FullSurvivabilityValue = 0;
            
            foreach (var durabilityHandler in _modulesDurabilityHandlers)
            {
                FullSurvivabilityValue += durabilityHandler.DurabilityValue.MaxValue;
                SurvivabilityValue += durabilityHandler.DurabilityValue.CurrentValue;
            }

            _durabilitySignals.DurabilityDamaged += OnDamagedSignal;
            _durabilitySignals.RequestRepair += OnRepairRequestSignal;

            _toggle.onValueChanged.AddListener(OnToggleValueChange);
            UpdateSurvivabilityText();


        }

        public void OnToggleValueChange(bool value)
        {
            foreach (var modules in _modulesDurabilityHandlers)
            {
                modules.ChangeToggle(value);
            }
        }

        private void OnRepairRequestSignal(DurabilityHandler crewHandler, int value)
        {
            if (_recoverabilityValue < 1)
            {
                _durabilitySignals.HaveRecoverabilityPoint(false);
                return;
            }
            value = value > _recoverabilityValue ? _recoverabilityValue : value;
            _recoverabilityValue -= value;

            value = _repairSkillHandle.AdvancedRepair && AdvancedRepair() ? value * 2 : value;
            SurvivabilityValue += value;
            crewHandler.Repair(value);
            UpdateSurvivabilityText();
        }

        private bool AdvancedRepair()
        {
            var advancedRepair = _repairSkillHandle.RepairSkill * 10;
            return Random.Range(0, 100) < advancedRepair;
        }

        private void OnDamagedSignal(int value)
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
                $"\nRecoverability: {_recoverabilityValue}/{_maxRecoverability}";
        }

        public void AddRecoverability(int value)
        {
            value = value > 0 ? value : 0;
            var possibleRecoverability = _maxRecoverability - _recoverabilityValue;
            value = value > possibleRecoverability ? possibleRecoverability : value;
            _recoverabilityValue += value;
            _durabilitySignals.HaveRecoverabilityPoint(_recoverabilityValue > 0);
            UpdateSurvivabilityText();
        }

        private void ShipDestroyed()
        {
            var time = DateTime.Now - _startTime;
            Debug.Log($"Ship begin sink at: {time.TotalSeconds}");
            EndBattle?.Invoke();
            EndBattle = null;
        }

        private void OnDestroy()
        {
            EndBattle = null;
            _durabilitySignals.DurabilityDamaged -= OnDamagedSignal;
            _durabilitySignals.RequestRepair -= OnRepairRequestSignal;
        }
    }
}