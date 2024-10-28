/*
 * Network Events
 * 
 * Author: Andre Ferreira
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

public struct NetworkEventData<T>
{
    public T Data { get; }
    public Connection Connection { get; }

    public NetworkEventData(T data, Connection connection)
    {
        Data = data;
        Connection = connection;
    }
}

public class NetworkEvents<T>
{
    private List<Action<T, Connection>> _subscribers = new List<Action<T, Connection>>();

    public void NetworkEvent<T>()
    {
        //Server.RegisterHandler<T>((byte)ServerPacket.UpdateEntity, Server.OnUpdateEntity);
    }

    public IDisposable Subscribe(Action<T, Connection> subscriber)
    {
        _subscribers.Add(subscriber);
        return new Unsubscriber<T>(_subscribers, subscriber);
    }

    public void Unsubscribe(Action<T, Connection> subscriber)
    {
        if (_subscribers.Contains(subscriber))
        {
            _subscribers.Remove(subscriber);
        }
    }

    public void ClearAll()
    {
        _subscribers.Clear();
    }

    public void Emit(T data, Connection socket)
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber(data, socket);
        }
    }

    private class Unsubscriber<TSubscriber> : IDisposable
    {
        private List<Action<TSubscriber, Connection>> _subscribers;
        private Action<TSubscriber, Connection> _subscriber;

        public Unsubscriber(List<Action<TSubscriber, Connection>> subscribers, Action<TSubscriber, Connection> subscriber)
        {
            _subscribers = subscribers;
            _subscriber = subscriber;
        }

        public void Dispose()
        {
            if (_subscribers.Contains(_subscriber))
                _subscribers.Remove(_subscriber);
        }
    }
}
