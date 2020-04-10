using System;
using System.Text;
using AutoStep.Execution.Dependency;
using AutoStep.Execution.Interaction;
using Microsoft.Extensions.DependencyInjection;

namespace AutoStep.Web
{

    public static class ResolveExtensions
    {
        public static Browser Browser(this IServiceProvider scope)
        {
            return scope.GetRequiredService<Browser>();
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
