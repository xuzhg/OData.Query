using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Query.Parser
{
    public enum ExpressionTokenKind
    {
        /// <summary>Unknown.</summary>
        Unknown = 0,

        /// <summary>Customized.</summary>
        Customized,

        /// <summary>End of text.</summary>
        End,

        /// <summary>Identifier.</summary>
        Identifier,

        /// <summary>End of text.</summary>
        TextEnd,

        /// <summary>Whitespace.</summary>
        Whitespace,

        /// <summary>Literal, could be an identifier, a number, null, guid, etc.</summary>
        Literal,

        /// <summary>NullLiteral.</summary>
        NullLiteral,

        /// <summary>BooleanLiteral.</summary>
        BooleanLiteral,

        /// <summary>StringLiteral.</summary>
        StringLiteral,

        /// <summary>IntegerLiteral.</summary>
        IntegerLiteral,

        /// <summary>Int64 literal.</summary>
        Int64Literal,

        /// <summary>Infinity Literal, -INF.</summary>
        NegativeInfinityLiteral,

        /// <summary>Infinity Literal, INF.</summary>
        PositiveInfinityLiteral,

        /// <summary>Single literal.</summary>
        SingleLiteral,

        /// <summary>Double literal.</summary>
        DoubleLiteral,

        /// <summary>Duration literal.</summary>
        DurationLiteral,

        /// <summary>Decimal literal.</summary>
        DecimalLiteral,

        /// <summary>GUID literal.</summary>
        GuidLiteral,

        /// <summary>Binary literal.</summary>
        BinaryLiteral,

        /// <summary>DateTimeOffset literal.</summary>
        DateTimeOffsetLiteral,

        /// <summary>DateOnly literal.</summary>
        DateOnlyLiteral,

        /// <summary>TimeOnly literal.</summary>
        TimeOnlyLiteral,

        /// <summary>Comma ','</summary>
        Comma,

        /// <summary>Colon.</summary>
        Colon,

        /// <summary>Equal '='</summary>
        Equal,

        /// <summary>OpenParen '('.</summary>
        OpenParen,

        /// <summary>CloseParen ')'.</summary>
        CloseParen,

        /// <summary>Minus.</summary>
        Minus,

        /// <summary>Slash.</summary>
        Slash,

        /// <summary>Question.</summary>
        Question,

        /// <summary>Dot.</summary>
        Dot,

        /// <summary>Star.</summary>
        Star,

        /// <summary>SemiColon</summary>
        SemiColon,

        /// <summary>ParameterAlias</summary>
        ParameterAlias,

        /// <summary>A BracedExpression is an expression within braces {}. It contains a JSON object.</summary>
        BracedExpression,

        /// <summary>A BracketedExpression is an expression within brackets []. It contains a JSON array.</summary>
        BracketedExpression,

        /// <summary>A ParenthesesExpression is an expression within parentheses (). It contains a list of objects.</summary>
        ParenthesesExpression,
    }
}
