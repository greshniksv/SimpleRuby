using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using SrbEngine;

namespace SrbRuby
{
    public static class GLOBALS
    {
        public static Hashtable Variables { get; set; }
        public static List<FunctionItem> Functions { get; set; }
        public static Classes ClassesList { get; set; }
    }


    public class ScriptEngine
    {

        //private readonly List<FunctionItem> _functionList = new List<FunctionItem>();
        

        public delegate void FunctionExecuteCode(string function, string command);
        public event FunctionExecuteCode FunctionExecuteCodeEvent = delegate { };


        public Commands Command
        {
            get { return new Commands(); }
        }

        public ScriptEngine()
        {
            GLOBALS.Functions = new List<FunctionItem>();
            GLOBALS.Variables = new Hashtable();
            GLOBALS.ClassesList = new Classes();
        }

        public void ExecuteFunction(string funcName = "main")
        {
            using (var functions = new Functions(GLOBALS.Functions.FirstOrDefault(
                i => string.Equals(i.Name, funcName, StringComparison.OrdinalIgnoreCase))))
            {
                functions.ExecuteCodeEvent += (function, command) => FunctionExecuteCodeEvent(function, command);
                functions.Execute(null);
            }
        }



        #region Loading script
        public void LoadFromString(string data)
        {
            GLOBALS.Functions.Clear();
            GLOBALS.Variables.Clear();

            CreateFunction(data.Split('\n'));
        }

        public void LoadFromFile(string file)
        {
            GLOBALS.Functions.Clear();
            GLOBALS.Variables.Clear();

            using (TextReader reader = new StreamReader(file))
            {
                CreateFunction(reader.ReadToEnd().Split('\n'));
            }
        }

        private void CreateFunction(IEnumerable<string> dataMass)
        {
            GLOBALS.Functions.Clear();

            int ifLevel = 0;
            var func = new List<FunctionItem>();
            var manageWords = new string[] { "if", "unless", "each", "for", "while" };
            foreach (var dataItem in dataMass)
            {
                var buf = dataItem.Trim();

                if (buf.Contains("def"))
                {
                    //var funcName = buf.Remove(buf.IndexOf("def"), 3).Trim();
	                var funcNameWithParam = buf.Split(' ');
	                var param = new List<string>();
	                for (int i = 2; i < funcNameWithParam.Length; i++) param.Add(funcNameWithParam[i]); 
					func.Add(new FunctionItem() { Name = funcNameWithParam[1], 
						Code = new List<string>(), Id = Guid.NewGuid(),Parameters = param});
                }

				// TODO: Detect manageword in a string. Fix this !
				if (manageWords.Any(i => buf.Contains(i+" ")))
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
                        GLOBALS.Functions.Add(func[func.Count - 1].Clone() as FunctionItem);
                        func.Remove(func[func.Count - 1]);
                    }
                }
            }
        }

        #endregion

    }
}
