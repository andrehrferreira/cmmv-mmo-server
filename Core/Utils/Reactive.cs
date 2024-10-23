
/// <summary>
/// A class that implements a reactive system for observing changes in a value of a generic type.
/// Allows subscribers to be notified whenever the value is updated.
/// </summary>
/// <typeparam name="T">The type of the value being observed.</typeparam>
public class Reactive<T>
{
    private T _value;
    private List<Action<T>> _subscribers = new List<Action<T>>();

    /// <summary>
    /// Gets or sets the observed value. When a new value is set, all subscribers are notified.
    /// </summary>
    public T Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                _value = value;
                NotifySubscribers(_value);
            }
        }
    }

    /// <summary>
    /// Subscribes a subscriber to be notified when the value changes.
    /// </summary>
    /// <param name="subscriber">An action that will be executed when the value is updated.</param>
    /// <returns>An IDisposable that can be used to unsubscribe.</returns>
    public IDisposable Subscribe(Action<T> subscriber)
    {
        _subscribers.Add(subscriber);
        subscriber(_value);

        return new Unsubscriber<T>(_subscribers, subscriber);
    }

    /// <summary>
    /// Unsubscribes a subscriber manually.
    /// </summary>
    /// <param name="subscriber">The subscriber action to be removed from the list of subscribers.</param>
    public void Unsubscribe(Action<T> subscriber)
    {
        if (_subscribers.Contains(subscriber))
        {
            _subscribers.Remove(subscriber);
        }
    }

    /// <summary>
    /// Clears all subscribers from the list.
    /// </summary>
    public void ClearAll()
    {
        _subscribers.Clear();
    }

    /// <summary>
    /// Notifies all subscribers with the new value.
    /// </summary>
    /// <param name="newValue">The new value to be sent to all subscribers.</param>
    private void NotifySubscribers(T newValue)
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber(newValue);
        }
    }

    /// <summary>
    /// A class that manages the removal of subscribers when they choose to unsubscribe.
    /// </summary>
    /// <typeparam name="TSubscriber">The type of the subscriber.</typeparam>
    private class Unsubscriber<TSubscriber> : IDisposable
    {
        private List<Action<TSubscriber>> _subscribers;
        private Action<TSubscriber> _subscriber;

        /// <summary>
        /// Initializes a new instance of the Unsubscriber class.
        /// </summary>
        /// <param name="subscribers">The list of subscribers to manage.</param>
        /// <param name="subscriber">The subscriber to be removed when Dispose is called.</param>

        public Unsubscriber(List<Action<TSubscriber>> subscribers, Action<TSubscriber> subscriber)
        {
            _subscribers = subscribers;
            _subscriber = subscriber;
        }

        /// <summary>
        /// Disposes of the subscription, removing the subscriber from the list.
        /// </summary>
        public void Dispose()
        {
            if (_subscribers.Contains(_subscriber))
                _subscribers.Remove(_subscriber);
        }
    }
}
