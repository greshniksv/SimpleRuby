using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SrbEngine.Libraries;
using SrbRuby;

namespace SrbEngine.Class
{
    public class GarbageFuscatorClass : IClass
    {
        private GarbageFuscator _gb;

        public GarbageFuscatorClass ()
        {
            _gb = new GarbageFuscator();
        }

	    public object Data()
	    {
		    throw new NotImplementedException();
	    }

	    public string Name()
        {
            return "GarbageFuscator";
        }

        public string Help()
        {
            return "GarbageFuscator.New \n GarbageFuscator.Crypt(key, data) \n GarbageFuscator.Decrypt(key, data)";
        }

	    public object Parse(string s)
	    {
		    throw new NotImplementedException();
	    }

	    public VariableItem Function(string name, List<VariableItem> param)
        {
            if (name == "Crypt")
            {
                if (param.Count != 2) throw new Exception("function Crypt take 2 parameters!");

                return new VariableItem((object) _gb.Crypt((string)param[0].Data, (string)param[1].Data));
            }

            if (name == "Decrypt")
            {
                if (param.Count != 2) throw new Exception("function Decrypt take 2 parameters!");

                return new VariableItem((object)_gb.Decrypt((string)param[0].Data, (string)param[1].Data));
            }

            return new VariableItem("nil");
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
