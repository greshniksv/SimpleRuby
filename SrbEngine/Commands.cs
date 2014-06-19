using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SrbEngine.Commands;

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

        public Commands()
        {
            // Add command to list
            _commandList = new List<ICommand>
			{
				new Alert(),
				new Confirm(),
                new Sleep(),
                new Garbagefuscator()
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

		public VariableItem Execute(string command, List<VariableItem> param)
        {
            var cmd = _commandHashTable[command] as ICommand;
            if (cmd != null)
                return cmd.Execute(param);
            else
                throw new Exception("Command '" + command + "' not found !");
        }


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
