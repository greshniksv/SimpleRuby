using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SrbRuby
{


    public class FunctionItem : ICloneable
    {
        public string Name { get; set; }
        public List<string> Code { get; set; }
        public Guid Id { get; set; }
        public List<string> Parameters { get; set; }

        public object Clone()
        {
            return new FunctionItem()
            {
                Name = this.Name,
                Code = new List<string>(this.Code),
                Id = new Guid(Id.ToString()),
                Parameters = new List<string>(Parameters)
            };
        }
    }



    internal class Functions : IDisposable
    {
        private Commands _commands;
        private readonly FunctionItem _currentFunc;
        private Variables _variables;
        private Hashtable _jumpList;
        private List<string> _statementList;

        public delegate void ExecuteCode(string function, string command);
        public event ExecuteCode ExecuteCodeEvent = delegate { };


        public Functions(FunctionItem function)
        {
            _variables = new Variables();
            _currentFunc = function;
            _commands = new Commands(_variables);
            _jumpList = new Hashtable();
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
				foreach (var variableItem in vars)_variables.Add(variableItem,_currentFunc.Id.ToString());

	        var lastItem = new VariableItem("false");
            var managerialWords = new List<string>() { "if ", "elseif ", "while ", "for " };
            _statementList = new List<string> { _currentFunc.Id.ToString() };
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
                    CalculateJumpOper(pos, ref _statementList);
                    if ((newPos = IsJumpTo(pos)) != null) { pos = (int)newPos; }
                    continue;
                }


                // Code simplification
				lastItem = Simplification(pos);





                
                // Remove last statement id
                if (codeItem.Contains("end"))
                {
                    _variables.ClearByStatementId(_statementList[_statementList.Count - 1]);
                    _statementList.Remove(_statementList[_statementList.Count - 1]);
                }
            }

            return lastItem;
        }


        private int GetLastFunctionParenthesis(string data, int start)
        {
            int parenthesisIdent = 0;

            for (int i = start; i < data.Length; i++)
            {
                if (data[i] == '(') parenthesisIdent++;
                if (data[i] == ')')
                {
                    if (parenthesisIdent > 1) parenthesisIdent--; else return i;
                }
            }
            return -1;
        }


        struct FindedItemWithParenthesisItem
        {
            public string Name { get; set; }
            public int Start { get; set; }
            public int End { get; set; }
        }

        private FindedItemWithParenthesisItem? FindItemWithParenthesis(string data, IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                int start = 0;
                if ((start=data.IndexOf(item,start))!=-1)
                {

                    if (start > 0)
                    {
                        if (data[start - 1] == ' ' &&
                            (data[start + item.Length] == ' ' || data[start + item.Length] == '('))
                        {
                            return new FindedItemWithParenthesisItem
                            {
                                Name = item,
								Start = start + item.Length,
                                End = GetLastFunctionParenthesis(data, start)
                            };
                        }
                    }
                    else
                    {
                        if ((data[start + item.Length] == ' ' || data[start + item.Length] == '('))
                        {
                            return new FindedItemWithParenthesisItem
                            {
                                Name = item,
                                Start = start + item.Length,
                                End = GetLastFunctionParenthesis(data, start)
                            };
                        }
                    }

                }
            }
            return null;
        }


        private VariableItem Simplification(int pos)
        {
            string codeItem = _currentFunc.Code[pos];
            var variableTypeList = _variables.GetVariableTypeList();
            var commandList = _commands.GetCommandNameList();
            var calculateElements = new List<string>() { "=", ">", "<", ">=", "<=", "==", "!=", "+", "-", "/", "*" };

            codeItem = StringProcessor(codeItem);

            // Find function
            var function = FindItemWithParenthesis(codeItem, GLOBALS.Functions.Select(i => i.Name).ToList());
            if (function != null)
            {
                var first = function.Value.Start;
                var last = function.Value.End;
                var item = GLOBALS.Functions.FirstOrDefault(i => i.Name == function.Value.Name);
                var funcParams = codeItem.Substring(first+1, last - (first+1));
                var funcParamsList = new List<string>(funcParams.Split(','));
                var varListParams = funcParamsList.Select(SimlifyExpressionByParenthesis).ToList();

                if (varListParams.Count != item.Parameters.Count)
                {
                    throw new Exception("Not match parameters for function: " + item.Name);
                }

                for (int i = 0; i < varListParams.Count; i++) varListParams[i].Name = item.Parameters[i];
                var ret = (new Functions(GLOBALS.Functions.FirstOrDefault(i => i.Id == item.Id)).Execute(varListParams));
                _variables.Add(ret,_statementList.Last());

				codeItem = codeItem.Substring(0, first - item.Name.Length) + ret.Name +
						   codeItem.Substring(last + 1, codeItem.Length - (last + 1));
            }








            // Find command
            var command = FindItemWithParenthesis(codeItem, _commands.GetCommandNameList());
            if (command != null)
            {
                var first = command.Value.Start;
                var last = command.Value.End;
                var item = command.Value.Name;
                var funcParams = codeItem.Substring(first+1, last - (first+1));
                var funcParamsList = new List<string>(funcParams.Split(','));
                var varListParams = funcParamsList.Select(SimlifyExpressionByParenthesis).ToList();

                var ret = _commands.Execute(item, varListParams);
                _variables.Add(ret,_statementList.Last());

                codeItem = codeItem.Substring(0, first-item.Length) + ret.Name +
                            codeItem.Substring(last + 1, codeItem.Length - (last + 1));
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


        #region StringProcessor
        public string StringProcessor(string codeItem)
        {

            // Find constant string and create variable
            int lastPos = 0;
            int start = 0;
            while ((lastPos = codeItem.IndexOf("'", lastPos)) != -1)
            {
                lastPos++;
                if (codeItem[lastPos - 2] == '\\') continue;

                if (start == 0) start = lastPos;
                else
                {
                    var s = codeItem.Substring(start, lastPos - start - 1);
                    s = s.Replace("\\'", "'").Replace("\\\"", "\"");
                    var v = _variables.Create(s, null, _statementList.Last());

                    codeItem = codeItem.Substring(0, start - 1) + v.Name +
                           codeItem.Substring(lastPos, codeItem.Length - (lastPos));

                    start = 0;
                    lastPos = 0;
                }
            }

            if (start != 0) throw new Exception("the number of characters [ ' ] must be even ");


            int firstPos = 0;
            while ((firstPos = codeItem.IndexOf("%q[")) != -1)
            {
                int lastpos = codeItem.IndexOf("]");
                if (lastpos == -1) throw new Exception("Can't find and of expression %q[] ");

                var s = codeItem.Substring(firstPos+3, lastpos - firstPos - 3);
                s = s.Replace("\\'", "'").Replace("\\\"", "\"");
                var v = _variables.Create(s, null, _statementList.Last());

                codeItem = codeItem.Substring(0, firstPos) + v.Name +
                       codeItem.Substring(lastpos, codeItem.Length - (lastpos+1));
            }



            // Find dynamic string, process and create variable

            lastPos = 0;
            start = 0;
            while ((lastPos = codeItem.IndexOf("\"", lastPos)) != -1)
            {
                lastPos++;
                if (codeItem[lastPos - 2] == '\\') continue;

                if (start == 0) start = lastPos;
                else
                {
                    var s = codeItem.Substring(start, lastPos - start - 1);
                    s = s.Replace("\\'", "'").Replace("\\\"", "\"");

                    // find block #{?} and process
                    while (s.Contains("#{"))
                    {
                        var startb = s.IndexOf("#{");
                        var endb = s.IndexOf("}");
                        var varData = SimlifyExpressionByParenthesis(s.Substring(startb + 2, endb - (startb + 2))).ToString();
                        s = s.Substring(0, startb) + varData + s.Substring(endb + 1, s.Length - (endb + 1));
                    }
                    var v = _variables.Create(s, null, _statementList.Last());

                    codeItem = codeItem.Substring(0, start - 1) + v.Name +
                           codeItem.Substring(lastPos, codeItem.Length - (lastPos));

                    start = 0;
                    lastPos = 0;
                }

            }

            if (start != 0) throw new Exception("the number of characters [ \" ] must be even ");



            // %Q[] and %q[]
            firstPos = 0;
            while ((firstPos = codeItem.IndexOf("%Q[")) != -1 || 
                (firstPos = codeItem.IndexOf("%[")) != -1)
            {
                var startp = (codeItem[firstPos+1] == 'Q' ? 3 : 2);
                int lastpos = codeItem.IndexOf("]");
                if (lastpos == -1) throw new Exception("Can't find and of expression %Q[] or %[] ");

                var s = codeItem.Substring(firstPos + startp, lastpos - firstPos - startp);
                s = s.Replace("\\'", "'").Replace("\\\"", "\"");

                while (s.Contains("#{"))
                {
                    var startb = s.IndexOf("#{");
                    var endb = s.IndexOf("}");
                    var varData = SimlifyExpressionByParenthesis(s.Substring(startb + 2, endb - (startb + 2))).ToString();
                    s = s.Substring(0, startb) + varData + s.Substring(endb + 1, s.Length - (endb + 1));
                }

                var v = _variables.Create(s, null, _statementList.Last());

                codeItem = codeItem.Substring(0, firstPos) + v.Name +
                       codeItem.Substring(lastpos, codeItem.Length - (lastpos + 1));
            }








            // %Q{} and %q{}
            firstPos = 0;
            while ((firstPos = codeItem.IndexOf("%Q{")) != -1 ||
                (firstPos = codeItem.IndexOf("%{")) != -1)
            {
                var startp = (codeItem[firstPos+1] == 'Q' ? 3 : 2);

                var brace = 0;
                var lastpos = 0;

                // find end
                for (int i = firstPos; i < codeItem.Length; i++)
                {
                    if (codeItem[i] == '{') brace++;
                    if (codeItem[i] == '}' && brace == 1)
                    {
                        lastpos = i; break;
                    }
                    if (codeItem[i] == '}') brace--; 
                }

                if (lastpos == -1) throw new Exception("Can't find and of expression %Q{} or %{} ");

                var s = codeItem.Substring(firstPos + startp, lastpos - firstPos - startp);
                s = s.Replace("\\'", "'").Replace("\\\"", "\"");

                while (s.Contains("#{"))
                {
                    var startb = s.IndexOf("#{");
                    var endb = s.IndexOf("}");
                    var varData = SimlifyExpressionByParenthesis(s.Substring(startb + 2, endb - (startb + 2))).ToString();
                    s = s.Substring(0, startb) + varData + s.Substring(endb + 1, s.Length - (endb + 1));
                }

                var v = _variables.Create(s, null, _statementList.Last());

                codeItem = codeItem.Substring(0, firstPos) + v.Name +
                       codeItem.Substring(lastpos, codeItem.Length - (lastpos + 1));
            }


            return codeItem;
        }

        #endregion

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
                if (codeItem[lastItem + 1] != '=' && codeItem[lastItem + 1] != '<' && codeItem[lastItem - 1] != '<')
                {
                    codeItem = codeItem.Substring(0, lastItem) + " < " +
                               codeItem.Substring(lastItem + 1, codeItem.Length - (lastItem + 1));
                    lastItem += 2;
                }
            }

            lastItem = -1;
            while ((lastItem = codeItem.IndexOf("<<", lastItem + 1)) != -1)
            {
                codeItem = codeItem.Substring(0, lastItem) + " << " +
                           codeItem.Substring(lastItem + 2, codeItem.Length - (lastItem + 2));
                lastItem += 2;
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

            int index = 0;

            index = 0;
            while (index < cmdList.Count)
            {
				if (!cmdList.Any(t => t.Equals("-") || t.Equals("+") || t.Equals("*") || t.Equals("/")))
					break;

                var i = index;
                if (cmdList[i].Equals("-") || cmdList[i].Equals("+") ||
                    cmdList[i].Equals("*") || cmdList[i].Equals("/"))
                {
                    var o1 = _variables.GetVariable(cmdList[i - 1]);
                    var o2 = _variables.GetVariable(cmdList[i + 1]);
                    VariableItem rez;

                    switch (cmdList[i])
                    {
                        case "+":
                            rez = o1 + o2;
                            break;
                        case "-":
                            rez = o1 - o2;
                            break;
                        case "*":
                            rez = o1*o2;
                            break;
                        case "/":
                            rez = o1/o2;
                            break;
                        default:
                            throw new Exception("MAGIC!");
                    }
                    cmdList[i - 1] = "";
                    cmdList[i + 1] = "";
                    cmdList[i] = (rez.Name = Guid.NewGuid().ToString().Replace("-", ""));
                    _variables.Add(rez,_statementList.Last());
                    cmdList.RemoveAll(j => j.Length < 1);
                    index = 0;
                }
                else index++;
            }

            cmdList.RemoveAll(i => i.Length < 1);


            index = 0;
            while (index < cmdList.Count)
            {
                if (!cmdList.Any(t => t.Equals(">>") || t.Equals("<<") ))
                    break;

                var i = index;
                if (cmdList[i].Equals(">>") || cmdList[i].Equals("<<"))
                {
                    var o1 = _variables.GetVariable(cmdList[i - 1]);
                    var o2 = _variables.GetVariable(cmdList[i + 1]);
                    VariableItem rez;

                    switch (cmdList[i])
                    {
                        case ">>":
                            //rez = new VariableItem(o1 >> o2);
                            throw new Exception("ERROR :)");
                            break;
                        case "<<":
                            rez = o1 + o2;
                            break;
                        default:
                            throw new Exception("MAGIC!");
                    }
                    cmdList[i - 1] = "";
                    cmdList[i + 1] = "";
                    cmdList[i] = (rez.Name = Guid.NewGuid().ToString().Replace("-", ""));
                    _variables.Add(rez, _statementList.Last());
                    cmdList.RemoveAll(j => j.Length < 1);
                    index = 0;
                }
                else index++;
            }

            cmdList.RemoveAll(i => i.Length < 1);


            index = 0;
            while (index < cmdList.Count)
            {
				if (!cmdList.Any(t => t.Equals(">") || t.Equals("<") || t.Equals(">=") || 
					t.Equals("<=") || t.Equals("==") || t.Equals("!=")))
					break;

                var i = index;
                if (cmdList[i].Equals(">") || cmdList[i].Equals(">=") ||
                    cmdList[i].Equals("<") || cmdList[i].Equals("<=") ||
                    cmdList[i].Equals("==") || cmdList[i].Equals("!="))
                {
                    var o1 = _variables.GetVariable(cmdList[i - 1]);
                    var o2 = _variables.GetVariable(cmdList[i + 1]);
                    VariableItem rez;

                    switch (cmdList[i])
                    {
                        case ">":
                            rez = new VariableItem(o1 > o2);
                            break;
                        case ">=":
                            rez = new VariableItem(o1 >= o2);
                            break;
                        case "<":
                            rez = new VariableItem(o1 < o2);
                            break;
                        case "<=":
                            rez = new VariableItem(o1 <= o2);
                            break;
                        case "==":
                            rez = new VariableItem(o1 == o2);
                            break;
                        case "!=":
                            rez = new VariableItem(o1 != o2);
                            break;
                        default:
                            throw new Exception("MAGIC!");
                    }
                    cmdList[i - 1] = "";
                    cmdList[i + 1] = "";
                    cmdList[i] = (rez.Name = Guid.NewGuid().ToString().Replace("-", ""));
                    _variables.Add(rez,_statementList.Last());
                    cmdList.RemoveAll(j => j.Length < 1);
                    index = 0;
                }
                else index++;
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
                    _variables.Add(rez,_statementList.Last());
                    cmdList.RemoveAll(j => j.Length < 1);
                    y = 0;
                }
                else y++;
            }

            cmdList.RemoveAll(i => i.Length < 1);

			index = 0;
			while (index < cmdList.Count)
			{
				if (!cmdList.Any(l => l.Equals("="))) break;

				var i = index;
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
					_variables.Add(rez, _statementList.Last());
                    cmdList.RemoveAll(j => j.Length < 1);
				}
				else index++;
            }

            cmdList.RemoveAll(i => i.Length < 1);


            if (cmdList.Count == 1)
            {
                var ret = _variables.GetVariable(cmdList[0]);
				_variables.Add(ret, _statementList.Last());
                return ret;
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
