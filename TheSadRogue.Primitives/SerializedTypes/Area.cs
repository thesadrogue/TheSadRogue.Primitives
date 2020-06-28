using System;
using System.Collections.Generic;
using System.Linq;

namespace SadRogue.Primitives.SerializedTypes
{
    /// <summary>
    /// Serializable (pure-data) object representing an <see cref="Area"/>.
    /// </summary>
    [Serializable]
    public struct AreaSerialized
    {
        public List<PointSerialized> Positions;

        public static implicit operator Area(AreaSerialized serialized)
            => new Area(serialized.Positions.Select(pos => (Point)pos));

        public static implicit operator AreaSerialized(Area area)
            => new AreaSerialized() { Positions = area.Positions.Select(p => (PointSerialized)p).ToList() };
    }
}
