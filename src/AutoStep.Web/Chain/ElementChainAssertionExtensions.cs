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

            return chain.AddNode($"{nameof(AssertAttribute)}('{attributeName}', '{attributeValue}')", elements =>
            {
                for (var idx = 0; idx < elements.Count; idx++)
                {
                    var actualAttributeValue = elements[idx].GetAttribute(attributeName);

                    if (actualAttributeValue != attributeValue)
                    {
                        throw new AssertionException($"Expecting an '{attributeName}' of '{attributeValue}' for element at index {idx} but found '{actualAttributeValue}'.");
                    }
                }
            });
        }
    }
}
