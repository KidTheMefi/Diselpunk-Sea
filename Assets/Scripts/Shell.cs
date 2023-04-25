using System;
using UnityEngine;

namespace DefaultNamespace
{

    public enum ShellType
    {
        Common, ArmourPiercing, HighExplosive,  Shrapnel
    }
    
    [Serializable]
    public class Shell
    {
        [SerializeField]
        private ShellType _shellType;
        [SerializeField]
        private int _armorPiercingClass;
        [SerializeField]
        private int _baseDamage;
        [SerializeField]
        private int _baseCrewDamage;
        
        public int ArmorPiercingClass => _armorPiercingClass;
        public int BaseDamage => _baseDamage;
        public int BaseCrewDamage => _baseCrewDamage;
        public ShellType ShellType => _shellType;
    }
}