﻿#region

using System;
using System.Linq;

#endregion

namespace LeChuck.Stateless.StateMachine.Extensions
{
    public static class StateMachineExtensions
    {
        public static Type GetStateMachineInterface(this Type machineType)
        {
            return machineType.GetInterfaces().FirstOrDefault(i => i.GetInterfaces().Contains(typeof(IStateMachine)));
        }
    }
}