using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphQL.Query.Builder
{
    public class CollectionQuery<TSource> : Query<TSource> where TSource : DynamicObject
    {
        public CollectionQuery(string name, QueryType type = QueryType.Query) : base(name, type)
        {
        }

        /// <summary>Adds a sub-object field to the query.</summary>
        /// <typeparam name="TSubSource">The sub-object type.</typeparam>
        /// <param name="selector">The field selector.</param>
        /// <param name="build">The sub-object query building function.</param>
        /// <returns>The query.</returns>
        public CollectionQuery<TSource> AddField<TSubSource>(
            Expression<Func<TSource, TSubSource>> selector,
            Func<IQuery<TSubSource>, IQuery<TSubSource>> build)
            where TSubSource : DynamicObject
        {
            RequiredArgument.NotNull(selector, nameof(selector));
            RequiredArgument.NotNull(build, nameof(build));

            PropertyInfo property = GetPropertyInfo<TSubSource>(selector);
            string name = this.GetPropertyName(property);

            return (CollectionQuery<TSource>)this.AddField(name, build);
        }

        private PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<TSource, TProperty>> lambda) where TProperty : class
        {
            RequiredArgument.NotNull(lambda, nameof(lambda));

            if (lambda.Body is not MemberExpression member)
            {
                throw new ArgumentException($"Expression '{lambda}' body is not member expression.");
            }

            if (member.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException($"Expression '{lambda}' not refers to a property.");
            }

            if (propertyInfo.ReflectedType is null)
            {
                throw new ArgumentException($"Expression '{lambda}' not refers to a property.");
            }

            Type type = typeof(TSource);
            if (type != propertyInfo.ReflectedType && !propertyInfo.ReflectedType.IsAssignableFrom(type))
            {
                throw new ArgumentException($"Expression '{lambda}' refers to a property that is not from type {type}.");
            }

            return propertyInfo;
        }
    }
}

