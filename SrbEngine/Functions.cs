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

        public void Execute()
        {
            VariableItem lastItem;
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
                //codeItem = Simplification(pos);





                // Detect variable
                //if (variableTypeList.Any(i => codeItem.ToLower().Contains(i.ToLower())))
                //    _variables.CreateVariable(codeItem, statementList[statementList.Count-1]);

                // Detect command
                if (commandList.Any(codeItem.Contains))
                    _commands.Execute(codeItem);

                // Remove last statement id
                if (codeItem.Contains("end"))
                {
                    _variables.ClearByStatementId(statementList[statementList.Count - 1]);
                    statementList.Remove(statementList[statementList.Count - 1]);
                }
            }
        }

        private string Simplification(int pos)
        {
            string codeItem = _currentFunc.Code[pos];
            var variableTypeList = _variables.GetVariableTypeList();
            var commandList = _commands.GetCommandNameList();


            // Find function
            foreach (var item in _funcList)
            {
                if (codeItem.Contains(item.Name))
                {
                    var first = codeItem.IndexOf("(", codeItem.IndexOf(item.Name));
                    if (first == -1) throw new Exception("Error not found parameter block functions");

                    var last = codeItem.IndexOf(")", first);
                    if (last == -1) throw new Exception("Error not found parameter block functions");

                    codeItem = codeItem.Substring(0, first) + "|" + codeItem.Substring(first + 1, codeItem.Length - (first + 1));
                    codeItem = codeItem.Substring(0, last) + "|" + codeItem.Substring(last + 1, codeItem.Length - (last + 1));
                }
            }


            // Simplify calculate block

            var cmdList = new List<string>(codeItem.Split(' '));
            cmdList.RemoveAll(i => i.Length < 1);

            /*List<int> buf;
            while ((buf = GetDeepExpList(cmdList)) != null)
            {
                var exp = buf.Select(i => cmdList[i]).ToList();
                exp.RemoveAll(i => i == "(" || i == ")");
                cmdList[buf[0]] = SimplifyExpression(exp) == new VariableItem("true") ? "true" : "false";
                for (int i = 1; i < buf.Count; i++) cmdList[buf[i]] = "";
                cmdList.RemoveAll(i => i.Length < 1);
            }*/



            return "";
        }




        private void CalculateJumpOper(int pos, ref List<string> statementList)
        {

            var codeItem = _currentFunc.Code[pos];
            var separateElements = new List<string>() { "(", ")", "{", "}", ">=", "<=", "==", "!=", "+", "-", "/", "*" };
            codeItem = separateElements.Aggregate(codeItem, (current, i) => current.Replace(i, " " + i + " "));

            statementList.Add(Guid.NewGuid().ToString());
            var cmdList = new List<string>(codeItem.Split(' '));
            cmdList.RemoveAll(i => i.Length < 1);

            if (cmdList[0].Equals("if", StringComparison.OrdinalIgnoreCase))
            {
                cmdList.RemoveAll(i => i == "if");

                List<int> buf;
                while ((buf = GetDeepExpList(cmdList)) != null)
                {
                    var exp = buf.Select(i => cmdList[i]).ToList();
                    exp.RemoveAll(i => i == "(" || i == ")");
                    cmdList[buf[0]] = SimplifyExpression(exp) == new VariableItem("true") ? "true" : "false";
                    for (int i = 1; i < buf.Count; i++) cmdList[buf[i]] = "";
                    cmdList.RemoveAll(i => i.Length < 1);
                }

                if (SimplifyExpression(cmdList) == new VariableItem("true"))
                {
                    // execute true block 
                    var elseBlock = FindBlock(pos, "else");
                    var endBlock = FindBlock(pos, "end");
                    if (elseBlock != null)
                    {
                        _jumpList[elseBlock] = new List<int> { (int)endBlock, (int)endBlock, (int)endBlock };
                    }

                }
                else
                {
                    // find else block and end
                    var elseBlock = FindBlock(pos, "else");
                    var endBlock = FindBlock(pos, "end");
                    if (elseBlock != null)
                        _jumpList.Add(pos, (int)elseBlock);
                    else
                        _jumpList.Add(pos, (int)endBlock);
                }

            }
        }


        #region Work with expression



        private int? FindBlock(int from, string block)
        {
            for (int i = from; i < _currentFunc.Code.Count; i++)
            {
                if (_currentFunc.Code[i] == block)
                {
                    return i;
                }
            }
            return null;
        }

        private List<int> GetDeepExpList(List<string> exp)
        {
            List<int> ret;

            if (exp.Any(i => i.Contains("(")))
            {
                var start = FindLastStr(exp, "(");
                ret = GetBetweenList((int)start, (int)FindFirstStr(exp, ")", (int)start) + 1);
            }
            else
            {
                return null;
            }
            return ret;
        }

        private List<int> GetBetweenList(int start, int end)
        {
            var ret = new List<int>();

            for (int i = start; i < end; i++)
            {
                ret.Add(i);
            }
            return ret;
        }

        private int? FindLastStr(List<string> list, string s)
        {
            int? last = null;
            for (int i = 0; i < list.Count; i++)
                if (list[i] == s)
                    last = i;
            return last;
        }

        private int? FindFirstStr(List<string> list, string s, int start)
        {
            for (int i = start; i < list.Count; i++)
                if (list[i] == s)
                {
                    return i;
                }
            return null;
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
                    cmdList[i] = rez.Name;
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
                    cmdList[i] = rez.Name;
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
                    cmdList[y] = rez.Name;
                    _variables.Add(rez);
                    cmdList.RemoveAll(j => j.Length < 1);
                    y = 0;
                }
                else y++;
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
