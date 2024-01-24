using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kinetix.Internal.Utils
{
    public static class MemorySizeUtils	
	{
#if UNITY_64 || UNITY_EDITOR_64
		public const int POINTER_SIZE = 8;
#else
		public const int POINTER_SIZE = 4;
#endif

		public const long DICTIONARY_HEADER_VALUE = 12;
		public const long DICTIONARY_HEADER_OBJECT = 12;

		/// <remarks>
		/// List`1[T] and Dictionary doesn't unclude cached capacity
		/// </remarks>
		public static long MemorySize<T>(this List<T> value) => MemorySize(value, typeof(T));
		/// <remarks>
		/// List`1[T] and Dictionary doesn't unclude cached capacity
		/// </remarks>
		public static long MemorySize<T>(this T value) => MemorySize(value, typeof(T));
		public static long MemorySize(this object value, Type type)
		{
			if (typeof(Array).IsAssignableFrom(type))
			{
				Array enumer = value as Array;
				Type itemType = type.GetElementType();
				bool isValueType = itemType.IsValueType;

				//lenght + _version + item size
				return sizeof(int) + sizeof(int) + enumer.Cast<object>().Sum(SumItems);
				long SumItems(object v) => (isValueType ? 0 : POINTER_SIZE) + MemorySize(v, itemType);
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
			{
				IEnumerable enumer = value as IEnumerable;
				Type itemType = type.GenericTypeArguments[0];
				bool isValueType = itemType.IsValueType;

				//Array pointer + lenght + _version + item size
				return POINTER_SIZE + sizeof(int) +sizeof(int) + enumer.Cast<object>().Sum(SumItems);
				long SumItems(object v) => (isValueType ? 0 : POINTER_SIZE) + MemorySize(v, itemType);
			}
			
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
			{
				//https://stackoverflow.com/questions/37578559/memory-usage-of-dictionaries-in-c-sharp#:~:text=if%20you%20have%20value%20types,so%20there%20is%20wasted%20space.
				Type keyType = type.GenericTypeArguments[0];
				bool keyIsValueType = keyType.IsValueType;
				Type valueType = type.GenericTypeArguments[1];
				bool valueIsValueType = keyType.IsValueType;
				IDictionary dictionary = value as IDictionary;

				int count = dictionary.Count;
				return
					POINTER_SIZE + //KeyCollection
					POINTER_SIZE + //ValueCollection
					POINTER_SIZE + count * sizeof(int) + //Buckets
					POINTER_SIZE + count * (sizeof(int) + sizeof(int)) +  //Entries (hash + next)
						dictionary.Keys.Cast<object>().Sum(SumKeys) +     //Entries (keys)
						dictionary.Values.Cast<object>().Sum(SumValues) + //Entries (values)
					sizeof(int)  + //Count
					sizeof(int)  + //Version
					sizeof(int)  + //freeList
					sizeof(int)  + //freeCount
					POINTER_SIZE   //_syncRoot
				;
			
				long SumKeys(object v) => (keyIsValueType ? 0 : POINTER_SIZE) + MemorySize(v, keyType);
				long SumValues(object v) => (valueIsValueType ? 0 : POINTER_SIZE) + MemorySize(v, valueType);
			}

			if (value is Enum _)					return MemorySizeOfEnum(type);
			if (value is string str)				return MemorySize(str);
			if (value is bool b)					return b.MemorySize();
			if (value is byte by)					return by.MemorySize();
			if (value is short s)					return s.MemorySize();
			if (value is int i)						return i.MemorySize();
			if (value is long l)					return l.MemorySize();
			if (value is ushort us)					return us.MemorySize();
			if (value is uint ui)					return ui.MemorySize();
			if (value is ulong ul)					return ul.MemorySize();
			if (value is float f)					return f.MemorySize();
			if (value is double d)					return d.MemorySize();
			if (value is decimal de)				return de.MemorySize();
			if (value is Vector2 v2)				return v2.MemorySize();
			if (value is Vector3 v3)				return v3.MemorySize();
			if (value is Vector4 v4)				return v4.MemorySize();
			if (value is Quaternion q)				return q.MemorySize();
			if (value is IComputeMemorySize mem)	return mem.MemorySize;

			KinetixLogger.LogError("Memory", "Couldn't find type " + type.FullName, true);

			return 0;
		}

		private static long MemorySizeOfEnum(Type enumType)
		{
			Type t = Enum.GetUnderlyingType(enumType);
			if (t == typeof(bool)) return MemorySize(default(bool));
			if (t == typeof(byte)) return MemorySize(default(byte));
			if (t == typeof(short)) return MemorySize(default(short));
			if (t == typeof(int)) return MemorySize(default(int));
			if (t == typeof(long)) return MemorySize(default(long));
			if (t == typeof(ushort)) return MemorySize(default(ushort));
			if (t == typeof(uint)) return MemorySize(default(uint));
			if (t == typeof(ulong)) return MemorySize(default(ulong));

			return 0;
		}

		public static long MemorySize(this string value) => 20 + (value.Length / 2) * 4;
		public static long MemorySize(this bool value) => sizeof(bool) + MemorySize(value);
		public static long MemorySize(this byte _) => sizeof(byte);
		public static long MemorySize(this short _) => sizeof(short);
		public static long MemorySize(this int _) => sizeof(int);
		public static long MemorySize(this long _) => sizeof(long);
		public static long MemorySize(this ushort _) => sizeof(ushort);
		public static long MemorySize(this uint _) => sizeof(uint);
		public static long MemorySize(this ulong _) => sizeof(ulong);
		public static long MemorySize(this float _) => sizeof(float);
		public static long MemorySize(this double _) => sizeof(double);
		public static long MemorySize(this decimal _) => sizeof(decimal);
		public static long MemorySize(this Vector2 _) => 2L * sizeof(float);
		public static long MemorySize(this Vector3 _) => 3L * sizeof(float);
		public static long MemorySize(this Vector4 _) => 4L * sizeof(float);
		public static long MemorySize(this Quaternion _) => 4L * sizeof(float);

		public static long MemorySize(this bool? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this byte? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this short? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this int? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this long? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this ushort? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this uint? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this ulong? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this float? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this double? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this decimal? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this Vector2? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this Vector3? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this Vector4? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize(this Quaternion? value) => sizeof(bool) + MemorySize(value.GetValueOrDefault());
		public static long MemorySize<T>(this T? value) where T : struct, IComputeMemorySize
			=> sizeof(bool) + value.GetValueOrDefault().MemorySize;
	}
}
