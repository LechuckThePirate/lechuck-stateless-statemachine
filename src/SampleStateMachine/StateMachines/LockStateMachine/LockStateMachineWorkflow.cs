using System;
using System.Collections.Generic;
using System.Text;
using LeChuck.Stateless.StateMachine.Models;

namespace SampleStateMachine.StateMachines.LockStateMachine
{
    public class LockStateMachineWorkflow : StateMachineWorkflow
    {
        public class States
        {
            public const string Open = "Open";
            public const string Closed = "Closed";
            public const string Locked = "Locked";
            public const string Finished = "Finished";
        }

        public class Commands
        {
            public const string Open = "Open";
            public const string Close = "Close";
            public const string Lock = "Lock";
            public const string Unlock = "Unlock";
            public const string Exit = "Exit";
        }

        public override string InitialState { get; } = States.Open;
        public override IEnumerable<StepMachineState> StateList { get; } = new List<StepMachineState>
        {
            new StepMachineState(States.Open)
            {
                AvailableCommands = new Dictionary<string, string>
                {
                    {Commands.Close, States.Closed },
                    {Commands.Exit, States.Finished }
                }
            },
            new StepMachineState(States.Closed)
            {
                AvailableCommands = new Dictionary<string, string>
                {
                    {Commands.Open, States.Open},
                    {Commands.Lock, States.Locked},
                    {Commands.Exit, States.Finished }
                }
            },
            new StepMachineState(States.Locked)
            {
                AvailableCommands = new Dictionary<string, string>
                {
                    {Commands.Unlock, States.Closed},
                    {Commands.Exit, States.Finished }
                }
            }
        };

    }
}
