// This file was generated automatically, please do not change it.

public struct PingPacket
{
    public ClientPacket Type = ClientPacket.Ping;

    public PingPacket() { }

    public static ByteBuffer Serialize(Ping data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.Timestamp);
        return buffer;
    }

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
