using System;

namespace InterfaceProviders
{
    public interface IMedicineProvider
    {
        public event Action MedicineSkillChanged;
        public int GetMedicineSkill();
    }
}