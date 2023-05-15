using System;
using DefaultNamespace;

namespace SomeMenu
{
    public class SosDamagedShipEvent : IMenuEvent
    {
        private BaseShip _playerShip;
        private MenuButtons _menuButtons;
        private event Action EndAction;
        private int _recoverabilityValue;
        private int _savedCrewValue;
        private int _repairSkillRequired;

        public SosDamagedShipEvent(BaseShip playerShip, MenuButtons menuButtons, Action endAction)
        {
            _recoverabilityValue = UnityEngine.Random.Range(10, 21);
            _savedCrewValue = UnityEngine.Random.Range(10, 21);
            _playerShip = playerShip;
            _menuButtons = menuButtons;
            EndAction = endAction;
            _repairSkillRequired = UnityEngine.Random.Range(2, 6);
        }

        public void BeginEvent()
        {
            EndEvent endEvent = new EndEvent(_menuButtons, EndAction);
            
            _menuButtons.AddButtonInteractable($"Fight for survivability \nRequired repair value: {_playerShip.RepairValue()}/{_repairSkillRequired}", () =>
            {
                _playerShip.ShipPrestigeHandler.AddPrestige(5);
                _playerShip.ShellsHandler.AddShell(ShellType.Shrapnel, UnityEngine.Random.Range(1, 6));
                _playerShip.ShellsHandler.AddShell(ShellType.HighExplosive, UnityEngine.Random.Range(1, 6));
                _playerShip.ShellsHandler.AddShell(ShellType.ArmorPiercing, UnityEngine.Random.Range(1, 6));
                endEvent.BeginEvent("You successfully repair damaged ship. Your prestige increased. Also, the saved crew gave you some of their shells.", "Leave");

            }, _playerShip.RepairValue() >= _repairSkillRequired);

            _menuButtons.AddButton($"Evacuate the crew. \n[Add {_savedCrewValue} crew]", () =>
            {
                _playerShip.ShipCrewHandler.AddNewCrew(_savedCrewValue);
                _playerShip.ShipPrestigeHandler.AddPrestige(2);
                endEvent.BeginEvent("You take all people on your ship and deliver them to nearby city. Some of the them joined your crew. Your prestige increased.", "Leave");
            });

            _menuButtons.AddButton($"Steal materials. \n[Add {_recoverabilityValue} Recoverability]", () =>
            {
                _playerShip.ShipSurvivability.AddRecoverability(_recoverabilityValue);
                endEvent.BeginEvent("You give drowning people some of your life saving boats, but don't take them to your ship. Your engineers try to collect something useful from sinking ship.", "Leave");
            });

            _menuButtons.AddButton("Leave", () =>
            {
                EndAction?.Invoke();
            });


            string menuInfo = "You see damaged ship.";
            _menuButtons.ShowMenu(menuInfo);
        }
    }
}