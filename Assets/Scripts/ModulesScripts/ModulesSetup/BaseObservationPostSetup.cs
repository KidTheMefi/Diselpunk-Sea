using UnityEngine;

namespace ModulesScripts.ModulesSetup
{
    [CreateAssetMenu(menuName = "Modules/ObservationPost")]
    public class BaseObservationPostSetup :  BaseModuleSetup
    {
        [SerializeField]
        private BaseObservationPost _baseObservationPostPrefab;
        [SerializeField]
        private int _detectionValue;
        public int DetectionValue => _detectionValue;

        public override ShipModule GetModule()
        {
            var module = Instantiate(_baseObservationPostPrefab, Vector3.zero, Quaternion.identity);
            module.Setup(this);
            return module;
        }
    }
}