using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace TitanBot.Core.Models
{
    public delegate void BindingUpdateEventHandler(object sender, BindingUpdateEventArgs e);

    public class BoundModel
    {
        public event BindingUpdateEventHandler BindingUpdated;

        private static ConcurrentDictionary<Type, ConcurrentDictionary<string, BindingUpdateEventHandler>> _handlers
            = new ConcurrentDictionary<Type, ConcurrentDictionary<string, BindingUpdateEventHandler>>();

        private Type _type;

        public BoundModel()
        {
            _type = GetType();
        }

        private void RemoveHandler(BoundModel scanner, string property)
        {
            if (_handlers.TryGetValue(_type, out var handlers) && handlers.TryRemove(property, out var handler))
                scanner.BindingUpdated -= handler;
        }

        private void AddHandler(BoundModel scanner, string property)
        {
            void _handle(object sender, BindingUpdateEventArgs e)
                => OnChange(property, scanner, scanner);

            scanner.BindingUpdated += _handle;

            _handlers.GetOrAdd(_type, t => new ConcurrentDictionary<string, BindingUpdateEventHandler>())[property] = _handle;
        }

        protected void OnChange(string propertyName, object oldValue, object newValue)
            => BindingUpdated?.Invoke(this, new BindingUpdateEventArgs(propertyName, oldValue, newValue));

        protected void Set<T>(ref T field, T value, [CallerMemberName]string caller = null)
        {
            var oldValue = field;
            field = value;

            if (oldValue is BoundModel o)
                RemoveHandler(o, caller);
            if (value is BoundModel v)
                AddHandler(v, caller);

            OnChange(caller, oldValue, value);
        }
    }

    public class BindingUpdateEventArgs
    {
        public string PropertyName { get; }
        public object OldValue { get; }
        public object NewValue { get; }

        public BindingUpdateEventArgs(string propertyName, object oldValue, object newValue)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}