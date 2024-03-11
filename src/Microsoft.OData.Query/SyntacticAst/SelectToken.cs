//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing a select query option.
/// </summary>
public sealed class SelectToken : List<SelectItemToken>, IQueryToken
{
    /// <summary>
    /// Gets the kind of the query token.
    /// </summary>
    public QueryTokenKind Kind => QueryTokenKind.Select;
}