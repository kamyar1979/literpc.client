namespace LiteRpc.Client
{
	using System;

	/// <summary>
	/// This attributes makes any meant interface the proxy/placeholder to some Json-Rpc web service.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface)]
	public class RpcAttribute : Attribute
	{
		/// <summary>
		/// The json-rpc service Uri.
		/// </summary>
		public string UriSettingsKey { get; set; }

		/// <summary>
		/// Json-Rpc domain/class/category name.
		/// </summary>
		public string DomainName { get; set; }
	}
	
	/// <summary>
	/// This attribute overrides the information of the parent interface to inner methods.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class RpcMethodAttribute : Attribute
	{
		/// <summary>
		/// The domain which is not identical to the parent interface RPC attribute.
		/// </summary>
		public string DomainName { get; set; }

		/// <summary>
		/// real RPC method name which may not be the same as this method name.
		/// </summary>
		public string MethodName { get; set; }
	}
}