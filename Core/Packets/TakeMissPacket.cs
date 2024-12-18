// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct TakeMissPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(TakeMissDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write((byte)ServerPacket.TakeMiss);
        buffer.Write(Base36.ToInt(data.Id));
        return buffer;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, TakeMissDTO data, Entity entity)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.TakeMiss, buffer, true, true);
    }
}

