namespace JsonToClass
{
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_sourceType = new System.Windows.Forms.ComboBox();
            this.comboBox_lang = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBox_origin = new System.Windows.Forms.TextBox();
            this.textBox_result = new System.Windows.Forms.TextBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.comboBox_sourceType);
            this.panel1.Controls.Add(this.comboBox_lang);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 413);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(641, 28);
            this.panel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(376, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "→";
            // 
            // comboBox_sourceType
            // 
            this.comboBox_sourceType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_sourceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_sourceType.FormattingEnabled = true;
            this.comboBox_sourceType.Location = new System.Drawing.Point(222, 1);
            this.comboBox_sourceType.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_sourceType.Name = "comboBox_sourceType";
            this.comboBox_sourceType.Size = new System.Drawing.Size(139, 25);
            this.comboBox_sourceType.TabIndex = 2;
            // 
            // comboBox_lang
            // 
            this.comboBox_lang.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_lang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_lang.FormattingEnabled = true;
            this.comboBox_lang.Location = new System.Drawing.Point(417, 1);
            this.comboBox_lang.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_lang.Name = "comboBox_lang";
            this.comboBox_lang.Size = new System.Drawing.Size(139, 25);
            this.comboBox_lang.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(560, 1);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 25);
            this.button1.TabIndex = 0;
            this.button1.Text = "转换";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBox_origin);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBox_result);
            this.splitContainer1.Size = new System.Drawing.Size(641, 413);
            this.splitContainer1.SplitterDistance = 263;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 3;
            // 
            // textBox_origin
            // 
            this.textBox_origin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_origin.Location = new System.Drawing.Point(0, 0);
            this.textBox_origin.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_origin.Multiline = true;
            this.textBox_origin.Name = "textBox_origin";
            this.textBox_origin.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_origin.Size = new System.Drawing.Size(263, 413);
            this.textBox_origin.TabIndex = 0;
            // 
            // textBox_result
            // 
            this.textBox_result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_result.Location = new System.Drawing.Point(0, 0);
            this.textBox_result.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_result.Multiline = true;
            this.textBox_result.Name = "textBox_result";
            this.textBox_result.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_result.Size = new System.Drawing.Size(376, 413);
            this.textBox_result.TabIndex = 1;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Right;
            this.propertyGrid1.Location = new System.Drawing.Point(643, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(204, 441);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.Visible = false;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(641, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(2, 441);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(847, 441);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.propertyGrid1);
            this.Name = "Form1";
            this.Text = "JsonToClass";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private ComboBox comboBox_lang;
        private Button button1;
        private SplitContainer splitContainer1;
        private TextBox textBox_origin;
        private TextBox textBox_result;
        private Label label1;
        private ComboBox comboBox_sourceType;
        private PropertyGrid propertyGrid1;
        private Splitter splitter1;
    }
}