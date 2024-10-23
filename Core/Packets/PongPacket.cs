// This file was generated automatically, please do not change it.

public struct PongPacket
{
    public ServerPacket Type = ServerPacket.Pong;

    public PongPacket() { }

    public static ByteBuffer Serialize(Pong data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.Timestamp);
        return buffer;
    }

    public static Pong Deserialize(ByteBuffer buffer)
    {
        var data = new Pong();
        data.Timestamp = buffer.ReadInt();
        return data;
    }
}

