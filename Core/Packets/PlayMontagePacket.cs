// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct PlayMontagePacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(PlayMontageDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(Base36.ToInt(data.Id));
        buffer.Write(data.Index);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PlayMontageDTO Deserialize(ByteBuffer buffer)
    {
        var data = new PlayMontageDTO();
        data.Id = buffer.ReadId();
        data.Index = buffer.ReadInt();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, PlayMontageDTO data, Entity entity)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.PlayMontage, buffer, true, true);
    }

}

public partial class Server
{
    public static NetworkEvents<PlayMontageDTO> OnPlayMontage = new NetworkEvents<PlayMontageDTO>();

    [Subscribe(ClientPacket.PlayMontage)]
    public static void OnPlayMontageHandler(PlayMontageDTO data, Connection conn)
    {
        var packet = PlayMontagePacket.Serialize(data);
        conn.Entity.Reply(ServerPacket.PlayMontage, packet, true);
    }
}
