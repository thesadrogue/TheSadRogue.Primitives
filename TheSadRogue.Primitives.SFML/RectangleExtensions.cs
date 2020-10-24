using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using SFML.Graphics;
using SadRogueRectangle = SadRogue.Primitives.Rectangle;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for <see cref="SadRogue.Primitives.Rectangle"/> that enable operations involving
    /// <see cref="SFML.Graphics.IntRect"/>.
    /// </summary>
    public static class RectangleExtensions
    {
        /// <summary>
        /// Converts from <see cref="SadRogue.Primitives.Rectangle"/> to <see cref="SFML.Graphics.IntRect"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntRect ToIntRect(this SadRogueRectangle self)
            => new IntRect(self.X, self.Y, self.X + self.Width, self.Y + self.Height);

        /// <summary>
        /// Compares a <see cref="SadRogue.Primitives.Rectangle"/> to a <see cref="SFML.Graphics.IntRect"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this SadRogueRectangle self, IntRect other)
            => self.X == other.Left && self.Y == other.Top && self.Width == other.Width && self.Height == other.Height;
    }
}

namespace SFML.Graphics
{
    /// <summary>
    /// Extension methods for <see cref="SFML.Graphics.IntRect"/> that enable operations involving
    /// <see cref="SadRogue.Primitives.Rectangle"/>.
    /// </summary>
    public static class IntRectExtensions
    {
        /// <summary>
        /// Converts from <see cref="SFML.Graphics.IntRect"/> to <see cref="SadRogue.Primitives.Rectangle"/>.
        /// </summary>
        /// <param name="self"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SadRogueRectangle ToRectangle(this IntRect self)
            => new SadRogueRectangle(self.Left, self.Top, self.Width - self.Left, self.Height - self.Top);

        /// <summary>
        /// Compares a <see cref="SFML.Graphics.IntRect"/> to a <see cref="SadRogue.Primitives.Rectangle"/>.
        /// </summary>
        /// <param name="self"/>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Matches(this IntRect self, SadRogueRectangle other)
            => self.Left == other.X && self.Top == other.Y && self.Width == other.Width && self.Height == other.Height;
    }
}
