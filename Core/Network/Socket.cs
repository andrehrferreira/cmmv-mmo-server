public class Socket
{
    public string Id;

    public Entity Entity;

    public void Send(ServerPacket packetType, ByteBuffer data)
    {
        Send(packetType, data.GetBuffer());
    }

    public void Send(ServerPacket packetType, byte[] data)
    {

    }

    public void Send(byte[] data)
    {

    }
}