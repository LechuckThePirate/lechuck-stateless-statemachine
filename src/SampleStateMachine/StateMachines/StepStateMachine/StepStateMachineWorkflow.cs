using System;
using System.Collections.Generic;
using LeChuck.StateMachine.Models;

namespace SampleStateMachine.StateMachines.StepStateMachine
{
    public class StepStateMachineWorkflow : StateMachineWorkflow
    {
        public class States
        {
            public const string Created = "Created";
            public const string InputName = "InputName";
            public const string InputEmail = "InputEmail";
            public const string Confirmation = "Confirmation";
            public const string Done = "Done";
            public const string Cancelled = "Cancelled";
        }


        public override string InitialState { get; } = States.Created;
        public override IEnumerable<StepMachineState> StateList { get; } = new List<StepMachineState>
        {
            new StepMachineState(States.Created)
            {
                NextState = States.InputName,
            },
            new StepMachineState(States.InputName)
            {
                NextState = States.InputEmail,
            },
            new StepMachineState(States.InputEmail)
            {
                NextState = States.Confirmation,
                PrevState = States.InputName,
            },
            new StepMachineState(States.Confirmation)
            {
                NextState = States.Done,
                PrevState = States.InputEmail,
            },
            new StepMachineState(States.Done),
            new StepMachineState(States.Cancelled)
        };
    }
}
