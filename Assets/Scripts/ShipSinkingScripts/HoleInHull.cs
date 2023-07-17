using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ShipCharacteristics;
using UnityEngine;

namespace ShipSinkingScripts
{
    public class HoleInHull : MonoBehaviour
    {
        public event Action<int> WaterLevelIncrease = delegate { };
        public event Action<ShipModulePlace> HoleRepairedAction = delegate { };
        public event Action<HoleInHull> RemoveEvent = delegate { };

        private CancellationTokenSource _cancellationTokenSource;
        private ShipModulePlace _shipModulePlace;
        [SerializeField]
        private SpriteRenderer _holeRenderer;
        [SerializeField]
        private SpriteRenderer _holeLevelSpriteRenderer;
        private RepairSkillHandler _repairSkillHandler;

        private int _holeSize;

        readonly System.Random _random = new System.Random();


        public void SinkingBegin(ShipModulePlace shipModulePlace, RepairSkillHandler repairSkillHandler, int sizeValue)
        {
            if (shipModulePlace.StatusIconsPlace == null)
            {
                Debug.Log("Place null");
                return;
            }
            
            //transform.position = shipModulePlace.StatusIconsPlace.transform.position;
            transform.SetParent(shipModulePlace.StatusIconsPlace.transform);
            transform.localPosition = Vector3.left * 0.3f;
            _repairSkillHandler = repairSkillHandler;
            _shipModulePlace = shipModulePlace;
            IncreaseHoleSize(sizeValue);
            _cancellationTokenSource = new CancellationTokenSource();
            OnSinkingAsync(_cancellationTokenSource.Token).Forget();
            ShowHoleSize(_holeSize);
            gameObject.SetActive(true);
        }

        public void IncreaseHoleSize(int value)
        {
            if (value > 0)
            {
                _holeSize += value * 10;
            }
            ShowHoleSize(_holeSize);
        }

        private void ShowHoleSize(int lvl)
        {
            _holeLevelSpriteRenderer.size = new Vector2(1.5f + lvl * 1f / 100, 1.5f + lvl * 1f / 100);
        }


        private async UniTask OnSinkingAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

            WaterLevelIncrease.Invoke(Mathf.CeilToInt(_holeSize/10f));
            WaterVisual().Forget();
            HoleSizeChangePerSecond();

            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                OnSinkingAsync(token).Forget();
            }
        }

        private async UniTask WaterVisual()
        {
            _holeRenderer.color = Color.red;
            await UniTask.Delay(TimeSpan.FromSeconds(0.2), cancellationToken: _cancellationTokenSource.Token);
            _holeRenderer.color = Color.white;
        }

        private void HoleSizeChangePerSecond()
        {
            int holeSizeChange = _shipModulePlace.CrewHandler.CrewValue.CurrentValue + _repairSkillHandler.RepairSkill;
            holeSizeChange = _shipModulePlace.IsCommanderOnPost ? holeSizeChange * 2 : holeSizeChange; // !!!! Or add hole repair skill to commander; !!!

            _holeSize -= holeSizeChange;
            ShowHoleSize(_holeSize);
            if (_holeSize <= 0)
            {
                HoleRepaired();
            }
        }

        private void HoleRepaired()
        {
            _cancellationTokenSource?.Cancel();
            _holeSize = 0;
            gameObject.SetActive(false);
            _holeRenderer.color = Color.white;
            HoleRepairedAction.Invoke(_shipModulePlace);
        }

        public void Remove()
        {
            RemoveEvent.Invoke(this);
        }
        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}