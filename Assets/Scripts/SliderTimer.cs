using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SliderTimer : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;

        private float _currentTime;
        private CancellationToken _token = CancellationToken.None;
        private void Awake()
        {
            ResetTimer();
        }
        private void ResetTimer()
        {
            _slider.minValue = 0;
            _slider.maxValue = 1;
            _slider.value = 1;
        }
        public async UniTask TimerAsync(float time, CancellationToken token)
        {
            _token = token;
            time = time < 0 ? 0 : time;
            _currentTime = time;
            _slider.minValue = 0;
            _slider.maxValue = _currentTime;
            _slider.value = _currentTime;

            await UniTask.WaitUntil(() =>
            {
                _currentTime -= Time.deltaTime;
                _slider.value = _currentTime;

                return _currentTime <= 0 || token.IsCancellationRequested;
            });
            ResetTimer();
        }

        public void ChangeRepairTime(float newTime)
        {

            if (_token == CancellationToken.None || _token.IsCancellationRequested)
            {
                return;
            }

            float newSliderValue = _slider.value * newTime / _slider.maxValue;
            _currentTime = newSliderValue;
            _slider.maxValue = newTime;

        }
    }
}