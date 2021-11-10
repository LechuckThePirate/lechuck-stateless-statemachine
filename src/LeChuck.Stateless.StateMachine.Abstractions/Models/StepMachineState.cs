#region

using System.Collections.Generic;

#endregion

namespace LeChuck.Stateless.StateMachine.Models
{
    public class StepMachineState
    {
        public string Name { get; set; }
        public string OnNext { get; set; }
        public string OnPrev { get; set; }
        public Dictionary<string, string> AvailableCommands { get; set; } = new Dictionary<string, string>();
        public bool EndMachine { get; set; }

        public StepMachineState()
        {
        }

        public StepMachineState(string name)
        {
            Name = name;
        }
    }
}