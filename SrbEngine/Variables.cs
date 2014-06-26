using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using SrbEngine;
using SrbEngine.Class.Variables;

namespace SrbRuby
{

    public class VariableItem
    {
        public string StatementId { get; set; }
        public string Name { get; set; }
        private IClass Variable { get; set; }
	    private string ClassName { get; set; }

	    public object Data {
            get { return Variable.Data(); }
        }

        #region Regular

		protected bool Equals(VariableItem other)
        {
            return string.Equals(StatementId, other.StatementId) &&
                string.Equals(Name, other.Name) &&
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
                hashCode = (hashCode * 397) ^ (Variable != null ? Variable.GetHashCode() : 0);
                return hashCode;
            }
        }
		#endregion

		public VariableItem(object ob, string name)
        {
			//TODO: detect class type !
			this.Name = (name ?? Guid.NewGuid().ToString().Replace("-", ""));
			if(ob is IClass) Variable = (IClass)ob;
        }

		public VariableItem(string ob, string name)
		{
			this.Name = (name ?? Guid.NewGuid().ToString().Replace("-", ""));
			Initialize(ob);
		}

		public VariableItem(IClass ob, string name)
		{
			this.Name = (name ?? Guid.NewGuid().ToString().Replace("-", ""));
			if (ob is IClass) Variable = (IClass)ob;
		}

        public VariableItem(object ob)
        {
			this.Name = Guid.NewGuid().ToString().Replace("-","");
			if (ob is IClass) Variable = (IClass)ob;
        }

        public VariableItem(string var)
        {
			this.Name = Guid.NewGuid().ToString().Replace("-", "");
            Initialize(var);
        }

		public void Initialize(string var)
        {
			var = var.Trim();

		    if (var.Contains(".new"))
		    {
		        Variable = GLOBALS.ClassesList.Get(var.Replace(".new", ""));
                return;
		    }

			foreach (var @class in GLOBALS.ClassesList.List)
			{
				object newVar = null;
				if ((newVar = @class.Parse(var)) != null)
				{
					Variable = (IClass)newVar;
					ClassName = Variable.Name();
					return;
				}
			}

            throw new Exception("Can not create variable: " + var);
        }

        #region Overload perators

	    public static bool operator &(VariableItem a, VariableItem b)
	    {
		    object ret;
		    if ((ret = ((IClass) a.Variable).Operator("&", b)) != null)
		    {
			    return (bool)ret;
		    }
		    else
		    {
				throw new Exception("Types can not be compared. Variable: [" + a + "] - [" + b + "]");
		    }
        }


        public static bool operator |(VariableItem a, VariableItem b)
        {
			object ret;
			if ((ret = ((IClass)a.Variable).Operator("|", b)) != null)
			{
				return (bool)ret;
			}
			else
			{
				throw new Exception("Types can not be compared. Variable: [" + a + "] - [" + b + "]");
			}
        }

        public static bool operator >(VariableItem a, VariableItem b)
        {
			object ret;
			if ((ret = ((IClass)a.Variable).Operator(">", b)) != null)
			{
				return (bool)ret;
			}
			else
			{
				throw new Exception("Types can not be compared. Variable: [" + a + "] - [" + b + "]");
			}
        }

        public static bool operator <(VariableItem a, VariableItem b)
        {
			object ret;
			if ((ret = ((IClass)a.Variable).Operator("<", b)) != null)
			{
				return (bool)ret;
			}
			else
			{
				throw new Exception("Types can not be compared. Variable: [" + a + "] - [" + b + "]");
			}
        }

        public static bool operator >=(VariableItem a, VariableItem b)
        {
			object ret;
			if ((ret = ((IClass)a.Variable).Operator(">=", b)) != null)
			{
				return (bool)ret;
			}
			else
			{
				throw new Exception("Types can not be compared. Variable: [" + a + "] - [" + b + "]");
			}
        }

        public static bool operator <=(VariableItem a, VariableItem b)
        {

			object ret;
			if ((ret = ((IClass)a.Variable).Operator("<=", b)) != null)
			{
				return (bool)ret;
			}
			else
			{
				throw new Exception("Types can not be compared. Variable: [" + a + "] - [" + b + "]");
			}
        }

        public static bool operator ==(VariableItem a, VariableItem b)
        {
			object ret;
			if ((ret = ((IClass)a.Variable).Operator("==", b)) != null)
			{
				return (bool)ret;
			}
			else
			{
				throw new Exception("Types can not be compared. Variable: [" + a + "] - [" + b + "]");
			}
        }

        public static bool operator !=(VariableItem a, VariableItem b)
        {
			object ret;
			if ((ret = ((IClass)a.Variable).Operator("!=", b)) != null)
			{
				return (bool)ret;
			}
			else
			{
				throw new Exception("Types can not be compared. Variable: [" + a + "] - [" + b + "]");
			}
        }


        public static VariableItem operator +(VariableItem a, VariableItem b)
        {
			object ret;
			if ((ret = ((IClass)a.Variable).Operator("+", b)) != null)
			{
				return new VariableItem(ret);
			}
			else
			{
				throw new Exception("Types can not be compared. Variable: [" + a + "] - [" + b + "]");
			}
        }

        public static VariableItem operator -(VariableItem a, VariableItem b)
		{
			object ret;
			if ((ret = ((IClass)a.Variable).Operator("-", b)) != null)
			{
				return new VariableItem(ret);
			}
			else
			{
				throw new Exception("Types can not be compared. Variable: [" + a + "] - [" + b + "]");
			}
        }

        public static VariableItem operator /(VariableItem a, VariableItem b)
        {
			object ret;
			if ((ret = ((IClass)a.Variable).Operator("/", b)) != null)
			{
				return new VariableItem(ret);
			}
			else
			{
				throw new Exception("Types can not be compared. Variable: [" + a + "] - [" + b + "]");
			}
        }


        public static VariableItem operator *(VariableItem a, VariableItem b)
        {
			object ret;
			if ((ret = ((IClass)a.Variable).Operator("*", b)) != null)
			{
				return new VariableItem(ret);
			}
			else
			{
				throw new Exception("Types can not be compared. Variable: [" + a + "] - [" + b + "]");
			}
        }

        #endregion

        public override string ToString()
        {
			return Variable.ToString();
        }
    }



    public class Variables : IDisposable
    {
        private Hashtable _variableList;

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

			// Check for request data from cass
			if (name.Contains("."))
			{
				if (name.Contains(".new"))
				{
					return new VariableItem(name);
				}
				else
				{
					var varName = name.Substring(0, name.IndexOf('.'));
					var classItem = ((VariableItem)(name.Contains("@") ? GLOBALS.Variables[varName] : _variableList[varName])).Data;

					if (name.Contains("("))
					{
						var functionName = name.Substring(name.IndexOf('.')+1, name.IndexOf('(') - (name.IndexOf('.')+1));
						var paramList = name.Substring(name.IndexOf('(')+1, name.IndexOf(')') - (name.IndexOf('(')+1)).Split(',');
						var paramVariables = paramList.Select(GetVariable).ToList();
						return ((IClass)classItem).Function(functionName, paramVariables);
					}
					else
					{
						return ((IClass)classItem).Properties(name.Replace(((IClass)classItem).Name() + ".", ""));
					}
				}
			}


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
