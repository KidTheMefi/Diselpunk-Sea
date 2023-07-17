using UnityEngine;

namespace ModulesScripts.ModulesSetup
{
    [CreateAssetMenu(menuName = "Modules/Sonar")]
    public class BaseSonarSetup : BaseModuleSetup
    {
        [SerializeField]
        private BaseSonar baseSonarPrefab;
        [SerializeField]
        private int _sonarValue;

        public int SonarValue => _sonarValue;
        
        public override ShipModule GetModule()
        {
            var module = Instantiate(baseSonarPrefab, Vector3.zero, Quaternion.identity);
            module.Setup(this);
            return module;
        }
    }
}