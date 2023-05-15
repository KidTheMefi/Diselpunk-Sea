using System;
using TMPro;
using UnityEngine;

namespace ShipCharacteristics
{
    public class ShipPrestigeHandler : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro _textMeshPro;

        private int _shipPrestigeValue;
        private int _shipUpgradePoint;

        public int UpgradePoint => _shipUpgradePoint;
        public int Prestige => _shipPrestigeValue;
        
        public void SetPrestige(int value)
        {
            value = value > 0 ? value : 0;
            _shipPrestigeValue = value;
            UpdateText();
        }
        
        public void AddPrestige(int value)
        {
            int upgradePointPrestigeRequired = 0;
            for (int i = 1; i < 6; i++)
            {
                upgradePointPrestigeRequired +=  2 * i;
                if (_shipPrestigeValue < upgradePointPrestigeRequired && _shipPrestigeValue + value >= upgradePointPrestigeRequired)
                {
                    _shipUpgradePoint++;
                }
            }
            _shipPrestigeValue += value;
            UpdateText();
        }
        private void UpdateText()
        {
            _textMeshPro.text = $"Prestige: {_shipPrestigeValue}. ";
            _textMeshPro.text += _shipUpgradePoint > 0 ? $"Upgrade point: {_shipUpgradePoint}" : "";
        }

        public void SpendUpgradePoint()
        {
            _shipUpgradePoint = _shipUpgradePoint - 1 > 0 ? _shipUpgradePoint - 1 : 0;
            UpdateText();
        }
    }
}