using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace ShellSelectorScripts
{
    public class ShellSelector : MonoBehaviour
    {
        public event Action<ShellType> ShellTypeSelected = delegate(ShellType type) { };
        
        [SerializeField]
        private List<ShellChoiceVariant> _shellChoiceVariants;
        [SerializeField]
        private GameObject _highlightObject;
        [SerializeField]
        private GameObject _selectorMenu;
        [SerializeField]
        private SpriteRenderer _currentShellRenderer;

        private Sprite _defaultSprite;


        private bool _open;

        public void SetDefaultShell()
        {
            _currentShellRenderer.sprite = _defaultSprite;
        }
        
        private void Awake()
        {
            _defaultSprite = _currentShellRenderer.sprite;
            foreach (var shellVariant in _shellChoiceVariants)
            {
                shellVariant.OnMouseDownEvent += ShellVariantOnOnMouseDownEvent;
                shellVariant.OnMouseEnterEvent += ShellVariantOnMouseEnterEvent;
            }
        }
        private void ShellVariantOnMouseEnterEvent(Vector3 obj)
        {
            _highlightObject.SetActive(true);
            _highlightObject.transform.position = obj;
        }
        private void ShellVariantOnOnMouseDownEvent(ShellChoiceVariant variant)
        {
            
            _currentShellRenderer.sprite = variant.Sprite;
            _selectorMenu.SetActive(false);
            _highlightObject.SetActive(false);
            _open = false;
            ShellTypeSelected.Invoke(variant.ShellType);
        }
        private void OnMouseDown()
        {
            if (_open)
            {
                _selectorMenu.SetActive(false);
                _highlightObject.SetActive(false);
            }
            else
            {
                _selectorMenu.SetActive(true);
            }

            _open = !_open;
        }

        private void OnDestroy()
        {
            foreach (var shellVariant in _shellChoiceVariants)
            {
                shellVariant.OnMouseDownEvent -= ShellVariantOnOnMouseDownEvent;
                shellVariant.OnMouseEnterEvent -= ShellVariantOnMouseEnterEvent;
            }
        }
    }
}