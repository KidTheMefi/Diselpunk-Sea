using UnityEngine;

namespace PrefabsStatic
{
    public static class EnemyShipPrefabs
    {
        public static BaseShip CorvettePrefab => Resources.Load<BaseShip>("Prefabs/BotShips/Corvette Ship");
        public static BaseShip FrigatePrefab => Resources.Load<BaseShip>("Prefabs/BotShips/Frigate Ship");
        public static BaseShip IroncladPrefab => Resources.Load<BaseShip>("Prefabs/BotShips/Ironclad Ship");
        public static BaseShip DestroyerPrefab => Resources.Load<BaseShip>("Prefabs/BotShips/Destroyer Ship");
        public static BaseShip CruiserPrefab => Resources.Load<BaseShip>("Prefabs/BotShips/Cruiser Ship");
        public static BaseShip PassengerPrefab => Resources.Load<BaseShip>("Prefabs/BotShips/Passenger Ship");



        public static BaseShip GetRandomShipPrefab()
        {
            var allShips = Resources.LoadAll<BaseShip>("Prefabs/BotShips/");

            if (allShips.Length == 0)
            {
                throw new UnityException();
            }
            
            return allShips[Random.Range(0, allShips.Length)];
        }
    }
}