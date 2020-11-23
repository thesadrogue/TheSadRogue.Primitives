using System;
using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.SerializedTypes;
using SadRogue.Primitives.SerializedTypes.GridViews;

namespace SadRogue.Primitives.UnitTests.Serialization
{
    internal static class Comparisons
    {
        // Dictionary of object types mapping them to custom methods to use in order to determine equality.
        private static readonly Dictionary<Type, Func<object, object, bool>> _equalityMethods =
            new Dictionary<Type, Func<object, object, bool>>
            {
                [typeof(ArrayView2D<int>)] = CastFromObject<IGridView<int>>(GridViewCompare),
                [typeof(ArrayView<int>)] = CastFromObject<IGridView<int>>(GridViewCompare),
                [typeof(ArrayViewSerialized<int>)] = CastFromObject<ArrayViewSerialized<int>>(ArrayViewSerializedCompare),
                [typeof(Area)] = CastFromObject<IReadOnlyArea>(MatchableCompare),
                [typeof(AreaSerialized)] = CastFromObject<AreaSerialized>(AreaSerializedCompare),
                [typeof(BoundedRectangle)] = CastFromObject<BoundedRectangle>(MatchableCompare),
                //[typeof(DiffAwareGridView<int>)] = CastFromObject<DiffAwareGridView<int>>(DiffAwareGridViewCompare),
                //[typeof(DiffAwareGridViewSerialized<int>)] = CastFromObject<DiffAwareGridViewSerialized<int>>(DiffAwareGridViewSerializedCompare),
                [typeof(Diff<int>)] = CastFromObject<Diff<int>>(DiffCompare),
                [typeof(DiffSerialized<int>)] = CastFromObject<DiffSerialized<int>>(DiffSerializedCompare),
                [typeof(Gradient)] = CastFromObject<Gradient>(MatchableCompare),
                [typeof(GradientSerialized)] = CastFromObject<GradientSerialized>(GradientSerializedCompare),
                [typeof(Palette)] = CastFromObject<Palette>(MatchableCompare),
                [typeof(PaletteSerialized)] = CastFromObject<PaletteSerialized>(PaletteSerializedCompare)
            };

        public static Func<object, object, bool> GetComparisonFunc(object obj)
            => _equalityMethods.GetValueOrDefault(obj.GetType(), (o1, o2) => o1.Equals(o2))!;

        private static Func<object, object, bool> CastFromObject<T>(Func<T, T, bool> func)
            => (o1, o2) => func((T) o1, (T) o2);

        // Compares two AreaSerialized instances
        private static bool AreaSerializedCompare(AreaSerialized o1, AreaSerialized o2)
            => ElementWiseEquality(o1.Positions, o2.Positions);

        /*
        private static bool DiffAwareGridViewCompare<T>(DiffAwareGridView<T> o1, DiffAwareGridView<T> o2)
            where T : struct
            => GridViewCompare(o1.BaseGrid, o2.BaseGrid) && ElementWiseEquality(o1.Diffs, o2.Diffs) &&
               o1.AutoCompress == o2.AutoCompress && o1.CurrentDiffIndex == o2.CurrentDiffIndex;

        private static bool DiffAwareGridViewSerializedCompare<T>(DiffAwareGridViewSerialized<T> o1,
                                                                  DiffAwareGridViewSerialized<T> o2)
            where T : struct
            => o1.AutoCompress == o2.AutoCompress && ElementWiseEquality(o1.Diffs, o2.Diffs) &&
               GridViewCompare(o1.BaseGrid, o2.BaseGrid) && o1.CurrentDiffIndex == o2.CurrentDiffIndex;
        */


        // This comparison doesn't work in all cases.  The serialization actually checks to see if the re-serialized
        // version is compressed; so if the original was compressed but not KNOWN to be as such (so marked as false)
        // then the comparison will fail.  This is relatively rare, however, and our test cases avoid it.
        private static bool DiffCompare<T>(Diff<T> o1, Diff<T> o2)
            where T : struct
            => ElementWiseEquality(o1.Changes, o2.Changes) && o1.IsCompressed == o2.IsCompressed;

        private static bool DiffSerializedCompare<T>(DiffSerialized<T> o1, DiffSerialized<T> o2)
            where T : struct
            => ElementWiseEquality(o1.Changes, o2.Changes);

        private static bool PaletteSerializedCompare(PaletteSerialized o1, PaletteSerialized o2)
            => ElementWiseEquality(o1.Colors, o2.Colors);

        private static bool GradientSerializedCompare(GradientSerialized o1, GradientSerialized o2)
            => ElementWiseEquality(o1.Stops, o2.Stops);

        private static bool ArrayViewSerializedCompare<T>(ArrayViewSerialized<T> o1, ArrayViewSerialized<T> o2)
            => ElementWiseEquality(o1.Data, o2.Data);

        private static bool MatchableCompare<T>(T o1, T o2)
            where T : class, IMatchable<T>
            => o1.Matches(o2);

        private static bool GridViewCompare<T>(IGridView<T> view1, IGridView<T> view2)
        {
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
    }
}
