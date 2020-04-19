using System;
using System.Threading.Tasks;
using LeChuck.Stateless.StateMachine;
using LeChuck.StateMachine;
using Microsoft.Extensions.Logging;

namespace SampleStateMachine.StateMachines.LockStateMachine
{

    public interface ILockStateMachine : IStateMachine { }
    public class LockStateMachine : StateMachine, ILockStateMachine
    {
        private readonly ILogger<LockStateMachine> _logger;

        public LockStateMachine(LockStateMachineWorkflow workflow, IStateMachineStore store, ILogger<LockStateMachine> logger) : base(workflow, store)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<bool> OnCommand(string command, string payload = null)
        {
            _logger.LogTrace($"OnCommand('{command}'");
            return await Task.FromResult(true);
        }

        protected override async Task OnNewState(string newState)
        {
            _logger.LogTrace($"OnNewState('{newState}'");
            await Task.CompletedTask;
        }

        protected override async Task<bool> OnNextStep(string currentState, string payload = null)
        {
            _logger.LogTrace($"OnNextStep('{currentState}'");
            return await Task.FromResult(true);
        }
    }
}
