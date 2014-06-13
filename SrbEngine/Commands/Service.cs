using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SrbRuby;

namespace SrbEngine.Commands
{
	public class Sleep : SrbRuby.Commands.ICommand
	{
		public string Name()
		{
			return "sleep";
		}

		public string Information()
		{
			return "Info: waiting miliseconds.\n\nExample: sleep(100)";
		}

		public VariableItem Execute(List<VariableItem> param)
		{
			if (param.Count() != 1) throw new Exception("sleep error! Count of params not valid! ");

			Thread.Sleep((int)param[0].GetData());

			return new VariableItem("nil");
		}

	}


}
