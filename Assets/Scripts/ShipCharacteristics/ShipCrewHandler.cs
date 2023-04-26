using System;
using ShipModuleScripts.ModuleCrew;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ShipCharacteristics
{
    public class ShipCrewHandler : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro _textMeshPro;
        [SerializeField]
        private Toggle _toggle;
        
        private CrewHandler[] _crewsOnModule;

        private int MaxCrewValue;
        private int OnDutyCrewValue;
        private int CurrentReserveCrewValue;
        private int CurrentInjuredCrewValue;
        private int CurrentDeadCrewValue;
        private int TotalActiveCrew => OnDutyCrewValue + CurrentReserveCrewValue;

        private CrewSignals _crewSignals = new CrewSignals();
        private DateTime _startTime;
        private MedicineHandler _medicineHandler;
        
        public void Begin(CrewHandler[] crews, MedicineHandler medicineHandler)
        {
            _medicineHandler = medicineHandler;
            _startTime = DateTime.Now;   
            _crewsOnModule = crews;
            CurrentInjuredCrewValue = 0;
            CurrentDeadCrewValue = 0;
            OnDutyCrewValue = 0;
            CurrentReserveCrewValue = 20;
            MaxCrewValue = CurrentReserveCrewValue;
            
            foreach (var crew in _crewsOnModule)
            {
                MaxCrewValue += crew.CrewValue.MaxValue;
                OnDutyCrewValue += crew.CrewValue.CurrentValue;
                crew.SetSignals(_crewSignals);
            }
            
            _crewSignals.CrewMembersDamaged += CrewSignalsOnCrewMembersDamaged;
            _crewSignals.CrewMembersGoToReserve += CrewSignalsOnCrewMembersGoToReserve;
            _crewSignals.RequestCrewMembersFromReserve += RequestCrewSignalsOnRequestCrewMembersFromReserve;
            _toggle.onValueChanged.AddListener(OnToggleValueChange);
            UpdateCrewText();
        }

        private void OnToggleValueChange(bool value)
        {
            foreach (var crew in _crewsOnModule)
            {
                crew.ChangeToggle(value);
            }
        }
        
        private void RequestCrewSignalsOnRequestCrewMembersFromReserve(CrewHandler crewHandler, int value)
        {
            if (CurrentReserveCrewValue < 1)
            {
                return;
            }
            value = value > CurrentReserveCrewValue ?  CurrentReserveCrewValue : value;
            CurrentReserveCrewValue -= value;
            OnDutyCrewValue += value;
            crewHandler.AddCrew(value);
            UpdateCrewText();
        }
        
        private void CrewSignalsOnCrewMembersGoToReserve(int value)
        {
            OnDutyCrewValue -= value;
            CurrentReserveCrewValue += value;
            UpdateCrewText();
        }


        private bool Survive()
        {
            var surviveChance = _medicineHandler.Medicine *10;
            return Random.Range(0, 100) < surviveChance;
        }
        
        private void CrewSignalsOnCrewMembersDamaged(int value)
        {
            for (int i = 0; i < value; i++)
            {
                OnDutyCrewValue--;
                
                if (Survive())
                {
                    if (_medicineHandler.QuickRecovery())
                    {
                        CurrentReserveCrewValue++;
                    }
                    else
                    {
                        CurrentInjuredCrewValue++;
                    }
                    
                }
                else
                {
                    if (_medicineHandler.QuickRecovery())
                    {
                        CurrentInjuredCrewValue++;
                        
                    }
                    else
                    {
                        CurrentDeadCrewValue++;
                    }
                }
                
            }
            
            if (TotalActiveCrew < 0.3f * MaxCrewValue)
            {
                ShipDestroyed();
            }
            UpdateCrewText();
        }
        

        private void ShipDestroyed()
        {
            var time = DateTime.Now - _startTime;
            Debug.Log($"Crew Eliminated at: {time.TotalSeconds}");
        }

        private void UpdateCrewText()
        {
            _textMeshPro.text = $"Total crew: {TotalActiveCrew}/{MaxCrewValue}" +
                $"\nOnDuty/Reserve {OnDutyCrewValue}/{CurrentReserveCrewValue}" +
                $"\nInjured/Dead: {CurrentInjuredCrewValue}/{CurrentDeadCrewValue}";
        }

        private void OnDestroy()
        {
            _crewSignals.CrewMembersDamaged -= CrewSignalsOnCrewMembersDamaged;
            _crewSignals.CrewMembersGoToReserve -= CrewSignalsOnCrewMembersGoToReserve;
            _crewSignals.RequestCrewMembersFromReserve -= RequestCrewSignalsOnRequestCrewMembersFromReserve;
        }
    }
}