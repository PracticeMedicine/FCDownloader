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

namespace AridityTeam.Base.Util;

public class CommandLineParser(string[] args)
{
    public bool FindParm(string parm)
    {
        foreach (var arg in args)
            return arg.Equals(parm);
        return false;
    }
    public int GetParm(string parm)
    {
        foreach (var arg in args)
            return arg.IndexOf(parm, StringComparison.OrdinalIgnoreCase);
        return -1;
    }

    public object? GetParmValue(int index)
    {
        if (index < 0 || index >= args.Length) return null;
        var input = args[index].Substring(0).Trim();
        string[] parts = input.Split(' ', '=', ':');
        return parts[0].Trim();
    }
}