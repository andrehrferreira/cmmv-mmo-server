/*
 * UDP Server Implementation
 * 
 * Author: Diego Guedes
 * Modified by: Andre Ferreira
 * 
 * Copyright (c) Uzmi Games. Licensed under the MIT License.
 * 
 * Description:
 * This class implements a UDP server using the NanoSockets library. It is designed for high-performance,
 * low-level UDP communication, including packet management, connection handling, and multithreading support 
 * for sending and receiving data. The server includes features such as a send queue, multi-threaded packet 
 * processing, and the ability to control the rate of packets sent per second.
 *  
 * Dependencies:
 * - NanoSockets library for low-level UDP communication.
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
using System;
using System.Diagnostics;

public unsafe partial class Server
{
    private static Socket UdpSocketv4;
    private static Dictionary<Address, Connection> Connections = new Dictionary<Address, Connection>();
    public static Thread SendThread;
    public static Thread ReceiveThread;
    private static volatile bool IsReceiveThreadStop;
    private static volatile bool IsSendThreadStop;
    private static AutoResetEvent SendEvent;
    private static ByteBufferPool GlobalSendQueue = new ByteBufferPool();

    private ByteBuffer EventQueue;
    private Connection First;
    private Connection PendingConnections;

    public static int MaxPacketsPerSecond = 1000;
    public uint TickNumber = 1;
    public static int Port;

    //Reliable
    private static ByteBuffer ReliableList;
    private static Stopwatch ReliableTimer = Stopwatch.StartNew();
    public static int ReliableResendThreshold = 30;
    private static Stopwatch PacketsCounterStopwatch = Stopwatch.StartNew();
    public static TimeSpan ReliableTimeout = TimeSpan.FromMilliseconds(250f);

    [ThreadStatic]
    static ByteBufferPool LocalSendQueue;

    public static Action<Func<Server, Connection>, Action, string> ConnectionRequestEvent;

    static Server()
    {

    }

    public static void Start(int port)
    {
        Port = port;

        Address listenAddress = new Address();

        listenAddress.Port = (ushort)port;

        UdpSocketv4 = UDP.Create(512 * 1024, 512 * 1024);

        while (UDP.SetIP(ref listenAddress, "::0") != 0)
        {
            Thread.Sleep(1000);
        }

        while (UDP.Bind(UdpSocketv4, ref listenAddress) != 0)
        {
            Thread.Sleep(1000);
        }

        Console.WriteLine("UDP Started without errors");

        SendToBackgroundThread();
    }

    public static void SendToBackgroundThread()
    {
        SendEvent = new AutoResetEvent(false);

        SendThread = new Thread(() =>
        {
            IsSendThreadStop = false;

            try
            {
                while (true)
                {
                    SendEvent.WaitOne(ReliableTimeout);

                    if (IsSendThreadStop)
                    {
                        throw new Exception();
                    }

                    var udpSocket = UdpSocketv4;

                    bool sent;

                    do
                    {
                        sent = false;

                        ByteBuffer buffer = null;

                        Address addr;

                        /*if (ReliableTimer.Elapsed >= ReliableTimeout)
                        {
                            ReliableTimer.Restart();

                        begin:
                            if (ReliableList != null)
                            {
                                sent = true;

                                if (Interlocked.Decrement(ref ReliableList.Acked) <= 0)
                                {
                                    var next = ReliableList.Next;

                                    ConcurrentByteBufferPool.Release(ReliableList);

                                    ReliableList = next;

                                    goto begin;
                                }

                                addr = ReliableList.Connection.RemoteEndPoint;

                                if (ReliableList.Data != null && !ReliableList.IsDestroyed)
                                    UDP.Unsafe.Send(udpSocket, &addr, ReliableList.Data, ReliableList.Size);

                                ByteBuffer before = ReliableList;
                                ByteBuffer current = ReliableList.Next;

                                while (current != null)
                                {
                                    if (Interlocked.Decrement(ref current.Acked) <= 0)
                                    {
                                        var next = current.Next;

                                        before.Next = next;

                                        ConcurrentByteBufferPool.Release(current);

                                        current = next;
                                    }
                                    else if (current.Connection != null)
                                    {
                                        addr = current.Connection.RemoteEndPoint;

                                        if (current.Data != null && !current.IsDestroyed)
                                            UDP.Unsafe.Send(udpSocket, &addr, current.Data, current.Size);

                                        before = current;

                                        current = current.Next;
                                    }
                                    else
                                    {
                                        ConcurrentByteBufferPool.Release(current);
                                    }
                                }
                            }
                        }*/
                    }
                    while (sent);
                }
            }
            catch (Exception e)
            {
                lock (GlobalSendQueue)
                {
                    GlobalSendQueue = new ByteBufferPool();

                    ReliableList = null;
                }
            }
        });

        SendThread.IsBackground = true;

        SendThread.Start();
    }
} 