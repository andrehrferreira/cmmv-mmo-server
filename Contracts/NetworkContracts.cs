
[Contract("Ping", PacketType.Client)]
public struct PingDTO
{
    [ContractField("int")]
    public int Timestamp;
}

[Contract("Pong", PacketType.Server)]
public struct PongDTO
{
    [ContractField("int")]
    public int Timestamp;
}

[Contract("LoginToken", PacketType.Server)]
public struct LoginTokenDTO
{
    [ContractField("string")]
    public string Token;
}