using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SrbRuby
{
    public class Commands : IDisposable
    {
        /* Commands:
         * Copy, Move, Rm, MkDir, RmDir, 
         * File?, Dir?, Zip?, 7zip?, Bzip?
         * UnZip, Un7zip, UnBzip
         * FileCountInDir, DirCount
         * FreeSpaceOnDrive, FileLength, DirLength
         * Alert, Confirm
         * 
         * 
         * 
         */

        public interface ICommand
        {
            string Name();
            string Information();
			VariableItem Execute(List<VariableItem> prmItem);
        }

        private List<ICommand> _commandList;
        private Hashtable _commandHashTable;
        private Variables _variables;

        public Commands(Variables variables)
        {
            _variables = variables;

            // Add command to list
            _commandList = new List<ICommand>
			{
				new Alert(),
				new Confirm()
			};

            _commandHashTable = new Hashtable();
            foreach (var command in _commandList)
            {
                _commandHashTable.Add(command.Name(), command);
            }

        }

        public IEnumerable<string> GetCommandNameList()
        {
            return _commandList.Select(i => i.Name());
        }

        public Hashtable GetCommandHashTable()
        {
            return _commandHashTable;
        }

        private List<VariableItem> GetParamsFromCommand(string command)
        {
            var prmList = command.Substring(command.IndexOf('(') + 1, command.IndexOf(')') - (command.IndexOf('(') + 1)).Trim().Split(',');
            return prmList.Select(s => _variables.GetVariable(s)).ToList();
        }

        private string GetCommandName(string command)
        {
            if (command.IndexOf("(", System.StringComparison.Ordinal) == -1)
                throw new Exception("Command does not have '(' !");

            return command.Substring(0, command.IndexOf("(", System.StringComparison.Ordinal)).Trim();
        }

		public VariableItem Execute(string command, List<VariableItem> param)
        {
            var cmd = _commandHashTable[command] as ICommand;
            if (cmd != null)
                return cmd.Execute(param);
            else
                throw new Exception("Command '" + command + "' not found !");
        }

        #region Commands

        private class Alert : ICommand
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

                return new VariableItem("true");
            }

        }


        private class Confirm : ICommand
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

        #endregion

        #region Despose patternt
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Implement dispose pattern
        public void Dispose(bool managed)
        {
            if (managed)
            {
                if (_commandList != null)
                {
                    _commandList.Clear();
                    _commandList = null;
                }

                if (_commandHashTable != null)
                {
                    _commandHashTable.Clear();
                    _commandHashTable = null;
                }
            }
        }
        #endregion

    }
}
