using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ButtonWithText : MonoBehaviour
    {
        public Action<ButtonWithText> RemoveEvent = delegate { };
        [SerializeField]
        private Button _button;
        [SerializeField]
        private TextMeshProUGUI _textMeshPro;
        public bool Interactable => _button.interactable;

        private Color _defaultTextColor;
        private void Awake()
        {
            _defaultTextColor = _textMeshPro.color;
        }
        public void CleanButton()
        {
            SetInteractable(true);
            _textMeshPro.text = null;
            _button.onClick.RemoveAllListeners();
        }

        public void SetupButton(string text, Action action)
        {
            _textMeshPro.text = text;
            if (action != null)
            {
                _button.onClick.AddListener(action.Invoke);
            }
        }

        public void SetInteractable(bool value)
        {
            _textMeshPro.color = value ? _defaultTextColor : new Color(_defaultTextColor.r, _defaultTextColor.g, _defaultTextColor.b, 0.5f);
            _button.interactable = value;
        }
        public void ReturnToPool()
        {
            RemoveEvent.Invoke(this);
        }
    }
}