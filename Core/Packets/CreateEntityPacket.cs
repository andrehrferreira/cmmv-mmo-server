// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct CreateEntityPacket
{
    public ServerPacket Type = ServerPacket.CreateEntity;

    public CreateEntityPacket() { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(CreateEntity data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.Id);
        buffer.Write(data.Name);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CreateEntity Deserialize(ByteBuffer buffer)
    {
        var data = new CreateEntity();
        data.Id = buffer.ReadInt();
        data.Name = buffer.ReadString();
        return data;
    }
}

