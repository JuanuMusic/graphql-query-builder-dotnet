using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace GraphQL.Query.Builder
{
    public static class Extensions
    {
        public static GraphQLObject ToGraphQLObject(this object obj)
        {
            // Null-check
            var retVal = new GraphQLObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj.GetType()))
            {
                retVal[property.Name] = property.GetValue(obj);
            }

            return retVal;
        }

        public static GraphQLObject ToGraphQLObject<T>(this Dictionary<string, T> obj)
        {
            // Null-check
            var retVal = new GraphQLObject();
            foreach (KeyValuePair<string, T> kvp in obj)
            {
                retVal[kvp.Key] = kvp.Value;
            }

            return retVal;
        }
    }
}

