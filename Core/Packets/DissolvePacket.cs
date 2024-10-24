// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct DissolvePacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(DissolveDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(Base36.ToInt(data.Id));
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DissolveDTO Deserialize(ByteBuffer buffer)
    {
        var data = new DissolveDTO();
        data.Id = buffer.ReadId();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, DissolveDTO data)
    {
        var buffer = Serialize(data);
        owner.Reply(ServerPacket.Dissolve, buffer, true);
    }

}

