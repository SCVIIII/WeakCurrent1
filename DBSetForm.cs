using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeakCurrent1.Common;

namespace WeakCurrent1
{
    public partial class DBSetForm : UIForm
    {

        // 由主页面传来:当前DWG文件名
        private string _dwgName; 
        public DBSetForm(string dwgName)
        {
            InitializeComponent();
            // dwg文件名传入
            _dwgName = dwgName;
        }


        string folderPath = @"D:\Mycode\Database\FAS\"; // 替换为实际文件夹路径

        /// <summary>
        /// 初始化及主要逻辑处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DBForm_Load(object sender, EventArgs e)
        {
            // 在窗口加载时查找指定文件夹中的SQLite文件
            // 添加需要查找的SQLite文件扩展名
            var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".db", ".sqlite", ".sqlite3", ".db3" }; 

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

                // 设置combox的数据源
                uiComboBox1.DataSource = file2;
                uiComboBox1.SelectedIndex = -1; // 设置默认选中项为无


            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("错误：指定的文件夹不存在。");
            }
        }

        /// <summary>
        /// Cancel键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            // 关闭当前窗口
            this.Close();
        }

        private void uiSymbolButton2_Click(object sender, EventArgs e)
        {
            string selectedDBName = uiComboBox1.SelectedItem?.ToString();

            // 当输入有效时，设置SQLiteConn.ConnDBName并返回OK结果
            if (!string.IsNullOrWhiteSpace(selectedDBName))
            {
                SQLiteConn.ConnDBName = selectedDBName;
                if(!string.IsNullOrWhiteSpace(_dwgName))
                {
                    // 如果dwg文件名有效，设置数据库名称
                    SQLiteConn.FasSQLSetAndUpdataDBName(_dwgName, selectedDBName);
                    // 修改FMain中的数据库名称
                    FMain.dbName=selectedDBName;
                    // 关闭窗口并返回OK结果
                    this.DialogResult = DialogResult.OK; 
                }

                else
                {
                    // 如果没有选择有效的数据库文件，显示错误消息
                    MessageBox.Show("AutoCAD文件名获取失败!");
                    this.DialogResult = DialogResult.Cancel; // 关闭窗口并返回Cancel结果
                }

            }
            else
            {
                // 如果没有选择有效的数据库文件，显示错误消息
                this.DialogResult = DialogResult.Cancel; // 关闭窗口并返回Cancel结果
            }


        }
    }
}
