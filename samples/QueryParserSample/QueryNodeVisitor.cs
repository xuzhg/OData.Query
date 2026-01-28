using Microsoft.OData.Query.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParserSample
{
    internal class QueryNodeVisitor
    {
        public void Visit(QueryNode node, int indent)
        {
            if (node is SingleValueNode singleValueNode)
            {
                Visit(singleValueNode, indent);
            }
            else if (node is CollectionValueNode collectionValueNode)
            {
                Visit(collectionValueNode, indent);
            }
        }

        public void Visit(SingleValueNode node, int indent)
        {
            switch (node.Kind)
            {
                case QueryNodeKind.Constant:
                    var constantNode = (ConstantNode)node;
                    Console.WriteLine($"{new string(' ', indent)}ConstantNode: {constantNode.Value}");
                    break;
                case QueryNodeKind.SingleValuePropertyAccess:
                    var propertyAccessNode = (SingleValuePropertyAccessNode)node;
                    Visit(propertyAccessNode, indent + 2);
                    break;

                case QueryNodeKind.BinaryOperator:
                    Visit((BinaryOperatorNode)node, indent + 2);
                    break;

                default:
                    Console.WriteLine($"{new string(' ', indent)}Unhandled SingleValueNode kind: {node.Kind}");
                    break;
            }
        }

        public void Visit(CollectionValueNode node, int indent)
        {
        }

        public void Visit(BinaryOperatorNode node, int indent)
        {
            Visit(node.Left, indent);
            Visit(node.Right, indent);
        }

        public void Visit(SingleValuePropertyAccessNode node, int indent)
        {
            Visit(node.Source, indent);

            Console.WriteLine($"{new string(' ', indent)}PropertyAccessNode: {node.Property.Name}");
        }
    }
}
