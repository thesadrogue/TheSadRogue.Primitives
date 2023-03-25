namespace SadRogue.Primitives.UnitTests.Mocks
{
    public class MockSpatialMapItem : IHasID, IHasLayer
    {
        private static readonly IDGenerator s_idGen = new IDGenerator();

        public MockSpatialMapItem(int layer)
        {
            ID = s_idGen.UseID();
            Layer = layer;
        }

        public uint ID { get; }
        public int Layer { get; }

        public override string ToString() => $"[{ID}, {Layer}]";
    }
}
