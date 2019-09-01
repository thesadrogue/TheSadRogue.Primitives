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

		public static IEnumerable<(T1, T2, T3)> Combinate<T1, T2, T3>(this IEnumerable<(T1 i1, T2 i2)> tuples, IEnumerable<T3> l2)
		{
			foreach (var (i1, i2) in tuples)
				foreach (var y in l2)
					yield return (i1, i2, y);
		}

		public static void AssertElementEquals<T>(params IReadOnlyList<T>[] lists)
		{
			var list1 = lists[0];
			int length = list1.Count;
			foreach (var list in lists.Skip(1))
				Assert.Equal(length, list.Count);

			foreach (var list in lists.Skip(1))
			{
				for (int i = 0; i < length; i++)
					Assert.Equal(list1[i], list[i]);
			}
		}

		public static void AssertElementEquals<T>(params T[][] lists)
		{
			var list1 = lists[0];
			int length = list1.Length;
			foreach (var list in lists.Skip(1))
				Assert.Equal(length, list.Length);

			foreach (var list in lists.Skip(1))
			{
				for (int i = 0; i < length; i++)
					Assert.Equal(list1[i], list[i]);
			}
		}

		public static IEnumerable<T> Enumerable<T>(params T[] objs) => objs;
	}
}
