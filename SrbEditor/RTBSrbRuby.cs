using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Drawing;

namespace SrbEditor
{
    public class SyntaxRichTextBox : System.Windows.Forms.RichTextBox
    {
        private SyntaxSettings m_settings = new SyntaxSettings();
        private static bool m_bPaint = true;
        private string m_strLine = "";
        private int m_nContentLength = 0;
        private int m_nLineLength = 0;
        private int m_nLineStart = 0;
        private int m_nLineEnd = 0;
        private string m_strKeywords = "";
        private string m_strManageKeywords = "";
        private int m_nCurSelection = 0;

        private Dictionary<string, Color> _colorizeWordDic = new Dictionary<string, Color>();

        public Dictionary<string, Color> ColorizeWordDic
        {
            get { return _colorizeWordDic; }
            set { _colorizeWordDic = value; }
        }

        public void InitColorizeWord(string s, Color c)
        {
            foreach (var s1 in s.Split(',')) _colorizeWordDic.Add(s1.ToLower(), c);
        }

        public void AddWord(string s, Color c)
        {
            _colorizeWordDic.Add(s.ToLower(), c);
        }

        /// <summary>
        /// The settings.
        /// </summary>
        public SyntaxSettings Settings
        {
            get { return m_settings; }
        }

        /// <summary>
        /// WndProc
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == 0x00f)
            {
                if (m_bPaint)
                    base.WndProc(ref m);
                else
                    m.Result = IntPtr.Zero;
            }
            else
                base.WndProc(ref m);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                SelectionLength = 0;
                SelectedText = new string(' ', 4);
                return true;
            }

            if (keyData == Keys.Enter)
            {
                var newTabWords = new List<string>() { "if", "elseif", "while", "for", "else", "def" };
                int endline = 0;
                var s = SelectionStart - 1;
                while (s-- > 0) if (Text[s] == '\n') break;
                endline = s + 2;
                while (endline++ < TextLength) if (Text[endline - 1] == '\n') break;

                var line = Text.Substring(s, (endline-1) - s);
                bool newTab = newTabWords.Any(line.Contains);

                int spaceCount = 0;
                while (s++ < TextLength-1) if (Text[s] != ' ') break; else spaceCount++;

                SelectionLength = 0;
                SelectedText = "\n" + new string(' ', spaceCount + (newTab ? 4 : 0));
                return true;
            }

            return base.ProcessCmdKey(ref m, keyData);
        }

        /// <summary>
        /// OnTextChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e)
        {
            // Calculate shit here.
            m_nContentLength = this.TextLength;

            int nCurrentSelectionStart = SelectionStart;
            int nCurrentSelectionLength = SelectionLength;

            m_bPaint = false;

            // Find the start of the current line.
            m_nLineStart = nCurrentSelectionStart;
            while ((m_nLineStart > 0) && (Text[m_nLineStart - 1] != '\n'))
                m_nLineStart--;
            // Find the end of the current line.
            m_nLineEnd = nCurrentSelectionStart;
            while ((m_nLineEnd < Text.Length) && (Text[m_nLineEnd] != '\n'))
                m_nLineEnd++;
            // Calculate the length of the line.
            m_nLineLength = m_nLineEnd - m_nLineStart;
            // Get the current line.
            m_strLine = Text.Substring(m_nLineStart, m_nLineLength);



            // Process this line.
            ProcessLine();

            m_bPaint = true;

            base.OnTextChanged(e);
        }
        /// <summary>
        /// Process a line.
        /// </summary>
        private void ProcessLine()
        {
            // Save the position and make the whole line black
            int nPosition = SelectionStart;
            SelectionStart = m_nLineStart;
            SelectionLength = m_nLineLength;
            SelectionColor = Color.Black;

            // Process the keywords
            //ProcessRegex(m_strKeywords, Settings.KeywordColor);
            // Process the manage keywords
            //ProcessRegex(m_strManageKeywords, Settings.ManageKeywordColor);
            ProgressWord(m_strLine);

            // Process numbers
            if (Settings.EnableIntegers)
                ProcessRegex("\\b(?:[0-9]*\\.)?[0-9]+\\b", Settings.IntegerColor);
            // Process strings
            if (Settings.EnableStrings)
            {
                ProcessRegex("\"[^\"\\\\\\r\\n]*(?:\\\\.[^\"\\\\\\r\\n]*)*\"", Settings.StringColor);
                ProcessRegex("\'[^\'\\\\\\r\\n]*(?:\\\\.[^\"\\\\\\r\\n]*)*\'", Settings.StringColor);
            }

            // Process comments
            if (Settings.EnableComments && !string.IsNullOrEmpty(Settings.Comment))
                ProcessRegex(Settings.Comment + ".*$", Settings.CommentColor);

            SelectionStart = nPosition;
            SelectionLength = 0;
            SelectionColor = Color.Black;

            m_nCurSelection = nPosition;
        }
        /// <summary>
        /// Process a regular expression.
        /// </summary>
        /// <param name="strRegex">The regular expression.</param>
        /// <param name="color">The color.</param>
        private void ProcessRegex(string strRegex, Color color)
        {
            Regex regKeywords = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Match regMatch;

            for (regMatch = regKeywords.Match(m_strLine); regMatch.Success; regMatch = regMatch.NextMatch())
            {
                // Process the words
                int nStart = m_nLineStart + regMatch.Index;
                int nLenght = regMatch.Length;
                SelectionStart = nStart;
                SelectionLength = nLenght;
                SelectionColor = color;
            }
        }

        private void ProgressWord(string line)
        {
            var lineMass = line.Split(' ');

            foreach (var word in lineMass)
            {
                if (_colorizeWordDic.ContainsKey(word.ToLower()))
                {
                    foreach (var pos in AllIndexesOf(line, word))
                    {
                        SelectionStart = m_nLineStart + pos;
                        SelectionLength = word.Length;
                        SelectionColor = _colorizeWordDic[word.ToLower()];
                    }
                }
            }
        }

        public static List<int> AllIndexesOf(string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            var indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        /// <summary>
        /// Compiles the keywords as a regular expression.
        /// </summary>
        /*public void CompileKeywords()
        {
            for (int i = 0; i < Settings.Keywords.Count; i++)
            {
                string strKeyword = Settings.Keywords[i];

                if (i == Settings.Keywords.Count-1)
                    m_strKeywords += "\\b" + strKeyword + "\\b";
                else
                    m_strKeywords += "\\b" + strKeyword + "\\b|";
            }

            for (int i = 0; i < Settings.ManageKeywords.Count; i++)
            {
                string strKeyword = Settings.ManageKeywords[i];

                if (i == Settings.ManageKeywords.Count - 1)
                    m_strManageKeywords += "\\b" + strKeyword + "\\b";
                else
                    m_strManageKeywords += "\\b" + strKeyword + "\\b|";
            }
        }*/

        public void ProcessAllLines()
        {
            m_bPaint = false;

            int nStartPos = 0;
            int i = 0;
            int nOriginalPos = SelectionStart;
            while (i < Lines.Length)
            {
                m_strLine = Lines[i];
                m_nLineStart = nStartPos;
                m_nLineEnd = m_nLineStart + m_strLine.Length;

                ProcessLine();
                i++;

                nStartPos += m_strLine.Length + 1;
            }

            m_bPaint = true;
        }
    }

    /// <summary>
    /// Class to store syntax objects in.
    /// </summary>
    public class SyntaxList
    {
        public List<string> m_rgList = new List<string>();
        public Color m_color = new Color();
    }

    /// <summary>
    /// Settings for the keywords and colors.
    /// </summary>
    public class SyntaxSettings
    {
        SyntaxList m_rgKeywords = new SyntaxList();
        SyntaxList m_rgManageKeywords = new SyntaxList();
        string m_strComment = "";
        Color m_colorComment = Color.Green;
        Color m_colorString = Color.Gray;
        Color m_colorInteger = Color.Red;
        bool m_bEnableComments = true;
        bool m_bEnableIntegers = true;
        bool m_bEnableStrings = true;

        #region Properties


        /// <summary>
        /// A list containing all keywords.
        /// </summary>
        public List<string> ManageKeywords
        {
            get { return m_rgManageKeywords.m_rgList; }
        }
        /// <summary>
        /// The color of keywords.
        /// </summary>
        public Color ManageKeywordColor
        {
            get { return m_rgManageKeywords.m_color; }
            set { m_rgManageKeywords.m_color = value; }
        }

        /// <summary>
        /// A list containing all keywords.
        /// </summary>
        public List<string> Keywords
        {
            get { return m_rgKeywords.m_rgList; }
        }
        /// <summary>
        /// The color of keywords.
        /// </summary>
        public Color KeywordColor
        {
            get { return m_rgKeywords.m_color; }
            set { m_rgKeywords.m_color = value; }
        }
        /// <summary>
        /// A string containing the comment identifier.
        /// </summary>
        public string Comment
        {
            get { return m_strComment; }
            set { m_strComment = value; }
        }
        /// <summary>
        /// The color of comments.
        /// </summary>
        public Color CommentColor
        {
            get { return m_colorComment; }
            set { m_colorComment = value; }
        }
        /// <summary>
        /// Enables processing of comments if set to true.
        /// </summary>
        public bool EnableComments
        {
            get { return m_bEnableComments; }
            set { m_bEnableComments = value; }
        }
        /// <summary>
        /// Enables processing of integers if set to true.
        /// </summary>
        public bool EnableIntegers
        {
            get { return m_bEnableIntegers; }
            set { m_bEnableIntegers = value; }
        }
        /// <summary>
        /// Enables processing of strings if set to true.
        /// </summary>
        public bool EnableStrings
        {
            get { return m_bEnableStrings; }
            set { m_bEnableStrings = value; }
        }
        /// <summary>
        /// The color of strings.
        /// </summary>
        public Color StringColor
        {
            get { return m_colorString; }
            set { m_colorString = value; }
        }
        /// <summary>
        /// The color of integers.
        /// </summary>
        public Color IntegerColor
        {
            get { return m_colorInteger; }
            set { m_colorInteger = value; }
        }
        #endregion
    }
}
