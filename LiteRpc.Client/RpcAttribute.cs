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
}