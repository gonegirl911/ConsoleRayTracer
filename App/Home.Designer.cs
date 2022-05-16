namespace App;

partial class Home
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
            this.btnStart = new System.Windows.Forms.Button();
            this.textBoxWidth = new System.Windows.Forms.TextBox();
            this.textBoxHeight = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnStart.Location = new System.Drawing.Point(0, 206);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(464, 254);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // textBoxWidth
            // 
            this.textBoxWidth.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.textBoxWidth.Location = new System.Drawing.Point(252, 35);
            this.textBoxWidth.Name = "textBoxWidth";
            this.textBoxWidth.Size = new System.Drawing.Size(200, 50);
            this.textBoxWidth.TabIndex = 1;
            this.textBoxWidth.TextChanged += new System.EventHandler(this.textBoxWidth_TextChanged);
            // 
            // textBoxHeight
            // 
            this.textBoxHeight.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.textBoxHeight.Location = new System.Drawing.Point(252, 114);
            this.textBoxHeight.Name = "textBoxHeight";
            this.textBoxHeight.Size = new System.Drawing.Size(200, 50);
            this.textBoxHeight.TabIndex = 2;
            this.textBoxHeight.TextChanged += new System.EventHandler(this.textBoxHeight_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 45);
            this.label1.TabIndex = 3;
            this.label1.Text = "Width (ch)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(603, 162);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 45);
            this.label2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(178, 45);
            this.label3.TabIndex = 5;
            this.label3.Text = "Height (ch)";
            // 
            // Home
            // 
            this.ClientSize = new System.Drawing.Size(464, 460);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxHeight);
            this.Controls.Add(this.textBoxWidth);
            this.Controls.Add(this.btnStart);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "Home";
            this.Text = "Home";
            this.Load += new System.EventHandler(this.Home_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private Button btnStart;
    private TextBox textBoxWidth;
    private TextBox textBoxHeight;
    private Label label1;
    private Label label2;
    private Label label3;
}
