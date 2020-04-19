using System;

namespace LeChuck.StateMachine.Exceptions
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException(string state) : base($"Invalid state: {state}") { }
    }
}