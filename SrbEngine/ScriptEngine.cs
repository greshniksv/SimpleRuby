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
	    public static List<RModule> ModuleList { get; set; }
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

        public void ExecuteFunction(string funcName = "*")
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

            CreateFunction(new List<string>(data.Split('\n')));
        }

        public void LoadFromFile(string file)
        {
            GLOBALS.Functions.Clear();
            GLOBALS.Variables.Clear();

            using (TextReader reader = new StreamReader(file))
            {
                CreateFunction(new List<string>(reader.ReadToEnd().Split('\n')));
            }
        }

        private void CreateFunction(List<string> dataMass)
        {
			GLOBALS.Functions.Clear();
	        dataMass.RemoveAll(string.IsNullOrEmpty);

			// split using ';' char and remove comment (#,=begin,=end)
	        var nDataMass = new List<string>();
	        bool comment = false;
	        foreach (var dataItem in dataMass)
	        {
				// remove string
				var item = RemoveString(dataItem);

				if (item.Contains("=begin ")) comment = true;

		        if (!comment)
		        {
					if (item.Contains(";"))
						nDataMass.AddRange(item.Split(';'));
			        else
						nDataMass.Add(item);
		        }

				if (item.Contains("=end ")) comment = false;
	        }
	        dataMass = nDataMass;
	        

			


	        int ifLevel = 0;

	        RModule rmodule = null;
			RClass rclass = null;
			FunctionItem func = null;
			FunctionItem baseFunc = new FunctionItem(){Id = Guid.NewGuid(),Name = "*"};
	        
			int funcDeep = -1;
			int classDeep = -1;
			int moduleDeep = -1;

            var manageWords = new string[] { " if ", " unless ", " each ", " for ", " while ", " do " };
            foreach (var dataItem in dataMass)
            {
				var bufOrig = dataItem.Trim();
				var buf = " " + RemoveString(dataItem.Trim())+" ";

	            if (buf.Contains("class"))
	            {

	            }


	            if (buf.Contains("def"))
                {
                    //var funcName = buf.Remove(buf.IndexOf("def"), 3).Trim();
					var funcNameWithParam = bufOrig.Split(' ');
	                var param = new List<string>();
	                for (int i = 2; i < funcNameWithParam.Length; i++) param.Add(funcNameWithParam[i]); 

					func = new FunctionItem() { Name = funcNameWithParam[1], 
						Code = new List<string>(), Id = Guid.NewGuid(),Parameters = param};
	                funcDeep = ifLevel;
                }

				if (manageWords.Any(buf.Contains)) ifLevel++;

				// adding base execute code
	            if (!buf.Contains("def") && !(ifLevel == 0 && buf.Contains("end")) && buf.Length > 0)
	            {
					if (func!=null) 
						func.Code.Add(buf);
					else
						baseFunc.Code.Add(buf);
	            }


                if (buf.Contains("end"))
                {
					// end of function
	                if (ifLevel == funcDeep)
	                {
						if (func == null)
							throw new Exception("ParceError. Finded end of function, but function not initialized!");

		                if (rclass != null)
			                rclass.FunctionList.Add(func.Clone() as FunctionItem);
						else
							GLOBALS.Functions.Add(func.Clone() as FunctionItem);
						
						func = null;
	                }

					// end of class
					if (ifLevel == classDeep)
					{

					}

					// end of module
					if (ifLevel == moduleDeep)
					{

					}

					if (ifLevel >= 0) ifLevel--;
					else
					{
						throw new Exception("ParceError. Detected excess 'end' ");
					}
                    
                }
            }
        }

	    private string RemoveString(string item)
	    {
			item = item.Replace("\\\"", "").Replace("\\'", "");
		    string buf = string.Empty;
		    bool cut = false;
		    foreach (char c in item)
		    {
			    if (c == '"' || c == '\'')
				    cut = !cut;
			    else
				    if (!cut) buf += c;
		    }
		    return buf;
	    }




	    #endregion

    }
}
