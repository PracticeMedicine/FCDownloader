/*
 * Copyright (c) 2025 The Aridity Team
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using AridityTeam.Base.Internal;
using AridityTeam.Base.Util;

namespace AridityTeam.Base;

/// <summary>
/// Assert class.
/// </summary>
public static class Assert
{
    /// <summary>
    /// Checks if the expected type is equal to the actual.
    /// </summary>
    /// <param name="expected">Expected type</param>
    /// <param name="actual">Actual type</param>
    /// <param name="message">Assertion message to throw when the assert trying to execute/compare failed</param>
    /// <typeparam name="T">Any type bro.</typeparam>
    /// <exception cref="AssertionFailedException">Throws when the expected type is not equal to the actual type.</exception>
    public static void AreEqual<T>(T expected, T actual, string? message = null)
    {
        if (!Equals(expected, actual))
        {
            throw new AssertionFailedException(message ?? $"Expected: {expected}, but was: {actual}");
        }
    }
    
    /// <summary>
    /// Same as the AreEqual function, but it's the complete opposite.
    /// </summary>
    /// <param name="expected">Expected type</param>
    /// <param name="actual">Actual type</param>
    /// <param name="message">Assertion message to throw when the assert trying to execute/compare failed</param>
    /// <typeparam name="T">Any type bro.</typeparam>
    /// <exception cref="AssertionFailedException">Throws when of course, the expected type is actually EQUAL to the actual type.</exception>
    public static void AreNotEqual<T>(T expected, T actual, string? message = null)
    {
        if (Equals(expected, actual))
        {
            throw new AssertionFailedException(message ?? $"Assertion failed: Expected: {expected} not equal to {actual}");
        }
    }

    /// <summary>
    /// Checks if the string contains the... You've already got what it means.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="actual"></param>
    /// <param name="message">Assertion message to throw when the assert trying to execute/compare failed</param>
    /// <exception cref="AssertionFailedException"></exception>
    public static void Contains(string expected, string actual, string? message = null)
    {
        if (!actual.Contains(expected)) 
            throw new AssertionFailedException(message ?? $"Assertion failed: expected for {expected} to be inside of the actual string");
    }
    
    /// <summary>
    /// The opposite of the Contains function.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="actual"></param>
    /// <param name="message">Assertion message to throw when the assert trying to execute/compare failed</param>
    /// <exception cref="AssertionFailedException"></exception>
    public static void DoesNotContain(string expected, string actual, string? message = null)
    {
        if (actual.Contains(expected)) 
            throw new AssertionFailedException(message ?? $"Assertion failed: expected for {expected} to be inside of the actual string");
    }
    
    /// <summary>
    /// Checks if the enumerable type contains the... Again, you already know what this means.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="collection"></param>
    /// <param name="message">Assertion message to throw when the assert trying to execute/compare failed</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="AssertionFailedException"></exception>
    public static void Contains<T>(T? expected, IEnumerable<T?> collection, string? message = null)
    {
        if (!collection.Contains(expected)) 
            throw new AssertionFailedException(message ?? $"Assertion failed: expected for {expected} to be inside of the collection");
    }
    
    /// <summary>
    /// Opposite of the Contains function.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="collection"></param>
    /// <param name="message">Assertion message to throw when the assert trying to execute/compare failed</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="AssertionFailedException"></exception>
    public static void DoesNotContain<T>(T expected, IEnumerable<T> collection, string? message = null)
    {
        if (collection.Contains(expected)) 
            throw new AssertionFailedException(message ?? $"Assertion failed: expected value ({expected}) is currently inside of the collection");
    }

    /// <summary>
    /// Checks if an action throws the expected exception.
    /// </summary>
    /// <param name="action">Action to test</param>
    /// <param name="message">Assertion message to throw when the assert trying to execute/compare failed</param>
    /// <typeparam name="TException"></typeparam>
    /// <exception cref="AssertionFailedException">Throws when the excepted exception isn't thrown</exception>
    public static void DoesThrow<TException>(Action action, string? message = null)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            // An unexpected exception was thrown
            if (ex.GetType() == typeof(TException)) return;
            throw new AssertionFailedException(message ??
                                               $"Assertion failed: expected exception of type {typeof(TException).Name}, but got {ex.GetType().Name}: {ex.Message}");
        }

        // If no exception was thrown, fail the assertion
        throw new AssertionFailedException(message ??
                                           $"Assertion failed: expected exception of type {typeof(TException).Name} was not thrown.");
    }

    public static void IsTrue(bool condition, string? message = null)
    {
        if (!condition)
        {
            throw new AssertionFailedException(message ?? "Assertion failed: expected true, but was false.");
        }
    }

    public static void IsFalse(bool condition, string? message = null)
    {
        if (condition)
        {
            throw new AssertionFailedException(message ?? "Assertion failed: expected false, but was true.");
        }
    }

    public static void IsNull(object? obj, string? message = null)
    {
        if (obj != null)
        {
            throw new AssertionFailedException(message ?? "Assertion failed: expected null, but was not null.");
        }
    }

    public static void IsNotNull(object? obj, string? message = null)
    {
        if (obj == null)
        {
            throw new AssertionFailedException(message ?? "Assertion failed: expected not null, but was null.");
        }
    }

    public static void IfResultSuccess<T>(Func<Result<T>> expected, string? message = null)
    {
        var res = expected.Invoke();
        if (!res.IsSuccess) throw new AssertionFailedException(res.IsSuccess, true, false);
    }
    
    public static void IfResultFailure<T>(Func<Result<T>> expected, string? message = null)
    {
        var res = expected.Invoke();
        if (res.IsSuccess) throw new AssertionFailedException(res.IsSuccess, false, true);
    }
}