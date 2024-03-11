//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing the All Query
/// </summary>
public sealed class AllToken : LambdaToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AllToken" /> class.
    /// </summary>
    /// <param name="expression">The associated expression.</param>
    /// <param name="parameter">The parameter denoting source type.</param>
    /// <param name="parent">The parent token.  Pass null if this property has no parent.</param>
    public AllToken(IQueryToken expression, string parameter, IQueryToken parent)
        : base(expression, parameter, parent)
    {
    }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.All;
}