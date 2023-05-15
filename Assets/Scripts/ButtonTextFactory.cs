using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ButtonTextFactory : MonoBehaviour
    {
        [SerializeField]
        private ButtonWithText _buttonTextPrefab;
        private ObjectPool<ButtonWithText> _buttonPool;

        private void Awake()
        {
            _buttonPool = new ObjectPool<ButtonWithText>(InstantiateButton, TurnButtonOn, TurnButtonOff, 5, true);
        }

        private ButtonWithText InstantiateButton()
        {
            var button = Instantiate(_buttonTextPrefab, transform);
            button.transform.position = Vector3.up * 20;
            button.RemoveEvent += BackToPool;
            return button;
        }

        private void TurnButtonOff(ButtonWithText button)
        {
            button.CleanButton();
            button.gameObject.SetActive(false);
        }
        private void TurnButtonOn(ButtonWithText bombshell)
        {
            bombshell.gameObject.SetActive(true);
        }

        public ButtonWithText CreateButton(string text, Action action)
        {
            var button = _buttonPool.GetObject();
            button.SetupButton(text, action);
            return button;
        }
        
        public ButtonWithText CreateButton()
        {
            var button = _buttonPool.GetObject();
            return button;
        }

        private void BackToPool(ButtonWithText button)
        {
            if (button == null)
            {
                return;
            }
            button.transform.SetParent(transform);
            button.transform.position = Vector3.up * 20;
            _buttonPool.ReturnObject(button);
        }
    }


    

}