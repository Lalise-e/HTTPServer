using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.hello_world
{
	internal class print : IGet
	{
		public string HandleGet(string requestBody, out int statusCode, out string message)
		{
			statusCode = 200;
			message = "OK";
			return "Hello, World!";
		}
	}
}
