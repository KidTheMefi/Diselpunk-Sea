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
        public event Action EndBattle;
        [SerializeField]
        private TextMeshPro _textMeshPro;
        [SerializeField]
        private Toggle _toggle;
        
        private CrewHandler[] _crewsOnModule;

        public int MaxCrewValue { get; private set; }
        public int OnDutyCrewValue { get; private set; }
        [SerializeField]
        private int reserveCrewValue;
        public int CurrentReserveCrewValue { get; private set; }
        private int CurrentInjuredCrewValue;
        private int CurrentDeadCrewValue;
        public int TotalActiveCrew => OnDutyCrewValue + CurrentReserveCrewValue;

        private CrewSignals _crewSignals = new CrewSignals();
        private DateTime _startTime;
        private MedicineHandler _medicineHandler;
        
        public void Setup(CrewHandler[] crews, MedicineHandler medicineHandler, Action endBattle)
        {
            EndBattle = endBattle;
            _medicineHandler = medicineHandler;
            _startTime = DateTime.Now;   
            _crewsOnModule = crews;

            UpdateCrew();
            
            foreach (var crew in _crewsOnModule)
            {
                crew.SetSignals(_crewSignals);
            }
            
            
            _crewSignals.CrewMembersDamaged += CrewSignalsOnCrewMembersDamaged;
            _crewSignals.CrewMembersGoToReserve += CrewSignalsOnCrewMembersGoToReserve;
            _crewSignals.RequestCrewMembersFromReserve += RequestCrewSignalsOnRequestCrewMembersFromReserve;
            _toggle.onValueChanged.AddListener(OnToggleValueChange);
            UpdateCrewText();
        }

        public void UpdateCrew()
        {
            CurrentInjuredCrewValue = 0;
            CurrentDeadCrewValue = 0;
            OnDutyCrewValue = 0;
            MaxCrewValue = 0;
            
            foreach (var crew in _crewsOnModule)
            {
                MaxCrewValue += crew.CrewValue.MaxValue;
                OnDutyCrewValue += crew.CrewValue.CurrentValue;
            }
            CurrentReserveCrewValue = reserveCrewValue > 0 ? reserveCrewValue : OnDutyCrewValue/2;
            MaxCrewValue += CurrentReserveCrewValue;
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

        public void AddNewCrew(int crew)
        {
            crew = crew > 0 ? crew : 0;

            var freeSpaceForCrew = MaxCrewValue - TotalActiveCrew;
            crew = crew > freeSpaceForCrew ? freeSpaceForCrew : crew;
            CurrentReserveCrewValue += crew;
            UpdateCrewText();
        }

        public void RecoverInjuredCrew()
        {
            AddNewCrew(CurrentInjuredCrewValue);
            CurrentInjuredCrewValue = 0;
            CurrentDeadCrewValue = 0;
            UpdateCrewText();
        }
        

        private void ShipDestroyed()
        {
            var time = DateTime.Now - _startTime;
            Debug.Log($"Crew Eliminated at: {time.TotalSeconds}");
            EndBattle?.Invoke();
            EndBattle = null;
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