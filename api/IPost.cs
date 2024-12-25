using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api
{
	/// <summary>
	/// Interface for handling POST requests targeting /api.
	/// </summary>
	public interface IPost
	{
		/// <summary>
		/// Handles an API GET request.
		/// </summary>
		/// <param name="requestBody">The body of the HTTP request.</param>
		/// <param name="statusCode">The status code of the response.</param>
		/// <param name="message">Status message</param>
		/// <returns>The response, will eventually be a json or something, idk haven't decided yet.</returns>
		public string HandlePOST(string requestBody, out int statusCode, out string message);
	}
}
