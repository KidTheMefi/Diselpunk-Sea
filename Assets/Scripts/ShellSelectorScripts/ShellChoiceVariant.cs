using System;
using DefaultNamespace;
using UnityEngine;

namespace ShellSelectorScripts
{
    public class ShellChoiceVariant : MonoBehaviour
    {
        public event Action<Vector3> OnMouseEnterEvent = delegate(Vector3 vector3) { };
        public event Action<ShellChoiceVariant> OnMouseDownEvent = delegate(ShellChoiceVariant ShellVariant) { };

        [SerializeField]
        private ShellType _shellType;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        public ShellType ShellType => _shellType;
        public Sprite Sprite => _spriteRenderer.sprite;
        private void OnMouseEnter()
        {
            OnMouseEnterEvent.Invoke(transform.position);
        }

        private void OnMouseDown()
        {
            OnMouseDownEvent.Invoke(this);
        }
    }
}