// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct DissolvePacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(DissolveDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write((byte)ServerPacket.Dissolve);
        buffer.Write(Base36.ToInt(data.Id));
        return buffer;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, DissolveDTO data, Entity entity)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.Dissolve, buffer, true, true);
    }
}

