﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.number
{
	public class current : IGet
	{
		public string HandleGet(string requestBody, out int statusCode, out string message)
		{
			statusCode = 200;
			message = "OK";
			return StateManagement.GetNumber().ToString();
		}
	}
}