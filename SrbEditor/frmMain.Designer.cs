namespace SrbEditor
{

    
        partial class frmMain
        {
            /// <summary>
            /// Required designer variable.
            /// </summary>
            private System.ComponentModel.IContainer components = null;

            /// <summary>
            /// Clean up any resources being used.
            /// </summary>
            /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            #region Windows Form Designer generated code

            /// <summary>
            /// Required method for Designer support - do not modify
            /// the contents of this method with the code editor.
            /// </summary>
            private void InitializeComponent()
            {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
                this.menuStrip1 = new System.Windows.Forms.MenuStrip();
                this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
                this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.executeF5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                this.statusStrip1 = new System.Windows.Forms.StatusStrip();
                this.toolStrip1 = new System.Windows.Forms.ToolStrip();
                this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
                this.splitContainer1 = new System.Windows.Forms.SplitContainer();
                this.lbLog = new System.Windows.Forms.ListBox();
                this.lbCommandList = new System.Windows.Forms.ListBox();
                this.label1 = new System.Windows.Forms.Label();
                this.rtbCommandInfo = new System.Windows.Forms.RichTextBox();
                this.splitContainer2 = new System.Windows.Forms.SplitContainer();
                this.splitContainer3 = new System.Windows.Forms.SplitContainer();
                this.srtbSource = new SyntaxRichTextBox();
                this.menuStrip1.SuspendLayout();
                this.toolStrip1.SuspendLayout();
                ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
                this.splitContainer1.Panel1.SuspendLayout();
                this.splitContainer1.Panel2.SuspendLayout();
                this.splitContainer1.SuspendLayout();
                ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
                this.splitContainer2.Panel1.SuspendLayout();
                this.splitContainer2.Panel2.SuspendLayout();
                this.splitContainer2.SuspendLayout();
                ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
                this.splitContainer3.Panel1.SuspendLayout();
                this.splitContainer3.Panel2.SuspendLayout();
                this.splitContainer3.SuspendLayout();
                this.SuspendLayout();
                // 
                // menuStrip1
                // 
                this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.runToolStripMenuItem});
                this.menuStrip1.Location = new System.Drawing.Point(0, 0);
                this.menuStrip1.Name = "menuStrip1";
                this.menuStrip1.Size = new System.Drawing.Size(885, 24);
                this.menuStrip1.TabIndex = 3;
                this.menuStrip1.Text = "menuStrip1";
                // 
                // fileToolStripMenuItem
                // 
                this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
                this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
                this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
                this.fileToolStripMenuItem.Text = "File";
                // 
                // openToolStripMenuItem
                // 
                this.openToolStripMenuItem.Name = "openToolStripMenuItem";
                this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
                this.openToolStripMenuItem.Text = "Open";
                // 
                // saveToolStripMenuItem
                // 
                this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
                this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
                this.saveToolStripMenuItem.Text = "Save";
                // 
                // toolStripMenuItem1
                // 
                this.toolStripMenuItem1.Name = "toolStripMenuItem1";
                this.toolStripMenuItem1.Size = new System.Drawing.Size(100, 6);
                // 
                // exitToolStripMenuItem
                // 
                this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
                this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
                this.exitToolStripMenuItem.Text = "Exit";
                // 
                // runToolStripMenuItem
                // 
                this.runToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.executeF5ToolStripMenuItem});
                this.runToolStripMenuItem.Name = "runToolStripMenuItem";
                this.runToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
                this.runToolStripMenuItem.Text = "Run";
                // 
                // executeF5ToolStripMenuItem
                // 
                this.executeF5ToolStripMenuItem.Name = "executeF5ToolStripMenuItem";
                this.executeF5ToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
                this.executeF5ToolStripMenuItem.Text = "Execute ( F5 )";
                // 
                // statusStrip1
                // 
                this.statusStrip1.Location = new System.Drawing.Point(0, 437);
                this.statusStrip1.Name = "statusStrip1";
                this.statusStrip1.Size = new System.Drawing.Size(885, 22);
                this.statusStrip1.TabIndex = 4;
                this.statusStrip1.Text = "statusStrip1";
                // 
                // toolStrip1
                // 
                this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
                this.toolStrip1.Location = new System.Drawing.Point(0, 24);
                this.toolStrip1.Name = "toolStrip1";
                this.toolStrip1.Size = new System.Drawing.Size(885, 25);
                this.toolStrip1.TabIndex = 5;
                this.toolStrip1.Text = "toolStrip1";
                // 
                // toolStripButton1
                // 
                this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
                this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
                this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
                this.toolStripButton1.Name = "toolStripButton1";
                this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
                this.toolStripButton1.Text = "toolStripButton1";
                this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
                // 
                // splitContainer1
                // 
                this.splitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
                this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
                this.splitContainer1.Location = new System.Drawing.Point(0, 0);
                this.splitContainer1.Name = "splitContainer1";
                this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
                // 
                // splitContainer1.Panel1
                // 
                this.splitContainer1.Panel1.Controls.Add(this.srtbSource);
                // 
                // splitContainer1.Panel2
                // 
                this.splitContainer1.Panel2.Controls.Add(this.lbLog);
                this.splitContainer1.Size = new System.Drawing.Size(663, 382);
                this.splitContainer1.SplitterDistance = 285;
                this.splitContainer1.TabIndex = 6;
                // 
                // lbLog
                // 
                this.lbLog.Dock = System.Windows.Forms.DockStyle.Fill;
                this.lbLog.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                this.lbLog.FormattingEnabled = true;
                this.lbLog.ItemHeight = 16;
                this.lbLog.Location = new System.Drawing.Point(0, 0);
                this.lbLog.Name = "lbLog";
                this.lbLog.Size = new System.Drawing.Size(663, 93);
                this.lbLog.TabIndex = 0;
                // 
                // lbCommandList
                // 
                this.lbCommandList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
                this.lbCommandList.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                this.lbCommandList.FormattingEnabled = true;
                this.lbCommandList.ItemHeight = 16;
                this.lbCommandList.Location = new System.Drawing.Point(3, 21);
                this.lbCommandList.Name = "lbCommandList";
                this.lbCommandList.Size = new System.Drawing.Size(188, 148);
                this.lbCommandList.TabIndex = 7;
                this.lbCommandList.SelectedIndexChanged += new System.EventHandler(this.lbCommandList_SelectedIndexChanged);
                // 
                // label1
                // 
                this.label1.AutoSize = true;
                this.label1.Location = new System.Drawing.Point(3, 5);
                this.label1.Name = "label1";
                this.label1.Size = new System.Drawing.Size(62, 13);
                this.label1.TabIndex = 8;
                this.label1.Text = "Commands:";
                // 
                // rtbCommandInfo
                // 
                this.rtbCommandInfo.Dock = System.Windows.Forms.DockStyle.Fill;
                this.rtbCommandInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                this.rtbCommandInfo.Location = new System.Drawing.Point(0, 0);
                this.rtbCommandInfo.Name = "rtbCommandInfo";
                this.rtbCommandInfo.Size = new System.Drawing.Size(194, 184);
                this.rtbCommandInfo.TabIndex = 10;
                this.rtbCommandInfo.Text = "";
                // 
                // splitContainer2
                // 
                this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
                this.splitContainer2.Location = new System.Drawing.Point(0, 0);
                this.splitContainer2.Name = "splitContainer2";
                this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
                // 
                // splitContainer2.Panel1
                // 
                this.splitContainer2.Panel1.Controls.Add(this.lbCommandList);
                this.splitContainer2.Panel1.Controls.Add(this.label1);
                // 
                // splitContainer2.Panel2
                // 
                this.splitContainer2.Panel2.Controls.Add(this.rtbCommandInfo);
                this.splitContainer2.Size = new System.Drawing.Size(194, 382);
                this.splitContainer2.SplitterDistance = 194;
                this.splitContainer2.TabIndex = 11;
                // 
                // splitContainer3
                // 
                this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
                this.splitContainer3.Location = new System.Drawing.Point(12, 52);
                this.splitContainer3.Name = "splitContainer3";
                // 
                // splitContainer3.Panel1
                // 
                this.splitContainer3.Panel1.Controls.Add(this.splitContainer1);
                // 
                // splitContainer3.Panel2
                // 
                this.splitContainer3.Panel2.Controls.Add(this.splitContainer2);
                this.splitContainer3.Size = new System.Drawing.Size(861, 382);
                this.splitContainer3.SplitterDistance = 663;
                this.splitContainer3.TabIndex = 12;
                // 
                // srtbSource
                // 
                this.srtbSource.AcceptsTab = true;
                this.srtbSource.Dock = System.Windows.Forms.DockStyle.Fill;
                this.srtbSource.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                this.srtbSource.Location = new System.Drawing.Point(0, 0);
                this.srtbSource.Name = "srtbSource";
                this.srtbSource.Size = new System.Drawing.Size(663, 285);
                this.srtbSource.TabIndex = 2;
                this.srtbSource.Text = "";
                this.srtbSource.TextChanged += new System.EventHandler(this.srtbSource_TextChanged);
                this.srtbSource.KeyUp += new System.Windows.Forms.KeyEventHandler(this.srtbSource_KeyUp);
                // 
                // frmMain
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(885, 459);
                this.Controls.Add(this.splitContainer3);
                this.Controls.Add(this.toolStrip1);
                this.Controls.Add(this.statusStrip1);
                this.Controls.Add(this.menuStrip1);
                this.Name = "frmMain";
                this.Text = "Form1";
                this.Load += new System.EventHandler(this.frmMain_Load);
                this.menuStrip1.ResumeLayout(false);
                this.menuStrip1.PerformLayout();
                this.toolStrip1.ResumeLayout(false);
                this.toolStrip1.PerformLayout();
                this.splitContainer1.Panel1.ResumeLayout(false);
                this.splitContainer1.Panel2.ResumeLayout(false);
                ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
                this.splitContainer1.ResumeLayout(false);
                this.splitContainer2.Panel1.ResumeLayout(false);
                this.splitContainer2.Panel1.PerformLayout();
                this.splitContainer2.Panel2.ResumeLayout(false);
                ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
                this.splitContainer2.ResumeLayout(false);
                this.splitContainer3.Panel1.ResumeLayout(false);
                this.splitContainer3.Panel2.ResumeLayout(false);
                ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
                this.splitContainer3.ResumeLayout(false);
                this.ResumeLayout(false);
                this.PerformLayout();

            }

            #endregion

            private SyntaxRichTextBox srtbSource;
            private System.Windows.Forms.MenuStrip menuStrip1;
            private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
            private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
            private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
            private System.Windows.Forms.StatusStrip statusStrip1;
            private System.Windows.Forms.ToolStrip toolStrip1;
            private System.Windows.Forms.ToolStripButton toolStripButton1;
            private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
            private System.Windows.Forms.ToolStripMenuItem executeF5ToolStripMenuItem;
            private System.Windows.Forms.SplitContainer splitContainer1;
            private System.Windows.Forms.ListBox lbLog;
            private System.Windows.Forms.ListBox lbCommandList;
            private System.Windows.Forms.Label label1;
            private System.Windows.Forms.RichTextBox rtbCommandInfo;
            private System.Windows.Forms.SplitContainer splitContainer2;
            private System.Windows.Forms.SplitContainer splitContainer3;
        }
    


}

