using DefaultNamespace;
using UnityEngine;

namespace ModulesScripts.ModulesSetup
{
    [CreateAssetMenu(menuName = "Modules/BaseArtillery")]
    public class BaseArtillerySetup : BaseModuleSetup
    {
        [SerializeField]
        protected BaseArtillery _baseArtilleryPrefab;
        [SerializeField, Range(0.5f, 15f)]
        private float _reloadTime;
        [SerializeField, Range(0.5f, 15f)]
        private float _aiming;
        [SerializeField]
        private Shell _shell;

        [SerializeField]
        private bool randomShellType;

        public float ReloadTime => _reloadTime;
        public float AimingTime => _aiming;
        public Shell BaseShell => _shell;
        
        public override ShipModule GetModule()
        {
            var module = Instantiate(_baseArtilleryPrefab, Vector3.zero, Quaternion.identity);
            module.Setup(this);
            return module;
        }

        public Shell GetShell()
        {
            return randomShellType ? new Shell(_shell, UsefulStatic.RandomEnumValue<ShellType>()) : _shell;
        }
    }
}