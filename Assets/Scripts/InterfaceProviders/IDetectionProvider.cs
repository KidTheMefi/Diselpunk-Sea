using System;

namespace InterfaceProviders
{
    public interface IDetectionProvider
    {
        public event Action DetectionChanged;
        public int GetDetection();
    }
}