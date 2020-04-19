using System;
using System.Threading.Tasks;

namespace LeChuck.StateMachine
{
    public interface IStateMachineStore
    {
        Task<(Type type, string data)> Retrieve(string machineId);

        Task StoreMachine(string machineId, Type type, string machineData);
    }
}