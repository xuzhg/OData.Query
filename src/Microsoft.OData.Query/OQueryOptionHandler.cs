//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Binders;
using Microsoft.OData.Query.Expressions;

namespace Microsoft.OData.Query;

public class OQueryOptionHandler : IOQueryOptionHandler
{
    public OQueryOptionHandler()
    {

    }

    public virtual ValueTask<IQueryable> ApplyTo(IQueryable query, OQueryOptionSettings settings)
    {
        if (query == null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        throw new NotImplementedException();
    }

    public virtual ValueTask<object> ApplyTo(object entity, OQueryOptionSettings settings)
    {
        throw new NotImplementedException();
    }
}

public static class IQueryExtensions
{
    // the query string is:
    // ?$select=name&$expand=orders
    // & in the value should be escaped as '%26'
    public static IQueryable ApplyTo<T>(this IQueryable<T> source, string query, OQueryOptionSettings settings)
    {
        //

        return source;
    }

    public static IQueryable ApplyTo(this IQueryable source, string query, OQueryOptionSettings settings)
    {

        return source;
    }
}
