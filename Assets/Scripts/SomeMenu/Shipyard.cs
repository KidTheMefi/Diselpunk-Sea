using System;
using LootSettings;
using UnityEngine;

namespace DefaultNamespace
{
    public class Shipyard
    {
        public event Action LeaveShipYard = delegate { };
        private BaseShip _playerShip;
        private MenuButtons _menuButtons;
        
        public Shipyard(BaseShip playerShip, MenuButtons menuButtons)
        {
            _playerShip = playerShip;
            _menuButtons = menuButtons;
        }
        
        public void ShowShipOptionsShipyard()
        {
            _playerShip.ShipCrewHandler.RecoverInjuredCrew();
        
            UpgradeDeckDurability();
            UpgradeDeckCrew();
            UpgradeDeckArmor();
            _menuButtons.ShowMenu(true);
        }
    
        private void UpgradeDeckDurability()
        {
            RandomDeckDurabilityUpgrade upgrade = new RandomDeckDurabilityUpgrade(_playerShip);
            CreateButton(upgrade);
        }

        private void UpgradeDeckCrew()
        {
            RandomDeckCrewUpgrade upgrade = new RandomDeckCrewUpgrade(_playerShip);
            CreateButton(upgrade);
        }

        private void UpgradeDeckArmor()
        {
            RandomDeckArmorUpgrade upgrade = new RandomDeckArmorUpgrade(_playerShip);
            CreateButton(upgrade);
        }

        private void CreateButton(ILootButtonSetting lootButtonSetting)
        {
            var action = lootButtonSetting.GetAction();
            action += LeaveShipYard.Invoke;
            _menuButtons.AddLootButton(lootButtonSetting.GetDescription(), action);
        }
    }
}