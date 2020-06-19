using System.Linq;
using AutoStep.Assertion;

namespace AutoStep.Web.Chain
{
    /// <summary>
    /// Extension methods for executing assertion chain operations.
    /// </summary>
    public static class ElementChainAssertionExtensions
    {
        /// <summary>
        /// Add an assertion operation to the element chain that checks for a single element. If there is not exactly 1 element present, an assertion failure will occur.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain AssertSingle(this IElementChain chain)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            return chain.AddNode($"{nameof(AssertSingle)}()", elements =>
            {
                if (elements.SingleOrDefault() is null)
                {
                    throw new AssertionException($"Expecting a single element, but found {elements.Count}.");
                }
            });
        }

        /// <summary>
        /// Add an assertion operation to the element chain that checks that there are at least the <paramref name="minimum"/> number of elements.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <param name="minimum">The minimum number of elements required.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain AssertAtLeast(this IElementChain chain, int minimum)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            return chain.AddNode($"{nameof(AssertAtLeast)}({minimum})", elements =>
            {
                if (elements.Count < minimum)
                {
                    throw new AssertionException($"Expecting at least {minimum} element(s), but found {elements.Count}.");
                }
            });
        }

        /// <summary>
        /// Add an assertion operation to the element chain that checks that all the elements in the set have the specified class in their class list.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <param name="className">The class name to require.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain AssertHasClass(this IElementChain chain, string className)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            if (string.IsNullOrEmpty(className))
            {
                throw new System.ArgumentException(ElementChainMessages.BlankStringParameter, nameof(className));
            }

            return chain.AddForEachNode($"{nameof(AssertHasClass)}('{className}')", (el, idx) =>
            {
                var classList = el.GetClassList();

                if (!classList.Contains(className))
                {
                    throw new AssertionException($"Expecting the element at index {idx} to have the class {className}, but actual class list is \"{string.Join(' ', classList)}\".");
                }
            });
        }

        /// <summary>
        /// Add an assertion operation to the element chain that checks that all the elements in the set have the specified class in their class list.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <param name="className">The class name to require.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain AssertDoesNotHaveClass(this IElementChain chain, string className)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            if (string.IsNullOrEmpty(className))
            {
                throw new System.ArgumentException(ElementChainMessages.BlankStringParameter, nameof(className));
            }

            return chain.AddForEachNode($"{nameof(AssertDoesNotHaveClass)}('{className}')", (el, idx) =>
            {
                var classList = el.GetClassList();

                if (classList.Contains(className))
                {
                    throw new AssertionException($"Expecting the element at index {idx} not to have the class {className}; actual class list is \"{string.Join(' ', classList)}\".");
                }
            });
        }

        /// <summary>
        /// Add an assertion operation to the element chain that checks that all the elements in the set have the specified <paramref name="attributeValue"/> for the
        /// <paramref name="attributeName"/> attribute.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <param name="attributeName">The attribute name.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain AssertAttribute(this IElementChain chain, string attributeName, string attributeValue)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            if (string.IsNullOrEmpty(attributeName))
            {
                throw new System.ArgumentException(ElementChainMessages.BlankStringParameter, nameof(attributeName));
            }

            attributeValue ??= string.Empty;

            return chain.AddForEachNode($"{nameof(AssertAttribute)}('{attributeName}', '{attributeValue}')", (el, idx) =>
            {
                var actualAttributeValue = el.GetAttribute(attributeName);

                if (actualAttributeValue != attributeValue)
                {
                    throw new AssertionException($"Expecting a '{attributeName}' of '{attributeValue}' for element at index {idx} but found '{actualAttributeValue}'.");
                }
            });
        }

        /// <summary>
        /// Add an assertion operation to the element chain that checks that all the elements in the set are enabled.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain AssertEnabled(this IElementChain chain)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            return chain.AddForEachNode($"{nameof(AssertEnabled)}()", (el, idx) =>
            {
                if (el.GetBooleanProperty("disabled"))
                {
                    throw new AssertionException($"Expecting element at index {idx} to be enabled, but the element was disabled.");
                }
            });
        }

        /// <summary>
        /// Add an assertion operation to the element chain that checks that all the elements in the set are disabled.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain AssertDisabled(this IElementChain chain)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            return chain.AddForEachNode($"{nameof(AssertDisabled)}()", (el, idx) =>
            {
                if (!el.GetBooleanProperty("disabled"))
                {
                    throw new AssertionException($"Expecting element at index {idx} to be disabled, but the element was enabled.");
                }
            });
        }

        /// <summary>
        /// Add an assertion operation to the element chain that checks that all the elements in the set have the specified trimmed innerText.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <param name="expectedText">The text to assert on.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain AssertText(this IElementChain chain, string expectedText)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            expectedText ??= string.Empty;

            return chain.AddForEachNode($"{nameof(AssertText)}('{expectedText}')", (el, idx) =>
            {
                var actualText = el.Text;

                if (actualText != expectedText)
                {
                    throw new AssertionException($"Expecting an element text of '{expectedText}' for element at index {idx} but found '{actualText}'.");
                }
            });
        }

        /// <summary>
        /// Add an assertion operation to the element chain that checks that none of the elements in the set have the specified trimmed innerText.
        /// </summary>
        /// <param name="chain">The element chain.</param>
        /// <param name="expectedText">The text to assert on.</param>
        /// <returns>The new element chain.</returns>
        public static IElementChain AssertTextIsNot(this IElementChain chain, string expectedText)
        {
            if (chain is null)
            {
                throw new System.ArgumentNullException(nameof(chain));
            }

            expectedText ??= string.Empty;

            return chain.AddForEachNode($"{nameof(AssertTextIsNot)}('{expectedText}')", (el, idx) =>
            {
                var actualText = el.Text;

                if (actualText == expectedText)
                {
                    throw new AssertionException($"Expecting an element text of '{expectedText}' for element at index {idx} but found '{actualText}'.");
                }
            });
        }
    }
}
