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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// Sends output on multiple <see cref="TextWriter"/>. <para/>
    /// https://stackoverflow.com/questions/18726852/redirecting-console-writeline-to-textbox
    /// </summary>
    public class MultiTextWriter : TextWriter
    {
        private readonly IEnumerable<TextWriter> _writers;
        public MultiTextWriter(IEnumerable<TextWriter> writers)
        {
            this._writers = writers.ToList();
        }
        public MultiTextWriter(params TextWriter[] writers)
        {
            this._writers = writers;
        }

        public void AddWriter(TextWriter w)
        {
            _writers.Append(w);
        }

        public override void Write(char value)
        {
            foreach (var writer in _writers)
                writer.Write(value);
        }

        public override void Write(string? value)
        {
            foreach (var writer in _writers)
                writer.Write(value);
        }

        public override void Flush()
        {
            foreach (var writer in _writers)
                writer.Flush();
        }

        public override void Close()
        {
            foreach (var writer in _writers)
                writer.Close();
        }

        public override Encoding Encoding => Encoding.ASCII;
    }
}
