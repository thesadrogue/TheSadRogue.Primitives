using System;
using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.SerializedTypes;

namespace SadRogue.Primitives.UnitTests.Serialization
{
    internal static class Comparisons
    {
        // Dictionary of object types mapping them to custom methods to use in order to determine equality.
        private static readonly Dictionary<Type, Func<object, object, bool>> _equalityMethods =
            new Dictionary<Type, Func<object, object, bool>>()
            {
                { typeof(ArrayView2D<int>), MapViewCompare<int> },
                { typeof(Area), MatchableCompare<IReadOnlyArea> },
                { typeof(AreaSerialized), AreaSerializedCompare },
                { typeof(BoundedRectangle), MatchableCompare<BoundedRectangle> },
                { typeof(Gradient), MatchableCompare<Gradient> },
                { typeof(GradientSerialized), GradientSerializedCompare },
                { typeof(Palette), MatchableCompare<Palette> },
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

        private static bool MatchableCompare<T>(object o1, object o2)
            where T : class, IMatchable<T>
            => ((T)o1).Matches((T?)o2);
        private static bool ElementWiseEquality<T>(IEnumerable<T> e1, IEnumerable<T> e2,
                                                   Func<T, T, bool>? compareFunc = null)
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

        private static bool MapViewCompare<T>(object o1, object o2)
        {
            var view1 = (IGridView<T>)o1;
            var view2 = (IGridView<T>)o2;

            if (ReferenceEquals(view1, view2))
                return true;

            if (view1.Width != view2.Width || view1.Height != view2.Height)
                return false;

            foreach (var pos in view1.Positions())
            {
                var val1 = view1[pos];
                var val2 = view2[pos];
                if (ReferenceEquals(val1, val2))
                    continue;

                if (val1 is null || val2 is null)
                    return false;

                if (!val1.Equals(val2))
                    return false;
            }

            return true;
        }
    }
}
