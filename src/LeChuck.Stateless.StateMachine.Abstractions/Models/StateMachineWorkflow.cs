using System.Collections.Generic;

namespace LeChuck.StateMachine.Models
{
    public abstract class StateMachineWorkflow
    {
        public abstract string InitialState { get; }
        public abstract IEnumerable<StepMachineState> StateList { get; }
        public string Name { get; set; }
    }
}
