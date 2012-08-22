namespace LiteRpc.Client
{
	using System.Collections.Generic;
	using Castle.DynamicProxy;

	/// <summary>
	/// Json-Rpc client helper class. Makes the required proxy and injects remote method into defined interface.
	/// </summary>
	public class JsonRpcInterceptor : IInterceptor
	{
		private static Dictionary<string, JsonRpc> rpc;

		/// <summary>
		/// Initializes inner static fileds of the class for the first time..
		/// </summary>
		static JsonRpcInterceptor()
		{
			rpc = new Dictionary<string, JsonRpc>();
		}

		/// <summary>
		/// Intercepts method call on Json-Rpc interfaces and forwards them to remote machine. Also forwards returned answer to the proxy method return value.
		/// </summary>
		/// <param name="invocation">object caontaining IoC method interceptor info.</param>
		public void Intercept(IInvocation invocation)
		{
			string key = Setup.ServiceParams[invocation.Method.DeclaringType].Uri.ToString();
			if (!rpc.ContainsKey(key))
			{
				rpc[key] = new JsonRpc(Setup.ServiceParams[invocation.Method.DeclaringType]);
			}
			var name = invocation.Method.Name;
			invocation.ReturnValue = rpc[key].DoRequest(string.Concat(Setup.ServiceParams[invocation.Method.DeclaringType].DomainName, ".", name), invocation.Method.ReturnType, invocation.Arguments);
		}
	}
}