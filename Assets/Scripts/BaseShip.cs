using System;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using InterfaceProviders;
using ShipCharacteristics;
using ShipModuleScripts.ModuleDurability;
using ShipSinkingScripts;
using UnityEngine;


public class BaseShip : MonoBehaviour
{
    public Action<ShipEndBattle> EndBattleEvent = delegate(ShipEndBattle battle) { };

    [SerializeField]
    private CharacteristicsHandler characteristicsHandler;

    [SerializeField]
    private ModulesHandler _modules;
    [SerializeField]
    private CommanderHandler commanderHandler;
    [SerializeField]
    private RetreatHandler _retreatHandler;

    [SerializeField]
    private bool controlledByPlayer;
    [SerializeField, Range(0, 10)]
    private int _prestigeReward;
    public int PrestigeReward => _prestigeReward;

    private BaseShip _enemyShip;
    public ShipCrewHandler ShipCrewHandler => characteristicsHandler.ShipCrewHandler;
    public ShipSurvivability ShipSurvivability => characteristicsHandler.ShipSurvivability;
    public ShipPrestigeHandler ShipPrestigeHandler => characteristicsHandler.ShipPrestigeHandler;
    public ShipFloodabilityHandler ShipFloodabilityHandler => characteristicsHandler.ShipFloodabilityHandler;


    public bool ControlledByPlayer => controlledByPlayer;
    public int MedicineValue() => characteristicsHandler.MedicineHandler.Medicine;
    public int RepairValue() => characteristicsHandler.RepairSkillHandler.RepairSkill;
    public int ManeuverabilityValue() => characteristicsHandler.ManeuverabilityHandler.Maneuverability;
    public bool CanEvade() => characteristicsHandler.ManeuverabilityHandler.CanEvade();
    public int DetectionValue => characteristicsHandler.DetectionHandler.Detection; // write detection handler and providers
    public ModulesHandler Modules => _modules;
    public SpeedHandler SpeedHandler => characteristicsHandler.SpeedHandler;
    public ShellsHandler ShellsHandler => characteristicsHandler.ShellsHandler;
    public int SonarDetection => characteristicsHandler.SonarHandler.SonarDetection;

    
    public async UniTask Setup()
    {
        await _modules.SetupDecks();

        if (characteristicsHandler.ShellsHandler != null)
        {
            characteristicsHandler.ShellsHandler.SetStartShellValue(5, 5, 5);
        }

        var durabilitySignal = new DurabilitySignals(characteristicsHandler.RepairSkillHandler);

        foreach (var durabilityHandler in _modules.GetDurabilityModuleHandlers())
        {
            durabilityHandler.SetupSignal(durabilitySignal);
        }

        SetupManeuverability();
        SetupMedicine();
        SetupRepair();

        characteristicsHandler.SpeedHandler.Setup(_modules.GetProvidersList<ISpeedProvider>());
        characteristicsHandler.DetectionHandler.Setup(1, _modules.GetProvidersList<IDetectionProvider>());
        characteristicsHandler.SonarHandler.Setup(_modules.GetProvidersList<ISonarValueProvider>());

        foreach (var shellsRequired in _modules.GetProvidersList<IShellsHandlerRequired>())
        {
            shellsRequired.SetShellsHandler(characteristicsHandler.ShellsHandler);
        }

        foreach (var moduleDetectionRequired in _modules.GetProvidersList<ICurrentDetectionRequired>())
        {
            moduleDetectionRequired.SetDetection(characteristicsHandler.DetectionHandler);
        }

        characteristicsHandler.ShipSurvivability.Setup(_modules.GetDurabilityModuleHandlers(), characteristicsHandler.RepairSkillHandler, _modules.GetProvidersList<IRecoverabilityProvider>(), durabilitySignal, () => EndBattleEvent.Invoke(ShipEndBattle.Sinking));
        characteristicsHandler.ShipCrewHandler.Setup(_modules.GetCrewModuleHandlers(), characteristicsHandler.MedicineHandler, () => EndBattleEvent.Invoke(ShipEndBattle.Surrender));
        characteristicsHandler.ShipFloodabilityHandler.Setup(characteristicsHandler.RepairSkillHandler, _modules.GetProvidersList<IPumpPowerProvider>(), () => EndBattleEvent.Invoke(ShipEndBattle.Sinking));
        
//        _retreatHandler.Setup(_speedHandler, _enemyShip.SpeedHandler, () => EndBattleEvent.Invoke(ShipEndBattle.Retreat), controlledByPlayer);
        commanderHandler.ControlledByPlayer(controlledByPlayer);
        Observation(false);
        if (characteristicsHandler.ShipPrestigeHandler != null)
        {
            characteristicsHandler.ShipPrestigeHandler.SetPrestige(_prestigeReward);
        }
        _modules.ActivateAllModules();
    }

