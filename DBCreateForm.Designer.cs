namespace WeakCurrent1
{
    partial class DBCreateForm
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
            this.UIB_OK = new Sunny.UI.UISymbolButton();
            this.UIB_CANCEL = new Sunny.UI.UISymbolButton();
            this.uiTextBox1 = new Sunny.UI.UITextBox();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.SuspendLayout();
            // 
            // UIB_OK
            // 
            this.UIB_OK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.UIB_OK.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UIB_OK.Location = new System.Drawing.Point(44, 254);
            this.UIB_OK.MinimumSize = new System.Drawing.Size(1, 1);
            this.UIB_OK.Name = "UIB_OK";
            this.UIB_OK.Size = new System.Drawing.Size(100, 35);
            this.UIB_OK.TabIndex = 5;
            this.UIB_OK.Text = "确定";
            this.UIB_OK.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UIB_OK.Click += new System.EventHandler(this.UIB_OK_Click);
            // 
            // UIB_CANCEL
            // 
            this.UIB_CANCEL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.UIB_CANCEL.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UIB_CANCEL.Location = new System.Drawing.Point(214, 254);
            this.UIB_CANCEL.MinimumSize = new System.Drawing.Size(1, 1);
            this.UIB_CANCEL.Name = "UIB_CANCEL";
            this.UIB_CANCEL.Size = new System.Drawing.Size(100, 35);
            this.UIB_CANCEL.Symbol = 61453;
            this.UIB_CANCEL.TabIndex = 4;
            this.UIB_CANCEL.Text = "取消";
            this.UIB_CANCEL.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // uiTextBox1
            // 
            this.uiTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBox1.Location = new System.Drawing.Point(44, 140);
            this.uiTextBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBox1.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBox1.Name = "uiTextBox1";
            this.uiTextBox1.Padding = new System.Windows.Forms.Padding(5);
            this.uiTextBox1.ShowText = false;
            this.uiTextBox1.Size = new System.Drawing.Size(270, 50);
            this.uiTextBox1.TabIndex = 6;
            this.uiTextBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiTextBox1.Watermark = "";
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.Location = new System.Drawing.Point(44, 97);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(160, 30);
            this.uiLabel1.TabIndex = 7;
            this.uiLabel1.Text = "输入数据库名称：";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DBCreateForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(350, 350);
            this.Controls.Add(this.uiLabel1);
            this.Controls.Add(this.uiTextBox1);
            this.Controls.Add(this.UIB_OK);
            this.Controls.Add(this.UIB_CANCEL);
            this.Name = "DBCreateForm";
            this.Text = "新建数据库";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.Load += new System.EventHandler(this.DBCreateForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UISymbolButton UIB_OK;
        private Sunny.UI.UISymbolButton UIB_CANCEL;
        private Sunny.UI.UITextBox uiTextBox1;
        private Sunny.UI.UILabel uiLabel1;
    }
}