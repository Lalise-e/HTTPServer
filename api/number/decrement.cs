using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.number
{
	public class decrement : IPost
	{
		public string HandlePOST(string requestBody, out int statusCode, out string message)
		{
			StateManagement.Add(-1);
			statusCode = 200;
			message = "OK";
			return StateManagement.GetNumber().ToString();
		}
	}
}
