using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Query.Parser
{
    internal static class ParserHelper
    {
        /// <summary>Determines whether the specified character is a valid hexadecimal digit.</summary>
        /// <param name="c">Character to check.</param>
        /// <returns>true if <paramref name="c"/> is a valid hex digit; false otherwise.</returns>
        public static bool IsCharHexDigit(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }
    }
}
