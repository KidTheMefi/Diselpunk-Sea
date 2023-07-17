using System;

namespace InterfaceProviders
{
    public interface ISonarValueProvider
    {
        public event Action SonarValueChanged;
        public int GetSonarValue();
    }
}