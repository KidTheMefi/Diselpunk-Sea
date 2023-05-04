using System;
using DefaultNamespace;

namespace LootSettings
{
    public class RandomDeckArmorUpgrade : ILootButtonSetting
    {
        private Action _action = delegate { };
        private string _description;

        public RandomDeckArmorUpgrade(BaseShip playerShip)
        {
            var deck = UsefulStatic.RandomEnumValue<ModuleLocation>();
            _description = $"Upgrade {deck} armor";
            _action += () => playerShip.UpgradeDeckArmor(deck);
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