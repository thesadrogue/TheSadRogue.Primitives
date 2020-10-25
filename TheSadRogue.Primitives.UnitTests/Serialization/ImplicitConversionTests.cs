using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests.Serialization
{
    public class ImplicitConversionTests
    {
        // Required to be in-class so we just forward
        public static IEnumerable<object> AllNonExpressiveTypes = TestData.AllNonExpressiveTypes;

        private readonly ITestOutputHelper _output;

        public ImplicitConversionTests(ITestOutputHelper output) => _output = output;

        private static MethodInfo? FindImplicitConversionOnType(Type typeToScan, Type from, Type to)
        {
            var methods = typeToScan.GetMethods();
            foreach (var method in methods)
            {
                if (method.IsStatic && method.Name == "op_Implicit" && method.ReturnType == to)
                {
                    // Check return type
                    var methodParams = method.GetParameters();
                    if (methodParams.Length != 1)
                        continue;

                    if (methodParams[0].ParameterType == from)
                        return method;
                }
            }

            return null;
        }
        private static MethodInfo? FindImplicitConversion(Type from, Type to)
        {
            var conversion = FindImplicitConversionOnType(from, from, to);
            return conversion ?? FindImplicitConversionOnType(to, from, to);
        }

        [Theory]
        [MemberDataEnumerable(nameof(AllNonExpressiveTypes))]
        public void RegularToSerializedConversion(object original)
        {
            var originalType = original.GetType();
            var expressiveType = TestData.RegularToExpressiveTypes[originalType];
            var equalityFunc = Comparisons.GetComparisonFunc(original);

            _output.WriteLine($"Testing expressive conversion from {originalType} to {expressiveType}.");

            // Retrieve each implicit conversion operator.  We must use reflection because there is no way to add type
            // constraints that tell the compiler that the object must have these implicit conversions without adding
            // unnecessary things to the source code.  The conversion operators must be there or it's an error
            // in the test case, so we check both types.
            var toExpressive = FindImplicitConversion(originalType, expressiveType);
            var toOriginal = FindImplicitConversion(expressiveType, originalType);

            // Make sure the conversion functions exist
            TestUtils.NotNull(toExpressive);
            TestUtils.NotNull(toOriginal);

            // Convert to expressive type and assert it returns a valid object of the correct type
            var expressive = toExpressive.Invoke(null, new[] { original });
            TestUtils.NotNull(expressive);
            Assert.Equal(expressiveType, expressive.GetType());

            // Convert from the expressive type back to the original and assert it returns a valid object
            // of the correct type
            var converted = toOriginal.Invoke(null, new[] { expressive });
            TestUtils.NotNull(converted);
            Assert.Equal(originalType, converted.GetType());

            // Assert the type is equivalent after conversion as compared to before
            Assert.True(equalityFunc(original, converted));
        }
    }
}
