//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

public sealed class StringLiteralToken : IQueryToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringLiteralToken" /> class.
    /// </summary>
    /// <param name="text">The text value for this token</param>
    internal StringLiteralToken(string text)
    {
        Text = text;
    }

    /// <summary>
    /// Gets the kind of the query token.
    /// </summary>
    public QueryTokenKind Kind => QueryTokenKind.StringLiteral;

    /// <summary>
    /// The original text value of the literal.
    /// </summary>
    /// <remarks>This is used only internally to simulate correct compat behavior with WCF DS.
    /// We should only use this during type promotion when applying metadata.</remarks>
    public string Text { get; }
}