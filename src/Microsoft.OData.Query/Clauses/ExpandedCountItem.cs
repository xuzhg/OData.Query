//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Paths;

namespace Microsoft.OData.Query.Clauses;

/// <summary>
/// Represents one level of expansion for a particular expansion tree with $count operation.
/// $expand=Nav/$count
/// </summary>
public sealed class ExpandedCountItem : ExpandedItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpandedCountItem" /> class.
    /// </summary>
    /// <param name="expandPath">The path to the navigation property for this expand item, including any type segments</param>
    public ExpandedCountItem(ExpandPathItem expandPath)
        : this (expandPath, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpandedCountItem" /> class.
    /// </summary>
    /// <param name="expandPath"></param>
    /// <param name="filter"></param>
    /// <param name="search"></param>
    public ExpandedCountItem(ExpandPathItem expandPath, FilterClause filter, SearchClause search)
        : base (expandPath, null, null, filter, null, null, null, null, search)
    {
    }
}
