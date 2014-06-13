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
        ListString, ListInt, ListBool, ListDouble, ListByte, ListChar, 
		Nil, Other
    }

    public class VariableItem
    {

		public class Nil
		{
			public override string ToString()
			{
				return "nil";
			}
		}

        public string StatementId { get; set; }
        public string Name { get; set; }
        public VariableType Type { get; set; }

		#region Regular

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
		#endregion

		public VariableItem(object ob, string name)
        {
			this.Name = (name ?? Guid.NewGuid().ToString().Replace("-", ""));
            Set(ob);
            
        }

        public VariableItem(object ob)
        {
			this.Name = Guid.NewGuid().ToString().Replace("-","");
            Set(ob);
           
        }

        public VariableItem(string var)
        {
			this.Name = Guid.NewGuid().ToString().Replace("-", "");
            Initialize(var);
        }

		#region Initialize.

		public void Initialize(string var)
        {
            //var var = _stringVariableForInit;
            // if true|false bool
			var = var.Trim();

            if (var == "true" || var == "false")
            {
                bool b = (var == "true");
                Set(b);
                return;
            }

			if (var.ToLower() == "nil")
			{
				Set(new Nil());
				return;
			}


            // if ' - char
            if (var.Contains("'"))
            {
                char d;
                if (char.TryParse(var.Replace("'", "").Trim(), out d))
                {
                    Set(d);
                    return;
                }
            }

            // if " - string
            if (var.Contains("\""))
            {
                Set(var.Replace("\"", "").Trim());
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
                        Set(d);
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
                    Set(b);
                    return;
                }
            }

            // int
            if (var.All(Char.IsDigit))
            {
                Int32 i;
                if (Int32.TryParse(var, out i))
                {
                    Set(i);
                    return;
                }
            }

            throw new Exception("Can not create variable: " + var);
        }

		#endregion

		#region Emplements variable.


		private object Variable { get; set; }

        public void Set(object ob)
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
                        throw new Exception("Not found variable type! Name:");
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
            if ((a.Type != b.Type) && (a.Type == VariableType.String || b.Type == VariableType.String))
            {
                if (a.Type == VariableType.String)
                    return new VariableItem("\"" + a + b + "\"");

                if (b.Type == VariableType.String)
                    return new VariableItem("\"" + a + b + "\"");
            }


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
                        return new VariableItem("\""+(string)a.Variable + (string)b.Variable+"\"");
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

		public void Add(VariableItem v, string statementId)
		{
			v.StatementId = statementId;

			if (v.Name.Contains("@"))
			{
				if (GLOBALS.Variables.Contains(v.Name))
					GLOBALS.Variables[v.Name] = v;
				else
					GLOBALS.Variables.Add(v.Name, v);
			}

			if (_variableList.Contains(v.Name)) _variableList[v.Name]= v; else _variableList.Add(v.Name, v);
        }

		public VariableItem Create(string v, string name, string statementId)
		{
			var newvar = new VariableItem(v, name);
			Add(newvar, statementId);
			return newvar;
		}

		public VariableItem Create(object v, string name, string statementId)
		{
			var newvar = new VariableItem(v, name);
			Add(newvar, statementId);
			return newvar;
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
            if (name.Contains("@"))
            {
                /* GLOBAL VARIABLE */
                var retVar = ((VariableItem) GLOBALS.Variables[name]);
                if (retVar == null)
                {
                    return new VariableItem(name);
                }
                else
                {
                    return retVar;
                }
            }
            else
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
