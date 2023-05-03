using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;

namespace ShipCharacteristics
{
    public class ShellsHandler : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro _textMeshPro;

        private int _shrapnelCount;
        private int _armorPiercingCount;
        private int _highExplosiveCount;

        public void AddShell(ShellType shellType, int value)
        {
            if (value < 0)
            {
                return;
            }

            switch (shellType)
            {
                case ShellType.Shrapnel:
                    _shrapnelCount += value;
                    break;
                case ShellType.HighExplosive:
                    _highExplosiveCount += value;
                    break;
                case ShellType.ArmorPiercing:
                    _armorPiercingCount += value;
                    break;
            }
            UpdateView();
        }

        public void SetStartShellValue(int shrapnel, int armorPiercing, int highExplosive)
        {
            _shrapnelCount = shrapnel > 0 ? shrapnel : 0;
            _armorPiercingCount = armorPiercing > 0 ? armorPiercing : 0;
            _highExplosiveCount = highExplosive > 0 ? highExplosive : 0;
            UpdateView();
        }

        public bool HaveShells(ShellType shellType)
        {
            bool canGet;

            switch (shellType)
            {
                default:
                    return true;
                case ShellType.Shrapnel:
                    canGet = _shrapnelCount > 0;
                    break;
                case ShellType.HighExplosive:
                    canGet = _highExplosiveCount > 0;
                    break;
                case ShellType.ArmorPiercing:
                    canGet = _armorPiercingCount > 0;
                    break;
            }
            UpdateView();
            return canGet;
        }
        
        public void SpentShell (ShellType shellType)
        {
            switch (shellType)
            {
                case ShellType.Shrapnel:
                    _shrapnelCount--;
                    _shrapnelCount = _shrapnelCount > 0 ? _shrapnelCount : 0;
                    break;
                case ShellType.HighExplosive:
                    _highExplosiveCount--;
                    _highExplosiveCount = _highExplosiveCount > 0 ? _highExplosiveCount : 0;
                    break;
                case ShellType.ArmorPiercing:
                    _armorPiercingCount--;
                    _armorPiercingCount = _armorPiercingCount > 0 ? _armorPiercingCount : 0;
                    break;
            }
            UpdateView();
        }

        private void UpdateView()
        {
            _textMeshPro.text = $"Shrapnel: {_shrapnelCount}, Armor-Piercing: {_armorPiercingCount}, High-Explosive: {_highExplosiveCount}";
        }
    }
}