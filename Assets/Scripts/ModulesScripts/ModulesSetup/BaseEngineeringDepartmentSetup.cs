using UnityEngine;

namespace ModulesScripts.ModulesSetup
{
    [CreateAssetMenu(menuName = "Modules/EngineeringDepartment")]
    public class BaseEngineeringDepartmentSetup : BaseModuleSetup
    {
        [SerializeField]
        private BaseEngineeringDepartment _baseEnginePrefab;
        [SerializeField, Range(0,10)]
        private int _repairSkillValue;
        [SerializeField, Range(0,50)]
        private int _recoverability;

        public int RepairSkillValue => _repairSkillValue;
        public int Recoverability => _recoverability;
        
        public override ShipModule GetModule()
        {
            var module = Instantiate(_baseEnginePrefab, Vector3.zero, Quaternion.identity);
            module.Setup(this);
            return module;
        }
    }
}