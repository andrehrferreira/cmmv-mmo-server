// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct RemoveEntityPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(RemoveEntityDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(Base36.ToInt(data.Id));
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RemoveEntityDTO Deserialize(ByteBuffer buffer)
    {
        var data = new RemoveEntityDTO();
        data.Id = buffer.ReadId();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, RemoveEntityDTO data, Entity entity)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.RemoveEntity, buffer, true, true);
    }

}

