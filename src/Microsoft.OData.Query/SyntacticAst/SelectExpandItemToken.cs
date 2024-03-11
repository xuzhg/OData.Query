//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Base class for select and expand item token.
/// </summary>
public abstract class SelectExpandItemToken : IQueryToken
{
    /// <summary>
    /// Initializes a new instance of  <see cref="SelectExpandItemToken"/> class.
    /// </summary>
    /// <param name="pathToProperty">the path to property for this select or expand term</param>
    /// <param name="filterOption">the filter option for this select or expand term</param>
    /// <param name="orderByOptions">the orderby options for this select or expand term</param>
    /// <param name="topOption">the top option for this select or expand term</param>
    /// <param name="skipOption">the skip option for this select or expand term</param>
    /// <param name="countQueryOption">the query count option for this select or expand term</param>
    /// <param name="searchOption">the search option for this select or expand term</param>
    /// <param name="selectOption">the select option for this select or expand term</param>
    /// <param name="computeOption">the compute option for this select or expand term</param>
    protected SelectExpandItemToken(
        SegmentToken pathToProperty,
        IQueryToken filterOption,
        IEnumerable<OrderByToken> orderByOptions,
        long? topOption,
        long? skipOption,
        bool? countQueryOption,
        IQueryToken searchOption,
        SelectToken selectOption,
        ComputeToken computeOption)
    {
       // ExceptionUtils.CheckArgumentNotNull(pathToProperty, "property");

        PathToProperty = pathToProperty;
        Filter = filterOption;
        OrderBy = orderByOptions;
        Top = topOption;
        Skip = skipOption;
        Count = countQueryOption;
        Search = searchOption;
        Select = selectOption;
        Compute = computeOption;
    }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public virtual QueryTokenKind Kind { get; }

    /// <summary>
    /// Gets the property for this select or expand item.
    /// </summary>
    public SegmentToken PathToProperty { get; set; }

    /// <summary>
    /// Gets/sets the inner filter option for this select or expand item.
    /// </summary>
    public IQueryToken Filter { get; set; }

    /// <summary>
    /// Gets/sets the inner orderby options for this select or expand item.
    /// </summary>
    public IEnumerable<OrderByToken> OrderBy { get; set; }

    /// <summary>
    /// Gets/sets the inner search option for this select or expand item.
    /// </summary>
    public IQueryToken Search { get; set; }

    /// <summary>
    /// Gets/sets the inner top option for this select or expand item.
    /// </summary>
    public long? Top { get; set; }

    /// <summary>
    /// Gets/sets the inner skip option for this select or expand item.
    /// </summary>
    public long? Skip { get; set; }

    /// <summary>
    /// Gets/sets the inner query count option for this select or expand item.
    /// </summary>
    public bool? Count { get; set; }

    /// <summary>
    /// Gets/sets the inner select option for this select or expand item.
    /// </summary>
    public SelectToken Select { get; set; }

    /// <summary>
    /// Gets/sets the inner compute option for this select or expand item.
    /// </summary>
    public ComputeToken Compute { get; set; }
}
