using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SrbRuby;

namespace SrbEngine.Class.Variables
{
	public class Nil: IClass
	{
		public override string ToString()
		{
			return "nil";
		}

		public object Data()
		{
		    return this;
		}

		public string Name()
		{
			return "Nil";
		}

		public string Help()
		{
			return "Nil";
		}

		IClass IClass.Parse(string s)
		{
		    if (s.ToLower().Trim() == "nil")
		    {
		        return new Nil();
		    }
		    return null;
		}

	    public bool IsClass(object s)
	    {
	        return (s is Nil);
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
		    return (o != null && (o is Nil));
		}

	   
	}
}
