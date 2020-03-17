using System.Text;
using AutoStep.Execution.Dependency;
using AutoStep.Execution.Interaction;

namespace AutoStep.Web
{

    public static class ResolveExtensions
    {
        public static Browser Browser(this IServiceScope scope)
        {
            return scope.Resolve<Browser>();
        }
    }

    public static class MethodContextExtensions
    {
        public static ElementsQuery? Elements(this MethodContext ctxt)
        {
            return ctxt.ChainValue as ElementsQuery;
        }

        public static void Elements(this MethodContext ctxt, ElementsQuery elementsQuery)
        {
            ctxt.ChainValue = elementsQuery;
        }
    }
}
