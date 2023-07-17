using DefaultNamespace;
using PrefabsStatic;
using Signals;
using UnityEngine;

namespace SomeMenu
{
    public class SosAmbushEvent : IMenuEvent
    {
        private BaseShip _playerShip;
        private MenuButtons _menuButtons;

        private int _ambushValue;

        public SosAmbushEvent(BaseShip playerShip, MenuButtons menuButtons)
        {
            _playerShip = playerShip;
            _menuButtons = menuButtons;
            _ambushValue = Random.Range(2, 8);
        }

        public void BeginEvent()
        {
            bool ambushChecked = _playerShip.DetectionValue > _ambushValue;

            string menuInfo = ambushChecked ? "On the way to the source of the signal, your officers notice strange interference in the radio. There is a chance that an ambush is ahead."
                : "You see a ship with a big cloud of smoke around it.";


            if (ambushChecked)
            {
                _menuButtons.AddButton("Set a course to be in an advantageous combat position.", () =>
                {
                    EndEvent endEvent = new EndEvent(_menuButtons, () => MenuSignal.Instance.InvokeBattleWith(EnemyShipPrefabs.GetRandomShipPrefab()));
                    endEvent.BeginEvent("Your position prevented the enemy from making an unexpected first strike.", "Battle");
                });
                _menuButtons.AddButton("This is too suspicious. Leave", MenuSignal.Instance.InvokePatrol);
            }
            else
            {
                _menuButtons.AddButton("Get closer", () =>
                {
                    foreach (var modulePlace in _playerShip.Modules.modulesPlaces)
                    {
                        modulePlace.DamageDurability(Random.Range(1, 3));
                    }

                    EndEvent endEvent = new EndEvent(_menuButtons, () => MenuSignal.Instance.InvokeBattleWith(EnemyShipPrefabs.GetRandomShipPrefab()));
                    endEvent.BeginEvent("Suddenly, volley of artillery fire come out of the smoke, damaging your ship. It's enemy ambush.", "Battle");
                });
            }

            _menuButtons.ShowMenu(menuInfo);
        }
    }
}