using System.Collections.Generic;
using ShipModuleScripts.ModuleCrew;
using ShipModuleScripts.ModuleDurability;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    
    public enum ModuleLocation
    {
        LowerDeck, MainDeck, DeckHouse
    }
    public class ModulesHandler : MonoBehaviour
    {
        private ShipPlaceSignal _shipPlaceSignal = new ShipPlaceSignal();
        public ShipPlaceSignal ShipPlaceSignal => _shipPlaceSignal;
        public List<ShipModulePlace> modulesPlaces { get; private set;}
        
        public ShipModulePlace[] LowerDeckPlaces { get; private set;}
        public ShipModulePlace[] MainDeckPlaces { get; private set;}
        public ShipModulePlace[] DeckHousePlaces { get; private set;}

        [SerializeField]
        private GameObject _lowerDeck;
        [SerializeField]
        private GameObject _mainDeck;
        [SerializeField]
        private GameObject _deckHouse;

        private void Awake()
        {
            //modulesPlaces = gameObject.GetComponentsInChildren<ShipModulePlace>();
            
            LowerDeckPlaces = _lowerDeck.GetComponentsInChildren<ShipModulePlace>();
            MainDeckPlaces = _mainDeck.GetComponentsInChildren<ShipModulePlace>();
            DeckHousePlaces = _deckHouse.GetComponentsInChildren<ShipModulePlace>();


            foreach (var modulePlace in LowerDeckPlaces)
            {
                SetupPlace(modulePlace, ModuleLocation.LowerDeck, 2);
            }
            
            foreach (var modulePlace in MainDeckPlaces)
            {
                SetupPlace(modulePlace, ModuleLocation.MainDeck, 3);
            }
            
            foreach (var modulePlace in DeckHousePlaces)
            {
                SetupPlace(modulePlace, ModuleLocation.DeckHouse, 1);
            }
            

            modulesPlaces = new List<ShipModulePlace>();
            modulesPlaces.AddRange(LowerDeckPlaces);
            modulesPlaces.AddRange(MainDeckPlaces);
            modulesPlaces.AddRange(DeckHousePlaces);
        }

        private void SetupPlace(ShipModulePlace shipModulePlace, ModuleLocation moduleLocation, int armor)
        {
            
            shipModulePlace.SetLocation(ModuleLocation.MainDeck);
            shipModulePlace.SetArmor(3);
            shipModulePlace.SetShipPlaceSignal(_shipPlaceSignal);
        }
        
        public void ActivateAllModules()
        {
            foreach (var modulePlace in modulesPlaces)
            {
                modulePlace.CheckAndStartModule();
            }
        }
        
        public ShipModulePlace GetRandomModule()
        {
            return modulesPlaces[Random.Range(0, modulesPlaces.Count)];
        }
        
        public CrewHandler[] GetCrewModuleHandlers()
        {
            CrewHandler[] crewModuleHandlers = new CrewHandler[modulesPlaces.Count];

            for (int i = 0; i < modulesPlaces.Count; i++)
            {
                crewModuleHandlers[i] = modulesPlaces[i].CrewHandler;
            }
            return crewModuleHandlers;
        }

        public DurabilityHandler[] GetDurabilityModuleHandlers()
        {
            DurabilityHandler[] durabilityHandler = new DurabilityHandler[modulesPlaces.Count];

            for (int i = 0; i < modulesPlaces.Count; i++)
            {
                durabilityHandler[i] = modulesPlaces[i].DurabilityHandler;
            }
            return durabilityHandler;
        }

        public List<T> GetProvidersList<T>()
        {
            List<T> providers = new List<T>();
            
            foreach (var t in modulesPlaces)
            {
                if (t.TryGetProvider<T>(out var provider))
                {
                    providers.Add(provider);
                }
            }
            return providers;
            
        }
    }
}