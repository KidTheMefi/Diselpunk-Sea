using UnityEngine;

namespace PrefabsStatic
{
    public static class PlayerShipPrefabs
    {
        public static BaseShip BattleShipPrefab => Resources.Load<BaseShip>("Prefabs/PlayerShips/PlayerBattleShip");
        public static BaseShip FrigatePrefab => Resources.Load<BaseShip>("Prefabs/PlayerShips/PlayerFrigateShip");
        public static BaseShip IroncladPrefab => Resources.Load<BaseShip>("Prefabs/PlayerShips/PlayerIronCladShip");
    }
}