using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using SrbRuby;

namespace SrbEngine.Classes.Variables
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
	        var count = (buf.Count(i => i == '"'));
	        if ((buf.Count(i => i == '"') == 2) || (buf.Count(i => i == '\'') == 2))
	        {
	            var first = buf.IndexOf('"', 0);
	            int end;
	            if (first == -1)
	            {
	                first = buf.IndexOf('\'', 0);
	                end = buf.IndexOf('\'', first + 1);
	            }
	            else
	                end = buf.IndexOf('"', first + 1);

	            string rest = first!=0 ?
                    (buf.Substring(0, first) + buf.Substring(end+1, buf.Length - (end+1))).Trim() :
                    (buf.Substring(end+1, buf.Length - (end+1))).Trim();

	            if (rest.Length > 0) return null;

	            return new String(s.Trim(new[] {' ', '"'}));
	        }
	        else
	        {
	            return null;
	        }
	    }

        public bool IsClass(object s)
        {
            return (s is String);
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
	        switch (type)
	        {
	            case "==":
                    return (o != null && _variable == o.ToString());

                case "!=":
                    return !(o != null && _variable == o.ToString());

                case "+":
                    return new String(_variable + ((IClass)o).Data());

                case "*":
	                if (o is Int)
	                {
	                    var ret = string.Empty;
	                    for (int i = 0; i < (int)((IClass)o).Data(); i++)
	                    {
	                        ret += _variable;
	                    }
                        return new String(ret);
	                }
	                else
	                    return null;

                default:
	                return null;
	        }


	        return null;
	    }
    }
}
