#region

using System.Threading.Tasks;

#endregion

namespace LeChuck.Stateless.StateMachine
{
    public interface IStateMachineFactory
    {
        Task<IStateMachine<TContext, TEntity>> Retrieve<TContext, TEntity>(string machineId)
            where TContext : class
            where TEntity : class;

        Task<IStateMachine> Retrieve(string machineId);

        Task<IStateMachine<TContext, TEntity>> Create<TContext, TEntity>(string machineId)
            where TContext : class
            where TEntity : class;
    }
}