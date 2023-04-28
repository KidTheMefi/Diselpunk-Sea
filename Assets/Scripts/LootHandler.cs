using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class LootHandler : MonoBehaviour
    {
        public event Action LootEnd = delegate { };

        [SerializeField]
        private GameObject backGround;
        [SerializeField]
        private ButtonTextFactory _buttonTextFactory;

        private List<ButtonWithText> _buttons = new List<ButtonWithText>();
        
        public void AddLootButton(string text, Action action)
        {
            void OnButtonClick()
            {
                action.Invoke();
                if (_buttons != null)
                {
                    foreach (var button in _buttons)
                    {
                        button.ReturnToPool();
                    }
                }
                LootEnd.Invoke();
                ShowLoot(false);
            }

           var button =  _buttonTextFactory.CreateButton(text, OnButtonClick);
           button.transform.SetParent(transform);
           _buttons.Add(button);
        }

        public void ShowLoot(bool value)
        {
            backGround.SetActive(value);
        }
    }
}