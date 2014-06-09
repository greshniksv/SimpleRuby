using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace SrbRuby
{
    public enum VariableType
    {
        String, Int, Bool, Double, Char, Byte,
        ListString, ListInt, ListBool, ListDouble, ListByte, ListChar
    }

    public partial class VariableItem
    {

        public string StatementId { get; set; }
        public string Name { get; set; }
        public VariableType Type { get; set; }

        protected bool Equals(VariableItem other)
        {
            return string.Equals(StatementId, other.StatementId) &&
                string.Equals(Name, other.Name) &&
                Type == other.Type &&
                Equals(Variable, other.Variable);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VariableItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (StatementId != null ? StatementId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ (Variable != null ? Variable.GetHashCode() : 0);
                return hashCode;
            }
        }

        public VariableItem(object ob, string name)
        {
            Name = name;
            Set(ob, name);
        }

        public VariableItem(object ob)
        {
            Name = Guid.NewGuid().ToString();
            Set(ob, Name);
        }



        public VariableItem(string var)
        {
            Initialize(var);
        }

        public virtual void Initialize(string var)
        {
            //var var = _stringVariableForInit;
            // if true|false bool
            if (var == "true" || var == "false")
            {
                bool b = (var == "true");
                Set(b, Guid.NewGuid().ToString());
                return;
            }

            // if ' - char
            if (var.Contains("'"))
            {
                char d;
                if (char.TryParse(var.Replace("'", "").Trim(), out d))
                {
                    Set(d, Guid.NewGuid().ToString());
                    return;
                }
            }

            // if " - string
            if (var.Contains("\""))
            {
                Set(var.Replace("\"", "").Trim(), Guid.NewGuid().ToString());
                return;
            }

            // if . and digits - double
            if (var.Contains("."))
            {
                if (var.Replace('.', '0').All(Char.IsDigit))
                {
                    double d;
                    if (double.TryParse(var, out d))
                    {
                        Set(d, Guid.NewGuid().ToString());
                        return;
                    }
                }
            }

            // byte
            if (var.All(Char.IsDigit))
            {
                byte b;
                if (byte.TryParse(var, out b))
                {
                    Set(b, Guid.NewGuid().ToString());
                    return;
                }
            }

            // int
            if (var.All(Char.IsDigit))
            {
                Int32 i;
                if (Int32.TryParse(var, out i))
                {
                    Set(i, Guid.NewGuid().ToString());
                    return;
                }
            }

            throw new Exception("Can not create variable: " + var);
        }



        #region Emplements variable.


        private object Variable { get; set; }

        public void Set(object ob, string name)
        {
            var typeSplited = new List<string>(ob.GetType().AssemblyQualifiedName.Split('[', ','));
            typeSplited.RemoveAll(i => i.Trim().Length < 1);
            Variable = ob;

            if (!ob.GetType().Name.Contains("List"))
            {
                switch (ob.GetType().Name)
                {
                    case "String": Type = VariableType.String;
                        break;
                    case "Boolean": Type = VariableType.Bool;
                        break;
                    case "Double": Type = VariableType.Double;
                        break;
                    case "Int32": Type = VariableType.Int;
                        break;
                    case "Char": Type = VariableType.Char;
                        break;
                    case "Byte": Type = VariableType.Byte;
                        break;

                    default:
                        throw new Exception("Not found variable type! Name:" + name);
                }
            }
            else
            {
                switch (typeSplited[1])
                {
                    case "System.String": Type = VariableType.ListString;
                        break;
                    case "System.Boolean": Type = VariableType.ListBool;
                        break;
                    case "System.Double": Type = VariableType.ListDouble;
                        break;
                    case "System.Int32": Type = VariableType.ListInt;
                        break;
                    case "System.Char": Type = VariableType.ListChar;
                        break;
                    case "System.Byte": Type = VariableType.Byte;
                        break;
                }
            }

        }

        public object GetData()
        {
            return Variable;
        }

        #endregion

        #region Overload perators

        public static bool operator &(VariableItem a, VariableItem b)
        {
            if (a.Type != b.Type) throw new Exception("Type not same for unary operation. Variable: [" + a.Name + "] - [" + b.Name + "]");

            if (a.Type == VariableType.Bool)
            {
                return ((bool)a.Variable && (bool)b.Variable);
            }
            else
            {
                throw new Exception("Types can not be compared. Variable: [" + a.Name + "] - [" + b.Name + "]");
            }

            return false;
        }


        public static bool operator |(VariableItem a, VariableItem b)
        {
            if (a.Type != b.Type) throw new Exception("Type not same for unary operation. Variable: [" + a.Name + "] - [" + b.Name + "]");

            if (a.Type == VariableType.Bool)
            {
                return ((bool)a.Variable || (bool)b.Variable);
            }
            else
            {
                throw new Exception("Types can not be compared. Variable: [" + a.Name + "] - [" + b.Name + "]");
            }

            return false;
        }



        public static bool operator >(VariableItem a, VariableItem b)
        {
            if ((a.Type == VariableType.Int && b.Type == VariableType.Double))
                return (int)a.Variable > (double)b.Variable;

            if ((b.Type == VariableType.Int && a.Type == VariableType.Double))
                return (double)a.Variable > (int)b.Variable;

            if ((a.Type == VariableType.Int && b.Type == VariableType.Byte))
                return (int)a.Variable > (byte)b.Variable;

            if ((b.Type == VariableType.Int && a.Type == VariableType.Byte))
                return (byte)a.Variable > (int)b.Variable;

            if ((a.Type == VariableType.Double && b.Type == VariableType.Byte))
                return (double)a.Variable > (byte)b.Variable;

            if ((b.Type == VariableType.Double && a.Type == VariableType.Byte))
                return (byte)a.Variable > (double)b.Variable;


            if (a.Type != b.Type) throw new Exception("Type not same for unary operation. Variable: [" + a.Name + "] - [" + b.Name + "]");




            if (a.Type == VariableType.Int || a.Type == VariableType.Double || a.Type == VariableType.Byte ||
                a.Type == VariableType.Char)
            {
                switch (a.Type)
                {
                    case VariableType.Int:
                        return (int)a.Variable > (int)b.Variable;
                    case VariableType.Double:
                        return (double)a.Variable > (double)b.Variable;
                    case VariableType.Byte:
                        return (byte)a.Variable > (byte)b.Variable;
                    case VariableType.Char:
                        return (char)a.Variable > (char)b.Variable;
                }
            }
            else
            {
                throw new Exception("Types can not be compared. Variable: [" + a.Name + "] - [" + b.Name + "]");
            }

            return false;
        }

        public static bool operator <(VariableItem a, VariableItem b)
        {

            if ((a.Type == VariableType.Int && b.Type == VariableType.Double))
                return (int)a.Variable < (double)b.Variable;

            if ((b.Type == VariableType.Int && a.Type == VariableType.Double))
                return (double)a.Variable < (int)b.Variable;

            if ((a.Type == VariableType.Int && b.Type == VariableType.Byte))
                return (int)a.Variable < (byte)b.Variable;

            if ((b.Type == VariableType.Int && a.Type == VariableType.Byte))
                return (byte)a.Variable < (int)b.Variable;

            if ((a.Type == VariableType.Double && b.Type == VariableType.Byte))
                return (double)a.Variable < (byte)b.Variable;

            if ((b.Type == VariableType.Double && a.Type == VariableType.Byte))
                return (byte)a.Variable < (double)b.Variable;


            if (a.Type != b.Type) throw new Exception("Type not same for unary operation. Variable: [" + a.Name + "] - [" + b.Name + "]");

            if (a.Type == VariableType.Int || a.Type == VariableType.Double || a.Type == VariableType.Byte ||
                a.Type == VariableType.Char)
            {
                switch (a.Type)
                {

                    case VariableType.Int:
                        return (int)a.Variable < (int)b.Variable;
                    case VariableType.Double:
                        return (double)a.Variable < (double)b.Variable;
                    case VariableType.Byte:
                        return (byte)a.Variable < (byte)b.Variable;
                    case VariableType.Char:
                        return (char)a.Variable < (char)b.Variable;
                }
            }
            else
            {
                throw new Exception("Types can not be compared. Variable: [" + a.Name + "] - [" + b.Name + "]");
            }

            return false;
        }

        public static bool operator >=(VariableItem a, VariableItem b)
        {

            if ((a.Type == VariableType.Int && b.Type == VariableType.Double))
                return (int)a.Variable >= (double)b.Variable;

            if ((b.Type == VariableType.Int && a.Type == VariableType.Double))
                return (double)a.Variable >= (int)b.Variable;

            if ((a.Type == VariableType.Int && b.Type == VariableType.Byte))
                return (int)a.Variable >= (byte)b.Variable;

            if ((b.Type == VariableType.Int && a.Type == VariableType.Byte))
                return (byte)a.Variable >= (int)b.Variable;

            if ((a.Type == VariableType.Double && b.Type == VariableType.Byte))
                return (double)a.Variable >= (byte)b.Variable;

            if ((b.Type == VariableType.Double && a.Type == VariableType.Byte))
                return (byte)a.Variable >= (double)b.Variable;



            if (a.Type != b.Type) throw new Exception("Type not same for unary operation. Variable: [" + a.Name + "] - [" + b.Name + "]");

            if (a.Type == VariableType.Int || a.Type == VariableType.Double || a.Type == VariableType.Byte ||
                a.Type == VariableType.Char)
            {
                switch (a.Type)
                {

                    case VariableType.Int:
                        return (int)a.Variable >= (int)b.Variable;
                    case VariableType.Double:
                        return (double)a.Variable >= (double)b.Variable;
                    case VariableType.Byte:
                        return (byte)a.Variable >= (byte)b.Variable;
                    case VariableType.Char:
                        return (char)a.Variable >= (char)b.Variable;

                }
            }
            else
            {
                throw new Exception("Types can not be compared. Variable: [" + a.Name + "] - [" + b.Name + "]");
            }

            return false;
        }

        public static bool operator <=(VariableItem a, VariableItem b)
        {

            if ((a.Type == VariableType.Int && b.Type == VariableType.Double))
                return (int)a.Variable <= (double)b.Variable;

            if ((b.Type == VariableType.Int && a.Type == VariableType.Double))
                return (double)a.Variable <= (int)b.Variable;

            if ((a.Type == VariableType.Int && b.Type == VariableType.Byte))
                return (int)a.Variable <= (byte)b.Variable;

            if ((b.Type == VariableType.Int && a.Type == VariableType.Byte))
                return (byte)a.Variable <= (int)b.Variable;

            if ((a.Type == VariableType.Double && b.Type == VariableType.Byte))
                return (double)a.Variable <= (byte)b.Variable;

            if ((b.Type == VariableType.Double && a.Type == VariableType.Byte))
                return (byte)a.Variable <= (double)b.Variable;



            if (a.Type != b.Type) throw new Exception("Type not same for unary operation. Variable: [" + a.Name + "] - [" + b.Name + "]");

            if (a.Type == VariableType.Int || a.Type == VariableType.Double || a.Type == VariableType.Byte ||
                a.Type == VariableType.Char)
            {
                switch (a.Type)
                {
                    case VariableType.Int:
                        return (int)a.Variable >= (int)b.Variable;
                    case VariableType.Double:
                        return (double)a.Variable >= (double)b.Variable;
                    case VariableType.Byte:
                        return (byte)a.Variable >= (byte)b.Variable;
                    case VariableType.Char:
                        return (char)a.Variable >= (char)b.Variable;

                }
            }
            else
            {
                throw new Exception("Types can not be compared. Variable: [" + a.Name + "] - [" + b.Name + "]");
            }

            return false;
        }

        public static bool operator ==(VariableItem a, VariableItem b)
        {
            if (((object)a) == null || ((object)b) == null)
            {
                return (((object)a) == null && ((object)b) == null);
            }

            if ((a.Type == VariableType.Int && b.Type == VariableType.Double))
                return (int)a.Variable == (double)b.Variable;

            if ((b.Type == VariableType.Int && a.Type == VariableType.Double))
                return (double)a.Variable == (int)b.Variable;

            if ((a.Type == VariableType.Int && b.Type == VariableType.Byte))
                return (int)a.Variable == (byte)b.Variable;

            if ((b.Type == VariableType.Int && a.Type == VariableType.Byte))
                return (byte)a.Variable == (int)b.Variable;

            if ((a.Type == VariableType.Double && b.Type == VariableType.Byte))
                return (double)a.Variable == (byte)b.Variable;

            if ((b.Type == VariableType.Double && a.Type == VariableType.Byte))
                return (byte)a.Variable == (double)b.Variable;



            if (a.Type != b.Type) throw new Exception("Type not same for unary operation. Variable: [" + a.Name + "] - [" + b.Name + "]");

            switch (a.Type)
            {
                case VariableType.Int:
                    return (int)a.Variable == (int)b.Variable;
                case VariableType.Double:
                    return (double)a.Variable == (double)b.Variable;
                case VariableType.Byte:
                    return (byte)a.Variable == (byte)b.Variable;
                case VariableType.Char:
                    return (char)a.Variable == (char)b.Variable;
                case VariableType.String:
                    return (string)a.Variable == (string)b.Variable;
                case VariableType.Bool:
                    return (bool)a.Variable == (bool)b.Variable;
            }

            return false;
        }

        public static bool operator !=(VariableItem a, VariableItem b)
        {
            if (((object)a) == null || ((object)b) == null)
            {
                return !(((object)a) == null && ((object)b) == null);
            }

            if ((a.Type == VariableType.Int && b.Type == VariableType.Double))
                return (int)a.Variable != (double)b.Variable;

            if ((b.Type == VariableType.Int && a.Type == VariableType.Double))
                return (double)a.Variable != (int)b.Variable;

            if ((a.Type == VariableType.Int && b.Type == VariableType.Byte))
                return (int)a.Variable != (byte)b.Variable;

            if ((b.Type == VariableType.Int && a.Type == VariableType.Byte))
                return (byte)a.Variable != (int)b.Variable;

            if ((a.Type == VariableType.Double && b.Type == VariableType.Byte))
                return (double)a.Variable != (byte)b.Variable;

            if ((b.Type == VariableType.Double && a.Type == VariableType.Byte))
                return (byte)a.Variable != (double)b.Variable;


            if (a.Type != b.Type) throw new Exception("Type not same for unary operation. Variable: [" + a.Name + "] - [" + b.Name + "]");

            switch (a.Type)
            {
                case VariableType.Int:
                    return (int)a.Variable != (int)b.Variable;
                case VariableType.Double:
                    return (double)a.Variable != (double)b.Variable;
                case VariableType.Byte:
                    return (byte)a.Variable != (byte)b.Variable;
                case VariableType.Char:
                    return (char)a.Variable != (char)b.Variable;
                case VariableType.String:
                    return (string)a.Variable != (string)b.Variable;
                case VariableType.Bool:
                    return (bool)a.Variable != (bool)b.Variable;
            }

            return false;
        }


        public static VariableItem operator +(VariableItem a, VariableItem b)
        {
            if (a.Type != b.Type) throw new Exception("Type not same for unary operation. Variable: [" + a.Name + "] - [" + b.Name + "]");

            if (a.Type == VariableType.Int || a.Type == VariableType.Double || a.Type == VariableType.Byte ||
                a.Type == VariableType.String)
            {
                switch (a.Type)
                {
                    case VariableType.Int:
                        return new VariableItem((int)a.Variable + (int)b.Variable);
                    case VariableType.Double:
                        return new VariableItem((double)a.Variable + (double)b.Variable);
                    case VariableType.Byte:
                        return new VariableItem((byte)a.Variable + (byte)b.Variable);
                    case VariableType.String:
                        return new VariableItem((string)a.Variable + (string)b.Variable);
                }
            }

            throw new Exception("Unary operation (+) return null !");
        }

        public static VariableItem operator -(VariableItem a, VariableItem b)
        {
            if (a.Type != b.Type) throw new Exception("Type not same for unary operation. Variable: [" + a.Name + "] - [" + b.Name + "]");

            if (a.Type == VariableType.Int || a.Type == VariableType.Double || a.Type == VariableType.Byte)
            {
                switch (a.Type)
                {
                    case VariableType.Int:
                        return new VariableItem((int)a.Variable - (int)b.Variable);
                    case VariableType.Double:
                        return new VariableItem((double)a.Variable - (double)b.Variable);
                    case VariableType.Byte:
                        return new VariableItem((byte)a.Variable - (byte)b.Variable);
                }
            }

            throw new Exception("Unary operation (+) return null !");
        }

        public static VariableItem operator /(VariableItem a, VariableItem b)
        {
            if (a.Type != b.Type) throw new Exception("Type not same for unary operation. Variable: [" + a.Name + "] - [" + b.Name + "]");

            if (a.Type == VariableType.Int || a.Type == VariableType.Double || a.Type == VariableType.Byte)
            {
                switch (a.Type)
                {
                    case VariableType.Int:
                        return new VariableItem((int)a.Variable / (int)b.Variable);
                    case VariableType.Double:
                        return new VariableItem((double)a.Variable / (double)b.Variable);
                    case VariableType.Byte:
                        return new VariableItem((byte)a.Variable / (byte)b.Variable);
                }
            }

            throw new Exception("Unary operation (+) return null !");
        }


        public static VariableItem operator *(VariableItem a, VariableItem b)
        {

            if (a.Type != b.Type || (a.Type == VariableType.String && b.Type == VariableType.Int))
                throw new Exception("Type not same for unary operation. Variable: [" + a.Name + "] - [" + b.Name + "]");

            if (a.Type == VariableType.Int || a.Type == VariableType.Double || a.Type == VariableType.Byte ||
                a.Type == VariableType.String)
            {
                switch (a.Type)
                {
                    case VariableType.Int:
                        return new VariableItem((int)a.Variable * (int)b.Variable);
                    case VariableType.Double:
                        return new VariableItem((double)a.Variable * (double)b.Variable);
                    case VariableType.Byte:
                        return new VariableItem((byte)a.Variable * (byte)b.Variable);

                    case VariableType.String:
                        string ret = string.Empty;
                        for (int i = 0; i < (int)b.Variable; i++) ret += (string)a.Variable;
                        return new VariableItem(ret);
                }
            }

            throw new Exception("Unary operation (*) return null !");
        }


        #endregion


        public override string ToString()
        {
            return GetData().ToString();
        }
    }



    public class Variables : IDisposable
    {
        private Hashtable _variableList;

        public string[] GetVariableTypeList()
        {
            return Enum.GetNames(typeof(VariableType));
        }

        public Variables()
        {
            _variableList = new Hashtable();
        }

        public void Add(VariableItem v)
        {
            _variableList.Add(v.Name, v);
        }

        public void Remove(VariableItem v)
        {
            if (v.Name !=null && _variableList.Contains(v.Name))
                _variableList.Remove(v.Name);
        }

        public bool Exist(string key)
        {
            return (_variableList[key] != null);
        }

        public VariableItem GetVariable(string name)
        {
            var retVar = ((VariableItem)_variableList[name]);
            if (retVar == null)
            {
                return new VariableItem(name);
            }
            else
            {
                return retVar;
            }
        }

        public void ClearByStatementId(string id)
        {
            var removeList =
                _variableList.Keys.Cast<object>()
                    .Where(key => ((VariableItem)_variableList[key]).StatementId == id)
                    .ToList();

            foreach (var removeKey in removeList)
            {
                _variableList.Remove(removeKey);
            }
        }

        #region Create Variable
        public void CreateVariable(string data, string statementId)
        {
            if (data.Contains("="))
            {
                // contains initializer
                var dataMas = data.Split(' ', '=');
                var dataList = new List<string>(dataMas);
                dataList.RemoveAll(i => i.Trim().Length < 1);

                var varContainer = data.Substring(data.IndexOf('=') + 1, data.Length - (data.IndexOf('=') + 1));
                //if (dataMass.Count() != 3)  throw new Exception("Variable with initializer structure error !");
                object variable = null;

                // example: string boby = "hi"
                if (string.Equals(dataList[0], "String", StringComparison.OrdinalIgnoreCase))
                    variable = varContainer.Replace('"', ' ').Trim();

                // example: int boby = 10
                if (string.Equals(dataList[0], "Int", StringComparison.OrdinalIgnoreCase))
                    variable = int.Parse(varContainer);

                // example: bool boby = false
                if (string.Equals(dataList[0], "Bool", StringComparison.OrdinalIgnoreCase))
                    variable = Boolean.Parse(varContainer);

                // example: double boby = 1.25
                if (string.Equals(dataList[0], "Double", StringComparison.OrdinalIgnoreCase))
                    variable = Double.Parse(varContainer, NumberStyles.Any);

                // example: byte boby = 125
                if (string.Equals(dataList[0], "Byte", StringComparison.OrdinalIgnoreCase))
                    variable = byte.Parse(varContainer, NumberStyles.Any);

                // example: char boby = 'a'
                if (string.Equals(dataList[0], "Byte", StringComparison.OrdinalIgnoreCase))
                    variable = char.Parse(varContainer);


                // Example: ListString Boby = {"1","1","1","1","1"}
                if (string.Equals(dataList[0], "ListString", StringComparison.OrdinalIgnoreCase))
                {
                    if (!varContainer.Contains('{') || !varContainer.Contains('}'))
                        throw new Exception("ListString initializer incorrect !");
                    variable = varContainer.Replace('{', ' ').Replace('}', ' ').Replace('"', ' ').Trim().Split(',').ToList();
                }

                // Example: ListInt Boby = {1,1,1,1,1}
                if (string.Equals(dataList[0], "ListInt", StringComparison.OrdinalIgnoreCase))
                {
                    if (!varContainer.Contains('{') || !varContainer.Contains('}'))
                        throw new Exception("ListInt initializer incorrect !");
                    var buf2 = varContainer.Replace('{', ' ').Replace('}', ' ').Trim().Split(',');
                    variable = buf2.Select(int.Parse).ToList();
                }

                // Example: ListDouble Boby = {1.56,1,1,1,1}
                if (string.Equals(dataList[0], "ListDouble", StringComparison.OrdinalIgnoreCase))
                {
                    if (!varContainer.Contains('{') || !varContainer.Contains('}'))
                        throw new Exception("ListDouble initializer incorrect !");
                    var buf2 = varContainer.Replace('{', ' ').Replace('}', ' ').Trim().Split(',');
                    variable = buf2.Select(i => double.Parse(i, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"))).ToList();
                }

                // Example: ListBool Boby = {true,false,true}
                if (string.Equals(dataList[0], "ListBool", StringComparison.OrdinalIgnoreCase))
                {
                    if (!varContainer.Contains('{') || !varContainer.Contains('}'))
                        throw new Exception("ListBool initializer incorrect !");
                    var buf2 = varContainer.Replace('{', ' ').Replace('}', ' ').Trim().Split(',');
                    variable = buf2.Select(bool.Parse).ToList();
                }

                // Example: ListByte Boby = {10,125,180}
                if (string.Equals(dataList[0], "ListByte", StringComparison.OrdinalIgnoreCase))
                {
                    if (!varContainer.Contains('{') || !varContainer.Contains('}'))
                        throw new Exception("ListByte initializer incorrect !");
                    var buf2 = varContainer.Replace('{', ' ').Replace('}', ' ').Trim().Split(',');
                    variable = buf2.Select(byte.Parse).ToList();
                }

                // Example: ListChar Boby = {'a',';','r'}
                if (string.Equals(dataList[0], "ListChar", StringComparison.OrdinalIgnoreCase))
                {
                    if (!varContainer.Contains('{') || !varContainer.Contains('}'))
                        throw new Exception("ListChar initializer incorrect !");
                    var buf2 = varContainer.Replace('{', ' ').Replace('}', ' ').Replace('\'', ' ').Trim().Split(',');
                    variable = buf2.Select(i => char.Parse(i.Trim())).ToList();
                }

                //dataMass: 0 - type, 1 - name, 2 - data
                _variableList.Add(dataList[1], new VariableItem(variable, dataList[1]) { StatementId = statementId });

            }
            else
            {
                // contains without initializer
                //var dataMass = data.Split(' ');
                var dataMas = data.Split(' ', '=');
                var dataList = new List<string>(dataMas);
                dataList.RemoveAll(i => i.Trim().Length < 1);

                if (dataList.Count() != 2) throw new Exception("Variable with initializer structure error !");
                object variable = null;
                Type varType = null;

                // example: string boby
                if (string.Equals(dataList[0], "String", StringComparison.OrdinalIgnoreCase))
                    variable = string.Empty;

                // example: int boby
                if (string.Equals(dataList[0], "Int", StringComparison.OrdinalIgnoreCase))
                    variable = new int();

                // example: bool boby
                if (string.Equals(dataList[0], "Bool", StringComparison.OrdinalIgnoreCase))
                    variable = new bool();

                // example: double boby
                if (string.Equals(dataList[0], "Double", StringComparison.OrdinalIgnoreCase))
                    variable = new double();

                // Example: ListString Boby
                if (string.Equals(dataList[0], "ListString", StringComparison.OrdinalIgnoreCase))
                    variable = new List<string>();

                // Example: ListString Boby
                if (string.Equals(dataList[0], "ListInt", StringComparison.OrdinalIgnoreCase))
                {
                    if (!dataList[3].Contains('{') || !dataList[3].Contains('}'))
                        throw new Exception("ListInt initializer incorrect !");
                    var buf2 = dataList[3].Replace('{', ' ').Replace('}', ' ').Trim().Split(',');
                    variable = buf2.Select(int.Parse).ToList();
                }

                //dataMass: 0 - type, 1 - name, 2 - data
                _variableList.Add(dataList[1], new VariableItem(variable, dataList[1]) { StatementId = statementId });

            }
        }

        #endregion

        #region VariablesProperties



        #endregion

        #region Despose patternt
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Implement dispose pattern
        public void Dispose(bool managed)
        {
            if (managed)
            {
                if (_variableList != null)
                {
                    _variableList.Clear();
                    _variableList = null;
                }
            }
        }
        #endregion
    }
}
