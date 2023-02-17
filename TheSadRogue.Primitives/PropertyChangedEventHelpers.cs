using System;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Event arguments for an event fired when an object's properties are changed.  Often used with
    /// <see cref="PropertyChangedEventHelpers.SafelySetProperty{TObject,TProperty}(TObject,ref TProperty,TProperty,System.EventHandler{SadRogue.Primitives.ValueChangedEventArgs{TProperty}}?)"/>
    /// and other overloads of that function.
    /// </summary>
    /// <remarks>
    /// It is fairly common to have an event that is fired when a property is changed; a common use case relative to
    /// 2D grids is objects that have a position and fire an event when that position changes. This class encapsulates a
    /// generic event argument for such occurrences.
    ///
    /// In addition to implementing basic functionality for handling value changes, it also has the appropriate flags for
    /// implementing the concept of "handling" an event; if supported by the event, this allows one event handler to
    /// mark the event as "handled" and stop other event handlers from being run.
    /// </remarks>
    /// <typeparam name="TProperty">Type of the property changed.</typeparam>
    public class ValueChangedEventArgs<TProperty> : EventArgs
    {
        /// <summary>
        /// Previous value of property.
        /// </summary>
        public readonly TProperty OldValue;

        /// <summary>
        /// New value of property.
        /// </summary>
        public readonly TProperty NewValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="oldValue">Previous value of property.</param>
        /// <param name="newValue">New value of property.</param>
        public ValueChangedEventArgs(TProperty oldValue, TProperty newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }


    /// <summary>
    /// Helper functions useful for implementing events using <see cref="ValueChangedEventArgs{TProperty}"/> as their parameter.
    /// </summary>
    public static class PropertyChangedEventHelpers
    {
        /// <summary>
        /// Sets the given field to the new value, and fires the corresponding event just after the value has been changed.
        /// The value will be properly reverted to the old value if the event handler throws InvalidOperationException.
        /// </summary>
        /// <remarks>
        /// It is fairly common to have an object with a property, such that when the property changes, an event is fired.
        /// <see cref="IPositionable"/> represents one such interface that is common when dealing with a 2D grid.
        ///
        /// Although the implementation of such a property is relatively trivial, there are a few subtle issues that
        /// can occur:
        /// - The event might be fired even if the property was set to the same value it is currently
        /// - If a handler throws something like InvalidOperationException, the value may not be reverted properly.
        ///
        /// This function represents a convenient way to implement such a property.  An implementation using this property
        /// might look like the example below.
        ///
        /// Note that this function can work for properties of any type; you could just as easily implement <see cref="IPositionable"/>
        /// via this interface, for example.
        ///
        /// <example>
        /// <code>
        /// class MyClass
        /// {
        ///     private int _myProperty;
        ///     public int MyProperty
        ///     {
        ///         get => _myProperty;
        ///         set => this.SafelySetProperty(ref _myProperty, value, MyPropertyChanged);
        ///     }
        ///     public event EventHandler&lt;ValueChangedEventArgs&lt;int&gt;&gt;? MyPropertyChanged;
        /// }
        /// </code>
        ///</example>
        /// </remarks>
        /// <param name="self" />
        /// <param name="propertyField">Field to set.</param>
        /// <param name="newValue">New value to set to given field.</param>
        /// <param name="changedEvent">Event to fire when change occurs.</param>
        /// <typeparam name="TObject">Type of the object the property resides on.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        public static void SafelySetProperty<TObject, TProperty>(this TObject self, ref TProperty propertyField, TProperty newValue,
                                                EventHandler<ValueChangedEventArgs<TProperty>>? changedEvent)
            where TObject : notnull
        {
            // Nothing to do; the value hasn't changed
            if (propertyField is null)
            {
                if (newValue is null) return;
            }
            else if (propertyField.Equals(newValue)) return;

            // Set new value and fire event
            var oldValue = propertyField;
            propertyField = newValue;
            try
            {
                changedEvent?.Invoke(self, new ValueChangedEventArgs<TProperty>(oldValue, newValue));
            }
            catch (InvalidOperationException)
            {
                // If exception, preserve old value for future
                propertyField = oldValue;
                throw;
            }
        }

        /// <summary>
        /// Sets the given field to the new value, and fires the corresponding events.  The value will be properly
        /// reverted to the old value if the Changed event handler throws InvalidOperationException.  The "changing"
        /// event will be fired just _before_ the value actually changes; the "changed" event will be fired just after.
        /// </summary>
        /// <param name="self" />
        /// <param name="propertyField">Field to set.</param>
        /// <param name="newValue">New value to set to given field.</param>
        /// <param name="changingEvent">Event to fire when change is about to occur.</param>
        /// <param name="changedEvent">Event to fire after change occurs.</param>
        /// <typeparam name="TObject">Type of the object the property resides on.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        public static void SafelySetProperty<TObject, TProperty>(this TObject self, ref TProperty propertyField, TProperty newValue,
                                                                 EventHandler<ValueChangedEventArgs<TProperty>>? changingEvent,
                                                                 EventHandler<ValueChangedEventArgs<TProperty>>? changedEvent)
            where TObject : notnull
        {
            // Nothing to do; the value hasn't changed
            if (propertyField is null)
            {
                if (newValue is null) return;
            }
            else if (propertyField.Equals(newValue)) return;

            // Create event arguments
            var args = new ValueChangedEventArgs<TProperty>(propertyField, newValue);

            // Fire "pre-change" event
            changingEvent?.Invoke(self, args);

            // Set new value and fire event
            var oldValue = propertyField;
            propertyField = newValue;
            try
            {
                changedEvent?.Invoke(self, args);
            }
            catch (InvalidOperationException)
            {
                // If exception, preserve old value for future
                propertyField = oldValue;
                throw;
            }
        }
    }
}
