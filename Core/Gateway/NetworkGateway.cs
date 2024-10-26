
class NetworkGateway
{
    [Subscribe(ClientPacket.Ping)]
    public static void OnPingHandler(PingDTO data, Connection conn)
    {
        var pongData = PongPacket.Serialize(new PongDTO
        {
            Timestamp = data.Timestamp
        });

        conn.Send(ServerPacket.Pong, pongData);
    }
}