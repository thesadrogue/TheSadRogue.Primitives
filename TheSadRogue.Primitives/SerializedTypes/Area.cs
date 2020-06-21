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
        {
            var area = new Area();

            foreach (var pos in serialized.Positions)
                area.Add(pos);

            return area;
        }

        public static implicit operator AreaSerialized(Area area)
            => new AreaSerialized() { Positions = new List<PointSerialized>(area.Positions.Cast<PointSerialized>()) };
    }
}
