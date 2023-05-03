using System;

namespace LootSettings
{
    public interface ILootButtonSetting
    {
        public Action GetAction();
        public string GetDescription();
    }
}