//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Tokenization;

/// <summary>
/// Tokenize the $compute query expression and produces the lexical object model.
/// The $compute system query option allows clients to define computed properties
/// that can be used in a $select or within a $filter or $orderby expression.
/// </summary>
public class ComputeOptionTokenizer : QueryTokenizer, IComputeOptionTokenizer
{
    internal static ComputeOptionTokenizer Default = new ComputeOptionTokenizer(ExpressionLexerFactory.Default);

    private ILexerFactory _lexerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComputeOptionTokenizer" /> class.
    /// </summary>
    /// <param name="factory"></param>
    public ComputeOptionTokenizer(ILexerFactory factory)
    {
        _lexerFactory = factory;
    }

    /// <summary>
    /// Tokenize $compute query option to lexical object model.
    /// </summary>
    /// <param name="compute">The $compute expression string to tokenize.</param>
    /// <returns>The compute token tokenized.</returns>
    public virtual ValueTask<ComputeToken> TokenizeAsync(string compute, QueryTokenizerContext context)
    {
        if (string.IsNullOrEmpty(compute))
        {
            return ValueTask.FromResult<ComputeToken>(null);
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        IExpressionLexer lexer = _lexerFactory.CreateLexer(compute, LexerOptions.Default);
        lexer.NextToken(); // move to first token

        context.EnterRecurse();

        List<ComputeItemToken> transformationTokens = new List<ComputeItemToken>();
        while (true)
        {
            ComputeItemToken computed = TokenizeComputeItem(lexer, context);
            transformationTokens.Add(computed);

            if (lexer.CurrentToken.Kind != ExpressionKind.Comma)
            {
                break;
            }

            // move over 'comma'
            lexer.NextToken();
        }

        context.LeaveRecurse();
        lexer.ValidateToken(ExpressionKind.EndOfInput);

        return ValueTask.FromResult(new ComputeToken(transformationTokens));
    }

    /// <summary>
    /// Tokenize compute item text into a token.
    /// </summary>
    /// <param name="lexer">The expression lexer.</param>
    /// <param name="context">The tokenizer context.</param>
    /// <returns>The lexical token representing the compute item text.</returns>
    protected virtual ComputeItemToken TokenizeComputeItem(IExpressionLexer lexer, QueryTokenizerContext context)
    {
        // compute expression
        IQueryToken expression = TokenizeExpression(lexer, context);

        if (!lexer.IsCurrentTokenIdentifier("as", true))
        {
            throw new QueryTokenizerException(Error.Format(SRResources.QueryTokenizer_KeyWordExpected, "as", lexer.CurrentToken.Position, lexer.ExpressionText));
        }

        // move over "as" keyword
        lexer.NextToken();

        // Get the identifier and move over
        string alias = lexer.GetIdentifier().ToString();
        lexer.NextToken();

        return new ComputeItemToken(expression, alias);
    }
}