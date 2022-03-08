//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System.Diagnostics;

namespace Microsoft.OData.Query
{
    /// <summary>
    /// Exception type representing exceptions in the library.
    /// </summary>
    [DebuggerDisplay("{Message}")]
    public class OException : InvalidOperationException
    {
        /// <summary>Creates a new instance of the <see cref="OException" /> class with an error message.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        public OException(string message)
            : this(message, null)
        {
        }

        /// <summary>Creates a new instance of the <see cref="Microsoft.OData.ODataException" /> class with an error message and an inner exception.</summary>
        /// <param name="message">The plain text error message for this exception.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception to be thrown.</param>
        public OException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}