using System;

namespace DefaultNamespace
{
    public class ShipPlaceSignal
    {
        public event Action<ShipModulePlace> OnShipModulePlaceClick = delegate(ShipModulePlace place) { };
        public event Action<ShipModulePlace> ShipModulePlaceOnFire = delegate(ShipModulePlace place) { };
        
        public void InvokePLaceClick(ShipModulePlace shipModulePlace)
        {
            OnShipModulePlaceClick.Invoke(shipModulePlace);
        }
        
        public void InvokeShipModulePlaceOnFire(ShipModulePlace shipModulePlace)
        {
            ShipModulePlaceOnFire.Invoke(shipModulePlace);
        }
    }
}