using UnityEngine;

namespace ModulesScripts.ModulesSetup
{
    [CreateAssetMenu(menuName = "Modules/CaptainsBridge")]
    public class BaseCaptainsBridgeSetup : BaseModuleSetup
    {
        [SerializeField]
        private BaseCaptainsBridge _baseCaptainsBridgePrefab;
        [SerializeField]
        private int _maneuverabilityValue;
        [SerializeField]
        private int _detectionValue;

        public int ManeuverabilityValue => _maneuverabilityValue;
        public int DetectionValue => _detectionValue;
        
        public override ShipModule GetModule()
        {
            var module = Instantiate(_baseCaptainsBridgePrefab, Vector3.zero, Quaternion.identity);
            module.Setup(this);
            return module;
        }
    }
}