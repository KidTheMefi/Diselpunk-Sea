using ModulesScripts.ModulesSetup;
using TMPro;
using UnityEngine;

namespace ModulesScripts
{
    public abstract class ShipModule : MonoBehaviour
    {
        [SerializeField, Range(1,25)]
        private int crewFull;
        [SerializeField, Range(1,25)]
        private int baseDurability;
        [SerializeField, Range(0, 25)]
        private int minCrewRequired;
        [SerializeField, Range(0, 25)]
        private int minDurabilityRequired;
        [SerializeField]
        protected ModuleDescription moduleDescription;
        protected bool hasCommanderOnDuty = false;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        
        public int MinCrewRequired => minCrewRequired > crewFull ? crewFull : minCrewRequired;
        public int CrewFull => crewFull;
        public int MinDurabilityRequired => minDurabilityRequired > baseDurability ? baseDurability : minDurabilityRequired;
        public int BaseDurability => baseDurability;
        public bool IsInOrder { get; protected set; }



        protected void BaseSetup( BaseModuleSetup baseModuleSetup)
        {
            crewFull = baseModuleSetup.CrewFull;
            baseDurability = baseModuleSetup.BaseDurability;
            minCrewRequired = baseModuleSetup.MinCrewRequired;
            minDurabilityRequired = baseModuleSetup.MinDurabilityRequired;
            if (baseModuleSetup.ModuleImage != null && _spriteRenderer != null)
            {
                _spriteRenderer.sprite = baseModuleSetup.ModuleImage;
            }
        }
        
        
        public virtual void ClickOn()
        {
        }
        
        public virtual void SetCommanderOnDuty(bool value)
        {
            hasCommanderOnDuty = value;
        }
        
        public abstract void SetActive(bool value);

        protected string GetBaseDescription()
        {
            return "";
           //return $"Crew: {MinCrewRequired}/{CrewFull}, Base Durability {MinDurabilityRequired}/{baseDurability}";
        }

        public void ShowDescription(bool value)
        {
            moduleDescription.gameObject.SetActive(value);
        }
    }
}