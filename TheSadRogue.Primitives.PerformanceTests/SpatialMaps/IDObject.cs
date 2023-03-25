using System;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.SpatialMaps
{
    public class IDObject : IHasID
    {
        private static readonly IDGenerator s_idGenerator = new();

        public IDObject()
        {
            ID = s_idGenerator.UseID();
        }

        public uint ID { get; }
    }

    public class IDLayerObject : IHasID, IHasLayer
    {
        private static readonly IDGenerator s_idGenerator = new();

        public IDLayerObject(int layer)
        {
            ID = s_idGenerator.UseID();
            Layer = layer;
        }

        public uint ID { get; }

        public int Layer { get; }
    }

    public class IDPositionLayerObject : IHasID, IPositionable, IHasLayer
    {
        private static readonly IDGenerator s_idGenerator = new();

        private Point _position;

        public Point Position
        {
            get => _position;
            set => this.SafelySetProperty(ref _position, value, PositionChanging, PositionChanged);
        }

        public event EventHandler<ValueChangedEventArgs<Point>>? PositionChanging;
        public event EventHandler<ValueChangedEventArgs<Point>>? PositionChanged;

        public int Layer { get; }

        public IDPositionLayerObject(int layer = 0)
        {
            ID = s_idGenerator.UseID();
            Layer = layer;
        }

        public uint ID { get; }
    }
}
