using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeakCurrent1.Common;

namespace WeakCurrent1
{
    public partial class DBCreateForm : UIForm
    {
        public DBCreateForm()
        {
            InitializeComponent();
        }

        private void DBCreateForm_Load(object sender, EventArgs e)
        {

        }

        private void UIB_OK_Click(object sender, EventArgs e)
        {
            // 数据库的名称
            string _databaseName = uiTextBox1.Text.Trim()+".db";
            // 数据库的路径
            string _databasePath = @"D:\Mycode\Database\FAS\" + _databaseName ;
            // 构建 SQLite 连接字符串，指定数据库文件路径
            string _connectionString = $"Data Source={_databasePath};Version=3;";

            

            #region 创建数据库文件并插入行


            // 检查数据库文件是否存在，如果不存在则创建数据库和表
            try
            {
                // 检查数据库文件是否存在，不存在则创建
                if (!File.Exists(_databasePath))
                {
                    SQLiteConnection.CreateFile(_databasePath);
                    MessageBox.Show($"数据库 {_databaseName} 创建成功");
                }
                else
                {
                    MessageBox.Show($"数据库 {_databaseName} 已存在");
                }

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    // 创建 fastable 表
                    string createFastableTableQuery = @"
                        CREATE TABLE IF NOT EXISTS fastable (
                        IdKey INTEGER PRIMARY KEY AUTOINCREMENT,
                        Id_Dianjing TEXT NOT NULL,
                        Floor1 TEXT DEFAULT NULL,
                        Floor2 INTEGER DEFAULT NULL,
                        SI INTEGER DEFAULT 0,
                        DianWeiall INTEGER DEFAULT 0,
                        DianWeiliandong INTEGER DEFAULT 0,
                        HuiLu INTEGER DEFAULT 0,
                        YanGan INTEGER DEFAULT 0,
                        ShouBao INTEGER DEFAULT 0,
                        ShengGuang INTEGER DEFAULT 0,
                        JuanLianA INTEGER DEFAULT 0,
                        JuanLianB INTEGER DEFAULT 0,
                        QieFei INTEGER DEFAULT 0,
                        DianTi INTEGER DEFAULT 0,
                        GP INTEGER DEFAULT 0,
                        ZYFJ INTEGER DEFAULT 0,
                        BFJ INTEGER DEFAULT 0,
                        PYFJ INTEGER DEFAULT 0,
                        XHSB INTEGER DEFAULT 0,
                        PLB INTEGER DEFAULT 0,
                        WYB INTEGER DEFAULT 0,
                        BEC INTEGER DEFAULT 0,
                        BED INTEGER DEFAULT 0,
                        BEEH INTEGER DEFAULT 0,
                        BECH INTEGER DEFAULT 0,
                        DYCB INTEGER DEFAULT 0,
                        Fa70 INTEGER DEFAULT 0,
                        Fa280 INTEGER DEFAULT 0,
                        ShuiLiuZSQ INTEGER DEFAULT 0,
                        XinHaofa INTEGER DEFAULT 0,
                        ShiShiBJF INTEGER DEFAULT 0,
                        LiuLiangKG INTEGER DEFAULT 0,
                        YaLiKG INTEGER DEFAULT 0,
                        WenGan INTEGER DEFAULT 0,
                        XiaoHuoshuan INTEGER DEFAULT 0,
                        EXWenGan INTEGER DEFAULT 0,
                        XFDianHua INTEGER DEFAULT 0,
                        LouCengXSQ INTEGER DEFAULT 0,
                        gmt_create TEXT DEFAULT NULL,
                        gmt_change TEXT DEFAULT NULL,
                        B INTEGER DEFAULT 0,
                        PaiYanchuang INTEGER DEFAULT 0,
                        XXGSYanGan INTEGER DEFAULT 0,
                        GuangBo INTEGER DEFAULT 0,
                        RD INTEGER DEFAULT 0,
                        RDK INTEGER DEFAULT 0,
                        UNIQUE (Id_Dianjing),
                        UNIQUE (IdKey)
                    );";

                    // 创建 anfangtable 表
                    string createAnfangTableQuery = @"
                        CREATE TABLE IF NOT EXISTS anfangtable (
                        IdKey INTEGER PRIMARY KEY AUTOINCREMENT,
                        Id_Dianjing TEXT NOT NULL,
                        Floor1 TEXT DEFAULT NULL,
                        Floor2 INTEGER DEFAULT NULL,
                        JianKong_qiuji INTEGER DEFAULT 0,
                        JianKong_qiangji INTEGER DEFAULT 0,
                        JianKong_renlian INTEGER DEFAULT 0,
                        JianKong_shiwai INTEGER DEFAULT 0,
                        JianKong INTEGER DEFAULT 0,
                        XinXifabu INTEGER DEFAULT 0,
                        NengHao INTEGER DEFAULT 0,
                        MenJinkongzhi INTEGER DEFAULT 0,
                        MenJin INTEGER DEFAULT 0,
                        XunGeng INTEGER DEFAULT 0,
                        WuZhangaihujiao INTEGER DEFAULT 0,
                        RuQintance INTEGER DEFAULT 0,
                        gmt_create TEXT DEFAULT NULL,
                        gmt_change TEXT DEFAULT NULL,
                        DDC INTEGER DEFAULT 0,
                        YuLiu1 INTEGER DEFAULT NULL,
                        YuLiu2 INTEGER DEFAULT NULL,
                        YuLiu3 INTEGER DEFAULT NULL,
                        YuLiu4 INTEGER DEFAULT NULL,
                        YuLiu5 INTEGER DEFAULT NULL,
                        YuLiu6 INTEGER DEFAULT NULL,
                        DianWei INTEGER DEFAULT 0,
                        ONU_24 INTEGER DEFAULT 0,
                        ONU_POE24 INTEGER DEFAULT 0,
                        ONU_GC24POE INTEGER DEFAULT 0,
                        FenGuangqi_216 INTEGER DEFAULT 0,
                        AHD INTEGER DEFAULT 0,
                        AP INTEGER DEFAULT 0,
                        UNIQUE (Id_Dianjing),
                        UNIQUE (IdKey)
                    );";

                    // 创建楼层表
                    string createLoucengTable = @"
                        CREATE TABLE IF NOT EXISTS louceng (
                        IdKey INTEGER PRIMARY KEY AUTOINCREMENT,
                        Floor1 TEXT DEFAULT NULL,
                        Floor2 INTEGER DEFAULT NULL
                    );";



                    // 执行创建表的命令
                    using (var fastableCommand = new SQLiteCommand(string.Format(createFastableTableQuery, _databaseName), connection))
                    {
                        fastableCommand.ExecuteNonQuery();
                       // MessageBox.Show("Fastable 表创建成功");
                    }

                    using (var anfangCommand = new SQLiteCommand(createAnfangTableQuery, connection))
                    {
                        anfangCommand.ExecuteNonQuery();
                       // MessageBox.Show("Anfangtable 表创建成功");
                    }

                    using (var loucengCommand = new SQLiteCommand(createLoucengTable, connection))
                    {
                        loucengCommand.ExecuteNonQuery();
                       // MessageBox.Show("Louceng 表创建成功");
                    }

                    // ================= 插入楼层数据 =================
                    string insertLoucengData = @"
                        INSERT INTO louceng (Floor1, Floor2) VALUES
                        ('B2F', -4),
                        ('B1F', -2),
                        ('1F', 2),
                        ('2F', 4),
                        ('3F', 6),
                        ('4F', 8),
                        ('5F', 10),
                        ('6F', 12),
                        ('7F', 14),
                        ('8F', 16),
                        ('9F', 18),
                        ('10F', 20),
                        ('11F', 22),
                        ('12F', 24),
                        ('13F', 26),
                        ('14F', 28),
                        ('15F', 30),
                        ('16F', 32),
                        ('17F', 34),
                        ('18F', 36),
                        ('19F', 38),
                        ('20F', 40),
                        ('21F', 42);";

                    using (var cmd = new SQLiteCommand(insertLoucengData, connection))
                    {
                        int rows = cmd.ExecuteNonQuery();
                       // MessageBox.Show($"成功插入{rows}条楼层数据");
                    }

                    // 修改当前对应dwg的数据库名称
                    // 先设置数据库连接
                    SQLiteConn.ConnDBName = _databaseName;
                    // 如果dwg文件名有效，设置数据库名称
                    SQLiteConn.FasSQLSetAndUpdataDBName(FMain.dwgName, _databaseName);
                    // 修改FMain中的数据库名称
                    FMain.dbName = _databaseName;
                    // 关闭窗口并返回OK结果
                    this.DialogResult = DialogResult.OK;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建数据库或表时发生错误: {ex.Message}");
            }

            #endregion
        }
    }
}
