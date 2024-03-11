//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

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
    /// Create an expand term using only the property and its sub-expand/select
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
    public ExpandItemToken(SegmentToken pathToNavigationProp, IQueryToken filterOption, IEnumerable<OrderByToken> orderByOptions, long? topOption, long? skipOption, bool? countQueryOption, long? levelsOption, IQueryToken searchOption, SelectToken selectOption, ExpandToken expandOption)
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
        IQueryToken filterOption,
        IEnumerable<OrderByToken> orderByOptions,
        long? topOption,
        long? skipOption,
        bool? countQueryOption,
        long? levelsOption,
        IQueryToken searchOption,
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
        IQueryToken filterOption,
        IEnumerable<OrderByToken> orderByOptions,
        long? topOption,
        long? skipOption,
        bool? countQueryOption,
        long? levelsOption,
        IQueryToken searchOption,
        SelectToken selectOption,
        ExpandToken expandOption,
        ComputeToken computeOption,
        ApplyToken applyOptions)
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
    /// Gets the expand option for expand item token.
    /// </summary>
    public ExpandToken ExpandOption { get; internal set; }

    /// <summary>
    /// Gets the levels option for this expand term.
    /// </summary>
    public long? LevelsOption { get; private set; }

    /// <summary>
    /// Gets the apply options for this expand term.
    /// </summary>
    public ApplyToken ApplyOptions { get; private set; }

    /// <summary>
    /// Gets the kind of this expand term.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.ExpandItem;
}
