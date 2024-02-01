//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing the last segment in a path.
/// </summary>
public sealed class EndPathToken : PathToken
{
    /// <summary>
    /// Create a EndPathToken given the Identifier and the NextToken (if any)
    /// </summary>
    /// <param name="identifier">The Identifier of the property to access.</param>
    /// <param name="nextToken">The NextToken token to access the property on. </param>
    public EndPathToken(string identifier, QueryToken nextToken)
    {
        if (string.IsNullOrEmpty(identifier))
        {
            throw new ArgumentNullException(nameof(identifier));
        }

        Identifier = identifier;
        NextToken = nextToken;
    }

    /// <summary>
    /// The kind of the query token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.EndPath;

    /// <summary>
    /// The NextToken token to access the property on.
    /// If this is null, then the property access has no NextToken. That usually means to access the property
    /// on the implicit parameter for the expression, the result on which the expression is being applied.
    /// </summary>
    public override QueryToken NextToken { get; set; }

    /// <summary>
    /// The Identifier of the property to access.
    /// </summary>
    public override string Identifier { get; }
}