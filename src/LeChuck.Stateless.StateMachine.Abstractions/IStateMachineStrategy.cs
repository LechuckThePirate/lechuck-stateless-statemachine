#region

using System.Threading.Tasks;

#endregion

namespace LeChuck.Stateless.StateMachine
{
    public interface IStateMachineStrategy<TContext, TEntity> where TEntity : class where TContext : class
    {
        bool CanHandle(string key);
        Task<bool> Handle(TContext context, IStateMachine<TContext, TEntity> stateMachine);
    }
}