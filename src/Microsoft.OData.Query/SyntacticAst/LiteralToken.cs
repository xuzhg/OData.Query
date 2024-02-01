//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing a literal value.
/// </summary>
public class LiteralToken : QueryToken
{
    /// <summary>
    /// Create a new LiteralToken given value and originalText
    /// </summary>
    /// <param name="value">The value of the literal. This is a parsed primitive value.</param>
    public LiteralToken(object value)
    {
        Value = value;
    }

    /// <summary>
    /// Create a new LiteralToken given value and originalText
    /// </summary>
    /// <param name="value">The value of the literal. This is a parsed primitive value.</param>
    /// <param name="originalText">The original text value of the literal.</param>
    /// <remarks>This is used internally to simulate correct compat behavior with WCF DS, and parameter alias.</remarks>
    internal LiteralToken(object value, string originalText)
        : this(value)
    {
        OriginalText = originalText;
    }

    /// <summary>
    /// Create a new LiteralToken given value and originalText
    /// </summary>
    /// <param name="value">The value of the literal. This is a parsed primitive value.</param>
    /// <param name="originalText">The original text value of the literal.</param>
    /// <param name="expectedEdmTypeReference">The expected EDM type of literal..</param>
    /// <remarks>This is used internally to simulate correct compat behavior with WCF DS, and parameter alias.</remarks>
    internal LiteralToken(object value, string originalText, Type expectedEdmTypeReference)
        : this(value, originalText)
    {
        ExpectedEdmTypeReference = expectedEdmTypeReference;
    }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.Literal;

    /// <summary>
    /// The value of the literal. This is a parsed primitive value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// The original text value of the literal.
    /// </summary>
    /// <remarks>This is used internally to simulate correct compat behavior with WCF DS, and parameter alias.
    /// We should use this during type promotion when applying metadata.</remarks>
    internal string OriginalText { get; }

    /// <summary>
    /// The expected EDM type of literal.
    /// </summary>
    internal Type ExpectedEdmTypeReference { get; }
}