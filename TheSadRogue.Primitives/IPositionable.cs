using System;
using SadRogue.Primitives.SpatialMaps;

namespace SadRogue.Primitives
{
    /// <summary>
    /// An interface describing an object which has a position on a grid and may be able to move around.
    /// </summary>
    /// <remarks>
    /// Objects which have a position can implement this interface, which can be useful to make such objects easily
    /// compatible with automatically syncing spatial map classes like <see cref="AutoSyncMultiSpatialMap{T}"/>.
    ///
    /// Other algorithms may be able to make use of this interface as well if all they need is an object with a position.
    /// </remarks>
    public interface IPositionable
    {
        /// <summary>
        /// The position of the object.
        /// </summary>
        Point Position { get; }

        /// <summary>
        /// Event which should be fired when the object's position changes.
        /// </summary>
        event EventHandler<ObjectPropertyChanged<Point>>? PositionChanged;
    }
}
