[Contract("SyncByte", PacketType.Server, PacketAction.None, true, PacketSendType.Self, true)]
public struct SyncByte
{
    [ContractField("byte")]
    public byte Model;

    [ContractField("byte")]
    public byte Index;

    [ContractField("byte")]
    public byte Value;
}

[Contract("SyncInt", PacketType.Server, PacketAction.None, true, PacketSendType.Self, true)]
public struct SyncInt
{
    [ContractField("byte")]
    public byte Model;

    [ContractField("byte")]
    public byte Index;

    [ContractField("int")]
    public int Value;
}

[Contract("SyncString", PacketType.Server, PacketAction.None, true, PacketSendType.Self, true)]
public struct SyncString
{
    [ContractField("byte")]
    public byte Model;

    [ContractField("byte")]
    public byte Index;

    [ContractField("string")]
    public string Value;
}