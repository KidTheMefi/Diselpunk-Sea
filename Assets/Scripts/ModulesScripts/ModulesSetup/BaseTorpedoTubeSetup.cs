using DefaultNamespace;
using UnityEngine;

namespace ModulesScripts.ModulesSetup
{
    [CreateAssetMenu(menuName = "Modules/TorpedoTube")]
    public class BaseTorpedoTubeSetup : BaseModuleSetup
    {
        [SerializeField]
        private BaseTorpedoTube _baseTorpedoTubePrefab;
        [SerializeField, Range(0.5f, 15f)]
        private float _reloadTime;
        [SerializeField, Range(0.5f, 15f)]
        private float _aiming;
        [SerializeField]
        private Torpedo _torpedo;

        public float ReloadTime => _reloadTime;
        public float AimingTime => _aiming;
        public Torpedo Torpedo => _torpedo;
        
        public override ShipModule GetModule()
        {
            var module = Instantiate(_baseTorpedoTubePrefab, Vector3.zero, Quaternion.identity);
            module.Setup(this);
            return module;
        }
    }
}