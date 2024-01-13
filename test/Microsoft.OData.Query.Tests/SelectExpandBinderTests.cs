//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Microsoft.OData.Query.Tests
{
    public class SelectExpandBinderTests
    {
        static IList<Customer> _customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "Peter", Age= 36,
                Location = new Address { City = "Redmond", Street = "156TH AVE"},
                Properties = new Dictionary<string, object> { { "MyData", 5.4f} } },
            new Customer { Id = 2, Name = "Sam", Age= 34, Location = new Address { City = "Shanghai", Street = "Zixin Rd"},
                Properties = new Dictionary<string, object> { { "MyData", 5.1f} } },
            new Customer
            {
                Id = 3,
                Name = "John",
                Age = 12,
                Location = new Address { City = "Beijing", Street = "Yunan Rd" },
                Properties = new Dictionary<string, object> { { "MyData", 4.1f } }
            },
             new Customer
            {
                Id = 4,
                Name = "Kerry",
                Age = 24,
                Location = new Address { City = "Seattle", Street = "145TH NE ST" },
                Properties = new Dictionary<string, object> { { "MyData", 6.1f } }
            },
             new Customer { Id = 5, Name = "Alex", Age = 78, Location = new Address { City = "Sammamish", Street = "225TH AVE" },

                Properties = new Dictionary<string, object> { { "MyData", 7.1f } } }
        };

        [Fact]
        public void SelectShouldWork()
        {
            LambdaExpression lambda = BuildSelectExpression<Customer>("");
            var a = lambda.ToString();

            Assert.NotNull(a);

            var enumerable = InvokeSelect(_customers, lambda);

            var result = enumerable as IEnumerable<IDictionary<string, object>>;
            Assert.NotNull(result);

            Assert.Equal(5, result.Count());
        }

        private LambdaExpression BuildSelectExpression<T>(string select)
        {
            // Supposed we input as "$select=Name,Age,Location"
            Type entityType = typeof(T);

            ParameterExpression parameter = Expression.Parameter(entityType, "a");

            MemberExpression nameProp = Expression.Property(parameter, "Name");
            MemberExpression ageProp = Expression.Property(parameter, "Age");
            UnaryExpression ageObjectProp = Expression.Convert(ageProp, typeof(object)); // need to convert to object, otherwise, it can't work in ElementInit
            MemberExpression locationProp = Expression.Property(parameter, "Location");

            // We can use IDictionary<string, object>, or use ExpandoObject, need to check with EF or EFCore
            MethodInfo addMethod = typeof(Dictionary<string, object>).GetMethod("Add");

            ElementInit nameInit = Expression.ElementInit(addMethod, Expression.Constant("Name", typeof(string)), nameProp);
            ElementInit ageInit = Expression.ElementInit(addMethod, Expression.Constant("Age", typeof(string)), ageObjectProp);
            ElementInit locationInit = Expression.ElementInit(addMethod, Expression.Constant("Location", typeof(string)), locationProp);

            NewExpression dictNew = Expression.New(typeof(Dictionary<string, object>));

            ListInitExpression dictInit = Expression.ListInit(dictNew, nameInit, ageInit, locationInit);

            return Expression.Lambda(dictInit, parameter);

            // a => new Dictionary<string, object>() { { "Name",  a.Name}, .... }
        }


        private static IEnumerable InvokeSelect<T>(IEnumerable<T> source, LambdaExpression lambda)
        {
            if (lambda == null)
            {
                throw new ArgumentNullException("lambda");
            }

            MethodInfo selectMethod = GetEnumerablSelect();

            selectMethod = selectMethod.MakeGenericMethod(typeof(T), lambda.Body.Type);

            Delegate delegateFunc = lambda.Compile();

            return selectMethod.Invoke(null, new object[] { source, delegateFunc }) as IEnumerable;
        }

        private static MethodInfo GetEnumerablSelect()
        {
            // public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector);
           //  public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector);
            var selectMethods = typeof(Enumerable).GetMethods().Where(x => x.Name == "Select");

            foreach (var selectMethod in selectMethods)
            {
                ParameterInfo[] parameters = selectMethod.GetParameters();
                if (parameters[1].ParameterType.GetGenericArguments().Length == 2)
                {
                    return selectMethod;
                }
            }

            throw new NotSupportedException();
        }
    }

    public class Address
    {
        public string City { get; set; }

        public string Street { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Age { get; set; }

        public Address Location { get; set; }

        public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }
}
