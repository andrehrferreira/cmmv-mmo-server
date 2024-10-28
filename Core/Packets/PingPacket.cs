// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct PingPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PingDTO Deserialize(ByteBuffer buffer)
    {
        var data = new PingDTO();
        data.Timestamp = buffer.ReadInt();
        return data;
    }
}

public partial class Server
{
    public static NetworkEvents<PingDTO> OnPing = new NetworkEvents<PingDTO>();
}
