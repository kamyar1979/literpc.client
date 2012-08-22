namespace LiteRpc.Client
{
	using System;
	using System.Net;

	internal class ServiceInfo
	{
		public string DomainName { get; set; }
		public Uri Uri { get; set; }
		public ICredentials Credential { get; set; }

	}
}
