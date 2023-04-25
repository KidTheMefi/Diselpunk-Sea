using System;

namespace DefaultNamespace
{
    public class ShipPlaceSignal
    {
        public event Action<ShipModulePlace> OnShipModulePlaceClick = delegate(ShipModulePlace place) { };
        
        public void InvokePLaceClick(ShipModulePlace shipModulePlace)
        {
            OnShipModulePlaceClick.Invoke(shipModulePlace);
        }
    }
}