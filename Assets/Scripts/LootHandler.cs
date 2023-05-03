using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class LootHandler : MonoBehaviour
    {
        public event Action LootEnd = delegate { };
        private event Action _endAction;
        [SerializeField]
        private GameObject backGround;
        [SerializeField]
        private ButtonTextFactory _buttonTextFactory;
        [SerializeField]
        private RectTransform _rectTransform;

        private List<ButtonWithText> _buttons = new List<ButtonWithText>();

        public void AddLootButton(string text, Action action, bool lootInvoke = true)
        {
            void OnButtonClick()
            {
                RemoveButtons();
                ShowMenu(false);
                action.Invoke();
                if (lootInvoke)
                {
                    LootEnd.Invoke();
                }
                _endAction?.Invoke();
            }

            var button = _buttonTextFactory.CreateButton(text, OnButtonClick);
            button.transform.SetParent(transform);
            _buttons.Add(button);
        }


        public void BeginSelecting(Action endAction)
        {
            _endAction = endAction;
            ShowMenu(true);
        }

        private void ShowMenu(bool value)
        {
            _rectTransform.sizeDelta = new Vector2(1 + _buttons.Count * 2, 2);
            backGround.SetActive(value);
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