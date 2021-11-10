#region

using System;
using LeChuck.Stateless.StateMachine.Models;

#endregion

namespace LeChuck.Stateless.StateMachine
{
    public class StateMachineWorkflowStateBuilder
    {
        private readonly StepMachineState _stepState;

        public StateMachineWorkflowStateBuilder(string state)
        {
            _stepState = new StepMachineState(state);
        }

        public StateMachineWorkflowStateBuilder WithNext(string command, string newState)
        {
            _stepState.OnNext = command;
            return AddCommand(command, newState);
        }

        public StateMachineWorkflowStateBuilder WithPrev(string command, string newState)
        {
            _stepState.OnPrev = command;
            return AddCommand(command, newState);
        }

        public StateMachineWorkflowStateBuilder AddCommand(string command, string newState)
        {
            _stepState.AvailableCommands.Add(command, newState);
            return this;
        }

        public StateMachineWorkflowStateBuilder IsEndState()
        {
            _stepState.EndMachine = true;
            return this;
        }

        public StepMachineState Build() => _stepState;
    }

    public class StateMachineWorkflowBuilder<T> where T : StateMachineWorkflow, new()
    {
        private readonly T _workflow;

        public StateMachineWorkflowBuilder(string name)
        {
            _workflow = new T {Name = name};
        }

        public StateMachineWorkflowBuilder<T> WithInitialState(string state)
        {
            _workflow.InitialState = state;
            return this;
        }

        public StateMachineWorkflowBuilder<T> AddState(string state,
            Action<StateMachineWorkflowStateBuilder> builderAction)
        {
            var stateBuilder = new StateMachineWorkflowStateBuilder(state);
            builderAction(stateBuilder);
            _workflow.StateList.Add(stateBuilder.Build());
            return this;
        }

        public T Build() => _workflow;
    }
}