using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SrbRuby
{
    internal class Functions : IDisposable
    {
        private Commands _commands;
        private readonly FunctionItem _currentFunc;
        private readonly List<FunctionItem> _funcList;
        private Variables _variables;
        private Hashtable _jumpList;

        public delegate void ExecuteCode(string function, string command);
        public event ExecuteCode ExecuteCodeEvent = delegate { };


        public Functions(FunctionItem function, List<FunctionItem> functionList)
        {
            _variables = new Variables();
            _currentFunc = function;
            _commands = new Commands(_variables);
            _jumpList = new Hashtable();
            _funcList = functionList;
        }

        private int? IsJumpTo(int pos)
        {
            if (_jumpList.ContainsKey(pos))
            {
                if (_jumpList[pos] is int)
                {
                    var p = (int)_jumpList[pos];
                    _jumpList.Remove(pos);
                    return p;
                }

                if (_jumpList[pos] is List<int>)
                {
                    var p = (List<int>)_jumpList[pos];
                    if (p.Count > 1)
                    {
                        var r = p.Last();
                        p.Remove(r);
                        return r;
                    }
                    else
                    {
                        var r = p.Last();
                        p.Remove(r);
                        _jumpList.Remove(pos);
                        return r;
                    }
                }

            }
            return null;
        }

        public VariableItem Execute(List<VariableItem> vars)
        {
	        if (vars != null && vars.Count > 0) 
				foreach (var variableItem in vars)_variables.Add(variableItem);

	        var lastItem = new VariableItem("false");
            var managerialWords = new List<string>() { "if ", "elseif ", "while ", "for " };
            var statementList = new List<string> { _currentFunc.Id.ToString() };
            var commandList = _commands.GetCommandNameList();

            for (var pos = 0; pos < _currentFunc.Code.Count; pos++)
            {
                int? newPos;
                if ((newPos = IsJumpTo(pos)) != null) { pos = (int)newPos; continue; }

                var codeItem = _currentFunc.Code[pos];
                ExecuteCodeEvent(_currentFunc.Name, codeItem);

                // remove comment string
                if (codeItem.Contains("#"))
                {
                    codeItem = codeItem.Substring(codeItem.IndexOf("#"), codeItem.Length - codeItem.IndexOf("#")).Trim();
                    if (codeItem.Length < 1) continue;
                }


                // Detect managed words
                if (managerialWords.Any(codeItem.Contains))
                {
                    /* Execute jumping operators */
                    CalculateJumpOper(pos, ref statementList);
                    if ((newPos = IsJumpTo(pos)) != null) { pos = (int)newPos; }
                    continue;
                }


                // Code simplification
				lastItem = Simplification(pos);





                
                // Remove last statement id
                if (codeItem.Contains("end"))
                {
                    _variables.ClearByStatementId(statementList[statementList.Count - 1]);
                    statementList.Remove(statementList[statementList.Count - 1]);
                }
            }

            return lastItem;
        }

        private VariableItem Simplification(int pos)
        {
            string codeItem = _currentFunc.Code[pos];
            var variableTypeList = _variables.GetVariableTypeList();
            var commandList = _commands.GetCommandNameList();
            var calculateElements = new List<string>() { "=", ">", "<", ">=", "<=", "==", "!=", "+", "-", "/", "*" };
	        //VariableItem ret

            // Find function
            foreach (var item in _funcList)
            {
                if (codeItem.Contains(item.Name))
                {
                    var first = codeItem.IndexOf("(", codeItem.IndexOf(item.Name));
                    if (first == -1) throw new Exception("Error not found parameter block functions");

                    var last = codeItem.IndexOf(")", first);
                    if (last == -1) throw new Exception("Error not found parameter block functions");

                    //codeItem = codeItem.Substring(0, first) + "|" + codeItem.Substring(first + 1, codeItem.Length - (first + 1));
                    //codeItem = codeItem.Substring(0, last) + "|" + codeItem.Substring(last + 1, codeItem.Length - (last + 1));

                    // split parameters
                    var funcParams = codeItem.Substring(first+1,last - (first+1));
                    //if (calculateElements.Any(funcParams.Contains))
                    //{
                        var funcParamsList = new List<string>(funcParams.Split(','));
                        var varListParams = funcParamsList.Select(SimlifyExpressionByParenthesis).ToList();

	                    if (varListParams.Count != item.Parameters.Count)
	                    {
							throw new Exception("Not match parameters for function: " + item.Name);
	                    }

						for (int i = 0; i < varListParams.Count; i++) varListParams[i].Name = item.Parameters[i]; 
	                    var ret = (new Functions(_funcList.FirstOrDefault(i => i.Id == item.Id), _funcList).Execute(varListParams));

						_variables.Add(ret);
	                    codeItem = codeItem.Substring(0, codeItem.IndexOf(item.Name)) + ret.Name +
								   codeItem.Substring(last+1, codeItem.Length - (last+1));
                    //}

                }
            }




            // Find command
            foreach (var item in _commands.GetCommandNameList())
            {
                if (codeItem.Contains(item))
                {
                    var first = codeItem.IndexOf("(", codeItem.IndexOf(item))+1;
                    if (first == -1) throw new Exception("Error not found parameter block functions");


					//TODO: if many () ?
                    var last = codeItem.IndexOf(")", first);
                    if (last == -1) throw new Exception("Error not found parameter block functions");

                    //codeItem = codeItem.Substring(0, first) + "|" + codeItem.Substring(first + 1, codeItem.Length - (first + 1));
                    //codeItem = codeItem.Substring(0, last) + "|" + codeItem.Substring(last + 1, codeItem.Length - (last + 1));

                    // split parameters
					var funcParamsList = new List<string>(codeItem.Substring(first, last - first).Split(','));
                    var varListParams = funcParamsList.Select(SimlifyExpressionByParenthesis).ToList();
                    var ret = _commands.Execute(item, varListParams);

					_variables.Add(ret);
					codeItem = codeItem.Substring(0, codeItem.IndexOf(item)) + ret.Name +
								   codeItem.Substring(last+1, codeItem.Length - (last+1));
                }
            }



            // Simplify calculate block


			return SimlifyExpressionByParenthesis(codeItem);
        }




        private void CalculateJumpOper(int pos, ref List<string> statementList)
        {

            var codeItem = _currentFunc.Code[pos];
           

            statementList.Add(Guid.NewGuid().ToString());
            var cmdList = new List<string>(codeItem.Split(' '));
            cmdList.RemoveAll(i => i.Length < 1);

            // ******************************
            // IF

            if (cmdList[0].Equals("if", StringComparison.OrdinalIgnoreCase))
            {
                cmdList.RemoveAll(i => i == "if");
                string exp = string.Empty;
                exp = cmdList.Aggregate(exp, (c, i) => c += " " + i + " ");

                if (SimlifyExpressionByParenthesis(exp) == new VariableItem("true"))
                {
                    // execute true block 
                    var elseBlock = _currentFunc.Code.IndexOf("else", pos);
                    var endBlock = _currentFunc.Code.IndexOf("end", pos);
                    if (elseBlock != -1) 
                        _jumpList[elseBlock] = new List<int> { endBlock, endBlock, endBlock };
                }
                else
                {
                    // Go to else block or end
                    var elseBlock = _currentFunc.Code.IndexOf("else", pos);
                    var endBlock = _currentFunc.Code.IndexOf("end", pos);
                    _jumpList.Add(pos, elseBlock != -1 ? elseBlock : endBlock);
                }
            }







        }


        #region Work with expression

        private List<int> GetDeepExpList(List<string> exp)
        {
            var ret =new List<int>();

            if (exp.Any(i => i.Contains("(")))
            {
                var start = exp.LastIndexOf("(");
                var end = exp.IndexOf(")", start);
                if (start != -1 && end == -1) throw new Exception("Can't find end parenthesis");
                for (int i = start; i <= end; i++) { ret.Add(i); }
            }
            else
            {
                return null;
            }
            return ret;
        }

        private VariableItem SimlifyExpressionByParenthesis(string codeItem)
        {
            var separateElements = new List<string>() { "(", ")", "{", "}", ">=", "<=", "==", "!=", "+", "-", "/", "*" };
            codeItem = separateElements.Aggregate(codeItem, (current, i) => current.Replace(i, " " + i + " "));

            int lastItem = -1;
            while ((lastItem = codeItem.IndexOf('=', lastItem+1)) != -1)
            {
                if (codeItem[lastItem - 1] != '<' && codeItem[lastItem - 1] != '>' &&
                    codeItem[lastItem - 1] != '=' && codeItem[lastItem + 1] != '=')
                {
                    codeItem = codeItem.Substring(0, lastItem) + " = " +
                               codeItem.Substring(lastItem + 1, codeItem.Length - (lastItem + 1));
                    lastItem += 2;
                }
            }

            lastItem = -1;
            while ((lastItem = codeItem.IndexOf('>', lastItem + 1)) != -1)
            {
                if (codeItem[lastItem + 1] != '=')
                {
                    codeItem = codeItem.Substring(0, lastItem) + " > " +
                               codeItem.Substring(lastItem + 1, codeItem.Length - (lastItem + 1));
                    lastItem += 2;
                }
            }

            lastItem = -1;
            while ((lastItem = codeItem.IndexOf('<', lastItem + 1)) != -1)
            {
                if (codeItem[lastItem + 1] != '=')
                {
                    codeItem = codeItem.Substring(0, lastItem) + " < " +
                               codeItem.Substring(lastItem + 1, codeItem.Length - (lastItem + 1));
                    lastItem += 2;
                }
            }



            var cmdList = new List<string>(codeItem.Split(' '));
            List<int> buf;
            while ((buf = GetDeepExpList(cmdList)) != null)
            {
                var exp = buf.Select(i => cmdList[i]).ToList();
                exp.RemoveAll(i => i == "(" || i == ")");
                cmdList[buf[0]] = SimplifyExpression(exp).Name;
                for (int i = 1; i < buf.Count; i++) cmdList[buf[i]] = "";
                cmdList.RemoveAll(i => i.Length < 1);
            }

            return SimplifyExpression(cmdList);
        }

        private VariableItem SimplifyExpression(List<string> exp)
        {
            var cmdList = exp;


            for (int i = 0; i < cmdList.Count; i++)
            {
                if (cmdList[i].Equals("-") || cmdList[i].Equals("+") ||
                    cmdList[i].Equals("*") || cmdList[i].Equals("/"))
                {
                    var o1 = _variables.GetVariable(cmdList[i - 1]);
                    var o2 = _variables.GetVariable(cmdList[i + 1]);
                    VariableItem rez;

                    switch (cmdList[i])
                    {
                        case "+": rez = o1 + o2; break;
                        case "-": rez = o1 - o2; break;
                        case "*": rez = o1 * o2; break;
                        case "/": rez = o1 / o2; break;
                        default: throw new Exception("MAGIC!");
                    }
                    cmdList[i - 1] = "";
                    cmdList[i + 1] = "";
                    cmdList[i] = (rez.Name = Guid.NewGuid().ToString().Replace("-", ""));
                    _variables.Add(rez);
                    cmdList.RemoveAll(j => j.Length < 1);
                }
            }

            cmdList.RemoveAll(i => i.Length < 1);


            for (int i = 0; i < cmdList.Count; i++)
            {
                if (cmdList[i].Equals(">") || cmdList[i].Equals(">=") ||
                    cmdList[i].Equals("<") || cmdList[i].Equals("<=") ||
                    cmdList[i].Equals("==") || cmdList[i].Equals("!="))
                {
                    var o1 = _variables.GetVariable(cmdList[i - 1]);
                    var o2 = _variables.GetVariable(cmdList[i + 1]);
                    VariableItem rez;

                    switch (cmdList[i])
                    {
                        case ">": rez = new VariableItem(o1 > o2); break;
                        case ">=": rez = new VariableItem(o1 >= o2); break;
                        case "<": rez = new VariableItem(o1 < o2); break;
                        case "<=": rez = new VariableItem(o1 <= o2); break;
                        case "==": rez = new VariableItem(o1 == o2); break;
                        case "!=": rez = new VariableItem(o1 != o2); break;
                        default: throw new Exception("MAGIC!");
                    }
                    cmdList[i - 1] = "";
                    cmdList[i + 1] = "";
                    cmdList[i] = (rez.Name = Guid.NewGuid().ToString().Replace("-", ""));
                    _variables.Add(rez);
                    cmdList.RemoveAll(j => j.Length < 1);
                }
            }

            cmdList.RemoveAll(i => i.Length < 1);



            int y = 0;
            while (true)
            {
                if (!cmdList.Any(i => i.Equals("and", StringComparison.OrdinalIgnoreCase) ||
                                     i.Equals("or", StringComparison.OrdinalIgnoreCase)))
                    break;

                if (cmdList[y].Equals("and", StringComparison.OrdinalIgnoreCase) ||
                    cmdList[y].Equals("or", StringComparison.OrdinalIgnoreCase))
                {
                    var o1 = _variables.GetVariable(cmdList[y - 1]);
                    var o2 = _variables.GetVariable(cmdList[y + 1]);
                    VariableItem rez;

                    switch (cmdList[y])
                    {
                        case "and":
                            rez = new VariableItem(o1 & o2);
                            break;
                        case "or":
                            rez = new VariableItem(o1 | o2);
                            break;
                        default:
                            throw new Exception("MAGIC!");
                    }

                    cmdList[y - 1] = "";
                    cmdList[y + 1] = "";
                    cmdList[y] = (rez.Name = Guid.NewGuid().ToString().Replace("-", ""));
                    _variables.Add(rez);
                    cmdList.RemoveAll(j => j.Length < 1);
                    y = 0;
                }
                else y++;
            }

            cmdList.RemoveAll(i => i.Length < 1);


            for (int i = 0; i < cmdList.Count; i++)
            {
                if (cmdList[i].Equals("="))
                {
                    VariableItem rez;
                   
                    if (_variables.Exist(cmdList[i - 1]))
                    {
                        var o1 = _variables.GetVariable(cmdList[i - 1]);
                        var o2 = _variables.GetVariable(cmdList[i + 1]);
                        rez = new VariableItem(o2.GetData(),o1.Name);
                        _variables.Remove(o2);
                        _variables.Remove(o1);
                    }
                    else
                    {
                        var o2 = _variables.GetVariable(cmdList[i + 1]);
                        rez = new VariableItem(o2.GetData(), cmdList[i - 1]);
                        _variables.Remove(o2);
                    }
                   
                    cmdList[i - 1] = "";
                    cmdList[i + 1] = "";
                    cmdList[i] = rez.Name;
                    _variables.Add(rez);
                    cmdList.RemoveAll(j => j.Length < 1);
                }
            }

            cmdList.RemoveAll(i => i.Length < 1);


            if (cmdList.Count == 1)
            {
                return _variables.GetVariable(cmdList[0]);
            }
            else
            {
                throw new Exception("BUG!");
            }
        }


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
                if (_jumpList != null)
                {
                    _jumpList.Clear();
                    _jumpList = null;
                }

                if (_variables != null)
                {
                    _variables.Dispose();
                    _variables = null;
                }

                if (_commands != null)
                {
                    _commands.Dispose();
                    _commands = null;
                }
            }
        }
        #endregion
    }
}
