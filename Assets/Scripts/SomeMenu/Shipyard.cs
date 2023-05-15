using System;
using System.Collections.Generic;
using LootSettings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Shipyard
    {
        private BaseShip _playerShip;
        private MenuButtons _menuButtons;

        private List<ButtonWithText> _upgradeButtons;
        private event Action EndAction;

        public Shipyard(BaseShip playerShip, MenuButtons menuButtons, Action endAction)
        {
            _upgradeButtons = new List<ButtonWithText>();
            _playerShip = playerShip;
            _menuButtons = menuButtons;
            EndAction = endAction;
        }

        
        public void ShowShipOptionsShipyard()
        {
            int crew = Random.Range(5, 15);
            int recoverability = Random.Range(10, 20);
            
            _playerShip.ShipCrewHandler.RecoverInjuredCrew();
            _playerShip.ShipCrewHandler.AddNewCrew(crew);
            _playerShip.ShipSurvivability.AddRecoverability(recoverability);
            
            UpgradeDeckDurability();
            UpgradeDeckCrew();
            UpgradeDeckArmor();
            
            //var testEvents = new TestSeaEvents(_playerShip, _menuButtons, EndAction);
            //_menuButtons.AddButton($"Test events", testEvents.ShowEventsOptions);
            
            _menuButtons.AddButton("Leave shipyard" , () =>
            {
                EndAction?.Invoke();
            });
            
            string menuInfo = "You returned to shipyard. " +
                $"\nYou receive new crew members [+{crew} Crew]. Engineers begin repairs [+{recoverability} Recoverability]." +
                $"\nYou have time for some upgrades if have upgrades point";
            _menuButtons.ShowMenu(menuInfo);
        }

        private void UpgradeDeckDurability()
        {
            RandomDeckDurabilityUpgrade upgrade = new RandomDeckDurabilityUpgrade(_playerShip);
            CreateUpgradeButton(upgrade, "Your durability upgraded.");
        }

        private void UpgradeDeckCrew()
        {
            RandomDeckCrewUpgrade upgrade = new RandomDeckCrewUpgrade(_playerShip);
            CreateUpgradeButton(upgrade, "You have more place for crew.");
        }

        private void UpgradeDeckArmor()
        {
            RandomDeckArmorUpgrade upgrade = new RandomDeckArmorUpgrade(_playerShip);
            CreateUpgradeButton(upgrade, "Your armor upgraded.");
        }

        
        private void SpendUpgradePoint()
        {
            _playerShip.ShipPrestigeHandler.SpendUpgradePoint();
            UpdateUpgradeButtons();
        }
        
        private void UpdateUpgradeButtons()
        {
            foreach (var button in _upgradeButtons)
            {
                if (button.Interactable)
                {
                    button.SetInteractable(_playerShip.ShipPrestigeHandler.UpgradePoint >0);
                }
            }
        }
        
        private void CreateUpgradeButton(ILootButtonSetting lootButtonSetting, string logText)
        {
            var action = lootButtonSetting.GetAction();
            action += SpendUpgradePoint;
            action += () => _menuButtons.AddTextToMenu(logText);
            var button = _menuButtons.AddButtonInteractable(lootButtonSetting.GetDescription(), action, _playerShip.ShipPrestigeHandler.UpgradePoint >0);
            _upgradeButtons.Add(button);
        }
    }
}