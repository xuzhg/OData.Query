//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

namespace Microsoft.OData.Query.Parser
{
    public struct ExpressionToken
    {
        /// <summary>InternalKind of token.</summary>
        public ExpressionTokenKind Kind;

        /// <summary>Token text.</summary>
        public string Text;

        /// <summary>Position of token.</summary>
        public int Position;

        /// <summary>
        /// Checks that this token has the specified identifier.
        /// </summary>
        /// <param name="id">Identifier to check.</param>
        /// <param name="enableCaseInsensitive">whether to allow case insensitive.</param>
        /// <returns>true if this is an identifier with the specified text.</returns>
        internal bool IdentifierIs(string id, bool enableCaseInsensitive)
        {
            return Kind == ExpressionTokenKind.Identifier
                && string.Equals(Text, id, enableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }
    }
}
