using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace AutoStep.Web
{
    public static class ElementExtensions
    {
        private static readonly string True = true.ToString(CultureInfo.InvariantCulture);

        public static bool GetBooleanProperty(this IWebElement element, string propertyName)
        {
            var prop = element.GetProperty(propertyName);

            if (prop is null)
            {
                return false;
            }

            return prop.Equals(True, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool HasClass(this IWebElement element, string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return false;
            }

            return element.GetClassList().Contains(className);
        }

        public static string[] GetClassList(this IWebElement element)
        {
            return element.GetAttribute("class")?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        }
    }
}
