using System;
using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives.SerializedTypes;

namespace SadRogue.Primitives.UnitTests.Serialization
{
    static class Comparisons
    {
        // Dictionary of object types mapping them to custom methods to use in order to determine equality.
        private static readonly Dictionary<Type, Func<object, object, bool>> _equalityMethods = new Dictionary<Type, Func<object, object, bool>>()
        {
            { typeof(AreaSerialized), AreaSerializedCompare },
            { typeof(GradientSerialized), GradientSerializedCompare },
            { typeof(PaletteSerialized), PaletteSerializedCompare }
        };

        public static Func<object, object, bool> GetComparisonFunc(object obj)
            => _equalityMethods.GetValueOrDefault(obj.GetType(), (o1, o2) => o1.Equals(o2))!;

        // Compares to AreaSerialized instances
        private static bool AreaSerializedCompare(object o1, object o2)
            => ElementWiseEquality(((AreaSerialized)o1).Positions, ((AreaSerialized)o2).Positions);

        private static bool PaletteSerializedCompare(object o1, object o2)
            => ElementWiseEquality(((PaletteSerialized)o1).Colors, ((PaletteSerialized)o2).Colors);

        private static bool GradientSerializedCompare(object o1, object o2)
            => ElementWiseEquality(((GradientSerialized)o1).Stops, ((GradientSerialized)o2).Stops);

        private static bool ElementWiseEquality<T>(IEnumerable<T> e1, IEnumerable<T> e2, Func<T, T, bool>? compareFunc = null)
        {
            compareFunc ??= (o1, o2) => o1?.Equals(o2) ?? o2 == null;

            var l1 = e1.ToList();
            var l2 = e2.ToList();

            if (l1.Count != l2.Count)
                return false;

            for (int i = 0; i < l1.Count; i++)
                if (!compareFunc(l1[i], l2[i]))
                    return false;

            return true;
        }
    }
}
