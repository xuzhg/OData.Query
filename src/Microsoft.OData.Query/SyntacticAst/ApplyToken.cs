//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing the $apply Query
/// </summary>
public sealed class ApplyToken : IQueryToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnyToken" /> class.
    /// </summary>
    /// <param name="expression">The associated expression.</param>
    /// <param name="parameter">The parameter denoting source type.</param>
    /// <param name="parent">The parent token.  Pass null if this property has no parent.</param>
    public ApplyToken(IList<IQueryToken> transformations)
    {
        Transformations = transformations;
    }

    public IEnumerable<IQueryToken> Transformations { get; }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public QueryTokenKind Kind => QueryTokenKind.Apply;
}