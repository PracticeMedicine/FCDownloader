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
 */
using System;
using System.Collections.Generic;

namespace System.Collections.Concurrent
{
    /// <summary>
    /// An observable <see cref="ConcurrentBag{T}"/>. 
    /// (Inherited from <see cref="RemovableConcurrentBag{T}"/>) <para/>
    /// An alternative to <see cref="ObservableList{T}"/> to save memory performance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableConcurrentBag<T> : RemovableConcurrentBag<T?>
    {
        public event Action<T?>? ItemAdded;
        public event Action<T?>? ItemRemoved;

        public ObservableConcurrentBag() : base() {}

        /// <summary>
        /// Adds an object to the <see cref="ObservableConcurrentBag{T}"/>.
        /// </summary>
        /// <param name="item">The object to be added to the 
        /// <see cref="ObservableConcurrentBag{T}"/>. The value can be a null reference
        /// (Nothing in Visual Basic) for reference types.</param>
        public new void Add(T? item)
        {
            base.Add(item);
            ItemAdded?.Invoke(item);
        }

        /// <summary>
        /// Removes an object from the <see cref="ObservableConcurrentBag{T}"/>. <para/>
        /// </summary>
        /// <param name="item">The object to be removed from the 
        /// <see cref="ObservableConcurrentBag{T}"/></param>
        public new bool Remove(T? item)
        {
            ItemRemoved?.Invoke(item);
            return base.Remove(item);
        }
    }
}
