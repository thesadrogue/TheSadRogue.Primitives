using System.Collections;
using System.Collections.Generic;

namespace SadRogue.Primitives.UnitTests.Mocks
{
    /// <summary>
    /// Mock area class which counts the number of times it's enumerated via various methods.
    /// </summary>
    public class ReadOnlyAreaCountEnumerations : IReadOnlyArea
    {
        public readonly List<Point> Points = new List<Point>{ (1, 2), (3, 4) };
        public int GetEnumeratorCount;
        public int GetIndexCount;

        // This should almost _never_ be public in production, but allows us to test code for now
        public bool UseIndexEnumeration { get; set; }


        public bool Matches(IReadOnlyArea? other) => throw new System.NotImplementedException();

        public IEnumerator<Point> GetEnumerator()
        {
            GetEnumeratorCount++;
            return Points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            GetEnumeratorCount++;
            return Points.GetEnumerator();
        }

        ReadOnlyAreaPositionsEnumerator IReadOnlyArea.GetEnumerator()
        {
            GetEnumeratorCount++;
            return new ReadOnlyAreaPositionsEnumerator(this);
        }

        public Rectangle Bounds => throw new System.NotImplementedException();

        public int Count => Points.Count;

        public Point this[int index]
        {
            get
            {
                GetIndexCount++;
                return Points[index];
            }
        }

        public bool Contains(IReadOnlyArea area) => throw new System.NotImplementedException();

        public bool Contains(Point position) => throw new System.NotImplementedException();

        public bool Contains(int positionX, int positionY) => throw new System.NotImplementedException();

        public bool Intersects(IReadOnlyArea area) => throw new System.NotImplementedException();

        public void ClearCounts()
        {
            GetEnumeratorCount = GetIndexCount = 0;
        }
    }

    public class MockReadOnlyAreaNoUseIndex : IReadOnlyArea
    {
        public bool Matches(IReadOnlyArea? other) => throw new System.NotImplementedException();

        public IEnumerator<Point> GetEnumerator() => throw new System.NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Rectangle Bounds => throw new System.NotImplementedException();

        public int Count => throw new System.NotImplementedException();

        public Point this[int index] => throw new System.NotImplementedException();

        public bool Contains(IReadOnlyArea area) => throw new System.NotImplementedException();

        public bool Contains(Point position) => throw new System.NotImplementedException();

        public bool Contains(int positionX, int positionY) => throw new System.NotImplementedException();

        public bool Intersects(IReadOnlyArea area) => throw new System.NotImplementedException();
    }

    public class MockReadOnlyAreaSetUseIndex : IReadOnlyArea
    {
        public bool UseIndexEnumeration => true;

        public bool Matches(IReadOnlyArea? other) => throw new System.NotImplementedException();

        public IEnumerator<Point> GetEnumerator() => throw new System.NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Rectangle Bounds => throw new System.NotImplementedException();

        public int Count => throw new System.NotImplementedException();

        public Point this[int index] => throw new System.NotImplementedException();

        public bool Contains(IReadOnlyArea area) => throw new System.NotImplementedException();

        public bool Contains(Point position) => throw new System.NotImplementedException();

        public bool Contains(int positionX, int positionY) => throw new System.NotImplementedException();

        public bool Intersects(IReadOnlyArea area) => throw new System.NotImplementedException();
    }
}
