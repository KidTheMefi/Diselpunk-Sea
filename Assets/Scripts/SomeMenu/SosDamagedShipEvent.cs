using System;

namespace DefaultNamespace
{
    public class SosDamagedShipEvent
    {
        private BaseShip _playerShip;
        private MenuButtons _menuButtons;
        private event Action EndAction;

        public SosDamagedShipEvent(BaseShip playerShip, MenuButtons menuButtons, Action endAction)
        {
            _playerShip = playerShip;
            _menuButtons = menuButtons;
            EndAction = endAction;
        }
        
        public void ShowEventsOptions()
        {

            if (_playerShip.RepairValue() > 1)
            {
                _menuButtons.AddButton($"Fight for survivability \nRepairValue{_playerShip.RepairValue()}/{1}" , () =>
                {
                    _playerShip.ShellsHandler.AddShell(ShellType.Shrapnel, 5);
                    _playerShip.ShellsHandler.AddShell(ShellType.HighExplosive, 5);
                    EndAction?.Invoke();
                });
            }
            
            _menuButtons.AddButton($"Evacuate the crew" , () =>
            {
                _playerShip.ShipCrewHandler.AddNewCrew(20);
                EndAction?.Invoke();
            });
            
            _menuButtons.AddButton($"Steal materials" , () =>
            {
                _playerShip.ShipSurvivability.AddRecoverability(20);
                EndAction?.Invoke();
            });
            
            _menuButtons.AddButton("Leave" , () =>
            {
                EndAction?.Invoke();
            });
            
            
            string menuInfo = "You see damaged ship.";
            _menuButtons.ShowMenu(menuInfo);
        }
    }
}