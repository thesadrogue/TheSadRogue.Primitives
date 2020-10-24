namespace SadRogue.Primitives
{
    /// <summary>
    /// Interface implemented by mutable reference types to implement an equality comparison that doesn't make
    /// guarantees about GetHashCode.
    /// </summary>
    /// <typeparam name="T">Type of object being compared.</typeparam>
    public interface IMatchable<in T>
        where T : class
    {
        /// <summary>
        /// Returns true if the given object is considered "equal" to the current one, based on the definition
        /// of equality for the object.
        /// </summary>
        /// <param name="other">Object to compare to.</param>
        /// <returns>True if the objects are considered equal, false, otherwise.</returns>
        bool Matches(T? other);
    }
}
