namespace SadRogue.Primitives
{
    /// <summary>
    /// Interface for objects that reside on a "Z-index" or "layer".
    /// </summary>
    /// <remarks>
    /// The concept of a z-index is fairly common in rendering and representation of objects in a 2d grid.  This interface
    /// can be used by any code that wants to work with objects that have some sort of layer assigned to them; but
    /// the library's code primarily uses it as part of the <see cref="SpatialMaps.LayeredSpatialMap{T}" /> implementation.
    /// </remarks>
    public interface IHasLayer
    {
        /// <summary>
        /// The layer on which the object should reside. Higher numbers indicate layers closer to the
        /// "top".
        /// </summary>
        /// <remarks>
        /// This value is typically assumed to remain constant while the object is within a data structure
        /// that uses this interface -- if it is modified, that data structure will become out of sync.
        /// </remarks>
        int Layer { get; }
    }
}
