//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.Paths;

namespace Microsoft.OData.Query.Clauses;

/// <summary>
/// Represents one level of expansion for a particular expansion tree with $ref operation.
/// $expand=Nav/$ref
/// </summary>
public sealed class ExpandedReferenceItem : ExpandedItem
{
    /// <summary>
    /// Initializes a new instance of <see cref="ExpandedReferenceItem"/> class.
    /// </summary>
    /// <param name="pathToNavigationProperty">the path to the navigation property for this expand item, including any type segments</param>
    /// <param name="navigationSource">the navigation source for this ExpandItem</param>
    public ExpandedReferenceItem(ExpandPathItem expandPath)
        : this(expandPath, null, null, null, null, null, null, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ExpandedReferenceItem"/> class.
    /// </summary>
    /// <param name="pathToNavigationProperty">the path to the navigation property for this expand item, including any type segments</param>
    /// <param name="navigationSource">the navigation source for this expand level.</param>
    /// <param name="filterOption">A filter clause for this expand (can be null)</param>
    /// <param name="orderByOption">An Orderby clause for this expand (can be null)</param>
    /// <param name="topOption">A top clause for this expand (can be null)</param>
    /// <param name="skipOption">A skip clause for this expand (can be null)</param>
    /// <param name="countOption">An query count clause for this expand (can be null)</param>
    /// <param name="searchOption">A search clause for this expand (can be null)</param>
    /// <param name="computeOption">A compute clause for this expand (can be null)</param>
    /// <param name="applyOption">A apply clause for this expand (can be null)</param>
    /// <exception cref="System.ArgumentNullException">Throws if input pathToNavigationProperty is null.</exception>
    public ExpandedReferenceItem(
         ExpandPathItem expandPath,
         ApplyClause apply,
         ComputeClause compute,
         FilterClause filter,
         OrderByClause orderBy,
         ConstantLongNode top,
         ConstantLongNode skip,
         ConstantBoolNode count,
         SearchClause search)
        : base(expandPath, apply, compute, filter, orderBy, top, skip, count, search)
    { }
}
