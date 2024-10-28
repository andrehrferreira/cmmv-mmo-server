// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct TakeDamagePacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(TakeDamageDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write((byte)ServerPacket.TakeDamage);
        buffer.Write(Base36.ToInt(data.Id));
        buffer.Write(Base36.ToInt(data.CauserId));
        buffer.Write(data.Damage);
        buffer.Write(data.DamageType);
        return buffer;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, TakeDamageDTO data, Entity entity)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.TakeDamage, buffer, true, true);
    }
}

