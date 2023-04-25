using System;

namespace InterfaceProviders
{
    public interface IRepairSkillProvider
    {
        public event Action RepairSkillChanged;
        public int GetRepairSkill();
    }
}