using ShipCharacteristics;

namespace InterfaceProviders
{
    public interface ICurrentDetectionRequired
    {
        public void SetDetection(DetectionHandler detectionHandler);
    }
}