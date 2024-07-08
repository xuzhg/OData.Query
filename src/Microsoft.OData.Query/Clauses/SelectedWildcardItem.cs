//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Clauses;

/// <summary>
/// Represents a '*' selection item, indicating that all structural properties should be selected.
/// </summary>
public sealed class SelectedWildcardItem : SelectedItem
{
    // $select=*
}
