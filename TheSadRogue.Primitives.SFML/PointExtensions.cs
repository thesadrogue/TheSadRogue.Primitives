using System;
using System.Diagnostics.Contracts;
using SadRogue.Primitives;
using SFML.System;
using SadRoguePoint = SadRogue.Primitives.Point;

namespace SadRogue.Primitives
{
    public static class SadRoguePointExtensions
    {
        [Pure]
        public static Vector2i ToVector2i(this SadRoguePoint self) => new Vector2i(self.X, self.Y);
        [Pure]
        public static Vector2u ToVector2u(this SadRoguePoint self) => new Vector2u((uint)self.X, (uint)self.Y);
        [Pure]
        public static Vector2f ToVector2f(this SadRoguePoint self) => new Vector2f(self.X, self.Y);
        [Pure]
        public static SadRoguePoint Add(this SadRoguePoint self, Vector2i other) => new SadRoguePoint(self.X + other.X, self.Y + other.Y);
        [Pure]
        public static SadRoguePoint Add(this SadRoguePoint self, Vector2u other) => new SadRoguePoint(self.X + (int)other.X, self.Y + (int)other.Y);
        [Pure]
        public static SadRoguePoint Add(this SadRoguePoint self, Vector2f other)
            => new SadRoguePoint((int)Math.Round(self.X + other.X, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y + other.Y, MidpointRounding.AwayFromZero));
        [Pure]
        public static SadRoguePoint Subtract(this SadRoguePoint self, Vector2i other) => new SadRoguePoint(self.X - other.X, self.Y - other.Y);
        [Pure]
        public static SadRoguePoint Subtract(this SadRoguePoint self, Vector2u other) => new SadRoguePoint(self.X - (int)other.X, self.Y - (int)other.Y);
        [Pure]
        public static SadRoguePoint Subtract(this SadRoguePoint self, Vector2f other)
            => new SadRoguePoint((int)Math.Round(self.X - other.X, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y - other.Y, MidpointRounding.AwayFromZero));

        [Pure]
        public static SadRoguePoint Multiply(this SadRoguePoint self, Vector2i other) => new SadRoguePoint(self.X * other.X, self.Y * other.Y);
        [Pure]
        public static SadRoguePoint Multiply(this SadRoguePoint self, Vector2u other) => new SadRoguePoint(self.X * (int)other.X, self.Y * (int)other.Y);
        [Pure]
        public static SadRoguePoint Multiply(this SadRoguePoint self, Vector2f other)
            => new SadRoguePoint((int)Math.Round(self.X * other.X, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y * other.Y, MidpointRounding.AwayFromZero));
        [Pure]
        public static SadRoguePoint Divide(this SadRoguePoint self, Vector2i other)
            => new SadRoguePoint((int)Math.Round(self.X / (double)other.X, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y / (double)other.Y, MidpointRounding.AwayFromZero));
        [Pure]
        public static SadRoguePoint Divide(this SadRoguePoint self, Vector2u other)
            => new SadRoguePoint((int)Math.Round(self.X / (double)other.X, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y / (double)other.Y, MidpointRounding.AwayFromZero));
        [Pure]
        public static SadRoguePoint Divide(this SadRoguePoint self, Vector2f other)
            => new SadRoguePoint((int)Math.Round(self.X / other.X, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y / other.Y, MidpointRounding.AwayFromZero));

        [Pure]
        public static bool Equals(this SadRoguePoint self, Vector2i other) => self.X == other.X && self.Y == other.Y;
        [Pure]
        public static bool Equals(this SadRoguePoint self, Vector2u other) => self.X == other.X && self.Y == other.Y;
        [Pure]
        public static bool Equals(this SadRoguePoint self, Vector2f other) => self.X == other.X && self.Y == other.Y;
    }
}

namespace SFML.System
{
    public static class Vector2iExtensions
    {
        [Pure]
        public static SadRoguePoint ToPoint(this Vector2i self) => new SadRoguePoint(self.X, self.Y);
        [Pure]
        public static Vector2i Add(this Vector2i self, SadRoguePoint other) => new Vector2i(self.X + other.X, self.Y + other.Y);
        [Pure]
        public static Vector2i Add(this Vector2i self, int i) => new Vector2i(self.X + i, self.Y + i);
        [Pure]
        public static Vector2i Add(this Vector2i self, Direction dir) => new Vector2i(self.X + dir.DeltaX, self.Y + dir.DeltaY);
        [Pure]
        public static Vector2i Subtract(this Vector2i self, SadRoguePoint other) => new Vector2i(self.X - other.X, self.Y - other.Y);
        [Pure]
        public static Vector2i Subtract(this Vector2i self, int i) => new Vector2i(self.X - i, self.Y - i);

        [Pure]
        public static Vector2i Multiply(this Vector2i self, SadRoguePoint other) => new Vector2i(self.X * other.X, self.Y * other.Y);
        [Pure]
        public static Vector2i Multiply(this Vector2i self, int i) => new Vector2i(self.X * i, self.Y * i);
        [Pure]
        public static Vector2i Multiply(this Vector2i self, double d)
            => new Vector2i((int)Math.Round(self.X * d, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y * d, MidpointRounding.AwayFromZero));

