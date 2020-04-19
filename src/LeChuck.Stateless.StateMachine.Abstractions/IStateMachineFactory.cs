using System.Threading.Tasks;

namespace LeChuck.StateMachine
{
    public interface IStateMachineFactory
    {
        Task<IStateMachine> Retrieve(string machineId);
        Task<TMachine> Create<TMachine>(string machineId) where TMachine : IStateMachine;
    }
}