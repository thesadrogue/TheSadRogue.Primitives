using MonoRectangle = Microsoft.Xna.Framework.Rectangle;
using SadRogueRectangle = SadRogue.Primitives.Rectangle;

namespace System.Numerics.Grid
{
	public static class RectangleExtensions
	{
		public static SadRogueRectangle ToRectangle(this MonoRectangle self) => new SadRogueRectangle(self.X, self.Y, self.Width, self.Height);
	}
}

namespace Microsoft.Xna.Framework
{
	public static class MonoRectangleExtensions
	{
		public static MonoRectangle ToMonoRectangle(this SadRogueRectangle self) => new MonoRectangle(self.X, self.Y, self.Width, self.Height);
	}
}
