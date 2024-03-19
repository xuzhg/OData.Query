//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.Nodes;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Base class to tokenize an OData query.
/// </summary>
public abstract class QueryTokenizer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IQueryTokenizer" /> class.
    /// </summary>
    protected QueryTokenizer()
    { }

    /// <summary>
    /// Tokenize the expression to tokens
    /// </summary>
    /// <param name="lexer">The expression lexer.</param>
    /// <param name="context">The tokenizer context.</param>
    /// <returns>The token tokenized.</returns>
    protected virtual IQueryToken TokenizeExpression(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        IQueryToken result = TokenizeLogicalOr(lexer, context);
        context.LeaveRecurse();
        return result;
    }

    /// <summary>
    /// Tokenize the 'or' operator.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected virtual IQueryToken TokenizeLogicalOr(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        IQueryToken left = TokenizeLogicalAnd(lexer, context);
        while (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordOr, context.EnableCaseInsensitive))
        {
            lexer.NextToken();
            IQueryToken right = this.TokenizeLogicalAnd(lexer, context);
            left = new BinaryOperatorToken(BinaryOperatorKind.Or, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Tokenize the 'and' operator.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected virtual IQueryToken TokenizeLogicalAnd(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        IQueryToken left = this.TokenizeComparison(lexer, context);
        while (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordAnd, context.EnableCaseInsensitive))
        {
            lexer.NextToken();
            IQueryToken right = TokenizeComparison(lexer, context);
            left = new BinaryOperatorToken(BinaryOperatorKind.And, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Tokenize the 'eq', 'ne', 'lt', 'gt', 'le', and 'ge' operators.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected virtual IQueryToken TokenizeComparison(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        IQueryToken left = TokenizeAdditive(lexer, context);
        while (true)
        {
            BinaryOperatorKind binaryOperatorKind;
            if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordEqual, context.EnableCaseInsensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.Equal;
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordNotEqual, context.EnableCaseInsensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.NotEqual;
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordGreaterThan, context.EnableCaseInsensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.GreaterThan;
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordGreaterThanOrEqual, context.EnableCaseInsensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.GreaterThanOrEqual;
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordLessThan, context.EnableCaseInsensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.LessThan;
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordLessThanOrEqual, context.EnableCaseInsensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.LessThanOrEqual;
            }
            else
            {
                break;
            }

            lexer.NextToken();
            IQueryToken right = TokenizeAdditive(lexer, context);
            left = new BinaryOperatorToken(binaryOperatorKind, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Tokenize the 'add', 'sub' operators.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected virtual IQueryToken TokenizeAdditive(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        IQueryToken left = TokenizeMultiplicative(lexer, context);
        while (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordAdd, context.EnableCaseInsensitive) ||
            lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordSub, context.EnableCaseInsensitive))
        {
            BinaryOperatorKind binaryOperatorKind;
            if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordAdd, context.EnableCaseInsensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.Add;
            }
            else
            {
                Debug.Assert(lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordSub, context.EnableCaseInsensitive), "Was a new binary operator added?");
                binaryOperatorKind = BinaryOperatorKind.Subtract;
            }

            lexer.NextToken();
            IQueryToken right = TokenizeMultiplicative(lexer, context);
            left = new BinaryOperatorToken(binaryOperatorKind, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Tokenize the '-', 'not' unary operators.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected virtual IQueryToken TokenizeUnary(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();

        if (lexer.CurrentToken.Kind == ExpressionKind.Minus || lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordNot, context.EnableCaseInsensitive))
        {
            ExpressionToken operatorToken = lexer.CurrentToken;
            lexer.NextToken();
            if (operatorToken.Kind == ExpressionKind.Minus && (ExpressionLexerUtils.IsNumericTokenKind(lexer.CurrentToken.Kind)))
            {
                //OToken numberLiteral = lexer.CurrentToken;
                //numberLiteral.Text = "-" + numberLiteral.Text;
                //numberLiteral.Position = operatorToken.Position;
                //lexer.CurrentToken = numberLiteral;
                //context.LeaveRecurse();
                //return TokenizeInHas(lexer, context);
                throw new QueryTokenizerException("TODO");
            }

            IQueryToken operand = TokenizeUnary(lexer, context);
            UnaryOperatorKind unaryOperatorKind;
            if (operatorToken.Kind == ExpressionKind.Minus)
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
        return TokenizeInHas(lexer, context);
    }

    /// <summary>
    /// Tokenize the 'has' and 'in' operators.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected virtual IQueryToken TokenizeInHas(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        IQueryToken left = TokenizePrimary(lexer, context);
        while (true)
        {
            if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordIn, context.EnableCaseInsensitive))
            {
                lexer.NextToken();
                IQueryToken right = TokenizePrimary(lexer, context);
                left = new InToken(left, right);
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordHas, context.EnableCaseInsensitive))
            {
                lexer.NextToken();
                IQueryToken right = TokenizePrimary(lexer, context);
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
    /// Tokenize the primary expressions.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected virtual IQueryToken TokenizePrimary(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        //IQueryToken expr = this.aggregateExpressionParents.Count > 0 ? this.aggregateExpressionParents.Peek() : null;
        //if (this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Slash)
        //{
        //    expr = TokenizeSegment(expr);
        //}
        //else
        //{
        //    expr = TokenizePrimaryStart(lexer, context);
        //}
        IQueryToken expr = TokenizePrimaryStart(lexer, context);

        while (lexer.CurrentToken.Kind == ExpressionKind.Slash)
        {
            lexer.NextToken();
            if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordAny, context.EnableCaseInsensitive))
            {
                expr = TokenizeAnyAll(lexer, context, expr, true);
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordAll, context.EnableCaseInsensitive))
            {
                expr = TokenizeAnyAll(lexer, context, expr, false);
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.QueryOptionCount, context.EnableCaseInsensitive))
            {
                expr = TokenizeCountSegment(lexer, context, expr);
            }
            //else if (this.lexer.PeekNextToken().Kind == ExpressionKind.Slash)
            //{
            //    expr = TokenizeSegment(expr);
            //}
            else
            {
                expr = TokenizeIdentifier(lexer, context, expr);
            }
        }

        context.LeaveRecurse();
        return expr;
    }

    protected virtual IQueryToken TokenizeIdentifier(IExpressionLexer lexer, QueryTokenizerContext context, IQueryToken parent)
    {
        lexer.ValidateToken(ExpressionKind.Identifier);

        string identifier = lexer.GetIdentifier().ToString();

        if (parent == null && context.ContainsParameter(identifier))
        {
            lexer.NextToken();
            return new RangeVariableToken(identifier);
        }

        lexer.NextToken();
        return new EndPathToken(identifier, parent);
    }

    /// <summary>
    /// Tokenize a $count segment.
    /// </summary>
    /// <param name="parent">The parent of the segment node.</param>
    /// <returns>The lexical token representing the $count segment.</returns>
    protected virtual IQueryToken TokenizeCountSegment(IExpressionLexer lexer, QueryTokenizerContext context, IQueryToken parent)
    {
        lexer.NextToken();

        //CountSegmentParser countSegmentParser = new CountSegmentParser(this.lexer, this);
        //return countSegmentParser.CreateCountSegmentToken(parent);
        return null;
    }

    /// <summary>
    /// Tokenize the start of primary expressions.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected virtual IQueryToken TokenizePrimaryStart(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        switch (lexer.CurrentToken.Kind)
        {
            //case ExpressionKind.ParameterAlias:
            //    {
            //        return TokenizeParameterAlias(this.lexer);
            //    }

            case ExpressionKind.Identifier:
                {
                    //IdentifierTokenizer identifierTokenizer = new IdentifierTokenizer(this.parameters, new FunctionCallParser(this.lexer, this, this.IsInAggregateExpression));
                    //IQueryToken parent = this.aggregateExpressionParents.Count > 0 ? this.aggregateExpressionParents.Peek() : null;
                    // return identifierTokenizer.TokenizeIdentifier(parent);
                    return TokenizeIdentifier(lexer, context, null);
                }

            case ExpressionKind.OpenParen:
                {
                    return TokenizeParenExpression(lexer, context);
                }

            //case ExpressionKind.Star:
            //    {
            //        IdentifierTokenizer identifierTokenizer = new IdentifierTokenizer(this.parameters, new FunctionCallParser(this.lexer, this, this.IsInAggregateExpression));
            //        return identifierTokenizer.ParseStarMemberAccess(null);
            //    }

            default:
                {
                    IQueryToken primitiveLiteralToken = TryTokenizeLiteral(lexer, context);
                    if (primitiveLiteralToken == null)
                    {
                        throw new QueryTokenizerException("ODataErrorStrings.UriQueryExpressionParser_ExpressionExpected(this.lexer.CurrentToken.Position, this.lexer.ExpressionText)");
                    }

                    return primitiveLiteralToken;
                }
        }
    }

    /// <summary>
    /// Tokenize a literal.
    /// </summary>
    /// <param name="lexer">The lexer to use.</param>
    /// <returns>The literal query token or null if something else was found.</returns>
    internal static LiteralToken TryTokenizeLiteral(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        switch (lexer.CurrentToken.Kind)
        {
            case ExpressionKind.BooleanLiteral:
                bool boolValue = bool.Parse(lexer.CurrentToken.Text);
                lexer.NextToken();
                return new LiteralToken(boolValue, null, typeof(bool));

            case ExpressionKind.DateOnlyLiteral:
                DateOnly dateOnlyValue = DateOnly.Parse(lexer.CurrentToken.Text);
                lexer.NextToken();
                return new LiteralToken(dateOnlyValue, null, typeof(DateOnly));

            case ExpressionKind.DecimalLiteral:
                decimal decimalValue = decimal.Parse(lexer.CurrentToken.Text);
                lexer.NextToken();
                return new LiteralToken(decimalValue, null, typeof(decimal));

            case ExpressionKind.StringLiteral:
                string stringValue = lexer.CurrentToken.Text.ToString();
                lexer.NextToken();
                return new LiteralToken(stringValue, null, typeof(string));

            case ExpressionKind.Int64Literal:
                long longValue = long.Parse(lexer.CurrentToken.Text);
                lexer.NextToken();
                return new LiteralToken(longValue, null, typeof(long));

            case ExpressionKind.IntegerLiteral:
                int intValue = int.Parse(lexer.CurrentToken.Text);
                lexer.NextToken();
                return new LiteralToken(intValue, null, typeof(int));

            case ExpressionKind.DoubleLiteral:
                double doubleValue = double.Parse(lexer.CurrentToken.Text);
                lexer.NextToken();
                return new LiteralToken(doubleValue, null, typeof(double));

            case ExpressionKind.SingleLiteral:
                float floatValue = float.Parse(lexer.CurrentToken.Text);
                lexer.NextToken();
                return new LiteralToken(floatValue, null, typeof(float));

            case ExpressionKind.GuidLiteral:
                Guid guidValue = Guid.Parse(lexer.CurrentToken.Text);
                lexer.NextToken();
                return new LiteralToken(guidValue, null, typeof(Guid));

            case ExpressionKind.BinaryLiteral:
                // Binary is a binary prefix base64 string.
                ReadOnlySpan<char> tokenText = lexer.CurrentToken.Text;
                tokenText = tokenText.Slice(7, tokenText.Length - 8);// binary + two single quotes
                byte[] byteArrayValue = Convert.FromBase64String(tokenText.ToString());
                lexer.NextToken();
                return new LiteralToken(byteArrayValue, null, typeof(byte[]));

            //case ExpressionKind.GeographyLiteral:
            //case ExpressionKind.GeometryLiteral:
            //case ExpressionKind.QuotedLiteral:
            case ExpressionKind.DurationLiteral:
                TimeSpan timeSpanValue = TimeSpan.Parse(lexer.CurrentToken.Text);
                lexer.NextToken();
                return new LiteralToken(timeSpanValue, null, typeof(TimeSpan));

            case ExpressionKind.TimeOnlyLiteral:
                TimeOnly timeOnlyValue = TimeOnly.Parse(lexer.CurrentToken.Text);
                lexer.NextToken();
                return new LiteralToken(timeOnlyValue, null, typeof(TimeOnly));

            case ExpressionKind.DateTimeOffsetLiteral:
                DateTimeOffset dtoValue = DateTimeOffset.Parse(lexer.CurrentToken.Text);
                lexer.NextToken();
                return new LiteralToken(dtoValue, null, typeof(DateTimeOffset));

            //     case ExpressionKind.CustomTypeLiteral:
            //IEdmTypeReference literalEdmTypeReference = lexer.CurrentToken.GetLiteralEdmTypeReference();

            //// Why not using EdmTypeReference.FullName? (literalEdmTypeReference.FullName)
            //string edmConstantName = GetEdmConstantNames(literalEdmTypeReference);
            //return new LiteralToken(lexer, literalEdmTypeReference, edmConstantName);

            //case ExpressionKind.BracedExpression:
            //case ExpressionKind.BracketedExpression:
            //case ExpressionKind.ParenthesesExpression:
            //    {
            //        LiteralToken result = new LiteralToken(lexer.CurrentToken.Text, lexer.CurrentToken.Text);
            //        lexer.NextToken();
            //        return result;
            //    }

            case ExpressionKind.NullLiteral:
                return TokenizeNullLiteral(lexer, context);

            default:
                return null;
        }
    }

    /// <summary>
    /// Tokenize 'null' literals.
    /// </summary>
    /// <param name="lexer">The lexer to use.</param>
    /// <returns>The literal token produced by building the given literal.</returns>
    private static LiteralToken TokenizeNullLiteral(IExpressionLexer lexer, QueryTokenizerContext context)
    {
       // Debug.Assert(lexer != null, "lexer != null");
      //  Debug.Assert(lexer.CurrentToken.Kind == ExpressionTokenKind.NullLiteral, "this.lexer.CurrentToken.InternalKind == ExpressionTokenKind.NullLiteral");

        LiteralToken result = new LiteralToken(null, lexer.CurrentToken.Text.ToString());

        lexer.NextToken();
        return result;
    }

    /// <summary>
    /// Tokenize parenthesized expressions.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    protected virtual IQueryToken TokenizeParenExpression(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        if (lexer.CurrentToken.Kind != ExpressionKind.OpenParen)
        {
            throw new QueryTokenizerException(Error.Format(SRResources.QueryOptionParser_OpenParenExpected, lexer.CurrentToken.Position, lexer.ExpressionText));
        }

        lexer.NextToken();
        IQueryToken result = TokenizeExpression(lexer, context);
        if (lexer.CurrentToken.Kind != ExpressionKind.CloseParen)
        {
            throw new QueryTokenizerException(Error.Format(SRResources.QueryOptionParser_CloseParenOrCommaExpected, lexer.CurrentToken.Position, lexer.ExpressionText));
        }

        lexer.NextToken();
        return result;
    }

    /// <summary>
    /// Tokenize the mul, div, mod operators.
    /// </summary>
    /// <returns>The lexical token representing the expression.</returns>
    private IQueryToken TokenizeMultiplicative(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        context.EnterRecurse();
        IQueryToken left = TokenizeUnary(lexer, context);
        while (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordMultiply, context.EnableCaseInsensitive) ||
            lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordDivide, context.EnableCaseInsensitive) ||
            lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordModulo, context.EnableCaseInsensitive))
        {
            BinaryOperatorKind binaryOperatorKind;
            if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordMultiply, context.EnableCaseInsensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.Multiply;
            }
            else if (lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordDivide, context.EnableCaseInsensitive))
            {
                binaryOperatorKind = BinaryOperatorKind.Divide;
            }
            else
            {
                Debug.Assert(lexer.IsCurrentTokenIdentifier(TokenConstants.KeywordModulo, context.EnableCaseInsensitive), "Was a new binary operator added?");
                binaryOperatorKind = BinaryOperatorKind.Modulo;
            }

            lexer.NextToken();
            IQueryToken right = TokenizeUnary(lexer, context);
            left = new BinaryOperatorToken(binaryOperatorKind, left, right);
        }

        context.LeaveRecurse();
        return left;
    }

    /// <summary>
    /// Tokenize the 'Any'/'All' portion of the query
    /// </summary>
    /// <param name="parent">The parent of the Any/All node.</param>
    /// <param name="isAny">Denotes whether an Any or All is to be tokenized.</param>
    /// <returns>The lexical token representing the Any/All query.</returns>
    private IQueryToken TokenizeAnyAll(IExpressionLexer lexer, QueryTokenizerContext context, IQueryToken parent, bool isAny)
    {
        lexer.NextToken();
        if (lexer.CurrentToken.Kind != ExpressionKind.OpenParen)
        {
            throw new QueryTokenizerException(Error.Format(SRResources.QueryOptionParser_OpenParenExpected, lexer.CurrentToken.Position, lexer.ExpressionText));
        }

        lexer.NextToken();

        // When faced with Any(), return the same thing as if you encountered Any(a : true)
        if (lexer.CurrentToken.Kind == ExpressionKind.CloseParen)
        {
            lexer.NextToken();
            if (isAny)
            {
                return new AnyToken(new LiteralToken(true, "True"), null, parent);
            }
            else
            {
                return new AllToken(new LiteralToken(true, "True"), null, parent);
            }
        }

        string parameter = lexer.GetIdentifier().ToString();
        context.AddParameter(parameter);

        // read the ':' separating the range variable from the expression.
        lexer.NextToken();
        ValidateToken(lexer, ExpressionKind.Colon);

        lexer.NextToken();
        IQueryToken expr = TokenizeExpression(lexer, context);
        if (lexer.CurrentToken.Kind != ExpressionKind.CloseParen)
        {
            throw new QueryTokenizerException(Error.Format(SRResources.QueryOptionParser_CloseParenOrCommaExpected, lexer.CurrentToken.Position, lexer.ExpressionText));
        }

        // forget about the range variable after parsing the expression for this lambda.
        context.RemoveParameter(parameter);

        lexer.NextToken();
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
    private static void ValidateToken(IExpressionLexer lexer, ExpressionKind kind)
    {
        if (lexer.CurrentToken.Kind != kind)
        {
            throw new QueryTokenizerException(Error.Format(SRResources.QueryOptionParser_TokenKindExpected, kind, lexer.CurrentToken.Kind));
        }
    }

    internal void ValidateToken(ExpressionKind kind)
    {
        if (kind != ExpressionKind.EndOfInput && kind != ExpressionKind.Ampersand)
        {
            throw new Exception("TODO:");
        }
    }

    public virtual QueryNode ParseQuery(QueryTokenizerContext context)
    {
        throw new NotImplementedException();
    }
}
