using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SrbRuby;

namespace SrbEngine.Class.Variables
{
    public class String : IClass
    {
	    private string _variable;

	    public String(string data)
	    {
		    _variable = data;
	    }

	    public object Data()
	    {
		    return (object) _variable;
	    }

	    public string Name()
	    {
		    return "String";
	    }

	    public string Help()
	    {
		    return "String help";
	    }

	    public IClass Parse(string s)
	    {
			var buf = s.Replace("\\'", "").Replace("\\\"", "").Trim();
			if ((buf.Count(i => i == '"') != 2) || (buf.Count(i => i == '\'') != 2)) return null;

		    var first = buf.IndexOf('"', 0);
		    int end = -1;
		    if (first == -1)
		    {
				first = buf.IndexOf('\'', 0);
				end = buf.IndexOf('\'', first + 1);
		    }
		    else
				end = buf.IndexOf('"', first + 1);

		    var rest = buf.Substring(first, end - first).Trim();
		    if (rest.Length > 0) return null;

		    return new String(s.Trim(new []{' ','"'}));
	    }

	    public IClass Parse(object s)
	    {
		    if(s is string) return new String((string)s);
		    return null;
	    }


	    public VariableItem Function(string name, List<VariableItem> param)
	    {
		    throw new NotImplementedException();
	    }

	    public VariableItem Properties(string name)
	    {
		    throw new NotImplementedException();
	    }

	    public object Operator(string type, object o)
	    {
		    throw new NotImplementedException();
	    }
    }
}
