using System;
using UnityEngine;

namespace DefaultNamespace
{

    public enum ShellType
    {
        Common, ArmorPiercing, HighExplosive,  Shrapnel
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

        public Shell(Shell baseShell, ShellType shellType)
        {
            _shellType = shellType;
            _baseDamage = baseShell.BaseDamage;
            _armorPiercingClass = baseShell.ArmorPiercingClass;
            _baseCrewDamage = baseShell.BaseCrewDamage;
            
            switch (shellType)
            {
                case ShellType.Shrapnel:
                    _armorPiercingClass = _armorPiercingClass > 0 ? _armorPiercingClass - 1 : _armorPiercingClass;
                    _baseCrewDamage++;
                    break;
                case ShellType.ArmorPiercing:
                    _armorPiercingClass += 2;
                    break;
                case ShellType.HighExplosive:
                    _baseCrewDamage = _baseCrewDamage > 0 ? _baseCrewDamage - 1 : _baseCrewDamage;
                    break;
            }
        }
        
        public int ArmorPiercingClass => _armorPiercingClass;
        public int BaseDamage => _baseDamage;
        public int BaseCrewDamage => _baseCrewDamage;
        public ShellType ShellType => _shellType;
    }
}