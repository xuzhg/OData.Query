//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Ast;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Parser;

public class ApplyOptionParser : QueryOptionParser, IApplyOptionParser
{
    public ApplyOptionParser()
    { }

    public virtual ApplyClause Parse(ApplyToken apply, QueryOptionParserContext context)
    {
        List<TransformationNode> transformations = new List<TransformationNode>();
        foreach (QueryToken token in apply.Transformations)
        {
            switch (token.Kind)
            {
                case QueryTokenKind.Aggregate:
                    AggregateTransformationNode aggregate = BindAggregate((AggregateToken)(token), context);
                    transformations.Add(aggregate);
                    //aggregateExpressionsCache = aggregate.AggregateExpressions;
                    //state.AggregatedPropertyNames = new HashSet<EndPathToken>(aggregate.AggregateExpressions.Select(statement => new EndPathToken(statement.Alias, null)));
                    //state.IsCollapsed = true;
                    break;
                case QueryTokenKind.AggregateGroupBy:
                    GroupByTransformationNode groupBy = BindGroupByToken((GroupByToken)(token), context);
                    transformations.Add(groupBy);
                  //  state.IsCollapsed = true;
                    break;
                case QueryTokenKind.Compute:
                    var compute = BindComputeToken((ComputeToken)token, context);
                    transformations.Add(compute);
                  //  state.AggregatedPropertyNames = new HashSet<EndPathToken>(compute.Expressions.Select(statement => new EndPathToken(statement.Alias, null)));
                    break;
                //case QueryTokenKind.Expand:
                //    SelectExpandClause expandClause = SelectExpandSemanticBinder.Bind(this.odataPathInfo, (ExpandToken)token, null, this.configuration, null);
                //    ExpandTransformationNode expandNode = new ExpandTransformationNode(expandClause);
                //    transformations.Add(expandNode);
                //    break;
                //default:
                //    FilterClause filterClause = this.filterBinder.BindFilter(token);
                //    FilterTransformationNode filterNode = new FilterTransformationNode(filterClause);
                //    transformations.Add(filterNode);
                //    break;
            }
        }

        return new ApplyClause(transformations);
    }

    protected virtual AggregateTransformationNode BindAggregate(AggregateToken token, QueryOptionParserContext context)
    {
        IEnumerable<AggregateTokenBase> aggregateTokens = MergeEntitySetAggregates(token.AggregateExpressions, context);
        List<AggregateExpressionBase> statements = new List<AggregateExpressionBase>();

        foreach (AggregateTokenBase statementToken in aggregateTokens)
        {
            statements.Add(BindAggregateExpressionToken(statementToken, context));
        }

        return new AggregateTransformationNode(statements);
    }

    private static IEnumerable<AggregateTokenBase> MergeEntitySetAggregates(IEnumerable<AggregateTokenBase> tokens, QueryOptionParserContext context)
    {
        List<AggregateTokenBase> mergedTokens = new List<AggregateTokenBase>();
        Dictionary<string, AggregateTokenBase> entitySetTokens = new Dictionary<string, AggregateTokenBase>();

        foreach (AggregateTokenBase token in tokens)
        {
            switch (token.Kind)
            {
                //case QueryTokenKind.EntitySetAggregateExpression:
                //    {
                //        AggregateTokenBase currentValue;
                //        EntitySetAggregateToken entitySetToken = token as EntitySetAggregateToken;
                //        string key = entitySetToken.Path();

                //        if (entitySetTokens.TryGetValue(key, out currentValue))
                //        {
                //            entitySetTokens.Remove(key);
                //        }

                //        entitySetTokens.Add(key, EntitySetAggregateToken.Merge(entitySetToken, currentValue as EntitySetAggregateToken));
                //        break;
                //    }

                case QueryTokenKind.AggregateExpression:
                    {
                        mergedTokens.Add(token);
                        break;
                    }
            }
        }

        return mergedTokens.Concat(entitySetTokens.Values).ToList();
    }

