using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utilities
{
	public static class Id
	{
		//TODO: set data type to be the same as the id in the engine
		public static int INVALID_ID => -1;
		public static bool IsValid(int id) => id != INVALID_ID;
	}

	public static class Utilities
	{
		public static float Epsilon => 0.00001f;

		public static bool IsTheSameAs(this float value, float other)
		{
			return Math.Abs(value - other) < Epsilon;
		}

		public static bool IsTheSameAs(this float? value, float? other)
		{
			if (!value.HasValue || !other.HasValue)
			{
				return false;
			}

			return Math.Abs(value.Value - other.Value) < Epsilon;
		}
	}
}
