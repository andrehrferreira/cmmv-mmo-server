/*
 * Connection
 * 
 * Author: Diego Guedes
 * Modified by: Andre Ferreira
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

using NanoSockets;
using System.Runtime.CompilerServices;
using System.Threading;

public enum ConnectionState
{
    Connecting = 0,
    Connected = 1,
    Disconnected = 2
}

public enum DisconnectReason
{
    Timeout,
    NegativeSequence,
    RemoteBufferTooBig,
    Other
}

public interface IHeaderWriter
{
    void PutUnreliableHeader(ByteBuffer buffer);
    void ReadUnreliableHeader(ByteBuffer buffer);
}

public class Connection
{
    public string Id;
    public Entity Entity;
    public const int Mtu = 1200;

    public Address RemoteEndPoint;
    public Server Manager;
    public string Token;
    public int PacketsPerSecond;
    public TimeSpan PacketsPerSecondTimeout;
    public IHeaderWriter HeaderWriter;
    public ConnectionState State;
    public Connection Next;
    public float TimeoutLeft = 120f;

    public DisconnectReason Reason;
    public DateTime PingSendAt;

    //Buffer
    internal ByteBuffer ReliableBuffer;
    internal ByteBuffer UnreliableBuffer;
    internal ByteBuffer AckBuffer;
    internal Dictionary<short, ByteBuffer> ReliablePackets = new Dictionary<short, ByteBuffer>();
    internal Dictionary<short, ByteBuffer> RemoteReliableOrderBuffer = new Dictionary<short, ByteBuffer>();
    private short Sequence = 1;
    public short NextRemoteSequence = 2;

    //Actions
    public Action OnDisconnect;
    public Action<ByteBuffer> OnReceive;


    public ByteBuffer BeginReliable()
    {
        if (ReliableBuffer == null)
        {
            ReliableBuffer = ConcurrentByteBufferPool.Acquire();
            ReliableBuffer.Connection = this;

            ReliableBuffer.Write((byte)NetworkPacketType.Reliable);
            ReliableBuffer.Write(Sequence);
            ReliableBuffer.Write(Manager.TickNumber);

            ReliableBuffer.Sequence = Sequence;
            ReliableBuffer.Reliable = true;
        }

        return ReliableBuffer;
    }

    public void Send(ServerPacket packetType, ByteBuffer data, bool encryptData = false)
    {
        Send(packetType, data.GetBuffer(), encryptData);
    }

    public void Send(ServerPacket packetType, byte[] data, bool encryptData = false)
    {
        
    }

    public void Send(byte[] data, bool encryptData = false)
    {

    }
}