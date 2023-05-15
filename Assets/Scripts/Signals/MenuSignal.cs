using System;

namespace Signals
{
        public class MenuSignal
        {
                private static MenuSignal _instance;
                public static MenuSignal Instance => _instance ??= new MenuSignal();
        
                public event Action<BaseShip> BattleWithShip = delegate(BaseShip ship) { };
                public event Action BackToPatrol =  delegate { };
                
                public void InvokeBattleWith(BaseShip baseShip)
                {
                        BattleWithShip.Invoke(baseShip);
                }
                
                public void InvokePatrol()
                {
                        BackToPatrol.Invoke();
                }
        }
}