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

namespace System.Collections.Concurrent;

/// <summary>
/// A <see cref="ConcurrentBag{T}"/> that can remove items.
/// </summary>
/// <typeparam name="T"></typeparam>
public class RemovableConcurrentBag<T> : ConcurrentBag<T?>
{
    private readonly Threading.Lock _lock = new();

    public RemovableConcurrentBag() : base()
    {
    }

    protected RemovableConcurrentBag(IEnumerable<T?> collection) : base(collection)
    {
    }

    /// <summary>
    /// Get all the items in the bag.
    /// </summary>
    /// <returns>Returns an array of all added items.</returns>
    public IEnumerable<T?> GetAll()
    {
        lock (_lock)
        {
            return ToArray(); // Return a copy of the items in the bag
        }
    }

    /// <summary>
    /// Removes an object from the <see cref="RemovableConcurrentBag{T}"/>. <para/>
    /// </summary>
    /// <param name="item">The object to be removed from the 
    /// <see cref="RemovableConcurrentBag{T}"/></param>
    public bool Remove(T? item)
    {
        var tempList = new List<T>();
        var found = false;

        while (TryTake(out var result))
        {
            if (result == null)
                return false;

            if (!result.Equals(item))
                tempList.Add(result); // Collect items that are not being removed
            else
                found = true; // Item found and removed
        }

        // Re-add the items that were not removed
        foreach (var tempItem in tempList)
            Add(tempItem);

        return found;
    }
}

