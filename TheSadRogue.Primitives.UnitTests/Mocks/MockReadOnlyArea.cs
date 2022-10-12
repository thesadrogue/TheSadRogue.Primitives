using System.Collections;
using System.Collections.Generic;

namespace SadRogue.Primitives.UnitTests.Mocks
{
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
