﻿/*
 * QueueBuffer
 * 
 * Author: Andre Ferreira
 * 
 * Copyright (c) Uzmi Games. Licensed under the MIT License.
 *    
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

public struct BufferDataRef
{
    public ServerPacket PacketType;
    public ByteBuffer Data;
}

public class QueueBuffer
{
    public static Dictionary<string, List<BufferDataRef>> Queues = new Dictionary<string, List<BufferDataRef>>();
    public static Dictionary<string, Connection> Sockets = new Dictionary<string, Connection>();
    public static int MaxBufferSize = 512 * 1024;
    public static byte EndOfPacketByte = 0xFE;
    public static int EndRepeatByte = 4;

    public static void AddSocket(string id, Connection socket)
    {
        Sockets[id] = socket;
    }

    public static void RemoveSocket(string id)
    {
        if (Sockets.ContainsKey(id))
            Sockets.Remove(id);
    }

    public static Connection? GetSocket(string id)
    {
        return Sockets.ContainsKey(id) ? Sockets[id] : null;
    }

    public static void AddBuffer(ServerPacket packetType, string socketId, ByteBuffer buffer)
    {
        if (!Queues.ContainsKey(socketId))
            Queues[socketId] = new List<BufferDataRef>();

        if (!IsDuplicatePacket(socketId, buffer))
        {
            Queues[socketId].Add(new BufferDataRef
            {
                PacketType = packetType,
                Data = buffer
            }) ; 

            CheckAndSend(socketId);
        }
    }

    public static void CheckAndSend(string socketId)
    {
        if (Queues.ContainsKey(socketId))
        {
            var buffers = Queues[socketId];
            int totalSize = buffers.Sum(buffer => buffer.Data.GetBuffer().Length);

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
                GetSocket(socketId)?.Send(finalBuffer);
                Queues[socketId].Clear();
            }
            else
            {
                GetSocket(socketId)?.Send(buffers[0].PacketType, buffers[0].Data.GetBuffer()); 
                Queues[socketId].Clear();
            }
        }
    }

    public static ByteBuffer CombineBuffers(List<BufferDataRef> buffers)
    {
        int totalLength = buffers.Sum(buffer => buffer.Data.GetBuffer().Length)
                          + (buffers.Count * (EndRepeatByte + 1)) 
                          + 1;  

        byte[] combinedArray = new byte[totalLength];
        int position = 0;

        combinedArray[position++] = (byte)ServerPacket.Queue;

        foreach (var buffer in buffers)
        {
            combinedArray[position++] = (byte)buffer.PacketType;

            var buf = buffer.Data.GetBuffer();
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
        var indexBuffer = recentPackets.Select(b => b.Data.ToHex());
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
}