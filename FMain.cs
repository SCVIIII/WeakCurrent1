using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeakCurrent1.Common;

namespace WeakCurrent1
{
    public partial class FMain : UIForm
    {
        // 局域变量
        // 当前DWG文件名
        public static string  dwgName; 
        public static string dbName; // 全局变量，用于存储连接的数据库名称


        public FMain()
        {
            InitializeComponent();
            


            if(false)
            {
                string folderPath = @"D:\Mycode\Database\FAS"; // 替换为实际文件夹路径
                var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".db", ".sqlite", ".sqlite3", ".db3" }; // 添加需要查找的SQLite文件扩展名

                try
                {

                    IEnumerable<string> sqliteFiles = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories);
                    List<string> file2 = new List<string>();

                    //MessageBox.Show("找到以下SQLite文件：");
                    foreach (var file in Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories))
                    {
                        string dbname = System.IO.Path.GetFileName(file);
                        file2.Add(dbname);
                    }



                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show("错误：指定的文件夹不存在。");
                }
            }



            Database db = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase;

            // 获取当前CAD文件的完整路径
            string dwgPath = db.Filename;

            // 从完整路径里提取文件名
            dwgName = System.IO.Path.GetFileName(db.Filename);

            // 获取当前DWG文件对应的数据库名称
            dbName = SQLiteConn.FasSQLGetDBName(dwgName);
            

            if (!string.IsNullOrWhiteSpace(dbName))
            {
                // 如果找到了对应的数据库名称，则设置连接字符串
                // 设置全局变量，供其他地方使用
                SQLiteConn.ConnDBName = dbName; 
                // 在UI文本框中显示数据库名称
                uiTextBox1.Text = dbName;
            }
            else
            {
                // 如果没有找到对应的数据库名称，则提示用户
                MessageBox.Show("未找到对应的数据库，请检查配置文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }



            // 把结果显示在命令行中
            //doc.Editor.WriteMessage("\n当前CAD文件: " + fileName);

            if (false)
            {
                //登录信息初始化
                //获取服务器名
                string strServer = OperatorFile.GetIniFileString("MySQL", "Server", "", Path.GetDirectoryName(db.OriginalFileName) + "\\ERP.ini");
                //获取登录用户
                string strUserID = OperatorFile.GetIniFileString("MySQL", "UserID", "", Path.GetDirectoryName(db.OriginalFileName) + "\\ERP.ini");
                //获取登录密码
                string strPwd = OperatorFile.GetIniFileString("MySQL", "Pwd", "", Path.GetDirectoryName(db.OriginalFileName) + "\\ERP.ini");
                //获取端口
                string strPort = OperatorFile.GetIniFileString("MySQL", "Port", "", Path.GetDirectoryName(db.OriginalFileName) + "\\ERP.ini");
                //获取库名
                string strDataBase = OperatorFile.GetIniFileString("MySQL", "DataBase", "", Path.GetDirectoryName(db.OriginalFileName) + "\\ERP.ini");
                //获取表名
                //string strTable = OperatorFile.GetIniFileString("MySQL", "Table", "", Path.GetDirectoryName(db.OriginalFileName) + "\\ERP.ini");
                //连接信息
                string connStr = "server = " + strServer + " ;user = " + strUserID + " ;port=" + strPort + ";password = " + strPwd;
            }
            
            Aside.TabControl = MainTabControl;

            //添加启动页面
            //添加火灾报警页面
            AddPage(new FasPage(), 1001);        //增加页面到Main
            Aside.CreateNode("火灾自动报警系统", 1001); //设置Header节点索引

            //添加安防系统页面
            AddPage(new AnFangPage(), 1002);
            Aside.CreateNode("安防系统", 1002);

            //添加广播及防火门系统页面
            AddPage(new FasRuoDian(), 1003);     //增加页面到Main
            Aside.CreateNode("广播及防火门系统", 1003); //设置Header节点索引

            //添加数据库管理页面
            //AddPage(new DBPage(), 1004);     //增加页面到Main
            //Aside.CreateNode("数据库管理", 1004); //设置Header节点索引


        }

        

        private void Login_Load(object sender, EventArgs e)
        {

        }

        

        private void MainContainer_Click(object sender, EventArgs e)
        {

        }

        private void Aside_MenuItemClick(TreeNode node, NavMenuItem item, int pageIndex)
        {

        }

        private void uiLabel2_Click(object sender, EventArgs e)
        {

        }

        private void uiTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void uiButton1_Click(object sender, EventArgs e)
        {

        }

        private void Login_ReceiveParams(object sender, UIPageParamsArgs e)
        {


        }

        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIComboBox combo = (UIComboBox)sender;
            string selectedValue = combo.SelectedItem.ToString();
            MessageBox.Show($"你选择了: {selectedValue}");
        }


        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            
            // 显示修改数据库连接信息的窗口
            DBSetForm dbSetForm = new DBSetForm(dwgName);

            if (dbSetForm.ShowDialog() == DialogResult.OK)
            {
                // 用户点击了确定按钮，在Textbox中显示新的数据库名称
                uiTextBox1.Text = dbName;
                // 如果用户点击了确定按钮，重新加载页面
                Refresh_Aside();
            }
        }

        private void uiB_Cre_Click(object sender, EventArgs e)
        {
            DBCreateForm dBCreateForm = new DBCreateForm();
            if (dBCreateForm.ShowDialog() == DialogResult.OK)
            {
                // 用户点击了确定按钮，在Textbox中显示新的数据库名称
                uiTextBox1.Text = dbName;
                // 如果用户点击了确定按钮，重新加载页面
                Refresh_Aside();
            }
        }

        private void Refresh_Aside()
        {
            // 当页面未加载时，Aside.TabControl为null
            if (Aside.TabControl != null)
            {
                var num = Aside.TabControl.RowCount; //获取当前页面数量
                                                     // 释放页面资源
                for (int i = 1001; i < num + 1001; i++)
                {
                    var page1 = Aside.TabControl.GetPage(i);
                    page1.Dispose();
                }
            }
            //清空侧面栏
            Aside.ClearAll();

            AddPage(new FasPage(), 1001);        //增加页面到Main
            Aside.CreateNode("火灾自动报警系统", 1001); //设置Header节点索引

            //添加安防系统页面
            AddPage(new AnFangPage(), 1002);
            Aside.CreateNode("安防系统", 1002);

            //添加广播及防火门系统页面
            AddPage(new FasRuoDian(), 1003);     //增加页面到Main
            Aside.CreateNode("广播及防火门系统", 1003); //设置Header节点索引
        }
    }
    
}
