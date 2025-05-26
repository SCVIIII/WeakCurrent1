using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

using System.Data.SqlClient;
using Sunny.UI;
using WeakCurrent1.Common;


using MySql.Data;
using MySql.Data.MySqlClient;

using Autodesk.AutoCAD.DatabaseServices;

namespace WeakCurrent1
{
    public partial class FMain : UIForm
    {
        MySqlConnection conn;

        public FMain()
        {
            InitializeComponent();
            Database db = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase;

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
            //CodeInfos.ini
            //InitializeFromSQL();
            Aside.TabControl = MainTabControl;


            //添加启动页面
            //AddPage(new FStart(), 1001);     //增加启动页面
            AddPage(new FasPage(connStr, strDataBase), 1002);        //增加页面到Main
            AddPage(new AnFangPage(connStr, strDataBase), 1003);     //增加页面到Main
            AddPage(new FasRuoDian(connStr, strDataBase), 1004);     //增加页面到Main
            //Aside.CreateNode("首页", 1001); //设置Header节点索引
            Aside.CreateNode("火灾自动报警系统", 1002); //设置Header节点索引
            Aside.CreateNode("安防系统", 1003); //设置Header节点索引
            Aside.CreateNode("广播及防火门系统", 1004); //设置Header节点索引

        }
        

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void InitializeFromSQL()
        {

        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            try
            {
                //MySQL登录信息
                string connStr = "server=localhost;user=root;database=testdb1;password=bxwh2010";
                // Connect to SQL
                conn = new MySqlConnection(connStr); 
                conn.Open();

                //Parameters参数查询
                int index1 = 2000;
                int index2 = 2;

                //Mysql查询
                //string sql1 = "SELECT IdKey, Id_Dianjing ,Floor1 FROM 海洋楼.mytest1 WHERE IdKey< @index1 AND Floor1 <@index2";
                string sql1 = "SELECT DISTINCT Floor1 FROM 海洋楼.mytest1 ";
                MySqlCommand cmd = new MySqlCommand(sql1, conn);
                cmd.Parameters.AddWithValue("@index1", index1);
                cmd.Parameters.AddWithValue("@index2", index2);

                MySqlDataAdapter daAdpter = new MySqlDataAdapter(cmd);
                
                DataSet dts = new DataSet();  
                daAdpter.Fill(dts, "Table1");


                ////DataGridView数据填充
                //uiDataGridView1.DataSource = dts;  
                //uiDataGridView1.DataMember = "Table1";

               

                

                ////根据楼层数,添加页面
                //var num_Louceng = dsCountry.Tables["Table1"].Rows.Count;
                
                //if (num_Louceng>0)
                //{
                //    int pageIndex = 2000;
                //    TreeNode parent = Aside.CreateNode("楼层信息", 61451, 24, pageIndex);
                //    //通过设置PageIndex关联，节点文字、图标由相应的Page的Text、Symbol提供
                //    for(int i =0;i<num_Louceng;i++)
                //    {
                //        string floor1 = dsCountry.Tables["Table1"].Rows[i][0].ToString();
                //        //Aside.CreateChildNode(parent, AddPage(new FPageLouceng(conn, floor1), ++pageIndex));
                //        Aside.CreateChildNode(parent, AddPage(new FPageLouceng(floor1), ++pageIndex));

                //    }
                //}
                
                

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //uiTextBox2.Text = ex.ToString();
            }
            conn.Close();

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


            //if (e.SourcePage == null)
            //{
            //    //来自页面框架的传值
            //    uiTextBox2.Text = e.Value.ToString();
            //    e.Handled = true;
            //}
        }
    }
    
}
