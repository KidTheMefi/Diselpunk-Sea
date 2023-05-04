using System;
using DefaultNamespace;

namespace LootSettings
{
    public class RandomDeckCrewUpgrade: ILootButtonSetting
    {
        private Action _action = delegate { };
        private string _description;

        public RandomDeckCrewUpgrade(BaseShip playerShip)
        {
            var deck = UsefulStatic.RandomEnumValue<ModuleLocation>();
            _description = $"Hire crew and upgrade {deck} max crew";
            _action += () =>   playerShip.UpgradeDeckCrew(deck);
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