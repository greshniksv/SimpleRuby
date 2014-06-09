using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SrbRuby;

namespace SrbEditor
{
    public partial class frmMain : Form
    {
        readonly ScriptEngine _engine = new ScriptEngine();
        public frmMain()
        {
            InitializeComponent();
        }

        
        private void frmMain_Load(object sender, EventArgs e)
        {
	        _engine.FunctionExecuteCodeEvent += (function, command) => lbLog.Items.Insert(0,function+": "+command);

            srtbSource.ColorizeWordDic.Add("def", Color.Blue);
			srtbSource.ColorizeWordDic.Add("if", Color.Blue);
			srtbSource.ColorizeWordDic.Add("then", Color.Blue);
			srtbSource.ColorizeWordDic.Add("else", Color.Blue);
			srtbSource.ColorizeWordDic.Add("elseif", Color.Blue);
			srtbSource.ColorizeWordDic.Add("end", Color.Blue);
			srtbSource.ColorizeWordDic.Add("var", Color.Blue);
			srtbSource.ColorizeWordDic.Add("true", Color.Blue);
			srtbSource.ColorizeWordDic.Add("false", Color.Blue);
			srtbSource.ColorizeWordDic.Add("or", Color.Blue);
			srtbSource.ColorizeWordDic.Add("and", Color.Blue);


            foreach (var varType in _engine.GetVariableTypeList())
            {
                //srtbSource.Settings.ManageKeywords.Add(varType);
				srtbSource.AddWord(varType, Color.Blue);
            }
            
            foreach (var name in _engine.Command.GetCommandNameList())
            {
                //srtbSource.Settings.Keywords.Add(name);
				srtbSource.AddWord(name, Color.DarkRed);
            }
          
            srtbSource.Settings.KeywordColor = Color.DarkRed;


            // Set the comment identifier. For Lua this is two minus-signs after each other (--). 
            // For C++ we would set this property to "//".
            srtbSource.Settings.Comment = "#";
            
            // Set the colors that will be used.
            srtbSource.Settings.ManageKeywordColor = Color.Blue;
            srtbSource.Settings.CommentColor = Color.Green;
            srtbSource.Settings.StringColor = Color.Gray;
            srtbSource.Settings.IntegerColor = Color.Red;

            // Let's not process strings and integers.
            srtbSource.Settings.EnableStrings = true;
            srtbSource.Settings.EnableIntegers = true;

            // Let's make the settings we just set valid by compiling
            // the keywords to a regular expression.
            //srtbSource.CompileKeywords();

            // Load a file and update the syntax highlighting.
            //srtbSourceScript.LoadFile("../script.lua", RichTextBoxStreamType.PlainText);
            srtbSource.ProcessAllLines();

            using (var tr = new StreamReader("text.rb"))
            {
                srtbSource.Text = tr.ReadToEnd();
            }
            srtbSource.ProcessAllLines();

            foreach (var cmd in _engine.Command.GetCommandNameList())
            {
                lbCommandList.Items.Add(cmd);
            }
        }

        private void srtbSource_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
				lbLog.Items.Clear();

                _engine.LoadFromString(srtbSource.Text);
                _engine.ExecuteFunction();
            }

            if (e.KeyCode == Keys.S && e.Control)
            {
                using (var tw = new StreamWriter("text.rb"))
                {
                    tw.Write(srtbSource.Text);
                }
	            if (Text.Contains("*")) Text = Text.Remove(Text.IndexOf('*'), 1);
            }
            
        }

        private void lbCommandList_SelectedIndexChanged(object sender, EventArgs e)
        {
			if (lbCommandList.SelectedIndex==-1) return;

	        rtbCommandInfo.Text =
		        ((Commands.ICommand) _engine.Command.GetCommandHashTable()[lbCommandList.SelectedItem]).Information();

        }

		private void srtbSource_TextChanged(object sender, EventArgs e)
		{
			if (!Text.Contains("*"))
			{
				Text = Text+@"*";
			}

			

		}

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string[] i = new string[1];
            i.GetType();

        }
    }


}

