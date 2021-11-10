namespace LeChuck.Stateless.StateMachine
{
    public interface IStateMachineStrategySelector<TContext, TEntity> where TEntity : class where TContext : class
    {
        IStateMachineStrategy<TContext, TEntity> GetHandlerFor(string selectKey);
    }
}