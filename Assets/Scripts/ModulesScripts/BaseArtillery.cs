using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using InterfaceProviders;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ModulesScripts
{
    public class BaseArtillery : ShipModule, ITargetRequired, IShipCharacteristicsRequired
    {
        [SerializeField]
        private TextMeshPro _timerText;
        [SerializeField, Range(0.5f, 15f)]
        protected float _reloadTime;
        [SerializeField, Range(0.5f, 15f)]
        protected float _aiming;
        [SerializeField]
        protected Shell _shell;
        [SerializeField]
        private LineRenderer _lineToTargetRenderer;
        [SerializeField]
        private SpriteRenderer _sightSpriteRenderer;

        private BaseShip _shipTarget;
        protected BaseShip _thisShip;
        private bool _waitingForTarget;
        private bool _inBattle;
        private CancellationTokenSource _fireCTS;
        private ShipModulePlace _targetPlace;
        
        public void SetShipTarget(BaseShip targetShip)
        {
            _shipTarget = targetShip;
            _inBattle = true;
            SetActive(IsInOrder);
        }

        public void SetCharacteristics(BaseShip thisShip)
        {
            _thisShip = thisShip;
        }
        public override void SetCommanderOnDuty(bool value)
        {
            base.SetCommanderOnDuty(value);
            //_sightSpriteRenderer.enabled = value;

            if (!value)
            {
                DisableAiming();
            }
        }

        private void DisableAiming()
        {
            _lineToTargetRenderer.enabled = false;
            _targetPlace = null;
            SetWaitingForTarget(false);
        }

        public override void ClickOn()
        {
            //Debug.Log($"{IsInOrder} && {hasCommanderOnDuty} && {!_waitingForTarget}");
            if (IsInOrder && hasCommanderOnDuty && !_waitingForTarget)
            {
                SetWaitingForTarget(true);
            }
        }

        private void SetWaitingForTarget(bool value)
        {
            if (_waitingForTarget == value)
            {
                return;
            }
            if (value)
            {
                _shipTarget.Modules.ShipPlaceSignal.OnShipModulePlaceClick += AimingForModulePlace;
            }
            else
            {
                _shipTarget.Modules.ShipPlaceSignal.OnShipModulePlaceClick -= AimingForModulePlace;
            }
            _sightSpriteRenderer.enabled = value;
            _waitingForTarget = value;
        }
        
        
        private void AimingForModulePlace(ShipModulePlace obj)
        {
            _lineToTargetRenderer.enabled = true;
            _lineToTargetRenderer.useWorldSpace = true;
            _lineToTargetRenderer.SetPositions(new[] { transform.position, obj.transform.position });
            _targetPlace = obj;
            SetWaitingForTarget(false);
        }


        protected virtual async UniTask ReloadAsync(CancellationToken token)
        {
            //_fireCTS = new CancellationTokenSource();
            UpdateDescription();
            await TimerAsync("Reload", _reloadTime, token);
            if (!token.IsCancellationRequested)
            {
                TargetingAsync(token).Forget();
            }
        }

        private async UniTask TargetingAsync(CancellationToken token)
        {
            float targetingDifficulty = _shipTarget.ManeuverabilityValue() - _thisShip.DetectionValue();
            targetingDifficulty = targetingDifficulty < 0 ? 0 : targetingDifficulty;
            float targetingTime = _aiming * (1 + targetingDifficulty * 0.1f);
            await TimerAsync("Targeting", targetingTime, token);
            if (!token.IsCancellationRequested)
            {
                Fire(token).Forget();
            }
        }

        protected virtual async UniTask Fire(CancellationToken token)
        {
            var targetModule = _targetPlace == null ? _shipTarget.Modules.GetRandomModule() : _targetPlace;
            ArtilleryVolleyFactory.Instance.FireBombshell(GetShell(), targetModule, transform.position, WillMiss());

            _timerText.color = new Color(1f, 0.5f, 0f, 1);
            _timerText.text = "FIRE!";
            await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
            _timerText.color = Color.white;
            _timerText.text = "";
            if (!token.IsCancellationRequested)
            {
                ReloadAsync(token).Forget();
            }
        }

        private bool WillMiss()
        {
            var rand = Random.Range(0, 20);
            bool miss = _shipTarget.CanEvade() && rand < _shipTarget.ManeuverabilityValue();
            //Debug.Log($"CanEvade({_shipTarget.CanEvade()}). {rand}/{_shipTarget.ManeuverabilityValue()} = {miss}");
            return miss;
        }

        protected async UniTask TimerAsync(string timerName, float time, CancellationToken token)
        {
            time = time < 0 ? 0 : time;

            string timeS = $"{time:f1}";

            await UniTask.WaitUntil(() =>
            {
                _timerText.text = $"{timerName}: {time:f1}/{timeS} sec.";
                time -= Time.deltaTime;

                return time <= 0 || token.IsCancellationRequested;
            });

            if (token.IsCancellationRequested)
            {
                _timerText.text = "Cancel!";
            }

            //_timerText.text = "";
        }

        private void UpdateDescription()
        {
            string description = $"Artillery. \n" +
                $"{GetShell().BaseDamage} Damage.  " +
                $"{GetShell().BaseCrewDamage} Crew damage.  " +
                $"{GetShell().ArmorPiercingClass} AP class.  " +
                $"{GetShell().ShellType}.  " +
                $"{_reloadTime}+{_aiming} min shoot Time. " /*+
                $"{GetBaseDescription()} "*/;
            moduleDescription.SetDescriptionText(description);
        }

        public override void SetActive(bool value)
        {
            IsInOrder = value;
            UpdateDescription();

            if (!_inBattle)
            {
                return;
            }
            
            if (IsInOrder)
            {
                _fireCTS?.Cancel();
                _fireCTS = new CancellationTokenSource();
                ReloadAsync(_fireCTS.Token).Forget();
            }
            else
            {
                DisableAiming();
                _fireCTS?.Cancel();
                _timerText.text = "out of order";
            }
        }

        protected virtual Shell GetShell()
        {
            return _shell;
        }

        public void BattleEnd()
        {
            DisableAiming();
            _fireCTS?.Cancel();
            _inBattle = false;
        }
        
        private void OnDisable()
        {
            SetActive(false);
        }
    }
}