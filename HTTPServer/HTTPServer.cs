//HTTP/1.1 server made with TCP
using api;
using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HTTP
{
	class HTTPServer
	{
		public const int PORT = 80;
		//Your local IP. Change this based on your network.
		public const string IP = "192.168.0.137";
		public const string LINEBREAK = "\r\n";
		public const string VERSION = "HTTP/1.1";
		//Add the location of your webpage here i.e. "C:\Niche-Interest\WebPage
		public static string WebPath = "";
		public static string DefaultFile = "index.html";
		public static Dictionary<string, IPost> _postTable = new();
		public static Dictionary<string, IGet> _getTable = new();
		public static async Task Main(string[] args)
		{
			LoadApi();
			TcpListener listener = new(IPAddress.Parse(IP), PORT);
			listener.Start();
			Log($"Started listening on port {PORT}...");
			while (true)
			{
				TcpClient client = await listener.AcceptTcpClientAsync();
				Thread connectionHandler = new Thread(() => HandleConnection(client));
				connectionHandler.Start();
			}
		}
		/// <summary>
		/// Writes the message to the console window with a timestamp.<br></br>
		/// TODO: Make this better
		/// </summary>
		public static void Log(string message)
		{
			Console.WriteLine($"[{DateTime.Now:HH:mm:ss:ff}]: {message}");
		}
		/// <summary>
		/// Loads all the classes with <see cref="IGet"/> and/or <see cref="IPost"/> from <see cref="api"/> into 
		/// <see cref="_getTable"/> and <see cref="_postTable"/>.
		/// </summary>
		private static void LoadApi()
		{
			Assembly assembly = typeof(IGet).Assembly;
			Console.WriteLine(assembly.FullName);
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				List<Type> interfaces = types[i].GetInterfaces().ToList();
				//Checks if type is valid
				if (interfaces.Count == 0)
					continue;
				if (!interfaces.Contains(typeof(IGet)) && !interfaces.Contains(typeof(IPost)))
					continue;
				//Creates path
				string path = '/' + (types[i].Namespace.Replace('.', '/')) + "/" + types[i].Name;
				//Creates instance of class
				ConstructorInfo constructor = types[i].GetConstructor(Array.Empty<Type>()) ??
					throw new Exception($"{types[i].FullName} lacks a public constructor with 0 arguments");
				object instance = constructor.Invoke(Array.Empty<object>());
				//Deals with GET
				if (interfaces.Contains(typeof(IGet)))
				{
					_getTable.Add(path, (IGet)instance);
					Log($"Added GET {path}");
				}
				if (interfaces.Contains(typeof(IPost)))
				{
					_postTable.Add(path, (IPost)instance);
					Log($"Added POST {path}");
				}
			}
			Log("API successfully loaded.");
		}

		/// <summary>
		/// Constructs a HTTP request from a TCP client and sends if off for further handling.
		/// </summary>
		private static void HandleConnection(TcpClient client)
		{
			string incomming = null;
			byte[] buffer = new byte[1048576];
			while (client.Connected)
			{
				NetworkStream stream = client.GetStream();
				int i;
				while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
				{
					incomming = Encoding.UTF8.GetString(buffer, 0, i);
					HTTPRequest request = null;
					try
					{
						request = new HTTPRequest(incomming, client.Client.RemoteEndPoint as IPEndPoint);
						if (request.GetField("user-agent") == string.Empty)
							throw new HTTPException("Missing User Agent.", 403);
						HandleRequest(client, request);
					}
					catch (HTTPException e) { HandleError(client, request, e); }
					return;
				}
			}
		}
		/// <summary>
		/// Handles a HTTP request based on the HTTP method specified in the request.<br></br>
		/// Writes a console message upon succesful completion.
		/// </summary>
		private static void HandleRequest(TcpClient client, HTTPRequest request)
		{
			if (request.RequestMethod == "GET" && request.RawUrl == "/")
				Log($"{request.GetField("user-agent")} connected from: {request.Adress}");
			switch (request.RequestMethod)
			{
				case "GET":
					HandleGET(client, request);
					break;
				case "HEAD":
					HandleHEAD(client, request);
					break;
				default:
					throw new HTTPException($"{request.RequestMethod} Is Not Implemented", 501);
			}
			Log($"Successfully handled {request.RequestMethod} for {request.RawUrl}");
		}
		/// <summary>
		/// Handles a HEAD request.
		/// </summary>
		private static void HandleHEAD(TcpClient client, HTTPRequest request)
		{
			byte[] message = ConstructMessage(200, "OK", ConstructHeader(), new byte[0]);
			SendMessage(client, message);
		}
		/// <summary>
		/// Handles a GET request.
		/// </summary>
		private static void HandleGET(TcpClient client, HTTPRequest request)
		{
			if(request.RawUrl.Length >= 4)
				if (request.RawUrl[..4] == "/api")
				{
					HandleGETApi(client, request);
					return;
				}
			string requestedPath = WebPath + request.RawUrl;
			if (!File.Exists(requestedPath))
			{
				if (!File.Exists(requestedPath + DefaultFile))
					throw new HTTPException("File Not Found", 404);
				requestedPath += DefaultFile;
			}
			string media;
			switch (Path.GetExtension(requestedPath))
			{
				case ".html":
					media = "text/html; charset=utf-8";
					break;
				case ".css":
					media = "text/css; charset=utf-8";
					break;
				case ".gif":
					media = "image/gif";
					break;
				case ".png":
					media = "image/png";
					break;
				case ".svg":
					media = "image/svg+xml";
					break;
				case ".ico":
					media = "image/x-icon";
					break;
				case ".ttf":
					media = "font/ttf";
					break;
				default:
					throw new HTTPException("Unhandled Media Type", 500);
			}
			string accept = request.GetField("accept");
			if (!(accept.Contains(media) || accept.Contains("*/*") || accept.Contains(media.Split('/')[0] + "/*")))
				throw new HTTPException("No valid format found", 415);
			string[] header = ConstructHeader($"content-type: {media}");
			byte[] body = File.ReadAllBytes(requestedPath);
			byte[] buffer = ConstructMessage(200, "OK", header, body);
			SendMessage(client, buffer);
		}
		/// <summary>
		/// Handles an API GET request.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="request"></param>
		private static void HandleGETApi(TcpClient client, HTTPRequest request)
		{
			throw new HTTPException("Not Implemented", 501);
		}
		/// <summary>
		/// Handles a HTTP error and notifies the client of the failure.
		/// </summary>
		private static void HandleError(TcpClient client, HTTPRequest request, HTTPException exception)
		{
			Log($"Failed {request.RequestMethod} for {request.RawUrl} {exception.ErrorCode} - {exception.Message}");
			string[] header;
			string body;
			string accept = request.GetField("accept");
			if(accept.ToLower().Contains("text/html"))
			{
				header = ConstructHeader("content-type: text/html; charset=utf-8");
				body = $"<!doctype html><title>{exception.Message}</title><h1>{exception.ErrorCode}</h1><p>{exception.Message}</p>";
			}
			else
			{
				header = ConstructHeader("content-type: text/plain; charset=utf-8;");
				body = $"{exception.ErrorCode} - {exception.Message}";
			}
			byte[] message = ConstructMessage(exception.ErrorCode, exception.Message, header.ToArray(),
				Encoding.UTF8.GetBytes(body));
			SendMessage(client, message);
		}
		/// <summary>
		/// Construcs a HTTP header meant for responses.<br></br>
		/// This is a terrible way to do it.
		/// </summary>
		/// <param name="headerFields">fields to be added to the header. Some are already in by default and will not be overwritten.</param>
		private static string[] ConstructHeader(params string[] headerFields)
		{
			List<string> result = ["server: custom HTTP server/0.1", "cache: no-store", "connection: close"];
			foreach (string field in headerFields)
				result.Add(field);
			return result.ToArray();
		}
		/// <summary>
		/// Creates a HTTP message
		/// </summary>
		/// <param name="statusCode">HTTP status code</param>
		/// <param name="statusMessage">Status/Error message</param>
		/// <param name="header">Array of all the header field, don't include length that is automatically added.</param>
		/// <param name="body">The body of the HTTP message.</param>
		/// <returns>An UTF-8 encoded HTTP message</returns>
		private static byte[] ConstructMessage(int statusCode, string statusMessage, string[] header, byte[] body)
		{
			string result = $"{VERSION} {statusCode} {statusMessage}{LINEBREAK}";
			for (int i = 0; i < header.Length; i++)
			{
				result += header[i] + LINEBREAK;
			}
			result += $"content-lentgth: {body.Length}{LINEBREAK}{LINEBREAK}";
			List<byte> buffer = [.. Encoding.UTF8.GetBytes(result), .. body];
			return buffer.ToArray();
		} 
		/// <summary>
		/// Send the message to the specified TCP client.
		/// </summary>
		private static void SendMessage(TcpClient client, byte[] message)
		{
			if (!client.Connected)
				return;
			Stream outStream = client.GetStream();
			outStream.Write(message, 0, message.Length);
			outStream.Dispose();
			client.Close();
			client.Dispose();
		}
	}
}