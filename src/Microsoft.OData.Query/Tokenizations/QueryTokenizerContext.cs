//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using Microsoft.OData.Query.Commons;
using Microsoft.OData.Query.Lexers;

namespace Microsoft.OData.Query.Tokenizations;

/// <summary>
/// The tokenizer context
/// </summary>
public class QueryTokenizerContext
{
    internal static QueryTokenizerContext Default = new QueryTokenizerContext();

    public LexerOptions LexerOptions { get; set; } = LexerOptions.Default;

    public IServiceProvider ServiceProvider { get; set; }

    public bool EnableCaseInsensitive { get; set; } = true;

    public bool EnableNoDollarPrefix { get; set; } = true;

    /// <summary>
    /// The maximum number of recursion nesting allowed.
    /// </summary>
    private int _maxDepth = int.MaxValue;

    /// <summary>
    /// Gets or sets the maximum depth of the recursion.
    /// </summary>
    public int MaxDepth
    {
        get => _maxDepth;
        set
        {
            if (value < 0)
            {
                throw new QueryTokenizerException(SRResources.QueryTokenizer_NegativeMaxDepth);
            }

            _maxDepth = value;
        }
    }

    /// <summary>
    /// Set of parsed parameters, it's case-sensitive.
    /// </summary>
    private readonly HashSet<string> _parameters = new HashSet<string>(StringComparer.Ordinal)
    {
        TokenConstants.It, // $it
        TokenConstants.This // $this
    };

    /// <summary>
    /// Add the range variable parameter into this context.
    /// </summary>
    /// <param name="parameter">The range variable parameter.</param>
    /// <exception cref="QueryTokenizerException">Throws if duplicated range variable defined.</exception>
    public void AddParameter(string parameter)
    {
        if (!_parameters.Add(parameter))
        {
            throw new QueryTokenizerException(Error.Format(SRResources.QueryTokenizer_RangeVariableAlreadyDeclared, parameter));
        }
    }

    /// <summary>
    /// Remove the range variable parameter from this context.
    /// </summary>
    /// <param name="parameter">The range variable parameter.</param>
    public void RemoveParameter(string parameter) => _parameters.Remove(parameter);

    /// <summary>
    /// Test whether the range variable parameter is defined or not.
    /// </summary>
    /// <param name="parameter">The range variable parameter.</param>
    /// <returns></returns>
    public bool ContainsParameter(string parameter) => _parameters.Contains(parameter);

    /// <summary>
    /// The current recursion depth.
    /// </summary>
    private int _recursionDepth = 0;

    /// <summary>
    /// Enter a recursive method.
    /// </summary>
    /// <exception cref="QueryTokenizerException">Throws if exceeds the max depth.</exception>
    public void EnterRecurse()
    {
        ++_recursionDepth;
        if (_recursionDepth > MaxDepth)
        {
            throw new QueryTokenizerException(Error.Format(SRResources.QueryTokenizer_RecurseTooDeep, _recursionDepth, MaxDepth));
        }
    }

    /// <summary>
    /// Leave a recursive method.
    /// </summary>
    public void LeaveRecurse()
    {
        --_recursionDepth;
        if (_recursionDepth < 0)
        {
            throw new QueryTokenizerException(SRResources.QueryTokenizer_RecurseMismatch);
        }
    }

    public IExpressionLexer CreateLexer(string text)
    {
        return CreateLexer(text.AsMemory());
    }

    public IExpressionLexer CreateLexer(ReadOnlyMemory<char> text)
    {
        var lexerOptions = CreateLexerOption();
        if (ServiceProvider != null)
        {
            ILexerFactory lexerFactory = ServiceProvider.GetService<ILexerFactory>();
            if (lexerFactory != null)
            {
                return lexerFactory.CreateLexer(text, lexerOptions);
            }
        }

        return new ExpressionLexer(text, lexerOptions);
    }

    internal StringComparison GetStringComparison()
    {
        return EnableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
    }

    internal LexerOptions CreateLexerOption()
    {
        return new LexerOptions
        {
            IgnoreWhitespace = true,
            UseSemicolonDelimiter = false,
        };
    }
}