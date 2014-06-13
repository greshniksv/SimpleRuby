using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SrbRuby;

namespace SrbEngine.Commands
{

	public class Alert : SrbRuby.Commands.ICommand
	{
		public string Name()
		{
			return "alert";
		}

		public string Information()
		{
			return "Info: Show a message.\n\nExample: alert(massage,caption,icon)\n\n" +
				"Icon: asterisk, error, exclamation, hand, information, none, question, stop, warning";
		}

		public VariableItem Execute(List<VariableItem> param)
		{
			if (param.Count() != 3) throw new Exception("Alert error! Count of params not valid! ");

			MessageBoxIcon icon;
			if (!Enum.TryParse(param[2].ToString(), true, out icon)) icon = MessageBoxIcon.Error;
			MessageBox.Show(param[0].ToString(), param[1].ToString(), MessageBoxButtons.OK, (MessageBoxIcon)icon);

			return new VariableItem("nil");
		}

	}


	public class Confirm : SrbRuby.Commands.ICommand
	{
		public string Name()
		{
			return "confirm";
		}

		public string Information()
		{
			return "Info: Show a message.\n\nExample: confirm(massage,caption,icon,button)\n\n" +
				"Icon: asterisk, error, exclamation, hand, information, none, question, stop, warning\n\n" +
				"Button: AbortRetryIgnore,OK,OKCancel,RetryCancel,YesNo,YesNoCancel";
		}

		public VariableItem Execute(List<VariableItem> param)
		{
			if (param.Count() != 4) throw new Exception("Alert error! Count of params not valid! ");

			MessageBoxIcon icon;
			if (!Enum.TryParse(param[2].ToString(), true, out icon)) icon = MessageBoxIcon.Error;

			MessageBoxButtons button;
			if (!Enum.TryParse(param[3].ToString(), true, out button)) button = MessageBoxButtons.YesNo;

			MessageBox.Show(param[0].ToString(), param[1].ToString(), button, icon);

			return null;
		}

	}
}
