//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Collections;
using System.Globalization;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Microsoft.OData.Query.Builders;

public class ODataQueryProviderOptions
{

}



public interface IODataQueryableSingle<TElement>
{
}

public class ODataQueryableSingle<TElement> : IODataQueryableSingle<TElement>
{
    public ODataQueryableSingle(IQueryable<TElement> source)
    {

    }
}

public static class QueryBuilderExtensions
{
    public static IODataQueryableSingle<TElement> ByKey<TElement>(this IQueryable<TElement> query)
    {
        return new ODataQueryableSingle<TElement>(query);
    }

    public static IODataQueryableSingle<TElement> ByKey<TElement>(this IQueryable<TElement> query, Dictionary<string, object> keys)
    {
        return new ODataQueryableSingle<TElement>(query);
    }

    public static IODataQueryableSingle<TElement> Expand<TElement>(this IODataQueryableSingle<TElement> query, string path)
    {
        return query;
    }

    public static IODataQueryableSingle<TElement> Expand<TElement, TTarget>(this IODataQueryableSingle<TElement> query,
        Expression<Func<TElement, TTarget>> navigationPropertyAccessor)
    {
        throw new NotImplementedException();
    }

    public static IODataQueryableSingle<TElement> Select<TElement>(this IODataQueryableSingle<TElement> query, string path)
    {
        return query;
    }

    public static IODataQueryableSingle<TTarget> Select<TElement, TTarget>(this IODataQueryableSingle<TElement> query,
        Expression<Func<TElement, TTarget>> selector)
    {
        throw new NotImplementedException();
    }


    public static ODataQueryable<TElement> CreateQuery<TElement>(this IQueryable<TElement> query)
    {
        return new ODataQueryable<TElement>(query.Expression);
    }


    public static IQueryable<TElement> Expand<TElement>(this IQueryable<TElement> query, string path)
    {
        return query;
    }

    public static IQueryable<TElement> Expand<TElement, TTarget>(this IQueryable<TElement> query,
        Expression<Func<TElement, TTarget>> navigationPropertyAccessor)
    {
        throw new NotImplementedException();
    }

    public static string ToQuery<TElement>(this IQueryable<TElement> query)
    {
        return query.ToString();
    }

    public static string ToQuery<TElement>(this IQueryable<TElement> query, ODataQueryProviderOptions options)
    {
        return query.ToString();
    }

    public static string ToQuery(this ODataQueryable query)
    {
        return query.ToString();
    }

    public static string ToQuery<T>(this ODataQueryable<T> query)
    {
        return query.ToString();
    }

    public static string ToQuery(this ODataQueryable query, ODataQueryProviderOptions options)
    {
        return query.ToString();
    }

    public static string ToQuery<T>(this ODataQueryable<T> query, ODataQueryProviderOptions options)
    {
        return query.ToString();
    }
}

public class Customer
{
    public int Id { get; set; }

    public Order Order { get; set; }
}

public class Order { }

public class QueryBuilderContext
{
    public void Usage()
    {
        ODataQueryable<Customer> queryable = new ODataQueryable<Customer>();
        queryable.Where(c => c.Id == 5)
            .Expand(c => c.Order).ToString();

        IQueryable<Customer> customers = new List<Customer>().AsQueryable();

        customers.Expand(c => c.Order).ToQuery();

        
    }
}

public interface IODataQueryOptionBuilder
{
    string Build(Expression expression, QueryBuilderContext context);
}


