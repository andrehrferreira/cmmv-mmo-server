public class Socket
{
    public string Id;

    public Entity Entity;

    public void Send(ByteBuffer data)
    {
        Send(data.GetBuffer());
    }

    public void Send(byte[] data)
    {

    }
}