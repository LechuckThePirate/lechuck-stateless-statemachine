using System;
using System.Threading.Tasks;
using LeChuck.StateMachine;
using Microsoft.Extensions.Logging;

namespace LeChuck.Stateless.StateMachine
{
    public class StateMachineFactory : IStateMachineFactory
    {
        private readonly IStateMachineStore _store;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StateMachineFactory> _logger;

        public StateMachineFactory(IStateMachineStore store, IServiceProvider serviceProvider, ILogger<StateMachineFactory> logger)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IStateMachine> Retrieve(string machineId)
        {
            var (machineType, machineData) = await _store.Retrieve(machineId);
            if (machineType == null)
                return null;

            var machine = (IStateMachine)_serviceProvider.GetService(machineType);
            machine?.DeserializeData(machineData);
            return machine;
        }

        public async Task<TMachine> Create<TMachine>(string machineId) where TMachine : IStateMachine
        {
            _logger.LogTrace($"Creating machine '{machineId}'");
            var machine = (TMachine)_serviceProvider.GetService(typeof(TMachine));
            machine.MachineId = machineId;
            var machineInterface = machine.MachineType;
            await _store.StoreMachine(machine.MachineId, machineInterface, machine.SerializeData());
            return machine;
        }

    }
}
