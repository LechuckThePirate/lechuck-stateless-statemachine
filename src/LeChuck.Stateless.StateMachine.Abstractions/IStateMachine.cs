using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeChuck.StateMachine
{
    public interface IStateMachine 
    {
        string MachineId { get; set; }
        string State { get; }
        string SerializeData();
        void DeserializeData(string data);
        Type MachineType { get; }
        Task ExecuteCommand(string command, string payload = null);
        Task ExecuteStep(string payload = null);
        Task Reset();
        Task NextStep();
        Task PrevStep();
        IEnumerable<string> GetAvailableCommands();
    }
}