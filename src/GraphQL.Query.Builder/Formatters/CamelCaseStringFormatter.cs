using System;
using System.Reflection;

namespace GraphQL.Query.Builder
{
    public class CamelCaseStringFormatter
    {
        /// <summary>Formats the property name in camel case.</summary>
        /// <value>The property.</value>
        public static Func<string, string> Formatter = property =>
        {
            RequiredArgument.NotNull(property, nameof(property));
            return char.ToLowerInvariant(property[0]) + property.Substring(1);
        };
    }
}

