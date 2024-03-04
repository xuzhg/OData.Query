//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

public class ComputeOptionTokenizer : QueryTokenizer, IComputeOptionTokenizer
{
    private ILexerFactory _lexerFactory;

    public ComputeOptionTokenizer(ILexerFactory factory)
    {
        _lexerFactory = factory;
    }

    public virtual ComputeToken Tokenize(string compute, QueryTokenizerContext context)
    {
        List<ComputeExpressionToken> transformationTokens = new List<ComputeExpressionToken>();

        if (string.IsNullOrEmpty(compute))
        {
            return new ComputeToken(transformationTokens);
        }

        IExpressionLexer lexer = _lexerFactory.CreateLexer(compute, LexerOptions.Default);

        context.EnterRecurse();
        // this.lexer = CreateLexerForFilterOrOrderByOrApplyExpression(compute);

        while (true)
        {
            ComputeExpressionToken computed = ParseComputeExpression(lexer, context);
            transformationTokens.Add(computed);
            if (lexer.CurrentToken.Kind != ExpressionKind.Comma)
            {
                break;
            }

            lexer.NextToken();
        }

        lexer.ValidateToken(ExpressionKind.EndOfInput);

        return new ComputeToken(transformationTokens);
    }

    protected virtual ComputeExpressionToken ParseComputeExpression(IExpressionLexer tokenizer, QueryTokenizerContext context)
    {
        throw new NotImplementedException();
    }
}