using System;

namespace InterfaceProviders
{
    public interface ISpeedProvider
    {
        public event Action SpeedChanged;
        public int GetSpeed();
    }
}