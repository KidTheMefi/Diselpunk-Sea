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

        public void CleanButton()
        {
            _textMeshPro.text = null;
            _button.onClick.RemoveAllListeners();
        }

        public void SetupButton(string text, Action action)
        {
            _textMeshPro.text = text;
            _button.onClick.AddListener(action.Invoke);
        }

        public void ReturnToPool()
        {
            RemoveEvent.Invoke(this);
        }
    }
}