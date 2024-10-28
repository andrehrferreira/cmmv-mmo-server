
class NetworkGateway
{
    [Subscribe(ClientPacket.Ping)]
    public static void OnPingHandler(Connection conn, PingDTO data)
    {
        /*var pongData = PongPacket.Serialize(new PongDTO
        {
            Timestamp = data.Timestamp
        });

        conn.Send(ServerPacket.Pong, pongData);*/
    }
}