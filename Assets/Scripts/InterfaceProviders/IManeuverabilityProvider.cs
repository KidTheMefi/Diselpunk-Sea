using System;

namespace DefaultNamespace
{
    public interface IManeuverabilityProvider
    {
        public event Action ManeuverabilityChanged;
        public int GetManeuverability();
    }
}