using System.Net;

namespace HTTP
{
	public class HTTPRequest
	{
		/// <summary>
		/// The HTTP method of the request.
		/// </summary>
		public string RequestMethod {  get; set; }
		/// <summary>
		/// The raw url of the request, does not include domain.
		/// </summary>
		public string RawUrl { get; set; }
		/// <summary>
		/// HTTP Version, the server does not support anything higher than HTTP/1.1.
		/// </summary>
		public string Version { get; set; }
		/// <summary>
		/// The body of the request.
		/// </summary>
		public string Body { get; set; }
		public IPEndPoint Adress { get; set; }
		private Dictionary<string, string> _header { get; set; } = new();
		public HTTPRequest(string request, IPEndPoint ip)
		{
			Adress = ip;
			string[] requestLines = request.Split(HTTPServer.LINEBREAK);
			string[] requestDeets = requestLines[0].Split(' ');
			RequestMethod = requestDeets[0];
			RawUrl = requestDeets[1];
			Version = requestDeets[2];
			int index = 1;
			for (; index < requestLines.Length; index++)
			{
				if (requestLines[index] == string.Empty)
					break;
				string[] field = requestLines[index].Split(':', 2);
				if (string.IsNullOrWhiteSpace(field[1][0].ToString()))
					field[1] = field[1][1..];
				if (_header.ContainsKey(field[0]))
				{
					File.WriteAllText($"Error {DateTime.Now:M HH:mm:ss:fff}.error", request);
					HTTPServer.Log("Duplicate field in header saved http message to file");
					continue;
				}
				_header.Add(field[0].ToLower(), field[1]);
			}
			for (; index < requestLines.Length; index++)
			{
				Body += requestLines[index];
				if (index != requestLines.Length - 1)
					Body += HTTPServer.LINEBREAK;
			}
		}
		/// <summary>
		/// Gets the value from a field in the HTTP header of the request.
		/// </summary>
		/// <param name="key">The name of the field</param>
		/// <returns>Value of the field or <see cref="string.Empty"/> if the field is missing.</returns>
		public string GetField(string key)
		{
			key = key.ToLower();
			if (!_header.ContainsKey(key))
				return string.Empty;
			return _header[key];
		}
	}
}
