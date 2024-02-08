//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Nodes;

/// <summary>
/// An item that has been computed by the query at the current level of the tree.
/// </summary>
public sealed class ComputeExpression
{
    private readonly SingleValueNode expression;

    private readonly string alias;

    private readonly Type typeReference;

    /// <summary>
    /// Create a ComputeExpression.
    /// </summary>
    /// <param name="expression">The compute expression.</param>
    /// <param name="alias">The compute alias.</param>
    /// <param name="typeReference">The <see cref="IEdmTypeReference"/> of this aggregate expression.</param>
    public ComputeExpression(SingleValueNode expression, string alias, Type typeReference)
    {
      //  ExceptionUtils.CheckArgumentNotNull(expression, "expression");
       // ExceptionUtils.CheckArgumentNotNull(alias, "alias");

        this.expression = expression;
        this.alias = alias;
        //// TypeReference is null for dynamic properties
        this.typeReference = typeReference;
    }

    /// <summary>
    /// Gets the aggregation expression.
    /// </summary>
    public SingleValueNode Expression
    {
        get
        {
            return this.expression;
        }
    }

    /// <summary>
    /// Gets the aggregation alias.
    /// </summary>
    public string Alias
    {
        get
        {
            return this.alias;
        }
    }

    /// <summary>
    /// Gets the <see cref="IEdmTypeReference"/> of this aggregate expression.
    /// </summary>
    public Type TypeReference
    {
        get
        {
            return this.typeReference;
        }
    }
}
