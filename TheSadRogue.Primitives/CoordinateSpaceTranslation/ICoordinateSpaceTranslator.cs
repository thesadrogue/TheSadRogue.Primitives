namespace SadRogue.Primitives.CoordinateSpaceTranslation
{
    /// <summary>
    /// Interface for a class which performs a translation from one coordinate space to another.
    /// </summary>
    /// <remarks>
    /// This interface provides no concrete definitions of the coordinate spaces, other than there are two of them
    /// and it is possible to translate between them bidirectionally.
    ///
    /// It consists simply of two functions which perform the translation, and the <see cref="UpdateSpaceDefinition"/>,
    /// which provides a method by which the interface may be instructed to update its definition based on whatever
    /// other structure defines it (as applicable).  This is important, because the definition of the coordinate space
    /// must ONLY change when the <see cref="UpdateSpaceDefinition"/> function is called.  This enables algorithms
    /// which use these translators to ensure the space definition does not change between when a calculation or procedure
    /// is performed and when the results of that procedure are accessed.
    /// </remarks>
    public interface ICoordinateSpaceTranslator
    {
        /// <summary>
        /// Translates a "global" coordinate to a local one.
        /// </summary>
        /// <param name="position">Global coordinate to translate.</param>
        /// <returns>The position of the given global coordinate in the local coordinate space.</returns>
        public Point GlobalToLocalPosition(Point position);

        /// <summary>
        /// Translates a "local" coordinate to a global one.
        /// </summary>
        /// <param name="position">Local coordinate to translate.</param>
        /// <returns>The position of the given local coordinate in the global coordinate space.</returns>
        public Point LocalToGlobalPosition(Point position);

        /// <summary>
        /// Updates/caches the offset or values which define the coordinate space.
        /// </summary>
        public void UpdateSpaceDefinition();
    }
}
