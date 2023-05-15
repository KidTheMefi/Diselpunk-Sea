using System;
using SomeMenu;
using UnityEngine;
using Random = UnityEngine.Random;

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
            //var shipyardOption = new SosDamagedShipEvent(_playerShip, _menuButtons, EndAction);
            //var shipyardOption = new SosDiseaseShipEvent(_playerShip, _menuButtons, EndAction);

            IMenuEvent shipyardOption = Random.Range(0, 3) switch
            {
                0 => new SosAmbushEvent(_playerShip, _menuButtons),
                1 => new SosDiseaseShipEvent(_playerShip, _menuButtons, EndAction),
                _ => new SosDamagedShipEvent(_playerShip, _menuButtons, EndAction)
            };
            
            _menuButtons.AddButton($"Set a course on the signal.", shipyardOption.BeginEvent);
            _menuButtons.AddButton("Ignore signal" , () =>
            {
                EndAction?.Invoke();
            });
            
            
            string menuInfo = "You receive S.O.S. signal";
            _menuButtons.ShowMenu(menuInfo);
        }
    }
}