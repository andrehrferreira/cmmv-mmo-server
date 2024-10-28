// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct HealPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(HealDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write((byte)ServerPacket.Heal);
        buffer.Write(Base36.ToInt(data.Id));
        buffer.Write(data.Type);
        buffer.Write(data.Value);
        buffer.Write(Base36.ToInt(data.CasterId));
        return buffer;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, HealDTO data, Entity entity)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.Heal, buffer, true, true);
    }
}

