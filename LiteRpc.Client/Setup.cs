﻿namespace LiteRpc.Client
{
	using System;
	using System.Collections.Generic;
	using Castle.DynamicProxy;
	using System.Configuration;
using System.Net;

	public static class Setup
	{
		private static JsonRpcInterceptor interceptor;

		static Setup()
		{
			interceptor = new JsonRpcInterceptor();
			ServiceParams = new Dictionary<Type, ServiceInfo>();
		}

		internal static Dictionary<Type,  ServiceInfo> ServiceParams  { get; set; }

		public static T CreateProxyFor<T>() where T:class
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

			return proexyGen.CreateInterfaceProxyWithoutTarget<T>(interceptor);
		}

		public static T CreateProxyFor<T>(ICredentials credentials) where T : class
		{
			T result = CreateProxyFor<T>();
			ServiceParams[typeof(T)].Credential = credentials;
			return result;
		}


		public static object CreateProxyFor(Type type)
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

			return proexyGen.CreateInterfaceProxyWithoutTarget(type, interceptor);
		}

		public static object CreateProxyFor(Type type, ICredentials credentials)
		{
			var result = CreateProxyFor(type);
			ServiceParams[type].Credential = credentials;
			return result;
		}


	}
}
