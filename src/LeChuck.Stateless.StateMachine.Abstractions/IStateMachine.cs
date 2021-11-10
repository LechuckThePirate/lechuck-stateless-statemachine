#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#endregion

namespace LeChuck.Stateless.StateMachine
{
    public interface IStateMachine
    {
        string MachineId { get; set; }
        string State { get; }
        string SerializeData();
        Type MachineType { get; }
        void DeserializeData(string data);
        Task Reset();
        IEnumerable<string> GetAvailableCommands();
        Task ExecuteCommand(string command, dynamic context = default);
        Task Run(string startInState = null, object context = default, object entity = default);
        void SetParameter(string key, object value);
        T GetParameter<T>(string key);
    }

    public interface IStateMachine<in TContext, TEntity> : IStateMachine
        where TContext : class
        where TEntity : class
    {
        TEntity GetEntity();
        Task ExecuteCommand(string command, TContext context = default);
        Task Run(string startInState = null, TContext context = default, TEntity entity = default);
        void SetEntity(TEntity entity);
    }
}