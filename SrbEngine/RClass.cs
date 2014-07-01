using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SrbRuby;

namespace SrbEngine
{
	public enum AttrAccess { Read, Write, ReadWrite }

	public class Attr
	{
		public AttrAccess Access { get; set; }
		public string Name { get; set; }
	}

	public class RClass
	{
		private List<FunctionItem> _functionItems = new List<FunctionItem>();

		public Dictionary<string, string> AliasList { get; set; }
		public List<Attr> AttrList { get; set; }

		public List<FunctionItem> FunctionList
		{
			get { return _functionItems; }
			set { _functionItems = value; }
		}
	}
}
