using System;
using System.Threading.Tasks;
using LeChuck.Stateless.StateMachine;
using Microsoft.Extensions.Logging;

namespace SampleStateMachine.StateMachines.LockStateMachine
{

    public interface ILockStateMachine : IStateMachine<object> { }
    public class LockStateMachine : StateMachine<object>, ILockStateMachine
    {
        private readonly ILogger<LockStateMachine> _logger;

        public LockStateMachine(LockStateMachineWorkflow workflow, IStateMachineStore store, ILogger<LockStateMachine> logger) : base(workflow, store)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<bool> OnCommand(string command, object payload = null)
        {
            _logger.LogTrace($"OnCommand('{command}'");
            return await Task.FromResult(true);
        }

        protected override async Task OnNewState(string newState, object payload = null)
        {
            _logger.LogTrace($"OnNewState('{newState}'");
            await Task.CompletedTask;
        }

        protected override async Task<bool> OnNextStep(string currentState, object payload = null)
        {
            _logger.LogTrace($"OnNextStep('{currentState}'");
            return await Task.FromResult(true);
        }
    }
}
