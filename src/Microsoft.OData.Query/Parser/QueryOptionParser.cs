//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

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
    /// Parses the expression.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected virtual QueryToken ParseExpression(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        context.EnterRecurse();
        QueryToken result = ParseLogicalOr(tokenizer, context);
        context.LeaveRecurse();
        return result;
    }

    /// <summary>
    /// Parses the or operator.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected QueryToken ParseLogicalOr(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        context.EnterRecurse();
        QueryToken left = ParseLogicalAnd(tokenizer, context);
        while (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordOr, context.EnableIdentifierCaseSensitive))
        {
            tokenizer.NextToken();
            QueryToken right = this.ParseLogicalAnd(tokenizer, context);
            left = new BinaryOperatorToken(BinaryOperatorKind.Or, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Parses the and operator.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    private QueryToken ParseLogicalAnd(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        context.EnterRecurse();
        QueryToken left = this.ParseComparison(tokenizer, context);
        while (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordAnd, context.EnableIdentifierCaseSensitive))
        {
            tokenizer.NextToken();
            QueryToken right = ParseComparison(tokenizer, context);
            left = new BinaryOperatorToken(BinaryOperatorKind.And, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Parses the eq, ne, lt, gt, le, and ge operators.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    private QueryToken ParseComparison(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        context.EnterRecurse();
        QueryToken left = ParseAdditive(tokenizer, context);
        while (true)
        {
            BinaryOperatorKind binaryOperatorKind;
            if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordEqual, context.EnableIdentifierCaseSensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.Equal;
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordNotEqual, context.EnableIdentifierCaseSensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.NotEqual;
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordGreaterThan, context.EnableIdentifierCaseSensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.GreaterThan;
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordGreaterThanOrEqual, context.EnableIdentifierCaseSensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.GreaterThanOrEqual;
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordLessThan, context.EnableIdentifierCaseSensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.LessThan;
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordLessThanOrEqual, context.EnableIdentifierCaseSensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.LessThanOrEqual;
            }
            else
            {
                break;
            }

            tokenizer.NextToken();
            QueryToken right = ParseAdditive(tokenizer, context);
            left = new BinaryOperatorToken(binaryOperatorKind, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Parses the add, sub operators.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    private QueryToken ParseAdditive(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        context.EnterRecurse();
        QueryToken left = ParseMultiplicative(tokenizer, context);
        while (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordAdd, context.EnableIdentifierCaseSensitive) ||
            tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordSub, context.EnableIdentifierCaseSensitive))
        {
            BinaryOperatorKind binaryOperatorKind;
            if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordAdd, context.EnableIdentifierCaseSensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.Add;
            }
            else
            {
                Debug.Assert(tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordSub, context.EnableIdentifierCaseSensitive), "Was a new binary operator added?");
                binaryOperatorKind = BinaryOperatorKind.Subtract;
            }

            tokenizer.NextToken();
            QueryToken right = ParseMultiplicative(tokenizer, context);
            left = new BinaryOperatorToken(binaryOperatorKind, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Parses the -, not unary operators.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    private QueryToken ParseUnary(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        context.EnterRecurse();

        if (tokenizer.CurrentToken.Kind == OTokenKind.Minus || tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordNot, context.EnableIdentifierCaseSensitive))
        {
            OToken operatorToken = tokenizer.CurrentToken;
            tokenizer.NextToken();
            if (operatorToken.Kind == OTokenKind.Minus && (OTokenizationUtils.IsNumericTokenKind(tokenizer.CurrentToken.Kind)))
            {
                //OToken numberLiteral = tokenizer.CurrentToken;
                //numberLiteral.Text = "-" + numberLiteral.Text;
                //numberLiteral.Position = operatorToken.Position;
                //tokenizer.CurrentToken = numberLiteral;
                //context.LeaveRecurse();
                //return ParseInHas(tokenizer, context);
                throw new OQueryParserException("TODO");
            }

            QueryToken operand = ParseUnary(tokenizer, context);
            UnaryOperatorKind unaryOperatorKind;
            if (operatorToken.Kind == OTokenKind.Minus)
            {
                unaryOperatorKind = UnaryOperatorKind.Negate;
            }
            else
            {
               // Debug.Assert(operatorToken.IdentifierIs(TokenConstants.KeywordNot, context.EnableIdentifierCaseSensitive), "Was a new unary operator added?");
                unaryOperatorKind = UnaryOperatorKind.Not;
            }

            context.LeaveRecurse();
            return new UnaryOperatorToken(unaryOperatorKind, operand);
        }

        context.LeaveRecurse();
        return ParseInHas(tokenizer, context);
    }

    /// <summary>
    /// Parses the has and in operators.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    private QueryToken ParseInHas(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        context.EnterRecurse();
        QueryToken left = ParsePrimary(tokenizer, context);
        while (true)
        {
            if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordIn, context.EnableIdentifierCaseSensitive))
            {
                tokenizer.NextToken();
                QueryToken right = ParsePrimary(tokenizer, context);
                left = new InToken(left, right);
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordHas, context.EnableIdentifierCaseSensitive))
            {
                tokenizer.NextToken();
                QueryToken right = this.ParsePrimary(tokenizer, context);
                left = new BinaryOperatorToken(BinaryOperatorKind.Has, left, right);
            }
            else
            {
                break;
            }
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Parses the primary expressions.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected virtual QueryToken ParsePrimary(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        context.EnterRecurse();
        //QueryToken expr = this.aggregateExpressionParents.Count > 0 ? this.aggregateExpressionParents.Peek() : null;
        //if (this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Slash)
        //{
        //    expr = ParseSegment(expr);
        //}
        //else
        //{
        //    expr = ParsePrimaryStart(tokenizer, context);
        //}
        QueryToken expr = ParsePrimaryStart(tokenizer, context);

        while (tokenizer.CurrentToken.Kind == OTokenKind.Slash)
        {
            tokenizer.NextToken();
            if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordAny, context.EnableIdentifierCaseSensitive))
            {
                expr = ParseAnyAll(tokenizer, context, expr, true);
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordAll, context.EnableIdentifierCaseSensitive))
            {
                expr = ParseAnyAll(tokenizer, context, expr, false);
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.QueryOptionCount, context.EnableIdentifierCaseSensitive))
            {
                expr = ParseCountSegment(tokenizer, context, expr);
            }
            //else if (this.lexer.PeekNextToken().Kind == OTokenKind.Slash)
            //{
            //    expr = ParseSegment(expr);
            //}
            else
            {
                expr = ParseIdentifier(tokenizer, context, expr);
            }
        }

        context.LeaveRecurse();
        return expr;
    }

    private QueryToken ParseIdentifier(IOTokenizer tokenizer, QueryOptionParserContext context, QueryToken parent)
    {
        tokenizer.ValidateToken(OTokenKind.Identifier);

        string identifier = tokenizer.GetIdentifier().ToString();

        if (parent == null && context.ContainsParameter(identifier))
        {
            tokenizer.NextToken();
            return new RangeVariableToken(identifier);
        }

        tokenizer.NextToken();
        return new EndPathToken(identifier, parent);
    }

    /// <summary>
    /// Parses a $count segment.
    /// </summary>
    /// <param name="parent">The parent of the segment node.</param>
    /// <returns>The lexical token representing the $count segment.</returns>
    private QueryToken ParseCountSegment(IOTokenizer tokenizer, QueryOptionParserContext context, QueryToken parent)
    {
        tokenizer.NextToken();

        //CountSegmentParser countSegmentParser = new CountSegmentParser(this.lexer, this);
        //return countSegmentParser.CreateCountSegmentToken(parent);
        return null;
    }

    /// <summary>
    /// Handles the start of primary expressions.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    private QueryToken ParsePrimaryStart(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        switch (tokenizer.CurrentToken.Kind)
        {
            //case OTokenKind.ParameterAlias:
            //    {
            //        return ParseParameterAlias(this.lexer);
            //    }

            case OTokenKind.Identifier:
                {
                    //IdentifierTokenizer identifierTokenizer = new IdentifierTokenizer(this.parameters, new FunctionCallParser(this.lexer, this, this.IsInAggregateExpression));
                    //QueryToken parent = this.aggregateExpressionParents.Count > 0 ? this.aggregateExpressionParents.Peek() : null;
                    // return identifierTokenizer.ParseIdentifier(parent);
                    return ParseIdentifier(tokenizer, context, null);
                }

            case OTokenKind.OpenParen:
                {
                    return ParseParenExpression(tokenizer, context);
                }

            //case OTokenKind.Star:
            //    {
            //        IdentifierTokenizer identifierTokenizer = new IdentifierTokenizer(this.parameters, new FunctionCallParser(this.lexer, this, this.IsInAggregateExpression));
            //        return identifierTokenizer.ParseStarMemberAccess(null);
            //    }

            default:
                {
                    QueryToken primitiveLiteralToken = TryParseLiteral(tokenizer, context);
                    if (primitiveLiteralToken == null)
                    {
                        throw new OQueryParserException("ODataErrorStrings.UriQueryExpressionParser_ExpressionExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
                    }

                    return primitiveLiteralToken;
                }
        }
    }

    /// <summary>
    /// Parses a literal.
    /// </summary>
    /// <param name="lexer">The lexer to use.</param>
    /// <returns>The literal query token or null if something else was found.</returns>
    internal static LiteralToken TryParseLiteral(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        switch (tokenizer.CurrentToken.Kind)
        {
            case OTokenKind.BooleanLiteral:
                bool boolValue = bool.Parse(tokenizer.CurrentToken.Text);
                tokenizer.NextToken();
                return new LiteralToken(boolValue, null, typeof(bool));

            case OTokenKind.DateOnlyLiteral:
                DateOnly dateOnlyValue = DateOnly.Parse(tokenizer.CurrentToken.Text);
                tokenizer.NextToken();
                return new LiteralToken(dateOnlyValue, null, typeof(DateOnly));

            case OTokenKind.DecimalLiteral:
                decimal decimalValue = decimal.Parse(tokenizer.CurrentToken.Text);
                tokenizer.NextToken();
                return new LiteralToken(decimalValue, null, typeof(decimal));

            case OTokenKind.StringLiteral:
                string stringValue = tokenizer.CurrentToken.Text.ToString();
                tokenizer.NextToken();
                return new LiteralToken(stringValue, null, typeof(string));

            case OTokenKind.Int64Literal:
                long longValue = long.Parse(tokenizer.CurrentToken.Text);
                tokenizer.NextToken();
                return new LiteralToken(longValue, null, typeof(long));

            case OTokenKind.IntegerLiteral:
                int intValue = int.Parse(tokenizer.CurrentToken.Text);
                tokenizer.NextToken();
                return new LiteralToken(intValue, null, typeof(int));

            case OTokenKind.DoubleLiteral:
                double doubleValue = double.Parse(tokenizer.CurrentToken.Text);
                tokenizer.NextToken();
                return new LiteralToken(doubleValue, null, typeof(double));

            case OTokenKind.SingleLiteral:
                float floatValue = float.Parse(tokenizer.CurrentToken.Text);
                tokenizer.NextToken();
                return new LiteralToken(floatValue, null, typeof(float));

            case OTokenKind.GuidLiteral:
                Guid guidValue = Guid.Parse(tokenizer.CurrentToken.Text);
                tokenizer.NextToken();
                return new LiteralToken(guidValue, null, typeof(Guid));

            case OTokenKind.BinaryLiteral:
                // Binary is a binary prefix base64 string.
                ReadOnlySpan<char> tokenText = tokenizer.CurrentToken.Text;
                tokenText = tokenText.Slice(7, tokenText.Length - 8);// binary + two single quotes
                byte[] byteArrayValue = Convert.FromBase64String(tokenText.ToString());
                tokenizer.NextToken();
                return new LiteralToken(byteArrayValue, null, typeof(byte[]));

            //case OTokenKind.GeographyLiteral:
            //case OTokenKind.GeometryLiteral:
            //case OTokenKind.QuotedLiteral:
            case OTokenKind.DurationLiteral:
                TimeSpan timeSpanValue = TimeSpan.Parse(tokenizer.CurrentToken.Text);
                tokenizer.NextToken();
                return new LiteralToken(timeSpanValue, null, typeof(TimeSpan));

            case OTokenKind.TimeOnlyLiteral:
                TimeOnly timeOnlyValue = TimeOnly.Parse(tokenizer.CurrentToken.Text);
                tokenizer.NextToken();
                return new LiteralToken(timeOnlyValue, null, typeof(TimeOnly));

            case OTokenKind.DateTimeOffsetLiteral:
                DateTimeOffset dtoValue = DateTimeOffset.Parse(tokenizer.CurrentToken.Text);
                tokenizer.NextToken();
                return new LiteralToken(dtoValue, null, typeof(DateTimeOffset));

            //     case OTokenKind.CustomTypeLiteral:
            //IEdmTypeReference literalEdmTypeReference = lexer.CurrentToken.GetLiteralEdmTypeReference();

            //// Why not using EdmTypeReference.FullName? (literalEdmTypeReference.FullName)
            //string edmConstantName = GetEdmConstantNames(literalEdmTypeReference);
            //return new LiteralToken(lexer, literalEdmTypeReference, edmConstantName);

            //case OTokenKind.BracedExpression:
            //case OTokenKind.BracketedExpression:
            //case OTokenKind.ParenthesesExpression:
            //    {
            //        LiteralToken result = new LiteralToken(lexer.CurrentToken.Text, lexer.CurrentToken.Text);
            //        lexer.NextToken();
            //        return result;
            //    }

            case OTokenKind.NullLiteral:
                return ParseNullLiteral(tokenizer, context);

            default:
                return null;
        }
    }

    /// <summary>
    /// Parses null literals.
    /// </summary>
    /// <param name="lexer">The lexer to use.</param>
    /// <returns>The literal token produced by building the given literal.</returns>
    private static LiteralToken ParseNullLiteral(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
       // Debug.Assert(lexer != null, "lexer != null");
      //  Debug.Assert(lexer.CurrentToken.Kind == ExpressionTokenKind.NullLiteral, "this.lexer.CurrentToken.InternalKind == ExpressionTokenKind.NullLiteral");

        LiteralToken result = new LiteralToken(null, tokenizer.CurrentToken.Text.ToString());

        tokenizer.NextToken();
        return result;
    }

    /// <summary>
    /// Parses parenthesized expressions.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    private QueryToken ParseParenExpression(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        if (tokenizer.CurrentToken.Kind != OTokenKind.OpenParen)
        {
            throw new OQueryParserException(Error.Format(SRResources.QueryOptionParser_OpenParenExpected, tokenizer.CurrentToken.Position, tokenizer.Text));
        }

        tokenizer.NextToken();
        QueryToken result = ParseExpression(tokenizer, context);
        if (tokenizer.CurrentToken.Kind != OTokenKind.CloseParen)
        {
            throw new OQueryParserException(Error.Format(SRResources.QueryOptionParser_CloseParenOrCommaExpected, tokenizer.CurrentToken.Position, tokenizer.Text));
        }

        tokenizer.NextToken();
        return result;
    }

    /// <summary>
    /// Parses the mul, div, mod operators.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    private QueryToken ParseMultiplicative(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        context.EnterRecurse();
        QueryToken left = ParseUnary(tokenizer, context);
        while (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordMultiply, context.EnableIdentifierCaseSensitive) ||
            tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordDivide, context.EnableIdentifierCaseSensitive) ||
            tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordModulo, context.EnableIdentifierCaseSensitive))
        {
            BinaryOperatorKind binaryOperatorKind;
            if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordMultiply, context.EnableIdentifierCaseSensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.Multiply;
            }
            else if (tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordDivide, context.EnableIdentifierCaseSensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.Divide;
            }
            else
            {
                Debug.Assert(tokenizer.IsCurrentTokenIdentifier(TokenConstants.KeywordModulo, context.EnableIdentifierCaseSensitive), "Was a new binary operator added?");
                binaryOperatorKind = BinaryOperatorKind.Modulo;
            }

            tokenizer.NextToken();
            QueryToken right = ParseUnary(tokenizer, context);
            left = new BinaryOperatorToken(binaryOperatorKind, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Parses the Any/All portion of the query
    /// </summary>
    /// <param name="parent">The parent of the Any/All node.</param>
    /// <param name="isAny">Denotes whether an Any or All is to be parsed.</param>
    /// <returns>The lexical token representing the Any/All query.</returns>
    private QueryToken ParseAnyAll(IOTokenizer tokenizer, QueryOptionParserContext context, QueryToken parent, bool isAny)
    {
        tokenizer.NextToken();
        if (tokenizer.CurrentToken.Kind != OTokenKind.OpenParen)
        {
            throw new OQueryParserException(Error.Format(SRResources.QueryOptionParser_OpenParenExpected, tokenizer.CurrentToken.Position, tokenizer.Text));
        }

        tokenizer.NextToken();

        // When faced with Any(), return the same thing as if you encountered Any(a : true)
        if (tokenizer.CurrentToken.Kind == OTokenKind.CloseParen)
        {
            tokenizer.NextToken();
            if (isAny)
            {
                return new AnyToken(new LiteralToken(true, "True"), null, parent);
            }
            else
            {
                return new AllToken(new LiteralToken(true, "True"), null, parent);
            }
        }

        string parameter = tokenizer.GetIdentifier().ToString();
        context.AddParameter(parameter);

        // read the ':' separating the range variable from the expression.
        tokenizer.NextToken();
        ValidateToken(tokenizer, OTokenKind.Colon);

        tokenizer.NextToken();
        QueryToken expr = ParseExpression(tokenizer, context);
        if (tokenizer.CurrentToken.Kind != OTokenKind.CloseParen)
        {
            throw new OQueryParserException(Error.Format(SRResources.QueryOptionParser_CloseParenOrCommaExpected, tokenizer.CurrentToken.Position, tokenizer.Text));
        }

        // forget about the range variable after parsing the expression for this lambda.
        context.RemoveParameter(parameter);

        tokenizer.NextToken();
        if (isAny)
        {
            return new AnyToken(expr, parameter, parent);
        }
        else
        {
            return new AllToken(expr, parameter, parent);
        }
    }

    /// <summary>
    /// Validates the current token is of the specified kind.
    /// </summary>
    /// <param name="kind">Expected token kind.</param>
    private static void ValidateToken(IOTokenizer tokenizer, OTokenKind kind)
    {
        if (tokenizer.CurrentToken.Kind != kind)
        {
            throw new OQueryParserException(Error.Format(SRResources.QueryOptionParser_TokenKindExpected, kind, tokenizer.CurrentToken.Kind));
        }
    }

    internal void ValidateToken(OTokenKind kind)
    {
        if (kind != OTokenKind.EndOfInput && kind != OTokenKind.Ampersand)
        {
            throw new Exception("TODO:");
        }
    }

    public virtual QueryNode ParseQuery(QueryOptionParserContext context)
    {
        throw new NotImplementedException();
    }
}
