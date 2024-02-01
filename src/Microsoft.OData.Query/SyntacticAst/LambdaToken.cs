//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Lexical token representing the Any/All Query
/// </summary>
public abstract class LambdaToken : QueryToken
{
    /// <summary>
    /// Create a AnyAllQueryToken given the expression, parameter, and parent
    /// </summary>
    /// <param name="expression">The associated expression.</param>
    /// <param name="parameter">The parameter denoting source type.</param>
    /// <param name="parent">The parent token.  Pass null if this property has no parent.</param>
    protected LambdaToken(QueryToken expression, string parameter, QueryToken parent)
    {
        Expression = expression;
        Parameter = parameter;
        Parent = parent;
    }

    /// <summary>
    /// The parent token.
    /// </summary>
    public QueryToken Parent { get; }

    /// <summary>
    /// The expression.
    /// </summary>
    public QueryToken Expression { get; }

    /// <summary>
    /// The parameter.
    /// </summary>
    public string Parameter { get; }
}
