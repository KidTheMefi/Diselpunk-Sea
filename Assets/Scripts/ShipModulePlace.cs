using System;
using CrewCommanders;
using DefaultNamespace;
using ModulesScripts;
using ShipModuleScripts.ModuleCrew;
using ShipModuleScripts.ModuleDurability;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipModulePlace : MonoBehaviour
{
    [SerializeField]
    private CrewHandler _crewHandler;
    public CrewHandler CrewHandler => _crewHandler;
    [SerializeField]
    private DurabilityHandler _durabilityHandler;
    public DurabilityHandler DurabilityHandler => _durabilityHandler;

    [SerializeField]
    private Fire _fire;

    [SerializeField]
    private ShipModule _shipModulePrefab;
    private ShipModule _shipModule;
    public ModuleLocation ModuleLocation { get; private set; }
    
    private int _baseFireExtinguishing = 1;
    public int Armor { get; private set; }

    private bool _isModuleActive = true;
    public bool IsOnFire => _fire.IsOnFire;

    private Commander _commanderOnPost;
    public Commander CommanderOnPost => _commanderOnPost;
    private ShipPlaceSignal _shipPlaceSignal;

    private void Awake()
    {
        

        if (_shipModulePrefab != null)
        {
            _shipModule = Instantiate(_shipModulePrefab, transform);
            _shipModule.transform.localPosition = Vector3.zero;
        }

        _fire.Setup(this);
        _crewHandler.FunctionalityChange += OnCrewFunctionalityChanged;
        _durabilityHandler.FunctionalityChange += OnFunctionalityChange;
    }

    public void Setup(int baseDurability, int baseCrew, int armor, ModuleLocation moduleLocation, ShipPlaceSignal shipPlaceSignal)
    {
        int minCrewRequired = 1;
        int minDurabilityRequired = 0;

        if (_shipModule != null)
        {
            baseDurability += _shipModule.BaseDurability;
            baseCrew += _shipModule.CrewFull;
            minCrewRequired = _shipModule.MinCrewRequired;
            minDurabilityRequired = _shipModule.MinDurabilityRequired;
        }
        
        _crewHandler.Setup(new IntValue(baseCrew), minCrewRequired);
        _durabilityHandler.Setup(new IntValue(baseDurability), minDurabilityRequired);
        
        Armor = armor > 0 ? armor : 0;
        ModuleLocation = moduleLocation;
        _shipPlaceSignal = shipPlaceSignal;
    }

    public void CheckAndStartModule()
    {
        _durabilityHandler.Begin();
        _crewHandler.Begin();
        if (_shipModule != null)
        {
            _shipModule.SetActive(_isModuleActive);
        }
    }

    public void SetCommander(Commander commander)
    {
        _commanderOnPost = commander;
        _durabilityHandler.CommanderOnPost(true);
        if (_shipModule != null)
        {
            _shipModule.SetCommanderOnDuty(true);
        }
    }

    public void RemoveCommander()
    {
        _commanderOnPost = null;
        _durabilityHandler.CommanderOnPost(false);
        if (_shipModule != null)
        {
            _shipModule.SetCommanderOnDuty(false);
        }
    }

    public void DamageCrew(int value)
    {
        if (_commanderOnPost != null && value > 0)
        {
            _commanderOnPost.Damage();
        }
        _crewHandler.DamageCrew(value);
    }

    public void DamageDurability(int value)
    {
        _durabilityHandler.Damage(value);
    }


    private void OnCrewFunctionalityChanged()
    {
        _durabilityHandler.EnoughCrewToRepair(_crewHandler.Functional);
        OnFunctionalityChange();
    }

    private void OnFunctionalityChange()
    {
        _isModuleActive = _crewHandler.Functional && _durabilityHandler.Functional;
        if (_shipModule != null)
        {
            _shipModule.SetActive(_isModuleActive);
        }
    }

    public int GetFireExtinguishing()
    {
        var fireExtinguishing = _baseFireExtinguishing + _crewHandler.CrewValue.CurrentValue * 2;
        fireExtinguishing += CommanderOnPost == null ? 0 : 10;
        return fireExtinguishing;
    }

    public void AddFire(int fireLvl)
    {
        _fire.AddFire(fireLvl);
        _shipPlaceSignal.InvokeShipModulePlaceOnFire(this);
    }


    public bool TryGetProvider<T>(out T provider)
    {
        provider = default(T);
        if (_shipModule is T newProvider)
        {
            provider = newProvider;
            return true;
        }
        return false;
    }

    public void HPVisible(bool value)
    {
        _durabilityHandler.gameObject.SetActive(value);
        _crewHandler.gameObject.SetActive(value);
    }
    private void OnMouseDown()
    {
        _shipPlaceSignal.InvokePLaceClick(this);
        if (_shipModule != null)
        {
            _shipModule.ClickOn();
        }
    }
}