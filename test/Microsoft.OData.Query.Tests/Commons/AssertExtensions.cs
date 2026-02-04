//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Query.Tests.Commons;

/// <summary>
/// Extension methods for assert.
/// </summary>
internal static class AssertExtensions
{
    /// <summary>
    /// Verifies that the test code does not throw.
    /// </summary>
    /// <param name="testCode">A delegate to the code to be tested</param>
    public static void DoesNotThrow(this Action testCode)
    {
        Exception ex = Record.Exception(testCode);
        Assert.Null(ex);
    }

    /// <summary>
    /// Verifies that the exact exception calling the test code action is thrown.
    /// </summary>
    /// <typeparam name="T">The type of the exception expected to be thrown</typeparam>
    /// <param name="testCode">A delegate to the code to be tested</param>
    /// <param name="errorMessage">The expected error message.</param>
    public static void Throws<T>(this Action testCode, string errorMessage) where T : Exception
    {
        T exception = Assert.Throws<T>(testCode);
        Assert.Equal(errorMessage, exception.Message);
    }

    /// <summary>
    /// Verifies that the exact exception calling the test code action is thrown.
    /// </summary>
    /// <typeparam name="T">The type of the exception expected to be thrown</typeparam>
    /// <param name="testCode">A delegate to the code to be tested</param>
    /// <param name="errorMessage">The expected error message.</param>
    public static async ValueTask ThrowsAsync<T>(this Func<Task> testCode, string errorMessage) where T : Exception
    {
        Exception exception = await Record.ExceptionAsync(testCode);

        Assert.NotNull(exception);
        T typedException = Assert.IsType<T>(exception);

       // T exception = await Assert.ThrowsAsync<T>(testCode);
        Assert.Equal(errorMessage, typedException.Message);
    }
}
