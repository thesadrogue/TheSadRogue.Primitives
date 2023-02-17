﻿using System;

namespace SadRogue.Primitives.UnitTests.Mocks
{
    public class MockPositionableSpatialMapItem : IHasID, IHasLayer, IPositionable
    {
        private static readonly IDGenerator _idGen = new IDGenerator();

        public MockPositionableSpatialMapItem(int layer, Point position)
        {
            ID = _idGen.UseID();
            Layer = layer;
            _position = position;
        }

        public uint ID { get; }
        public int Layer { get; }

        private Point _position;

        public Point Position
        {
            get => _position;
            set
            {
                this.SafelySetProperty(ref _position, value, PositionChanged);
            }
        }

        public event EventHandler<ValueChangedEventArgs<Point>>? PositionChanged;

        public override string ToString() => $"[{ID}, {Layer}]";
    }
}