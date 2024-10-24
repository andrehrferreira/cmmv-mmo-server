
class NetworkGateway
{
    //[Subscribe(ClientPacket.Ping)]
    public static void OnPingHandler(PingDTO data, Socket socket)
    {
        /*var pongData = PongPacket.Serialize(new Pong
        {
            Timestamp = data.Timestamp
        });*/

       
    }
}