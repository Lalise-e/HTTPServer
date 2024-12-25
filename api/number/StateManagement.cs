using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.number
{
	internal static class StateManagement
	{
		private static bool _inProgress = false;
		private static int _number;
		internal static void Add(int diff)
		{
			while (_inProgress)
				Thread.Sleep(1);
			_inProgress = true;
			_number += diff;
			_inProgress = false;
		}
		internal static int GetNumber() => _number;
	}
}
