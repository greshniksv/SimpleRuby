using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SrbEngine.Libraries;
using SrbRuby;

namespace SrbEngine.Commands
{
    public class Garbagefuscator : SrbRuby.Commands.ICommand
    {
        public string Name()
        {
            return "gfuscator";
        }

        public string Information()
        {
            return "Info: waiting miliseconds.\n\nExample: sleep(100)";
        }

        public VariableItem Execute(List<VariableItem> param)
        {
            if (param.Count() != 2) throw new Exception("sleep error! Count of params not valid! ");

            var fuscator = new GarbageFuscator();
            var ret =fuscator.Crypt((string) param[0].GetData(), (string) param[1].GetData());
			ret = fuscator.Decrypt((string)param[0].GetData(), ret);
            return new VariableItem((object)ret);
        }

    }



}
