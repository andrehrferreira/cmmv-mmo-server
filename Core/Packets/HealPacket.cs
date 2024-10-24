// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct HealPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(HealDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(Base36.ToInt(data.Id));
        buffer.Write(data.Type);
        buffer.Write(data.Value);
        buffer.Write(Base36.ToInt(data.CasterId));
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HealDTO Deserialize(ByteBuffer buffer)
    {
        var data = new HealDTO();
        data.Id = buffer.ReadId();
        data.Type = buffer.ReadByte();
        data.Value = buffer.ReadInt();
        data.CasterId = buffer.ReadId();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, HealDTO data)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.Heal, buffer, true);
    }

}

