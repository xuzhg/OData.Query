//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Parser;

public abstract class QueryOptionParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryOptionParser" /> class.
    /// </summary>
    protected QueryOptionParser()
    {
    }

    /// <summary>
    /// Visits a <see cref="IQueryToken"/> in the lexical tree and binds it to metadata producing a semantic <see cref="QueryNode"/>.
    /// </summary>
    /// <param name="token">The query token on the input.</param>
    /// <returns>The bound query node output.</returns>
    protected virtual QueryNode Bind(IQueryToken token, QueryParserContext context)
    {
        //ExceptionUtils.CheckArgumentNotNull(token, "token");
        context.EnterRecurse();
        QueryNode result;
        switch (token.Kind)
        {
            case QueryTokenKind.Any:
                result = BindAnyAll((AnyToken)token, context);
                break;
            case QueryTokenKind.All:
                result = BindAnyAll((AllToken)token, context);
                break;
            //case QueryTokenKind.InnerPath:
            //    result = this.BindInnerPathSegment((InnerPathToken)token, context);
            //    break;
            case QueryTokenKind.Literal:
                result = BindLiteral((LiteralToken)token, context);
                break;
            //case QueryTokenKind.StringLiteral:
            //    result = this.BindStringLiteral((StringLiteralToken)token, context);
            //    break;
            case QueryTokenKind.BinaryOperator:
                result = BindBinaryOperator((BinaryOperatorToken)token, context);
                break;
            case QueryTokenKind.UnaryOperator:
                result = BindUnaryOperator((UnaryOperatorToken)token, context);
                break;
            case QueryTokenKind.EndPath:
                result = BindEndPath((EndPathToken)token, context);
                break;
            //case QueryTokenKind.FunctionCall:
            //    result = this.BindFunctionCall((FunctionCallToken)token, context);
            //    break;
            //case QueryTokenKind.DottedIdentifier:
            //    result = this.BindCast((DottedIdentifierToken)token, context);
            //    break;
            case QueryTokenKind.RangeVariable:
                result = BindRangeVariable((RangeVariableToken)token, context);
                break;
            //case QueryTokenKind.FunctionParameterAlias:
            //    result = this.BindParameterAlias((FunctionParameterAliasToken)token, context);
            //    break;
            //case QueryTokenKind.FunctionParameter:
            //    result = this.BindFunctionParameter((FunctionParameterToken)token, context);
            //    break;
            //case QueryTokenKind.In:
            //    result = this.BindIn((InToken)token, context);
            //    break;
            //case QueryTokenKind.CountSegment:
            //    result = this.BindCountSegment((CountSegmentToken)token, context);
            //    break;
            default:
                throw new Exception("ODataErrorStrings.MetadataBinder_UnsupportedQueryTokenKind(token.Kind)");
        }

        if (result == null)
        {
            throw new Exception("ODataErrorStrings.MetadataBinder_BoundNodeCannotBeNull(token.Kind)");
        }

        context.LeaveRecurse();
        return result;
    }

    ///// <summary>
    ///// Bind parameter alias (figuring out its type by first parsing and binding its value expression).
    ///// </summary>
    ///// <param name="functionParameterAliasToken">The alias syntactics token.</param>
    ///// <returns>The semantics node for parameter alias.</returns>
    //protected virtual SingleValueNode BindParameterAlias(FunctionParameterAliasToken functionParameterAliasToken)
    //{
    //    ParameterAliasBinder binder = new ParameterAliasBinder(this.Bind);
    //    return binder.BindParameterAlias(this.BindingState, functionParameterAliasToken);
    //}

    /// <summary>
    /// Bind a function parameter token
    /// </summary>
    /// <param name="token">The token to bind.</param>
    /// <returns>A semantically bound FunctionCallNode</returns>
    //protected virtual QueryNode BindFunctionParameter(FunctionParameterToken token)
    //{
    //    // TODO: extract this into its own binder class.
    //    if (token.ParameterName != null)
    //    {
    //        return new NamedFunctionParameterNode(token.ParameterName, this.Bind(token.ValueToken));
    //    }

    //    return this.Bind(token.ValueToken);
    //}

    /// <summary>
    /// Binds an InnerPathToken.
    /// </summary>
    /// <param name="token">Token to bind.</param>
    /// <returns>Either a SingleNavigationNode, CollectionNavigationNode, SinglePropertyAccessNode (complex),
    /// or CollectionPropertyAccessNode (primitive or complex) that is the metadata-bound version of the given token.</returns>
    //protected virtual QueryNode BindInnerPathSegment(InnerPathToken token)
    //{
    //    InnerPathTokenBinder innerPathTokenBinder = new InnerPathTokenBinder(this.Bind, this.BindingState);
    //    return innerPathTokenBinder.BindInnerPathSegment(token);
    //}

    /// <summary>
    /// Binds a parameter token.
    /// </summary>
    /// <param name="rangeVariableToken">The parameter token to bind.</param>
    /// <returns>The bound query node.</returns>
    protected virtual SingleValueNode BindRangeVariable(RangeVariableToken rangeVariableToken, QueryParserContext context)
    {
        RangeVariable variable = context.GetRangeVariable(rangeVariableToken.Name);
        if (variable == null)
        {
            throw new Exception("ODataErrorStrings.MetadataBinder_ParameterNotInScope(rangeVariableToken.Name)");
        }

        return new RangeVariableReferenceNode(variable);
    }

    /// <summary>
    /// Binds a literal token.
    /// </summary>
    /// <param name="literalToken">The literal token to bind.</param>
    /// <returns>The bound literal token.</returns>
    protected virtual QueryNode BindLiteral(LiteralToken literalToken, QueryParserContext context)
    {
        if (!string.IsNullOrEmpty(literalToken.OriginalText))
        {
            if (literalToken.ExpectedType != null)
            {
                return new ConstantNode(literalToken.Value, literalToken.OriginalText, literalToken.ExpectedType);
            }

            return new ConstantNode(literalToken.Value, literalToken.OriginalText);
        }

        return new ConstantNode(literalToken.Value);
    }

    /// <summary>
    /// Binds a binary operator token.
    /// </summary>
    /// <param name="binaryOperatorToken">The binary operator token to bind.</param>
    /// <returns>The bound binary operator token.</returns>
    protected virtual QueryNode BindBinaryOperator(BinaryOperatorToken binaryOperatorToken, QueryParserContext context)
    {
        SingleValueNode left = Bind(binaryOperatorToken.Left, context) as SingleValueNode;
        if (left == null)
        {
            throw new Exception("ODataErrorStrings.MetadataBinder_BinaryOperatorOperandNotSingleValue(operatorKind.ToString())");
        }

        SingleValueNode right = Bind(binaryOperatorToken.Right, context) as SingleValueNode;
        if (left == null)
        {
            throw new Exception("ODataErrorStrings.MetadataBinder_BinaryOperatorOperandNotSingleValue(operatorKind.ToString())");
        }

        // Maybe need covert implicitly
        // left = MetadataBindingUtils.ConvertToTypeIfNeeded(left, leftType);
        // right = MetadataBindingUtils.ConvertToTypeIfNeeded(right, rightType);

        return new BinaryOperatorNode(binaryOperatorToken.OperatorKind, left, right, left.NodeType);
    }

    /// <summary>
    /// Binds a unary operator token.
    /// </summary>
    /// <param name="unaryOperatorToken">The unary operator token to bind.</param>
    /// <returns>The bound unary operator token.</returns>
    protected virtual QueryNode BindUnaryOperator(UnaryOperatorToken unaryOperatorToken, QueryParserContext context)
    {
        SingleValueNode operand = Bind(unaryOperatorToken.Operand, context) as SingleValueNode;
        if (operand == null)
        {
            throw new Exception("ODataErrorStrings.MetadataBinder_UnaryOperatorOperandNotSingleValue(unaryOperatorToken.OperatorKind.ToString())");
        }

        //IEdmTypeReference typeReference = UnaryOperatorBinder.PromoteOperandType(operand, unaryOperatorToken.OperatorKind);
        //Debug.Assert(typeReference == null || typeReference.IsODataPrimitiveTypeKind(), "Only primitive types should be able to get here.");
        //operand = MetadataBindingUtils.ConvertToTypeIfNeeded(operand, typeReference);

        return new UnaryOperatorNode(unaryOperatorToken.OperatorKind, operand);
    }

    /// <summary>
    /// Binds a type startPath token.
    /// </summary>
    /// <param name="dottedIdentifierToken">The type startPath token to bind.</param>
    /// <returns>The bound type startPath token.</returns>
    //protected virtual QueryNode BindCast(DottedIdentifierToken dottedIdentifierToken)
    //{
    //    DottedIdentifierBinder dottedIdentifierBinder = new DottedIdentifierBinder(this.Bind, this.BindingState);
    //    return dottedIdentifierBinder.BindDottedIdentifier(dottedIdentifierToken);
    //}

    /// <summary>
    /// Binds a LambdaToken.
    /// </summary>
    /// <param name="lambdaToken">The LambdaToken to bind.</param>
    /// <returns>A bound Any or All node.</returns>
    protected virtual QueryNode BindAnyAll(LambdaToken lambdaToken, QueryParserContext context)
    {
        //ExceptionUtils.CheckArgumentNotNull(lambdaToken, "LambdaToken");

        // Start by binding the parent token
        CollectionValueNode parent = Bind(lambdaToken.Parent, context) as CollectionValueNode;
        if (parent == null)
        {
            throw new Exception("ODataErrorStrings.MetadataBinder_LambdaParentMustBeCollection");
        }

        // Add the lambda variable to the stack
        RangeVariable rangeVariable = null;
        if (lambdaToken.Parameter != null)
        {
            rangeVariable = new RangeVariable(lambdaToken.Parameter, parent.ElementType);
            context.RangeVariables.Push(rangeVariable);
        }

        // Bind the expression
        SingleValueNode expression = Bind(lambdaToken.Expression, context) as SingleValueNode;
        if (expression == null)
        {
            throw new Exception("MetadataBinder_AnyAllExpressionNotSingleValue");
        }

        // type reference is allowed to be null for open properties.
        //IEdmTypeReference expressionTypeReference = expression.GetEdmTypeReference();
        //if (expressionTypeReference != null && !expressionTypeReference.AsPrimitive().IsBoolean())
        //{
        //    throw new ODataException(ODataErrorStrings.MetadataBinder_AnyAllExpressionNotSingleValue);
        //}

        LambdaNode lambdaNode;
        if (lambdaToken.Kind == QueryTokenKind.Any)
        {
            lambdaNode = new AnyNode(parent, new Collection<RangeVariable>(context.RangeVariables.ToList()), rangeVariable, expression);
        }
        else
        {
            lambdaNode = new AllNode(parent, new Collection<RangeVariable>(context.RangeVariables.ToList()), rangeVariable, expression);
        }

        // Remove the lambda variable as it is now out of scope
        if (rangeVariable != null)
        {
            context.RangeVariables.Pop();
        }

        return lambdaNode;
    }

    /// <summary>
    /// Binds a property access token.
    /// </summary>
    /// <param name="endPathToken">The property access token to bind.</param>
    /// <returns>The bound property access token.</returns>
    protected virtual QueryNode BindEndPath(EndPathToken endPathToken, QueryParserContext context)
    {
        QueryNode parent = DetermineParentNode(endPathToken, context);

        if (parent is SingleValueNode singleValueParent)
        {
            // TODO: check computed? check aggregated?

            Type type = singleValueParent.NodeType;

            PropertyInfo propertyInfo = context.Resolver.ResolveProperty(type, endPathToken.Identifier);
            if (propertyInfo != null)
            {
                if (propertyInfo.PropertyType.IsCollection())
                {
                    return new CollectionValuePropertyAccessNode(singleValueParent, propertyInfo);
                }
                else
                {
                    return new SingleValuePropertyAccessNode(singleValueParent, propertyInfo);
                }
            }

            // if not declared property
            //if (functionCallBinder.TryBindEndPathAsFunctionCall(endPathToken, singleValueParent, state, out boundFunction))
            //{
            //    return boundFunction;
            //}

            // or return as dynamic property?
            throw new NotImplementedException();
        }

        // Collection with any or all expression is already supported and handled separately.
        // Add support of collection with $count segment.
        if (parent is CollectionValueNode colNode
            && endPathToken.Identifier.Equals("$count", StringComparison.Ordinal))
        {
            // create a collection count node for collection node property.
            return new DollarCountNode(colNode);
        }

        //if (functionCallBinder.TryBindEndPathAsFunctionCall(endPathToken, parent, state, out boundFunction))
        //{
        //    return boundFunction;
        //}

        throw new Exception("ODataErrorStrings.MetadataBinder_PropertyAccessSourceNotSingleValue(endPathToken.Identifier)");
    }

    private QueryNode DetermineParentNode(EndPathToken segmentToken, QueryParserContext context)
    {
        //ExceptionUtils.CheckArgumentNotNull(segmentToken, "segmentToken");
        //ExceptionUtils.CheckArgumentNotNull(state, "state");

        if (segmentToken.NextToken != null)
        {
            return Bind(segmentToken.NextToken, context);
        }
        else
        {
            RangeVariable implicitRangeVariable = context.ImplicitRangeVariable;
            return new RangeVariableReferenceNode(implicitRangeVariable);
        }
    }

    /// <summary>
    /// Binds a function call token.
    /// </summary>
    /// <param name="functionCallToken">The function call token to bind.</param>
    /// <returns>The bound function call token.</returns>
    //protected virtual QueryNode BindFunctionCall(FunctionCallToken functionCallToken, QueryOptionParserContext context)
    //{
    //    FunctionCallBinder functionCallBinder = new FunctionCallBinder(this.Bind, this.BindingState);
    //    return functionCallBinder.BindFunctionCall(functionCallToken);
    //}

    /// <summary>
    /// Binds a StringLiteral token.
    /// </summary>
    /// <param name="stringLiteralToken">The StringLiteral token to bind.</param>
    /// <returns>The bound StringLiteral token.</returns>
    //protected virtual QueryNode BindStringLiteral(StringLiteralToken stringLiteralToken)
    //{
    //    return new SearchTermNode(stringLiteralToken.Text);
    //}

    /// <summary>
    /// Binds an In token.
    /// </summary>
    /// <param name="inToken">The In token to bind.</param>
    /// <returns>The bound In token.</returns>
    //protected virtual QueryNode BindIn(InToken inToken)
    //{
    //    Func<QueryToken, QueryNode> InBinderMethod = (queryToken) =>
    //    {
    //        ExceptionUtils.CheckArgumentNotNull(queryToken, "queryToken");

    //        if (queryToken.Kind == QueryTokenKind.Literal)
    //        {
    //            return LiteralBinder.BindInLiteral((LiteralToken)queryToken);
    //        }

    //        return this.Bind(queryToken);
    //    };

    //    InBinder inBinder = new InBinder(InBinderMethod);
    //    return inBinder.BindInOperator(inToken, this.BindingState);
    //}

    /// <summary>
    /// Binds a CountSegment token.
    /// </summary>
    /// <param name="countSegmentToken">The CountSegment token to bind.</param>
    /// <returns>The bound CountSegment token.</returns>
    //protected virtual QueryNode BindCountSegment(CountSegmentToken countSegmentToken)
    //{
    //    CountSegmentBinder countSegmentBinder = new CountSegmentBinder(this.Bind, this.BindingState);
    //    return countSegmentBinder.BindCountSegment(countSegmentToken);
    //}
}
