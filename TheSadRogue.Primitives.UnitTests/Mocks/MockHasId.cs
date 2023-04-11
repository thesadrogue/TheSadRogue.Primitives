namespace SadRogue.Primitives.UnitTests.Mocks
{
    internal class MyIDImpl : IHasID
    {
        private static readonly IDGenerator s_idGen = new IDGenerator();

        public MyIDImpl(int myInt)
        {
            ID = s_idGen.UseID();
            MyInt = myInt;
        }

        private int MyInt { get; }

        public uint ID { get; }

        public override string ToString() => "Thing " + MyInt;
    }
}
