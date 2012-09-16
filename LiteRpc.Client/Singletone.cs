namespace LiteRpc.Client
{
	using System;
	using System.Collections.Generic;

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
		private static Dictionary<Type, object> instanceContainer = new Dictionary<Type, object>();

		public static object InstanceOf(Type t)
		{
			if (instanceContainer.ContainsKey(t))
			{
				return instanceContainer[t];
			}
			else
			{
				return null;
			}
		}

		public static void SetInstance(object value)
		{
			instanceContainer[value.GetType()] = value;
		}
	}
}
