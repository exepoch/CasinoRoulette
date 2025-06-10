using System;

namespace UI
{
    // A property that notifies subscribers when its value changes
    public class BindableProperty<T>
    {
        public event Action<T> OnValueChanged; // Event triggered when value changes

        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                // Only trigger event if new value is different
                if (!Equals(_value, value))
                {
                    _value = value;
                    OnValueChanged?.Invoke(_value);
                }
            }
        }
    }
}