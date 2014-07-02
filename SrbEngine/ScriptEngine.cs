using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
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
        public static ClassManage ClassesList { get; set; }
	    public static List<RModule> RModuleList { get; set; }
        public static List<RClass> RClassesList { get; set; }
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
            GLOBALS.ClassesList = new ClassManage();
            GLOBALS.RClassesList = new List<RClass>();
            GLOBALS.RModuleList = new List<RModule>();
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
	        dataMass.RemoveAll(i=>string.IsNullOrEmpty(i.Trim()));

			// split using ';' char and remove comment (#,=begin,=end)
	        var nDataMass = new List<string>();
	        bool comment = false;
	        foreach (var dataItem in dataMass)
	        {
				// remove string
                var item = " " + EncodeString(dataItem) + " ";
                

                // remove comment
	            if (item.Contains("#"))
	            {
                    var p = item.IndexOf('#');
                    item = item.Substring(0, p);
	            }

	            if (item.Contains("=begin ")) comment = true;

		        if (!comment)
		        {
		            if (item.Contains(";"))
		            {
		                nDataMass.AddRange(item.Split(';').Select(DecodeString));
		            }
		            else
		            {
                        nDataMass.Add(DecodeString(item));
		            }
		        }

				if (item.Contains("=end ")) comment = false;
	        }
	        dataMass = nDataMass;
	        

			


	        int ifLevel = 0;

	        RModule rmodule = null;
			RClass rclass = null;
            FunctionItem func = null;
            var baseFunc = new FunctionItem() { Id = Guid.NewGuid(), Name = "*" + Guid.NewGuid(), Code = new List<string>(), Parameters = new List<string>() };
	        
			int funcDeep = -1;
			int classDeep = -1;
			int moduleDeep = -1;
            int lineNumber = 0;

            var manageWords = new string[] { " if ", " unless ", " each ", " for ", " while ", " do " };
            var attrs = new string[] { "attr_reader ", "attr_writer ", "attr_accessor " };
            foreach (var dataItem in dataMass)
            {
                lineNumber++;
				//var bufOrig = dataItem.Trim();
				var buf = " " + EncodeString(dataItem.Trim())+" ";

                if (buf.Contains("module"))
                {
                    moduleDeep = ++ifLevel;
                    var s = (buf.IndexOf("module") + 6);
                    var name = buf.Substring(s, buf.Length - s).Trim();

                    if (!char.IsUpper(name[0]))
                        throw new Exception("class/module name must be CONSTANT");

                    rmodule = new RModule(){Name = name,ClassList = new List<RClass>()};
                }


	            if (buf.Contains("class"))
                {
                    classDeep = ++ifLevel;
	                var s = (buf.IndexOf("class") + 6);
                    var name = buf.Substring(s, buf.Length - s).Trim();

	                if (!char.IsUpper(name[0]))
                        throw new Exception("class/module name must be CONSTANT");

                    rclass = new RClass()
                    {
                        Name = name,
                        FunctionList = new List<FunctionItem>(),
                        Properties = new List<Properties>(),
                    };
                }


	            if (buf.Contains("def"))
                {
                    //var funcName = buf.Remove(buf.IndexOf("def"), 3).Trim();
                    var funcNameWithParam = new List<string>(buf.Split(' '));
                    funcNameWithParam.RemoveAll(string.IsNullOrEmpty);
                    var param =new List<string> (funcNameWithParam.Skip(2));
	                //for (int i = 2; i < funcNameWithParam.Count; i++) param.Add(funcNameWithParam[i]);

                    var name = funcNameWithParam[1];
                    var isStatic = (name.Contains("self."));
                    name = name.Replace("self.", "");

					func = new FunctionItem() { Name = name, 
						Code = new List<string>(), Id = Guid.NewGuid(),Parameters = param,IsStatic = isStatic};
	                funcDeep = ++ifLevel;
                }


                if (attrs.Any(buf.Contains))
                {
                    var type = AttrAccess.Read;
                    if (attrs.Contains("attr_accessor")) type = AttrAccess.ReadWrite;
                    if (attrs.Contains("attr_reader")) type = AttrAccess.Read;
                    if (attrs.Contains("attr_writer")) type = AttrAccess.Write;

                    var b = buf.Replace("attr_accessor", "").Replace("attr_reader", "").Replace("attr_writer", "").Trim();
                    var list = new List<string>(b.Split(',')); 
                    list.RemoveAll(string.IsNullOrEmpty);

                    foreach (var l in list)
                    {
                        var p = new Properties() { Name = l.Trim(), Access = type };
                        rclass.Properties.Add(p);
                    }
                   
                }



                if (manageWords.Any(buf.Contains)) ifLevel++;




				// adding base execute code
	            if (!buf.Contains("def") && !buf.Contains("class") && !buf.Contains("module")
                    && !((ifLevel == funcDeep || ifLevel == classDeep || ifLevel == moduleDeep) && buf.Contains("end")) 
                    && buf.Length > 0)
	            {
					if (func!=null)
                        func.Code.Add(DecodeString(buf));
					else
                        baseFunc.Code.Add(DecodeString(buf));
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
                        if (rclass == null) throw new Exception("ParceError. Finded end of Class, but he not initialized!");

                        if(rmodule==null)
                        {
                            GLOBALS.RClassesList.Add(rclass.Clone() as RClass);
                            rclass = null;
                        }
                        else
                        {
                            rmodule.ClassList.Add(rclass.Clone() as RClass);
                            rclass = null;
                        }
					}

					// end of module
					if (ifLevel == moduleDeep)
					{
                        if (rmodule == null) throw new Exception("ParceError. Finded end of Module, but he not initialized!");
                        GLOBALS.RModuleList.Add(rmodule);
					    rmodule = null;
					}

					if (ifLevel >= 0) ifLevel--;
					else
						throw new Exception("ParceError. Detected unexpected 'end' ");
                }
            }


            if (baseFunc.Code.Count > 0) ExecuteFunction(baseFunc.Name);
        }


        private string EncodeString(string item)
        {
            //  \" = ☺(alt+1); 
            //  \' = ☻(alt+2); 
            item = item.Replace("\\\"", "☺").Replace("\\'", "☻");

            string bufs = string.Empty;
            string buf = string.Empty;
            bool cut = false;
            foreach (char c in item)
            {
                if (c == '"' || c == '\'')
                {
                    cut = !cut;
                    if (!cut)
                    {
                        var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(bufs)); 
                        buf += "\"" +b64+ "\"";
                        bufs = string.Empty;
                    }
                }
                else
                    if (!cut) buf += c;
                    else
                    {
                        bufs += c;
                    }
            }
            return buf;
        }

        private string DecodeString(string item)
        {
            //  \" = ☺(alt+1); 
            //  \' = ☻(alt+2); 

            string bufs = string.Empty;
            string buf = string.Empty;
            bool cut = false;
            foreach (char c in item)
            {
                if (c == '"' || c == '\'')
                {
                    cut = !cut;
                    if (!cut)
                    {
                        buf += "\"" + Encoding.UTF8.GetString(Convert.FromBase64String(bufs)) + "\"";
                        bufs = string.Empty;
                    }
                }
                else
                    if (!cut) buf += c;
                    else
                    {
                        bufs += c;
                    }
            }

            buf = buf.Replace("☺", "\\\"").Replace("☻", "\\'");
            return buf;
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
