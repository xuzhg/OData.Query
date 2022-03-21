//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.OData.Query
{
    /// <summary>
    /// same as UseMiddlewareExtensions.cs
    /// </summary>
    public static partial class UseQueryExtensions
    {
        internal const string ApplyMethodName = "Apply";
        internal const string ApplyAsyncMethodName = "ApplyAsync";

        private static readonly MethodInfo GetServiceInfo = typeof(UseQueryExtensions).GetMethod(nameof(GetService), BindingFlags.NonPublic | BindingFlags.Static)!;

        /// <summary>
        /// Add a query into the query builder
        /// </summary>
        /// <typeparam name="T">The query type.</typeparam>
        /// <param name="builder">The <see cref="IODataQueryBuilder"/> instance.</param>
        /// <param name="args">The arguments to pass to the query type instance's constructor.</param>
        /// <returns>The <see cref="IODataQueryBuilder"/> instance.</returns>
        public static IODataQueryBuilder UseQuery<T>(this IODataQueryBuilder builder, params object[] args)
        {
            return builder.UseQuery(typeof(T), args);
        }

        /// <summary>
        /// Add a query type into the query builder
        /// </summary>
        /// <param name="builder">The <see cref="IODataQueryBuilder"/> instance.</param>
        /// <param name="queryType">The query type.</param>
        /// <param name="args">The arguments to pass to the query type instance's constructor.</param>
        /// <returns>The <see cref="IODataQueryBuilder"/> instance.</returns>
        public static IODataQueryBuilder UseQuery(this IODataQueryBuilder builder, Type queryType, params object[] args)
        {
            var applicationServices = builder.ApplicationServices;
            return builder.Use(next =>
            {
                var methods = queryType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

                var applyMethods = methods.Where(m =>
                    string.Equals(m.Name, ApplyMethodName, StringComparison.Ordinal) ||
                    string.Equals(m.Name, ApplyAsyncMethodName, StringComparison.Ordinal)).ToArray();

                if (applyMethods.Length > 1)
                {
                    throw new OException("TODO");
                }

                if (applyMethods.Length == 0)
                {
                    throw new OException("TODO");
                }

                var methodInfo = applyMethods[0];
                if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
                {
                    throw new OException("");
                }

                var parameters = methodInfo.GetParameters();
                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(QueryContext))
                {
                    throw new OException("TODO");
                }

                var ctorArgs = new object[args.Length + 1];
                ctorArgs[0] = next;
                Array.Copy(args, 0, ctorArgs, 1, args.Length);

                // TODO:
                var instance = Activator.CreateInstance(queryType, ctorArgs);

                if (parameters.Length == 1)
                {
                    // Only have the QueryContext
                    return (QueryDelegate)methodInfo.CreateDelegate(typeof(QueryDelegate), instance);
                }

                var factory = Compile<object>(methodInfo, parameters);

                return context =>
                {
                    var serviceProvider = context.ContextServices ?? applicationServices;
                    if (serviceProvider == null)
                    {
                        throw new OException("TODO");
                        // throw new InvalidOperationException(Resources.FormatException_UseMiddlewareIServiceProviderNotAvailable(nameof(IServiceProvider)));
                    }

                    return factory(instance, context, serviceProvider);
                };
            });
        }

        private static Func<T, QueryContext, IServiceProvider, Task> Compile<T>(MethodInfo methodInfo, ParameterInfo[] parameters)
        {
            var middleware = typeof(T);

            var httpContextArg = Expression.Parameter(typeof(QueryContext), "queryContext");
            var providerArg = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
            var instanceArg = Expression.Parameter(middleware, "middleware");

            var methodArguments = new Expression[parameters.Length];
            methodArguments[0] = httpContextArg;
            for (int i = 1; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                if (parameterType.IsByRef)
                {
                    throw new OException("Not Supported");
                    // throw new NotSupportedException(Resources.FormatException_InvokeDoesNotSupportRefOrOutParams(InvokeMethodName));
                }

                var parameterTypeExpression = new Expression[]
                {
                    providerArg,
                    Expression.Constant(parameterType, typeof(Type)),
                    Expression.Constant(methodInfo.DeclaringType, typeof(Type))
                };

                var getServiceCall = Expression.Call(GetServiceInfo, parameterTypeExpression);
                methodArguments[i] = Expression.Convert(getServiceCall, parameterType);
            }

            Expression middlewareInstanceArg = instanceArg;
            if (methodInfo.DeclaringType != null && methodInfo.DeclaringType != typeof(T))
            {
                middlewareInstanceArg = Expression.Convert(middlewareInstanceArg, methodInfo.DeclaringType);
            }

            var body = Expression.Call(middlewareInstanceArg, methodInfo, methodArguments);

            var lambda = Expression.Lambda<Func<T, QueryContext, IServiceProvider, Task>>(body, instanceArg, httpContextArg, providerArg);

            return lambda.Compile();
        }

        private static object GetService(IServiceProvider sp, Type type, Type middleware)
        {
            var service = sp.GetService(type);
            if (service == null)
            {
                throw new OException("Invalid");
                // throw new InvalidOperationException(Resources.FormatException_InvokeMiddlewareNoService(type, middleware));
            }

            return service;
        }
    }
}
