using SFML.Graphics;
using SadRogueRectangle = SadRogue.Primitives.Rectangle;

namespace SadRogue.Primitives
{
    public static class RectangleExtensions
    {
        public static IntRect ToIntRect(this SadRogueRectangle self) => new IntRect(self.X, self.Y, self.Width, self.Height);

        public static bool Equals(this SadRogueRectangle self, IntRect other)
            => self.X == other.Left && self.Y == other.Top && self.Width == other.Width && self.Height == other.Height;
    }
}

namespace SFML.Graphics
{
    public static class IntRectExtensions
    {
        public static SadRogueRectangle ToRectangle(this IntRect self) => new SadRogueRectangle(self.Left, self.Top, self.Width, self.Height);

        public static bool Equals(this IntRect self, SadRogueRectangle other)
            => self.Left == other.X && self.Top == other.Y && self.Width == other.Width && self.Height == other.Height;
    }
}
