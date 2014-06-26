using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SrbEngine.Class;
using SrbRuby;

namespace SrbEngine
{
    public interface IClass
    {
        string Name();
        string Help();
        VariableItem Function(string name, List<VariableItem> param);
        VariableItem Properties(string name);
        bool OperatorB(string type, object o1, object o2); /* type: <,>,==,!=,>=,<= */
        object OperatorD(string type, object o1, object o2); /* type: >>,<<,+,-,*,/,^,%,~ */
    }


    public class Classes : IDisposable
    {
        private readonly Hashtable _classList = new Hashtable();

        public Classes()
        {
            var classes = new List<IClass> 
            {
                new File(),
                new GarbageFuscatorClass()
            };





            foreach (var @class in classes)
                _classList.Add(@class.Name(), @class);
        }

        public IClass Get(string name)
        {
            if (_classList.Contains(name)) return (IClass)_classList[name];
            return null;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
