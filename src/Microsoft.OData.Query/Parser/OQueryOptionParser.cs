//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Lexers;
using Microsoft.OData.Query.SyntacticAst;

namespace Microsoft.OData.Query.Parser;

public class OQueryOptionParser : IOQueryOptionParser
{
    private readonly ILexerFactory _lexerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="OQueryOptionParser" /> class.
    /// </summary>
    /// <param name="lexerFactory">The required tokenizer.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public OQueryOptionParser(ILexerFactory lexerFactory)
    {
        _lexerFactory = lexerFactory ?? throw new ArgumentNullException(nameof(lexerFactory));
    }

    public virtual QueryToken ParseQuery(string query, QueryOptionParserContext context)
    {
        IExpressionLexer lexer = _lexerFactory.CreateLexer(query, LexerOptions.Default);

        while (lexer.NextToken())
        {
            ExpressionToken token = lexer.CurrentToken;

            if (token.Kind == ExpressionKind.Dollar)
            {
                if (!lexer.NextToken())
                {
                    break;
                }

                token = lexer.CurrentToken;
            }

            //if (token.Kind != lexer.Identifier)
            //{
            //    continue;
            //}

            ReadOnlySpan<char> identifier = token.Text.StartsWith("$", StringComparison.Ordinal) ? token.Text[1..] : token.Text;
            if (identifier.Equals("filter", StringComparison.OrdinalIgnoreCase))
            {

            }
         }

        throw new NotImplementedException();
    }
}
