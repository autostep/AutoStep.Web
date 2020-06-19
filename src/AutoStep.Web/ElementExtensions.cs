using System;
using System.Globalization;
using System.Linq;
using OpenQA.Selenium;

namespace AutoStep.Web
{
    /// <summary>
    /// Provides extension methods on the selenium <see cref="IWebElement"/> interface.
    /// </summary>
    public static class ElementExtensions
    {
        private static readonly string True = true.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Get a JS property of an element that is boolean in nature (e.g. 'disabled').
        /// </summary>
        /// <param name="element">The web element.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>True if the property value is 'true'; otherwise, false is returned.</returns>
        public static bool GetBooleanProperty(this IWebElement element, string propertyName)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var prop = element.GetProperty(propertyName);

            if (prop is null)
            {
                return false;
            }

            return prop.Equals(True, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Checks if an element has the specified class in its class list.
        /// </summary>
        /// <param name="element">The web element.</param>
        /// <param name="className">The class name.</param>
        /// <returns>True if the class is present; false otherwise.</returns>
        public static bool HasClass(this IWebElement element, string className)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (string.IsNullOrWhiteSpace(className))
            {
                return false;
            }

            return element.GetClassList().Contains(className);
        }

        /// <summary>
        /// Gets the list of classes applied to the element (via the class attribute).
        /// </summary>
        /// <param name="element">The web element.</param>
        /// <returns>The set of class names.</returns>
        public static string[] GetClassList(this IWebElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return element.GetAttribute("class")?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        }
    }
}
