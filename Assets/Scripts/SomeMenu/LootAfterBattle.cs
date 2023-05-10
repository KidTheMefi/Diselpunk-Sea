

using System;
using LootSettings;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class LootAfterBattle
    {
        public event Action LeaveLoot = delegate { };
        
        private BaseShip _playerShip;
        private BaseShip _enemyShip;
        private MenuButtons _menuButtons;
        
        public LootAfterBattle(BaseShip playerShip, MenuButtons menuButtons)
        {
            _playerShip = playerShip;
            _menuButtons = menuButtons;
        }


        public void SetShipToLoot(BaseShip enemyShip)
        {
            _enemyShip = enemyShip;
        }
        
        public void LootBegin()
        {
            _playerShip.ShipCrewHandler.RecoverInjuredCrew();

            if (_enemyShip == null)
            {
                LeaveLoot.Invoke();
                return;
            }
            
            AddRecoverability();
            AddCrew();
            AddRandomShells();

            _enemyShip.Destroy();
            _menuButtons.ShowMenu("You defeat other ship and have time for some loot");
        }
    
    
        private void AddRecoverability()
        {
            var possibleRecoverability = _enemyShip.ShipSurvivability.SurvivabilityValue + Random.Range(1,6);
            _menuButtons.AddButton($"Add recoverability {possibleRecoverability}",
                () =>
                {
                    _playerShip.ShipSurvivability.AddRecoverability(possibleRecoverability);
                    LeaveLoot.Invoke();
                });
        }

        private void AddCrew()
        {
            var possibleCrewRecruit = _enemyShip.ShipCrewHandler.OnDutyCrewValue / 2 + Random.Range(1,4);
            _menuButtons.AddButton($"Add crew {possibleCrewRecruit}",
                () =>
                {
                    _playerShip.ShipCrewHandler.AddNewCrew(possibleCrewRecruit); 
                    LeaveLoot.Invoke();
                });
        }

        private void AddRandomShells()
        {
            RandomShellsLoot randomShellsLoot = new RandomShellsLoot(_playerShip);
            var action = randomShellsLoot.GetAction();
            action += LeaveLoot.Invoke;
            _menuButtons.AddButton(randomShellsLoot.GetDescription(), action);
        }
        
    }
}