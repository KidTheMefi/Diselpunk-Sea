using System;
using DefaultNamespace;

namespace LootSettings
{
    public class RandomShellsLoot : ILootButtonSetting
    {
        private Action _action = delegate { };
        private string _description = "";

        public RandomShellsLoot(BaseShip playerShip)
        {
            int shrapnelShells = UnityEngine.Random.Range(-1, 5);
            if (shrapnelShells > 0)
            {
                _description += $"Add {shrapnelShells} shrapnel shells. \n";
                _action += () =>   playerShip.ShellsHandler.AddShell(ShellType.Shrapnel, shrapnelShells);
            }
            
            int armorPiercing = UnityEngine.Random.Range(-1, 5);
            if (armorPiercing > 0)
            {
                _description += $"Add {armorPiercing} Armor Piercing shells. \n";
                _action += () =>   playerShip.ShellsHandler.AddShell(ShellType.ArmorPiercing, armorPiercing);
            }
         
            int highExplosive = UnityEngine.Random.Range(-1, 5);
            if (highExplosive > 0)
            {
                _description += $"Add {highExplosive} High Explosive shells. \n";
                _action += () =>   playerShip.ShellsHandler.AddShell(ShellType.HighExplosive, highExplosive);
            }

            if (_description == "")
            {
                _description = "Add 10 Recoverability and 10 Crew";
                _action += () =>   playerShip.ShipSurvivability.AddRecoverability(10);
                _action += () =>   playerShip.ShipCrewHandler.AddNewCrew(10);
            }
        }
        

        public Action GetAction()
        {
            return _action;
        }
        public string GetDescription()
        {
            return _description;
        }
    }
}