using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Sdk;

namespace TheSadRogue.Primitives.UnitTests
{
    /// <summary>
    /// Improved member data attribute that can source data from IEnumerable&lt;ValueTuple&gt; in addition to
    /// IEnumerable&lt;object[]&gt;.
    /// </summary>
    //[CLSCompliant(false)]
    [DataDiscoverer("Xunit.Sdk.MemberDataDiscoverer", "xunit.core")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class MemberData2Attribute : MemberDataAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberDataAttribute"/> class.
        /// </summary>
        /// <param name="memberName">The name of the public static member on the test class that will provide the test data</param>
        /// <param name="parameters">The parameters for the member (only supported for methods; ignored for everything else)</param>
        public MemberData2Attribute(string memberName, params object[] parameters)
            : base(memberName, parameters) { }

        /// <inheritdoc/>
        protected override object[] ConvertDataItem(MethodInfo testMethod, object item)
        {
            if (item == null)
                return null;

            if (item is object[] array)
                return array;

            if (item is ITuple tuple)
            {
                List<object> objs = new List<object>(tuple.Length);
                for (int i = 0; i < tuple.Length; i++)
                    objs.Add(tuple[i]);

                return objs.ToArray();
            }

            throw new ArgumentException($"Property {MemberName} on {MemberType ?? testMethod.DeclaringType} yielded an item that is not an object[] or ITuple");
        }
    }

    /// <summary>
    /// Static/extension methods to help with creating test variables/enumerables for XUnit
    /// </summary>
    public static class TestUtils
    {
        public static IEnumerable<T> ToEnumerable<T>(this T obj)
        {
            yield return obj;
        }

        public static ValueTuple<T> ToValueTuple<T>(this T obj) => new ValueTuple<T>(obj);

        public static IEnumerable<ValueTuple<T>> ToValueTuples<T>(this IEnumerable<T> objs) => objs.Select(i => i.ToValueTuple());

        public static IEnumerable<(T1, T2)> Combinate<T1, T2>(this IEnumerable<T1> l1, IEnumerable<T2> l2)
        {
            foreach (var x in l1)
                foreach (var y in l2)
                    yield return (x, y);
        }

        public static IEnumerable<T> Enumerable<T>(params T[] objs) => objs;
    }
}
