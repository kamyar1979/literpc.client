namespace LiteRpc.Client
{
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
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
			var rpcAttrib = invocation.Method.DeclaringType.GetCustomAttributes(typeof(RpcAttribute), false).First() as RpcAttribute;
			if (!rpc.ContainsKey(ConfigurationManager.AppSettings[rpcAttrib.UriSettingsKey]))
			{
				rpc[ConfigurationManager.AppSettings[rpcAttrib.UriSettingsKey]] = new JsonRpc(ConfigurationManager.AppSettings[rpcAttrib.UriSettingsKey]);
			}
			var name = invocation.Method.Name;
			invocation.ReturnValue = rpc[ConfigurationManager.AppSettings[rpcAttrib.UriSettingsKey]].DoRequest(string.Concat(rpcAttrib.DomainName, ".", name), invocation.Method.ReturnType, invocation.Arguments);
		}
	}
}