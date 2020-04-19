using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LeChuck.Stateless.StateMachine.Extensions;
using LeChuck.StateMachine;
using LeChuck.StateMachine.Exceptions;
using LeChuck.StateMachine.Models;

namespace LeChuck.Stateless.StateMachine
{
    public abstract class StateMachine : IStateMachine 
    {
        public string MachineId { get; set; }
        public string State { get; protected set; }
        public Type MachineType { get; set; }

        protected readonly StateMachineWorkflow Workflow;
        private readonly IStateMachineStore _store;

        protected StateMachine(StateMachineWorkflow workflow, IStateMachineStore store)
        {
            Workflow = workflow ?? throw new ArgumentNullException(nameof(workflow));
            State = workflow.InitialState ?? throw new ArgumentException("No initial state was specified");
            _store = store ?? throw new ArgumentNullException(nameof(store));
            MachineType = this.GetType().GetStateMachineInterface();
        }

        // Abstract methods to implement in derived class
        /// <summary>
        /// It's called whenever a ExecuteCommand is done on the machine
        /// </summary>
        /// <param name="command">The command that was just executed</param>
        /// <param name="payload">The 'payload' or parameters for the command if there are any</param>
        /// <returns>Returning False will cancel the next step operation. I.E: validation failed!</returns>
        protected abstract Task<bool> OnCommand(string command, string payload = null);

        /// <summary>
        /// It's called whenever a ExecuteStep is done on the machine, before the
        /// next step is executed to allow behavior before moving to next step
        /// </summary>
        /// <param name="currentState">Current State of the machine before switching to next step</param>
        /// <param name="payload">The 'payload' or parameters to execute any action before next step</param>
        /// <returns>Returning False will cancel the next step operation. I.E: validation failed!</returns>
        protected abstract Task<bool> OnNextStep(string currentState, string payload);

        /// <summary>
        /// It's called wheneve the machine goes into a new state. Useful for sending info to screen, user, etc...
        /// </summary>
        /// <param name="newState">New machine's state</param>
        protected abstract Task OnNewState(string newState);

        /// <summary>
        /// Executes a command on the machine. If the command is invalid (for that state) will
        /// throw a BadCommandException();
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="payload">Parameters if needed</param>
        public virtual async Task ExecuteCommand(string command, string payload = null)
        {
            var currentState = Workflow.StateList.FirstOrDefault(s => s.Name == State)
                               ?? throw new InvalidStateException(State);

            if (!currentState.AvailableCommands.ContainsKey(command))
                throw new BadCommandException();

            if (await OnCommand(command, payload)) 
                await SetNewState(currentState.AvailableCommands[command]);

            await PersistMachine();
        }

        /// <summary>
        /// Moves the machine to the next step, unless some validation in OnNextStep returns
        /// false.
        /// </summary>
        /// <param name="payload">Optional parameters</param>
        public async Task ExecuteStep(string payload)
        {
            if (await OnNextStep(State, payload))
                await NextStep();
        }

        /// <summary>
        /// Restarts the machine
        /// </summary>
        public virtual async Task Reset()
        {
            await SetNewState(Workflow.InitialState);
            await PersistMachine();
        }

        /// <summary>
        /// Moves the machine to the next step without any validation nor parameters
        /// </summary>
        public async Task NextStep()
        {
            var currentState = Workflow.StateList.FirstOrDefault(s => s.Name == State)
                               ?? throw new InvalidStateException(State);
            var nextState = currentState.NextState ?? throw new InvalidStateException($"No next state for {State}");
            await SetNewState(nextState);
            await PersistMachine();
        }

        /// <summary>
        /// Moves the machine to the previous step without any validation nor parameters.
        /// </summary>
        public async Task PrevStep()
        {
            var currentState = Workflow.StateList.FirstOrDefault(s => s.Name == State)
                               ?? throw new InvalidStateException(State);
            var prevState = currentState.PrevState ?? throw new InvalidStateException($"No prev state for {State}");
            await SetNewState(prevState);
            await PersistMachine();
        }

        /// <summary>
        /// Returns the available commands in the current machine state
        /// </summary>
        /// <returns>List of string commands available in current state</returns>
        public IEnumerable<string> GetAvailableCommands()
        {
            return Workflow.StateList?.FirstOrDefault(s => s.Name == State)?.AvailableCommands?.Select(k => k.Key);
        }

        /// <summary>
        /// Moves the machine to a specific state
        /// </summary>
        /// <param name="newState">New state for the machine</param>
        protected async Task SetNewState(string newState)
        {
            State = newState;
            await OnNewState(State);
        }

        /// <summary>
        /// Persists the machine using the injected IStateMachineStore object
        /// </summary>
        protected async Task PersistMachine()
        {
            await _store.StoreMachine(MachineId, MachineType, SerializeData());
        }

        /// <summary>
        /// Serializes the data to persist in storage. Override if you need to add custom data.
        /// </summary>
        /// <returns>Serialized information of the machine (can be deserialized as Dictionary&lt;string,string&gt;)</returns>
        public virtual string SerializeData()
        {
            var data = new
            {
                MachineId,
                State
            };

            return JsonSerializer.Serialize(data);
        }

        /// <summary>
        /// Deserialize the machine info generated by SerializeData. Override to deserialize additional custom fields
        /// </summary>
        /// <param name="data"></param>
        public virtual void DeserializeData(string data)
        {
            var dict = JsonSerializer.Deserialize<IDictionary<string, object>>(data);
            MachineId = dict.ContainsKey(nameof(MachineId)) ? dict[nameof(MachineId)]?.ToString() : null;
            State = dict.ContainsKey(nameof(State)) ? dict[nameof(State)]?.ToString() : default;
        }

        public override string ToString()
        {
            return $"Id: '{MachineId}', State: '{State}'";
        }
    }
}
