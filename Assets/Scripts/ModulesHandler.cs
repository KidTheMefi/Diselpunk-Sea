using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        private Deck _lowerDeck;
        [SerializeField]
        private Deck _mainDeck;
        [SerializeField]
        private Deck _deckHouse;
        

        public async UniTask SetupDecks()
        {
            _lowerDeck.SetupDeck(_shipPlaceSignal);
            _mainDeck.SetupDeck(_shipPlaceSignal);
            _deckHouse.SetupDeck(_shipPlaceSignal);
            
            modulesPlaces = new List<ShipModulePlace>();
            
            modulesPlaces.AddRange(_lowerDeck.DeckPlaces);
            modulesPlaces.AddRange(_mainDeck.DeckPlaces);
            modulesPlaces.AddRange(_deckHouse.DeckPlaces);

            await UniTask.Yield();
        }

        public void EnableHpVisual(bool value)
        {
            foreach (var places in modulesPlaces)
            {
                places.InformationVisible(value);
            }
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