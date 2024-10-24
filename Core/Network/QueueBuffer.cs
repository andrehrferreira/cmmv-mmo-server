public class QueueBuffer
{
    public static Dictionary<string, List<ByteBuffer>> Queues = new Dictionary<string, List<ByteBuffer>>();
    public static Dictionary<string, Socket> Sockets = new Dictionary<string, Socket>();
    public static int MaxBufferSize = 512 * 1024;
    public static byte EndOfPacketByte = 0xFE;
    public static int EndRepeatByte = 4;

    public static void AddSocket(string id, Socket socket)
    {
        Sockets[id] = socket;
    }

    public static void RemoveSocket(string id)
    {
        if (Sockets.ContainsKey(id))
            Sockets.Remove(id);
    }

    public static Socket? GetSocket(string id)
    {
        return Sockets.ContainsKey(id) ? Sockets[id] : null;
    }

    public static void AddBuffer(string socketId, ByteBuffer buffer)
    {
        if (!Queues.ContainsKey(socketId))
            Queues[socketId] = new List<ByteBuffer>();

        if (!IsDuplicatePacket(socketId, buffer))
        {
            Queues[socketId].Add(buffer);
            CheckAndSend(socketId);
        }
    }

    public static void CheckAndSend(string socketId)
    {
        if (Queues.ContainsKey(socketId))
        {
            var buffers = Queues[socketId];
            int totalSize = buffers.Sum(buffer => buffer.GetBuffer().Length);

            if (totalSize >= MaxBufferSize)
            {
                SendBuffers(socketId);
            }                
        }
    }

    public static void SendBuffers(string socketId)
    {
        if (Queues.ContainsKey(socketId))
        {
            var buffers = Queues[socketId];
            if (buffers.Count > 1)
            {
                var combinedBuffer = CombineBuffers(buffers);
                var finalBuffer = combinedBuffer.GetBuffer();
                GetSocket(socketId)?.Send(finalBuffer); // Replace `Send` with actual socket send method
                Queues[socketId].Clear();
            }
            else
            {
                GetSocket(socketId)?.Send(buffers[0].GetBuffer()); // Replace `Send` with actual socket send method
                Queues[socketId].Clear();
            }
        }
    }

    public static ByteBuffer CombineBuffers(List<ByteBuffer> buffers)
    {
        int totalLength = buffers.Sum(buffer => buffer.GetBuffer().Length) + (buffers.Count * EndRepeatByte) + 1;
        byte[] combinedArray = new byte[totalLength];
        int position = 0;

        combinedArray[position++] = (byte)ServerPacket.Queue;

        foreach (var buffer in buffers)
        {
            var buf = buffer.GetBuffer();
            Array.Copy(buf, 0, combinedArray, position, buf.Length);
            position += buf.Length;

            for (int i = 0; i < EndRepeatByte; i++)            
                combinedArray[position++] = EndOfPacketByte;            
        }

        if (position != totalLength)        
            throw new InvalidOperationException("Combined buffer size does not match the expected size.");

        return new ByteBuffer(combinedArray);
    }


    public static bool IsDuplicatePacket(string socketId, ByteBuffer buffer)
    {
        if (!Queues.ContainsKey(socketId))
            return false;

        var recentPackets = Queues[socketId];
        var indexBuffer = recentPackets.Select(b => b.ToHex());
        var bufferHex = buffer.ToHex();
        return indexBuffer.Contains(bufferHex);
    }

    public static void Tick(object? state)
    {
        foreach (var kvp in Queues)
        {
            if (kvp.Value.Count > 0)
            {
                SendBuffers(kvp.Key);
            }                
        }
    }

    public static void StartTicking(int intervalMilliseconds)
    {
        Timer timer = new Timer(Tick, null, 0, intervalMilliseconds);
    }
}