#region

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

#endregion

namespace LeChuck.Stateless.StateMachine
{
    public class StateMachineFactory : IStateMachineFactory
    {
        private readonly IStateMachineStore _store;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StateMachineFactory> _logger;

        public StateMachineFactory(IStateMachineStore store, IServiceProvider serviceProvider,
            ILogger<StateMachineFactory> logger)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IStateMachine<TContext, TEntity>> Retrieve<TContext, TEntity>(string machineId)
            where TContext : class
            where TEntity : class
        {
            var (machineType, machineData) = await _store.RetrieveMachine(machineId);
            if (machineType == null)
                return null;

            var machine = (IStateMachine<TContext, TEntity>) _serviceProvider.GetService(machineType);
            machine?.DeserializeData(machineData);
            return machine;
        }

        public async Task<IStateMachine> Retrieve(string machineId)
        {
            var (machineType, machineData) = await _store.RetrieveMachine(machineId);
            if (machineType == null)
                return null;

            var machine = (IStateMachine) _serviceProvider.GetService(machineType);
            machine?.DeserializeData(machineData);
            return machine;
        }

        public async Task<IStateMachine<TContext, TEntity>> Create<TContext, TEntity>(string machineId)
            where TContext : class
            where TEntity : class
        {
            _logger.LogTrace($"Creating machine '{machineId}'");
            var machine =
                (IStateMachine<TContext, TEntity>) _serviceProvider.GetService(
                    typeof(IStateMachine<TContext, TEntity>));
            machine.MachineId = machineId;
            var machineInterface = machine.MachineType;
            await _store.StoreMachine(machine.MachineId, machineInterface, machine.SerializeData());
            return machine;
        }
    }
}