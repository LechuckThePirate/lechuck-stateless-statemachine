#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace LeChuck.Stateless.StateMachine.Models
{
    public class StateMachineWorkflow
    {
        public string InitialState { get; set; }
        public List<StepMachineState> StateList { get; } = new List<StepMachineState>();
        public string Name { get; set; }

        public StepMachineState GetState(string state) => StateList.FirstOrDefault(s => s.Name == state);

        public class BaseStates
        {
            public const string Done = "done";
            public const string Cancelled = "cancelled";
        }

        public class BaseCommands
        {
            public const string Next = "next";
            public const string Back = "back";
            public const string Cancel = "cancel";
        }
    }
}