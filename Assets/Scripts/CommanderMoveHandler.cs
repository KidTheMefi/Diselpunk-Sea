using System.Collections.Generic;
using CrewCommanders;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
    public class CommanderMoveHandler : MonoBehaviour
    {
        private Commander _selectedCommander;

        [SerializeField]
        private List<Commander> _shipCommanders;

        [SerializeField]
        private ModulesHandler _modulesHandler;

        public void ControlledByPlayer(bool value)
        {
            if (value)
            {
                SubscribeForClick();
            }
            else
            {
                _modulesHandler.ShipPlaceSignal.ShipModulePlaceOnFire += ShipPlaceSignalOnShipModulePlaceOnFire;
                foreach (var commander in _shipCommanders)
                {
                    commander.ReturnedTo += CommanderOnReturnedTo;
                    commander.MoveToPlaceAsync(_modulesHandler.GetRandomModule()).Forget();
                }
            }
        }

        public void ShowCommanders(bool value)
        {
            foreach (var commander in _shipCommanders)
            {
                commander.gameObject.SetActive(value);
            }
        }
        
        private void CommanderOnReturnedTo(Commander commander)
        {
            commander.MoveToPlaceAsync(_modulesHandler.GetRandomModule()).Forget();
        }
        
        private void ShipPlaceSignalOnShipModulePlaceOnFire(ShipModulePlace place)
        {
            if (place.CommanderOnPost != null)
            {
                return;
            }
            var commander = _shipCommanders.Find(c => !c.Moving);
            if (commander != null)
            {
                commander.MoveToPlaceAsync(place).Forget();
            }
        }

        private void OnMouseDown()
        {
            if (_selectedCommander != null)
            {
                _selectedCommander.MoveBackToDefaultPosition().Forget();

            }
            UnSelect();
        }
        private void CommanderSignalOnOnCommanderClick(Commander commander)
        {
            if (_selectedCommander != commander)
            {
                UnSelect();
                _selectedCommander = commander;
                _selectedCommander.Select(true);
            }
            else
            {
                UnSelect();
            }
        }

        private void UnSelect()
        {
            if (_selectedCommander != null)
            {
                _selectedCommander.Select(false);
                _selectedCommander = null;
            }
        }


        private void ShipPlaceSignalOnOnShipModulePlaceClick(ShipModulePlace obj)
        {
            if (_selectedCommander != null)
            {
                _selectedCommander.MoveToPlaceAsync(obj).Forget();
                UnSelect();
            }
        }

        private void SubscribeForClick()
        {
            _modulesHandler.ShipPlaceSignal.OnShipModulePlaceClick += ShipPlaceSignalOnOnShipModulePlaceClick;
            foreach (var commander in _shipCommanders)
            {
                commander.OnCommanderClick += CommanderSignalOnOnCommanderClick;
            }
        }


        private void UnSubAll()
        {
            _modulesHandler.ShipPlaceSignal.ShipModulePlaceOnFire -= ShipPlaceSignalOnShipModulePlaceOnFire;
            _modulesHandler.ShipPlaceSignal.OnShipModulePlaceClick -= ShipPlaceSignalOnOnShipModulePlaceClick;
            foreach (var commander in _shipCommanders)
            {
                commander.OnCommanderClick -= CommanderSignalOnOnCommanderClick;
                commander.ReturnedTo -= CommanderOnReturnedTo;
            }
        }

        private void OnDestroy()
        {
            UnSubAll();
        }
    }
}