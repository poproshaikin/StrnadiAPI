using System.Net.WebSockets;

namespace StrnadiAPI.Services;

public interface IStreamHelper
{
    Task<byte[]> ReadBytesAsync(int count);
    Task<int> ReadIntAsync();
}

public class StreamHelper : IStreamHelper
{
    public Stream Stream { get; }

    public StreamHelper(Stream stream)
    {
        Stream = stream;
    }

    public async Task<byte[]> ReadBytesAsync(int count)
    {
        byte[] buffer = new byte[count];
        _ = await Stream.ReadAsync(buffer);
        return buffer;
    }

    public async Task<int> ReadIntAsync()
    {
        return BitConverter.ToInt32(await ReadBytesAsync(sizeof(int)));
    }
}