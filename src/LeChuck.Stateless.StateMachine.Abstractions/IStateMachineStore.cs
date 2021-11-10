#region

using System;
using System.Threading.Tasks;

#endregion

namespace LeChuck.Stateless.StateMachine
{
    public interface IStateMachineStore
    {
        Task<(Type type, string data)> RetrieveMachine(string machineId);

        Task StoreMachine(string machineId, Type type, string machineData);
        Task DeleteMachine(string machineId);
    }
}