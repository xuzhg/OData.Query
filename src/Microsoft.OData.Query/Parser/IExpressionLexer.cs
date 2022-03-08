using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Query.Parser
{
    public interface IExpressionLexer
    {
        /// <summary>
        /// Token being processed.
        /// </summary>
        ExpressionToken Token { get; }

        /// <summary>
        /// Reads the next token, skipping whitespace as necessary.
        /// Advancing the lexer.
        /// </summary>
        /// <returns></returns>
        bool Next();
    }
}
