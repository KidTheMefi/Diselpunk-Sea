using System;

namespace DefaultNamespace
{
    public enum ShipEndBattle
    {
        Retreat, Sinking, Surrender
    }
    public class EndBattleSignal
    {
        private event Action<BaseShip, ShipEndBattle> EndBattle = delegate(BaseShip ship, ShipEndBattle battleEnd) { };

        public void InvokeEndBattle(BaseShip ship, ShipEndBattle battleEnd)
        {
            EndBattle.Invoke(ship, battleEnd);
        }
    }
}