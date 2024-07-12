//-----------------------------------------------------------------------
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// See License.txt in the project root for license information.
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.OData.Query.Tests.Commons;

internal class ExceptionAssert
{
    /// <summary>
    /// Verifies that an exception of the given type (or optionally a derived type) is thrown.
    /// </summary>
    /// <typeparam name="TException">The type of the exception expected to be thrown</typeparam>
    /// <param name="testCode">A delegate to the code to be tested</param>
    /// <param name="allowDerivedExceptions">Pass true to allow exceptions which derive from TException; pass false, otherwise</param>
    /// <returns>The exception that was thrown, when successful</returns>
    /// <exception cref="ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
    public static TException Throws<TException>(Action testCode, bool allowDerivedExceptions)
        where TException : Exception
    {
        Type exceptionType = typeof(TException);
        Exception exception = RecordException(testCode);

        TargetInvocationException tie = exception as TargetInvocationException;
        if (tie != null)
        {
            exception = tie.InnerException;
        }

        if (exception == null)
        {
            throw new ThrowsException(exceptionType);
        }

        var typedException = exception as TException;
        if (typedException == null || (!allowDerivedExceptions && typedException.GetType() != typeof(TException)))
        {
            throw new ThrowsException(exceptionType, exception);
        }

        return typedException;
    }

    /// <summary>
    /// Verifies that the code throws an ArgumentNullException (or optionally any exception which derives from it).
    /// </summary>
    /// <param name="testCode">A delegate to the code to be tested</param>
    /// <param name="paramName">The name of the parameter that should throw the exception</param>
    /// <returns>The exception that was thrown, when successful</returns>
    /// <exception cref="ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
    public static ArgumentNullException ThrowsArgumentNull(Action testCode, string paramName)
    {
        var ex = Throws<ArgumentNullException>(testCode, allowDerivedExceptions: false);

        if (paramName != null)
        {
            Assert.Equal(paramName, ex.ParamName);
        }

        return ex;
    }

    /// <summary>
    /// Verifies that an exception of the given type is thrown.
    /// </summary>
    /// <typeparam name="TException">The type of the exception expected to be thrown</typeparam>
    /// <param name="testCode">A delegate to the code to be tested</param>
    /// <returns>The exception that was thrown, when successful</returns>
    /// <exception cref="ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
    /// <remarks>
    /// Unlike other Throws* methods, this method does not enforce running the exception delegate with a known Thread Culture.
    /// </remarks>
    public static async Task<TException> ThrowsAsync<TException>(Func<Task> testCode, bool allowDerivedExceptions = false)
        where TException : Exception
    {
        Type exceptionType = typeof(TException);
        Exception exception = null;
        try
        {
            // The 'testCode' Task might execute asynchronously in a different thread making it hard to enforce the thread culture.
            // The correct way to verify exception messages in such a scenario would be to run the task synchronously inside of a 
            // culture enforced block.
            await testCode();
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        if (exception == null)
        {
            throw new ThrowsException(exceptionType);
        }

        var typedException = exception as TException;
        if (typedException == null || (!allowDerivedExceptions && typedException.GetType() != typeof(TException)))
        {
            throw new ThrowsException(exceptionType, exception);
        }

        return (TException)exception;
    }

    /// <summary>
    /// Verifies that an exception of the given type (or optionally a derived type) is thrown.
    /// Also verifies that the exception message matches.
    /// </summary>
    /// <typeparam name="TException">The type of the exception expected to be thrown</typeparam>
    /// <param name="testCode">A delegate to the code to be tested</param>
    /// <param name="exceptionMessage">The exception message to verify</param>
    /// <param name="allowDerivedExceptions">Pass true to allow exceptions which derive from TException; pass false, otherwise</param>
    /// <returns>The exception that was thrown, when successful</returns>
    /// <exception cref="ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
    public static async Task<TException> ThrowsAsync<TException>(Func<Task> testCode, string exceptionMessage, bool allowDerivedExceptions = false, bool partialMatch = false)
        where TException : Exception
    {
        var ex = await ThrowsAsync<TException>(testCode, allowDerivedExceptions);
        VerifyExceptionMessage(ex, exceptionMessage, partialMatch);
        return ex;
    }

    /// <summary>
    /// Verifies that the code throws an ArgumentNullException (or optionally any exception which derives from it).
    /// </summary>
    /// <param name="testCode">A delegate to the code to be tested</param>
    /// <param name="paramName">The name of the parameter that should throw the exception</param>
    /// <returns>The exception that was thrown, when successful</returns>
    /// <exception cref="ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
    public static async Task<ArgumentNullException> ThrowsArgumentNullAsync(Func<Task> testCode, string paramName)
    {
        var ex = await ThrowsAsync<ArgumentNullException>(testCode);

        if (paramName != null)
        {
            Assert.Equal(paramName, ex.ParamName);
        }

        return ex;
    }

    // We've re-implemented all the xUnit.net Throws code so that we can get this 
    // updated implementation of RecordException which silently unwraps any instances
    // of AggregateException. In addition to unwrapping exceptions, this method ensures 
    // that tests are executed in with a known set of Culture and UICulture. This prevents
    // tests from failing when executed on a non-English machine. 
    public static Exception RecordException(Action testCode)
    {
        try
        {
            testCode();
            return null;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }

    private static void VerifyExceptionMessage(Exception exception, string expectedMessage, bool partialMatch = false)
    {
        if (expectedMessage != null)
        {
            if (!partialMatch)
            {
                Assert.Equal(expectedMessage, exception.Message);
            }
            else
            {
                Assert.Contains(expectedMessage, exception.Message);
            }
        }
    }
}
