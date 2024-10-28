// This file was generated automatically, please do not change it.

using System.Runtime.CompilerServices;

public struct CharacterListPacket
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CharacterListDTO Deserialize(ByteBuffer buffer)
    {
        var data = new CharacterListDTO();
        return data;
    }
}

public partial class Server
{
    public static NetworkEvents<CharacterListDTO> OnCharacterList = new NetworkEvents<CharacterListDTO>();
}
