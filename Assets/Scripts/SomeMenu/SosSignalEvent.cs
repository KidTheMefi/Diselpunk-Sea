using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class SosSignalEvent
    {
        private BaseShip _playerShip;
        private MenuButtons _menuButtons;
        private event Action EndAction;

        public SosSignalEvent(BaseShip playerShip, MenuButtons menuButtons, Action endAction)
        {
            _playerShip = playerShip;
            _menuButtons = menuButtons;
            EndAction = endAction;
        }
        
        public void ShowEventsOptions()
        {
            var shipyardOption = new SosDamagedShipEvent(_playerShip, _menuButtons, EndAction);
            
            _menuButtons.AddButton($"Set a course on the signal.", shipyardOption.ShowEventsOptions);
            _menuButtons.AddButton("Ignore signal" , () =>
            {
                EndAction?.Invoke();
            });
            
            
            string menuInfo = "You receive SOS signal";
            _menuButtons.ShowMenu(menuInfo);
        }
    }
}