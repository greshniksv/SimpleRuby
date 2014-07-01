using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SrbEngine.RClass;
using SrbEngine.RClass.Variables;
using SrbEngine.Variables;
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


    public class Classes : IDisposable
    {
	    private readonly List<IClass> _classList;

	    public List<IClass> List {
		    get { return _classList; }
	    }

	    public Classes()
        {
			_classList = new List<IClass> 
            {
				new RClass.Variables.String(null),
				new RClass.Variables.Bool(false),
				new SrbEngine.RClass.Variables.Nil(),
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
