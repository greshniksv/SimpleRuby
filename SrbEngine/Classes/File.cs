﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SrbRuby;

namespace SrbEngine.Class
{
	class File : IClass
	{
	    public string Name()
	    {
	        return "File";
	    }

	    public string Help()
	    {
	        throw new NotImplementedException();
	    }

	    public VariableItem Function(string name, List<VariableItem> param)
	    {
	        throw new NotImplementedException();
	    }

	    public VariableItem Properties(string name)
	    {
	        throw new NotImplementedException();
	    }

	   
	}
}