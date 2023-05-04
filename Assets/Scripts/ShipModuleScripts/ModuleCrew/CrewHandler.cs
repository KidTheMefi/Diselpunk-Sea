using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShipModuleScripts.ModuleCrew
{
    public class CrewHandler : MonoBehaviour
    {
        public event Action FunctionalityChange = delegate { };
        [SerializeField]
        private Toggle _crewRestore;
        [SerializeField]
        private TextMeshPro _textMeshPro;
        [SerializeField]
        private SpriteRenderer _damageSpriteRenderer;
        [SerializeField]
        private ValueVisualView _valueVisualView;
        [SerializeField]
        private SpriteRenderer _warningRenderer;
        private CancellationTokenSource _crewRestoreCTS;
        private int _minCrewRequired = 3;
        private bool _canBeRestored;
        private CrewSignals _crewSignals;
        public bool Functional { get; private set; }
        private IntValue _crewValue = new IntValue(10);
        public IntValue CrewValue => _crewValue;
        private float crewRestorationTime = 3f;
        private Color _defaultColor;


        public void Setup(IntValue crewValue, int minCrewRequired)
        {
            _minCrewRequired = minCrewRequired;
            _crewValue = crewValue;
            _defaultColor = _damageSpriteRenderer.color;
            _crewRestore.onValueChanged.AddListener(CrewChangeRestore);
            SetupSlider();
        }
        
        public void SetSignals(CrewSignals crewSignals)
        {
            _crewSignals = crewSignals;
        }

        public void Begin()
        { 
            FunctionalityCheck();
            CanBeRestored(true);
            UpdateVisualView();
            SetupSlider();
        }

        private void SetupSlider()
        {
            _valueVisualView.Setup(_minCrewRequired);
            _valueVisualView.UpdateVisualPoints(_crewValue);
        }

        public void ChangeToggle(bool value)
        {
            CrewChangeRestore(value);
            _crewRestore.isOn = value;
        }
    
        private void CanBeRestored(bool value)
        {
            if (value)
            {
                _canBeRestored = true;
                CrewChangeRestore(_crewRestore.isOn);
                _crewRestore.image.color = Color.white;
            
            }
            else
            {
                _crewRestore.isOn = false;
                _canBeRestored = false;
                _crewRestore.image.color = new Color(0.3f, 0, 0, 1);
            }
        }


        private void CrewChangeRestore(bool value)
        {
            if (value)
            {
                _crewRestoreCTS?.Cancel();
                _crewRestoreCTS = new CancellationTokenSource();
                CrewRestoreAsync(_crewRestoreCTS.Token).Forget();
            }
            else
            {
                _crewRestoreCTS?.Cancel();
            }
        }

        private async UniTask CrewRestoreAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _canBeRestored)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(crewRestorationTime), cancellationToken: token);
                if (token.IsCancellationRequested || !_canBeRestored)
                {
                    return;
                }
                if (!_crewValue.FullValue)
                {
                    //Debug.Log($"{gameObject.name} {_crewValue.CurrentValue}/{_crewValue.MaxValue} required crew from reserve");
                    _crewSignals.RequestCrewMembersFromReserveInvoke(this, 1);
                }
            }
        }

        public void AddCrew(int value)
        {
            if (value < 1)
            {
                return;
            }
            ChangeCrewFor(value);
        }

        private void ChangeCrewFor(int value)
        {
            _crewValue.ChangeValueFor(value);
            FunctionalityCheck();
            UpdateVisualView();
        }

        private void UpdateVisualView()
        {
            _textMeshPro.text = $"{_crewValue.CurrentValue}/{_crewValue.MaxValue}";
            _textMeshPro.text += Functional ? "" : " not enough man!";
            _textMeshPro.color = Functional ? Color.white : Color.red;
            
            _warningRenderer.enabled = !Functional;
            
            if (_valueVisualView.awaken)
            {
                _valueVisualView.UpdateVisualPoints(_crewValue);
            }
        }

        public void DamageCrew(int value)
        {
            value = value > _crewValue.CurrentValue ? _crewValue.CurrentValue : value;

            if (value < 1)
            {
                return;
            }
            CrewChangeRestore(_crewRestore.isOn);
            DamageHighlight().Forget();
            _crewSignals.CrewMembersDamagedInvoke(value);
            ChangeCrewFor(-value);
        }

        private async UniTask DamageHighlight()
        {
            _damageSpriteRenderer.color = Color.red;
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: _crewRestoreCTS.Token);
            _damageSpriteRenderer.color = _defaultColor;
        }

        public void SendCrewToReserve()
        {
            CanBeRestored(false);
            _crewSignals.CrewMembersGoToReserveInvoke(_crewValue.CurrentValue);
            ChangeCrewFor(-_crewValue.CurrentValue);
        }

        private void FunctionalityCheck()
        {
            if (_crewValue.CurrentValue >= _minCrewRequired != Functional)
            {
                
                Functional = !Functional;
                FunctionalityChange.Invoke();
            }
        }
        private void OnDestroy()
        {
            _crewRestoreCTS?.Cancel();
        }
    }
}