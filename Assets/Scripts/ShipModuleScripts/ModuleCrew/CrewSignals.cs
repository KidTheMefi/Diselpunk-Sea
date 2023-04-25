using System;

namespace ShipModuleScripts.ModuleCrew
{
    public class CrewSignals
    {
        public event Action<int> CrewMembersDamaged = delegate(int i) { }; 
        public event Action<int> CrewMembersGoToReserve = delegate(int i) { }; 
        public event Action<CrewHandler, int> RequestCrewMembersFromReserve = delegate(CrewHandler crewModuleHandler, int i) { };

        public void CrewMembersDamagedInvoke(int value)
        {
            CrewMembersDamaged.Invoke(value);
        }
        
        public void CrewMembersGoToReserveInvoke(int value)
        {
            CrewMembersGoToReserve.Invoke(value);
        }
        
        public void RequestCrewMembersFromReserveInvoke(CrewHandler crewHandler, int value)
        {
            RequestCrewMembersFromReserve.Invoke(crewHandler, value);
        }
    }
}