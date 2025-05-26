
namespace WeakCurrent1
{
    partial class FMain
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
            this.uiTabControl1 = new Sunny.UI.UITabControl();
            this.MainContainer = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.Aside = new Sunny.UI.UINavMenu();
            this.uiTabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTabControl1
            // 
            this.uiTabControl1.Controls.Add(this.MainContainer);
            this.uiTabControl1.Controls.Add(this.tabPage2);
            this.uiTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControl1.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTabControl1.Frame = null;
            this.uiTabControl1.ItemSize = new System.Drawing.Size(0, 1);
            this.uiTabControl1.Location = new System.Drawing.Point(209, 38);
            this.uiTabControl1.MainPage = "";
            this.uiTabControl1.Name = "uiTabControl1";
            this.uiTabControl1.SelectedIndex = 0;
            this.uiTabControl1.Size = new System.Drawing.Size(812, 650);
            this.uiTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControl1.TabIndex = 6;
            this.uiTabControl1.TabVisible = false;
            this.uiTabControl1.TipsFont = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTabControl1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // MainContainer
            // 
            this.MainContainer.Location = new System.Drawing.Point(0, 0);
            this.MainContainer.Name = "MainContainer";
            this.MainContainer.Size = new System.Drawing.Size(812, 650);
            this.MainContainer.TabIndex = 0;
            this.MainContainer.Text = "tabPage1";
            this.MainContainer.UseVisualStyleBackColor = true;
            this.MainContainer.Click += new System.EventHandler(this.MainContainer_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(0, 30);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(548, 620);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // Aside
            // 
            this.Aside.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Aside.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.Aside.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Aside.FullRowSelect = true;
            this.Aside.ItemHeight = 50;
            this.Aside.Location = new System.Drawing.Point(3, 38);
            this.Aside.Name = "Aside";
            this.Aside.ShowLines = false;
            this.Aside.Size = new System.Drawing.Size(200, 650);
            this.Aside.TabIndex = 7;
            this.Aside.TipsFont = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Aside.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.Aside.MenuItemClick += new Sunny.UI.UINavMenu.OnMenuItemClick(this.Aside_MenuItemClick);
            // 
            // Login
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1024, 700);
            this.Controls.Add(this.Aside);
            this.Controls.Add(this.uiTabControl1);
            this.Name = "Login";
            this.Text = "Login";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.ReceiveParams += new Sunny.UI.OnReceiveParams(this.Login_ReceiveParams);
            this.Load += new System.EventHandler(this.Login_Load);
            this.uiTabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Sunny.UI.UITabControl uiTabControl1;
        private System.Windows.Forms.TabPage MainContainer;
        private System.Windows.Forms.TabPage tabPage2;
        private Sunny.UI.UINavMenu Aside;
    }
}