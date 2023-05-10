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

        private event Action EndAction;

        public Shipyard(BaseShip playerShip, MenuButtons menuButtons, Action endAction)
        {
            _playerShip = playerShip;
            _menuButtons = menuButtons;
            EndAction = endAction;
        }

        
        public void ShowShipOptionsShipyard()
        {
            _playerShip.ShipCrewHandler.RecoverInjuredCrew();

            UpgradeDeckDurability();
            UpgradeDeckCrew();
            UpgradeDeckArmor();
            
            var lootAfterOption = new TestSeaEvents(_playerShip, _menuButtons, EndAction);
            _menuButtons.AddButton($"Test events", lootAfterOption.ShowEventsOptions);
            
            string menuInfo = "You returned to shipyard. You have time for some upgrades";
            _menuButtons.ShowMenu(menuInfo);
        }

        private void UpgradeDeckDurability()
        {
            RandomDeckDurabilityUpgrade upgrade = new RandomDeckDurabilityUpgrade(_playerShip);
            CreateButtonWithEndAction(upgrade);
        }

        private void UpgradeDeckCrew()
        {
            RandomDeckCrewUpgrade upgrade = new RandomDeckCrewUpgrade(_playerShip);
            CreateButtonWithEndAction(upgrade);
        }

        private void UpgradeDeckArmor()
        {
            RandomDeckArmorUpgrade upgrade = new RandomDeckArmorUpgrade(_playerShip);
            CreateButtonWithEndAction(upgrade);
        }

        private void CreateButtonWithEndAction(ILootButtonSetting lootButtonSetting)
        {
            var action = lootButtonSetting.GetAction();
            //action += LeaveShipYard.Invoke;

            if (EndAction != null)
            {
                action += EndAction.Invoke;
                action += () => EndAction = null;
            }

            _menuButtons.AddButton(lootButtonSetting.GetDescription(), action);
        }
    }
}