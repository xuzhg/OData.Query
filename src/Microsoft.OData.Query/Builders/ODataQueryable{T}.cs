//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Collections;
using System.Linq.Expressions;

namespace Microsoft.OData.Query.Builders;

public class ODataQueryable<TElement> : ODataQueryable, IOrderedQueryable<TElement>
{
    private readonly Expression _queryExpression;
    private readonly IQueryProvider _queryProvider;

    public ODataQueryable()
        : this(Expression.Parameter(typeof(TElement)))
    { }

    public ODataQueryable(Expression expression) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ODataQueryable{TElement}" /> class.
    /// </summary>
    /// <param name="expression">The expression for query.</param>
    /// <param name="provider">The query provider for query.</param>
    public ODataQueryable(Expression expression, IQueryProvider provider)
    {
        _queryExpression = expression ?? throw new ArgumentNullException(nameof(expression));
        _queryProvider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <summary>
    /// Gets the type of the object used in the query to create the <see cref="ODataQueryable{TElement}" /> instance.
    /// </summary>
    /// <returns>Returns <see cref="Type" /> representing the type used in the template when the query is created.</returns>
    public override Type ElementType => typeof(TElement);

    /// <summary>
    /// Represents an expression containing the query to the data service.
    /// </summary>
    /// <returns>A <see cref="Expression" /> object representing the query.</returns>
    public override Expression Expression => _queryExpression;

    /// <summary>
    /// Represents the query provider instance.
    /// </summary>
    /// <returns>A <see cref="IQueryProvider" /> representing the data source provider.</returns>
    public override IQueryProvider Provider => _queryProvider;

    /// <summary>
    /// Gets the <see cref="IEnumerator" /> object that can be used to iterate through the collection returned by the query.
    /// </summary>
    /// <returns>An enumerator over the query results.</returns>
    IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
    {
        // We don't need to implement this since we are only to build the query string.
        throw new NotImplementedException();
    }

    public virtual ODataQueryable<TElement> Expand(string path)
    {
        return this;
    }

    public virtual ODataQueryable<TElement> Expand<TTarget>(Expression<Func<TElement, TTarget>> navigationPropertyAccessor)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Represents the URI of the query to the data service.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override string ToString()
    {
        throw new NotImplementedException();
    }
}


