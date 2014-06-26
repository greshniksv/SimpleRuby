using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SrbRuby;

namespace SrbEngine.Variables
{
    public class String : IClass
    {
	    public object Data()
	    {
		    throw new NotImplementedException();
	    }

	    public string Name()
	    {
		    throw new NotImplementedException();
	    }

	    public string Help()
	    {
		    throw new NotImplementedException();
	    }

	    public object Parse(string s)
	    {


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
