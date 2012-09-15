namespace LiteRpc.Client
{
	using System;

	internal class Singletone<T>
	{
		private static T instance;

		public static T Instance
		{
			get
			{
				return instance;
			}
		}

		public static void SetInstance(T value)
		{
			instance = value;
		}
	}

	internal class Singletone
	{
		private static object instance;

		public static object Instance
		{
			get
			{
				return instance;
			}
		}

		public static void SetInstance(object value)
		{
			instance = value;
		}
	}

}
