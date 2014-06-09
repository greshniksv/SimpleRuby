using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace SrbRuby
{

    public class FunctionItem : ICloneable
    {
        public string Name { get; set; }
        public List<string> Code { get; set; }
        public Guid Id { get; set; }

        public object Clone()
        {
            return new FunctionItem() { Name = this.Name, Code = new List<string>(this.Code), Id = new Guid(Id.ToString()) };
        }
    }


    public class ScriptEngine
    {

        private readonly List<FunctionItem> _functionList = new List<FunctionItem>();
        private Variables _globalVariables;

        public delegate void FunctionExecuteCode(string function, string command);
        public event FunctionExecuteCode FunctionExecuteCodeEvent = delegate { };


        public Commands Command
        {
            get { return new Commands(null); }
        }
        public string[] GetVariableTypeList()
        {
            return new Variables().GetVariableTypeList();
        }

        public ScriptEngine()
        {
            _functionList = new List<FunctionItem>();
            _globalVariables = new Variables();
        }

        public void ExecuteFunction(string funcName = "main")
        {
            using (var functions = new Functions(_functionList.FirstOrDefault(
                i => string.Equals(i.Name, funcName, StringComparison.OrdinalIgnoreCase)), _functionList))
            {
                functions.ExecuteCodeEvent += (function, command) => FunctionExecuteCodeEvent(function, command);
                functions.Execute();
            }
        }



        #region Loading script
        public void LoadFromString(string data)
        {
            CreateFunction(data.Split('\n'));
        }

        public void LoadFromFile(string file)
        {
            int ifLevel = 0;
            var func = new List<FunctionItem>();

            using (TextReader reader = new StreamReader(file))
            {
                CreateFunction(reader.ReadToEnd().Split('\n'));
            }
        }

        private void CreateFunction(IEnumerable<string> dataMass)
        {
            _functionList.Clear();

            int ifLevel = 0;
            var func = new List<FunctionItem>();
            var manageWords = new string[] { "if", "unless", "each", "for", "while" };
            foreach (var dataItem in dataMass)
            {
                var buf = dataItem.Trim();

                if (buf.Contains("def"))
                {
                    var funcName = buf.Remove(buf.IndexOf("def"), 3).Trim();
                    func.Add(new FunctionItem() { Name = funcName, Code = new List<string>(), Id = Guid.NewGuid() });
                }

                if (manageWords.Any(buf.Contains))
                {
                    ifLevel++;
                }


                if (!buf.Contains("def")
                    && !(ifLevel == 0 && buf.Contains("end"))
                    && buf.Length > 0)
                {
                    func[func.Count - 1].Code.Add(buf);
                }


                if (buf.Contains("end"))
                {
                    if (ifLevel > 0) ifLevel--;
                    else
                    {
                        _functionList.Add(func[func.Count - 1].Clone() as FunctionItem);
                        func.Remove(func[func.Count - 1]);
                    }
                }
            }
        }

        #endregion

    }
}
