#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LeChuck.Stateless.StateMachine.Exceptions;
using LeChuck.Stateless.StateMachine.Extensions;
using LeChuck.Stateless.StateMachine.Models;
using Microsoft.Extensions.Logging;

#endregion

namespace LeChuck.Stateless.StateMachine
{
    public abstract class StateMachine<TContext, TEntity> : IStateMachine<TContext, TEntity>
        where TContext : class
        where TEntity : class, new()
    {
        protected TEntity Entity = new TEntity();
        protected Dictionary<string, string> StateParams { get; set; } = new Dictionary<string, string>();
        public string MachineId { get; set; }
        public string State { get; protected set; }
        public Type MachineType { get; set; }

        protected readonly StateMachineWorkflow Workflow;
        private readonly IStateMachineStore _store;
        private readonly ILogger _logger;
        private readonly IStateMachineStrategySelector<TContext, TEntity> _strategySelector;

        protected StateMachine(StateMachineWorkflow workflow, IStateMachineStore stateMachineStore, ILogger logger,
            IStateMachineStrategySelector<TContext, TEntity> strategySelector)
        {
            Workflow = workflow ?? throw new ArgumentNullException(nameof(workflow));
            State = workflow.InitialState ?? throw new ArgumentException("No initial state was specified");
            _store = stateMachineStore ?? throw new ArgumentNullException(nameof(stateMachineStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _strategySelector = strategySelector;
            MachineType = GetType().GetStateMachineInterface();
        }

        protected virtual async Task<bool> OnCommand(string command, TContext context = default)
        {
            _logger.LogInformation($"OnCommand('{command}','{context}')");
            return await RunStrategy(context, command);
        }

        protected virtual async Task OnNewState(string newState, TContext context = default)
        {
            _logger.LogInformation($"OnNewState('{newState}')");
            await RunStrategy(context, newState);
        }

        public TEntity GetEntity()
        {
            return Entity;
        }

        public virtual async Task ExecuteCommand(string command, TContext context = null)
        {
            if (!Workflow.GetState(State).AvailableCommands.ContainsKey(command))
                throw new BadCommandException();

            if (await OnCommand(command, context))
                await SetNewState(Workflow.GetState(State).AvailableCommands[command], context);

            await PersistMachine();
        }

        public virtual async Task Reset()
        {
            await SetNewState(Workflow.InitialState);
            await PersistMachine();
        }

        public IEnumerable<string> GetAvailableCommands()
        {
            return Workflow.StateList?.FirstOrDefault(s => s.Name == State)?
                .AvailableCommands?.Select(k => k.Key);
        }

        public async Task ExecuteCommand(string command, dynamic context = null)
        {
            await ExecuteCommand(command, context as TContext);
        }

        public async Task ExecuteStep(object context = default)
        {
            await ExecuteStep(context as TContext);
        }

        public async Task Run(string startInState = null, object context = default, object entity = default)
        {
            await Run(startInState, context as TContext, entity as TEntity);
        }

        public void SetParameter(string key, object value)
        {
            if (StateParams.ContainsKey(key))
                StateParams[key] = JsonSerializer.Serialize(value);
            else
                StateParams.Add(key, JsonSerializer.Serialize(value));
        }

        public T GetParameter<T>(string key)
        {
            try
            {
                return (StateParams.ContainsKey(key))
                    ? JsonSerializer.Deserialize<T>(StateParams[key])
                    : default;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Parameter type mismatch! {key}\n" +
                                   $"{ex.Message}\n" +
                                   $"{ex.StackTrace}");
                return default;
            }
        }

        public StepMachineState GetCurrentState()
        {
            return GetState(State);
        }

        public StepMachineState GetState(string state)
        {
            return Workflow.StateList.FirstOrDefault(s => s.Name.ToString() == state)
                   ?? throw new InvalidStateException(state);
        }

        public async Task Run(string startInState = null, TContext context = default, TEntity entity = default)
        {
            State = startInState ?? State;
            Entity = entity ?? new TEntity();
            await OnNewState(State, context);
            await PersistMachine();
        }

        public void SetEntity(TEntity entity)
        {
            this.Entity = entity;
        }

        protected virtual async Task SetNewState(string newState, TContext context = default)
        {
            State = newState;
            await OnNewState(newState, context);
        }

        protected async Task PersistMachine()
        {
            if (GetCurrentState().EndMachine)
                await _store.DeleteMachine(MachineId);
            else
                await _store.StoreMachine(MachineId, MachineType, SerializeData());
        }

        public virtual string SerializeData()
        {
            var data = new
            {
                MachineId,
                State,
                Entity = JsonSerializer.Serialize(Entity),
                StateParams = JsonSerializer.Serialize(StateParams)
            };

            return JsonSerializer.Serialize(data);
        }

        public virtual void DeserializeData(string data)
        {
            var dict = JsonSerializer.Deserialize<IDictionary<string, object>>(data);
            MachineId = dict.ContainsKey(nameof(MachineId)) ? dict[nameof(MachineId)]?.ToString() : null;
            State = dict.ContainsKey(nameof(State)) ? dict[nameof(State)]?.ToString() : default;
            Entity = (dict.ContainsKey(nameof(Entity)))
                ? JsonSerializer.Deserialize<TEntity>(dict[nameof(Entity)].ToString())
                : null;
            StateParams = (dict.ContainsKey(nameof(StateParams)))
                ? JsonSerializer.Deserialize<Dictionary<string, string>>(dict[nameof(StateParams)].ToString())
                : new Dictionary<string, string>();
        }

        protected async Task<bool> RunStrategy(TContext context, string strategyName)
        {
            var strategy = _strategySelector.GetHandlerFor(strategyName);
            try
            {
                if (strategy == null)
                {
                    _logger.LogInformation($"No strategy found for '{strategyName}'");
                    return true;
                }

                return await strategy.Handle(context, this);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing strategy {strategyName}\n" +
                                 $"{ex.Message}\n" +
                                 $"{ex.StackTrace}", ex);
                return false;
            }
        }

        public override string ToString()
        {
            return $"Id: '{MachineId}', State: '{State}'";
        }
    }
}