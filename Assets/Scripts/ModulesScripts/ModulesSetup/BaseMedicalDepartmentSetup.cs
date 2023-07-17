using UnityEngine;

namespace ModulesScripts.ModulesSetup
{
    [CreateAssetMenu(menuName = "Modules/MedicalDepartment")]
    public class BaseMedicalDepartmentSetup : BaseModuleSetup
    {
        [SerializeField]
        private BaseMedicalDepartment baseMedicalDepartmentSetupPrefab;
        [SerializeField]
        private int _medicineSkill;

        public int MedicineSkill => _medicineSkill;
        
        public override ShipModule GetModule()
        {
            var module = Instantiate(baseMedicalDepartmentSetupPrefab, Vector3.zero, Quaternion.identity);
            module.Setup(this);
            return module;
        }
    }
}