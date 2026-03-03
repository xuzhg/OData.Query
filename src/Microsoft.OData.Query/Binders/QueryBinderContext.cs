//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Linq.Expressions;

namespace Microsoft.OData.Query.Binders;

/// <summary>
/// Encapsulates all binder information about an individual query option binding.
/// </summary>
public class QueryBinderContext
{
    /// <summary>
    /// The parameter name for root type.(it could be renamed as $root).
    /// </summary>
    private const string DollarIt = "$it";

    /// <summary>
    /// The parameter name for current type.
    /// </summary>
    private const string DollarThis = "$this";

    /// <summary>
    /// All parameters present in current context.
    /// </summary>
    private IDictionary<string, ParameterExpression> _lambdaParameters;

    public QueryBinderContext(Type elementType)
    {
        ElementClrType = elementType;

        ParameterExpression thisParameters = Expression.Parameter(elementType, "$it");
        _lambdaParameters = new Dictionary<string, ParameterExpression>();
        _lambdaParameters["$it"] = thisParameters;
    }

    /// <summary>
    /// Gets the Element Clr type.
    /// </summary>
    public Type ElementClrType { get; }

    /// <summary>
    /// Gets the current parameter. Current parameter is the parameter at root of this context.
    /// </summary>
    public ParameterExpression CurrentParameter => _lambdaParameters["$it"];
}
