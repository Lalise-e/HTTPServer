namespace HTTPFramework
{
	/// <summary>
	/// Interface for handling POST requests targeting /api.
	/// </summary>
	public interface IApiCall
	{
		/// <summary>
		/// Targeted HTTP method, atm only GET and POST are implemented.
		/// </summary>
		public HTTPMethod TargetMethod { get; }
		/// <summary>
		/// Handles an API call.
		/// </summary>
		/// <param name="request">The <see cref="HTTPRequest"/></param>
		/// <param name="contentType">the content type, will be null if no body will be sent.</param>
		/// <returns>Encoded body of the response.</returns>
		public byte[] HandleCall(HTTPRequest request, out string contentType);
	}
}
