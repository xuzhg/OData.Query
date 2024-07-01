//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Collections;
using System.Linq.Expressions;

namespace Microsoft.OData.Query.Builders;

public abstract class ODataQueryable : IOrderedQueryable
{
    /// <summary>
    /// Gets the type of the object used in the query to create the <see cref="ODataQueryable{TElement}" /> instance.
    /// </summary>
    public abstract Type ElementType { get; }

    /// <summary>
    /// Gets an expression containing the query to the query.
    /// </summary>
    public abstract Expression Expression { get; }

    /// <summary>
    /// Ges the query provider instance.
    /// </summary>
    /// <returns>An <see cref="IQueryProvider" /> representing the data source provider.</returns>
    public abstract IQueryProvider Provider { get; }

    /// <summary>
    /// Gets the <see cref="IEnumerator" /> object that can be used to iterate through the collection returned by the query.
    /// </summary>
    public IEnumerator GetEnumerator()
    {
        // We don't need to implement this since we are only to build the query string.
        throw new NotImplementedException();
    }
}


