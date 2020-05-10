using System;
using System.Collections.Generic;
using System.Text;

namespace AutoStep.Web.Scripts
{
    public class ScriptException : Exception
    {
        public ScriptException(string? message) : base(message)
        {
        }

        public ScriptException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
