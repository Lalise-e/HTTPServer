using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.number
{
	public class increment : IPost
	{
		public string HandlePOST(string requestBody, out int statusCode, out string message)
		{
			statusCode = 200;
			message = "OK";
			StateManagement.Add(1);
			return StateManagement.GetNumber().ToString();
		}
	}
}
