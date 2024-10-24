// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct TakeMissPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(TakeMissDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(Base36.ToInt(data.Id));
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TakeMissDTO Deserialize(ByteBuffer buffer)
    {
        var data = new TakeMissDTO();
        data.Id = buffer.ReadId();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, TakeMissDTO data)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.TakeMiss, buffer, true);
    }

}

