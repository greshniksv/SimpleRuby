using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SrbRuby;

namespace SrbEngine
{
	public enum AttrAccess { Read, Write, ReadWrite }

    public class Properties
	{
		public AttrAccess Access { get; set; }
		public string Name { get; set; }
	}

	public class RClass : ICloneable
	{
		private List<FunctionItem> _functionItems = new List<FunctionItem>();

        public List<Properties> Properties { get; set; }
	    public string Name { get; set; }

	    public List<FunctionItem> FunctionList
		{
			get { return _functionItems; }
			set { _functionItems = value; }
		}

	    public object Clone()
	    {
            return new RClass
	        {
	            Name = Name,
	            Properties = new List<Properties>(Properties),
	            FunctionList = new List<FunctionItem>(FunctionList)
	        };
	    }
	}
}
