namespace CgaLabs;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();

        this.menuStrip1 = new System.Windows.Forms.MenuStrip();
        this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.menuStrip1.SuspendLayout();
        this.SuspendLayout();
        //
        // menuStrip1
        //
        this.menuStrip1.ImageScalingSize = new System.Drawing.Size(18, 18);
        this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
        this.menuStrip1.Location = new System.Drawing.Point(0, 0);
        this.menuStrip1.Name = "menuStrip1";
        this.menuStrip1.Size = new System.Drawing.Size(1128, 27);
        this.menuStrip1.TabIndex = 0;
        this.menuStrip1.Text = "menuStrip1";
        //
        // fileToolStripMenuItem
        //
        this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem});
        this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        this.fileToolStripMenuItem.Size = new System.Drawing.Size(43, 23);
        this.fileToolStripMenuItem.Text = "File";
        //
        // loadToolStripMenuItem
        //
        this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
        this.loadToolStripMenuItem.Size = new System.Drawing.Size(116, 24);
        this.loadToolStripMenuItem.Text = "Load";
        this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
        //
        // Form1
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.SystemColors.Window;
        this.ClientSize = new System.Drawing.Size(1128, 602);
        this.Controls.Add(this.menuStrip1);
        this.DoubleBuffered = true;
        this.MainMenuStrip = this.menuStrip1;
        this.Name = "Form1";
        this.Text = "Lab12345";
        this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
        this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseWheel);
        this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
        this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
        this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
        this.Resize += new System.EventHandler(this.Form1_Resize);
        this.menuStrip1.ResumeLayout(false);
        this.menuStrip1.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private MenuStrip menuStrip1;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem loadToolStripMenuItem;
}