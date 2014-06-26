using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SrbRuby;

namespace SrbEngine.Class.Variables
{
	internal class Bool : IClass
	{
		private bool _variable;

		public Bool(bool data)
		{
			_variable = data;
		}

		public object Data()
		{
			return _variable;
		}

		public string Name()
		{
			return "Bool";
		}

		public string Help()
		{
			throw new NotImplementedException();
		}

		public IClass Parse(string s)
		{
			if (s.Trim().ToLower() == "true" || s.Trim().ToLower() == "false")
			{
				return new Bool((s.Trim().ToLower() == "true"));
			}
			return null;
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
