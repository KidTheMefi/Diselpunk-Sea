using System;
using DefaultNamespace;
using PrefabsStatic;

namespace SomeMenu
{
    public class PlayerShipSelect
    {
        public event Action<BaseShip> ShipSelected = delegate(BaseShip ship) { };
        private BaseShip _currentPlayerShip;

        private MenuButtons _menuButtons;


        public PlayerShipSelect(MenuButtons menuButtons)
        {
            _menuButtons = menuButtons;
        }

        public void ShowShipsVariants()
        {
            _menuButtons.RemoveButtons();
            
            _menuButtons.AddButton("BattleShip", () => ShipSelected.Invoke(PlayerShipPrefabs.BattleShipPrefab));
            _menuButtons.AddButton("Ironclad", () => ShipSelected.Invoke(PlayerShipPrefabs.IroncladPrefab));
            _menuButtons.AddButton("Frigate", () => ShipSelected.Invoke(PlayerShipPrefabs.FrigatePrefab));
            _menuButtons.AddButton("TestShip", () => ShipSelected.Invoke(PlayerShipPrefabs.TestShipPrefab));
            
            _menuButtons.ShowMenu("BattleShip - big ship with 4 different type artillery position. " +
                "\nIronClad - ship with better armor and 3 default artillery position. " +
                "\nFrigate - ship with great maneuverability and 3 light artillery position. ");
        }
    }
}