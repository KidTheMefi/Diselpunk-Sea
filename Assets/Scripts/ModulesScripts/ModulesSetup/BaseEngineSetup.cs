using UnityEngine;

namespace ModulesScripts.ModulesSetup
{
    [CreateAssetMenu(menuName = "Modules/Engine")]
    public class BaseEngineSetup : BaseModuleSetup
    {
        [SerializeField]
        private BaseEngine _baseEngine;
        [SerializeField]
        private int _maneuverabilityValue;
        [SerializeField]
        private int _speed;

        public override ShipModule GetModule()
        {
            var engine = Instantiate(_baseEngine, Vector3.zero, Quaternion.identity);
            engine.Setup(_speed, _maneuverabilityValue, this);
            return engine;
        }
    }
}