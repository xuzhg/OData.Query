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
    }
}
