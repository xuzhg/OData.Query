//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.SyntacticAst;
using Microsoft.OData.Query.Tokenization;

namespace Microsoft.OData.Query.Parser;

public class ComputeOptionParser : QueryOptionParser, IComputeOptionParser
{
    private IOTokenizerFactory _tokenizerFactory;

    public ComputeOptionParser(IOTokenizerFactory factory)
    {
        _tokenizerFactory = factory;
    }

    public virtual ComputeToken ParseCompute(string compute, QueryOptionParserContext context)
    {
        List<ComputeExpressionToken> transformationTokens = new List<ComputeExpressionToken>();

        if (string.IsNullOrEmpty(compute))
        {
            return new ComputeToken(transformationTokens);
        }

        IOTokenizer tokenizer = _tokenizerFactory.CreateTokenizer(compute, OTokenizerContext.Default);

        context.EnterRecurse();
       // this.lexer = CreateLexerForFilterOrOrderByOrApplyExpression(compute);

        while (true)
        {
            ComputeExpressionToken computed = ParseComputeExpression(tokenizer, context);
            transformationTokens.Add(computed);
            if (tokenizer.CurrentToken.Kind != OTokenKind.Comma)
            {
                break;
            }

            tokenizer.NextToken();
        }

        tokenizer.ValidateToken(OTokenKind.EndOfInput);

        return new ComputeToken(transformationTokens);
    }

    private ComputeExpressionToken ParseComputeExpression(IOTokenizer tokenizer, QueryOptionParserContext context)
    {
        throw new NotImplementedException();
    }
}