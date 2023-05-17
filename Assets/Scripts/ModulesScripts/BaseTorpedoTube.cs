using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using InterfaceProviders;
using ShipCharacteristics;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ModulesScripts
{
    public class BaseTorpedoTube: ShipModule, ITargetRequired, ICurrentDetectionRequired, IAfterBattleTurnOff
    {
        [SerializeField]
        private TextMeshPro _timerText;
        [SerializeField, Range(0.5f, 15f)]
        private float _reloadTime;
        [SerializeField, Range(0.5f, 15f)]
        private float _aiming;
        [SerializeField]
        private Torpedo _torpedo;
      
        private float missChancePerManeuverability = 7.5f;

        private BaseShip _shipTarget;
        private DetectionHandler _thisShipDetectionHandler;
        
        private bool _inBattle;
        private CancellationTokenSource _fireCTS;

        
        public void SetShipTarget(BaseShip targetShip)
        {
            _shipTarget = targetShip;
            _inBattle = true;
            SetActive(IsInOrder);
        }

        public void SetDetection(DetectionHandler detectionHandler)
        {
            _thisShipDetectionHandler = detectionHandler;
        }

        private async UniTask ReloadAsync(CancellationToken token)
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
            float targetingDifficulty = _shipTarget.ManeuverabilityValue() - _thisShipDetectionHandler.Detection;
            targetingDifficulty = targetingDifficulty < 0 ? 0 : targetingDifficulty;
            
            float targetingTime = _aiming * (1 + targetingDifficulty * 0.1f);
            targetingTime = hasCommanderOnDuty ? targetingTime / 2 : targetingTime;
            await TimerAsync("Targeting", targetingTime, token);
            if (!token.IsCancellationRequested)
            {
                Fire(token).Forget();
            }
        }

        private async UniTask Fire(CancellationToken token)
        {
            if (_shipTarget == null)
            {
                Debug.Log("target zero");
            }
            TorpedoFactory.Instance.TorpedoLaunch(_torpedo, _shipTarget, transform.position);
            _timerText.color = new Color(1f, 0.5f, 0f, 1);
            _timerText.text = "Launch!";
            await UniTask.Delay(TimeSpan.FromSeconds(0.25f));
            _timerText.color = Color.white;
            _timerText.text = "";
            if (!token.IsCancellationRequested)
            {
                ReloadAsync(token).Forget();
            }
        }

        private async UniTask TimerAsync(string timerName, float time, CancellationToken token)
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
            string description = $"Torpedo. \n" +
                $"{_reloadTime}+{_aiming} min shoot Time. " /*+
                $"{GetBaseDescription()} "*/;
            moduleDescription.SetDescriptionText(description);
        }

        public override void SetActive(bool value)
        {
            IsInOrder = value;
            UpdateDescription();

            if (!_inBattle || _shipTarget == null)
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
                _fireCTS?.Cancel();
                _timerText.text = "out of order";
            }
        }

        public void BattleEnd()
        {
            _fireCTS?.Cancel();
            _inBattle = false;
        }
        
        private void OnDisable()
        {
            SetActive(false);
        }
    }
}