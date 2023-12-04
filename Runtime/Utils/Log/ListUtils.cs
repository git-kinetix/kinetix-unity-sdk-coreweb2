using System;
using System.Collections.Generic;

namespace Kinetix.Internal.Utils
{
    public static class ListUtils
	{
		/// <summary>
		/// Get the index of closest value in list
		/// </summary>
		public static int IndexOfClosest(this IList<double> values, double value)
		{
			double difference;
			double lowest = double.MaxValue;
			int iLowest = -1;
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				difference = Math.Abs(value - values[i]);
				if (difference < lowest)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}

		/// <summary>
		/// Get the index of closest value in list
		/// </summary>
		public static int IndexOfClosest(this IList<float> values, float value)
		{
			float difference;
			float lowest = float.MaxValue;
			int iLowest = -1;
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				difference = Math.Abs(value - values[i]);
				if (difference < lowest)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}

		/// <summary>
		/// Get the index of closest value in list
		/// </summary>
		public static int IndexOfClosest(this IList<decimal> values, decimal value)
		{
			decimal difference;
			decimal lowest = decimal.MaxValue;
			int iLowest = -1;
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				difference = Math.Abs(value - values[i]);
				if (difference < lowest)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}

		/// <summary>
		/// Get the index of closest value in list
		/// </summary>
		public static int IndexOfClosest(this IList<long> values, long value)
		{
			long difference;
			long lowest = long.MaxValue;
			int iLowest = -1;
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				difference = Math.Abs(value - values[i]);
				if (difference < lowest)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}

		/// <summary>
		/// Get the index of closest value in list
		/// </summary>
		public static int IndexOfClosest(this IList<int> values, int value)
		{
			int difference;
			int lowest = int.MaxValue;
			int iLowest = -1;
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				difference = Math.Abs(value - values[i]);
				if (difference < lowest)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}

		//------------------------------
		// Inferior
		//------------------------------

		/// <summary>
		/// Get the index of closest value in list which isn't higher than provided value
		/// </summary>
		public static int IndexOfInferiorClosest(this IList<double> values, double value)
		{
			double difference;
			double lowest = double.MaxValue;
			int iLowest = 0;
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				difference = value - values[i];
				if (difference < lowest && difference >= 0)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}

		/// <summary>
		/// Get the index of closest value in list which isn't higher than provided value
		/// </summary>
		public static int IndexOfInferiorClosest(this IList<float> values, float value)
		{
			float difference;
			float lowest = float.MaxValue;
			int iLowest = 0;
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				difference = value - values[i];
				if (difference < lowest && difference >= 0)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}

		/// <summary>
		/// Get the index of closest value in list which isn't higher than provided value
		/// </summary>
		public static int IndexOfInferiorClosest(this IList<decimal> values, decimal value)
		{
			decimal difference;
			decimal lowest = decimal.MaxValue;
			int iLowest = 0;
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				difference = value - values[i];
				if (difference < lowest && difference >= 0)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}


		/// <summary>
		/// Get the index of closest value in list which isn't higher than provided value
		/// </summary>
		public static int IndexOfInferiorClosest(this IList<long> values, long value)
		{
			long difference;
			long lowest = long.MaxValue;
			int iLowest = 0;
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				difference = value - values[i];
				if (difference < lowest && difference >= 0)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}


		/// <summary>
		/// Get the index of closest value in list which isn't higher than provided value
		/// </summary>
		public static int IndexOfInferiorClosest(this IList<int> values, int value)
		{
			int difference;
			int lowest = int.MaxValue;
			int iLowest = 0;
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				difference = value - values[i];
				if (difference < lowest && difference >= 0)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}

		//------------------------------
		// Superior
		//------------------------------

		/// <summary>
		/// Get the index of closest value in list which isn't lower than provided value
		/// </summary>
		public static int IndexOfSuperiorClosest(this IList<double> values, double value)
		{
			double difference;
			double lowest = double.MaxValue;
            int count = values.Count;
            int iLowest = count;
			for (int i = 0; i < count; i++)
			{
				difference = values[i] - value;
				if (difference < lowest && difference >= 0)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}

		/// <summary>
		/// Get the index of closest value in list which isn't lower than provided value
		/// </summary>
		public static int IndexOfSuperiorClosest(this IList<float> values, float value)
		{
			float difference;
			float lowest = float.MaxValue;
			int count = values.Count;
			int iLowest = count;
			for (int i = 0; i < count; i++)
			{
				difference = values[i] - value;
				if (difference < lowest && difference >= 0)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}

		/// <summary>
		/// Get the index of closest value in list which isn't lower than provided value
		/// </summary>
		public static int IndexOfSuperiorClosest(this IList<decimal> values, decimal value)
		{
			decimal difference;
			decimal lowest = decimal.MaxValue;
			int count = values.Count;
			int iLowest = count;
			for (int i = 0; i < count; i++)
			{
				difference = values[i] - value;
				if (difference < lowest && difference >= 0)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}


		/// <summary>
		/// Get the index of closest value in list which isn't lower than provided value
		/// </summary>
		public static int IndexOfSuperiorClosest(this IList<long> values, long value)
		{
			long difference;
			long lowest = long.MaxValue;
			int count = values.Count;
			int iLowest = count;
			for (int i = 0; i < count; i++)
			{
				difference = values[i] - value;
				if (difference < lowest && difference >= 0)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}


		/// <summary>
		/// Get the index of closest value in list which isn't lower than provided value
		/// </summary>
		public static int IndexOfSuperiorClosest(this IList<int> values, int value)
		{
			int difference;
			int lowest = int.MaxValue;
			int count = values.Count;
			int iLowest = count;
			for (int i = 0; i < count; i++)
			{
				difference = values[i] - value;
				if (difference < lowest && difference >= 0)
				{
					iLowest = i;
					lowest = difference;
				}
			}

			return iLowest;
		}
	}
}
