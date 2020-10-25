using System.Diagnostics.CodeAnalysis;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Interface implemented to define a form of checking value equality, without guarantees that it corresponds
    /// to GetHashCode.
    /// </summary>
    /// <typeparam name="T">Type of object being compared.</typeparam>
    public interface IMatchable<in T>
    {
        /// <summary>
        /// Returns true if the given object is considered "equal" to the current one, based on the definition
        /// of equality for the object.
        /// </summary>
        /// <param name="other">Object to compare to.</param>
        /// <returns>True if the objects are considered equal, false, otherwise.</returns>
        bool Matches([AllowNull]T other);
    }
}
