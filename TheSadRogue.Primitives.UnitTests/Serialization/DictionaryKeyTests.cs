using System.Collections.Generic;
using Newtonsoft.Json;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests.Serialization
{
    public class DictionaryKeyTests
    {
        // Settings used for serialization/deserialization.  Ensures that things stored as polymorphic types
        // deserialize properly
        private static readonly JsonSerializerSettings s_settings =
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        // Tests require the properties used for theories to be in the same class, so we refer to TestData
        public static IEnumerable<object> NonExpressiveSerializableObjectTypes => TestData.NonExpressiveSerializableObjectTypes;

        [Theory]
        [MemberDataEnumerable(nameof(NonExpressiveSerializableObjectTypes))]
        public void JsonObjectsSerializeAsDictionaryKeys(object obj)
        {
            var dict = new Dictionary<object, bool> { { obj, true } };

            var json = JsonConvert.SerializeObject(dict, s_settings);
            var dict2 = JsonConvert.DeserializeObject<Dictionary<object, bool>>(json, s_settings);

            Assert.NotNull(dict2);
            Assert.Equal(dict.Keys, dict2.Keys);
            foreach (object key in dict.Keys)
                Assert.Equal(dict[key], dict2[key]);
        }
    }
}
