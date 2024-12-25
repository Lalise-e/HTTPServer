using HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.number
{
	public class current : IApiCall
	{
		public HTTPMethod TargetMethod { get { return HTTPMethod.GET; } }

		public byte[] HandleCall(HTTPRequest request, out string contentType)
		{
			contentType = "content-type: text/plain";
			return Encoding.UTF8.GetBytes(StateManagement.GetNumber().ToString());
		}
	}
}
