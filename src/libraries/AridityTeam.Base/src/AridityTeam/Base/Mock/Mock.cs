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
using System.Linq.Expressions;
using System.Reflection;

namespace AridityTeam.Base.Mock;

public class Mock<T> : DispatchProxy where T : class
{
    private readonly Dictionary<string, List<object>> _returns = new();
    private readonly Dictionary<string, int> _callCounts = new();

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null) return null;
        var methodName = targetMethod.Name;

        // Record the call
        if (!_callCounts.TryAdd(methodName, 1))
        {
            _callCounts[methodName]++;
        }

        // Return the predefined value if set
        if (_returns.TryGetValue(methodName, out var returnValues) && returnValues.Count > 0)
        {
            return returnValues[0]; // Return the first value for simplicity
        }

        throw new Exception($"No return value set for method {methodName}.");
    }

    public void Setup(Expression<Func<T, object>> expression)
    {
        var methodName = GetMethodName(expression);
        if (!_returns.ContainsKey(methodName))
        {
            _returns[methodName] = [];
        }
    }

    public void Returns(Expression<Func<T, object>> expression, object returnValue)
    {
        var methodName = GetMethodName(expression);
        if (_returns.TryGetValue(methodName, out var @return))
        {
            @return.Add(returnValue);
        }
    }

    public void Verify(Expression<Func<T, object>> expression, int expectedCallCount)
    {
        var methodName = GetMethodName(expression);
        if (_callCounts.TryGetValue(methodName, out var actualCallCount))
        {
            if (actualCallCount != expectedCallCount)
            {
                throw new Exception($"Method {methodName} was called {actualCallCount} times, but expected {expectedCallCount}.");
            }
        }
        else
        {
            throw new Exception($"Method {methodName} was never called.");
        }
    }

    public static Mock<T> Create()
    {
        object proxy = Create<T, Mock<T>>();
        return (Mock<T>)proxy;
    }

    private string GetMethodName(Expression expression)
    {
        if (expression is LambdaExpression { Body: MethodCallExpression methodCall })
        {
            return methodCall.Method.Name;
        }
        throw new ArgumentException("Invalid expression");
    }
}