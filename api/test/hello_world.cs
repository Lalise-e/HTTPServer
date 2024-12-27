using HTTPFramework;
using System.Text;

namespace api.test
{
	internal class hello_world : IApiCall
	{
		public HTTPMethod TargetMethod { get { return HTTPMethod.GET; } }

		public byte[] HandleCall(HTTPRequest request, out string contentType)
		{
			contentType = "content-type: text/plain";
			return Encoding.UTF8.GetBytes("Hello, World!");
		}
	}
}
