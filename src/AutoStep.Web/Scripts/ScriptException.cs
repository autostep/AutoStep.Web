using System;

namespace AutoStep.Web.Scripts
{
    /// <summary>
    /// Defines exceptions that occur when loading/invoking JS.
    /// </summary>
    public class ScriptException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ScriptException(string? message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ScriptException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
