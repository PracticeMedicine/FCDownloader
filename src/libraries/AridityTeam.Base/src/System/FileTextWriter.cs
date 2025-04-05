using System.IO;
using System.Text;

namespace System;

public class FileTextWriter : TextWriter
{
    private readonly string _filePath;
    private StreamWriter? _streamWriter;

    public FileTextWriter(string filePath, bool append = false, Encoding? encoding = null)
    {
        _filePath = filePath;
        _streamWriter = new StreamWriter(_filePath, append, encoding ?? Encoding.UTF8);
    }

    public override void Write(char value)
    {
        EnsureStreamWriter();
        _streamWriter?.Write(value);
    }

    public override void Write(string? value)
    {
        EnsureStreamWriter();
        _streamWriter?.Write(value);
    }

    public override void Flush()
    {
        EnsureStreamWriter();
        _streamWriter?.Flush();
    }

    public override void Close()
    {
        Dispose(true);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _streamWriter?.Close();
            _streamWriter = null;
        }
        base.Dispose(disposing);
    }

    public override Encoding Encoding => Encoding.UTF8;

    private void EnsureStreamWriter()
    {
        if (_streamWriter == null)
        {
            throw new ObjectDisposedException(nameof(FileTextWriter), "The writer has been disposed.");
        }
    }
}