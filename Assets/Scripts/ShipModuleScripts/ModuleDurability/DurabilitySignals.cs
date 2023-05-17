using System;
using ShipCharacteristics;

namespace ShipModuleScripts.ModuleDurability
{
    public class DurabilitySignals
    {
        public event Action<int> DurabilityDamaged = delegate(int i) { };
        public event Action<DurabilityHandler, int> RequestRepair = delegate(DurabilityHandler crewModuleHandler, int i) { };
        public bool HasRecoverabilityPoint { get; private set; }

        public int GetRepairSkill()
        {
            return _repairSkillHandler.RepairSkill;
        }

        private readonly RepairSkillHandler _repairSkillHandler;

        public  DurabilitySignals(RepairSkillHandler repairSkillHandler)
        {
            _repairSkillHandler = repairSkillHandler;

        }
        
        public void DurabilityDamagedInvoke(int value)
        {
            DurabilityDamaged.Invoke(value);
        }
        public void RequestRepairInvoke(DurabilityHandler durabilityHandler, int value)
        {
            RequestRepair.Invoke(durabilityHandler, value);
        }

        public void HaveRecoverabilityPoint(bool value)
        {
            HasRecoverabilityPoint = value;
        }
    }
}