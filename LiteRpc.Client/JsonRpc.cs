namespace LiteRpc.Client
{
	using System;
	using System.Net;
	using System.Text;
	using ServiceStack.Text;


	/// <summary>
	/// JsonRpc core class.It makes requests and send to the server and gets the response.
	/// </summary>
	internal class JsonRpc
	{
		private const string MIME_JSON = "application/json";

		private WebClient client;
		private WebHeaderCollection headers;

		/// <summary>
		/// The address of the json-rpc server
		/// </summary>
		public string Uri
		{
			get;
			set;
		}

		/// <summary>
		/// Username/password of the service, when the authentication is basic.
		/// </summary>
		public ICredentials Credentials
		{
			get;
			set;
		}

		/// <summary>
		/// Json-Rpc domain name, for example in ticket.get, the domain name is tikcet.
		/// </summary>
		public string DomainName
		{
			get;
			set;
		}

		/// <summary>
		/// Intializes a new instance of the class.
		/// </summary>
		public JsonRpc()
		{
			this.headers = new WebHeaderCollection();
		}

		/// <summary>
		/// Initializes a new instance of the class, getting the required uri parameter.
		/// </summary>
		/// <param name="uri">The address of the service</param>
		public JsonRpc(string uri)
			: this()
		{
			this.Uri = uri;
		}

		/// <summary>
		/// Initializes a new instance of the class, getting the required uri parameter, and username/password pair.
		/// </summary>
		/// <param name="uri">The address of the service</param>
		/// <param name="username">username for basic authentication</param>
		/// <param name="password">password for basic authentication</param>
		public JsonRpc(string uri, string username, string password)
			: this(uri)
		{
			this.Credentials = new NetworkCredential(username, password);
		}

		public JsonRpc(ServiceInfo serviceInfo)
			: this()
		{
			this.Uri = serviceInfo.Uri.ToString();
			this.Credentials = serviceInfo.Credential;
			this.DomainName = serviceInfo.DomainName;
		}

		/// <summary>
		/// The Core method which sends the request to the http server and returns the result and parses it into object.
		/// </summary>
		/// <param name="name">Method name as in json-rpc mentioned.</param>
		/// <param name="args">Argument list to be sent to the remote method.</param>
		/// <returns>Parsed return value, as in 'result' property of the json-rpc spec.</returns>
		internal object DoRequest(string name, params object[] args)
		{
			using (this.client = new WebClient())
			{
				client.Headers[HttpRequestHeader.Cookie] = headers[HttpResponseHeader.SetCookie];
				client.Headers[HttpRequestHeader.ContentType] = MIME_JSON;
				client.Encoding = Encoding.UTF8;

				this.client.Credentials = this.Credentials;

				dynamic json = JsonSerializer.DeserializeFromString<object>(this.client.UploadString(this.Uri, JsonSerializer.SerializeToString(new { method = name, @params = args })));

				if (!string.IsNullOrEmpty(this.client.ResponseHeaders[HttpResponseHeader.SetCookie]))
				{
					this.headers[HttpResponseHeader.SetCookie] = this.client.ResponseHeaders[HttpResponseHeader.SetCookie];
				}


				if (json.error == null)
				{
					return json.result;
				}
				else
				{
					throw new ApplicationException(json.error);
				}
			}
		}

		/// <summary>
		/// The Core method which sends the request to the http server and returns the result and parses it into object.
		/// </summary>
		/// <param name="name">Method name as in json-rpc mentioned.</param>
		/// <param name="args">Argument list to be sent to the remote method.</param>
		/// <returns>Parsed return value, as in 'result' property of the json-rpc spec.</returns>
		internal T DoRequest<T>(string name, params object[] args)
		{
			using (this.client = new WebClient())
			{
				client.Headers[HttpRequestHeader.Cookie] = headers[HttpResponseHeader.SetCookie];
				client.Headers[HttpRequestHeader.ContentType] = MIME_JSON;
				client.Encoding = Encoding.UTF8;
				var cookies = new CookieContainer();

				this.client.Credentials = this.Credentials;

				var json = new JsonRpcResult<T>();

				json = JsonSerializer.DeserializeFromString<JsonRpcResult<T>>(this.client.UploadString(this.Uri, JsonSerializer.SerializeToString(new { method = name, @params = args })));
				if (this.headers == null)
				{
					this.headers = new WebHeaderCollection();
				}

				if (!string.IsNullOrEmpty(this.client.ResponseHeaders[HttpResponseHeader.SetCookie]))
				{
					this.headers[HttpResponseHeader.SetCookie] = this.client.ResponseHeaders[HttpResponseHeader.SetCookie];
				}

				if (json.error == null)
				{
					return json.result;
				}
				else
				{
					throw new ApplicationException(json.error);
				}
			}
		}

		/// <summary>
		/// The Core method which sends the request to the http server and returns the result and parses it into object.
		/// </summary>
		/// <param name="name">Method name as in json-rpc mentioned.</param>
		/// <param name="returnType">The type of the return value.</param>
		/// <param name="args">Argument list to be sent to the remote method.</param>
		/// <returns>Parsed return value, as in 'result' property of the json-rpc spec.</returns>
		internal object DoRequest(string name, Type returnType, params object[] args)
		{
			using (this.client = new WebClient())
			{
				if (headers != null)
				{
					client.Headers[HttpRequestHeader.Cookie] = headers[HttpResponseHeader.SetCookie];
				}
				client.Headers[HttpRequestHeader.ContentType] = MIME_JSON;
				client.Encoding = Encoding.UTF8;

				this.client.Credentials = this.Credentials;


				dynamic json = JsonSerializer.DeserializeFromString(this.client.UploadString(this.Uri, JsonSerializer.SerializeToString(new { method = name, @params = args })), typeof(JsonRpcResult<>).MakeGenericType(returnType));
				if (this.headers == null)
				{
					this.headers = new WebHeaderCollection();
				}

				if (!string.IsNullOrEmpty(this.client.ResponseHeaders[HttpResponseHeader.SetCookie]))
				{
					this.headers[HttpResponseHeader.SetCookie] = this.client.ResponseHeaders[HttpResponseHeader.SetCookie];
				}

				if (json.error == null)
				{
					return json.result;
				}
				else
				{
					throw new ApplicationException(json.error);
				}
			}
		}

	}

	/// <summary>
	/// The class holding JsonRPC result
	/// </summary>
	/// <typeparam name="T">The type of the result</typeparam>
	public class JsonRpcResult<T>
	{
		/// <summary>
		/// The main result
		/// </summary>
		public T result { get; set; }

		/// <summary>
		/// Error, if there is one
		/// </summary>
		public string error { get; set; }
	}

	/// <summary>
	/// Class containing the version information of the Json-Rpc service
	/// </summary>
	public class VersionInfo
	{
		/// <summary>
		/// The full title of the service
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// The provider company name
		/// </summary>
		public string Provider { get; set; }

		/// <summary>
		/// Full version information
		/// </summary>
		public string Version { get; set; }
	}
}