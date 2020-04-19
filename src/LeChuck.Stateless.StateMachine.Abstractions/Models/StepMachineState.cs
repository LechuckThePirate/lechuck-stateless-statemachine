using System;
using System.Collections.Generic;

namespace LeChuck.StateMachine.Models
{
    public class StepMachineState
    {
        public String Name { get; set; }
        public string NextState { get; set; }
        public string PrevState { get; set; }
        public Dictionary<string, string> AvailableCommands { get; set; }

        public StepMachineState() {}

        public StepMachineState(string name)
        {
            Name = name;
        }
    }
}
