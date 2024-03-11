//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing a select operation.
/// </summary>
public sealed class SelectItemToken : SelectExpandItemToken
{
    /// <summary>
    /// Initializes a new instance of  <see cref="SelectTermToken"/> class.
    /// </summary>
    /// <param name="pathToProperty">the path to the property for this select term</param>
    public SelectItemToken(SegmentToken pathToProperty)
        : this(pathToProperty, null)
    {
    }

    /// <summary>
    /// Create an select term using only the property and its sub-expand/select
    /// </summary>
    /// <param name="pathToProperty">the path to the property for this select term</param>
    /// <param name="selectOption">the sub select for this token</param>
    public SelectItemToken(SegmentToken pathToProperty, SelectToken selectOption)
        : this(pathToProperty, null, null, null, null, null, null, selectOption, null)
    {
    }

    /// <summary>
    /// Create a select term using only the property and its supporting query options.
    /// </summary>
    /// <param name="pathToProperty">the path to the property for this select term</param>
    /// <param name="filterOption">the filter option for this select term</param>
    /// <param name="orderByOptions">the orderby options for this select term</param>
    /// <param name="topOption">the top option for this select term</param>
    /// <param name="skipOption">the skip option for this select term</param>
    /// <param name="countQueryOption">the query count option for this select term</param>
    /// <param name="searchOption">the search option for this select term</param>
    /// <param name="selectOption">the select option for this select term</param>
    /// <param name="computeOption">the compute option for this select term</param>
    public SelectItemToken(SegmentToken pathToProperty,
        IQueryToken filterOption, IEnumerable<OrderByToken> orderByOptions, long? topOption, long? skipOption, bool? countQueryOption, IQueryToken searchOption, SelectToken selectOption, ComputeToken computeOption)
        : base(pathToProperty, filterOption, orderByOptions, topOption, skipOption, countQueryOption, searchOption, selectOption, computeOption)
    {
    }

    /// <summary>
    /// Gets the kind of this expand term.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.SelectItem;
}
