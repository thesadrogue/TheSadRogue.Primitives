using System;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Event arguments for an event fired when an object's properties are changed.  Often used with
    /// <see cref="PropertyChangedEventHelpers.SafelySetProperty{TObject,TProperty}(TObject,ref TProperty,TProperty,System.EventHandler{SadRogue.Primitives.ValueChangedEventArgs{TProperty}}?,bool)"/>
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
        /// When <see langword="true"/>, indicates this value change can be flagged as handled and stop further event handlers; otherwise <see langword="false"/>.
        /// </summary>
        public readonly bool SupportsHandled;

        /// <summary>
        /// When <see cref="SupportsHandled"/> is <see langword="true"/>, setting this property to <see langword="true"/> flags this change as handled and to stop processing further event handlers.
        /// </summary>
        public bool IsHandled { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="oldValue">Previous value of property.</param>
        /// <param name="newValue">New value of property.</param>
        /// <param name="supportsHandled">When <see langword="true"/>, indicates this value change can be flagged as handled and stop further event handlers.</param>
        public ValueChangedEventArgs(TProperty oldValue, TProperty newValue, bool supportsHandled = false) =>
            (OldValue, NewValue, SupportsHandled) = (oldValue, newValue, supportsHandled);
    }

    /// <summary>
    /// Event arguments for an event fired when an object's properties are changed.  Often used with
    /// <see cref="PropertyChangedEventHelpers.SafelySetProperty{TObject,TProperty}(TObject,ref TProperty,TProperty,System.EventHandler{SadRogue.Primitives.ValueChangingEventArgs{TProperty}}?,System.EventHandler{SadRogue.Primitives.ValueChangedEventArgs{TProperty}}?,bool,bool)"/>.
    /// </summary>
    /// <remarks>
    /// It is fairly common to have an event that is fired when a property is changed and/or about to change; a common use case relative to
    /// This class encapsulates a generic event argument for such occurrences.
    ///
    /// In addition to implementing basic functionality for handling value changes, it also has the appropriate flags for
    /// implementing the concept of "canceling" event during the pre-change event, which if supported by the event, allows the event
    /// handler to stop the change from occuring and prevent other handlers from running.
    ///
    /// It also supports the concept of "handling" an event during the "changed" event, which, if supported by the event,
    /// allows one event handler to mark the event as "handled" and stop other event handlers from being run.
    /// </remarks>
    /// <typeparam name="TProperty">Type of the property changed.</typeparam>
    public class ValueChangingEventArgs<TProperty> : EventArgs
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
        /// When <see langword="true"/>, indicates this value change can be cancelled; otherwise <see langword="false"/>.
        /// </summary>
        public readonly bool SupportsCancel;

        /// <summary>
        /// When <see cref="SupportsCancel"/> is <see langword="true"/>, setting this property to <see langword="true"/>
        /// causes the value change to be cancelled and to stop processing further event handlers for this change.
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="oldValue">Previous value of property.</param>
        /// <param name="newValue">New value of property.</param>
        /// <param name="supportsCancel">When <see langword="true"/>, indicates this value change can be cancelled; otherwise <see langword="false"/>.</param>
        public ValueChangingEventArgs(TProperty oldValue, TProperty newValue, bool supportsCancel = false) =>
            (OldValue, NewValue, SupportsCancel) = (oldValue, newValue, supportsCancel);
    }


    /// <summary>
    /// Helper functions useful for implementing events using <see cref="ValueChangedEventArgs{TProperty}"/>
    /// or <see cref="ValueChangingEventArgs{TProperty}"/> as their parameter.
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
        ///     public event EventHandler&lt;ObjectPropertyChanged&lt;int&gt;&gt;? MyPropertyChanged;
        /// }
        /// </code>
        ///</example>
        /// </remarks>
        /// <param name="self" />
        /// <param name="propertyField">Field to set.</param>
        /// <param name="newValue">New value to set to given field.</param>
        /// <param name="changedEvent">Event to fire when change occurs.</param>
        /// <param name="supportsHandled">When <see langword="true"/>, indicates this value change can be flagged as handled and stop further event handlers; otherwise <see langword="false"/>.</param>
        /// <typeparam name="TObject">Type of the object the property resides on.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        public static void SafelySetProperty<TObject, TProperty>(this TObject self, ref TProperty propertyField, TProperty newValue,
                                                EventHandler<ValueChangedEventArgs<TProperty>>? changedEvent,
                                                bool supportsHandled = false)
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
                if (changedEvent != null)
                {
                    var args = new ValueChangedEventArgs<TProperty>(oldValue, newValue, supportsHandled);
                    if (supportsHandled)
                    {
                        foreach (var del in changedEvent.GetInvocationList())
                        {
                            var handler = (EventHandler<ValueChangedEventArgs<TProperty>>)del;
                            handler(self, args);
                            if (args.IsHandled)
                                return;
                        }
                    }
                    else // This is faster than the foreach loop above, so we should have a special case for this
                        changedEvent.Invoke(self, args);
                }

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
        /// reverted to the old value if the Changed event handler throws InvalidOperationException or if the "changing"
        /// event supports cancellation and is cancelled.  The "changing" event will be fired just _before_ the value actually changes;
        /// the "changed" event will be fired just after.
        /// </summary>
        /// <param name="self" />
        /// <param name="propertyField">Field to set.</param>
        /// <param name="newValue">New value to set to given field.</param>
        /// <param name="changingEvent">Event to fire when change is about to occur.</param>
        /// <param name="changedEvent">Event to fire after change occurs.</param>
        /// <param name="changingSupportsCancel">
        /// When <see langword="true"/>, indicates that, during the <paramref name="changingEvent"/> event, this value
        /// change can be cancelled, which stops future changing handlers and prevents the <paramref name="changedEvent"/>
        /// from firing; otherwise <see langword="false"/>.
        /// </param>
        /// <param name="changedSupportsHandled">
        /// When <see langword="true"/>, indicates that, during the <paramref name="changedEvent"/> event, this value change
        /// can be flagged as handled and stop further event handlers; otherwise <see langword="false"/>.
        /// </param>
        /// <typeparam name="TObject">Type of the object the property resides on.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        public static void SafelySetProperty<TObject, TProperty>(this TObject self, ref TProperty propertyField, TProperty newValue,
                                                                 EventHandler<ValueChangingEventArgs<TProperty>>? changingEvent,
                                                                 EventHandler<ValueChangedEventArgs<TProperty>>? changedEvent,
                                                                 bool changingSupportsCancel = false, bool changedSupportsHandled = false)
            where TObject : notnull
        {
            // Nothing to do; the value hasn't changed
            if (propertyField is null)
            {
                if (newValue is null) return;
            }
            else if (propertyField.Equals(newValue)) return;

            // Create event arguments
            var changingArgs = new ValueChangingEventArgs<TProperty>(propertyField, newValue, changingSupportsCancel);

            // Fire "pre-change" event
            if (changingEvent != null)
            {
                if (changingSupportsCancel)
                {
                    foreach (var del in changingEvent.GetInvocationList())
                    {
                        var handler = (EventHandler<ValueChangingEventArgs<TProperty>>)del;
                        handler(self, changingArgs);
                        if (changingArgs.IsCancelled)
                            return;
                    }
                }
                else // This is faster than the foreach loop above, so we should have a special case for this
                    changingEvent.Invoke(self, changingArgs);
            }

            // Set new value and fire event
            var oldValue = propertyField;
            propertyField = newValue;
            try
            {
                if (changedEvent != null)
                {
                    var changedArgs = new ValueChangedEventArgs<TProperty>(oldValue, newValue, changedSupportsHandled);
                    if (changedSupportsHandled)
                    {
                        foreach (var del in changedEvent.GetInvocationList())
                        {
                            var handler = (EventHandler<ValueChangedEventArgs<TProperty>>)del;
                            handler(self, changedArgs);
                            if (changedArgs.IsHandled)
                                return;
                        }
                    }
                    else // This is faster than the foreach loop above, so we should have a special case for this
                        changedEvent.Invoke(self, changedArgs);
                }

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
