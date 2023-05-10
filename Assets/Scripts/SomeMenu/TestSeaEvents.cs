using System;
using LootSettings;
using UnityEngine;

namespace DefaultNamespace
{
    public class TestSeaEvents
    {
        private BaseShip _playerShip;
        private MenuButtons _menuButtons;
        private event Action EndAction;

        public TestSeaEvents(BaseShip playerShip, MenuButtons menuButtons, Action endAction)
        {
            _playerShip = playerShip;
            _menuButtons = menuButtons;
            EndAction = endAction;
        }
        
        public void ShowEventsOptions()
        {
            var shipyardOption = new Shipyard(_playerShip, _menuButtons, EndAction);
            _menuButtons.AddButton($"Back to shipyard", shipyardOption.ShowShipOptionsShipyard);

            _menuButtons.AddButton("Return" , () =>
            {
                EndAction?.Invoke();
            });
            
            var continueOption = new TestSeaEvents(_playerShip, _menuButtons, EndAction);
            _menuButtons.AddButton("DebugLog()" , () =>
            {
                Debug.Log("Debug.Log");
                continueOption.ShowEventsOptions();
            });
            
            string menuInfo = "Test Event";
            _menuButtons.ShowMenu(menuInfo);
        }
    }
}