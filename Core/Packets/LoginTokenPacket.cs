// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct LoginTokenPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ByteBuffer Serialize(LoginTokenDTO data)
    {
        var buffer = ByteBuffer.CreateEmptyBuffer();
        buffer.Write(data.Token);
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LoginTokenDTO Deserialize(ByteBuffer buffer)
    {
        var data = new LoginTokenDTO();
        data.Token = buffer.ReadString();
        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Send(Entity owner, LoginTokenDTO data)
    {
        var buffer = Serialize(data);
        owner.Conn.Send(ServerPacket.LoginToken, buffer, false);
    }

}

