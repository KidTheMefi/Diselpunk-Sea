using UnityEngine;

namespace ModulesScripts.ModulesSetup
{
    public abstract class BaseModuleSetup : ScriptableObject
    {
        [SerializeField]
        private Sprite _moduleImage;
        [SerializeField, Range(1,25)]
        private int crewFull;
        [SerializeField, Range(1,25)]
        private int baseDurability;
        [SerializeField, Range(0, 25)]
        private int minCrewRequired;
        [SerializeField, Range(0, 25)]
        private int minDurabilityRequired;
        
        public int MinCrewRequired => minCrewRequired > crewFull ? crewFull : minCrewRequired;
        public int CrewFull => crewFull;
        public int MinDurabilityRequired => minDurabilityRequired > baseDurability ? baseDurability : minDurabilityRequired;
        public int BaseDurability => baseDurability;
        public Sprite ModuleImage =>  _moduleImage;
        
        public abstract ShipModule GetModule();
    }
}