using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using SrbRuby;

namespace SrbExecutor
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileToExecute = "test.rb";

            if (!File.Exists(fileToExecute))
            {
                Console.WriteLine("Scripot not found:"+fileToExecute);
                return;
            }

            Console.WriteLine("Execute script: " + fileToExecute); 
            var engine = new ScriptEngine();
            engine.FunctionExecuteCodeEvent += (function, command) =>
            {
                Console.WriteLine(function + ": " + command);
                Console.Out.Flush();
            };
            engine.LoadFromFile(fileToExecute);
            engine.ExecuteFunction();

            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
