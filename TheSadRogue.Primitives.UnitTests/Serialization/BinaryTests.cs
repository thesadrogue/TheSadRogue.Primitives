using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests.Serialization
{
    public class BinaryTests
    {
        // Tests require the properties used for theories to be in the same class, so we refer to TestData
        public static IEnumerable<object> BinarySerializableTypes => TestData.BinarySerializableTypes;

        [Theory]
        [MemberDataEnumerable(nameof(BinarySerializableTypes))]
        public void SerializeToDeserializeEquivalence(object objToSerialize)
        {
            Func<object, object, bool> equalityFunc = Comparisons.GetComparisonFunc(objToSerialize);

            string name = $"{objToSerialize.GetType().FullName}.bin";

            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(name, FileMode.Create, FileAccess.Write))
                formatter.Serialize(stream, objToSerialize);

            object reSerialized;
            using (var stream = new FileStream(name, FileMode.Open, FileAccess.Read))
                reSerialized = formatter.Deserialize(stream);

            File.Delete(name);
            Assert.True(equalityFunc(objToSerialize, reSerialized));
        }
    }
}
