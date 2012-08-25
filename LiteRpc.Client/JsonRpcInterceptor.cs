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
			string domainName = Setup.ServiceParams[invocation.Method.DeclaringType].DomainName;
			string methodName = invocation.Method.Name;
			if (RpcMethodAttribute.IsDefined(invocation.Method, typeof(RpcMethodAttribute)))
			{
				var attribute = RpcMethodAttribute.GetCustomAttribute(invocation.Method, typeof(RpcMethodAttribute)) as RpcMethodAttribute;
				if (!string.IsNullOrEmpty(attribute.DomainName))
					domainName = attribute.DomainName;
				if (!string.IsNullOrEmpty(attribute.MethodName))
					methodName = attribute.MethodName;
			}
			invocation.ReturnValue = rpc[key].DoRequest(string.Concat(domainName, ".", methodName), invocation.Method.ReturnType, invocation.Arguments);			
		}
	}
}