﻿#region

using System;

#endregion

namespace LeChuck.Stateless.StateMachine.Exceptions
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException(string state) : base($"Invalid state: {state}")
        {
        }
    }
}