    protected virtual AggregateExpressionBase BindAggregateExpressionToken(AggregateTokenBase aggregateToken, QueryOptionParserContext context)
    {
        switch (aggregateToken.Kind)
        {
            case QueryTokenKind.AggregateExpression:
                {
                    AggregateExpressionToken token = aggregateToken as AggregateExpressionToken;
                    SingleValueNode expression = Bind(token.Expression, context) as SingleValueNode;
                    //IEdmTypeReference typeReference = CreateAggregateExpressionTypeReference(expression, token.MethodDefinition);

                    //// TODO: Determine source
                    //return new AggregateExpression(expression, token.MethodDefinition, token.Alias, typeReference);
                }

                //case QueryTokenKind.EntitySetAggregateExpression:
                //    {
                //        EntitySetAggregateToken token = aggregateToken as EntitySetAggregateToken;
                //        QueryNode boundPath = Bind(token.EntitySet, context);

                //        state.InEntitySetAggregation = true;
                //        IEnumerable<AggregateExpressionBase> children = token.Expressions.Select(x => BindAggregateExpressionToken(x)).ToList();
                //        state.InEntitySetAggregation = false;
                //        return new EntitySetAggregateExpression((CollectionNavigationNode)boundPath, children);
                //    }
                break;

            default:
                throw new Exception("ODataErrorStrings.ApplyBinder_UnsupportedAggregateKind(aggregateToken.Kind)");
        }

        return null;
    }

    protected virtual GroupByTransformationNode BindGroupByToken(GroupByToken token, QueryOptionParserContext context)
    {
        List<GroupByPropertyNode> properties = new List<GroupByPropertyNode>();

        foreach (EndPathToken propertyToken in token.Properties)
        {
            QueryNode bindResult = Bind(propertyToken, context);
            SingleValuePropertyAccessNode property = bindResult as SingleValuePropertyAccessNode;
            //SingleComplexNode complexProperty = bindResult as SingleComplexNode;

            //if (property != null)
            //{
            //    RegisterProperty(properties, ReversePropertyPath(property));
            //}
            //else if (complexProperty != null)
            //{
            //    RegisterProperty(properties, ReversePropertyPath(complexProperty));
            //}
            //else
            //{
            //    SingleValueOpenPropertyAccessNode openProperty = bindResult as SingleValueOpenPropertyAccessNode;
            //    if (openProperty != null)
            //    {
            //        IEdmTypeReference type = GetTypeReferenceByPropertyName(openProperty.Name);
            //        properties.Add(new GroupByPropertyNode(openProperty.Name, openProperty, type));
            //    }
            //    else
            //    {
            //        throw new ODataException(
            //            ODataErrorStrings.ApplyBinder_GroupByPropertyNotPropertyAccessValue(propertyToken.Identifier));
            //    }
            //}
        }

        var newProperties = new HashSet<EndPathToken>(((GroupByToken)token).Properties);

        TransformationNode aggregate = null;
        if (token.Child != null)
        {
            if (token.Child.Kind == QueryTokenKind.Aggregate)
            {
                aggregate = BindAggregate((AggregateToken)token.Child, context);
                //aggregateExpressionsCache = ((AggregateTransformationNode)aggregate).AggregateExpressions;
                //newProperties.UnionWith(aggregateExpressionsCache.Select(statement => new EndPathToken(statement.Alias, null)));
            }
            else
            {
                throw new Exception("ODataErrorStrings.ApplyBinder_UnsupportedGroupByChild(token.Child.Kind)");
            }
        }

        //state.AggregatedPropertyNames = newProperties;

        // TODO: Determine source
        return new GroupByTransformationNode(properties, aggregate, null);
    }

    protected virtual ComputeTransformationNode BindComputeToken(ComputeToken token, QueryOptionParserContext context)
    {
        var statements = new List<ComputeExpression>();
        foreach (ComputeExpressionToken statementToken in token.Expressions)
        {
            var singleValueNode = (SingleValueNode)Bind(statementToken.Expression, context);
            statements.Add(new ComputeExpression(singleValueNode, statementToken.Alias, singleValueNode.NodeType));
        }

        return new ComputeTransformationNode(statements);
    }
}
