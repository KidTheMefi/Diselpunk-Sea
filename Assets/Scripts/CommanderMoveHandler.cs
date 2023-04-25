using System;
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

        private void Start()
        {
            _modulesHandler.ShipPlaceSignal.OnShipModulePlaceClick += ShipPlaceSignalOnOnShipModulePlaceClick;

            foreach (var commander in _shipCommanders)
            {
                commander.OnCommanderClick += CommanderSignalOnOnCommanderClick;
            }
        }


        private void OnDestroy()
        {
            _modulesHandler.ShipPlaceSignal.OnShipModulePlaceClick -= ShipPlaceSignalOnOnShipModulePlaceClick;

            foreach (var commander in _shipCommanders)
            {
                commander.OnCommanderClick -= CommanderSignalOnOnCommanderClick;
            }
        }

        private void OnMouseDown()
        {
            if (_selectedCommander != null)
            {
                _selectedCommander.MoveFromPost(transform.position);

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
    }
}