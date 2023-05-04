using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using InterfaceProviders;
using ModulesScripts;
using ShipCharacteristics;
using ShipModuleScripts.ModuleDurability;
using UnityEngine;


public class BaseShip : MonoBehaviour
{
    public Action<ShipEndBattle> EndBattleEvent = delegate(ShipEndBattle battle) { };

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
    private ShellsHandler _shellsHandler;
    [SerializeField]
    private bool controlledByPlayer;

    private BaseShip _enemyShip;
    public ShipCrewHandler ShipCrewHandler => shipCrewHandler;
    public ShipSurvivability ShipSurvivability => shipSurvivability;

    public int ManeuverabilityValue() => _maneuverabilityHandler.Maneuverability;
    public bool CanEvade() => _maneuverabilityHandler.CanEvade();
    public int DetectionValue() => _detectionHandler.Detection; // write detection handler and providers
    public ModulesHandler Modules => _modules;
    public SpeedHandler SpeedHandler => _speedHandler;
    public ShellsHandler ShellsHandler => _shellsHandler;


    public async UniTask Setup()
    {
        await _modules.SetupDecks();

        if (_shellsHandler != null)
        {
            _shellsHandler.SetStartShellValue(5,5,5);
        }
        
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


        foreach (var moduleSCR in _modules.GetProvidersList<IShipCharacteristicsRequired>())
        {
            moduleSCR.SetCharacteristics(this);
        }

        shipSurvivability.Setup(_modules.GetDurabilityModuleHandlers(), _repairSkillHandler, _modules.GetProvidersList<IRecoverabilityProvider>(), durabilitySignal, () => EndBattleEvent.Invoke(ShipEndBattle.Sinking));
        shipCrewHandler.Setup(_modules.GetCrewModuleHandlers(), _medicineHandler, () => EndBattleEvent.Invoke(ShipEndBattle.Surrender));


//        _retreatHandler.Setup(_speedHandler, _enemyShip.SpeedHandler, () => EndBattleEvent.Invoke(ShipEndBattle.Retreat), controlledByPlayer);
        _commanderMoveHandler.ControlledByPlayer(controlledByPlayer);
        Observation(false);
    }

    public void SetEnemy(BaseShip enemyShip)
    {
        _enemyShip = enemyShip;
        _retreatHandler.Setup(_speedHandler, _enemyShip.SpeedHandler, () => EndBattleEvent.Invoke(ShipEndBattle.Retreat), controlledByPlayer);
        foreach (var moduleTR in _modules.GetProvidersList<ITargetRequired>())
        {
            moduleTR.SetShipTarget(_enemyShip);
        }

        _modules.ActivateAllModules();
        shipSurvivability.OnToggleValueChange(true);
    }

    public void UpgradeDeckDurability(ModuleLocation moduleLocation)
    {
        _modules.UpgradeDeckDurability(moduleLocation);
        shipSurvivability.UpdateSurvivability();
    }
    
    public void UpgradeDeckCrew(ModuleLocation moduleLocation)
    {
        _modules.UpgradeDeckCrew(moduleLocation);
        shipCrewHandler.UpdateCrew();
    }
    
    public void UpgradeDeckArmor(ModuleLocation moduleLocation)
    {
        _modules.UpgradeDeckArmor(moduleLocation);
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
        _medicineHandler.Setup(0, _modules.GetProvidersList<IMedicineProvider>(), quickRecoveryProvider);
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
        _modules.EnableHpVisual(value);
    }

    public void FinishBattle()
    {
        foreach (var artillery in _modules.GetProvidersList<BaseArtillery>())
        {
            artillery.BattleEnd();
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}