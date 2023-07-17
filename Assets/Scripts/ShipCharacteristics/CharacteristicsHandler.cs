using ShipSinkingScripts;
using UnityEngine;

namespace ShipCharacteristics
{
    public class CharacteristicsHandler : MonoBehaviour
    {
        [SerializeField]
        private ShipCrewHandler _shipCrewHandler;
        [SerializeField]
        private ShipSurvivability _shipSurvivability;
        [SerializeField]
        private ManeuverabilityHandler _maneuverabilityHandler;
        [SerializeField]
        private DetectionHandler _detectionHandler;
        [SerializeField]
        private RepairSkillHandler _repairSkillHandler;
        [SerializeField]
        private MedicineHandler _medicineHandler;
        [SerializeField]
        private SpeedHandler _speedHandler;
        [SerializeField]
        private SonarHandler _sonarHandler;
        [SerializeField]
        private ShellsHandler _shellsHandler;
        [SerializeField]
        private ShipPrestigeHandler _shipPrestigeHandler;
        [SerializeField]
        private ShipFloodabilityHandler shipFloodabilityHandler;
        
        public ShipCrewHandler ShipCrewHandler => _shipCrewHandler;
        public ShipSurvivability ShipSurvivability => _shipSurvivability;
        public ManeuverabilityHandler ManeuverabilityHandler => _maneuverabilityHandler;
        public DetectionHandler DetectionHandler => _detectionHandler;
        public RepairSkillHandler RepairSkillHandler => _repairSkillHandler;
        public MedicineHandler MedicineHandler => _medicineHandler;
        public SpeedHandler SpeedHandler => _speedHandler;
        public ShellsHandler ShellsHandler => _shellsHandler;
        public ShipPrestigeHandler ShipPrestigeHandler => _shipPrestigeHandler;
        public SonarHandler SonarHandler => _sonarHandler;
        public ShipFloodabilityHandler ShipFloodabilityHandler => shipFloodabilityHandler;

    }
}