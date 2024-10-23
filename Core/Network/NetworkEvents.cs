public struct NetworkEventData<T>
{
    public T Data { get; }
    public Socket Connection { get; }

    public NetworkEventData(T data, Socket connection)
    {
        Data = data;
        Connection = connection;
    }
}

public class NetworkEvents<T>
{
    private List<Action<T, Socket>> _subscribers = new List<Action<T, Socket>>();

    public IDisposable Subscribe(Action<T, Socket> subscriber)
    {
        _subscribers.Add(subscriber);
        return new Unsubscriber<T>(_subscribers, subscriber);
    }

    public void Unsubscribe(Action<T, Socket> subscriber)
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

    public void Emit(T data, Socket socket)
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber(data, socket);
        }
    }

    private class Unsubscriber<TSubscriber> : IDisposable
    {
        private List<Action<TSubscriber, Socket>> _subscribers;
        private Action<TSubscriber, Socket> _subscriber;

        public Unsubscriber(List<Action<TSubscriber, Socket>> subscribers, Action<TSubscriber, Socket> subscriber)
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
