using UnityEngine;

namespace DefaultNamespace
{
    public class ValueBarView : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _currentValueVisual;

        private int _currentValue;
        private int _maxValue;

        public void Setup(int currentValue, int maxValue)
        {
            _maxValue = maxValue;

            SetCurrentValue(currentValue);
        }

        public void SetCurrentValue(int currentValue)
        {
            if (_currentValueVisual == null)
            {
                return;
            }
            _currentValue = currentValue < 0 ? 0 : currentValue > _maxValue ? _maxValue : currentValue;
            float currentSizeInPercent = (float)_currentValue / _maxValue;
            _currentValueVisual.size = new Vector2(currentSizeInPercent * 4, _currentValueVisual.size.y);
        }
    }
}