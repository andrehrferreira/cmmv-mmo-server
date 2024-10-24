// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct UpdateEntityPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(UpdateEntity data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.Id);
        buffer.Write(data.States);
        buffer.Write(data.Buffs);
        buffer.Write(data.Position);
        buffer.Write(data.Life);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UpdateEntity Deserialize(ByteBuffer buffer)
    {
        var data = new UpdateEntity();
        data.Id = buffer.ReadInt();
        data.States = buffer.ReadInt();
        data.Buffs = buffer.ReadInt();
        data.Position = buffer.ReadVector3();
        data.Life = buffer.ReadInt();
        return data;
    }
}

