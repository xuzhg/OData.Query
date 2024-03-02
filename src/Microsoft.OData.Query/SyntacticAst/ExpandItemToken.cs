//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

public abstract class SelectExpandItemToken : QueryToken
{
    /// <summary>
    /// Initializes a new instance of  <see cref="SelectExpandTermToken"/> class.
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
        QueryToken filterOption,
        IEnumerable<OrderByToken> orderByOptions,
        long? topOption,
        long? skipOption,
        bool? countQueryOption,
        QueryToken searchOption,
        SelectToken selectOption,
        ComputeToken computeOption)
    {
       // ExceptionUtils.CheckArgumentNotNull(pathToProperty, "property");

        PathToProperty = pathToProperty;
        FilterOption = filterOption;
        OrderByOptions = orderByOptions;
        TopOption = topOption;
        SkipOption = skipOption;
        CountQueryOption = countQueryOption;
        SearchOption = searchOption;
        SelectOption = selectOption;
        ComputeOption = computeOption;
    }

    /// <summary>
    /// Gets the property for this select or expand term.
    /// </summary>
    public SegmentToken PathToProperty { get; internal set; }

    /// <summary>
    /// Gets the filter option for this select or expand term.
    /// </summary>
    public QueryToken FilterOption { get; private set; }

    /// <summary>
    /// Gets the orderby options for this select or expand term.
    /// </summary>
    public IEnumerable<OrderByToken> OrderByOptions { get; private set; }

    /// <summary>
    /// Gets the search option for this select or expand term.
    /// </summary>
    public QueryToken SearchOption { get; private set; }

    /// <summary>
    /// Gets the top option for this select or expand term.
    /// </summary>
    public long? TopOption { get; private set; }

    /// <summary>
    /// Gets the skip option for this select or expand term.
    /// </summary>
    public long? SkipOption { get; private set; }

    /// <summary>
    /// Gets the query count option for this select or expand term.
    /// </summary>
    public bool? CountQueryOption { get; private set; }

    /// <summary>
    /// Gets the select option for this select or expand term.
    /// </summary>
    public SelectToken SelectOption { get; internal set; }

    /// <summary>
    /// Gets the compute option for this select or expand term.
    /// </summary>
    public ComputeToken ComputeOption { get; private set; }
}

/// <summary>
/// Lexical token representing an expand operation.
/// </summary>
public sealed class ExpandItemToken : SelectExpandItemToken
{
    /// <summary>
    /// Create an expand term token using only a property
    /// </summary>
    /// <param name="pathToNavigationProp">the path to the navigation property</param>
    public ExpandItemToken(SegmentToken pathToNavigationProp)
        : this(pathToNavigationProp, null, null)
    {
    }

    /// <summary>
    /// Create an expand term using only the property and its subexpand/select
    /// </summary>
    /// <param name="pathToNavigationProp">the path to the navigation property for this expand term</param>
    /// <param name="selectOption">the sub select for this token</param>
    /// <param name="expandOption">the sub expand for this token</param>
    public ExpandItemToken(SegmentToken pathToNavigationProp, SelectToken selectOption, ExpandToken expandOption)
        : this(pathToNavigationProp, null, null, null, null, null, null, null, selectOption, expandOption)
    {
    }

    /// <summary>
    /// Create an expand term token
    /// </summary>
    /// <param name="pathToNavigationProp">the nav prop for this expand term</param>
    /// <param name="filterOption">the filter option for this expand term</param>
    /// <param name="orderByOptions">the orderby options for this expand term</param>
    /// <param name="topOption">the top option for this expand term</param>
    /// <param name="skipOption">the skip option for this expand term</param>
    /// <param name="countQueryOption">the query count option for this expand term</param>
    /// <param name="levelsOption">the levels option for this expand term</param>
    /// <param name="searchOption">the search option for this expand term</param>
    /// <param name="selectOption">the select option for this expand term</param>
    /// <param name="expandOption">the expand option for this expand term</param>
    public ExpandItemToken(SegmentToken pathToNavigationProp, QueryToken filterOption, IEnumerable<OrderByToken> orderByOptions, long? topOption, long? skipOption, bool? countQueryOption, long? levelsOption, QueryToken searchOption, SelectToken selectOption, ExpandToken expandOption)
        : this(pathToNavigationProp, filterOption, orderByOptions, topOption, skipOption, countQueryOption, levelsOption, searchOption, selectOption, expandOption, null)
    {
    }

