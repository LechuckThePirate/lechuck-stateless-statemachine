using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LeChuck.StateMachine;
using Microsoft.Extensions.Logging;

namespace SampleStateMachine.StateMachines
{
    public class StateMachineInMemoryStore : IStateMachineStore
    {
        private readonly ILogger<StateMachineInMemoryStore> _logger;

        private static readonly Dictionary<string, (Type type, string data)> _storage
            = new Dictionary<string, (Type type, string data)>();

        public StateMachineInMemoryStore(ILogger<StateMachineInMemoryStore> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(Type type, string data)> Retrieve(string machineId)
        {
            if (_storage.ContainsKey(machineId))
            {
                _logger.LogTrace($"Retrieved machine '{machineId}'");
                return await Task.FromResult(_storage[machineId]);
            }

            return (null, null);
        }

        public async Task StoreMachine(string machineId, Type type, string machineData)
        {
            if (_storage.ContainsKey(machineId))
            {
                _storage[machineId] = (type, machineData);
                return;
            }

            _storage.Add(machineId, (type, machineData));
            _logger.LogTrace($"Stored machine '{machineId}'");
            await Task.CompletedTask;
        }
    }
}
