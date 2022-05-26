namespace SilentOrbit.ProtocolBuffers;

/// <summary>
/// Wrapper for streams that does not support the Position property.
/// Adds support for the Position property.
/// </summary>
public class PositionStream : Stream
{
    readonly Stream stream;

    /// <summary>
    /// Bytes read in the stream starting from the beginning.
    /// </summary>
    public int BytesRead { get; private set; }

    /// <summary>
    /// Define how many bytes are allowed to read
    /// </summary>
    /// <param name='baseStream'>
    /// Base stream.
    /// </param>
    /// <param name='maxLength'>
    /// Max length allowed to read from the stream.
    /// </param>
    public PositionStream(Stream baseStream)
    {
        this.stream = baseStream;
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int read = stream.Read(buffer, offset, count);
        BytesRead += read;
        return read;
    }

    public override int ReadByte()
    {
        int b = stream.ReadByte();
        BytesRead += 1;
        return b;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        if (stream.CanSeek)
            return stream.Seek(offset, origin);

        if (origin == SeekOrigin.Current && offset >= 0)
        {
            var buffer = new byte[Math.Min(offset, 10000)];
            long end = BytesRead + offset;
            while (BytesRead < end)
            {
                int read = stream.Read(buffer, 0, (int)Math.Min(end - BytesRead, buffer.Length));
                if (read == 0)
                    break;
                BytesRead += read;
            }
            return BytesRead;
        }

        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override bool CanRead
    {
        get
        {
            return true;
        }
    }

    public override bool CanSeek
    {
        get
        {
            return false;
        }
    }

    public override bool CanWrite
    {
        get
        {
            return false;
        }
    }

    public override long Length
    {
        get
        {
            return stream.Length;
        }
    }

    public override long Position
    {
        get
        {
            return this.BytesRead;
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    protected override void Dispose(bool disposing)
    {
        stream.Dispose();
        base.Dispose(disposing);
    }
}
