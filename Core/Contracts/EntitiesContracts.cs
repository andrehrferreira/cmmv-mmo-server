
[Contract("CreateEntity", PacketType.Server)]
public struct CreateEntity
{
    [ContractField("int")]
    public int Id;

    [ContractField("string")]
    public string Name;
}