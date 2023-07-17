using System;

namespace InterfaceProviders
{
    public interface IPumpPowerProvider
    {
        public event Action PumpPowerChanged;
        public int GetPumpPower();
    }
}