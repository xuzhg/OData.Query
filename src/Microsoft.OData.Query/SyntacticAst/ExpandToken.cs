//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing an expand query option.
/// </summary>
public sealed class ExpandToken : List<ExpandItemToken>, IQueryToken
{
    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public QueryTokenKind Kind => QueryTokenKind.Expand;
}