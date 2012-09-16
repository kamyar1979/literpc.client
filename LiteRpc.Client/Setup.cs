namespace LiteRpc.Client
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Net;
	using Castle.DynamicProxy;

	public static class Setup
	{
		private static JsonRpcInterceptor interceptor;

		static Setup()
		{
			interceptor = new JsonRpcInterceptor();
			ServiceParams = new Dictionary<Type, ServiceInfo>();
		}

		internal static Dictionary<Type, ServiceInfo> ServiceParams { get; set; }

		public static T CreateProxyFor<T>() where T : class
		{
			if (Singletone<T>.Instance == null)
			{
				var proexyGen = new ProxyGenerator();
				var info = new ServiceInfo();

				if (Attribute.IsDefined(typeof(T), typeof(RpcAttribute)))
				{
					var rpcAttrib = Attribute.GetCustomAttribute(typeof(T), typeof(RpcAttribute), false) as RpcAttribute;
					if (!string.IsNullOrEmpty(rpcAttrib.UriSettingsKey))
					{
						info.Uri = new Uri(ConfigurationManager.AppSettings[rpcAttrib.UriSettingsKey]);
					}
					if (!string.IsNullOrEmpty(rpcAttrib.DomainName))
					{
						info.DomainName = rpcAttrib.DomainName;
					}
				}

				ServiceParams[typeof(T)] = info;

				Singletone<T>.SetInstance(proexyGen.CreateInterfaceProxyWithoutTarget<T>(interceptor));
			}
			return Singletone<T>.Instance;
		}

		public static T CreateProxyFor<T>(ICredentials credentials) where T : class
		{
			T result = CreateProxyFor<T>();
			ServiceParams[typeof(T)].Credential = credentials;
			return result;
		}

		public static T CreateProxyFor<T>(Uri uri, string domainName) where T : class
		{
			T result = CreateProxyFor<T>();
			ServiceParams[typeof(T)].Uri = uri;
			ServiceParams[typeof(T)].DomainName = domainName;
			return result;
		}

		public static T CreateProxyFor<T>(Uri uri, string domainName, ICredentials credentials) where T : class
		{
			T result = CreateProxyFor<T>();
			ServiceParams[typeof(T)].Uri = uri;
			ServiceParams[typeof(T)].DomainName = domainName;
			ServiceParams[typeof(T)].Credential = credentials;
			return result;
		}

		public static object CreateProxyFor(Type type)
		{			
			if (Singletone.InstanceOf(type) == null)
			{
				var proexyGen = new ProxyGenerator();
				var info = new ServiceInfo();

				if (Attribute.IsDefined(type, typeof(RpcAttribute)))
				{
					var rpcAttrib = Attribute.GetCustomAttribute(type, typeof(RpcAttribute), false) as RpcAttribute;
					if (!string.IsNullOrEmpty(rpcAttrib.UriSettingsKey))
					{
						info.Uri = new Uri(ConfigurationManager.AppSettings[rpcAttrib.UriSettingsKey]);
					}
					if (!string.IsNullOrEmpty(rpcAttrib.DomainName))
					{
						info.DomainName = rpcAttrib.DomainName;
					}
				}

				ServiceParams[type] = info;

				Singletone.SetInstance(proexyGen.CreateInterfaceProxyWithoutTarget(type, interceptor));
			}
			return Singletone.InstanceOf(type);
		}

		public static object CreateProxyFor(Type type, ICredentials credentials)
		{
			var result = CreateProxyFor(type);
			ServiceParams[type].Credential = credentials;
			return result;
		}

		public static object CreateProxyFor(Type type, Uri uri, string domainName)
		{
			var result = CreateProxyFor(type);
			ServiceParams[type].Uri = uri;
			ServiceParams[type].DomainName = domainName;
			return result;
		}

		public static object CreateProxyFor(Type type, Uri uri, string domainName, ICredentials credentials)
		{
			var result = CreateProxyFor(type);
			ServiceParams[type].Uri = uri;
			ServiceParams[type].DomainName = domainName;
			ServiceParams[type].Credential = credentials;
			return result;
		}
	}
}
