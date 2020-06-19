using System.Globalization;

namespace AutoStep.Web
{
    /// <summary>
    /// Extension methods for formatting strings in the correct culture.
    /// </summary>
    public static class FormatExtensions
    {
        /// <summary>
        /// Uses a string to format some arguments, in the correct culture.
        /// </summary>
        /// <param name="fmt">The format string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatWith(this string fmt, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, fmt, args);
        }
    }
}
