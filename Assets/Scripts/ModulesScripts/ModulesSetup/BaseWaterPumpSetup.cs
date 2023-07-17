using UnityEngine;

namespace ModulesScripts.ModulesSetup
{
    [CreateAssetMenu(menuName = "Modules/WaterPump")]
    public class BaseWaterPumpSetup : BaseModuleSetup
    {
        [SerializeField]
        private BaseWaterPump baseSonarPrefab;
        [SerializeField]
        private int _pumpPowerValue;

        public int PumpPowerValue => _pumpPowerValue;
        
        public override ShipModule GetModule()
        {
            var module = Instantiate(baseSonarPrefab, Vector3.zero, Quaternion.identity);
            module.Setup(this);
            return module;
        }
    }
}