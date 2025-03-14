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
using System.IO;
using System.Threading.Tasks;

namespace AridityTeam.Base.Tests.ResourceMgrTest
{
    public class ResourceManagerTests
    {
        [Fact]
        public void PrecacheTest()
        {
            ResourceManager resourceMgr = new ResourceManager();
            
            Task.Run(async () =>
            {
                await resourceMgr.LoadResourceAsync(Path.Combine("./Images/bf_logo.png"), true);
            });
        }
    }
}
