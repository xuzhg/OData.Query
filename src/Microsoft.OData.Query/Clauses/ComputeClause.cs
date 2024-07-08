//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Clauses;

/// <summary>
/// The result of parsing a $compute query option.
/// </summary>
public class ComputeClause : List<ComputedItem>
{ }