namespace LiteRpc.Client
{
	using System;
	using Castle.DynamicProxy;

	public static class Setup
	{
		private static JsonRpcInterceptor interceptor;

		static Setup()
		{
			interceptor = new JsonRpcInterceptor();
		}

		public static T CreateProxyFor<T>() where T:class
		{			
			var proexyGen = new ProxyGenerator();
			return proexyGen.CreateInterfaceProxyWithoutTarget<T>(interceptor);
		}

		public static object CreateProxyFor(Type type)
		{
			var proexyGen = new ProxyGenerator();
			return proexyGen.CreateInterfaceProxyWithoutTarget(type, interceptor);
		}

	}
}
