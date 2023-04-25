
using DefaultNamespace;
using InterfaceProviders;
using ShipCharacteristics;
using UnityEngine;

public class BaseShip : MonoBehaviour
{
    
    [SerializeField]
    private ShipCrewHandler shipCrewHandler;
    [SerializeField]
    private ShipSurvivability shipSurvivability;
    [SerializeField]
    private ManeuverabilityHandler _maneuverabilityHandler;
    [SerializeField]
    private DetectionHandler _detectionHandler;
    [SerializeField]
    private RepairSkillHandler _repairSkillHandler;
    [SerializeField]
    private MedicineHandler _medicineHandler;
    [SerializeField]
    private ModulesHandler _modules;
    

    [SerializeField]
    private BaseShip _ememyShip;

    public int ManeuverabilityValue() => _maneuverabilityHandler.Maneuverability;
    public bool CanEvade() => _maneuverabilityHandler.CanEvade();
    public int DetectionValue() => _detectionHandler.Detection; // write detection handler and providers
    public ModulesHandler Modules => _modules;
    
    void Start()
    {
        var evasionProviderList = _modules.GetProvidersList<IEvasionProvider>();
        var evasionProvider = evasionProviderList.Count > 0 ? evasionProviderList[0] : null;
        _maneuverabilityHandler.Setup(1, _modules.GetProvidersList<IManeuverabilityProvider>(), evasionProvider);
        
        var quickRecoveryProviderList = _modules.GetProvidersList<IQuickRecoveryProvider>();
        var quickRecoveryProvider = quickRecoveryProviderList.Count > 0 ? quickRecoveryProviderList[0] : null;
        _medicineHandler.Setup(0,_modules.GetProvidersList<IMedicineProvider>(), quickRecoveryProvider);
        
        
        var advancedRepairProviderList = _modules.GetProvidersList<IAdvancedRepairProvider>();
        var advancedRepairProvider = advancedRepairProviderList.Count > 0 ? advancedRepairProviderList[0] : null;
        _repairSkillHandler.Setup(0, _modules.GetProvidersList<IRepairSkillProvider>(), advancedRepairProvider);        
        
        _detectionHandler.Setup(1, _modules.GetProvidersList<IDetectionProvider>());
        
        
        
        
        shipSurvivability.Setup(_modules.GetDurabilityModuleHandlers(), _repairSkillHandler, _modules.GetProvidersList<IRecoverabilityProvider>());
        shipCrewHandler.Begin(_modules.GetCrewModuleHandlers(), _medicineHandler);
        foreach (var moduleTR in _modules.GetProvidersList<ITargetRequired>())
        {
            moduleTR.SetShipTarget(_ememyShip);
        }

        foreach (var moduleSCR in _modules.GetProvidersList<IShipCharacteristicsRequired>())
        {
            moduleSCR.SetCharacteristics(this);
        }
        _modules.ActivateAllModules();
    }

    
}
