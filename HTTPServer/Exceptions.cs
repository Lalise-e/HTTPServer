using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTTP
{
	public class HTTPException : Exception
	{
		public int ErrorCode = 200;
		public HTTPException(string? message, int errorCode) : base(message)
		{
			ErrorCode = errorCode;
		}
	}
}
