using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class MenuButtons : MonoBehaviour
    {
        [SerializeField]
        private GameObject backGround;
        [SerializeField]
        private ButtonTextFactory _buttonTextFactory;
        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private TextMeshProUGUI _menuInfoTMPro;

        private List<ButtonWithText> _buttons = new List<ButtonWithText>();

        public void AddButton(string text, Action action)
        {
            void OnButtonClick()
            {
                RemoveButtons();
                HideMenu();
                action.Invoke();
            }

            var button = _buttonTextFactory.CreateButton(text, OnButtonClick);
            button.transform.SetParent(transform);
            _buttons.Add(button);
        }

        public void ShowMenu(string menuInfo)
        {
            _menuInfoTMPro.text = menuInfo;
            _rectTransform.sizeDelta = new Vector2(1 + _buttons.Count * 2, _rectTransform.sizeDelta.y);
            backGround.SetActive(true);
        }

        private void HideMenu()
        {
            backGround.SetActive(false);
        }


        private void RemoveButtons()
        {
            if (_buttons != null)
            {
                foreach (var button in _buttons)
                {
                    button.ReturnToPool();
                }
                _buttons.Clear();
            }
        }
    }
}