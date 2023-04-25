using System;
using CrewCommanders;

namespace DefaultNamespace
{
    public class CommanderSignal
    {
        public event Action<Commander> OnCommanderClick = delegate(Commander place) { };
        private static CommanderSignal _instance;

        public static CommanderSignal GetInstance()
        {
            return _instance ??= new CommanderSignal();
        }

        public void InvokeCommanderClick(Commander commander)
        {
            OnCommanderClick.Invoke(commander);
        }
    }
}