//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// Node representing count of related entities or items within a collection-valued property.
/// </summary>
public sealed class DollarCountNode : SingleValueNode
{
    /// <summary>
    /// Constructs a new <see cref="DollarCountNode"/>.
    /// </summary>
    /// <param name="source">The value containing the property.</param>
    /// <exception cref="System.ArgumentNullException">Throws if the input source is null.</exception>
    public DollarCountNode(CollectionValueNode source)
    //    : this(source, null, null)
    {
      //  ExceptionUtils.CheckArgumentNotNull(source, "source");

        Source = source;
    }

    ///// <summary>
    ///// Constructs a new <see cref="CountNode"/>.
    ///// </summary>
    ///// <param name="source">The value containing the property.</param>
    ///// <param name="filterClause">The <see cref="Microsoft.OData.UriParser.FilterClause"/>in the count node.</param>
    ///// <param name="searchClause">The <see cref="Microsoft.OData.UriParser.SearchClause"/>in the count node.</param>
    ///// <exception cref="System.ArgumentNullException">Throws if the input source is null.</exception>
    //public CountNode(CollectionValueNode source, FilterClause filterClause, SearchClause searchClause)
    //{
    //   // ExceptionUtils.CheckArgumentNotNull(source, "source");

    //    this.source = source;
    //    this.filterClause = filterClause;
    //    this.searchClause = searchClause;
    //}

    /// <summary>
    /// Gets the collection property node to be counted.
    /// </summary>
    public CollectionValueNode Source { get; }


    ///// <summary>
    ///// Gets the filter node.
    ///// </summary>
    //public FilterClause FilterClause
    //{
    //    get { return this.filterClause; }
    //}

    ///// <summary>
    ///// Gets the search node.
    ///// </summary>
    //public SearchClause SearchClause
    //{
    //    get { return this.searchClause; }
    //}

    /// <summary>
    /// Gets the value type this node represents.
    /// </summary>
    public override Type NodeType => typeof(long);

    public override QueryNodeKind Kind => QueryNodeKind.DollarCount;
}