        [Pure]
        public static Vector2i Divide(this Vector2i self, SadRoguePoint other)
            => new Vector2i((int)Math.Round(self.X / (double)other.X, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y / (double)other.Y, MidpointRounding.AwayFromZero));

        [Pure]
        public static Vector2i Divide(this Vector2i self, double d)
            => new Vector2i((int)Math.Round(self.X / d, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y / d, MidpointRounding.AwayFromZero));

        [Pure]
        public static bool Equals(this Vector2i self, SadRoguePoint other) => self.X == other.X && self.Y == other.Y;
    }

    public static class Vector2uExtensions
    {
        [Pure]
        public static SadRoguePoint ToPoint(this Vector2u self) => new SadRoguePoint((int)self.X, (int)self.Y);
        [Pure]
        public static Vector2u Add(this Vector2u self, SadRoguePoint other) => new Vector2u(self.X + (uint)other.X, self.Y + (uint)other.Y);
        [Pure]
        public static Vector2u Add(this Vector2u self, int i) => new Vector2u(self.X + (uint)i, self.Y + (uint)i);
        [Pure]
        public static Vector2u Add(this Vector2u self, Direction dir) => new Vector2u((uint)(self.X + dir.DeltaX), (uint)(self.Y + dir.DeltaY));
        [Pure]
        public static Vector2u Subtract(this Vector2u self, SadRoguePoint other) => new Vector2u((uint)(self.X - other.X), (uint)(self.Y - other.Y));
        [Pure]
        public static Vector2u Subtract(this Vector2u self, int i) => new Vector2u((uint)(self.X - i), (uint)(self.Y - i));

        [Pure]
        public static Vector2u Multiply(this Vector2u self, SadRoguePoint other) => new Vector2u((uint)(self.X * other.X), (uint)(self.Y * other.Y));
        [Pure]
        public static Vector2u Multiply(this Vector2u self, int i) => new Vector2u((uint)(self.X * i), (uint)(self.Y * i));
        [Pure]
        public static Vector2u Multiply(this Vector2u self, double d)
            => new Vector2u((uint)Math.Round(self.X * d, MidpointRounding.AwayFromZero), (uint)Math.Round(self.Y * d, MidpointRounding.AwayFromZero));

        [Pure]
        public static Vector2u Divide(this Vector2u self, SadRoguePoint other)
            => new Vector2u((uint)Math.Round(self.X / (double)other.X, MidpointRounding.AwayFromZero), (uint)Math.Round(self.Y / (double)other.Y, MidpointRounding.AwayFromZero));

        [Pure]
        public static Vector2u Divide(this Vector2u self, double d)
            => new Vector2u((uint)Math.Round(self.X / d, MidpointRounding.AwayFromZero), (uint)Math.Round(self.Y / d, MidpointRounding.AwayFromZero));

        [Pure]
        public static bool Equals(this Vector2u self, SadRoguePoint other) => self.X == other.X && self.Y == other.Y;
    }

    public static class Vector2fExtensions
    {
        [Pure]
        public static SadRoguePoint ToPoint(this Vector2f self)
            => new SadRoguePoint((int)Math.Round(self.X, MidpointRounding.AwayFromZero), (int)Math.Round(self.Y, MidpointRounding.AwayFromZero));
        [Pure]
        public static Vector2f Add(this Vector2f self, SadRoguePoint other) => new Vector2f(self.X + other.X, self.Y + other.Y);
        [Pure]
        public static Vector2f Add(this Vector2f self, int i) => new Vector2f(self.X + i, self.Y + i);
        [Pure]
        public static Vector2f Add(this Vector2f self, Direction dir) => new Vector2f(self.X + dir.DeltaX, self.Y + dir.DeltaY);

        [Pure]
        public static Vector2f Subtract(this Vector2f self, SadRoguePoint other) => new Vector2f(self.X - other.X, self.Y - other.Y);
        [Pure]
        public static Vector2f Subtract(this Vector2f self, int i) => new Vector2f(self.X - i, self.Y - i);

        [Pure]
        public static Vector2f Multiply(this Vector2f self, SadRoguePoint other) => new Vector2f(self.X * other.X, self.Y * other.Y);
        [Pure]
        public static Vector2f Multiply(this Vector2f self, int i) => new Vector2f(self.X * i, self.Y * i);
        [Pure]
        public static Vector2f Multiply(this Vector2f self, double d) => new Vector2f(self.X * (float)d, self.Y * (float)d);

        [Pure]
        public static Vector2f Divide(this Vector2f self, SadRoguePoint other) => new Vector2f(self.X / other.X, self.Y / other.Y);
        [Pure]
        public static Vector2f Divide(this Vector2f self, double d) => new Vector2f(self.X / (float)d, self.Y / (float)d);

        [Pure]
        public static bool Equals(this Vector2f self, SadRoguePoint other) => self.X == other.X && self.Y == other.Y;
    }
}