    /// <summary>
    /// Create an expand term token
    /// </summary>
    /// <param name="pathToNavigationProp">the nav prop for this expand term</param>
    /// <param name="filterOption">the filter option for this expand term</param>
    /// <param name="orderByOptions">the orderby options for this expand term</param>
    /// <param name="topOption">the top option for this expand term</param>
    /// <param name="skipOption">the skip option for this expand term</param>
    /// <param name="countQueryOption">the query count option for this expand term</param>
    /// <param name="levelsOption">the levels option for this expand term</param>
    /// <param name="searchOption">the search option for this expand term</param>
    /// <param name="selectOption">the select option for this expand term</param>
    /// <param name="expandOption">the expand option for this expand term</param>
    /// <param name="computeOption">the compute option for this expand term.</param>
    public ExpandItemToken(
        SegmentToken pathToNavigationProp,
        QueryToken filterOption,
        IEnumerable<OrderByToken> orderByOptions,
        long? topOption,
        long? skipOption,
        bool? countQueryOption,
        long? levelsOption,
        QueryToken searchOption,
        SelectToken selectOption,
        ExpandToken expandOption,
        ComputeToken computeOption)
        : this(pathToNavigationProp, filterOption, orderByOptions, topOption, skipOption, countQueryOption, levelsOption, searchOption, selectOption, expandOption, computeOption, null)
    {
    }

    /// <summary>
    /// Create an expand term token
    /// </summary>
    /// <param name="pathToNavigationProp">the nav prop for this expand term</param>
    /// <param name="filterOption">the filter option for this expand term</param>
    /// <param name="orderByOptions">the orderby options for this expand term</param>
    /// <param name="topOption">the top option for this expand term</param>
    /// <param name="skipOption">the skip option for this expand term</param>
    /// <param name="countQueryOption">the query count option for this expand term</param>
    /// <param name="levelsOption">the levels option for this expand term</param>
    /// <param name="searchOption">the search option for this expand term</param>
    /// <param name="selectOption">the select option for this expand term</param>
    /// <param name="expandOption">the expand option for this expand term</param>
    /// <param name="computeOption">the compute option for this expand term.</param>
    /// <param name="applyOptions">the apply options for this expand term.</param>
    public ExpandItemToken(
        SegmentToken pathToNavigationProp,
        QueryToken filterOption,
        IEnumerable<OrderByToken> orderByOptions,
        long? topOption,
        long? skipOption,
        bool? countQueryOption,
        long? levelsOption,
        QueryToken searchOption,
        SelectToken selectOption,
        ExpandToken expandOption,
        ComputeToken computeOption,
        IEnumerable<QueryToken> applyOptions)
        : base(pathToNavigationProp, filterOption, orderByOptions, topOption, skipOption, countQueryOption, searchOption, selectOption, computeOption)
    {
        ExpandOption = expandOption;
        LevelsOption = levelsOption;
        ApplyOptions = applyOptions;
    }

    /// <summary>
    /// Gets the navigation property for this expand term.
    /// </summary>
    public SegmentToken PathToNavigationProp
    {
        get
        {
            return PathToProperty;
        }
    }

    /// <summary>
    /// Gets the expand option for this select or expand term.
    /// </summary>
    public ExpandToken ExpandOption { get; internal set; }

    /// <summary>
    /// Gets the levels option for this expand term.
    /// </summary>
    public long? LevelsOption { get; private set; }

    /// <summary>
    /// Gets the apply options for this expand term.
    /// </summary>
    public IEnumerable<QueryToken> ApplyOptions { get; private set; }

    /// <summary>
    /// Gets the kind of this expand term.
    /// </summary>
    public override QueryTokenKind Kind
    {
        get { return QueryTokenKind.ExpandTerm; }
    }
}
