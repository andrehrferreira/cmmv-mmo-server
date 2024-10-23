// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct PingPacket
{
    public ClientPacket Type = ClientPacket.Ping;

    public PingPacket() { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(Ping data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.Timestamp);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ping Deserialize(ByteBuffer buffer)
    {
        var data = new Ping();
        data.Timestamp = buffer.ReadInt();
        return data;
    }
}

public partial class Server
{
    public static NetworkEvents<Ping> OnPing = new NetworkEvents<Ping>();
}