    public void SetEnemy(BaseShip enemyShip)
    {
        _enemyShip = enemyShip;
        _retreatHandler.Setup(characteristicsHandler.SpeedHandler, _enemyShip.SpeedHandler, () => EndBattleEvent.Invoke(ShipEndBattle.Retreat), controlledByPlayer);

        foreach (var moduleTR in _modules.GetProvidersList<ITargetRequired>())
        {
            moduleTR.SetShipTarget(_enemyShip);
        }

        _modules.ActivateAllModules();
        characteristicsHandler.ShipSurvivability.OnToggleValueChange(true);
    }

    public void UpgradeDeckDurability(ModuleLocation moduleLocation)
    {
        _modules.UpgradeDeckDurability(moduleLocation);
        characteristicsHandler.ShipSurvivability.UpdateSurvivability();
    }

    public void UpgradeDeckCrew(ModuleLocation moduleLocation)
    {
        _modules.UpgradeDeckCrew(moduleLocation);
        characteristicsHandler.ShipCrewHandler.UpdateCrew();
    }

    public void UpgradeDeckArmor(ModuleLocation moduleLocation)
    {
        _modules.UpgradeDeckArmor(moduleLocation);
    }

    private void SetupManeuverability()
    {
        var evasionProviderList = _modules.GetProvidersList<IEvasionProvider>();
        var evasionProvider = evasionProviderList.Count > 0 ? evasionProviderList[0] : null;
        characteristicsHandler.ManeuverabilityHandler.Setup(_modules.GetProvidersList<IManeuverabilityProvider>(), evasionProvider);
    }

    private void SetupMedicine()
    {
        var quickRecoveryProviderList = _modules.GetProvidersList<IQuickRecoveryProvider>();
        var quickRecoveryProvider = quickRecoveryProviderList.Count > 0 ? quickRecoveryProviderList[0] : null;
        characteristicsHandler.MedicineHandler.Setup(_modules.GetProvidersList<IMedicineProvider>(), quickRecoveryProvider);
    }
    

    private void SetupRepair()
    {
        var advancedRepairProviderList = _modules.GetProvidersList<IAdvancedRepairProvider>();
        var advancedRepairProvider = advancedRepairProviderList.Count > 0 ? advancedRepairProviderList[0] : null;
        characteristicsHandler.RepairSkillHandler.Setup(0, _modules.GetProvidersList<IRepairSkillProvider>(), advancedRepairProvider);
    }

    public void Observation(bool value)
    {
        if (controlledByPlayer)
        {
            return;
        }
        commanderHandler.ShowCommanders(value);
        _modules.EnableHpVisual(value);
    }

    public void FinishBattle()
    {
        foreach (var module in _modules.GetProvidersList<IAfterBattleTurnOff>())
        {
            module.BattleEnd();
        }
        _retreatHandler.DisableRetreat();
    }

    public void DamageRecoverAbility(int damageValue)
    {
        characteristicsHandler.ShipSurvivability.RecoverabilityDamage(damageValue);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}