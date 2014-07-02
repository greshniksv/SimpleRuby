using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SrbEngine.Classes;
using SrbRuby;

namespace SrbEngine
{
    public interface IClass
    {
	    object Data();
        string Name();
        string Help();
	    IClass Parse(string s);
		bool IsClass(object s);
        VariableItem Function(string name, List<VariableItem> param);
        VariableItem Properties(string name);
		object Operator(string type, object o); /* type: <,>,==,!=,>=,<=,>>,<<,+,-,*,/,^,%,~ */
    }


    public class ClassManage : IDisposable
    {
	    private readonly List<IClass> _classList;

	    public List<IClass> List {
		    get { return _classList; }
	    }

        public ClassManage()
        {
			_classList = new List<IClass> 
            {
				new Classes.Variables.String(null),
				new Classes.Variables.Bool(false),
				new Classes.Variables.Nil(),
                //new File(),
                new GarbageFuscatorClass()
            };
        }

        public IClass Get(string name)
        {
	        return _classList.FirstOrDefault(@class => @class.Name() == name);
        }

	    public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
