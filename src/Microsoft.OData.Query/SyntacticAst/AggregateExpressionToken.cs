//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.SyntacticAst;

/// <summary>
/// Query token representing an Aggregate expression.
/// </summary>
public sealed class AggregateExpressionToken : AggregateTokenBase
{
    /// <summary>
    /// Create an AggregateExpressionToken.
    /// </summary>
    /// <param name="expression">The aggregate expression.</param>
    /// <param name="method">The aggregation method.</param>
    /// <param name="alias">The alias for this query token.</param>
    public AggregateExpressionToken(IQueryToken expression, AggregationMethod method, string alias)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        Method = method;
        Alias = alias ?? throw new ArgumentNullException(nameof(alias));
    }

    ///// <summary>
    ///// Create an AggregateExpressionToken.
    ///// </summary>
    ///// <param name="expression">The aggregate expression.</param>
    ///// <param name="methodDefinition">The aggregate method definition.</param>
    ///// <param name="alias">The alias for this query token.</param>
    //public AggregateExpressionToken(QueryToken expression, AggregationMethodDefinition methodDefinition, string alias)
    //    : this(expression, methodDefinition.MethodKind, alias)
    //{
    //    this.methodDefinition = methodDefinition;
    //}

    /// <summary>
    /// Gets the kind of this token.
    /// </summary>
    public override QueryTokenKind Kind => QueryTokenKind.AggregateExpression;

    /// <summary>
    /// Gets the AggregationMethod of this token.
    /// </summary>
    public AggregationMethod Method { get; }

    /// <summary>
    /// Gets the expression.
    /// </summary>
    public IQueryToken Expression { get; }

    ///// <summary>
    ///// Gets the aggregate method definition.
    ///// </summary>
    //public AggregationMethodDefinition MethodDefinition
    //{
    //    get
    //    {
    //        return methodDefinition;
    //    }
    //}

    /// <summary>
    /// Gets the alias.
    /// </summary>
    public string Alias { get; }
}
