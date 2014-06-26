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
			throw new NotImplementedException();
		}

		public string Name()
		{
			return "Nil";
		}

		public string Help()
		{
			throw new NotImplementedException();
		}

		IClass IClass.Parse(string s)
		{
			throw new NotImplementedException();
		}

		public IClass Parse(object s)
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

		public object Operator(string type, object o)
		{
			throw new NotImplementedException();
		}
	}
}
