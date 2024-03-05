//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

public class StringLiteralToken : QueryToken
{
    /// <summary>
    /// Constructor for the StringLiteralToken
    /// </summary>
    /// <param name="text">The text value for this token</param>
    internal StringLiteralToken(string text)
    {
        Text = text;
    }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.StringLiteral;

    /// <summary>
    /// The original text value of the literal.
    /// </summary>
    /// <remarks>This is used only internally to simulate correct compat behavior with WCF DS.
    /// We should only use this during type promotion when applying metadata.</remarks>
    public string Text { get; }
}