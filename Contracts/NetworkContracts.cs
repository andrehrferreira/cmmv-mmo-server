
[Contract("Ping", PacketType.Client)]
public struct Ping
{
    [ContractField("int")]
    public int Timestamp;
}

[Contract("Pong", PacketType.Server)]
public struct Pong
{
    [ContractField("int")]
    public int Timestamp;
}