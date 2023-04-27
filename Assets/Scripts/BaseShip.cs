
using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using InterfaceProviders;
using ShipCharacteristics;
using ShipModuleScripts.ModuleDurability;
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
    private SpeedHandler _speedHandler;
    [SerializeField]
    private ModulesHandler _modules;
    [SerializeField]
    private CommanderMoveHandler _commanderMoveHandler;
    [SerializeField]
    private RetreatHandler _retreatHandler;
    

    [SerializeField]
    private BaseShip _ememyShip;
    [SerializeField]
    private bool controlledByPlayer;

    public int ManeuverabilityValue() => _maneuverabilityHandler.Maneuverability;
    public bool CanEvade() => _maneuverabilityHandler.CanEvade();
    public int DetectionValue() => _detectionHandler.Detection; // write detection handler and providers
    public ModulesHandler Modules => _modules;
    public SpeedHandler SpeedHandler => _speedHandler;
    
    void Start()
    {
        Setup().Forget();
    }

    private async UniTask Setup()
    {
        await _modules.SetupDecks();
        
        var durabilitySignal = new DurabilitySignals(_repairSkillHandler);

        foreach (var durabilityHandler in _modules.GetDurabilityModuleHandlers())
        {
            durabilityHandler.SetupSignal(durabilitySignal);
        }
        
        SetupManeuverability();
        SetupMedicine();
        SetupRepair();
        
        _speedHandler.Setup(_modules.GetProvidersList<ISpeedProvider>());
        _detectionHandler.Setup(1, _modules.GetProvidersList<IDetectionProvider>());
        
        
        foreach (var moduleTR in _modules.GetProvidersList<ITargetRequired>())
        {
            moduleTR.SetShipTarget(_ememyShip);
        }

        foreach (var moduleSCR in _modules.GetProvidersList<IShipCharacteristicsRequired>())
        {
            moduleSCR.SetCharacteristics(this);
        }

        
        shipSurvivability.Setup(_modules.GetDurabilityModuleHandlers(), _repairSkillHandler, _modules.GetProvidersList<IRecoverabilityProvider>(), durabilitySignal);
        shipCrewHandler.Setup(_modules.GetCrewModuleHandlers(), _medicineHandler);
        _modules.ActivateAllModules();


        _retreatHandler.Setup(_speedHandler, _ememyShip.SpeedHandler, controlledByPlayer);
        _commanderMoveHandler.ControlledByPlayer(controlledByPlayer);
        Observation(false);
    }

    private void SetupManeuverability()
    {
        var evasionProviderList = _modules.GetProvidersList<IEvasionProvider>();
        var evasionProvider = evasionProviderList.Count > 0 ? evasionProviderList[0] : null;
        _maneuverabilityHandler.Setup(1, _modules.GetProvidersList<IManeuverabilityProvider>(), evasionProvider);
    }

    private void SetupMedicine()
    {
        var quickRecoveryProviderList = _modules.GetProvidersList<IQuickRecoveryProvider>();
        var quickRecoveryProvider = quickRecoveryProviderList.Count > 0 ? quickRecoveryProviderList[0] : null;
        _medicineHandler.Setup(0,_modules.GetProvidersList<IMedicineProvider>(), quickRecoveryProvider);
    }

    private void SetupRepair()
    {
        var advancedRepairProviderList = _modules.GetProvidersList<IAdvancedRepairProvider>();
        var advancedRepairProvider = advancedRepairProviderList.Count > 0 ? advancedRepairProviderList[0] : null;
        _repairSkillHandler.Setup(0, _modules.GetProvidersList<IRepairSkillProvider>(), advancedRepairProvider);
    }

    public void Observation(bool value)
    {
        if (controlledByPlayer)
        {
            return;
        }
        _commanderMoveHandler.ShowCommanders(value);
        _modules.EnableHPVisual(value);
    }

    
}
