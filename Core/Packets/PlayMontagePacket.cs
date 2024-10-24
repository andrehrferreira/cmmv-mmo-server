// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct PlayMontagePacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(PlayMontage data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.Id);
        buffer.Write(data.Index);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PlayMontage Deserialize(ByteBuffer buffer)
    {
        var data = new PlayMontage();
        data.Id = buffer.ReadInt();
        data.Index = buffer.ReadInt();
        return data;
    }
}

public partial class Server
{
    public static NetworkEvents<PlayMontage> OnPlayMontage = new NetworkEvents<PlayMontage>();

    [Subscribe(ClientPacket.PlayMontage)]
    public static void OnPlayMontageHandler(PlayMontage data, Socket socket)
    {
        var packet = PlayMontagePacket.Serialize(data);
        socket.Entity.Reply(ServerPacket.PlayMontage, packet, true);
    }
}
