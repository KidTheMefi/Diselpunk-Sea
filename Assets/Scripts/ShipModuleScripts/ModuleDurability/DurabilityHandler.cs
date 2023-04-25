using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShipModuleScripts.ModuleDurability
{
    public class DurabilityHandler : MonoBehaviour
    {
        public event Action FunctionalityChange = delegate { };
        [SerializeField]
        private Toggle _durabilityRestore;
        [SerializeField]
        private TextMeshPro _textMeshPro;
        [SerializeField]
        private SpriteRenderer _damageSpriteRenderer;
        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private SliderTimer _repairTimer;
        [SerializeField]
        private SpriteRenderer _warningRenderer;
        [SerializeField]
        private Slider _repairValueSlider;
        private CancellationTokenSource _repairCTS;
        private int _minDurabilityRequired = 3;
        private bool _enoughCrewForRepair = true;
        private DurabilitySignals _durabilitySignals;
        public bool Functional { get; private set; }
        private IntValue _durabilityValue = new IntValue(15);
        public IntValue DurabilityValue => _durabilityValue;
        private float _repairTime = 10f;
        private Color _defaultColor;

        public void Setup(IntValue durabilityValue, int minDurabilityRequired)
        {
            _minDurabilityRequired = minDurabilityRequired;
            _durabilityValue = durabilityValue;
            _defaultColor = _damageSpriteRenderer.color;
            _durabilityRestore.onValueChanged.AddListener(ChangeToggle);
            SetupSlider();
            FunctionalityCheck();
            UpdateVisualView();
        }

        public void SetupSignal(DurabilitySignals durabilitySignals)
        {
            _durabilitySignals = durabilitySignals;
        }

        private void RepairToValue(float value)
        {
            if (_repairValueSlider.value > _durabilityValue.CurrentValue)
            {
                Repair();
            }
            else
            {
                _repairCTS?.Cancel();
            }
            
        }
        
        private void SetupSlider()
        {
            _slider.maxValue = _durabilityValue.MaxValue;
            _slider.value = _durabilityValue.CurrentValue;
            _slider.minValue = _durabilityValue.MinValue;
            
            _repairValueSlider.maxValue = _durabilityValue.MaxValue;
            //_repairValueSlider.SetValueWithoutNotify(_minDurabilityRequired);
            _repairValueSlider.value = _durabilityValue.MaxValue;
            _repairValueSlider.minValue = _durabilityValue.MinValue;
            _repairValueSlider.onValueChanged.AddListener(RepairToValue);
            
        }

        public void ChangeToggle(bool value)
        {
            if (_durabilityRestore.isOn != value)
            {
                _durabilityRestore.isOn = value;
            }

            Repair();
        }

        public void EnoughCrewToRepair(bool value)
        {
            if (value)
            {
                _enoughCrewForRepair = true;
                UpdateVisualView();
            }
            else
            {
                _enoughCrewForRepair = false;
            }
            Repair();
        }

        private void Repair()
        {
            if (CanBeRepaired())
            {
                if (_repairCTS == null || _repairCTS.Token.IsCancellationRequested)
                {
                    _repairCTS = new CancellationTokenSource();
                    RepairAsync(_repairCTS.Token).Forget();
                }
            }
            else
            {
                _repairCTS?.Cancel();
            }
        }

        private bool CanBeRepaired()
        {
            return _durabilityRestore.isOn && _enoughCrewForRepair && _durabilitySignals.HasRecoverabilityPoint;
        }

        private async UniTask RepairAsync(CancellationToken token)
        {
            if (_repairValueSlider.value <= _durabilityValue.CurrentValue  || !CanBeRepaired() || _durabilityValue.FullValue)
            {
                _repairCTS.Cancel();
                return;
            }
            float repairTime = (1 - 0.05f * _durabilitySignals.GetRepairSkill()) * _repairTime;
           // repairTime = _dutyPost.CommanderOnPost == null ? repairTime : repairTime / 3;
            repairTime = repairTime < 0.3f ? 0.3f : repairTime; 
            await _repairTimer.TimerAsync(repairTime, token);
            
            if (token.IsCancellationRequested)
            {
                return;
            }

            if (/*!_durabilityValue.FullValue*/ _repairValueSlider.value > _durabilityValue.CurrentValue)
            {
                _durabilitySignals.RequestRepairInvoke(this, 1);
            }
            RepairAsync(token).Forget();
        }
        
        public void CommanderOnPost(bool value)
        {
            _repairTime = value ? 3 : 10;
            float repairTime = (1 - 0.05f * _durabilitySignals.GetRepairSkill()) * _repairTime;
            repairTime = repairTime < 0.3f ? 0.3f : repairTime; 
            _repairTimer.ChangeRepairTime(repairTime); 
        }
        
        public void Repair(int value)
        {
            if (value < 1)
            {
                return;
            }
            ChangeDurabilityFor(value);
        }

        private void ChangeDurabilityFor(int value)
        {
            _durabilityValue.ChangeValueFor(value);
            FunctionalityCheck();
            UpdateVisualView();
        }

        private void UpdateVisualView()
        {
            _textMeshPro.text = $" {_durabilityValue.CurrentValue}/{_durabilityValue.MaxValue} ";
            _textMeshPro.text += Functional ? "" : "Out of order! ";
            _textMeshPro.text += _enoughCrewForRepair ? "" : "Сan't be repair ";
            _textMeshPro.color = _enoughCrewForRepair ? Color.white : new Color(1f, 0.4f, 0, 1);
            _textMeshPro.color = Functional ? _textMeshPro.color : Color.red;
            
            
            if (!Functional || !_enoughCrewForRepair)
            {
                _warningRenderer.gameObject.SetActive(true);
            }
            else
            {
                _warningRenderer.gameObject.SetActive(false); 
            }
            _slider.value = _durabilityValue.CurrentValue;
        }

        public void Damage(int value)
        {
            value = value > _durabilityValue.CurrentValue ? _durabilityValue.CurrentValue : value;

            if (value < 1)
            {
                return;
            }

            DamageHighlight().Forget();
            _durabilitySignals.DurabilityDamagedInvoke(value);
            ChangeDurabilityFor(-value);
            Repair();
        }

        private async UniTask DamageHighlight()
        {
            _damageSpriteRenderer.color = Color.red;
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            _damageSpriteRenderer.color = _defaultColor;
        }

        private void FunctionalityCheck()
        {
            if (_durabilityValue.CurrentValue >= _minDurabilityRequired != Functional)
            {
                Functional = !Functional;
                FunctionalityChange.Invoke();
            }
        }
    }
}