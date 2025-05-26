using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using Sunny.UI;

using MySql.Data.MySqlClient;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;


using Color = Autodesk.AutoCAD.Colors.Color;

namespace WeakCurrent1
{
    public partial class FasRuoDian : UIPage
    {

        Database db;
        Editor ed;
        Transaction trans;
        MySqlConnection conn;
        string tablename;


        public FasRuoDian(string connStr, string strTable)
        {
            InitializeComponent();

            //SQL初始化
            tablename = strTable;
            conn = new MySqlConnection(connStr);

            InitializeImportDWG();   //导入图块

        }

        /// <summary>
        /// 初始化db,ed 并从默认位置导入火灾自动报警系统图块
        /// </summary>
        public void InitializeImportDWG()
        {
            //CAD所需变量初始化
            db = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase;
            ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            //首先尝试从默认位置导入外部图块,导入成功后再导入计算书
            string fileDwgBlk = @"D:\Mycode\BlkDYPD\BlkFAS.dwg";
            if (FasPage.ImportBlocksFromDwg_RD(db, fileDwgBlk))
            {
                uiLabel1.Text="图块导入成功";
            }

            else
            {

            }
        } // end of InitializeImportDWG()

        private void GenericCablingPage_Load(object sender, EventArgs e)
        {
            //打开数据库连接
            conn.Open();
            //Parameters参数查询
            //230409增加SI
            string fastablename = tablename + ".fastable";

            string sql1 = "SELECT IdKey, Id_Dianjing, GuangBo, RD, RDK, Floor1, Floor2 " +
              "FROM " + fastablename;


            MySqlCommand cmd = new MySqlCommand(sql1, conn);

            MySqlDataAdapter daAdpter = new MySqlDataAdapter(cmd);

            DataSet dts = new DataSet();
            daAdpter.Fill(dts, "Table3");
            //关闭数据库连接
            conn.Close();
            //DataGridView数据填充
            uiDG_All.DataSource = dts;
            uiDG_All.DataMember = "Table3";
        }

        private void GenericCablingPage_Initialize(object sender, EventArgs e)
        {

        }

        private void uiDG1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        /// <summary>
        /// 全选按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiB1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < uiDG_All.RowCount - 1; i++)
            {
                uiDG_All.Rows[i].Cells["Check"].Value = true;   //列名为Check
            }
        }

        /// <summary>
        /// 取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiB2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < uiDG_All.RowCount - 1; i++)
            {
                uiDG_All.Rows[i].Cells["Check"].Value = false;   //列名为Check
            }
        }

        /// <summary>
        /// 刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiB3_Click(object sender, EventArgs e)
        {
            //连接MySQL
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                string fastablename = tablename + ".fastable";
                //SQL查询指令
                string sql1 ="SELECT IdKey, Id_Dianjing,GuangBo,RD,RDK ,Floor1,Floor2 FROM " + fastablename;

                //查询,并将结果存入
                MySqlCommand cmd = new MySqlCommand(sql1, conn);
                MySqlDataAdapter daAdpter = new MySqlDataAdapter(cmd);

                DataSet dts = new DataSet();
                daAdpter.Fill(dts, "Table_Re");

                //DataGridView数据填充
                uiDG_All.DataSource = dts;
                uiDG_All.DataMember = "Table_Re";

                //关闭数据库连接
                conn.Close();
            } // end of if
        } // end of 刷新按钮


        /// <summary>
        /// 插入广播
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiB_GBChaRu_Click(object sender, EventArgs e)
        {
            UIBInsert_FasRuoDianBlk("广播");
        }


        /// <summary>
        /// 修改广播按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiB_GBXiuGai_Click(object sender, EventArgs e)
        {
            string purpose = "广播";
            //在CAD中修改对应数量
            bool isModified = ParModify_FasRDBlk(db, ed, purpose, uiDG_All);
            //完成后弹窗提示
            if (isModified)
            {
                MessageBox.Show("广播系统修改完成");
            } //end of if
        } // end of 修改广播按钮



        /// <summary>
        /// 修改防火门按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiB_RDXiuGai_Click(object sender, EventArgs e)
        {
            string purpose = "防火门";
            //在CAD中修改对应数量
            bool isModified = ParModify_FasRDBlk(db, ed, purpose, uiDG_All);
            //完成后弹窗提示
            if (isModified)
            {
                MessageBox.Show("防火门系统修改完成");
            } //end of if
        } //end of 修改防火门按钮



        /// <summary>
        /// 插入防火门按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiB_RDChaRu_Click(object sender, EventArgs e)
        {
            UIBInsert_FasRuoDianBlk("防火门");
        }





        /// <summary>
        /// 消防弱电通用插入函数
        /// </summary>
        /// <param name="purpose"></param>
        private void UIBInsert_FasRuoDianBlk(string purpose)
        {
            //插入块所需信息的初始化
            //列、列间距
            int distance_row, distance_column;


            //将选中的行转为list
            List<FasRDClass> list_Insert = Common.FASTools.UIDGSecToFasRDClassList(uiDG_All);


            //当行数>0时开始数据处理
            if (list_Insert.Count > 0)
            {

                //根据系统对象purpose,确定插入的参数
                if (purpose == "广播")
                {
                    //列间距
                    distance_column = 13500;
                    //行间距
                    distance_row = 2000;
                }

                else if (purpose == "防火门")
                {
                    //列间距
                    distance_column = 6000;
                    //行间距
                    distance_row = 2800;
                }

                else
                {
                    MessageBox.Show("\n函数功能未找到:" + purpose + "\n请复核程序");
                    distance_column = 17000;
                    //行间距
                    distance_row = 5300;
                }
                //楼层号(int)的列表
                List<int> listFloor2 = (from d in list_Insert
                                        select d.Floor2).Distinct().ToList();
                //行数=楼层数(去重后)
                int num_All = listFloor2.Count;
                //地下的楼层数(地上由1F起,夹层均计入地下),此功能暂未加入,仅为预留
                int num_DiXia = (from d in list_Insert
                                 where d.Floor2 < 0
                                 select d.Floor2).Distinct().ToList().Count;

                //引入AutoCAD的事务处理 trans,开始插入块
                using (trans = db.TransactionManager.StartTransaction())
                {
                    if (true) //预留if
                    {
                        //获取块插入点
                        PromptPointOptions pPtOpts = new PromptPointOptions("")
                        {
                            Message = "\n选择块插入点: "
                        };
                        Point3d ptStart = ed.GetPoint(pPtOpts).Value;

                        //各行的起点
                        Point3d[] ptRowStart = new Point3d[num_All];
                        //地下的各行起点,间隔4500
                        for (int i = 0; i < num_DiXia; i++)
                        {
                            ptRowStart[i] = ptStart.PolarPoint(Math.PI / 2, distance_row * i);
                        }
                        //地上的各行起点,间隔4500,与地下额外加1000的间距
                        for (int i = num_DiXia; i < num_All; i++)
                        {
                            ptRowStart[i] = ptStart.PolarPoint(Math.PI / 2, distance_row * i + 1000);
                        }

                        //逐行插入块
                        if (num_All > 0)
                        {
                            Point3d ptColumn = new Point3d();
                            //第i行
                            for (int i = 0; i < num_All; i++)
                            {
                                //第i行对应的数据
                                List<FasRDClass> list_rowi = (from d in list_Insert
                                                              where d.Floor2 == listFloor2[i]  //所在楼层号
                                                              select d).ToList();
                                //第i列
                                for (int j = 0; j < list_rowi.Count; j++)
                                {
                                    //第j列的插入点
                                    ptColumn = ptRowStart[i].PolarPoint(0, distance_column * j);
                                    //插入块
                                    Common.FASTools.Insert_FasRuoDianblk1(db, list_rowi[j], ptColumn, purpose);
                                }
                            } // end of for
                        }// end of 插入块
                    } //end of if true
                    if (trans != null)
                    {
                        trans.Commit();//提交事务处理
                        trans = null;
                    }//测试用,防止错误
                } //end of using trans
            } //end of if(list_Insert.Count > 0)
        }//end of 消防弱电通用插入函数


        /// <summary>
        /// 修改广播、防火门按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static bool ParModify_FasRDBlk(Database db, Editor ed, string purpose, UIDataGridView uiDG_All)
        {
            bool isModified = true;
            //引入trans开始处理CAD图元
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {

                //将选中的行转为list
                List<FasRDClass> list_Insert = Common.FASTools.UIDGSecToFasRDClassList(uiDG_All);
                //对应的块名存放于此
                List<string> listBlks = new List<string>() { "FAS-广播1", "FAS-防火门系统图1" };
                //此功能需要在定义trans的前提下使用
                string blkTKNames = ARXTools.GetAnonymousBlk(db, listBlks);

                //防火分区框的筛选条件
                TypedValue[] acTypValAr_XT = new TypedValue[2];
                acTypValAr_XT.SetValue(new TypedValue((int)DxfCode.BlockName, blkTKNames), 0);
                acTypValAr_XT.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 1);
                SelectionFilter acSelFtr_XT = new SelectionFilter(acTypValAr_XT);
                // Request for objects to be selected in the drawing area
                PromptSelectionResult acSSPrompt_XT = ed.GetSelection(acSelFtr_XT);


                //获取防火分区信息
                if (acSSPrompt_XT.Status == PromptStatus.OK)  //当选中实体时执行
                {


                    int num_acSSPrompt = acSSPrompt_XT.Value.Count;         //被选中的安防系统图数量
                    ObjectId[] id_ss = acSSPrompt_XT.Value.GetObjectIds();  //将选中对象的ObjectId存入数组id_ss

                    //对所选图块进行处理:
                    //230511由`for`循环修改为`foreach`简化代码,提高可读性
                    foreach (ObjectId id in id_ss)
                    {
                        //获取防火分区信息改为获取IDKEY
                        string Id_Dianjing = id.GetAttributeInBlockReference("电井/防火分区");
                        string IdKey = id.GetAttributeInBlockReference("IDKEY");

                        //获取对应防火分区的SQL信息
                        List<FasRDClass> list_XT = list_Insert.Where(d => d.IdKey == IdKey.ToInt()).ToList();

                        //当SQL中存在此防火分区时继续执行
                        if (list_XT.Count == 1)
                        {
                            var row_Insert = list_XT[0];
                            //根据ObjID转化为块记录
                            BlockReference blkRef = (BlockReference)acTrans.GetObject(id, OpenMode.ForRead);
                            //修改块属性
                            Dictionary<string, string> atts = new Dictionary<string, string>();
                            //修改广播系统图
                            if ((purpose == "广播") && (id.GetBlockName() == "FAS-广播1"))
                            {
                                atts.Add("电井编号", row_Insert.Id_Dianjing.ToString());
                                atts.Add("IDKEY", row_Insert.IdKey.ToString());
                                atts.Add("FLOOR1", row_Insert.Floor1.ToString());
                                atts.Add("广播", row_Insert.GuangBo.ToString());
                                id.UpdateAttributesInBlock(atts);
                            } //end of 广播

                            else if ((purpose == "防火门") && (id.GetBlockName() == "FAS-防火门系统图1"))
                            {
                                atts.Add("电井编号", row_Insert.Id_Dianjing.ToString());
                                atts.Add("IDKEY", row_Insert.IdKey.ToString());
                                atts.Add("FLOOR1", row_Insert.Floor1.ToString());
                                atts.Add("常闭门", row_Insert.RD.ToString());
                                atts.Add("常开门", row_Insert.RDK.ToString());
                                id.UpdateAttributesInBlock(atts);
                            } //end of 防火门
                        } //end of if(list_XT.Count == 1)

                    } //end of foreach
                    acTrans.Commit(); //提交事务处理//
                } // end of if (acSSPrompt_XT.Status == PromptStatus.OK) 
            } //end of using trans
            //新增返回值,暂无校验条件,直接返回true
            return isModified;
        } //end of FasRDXiuGai

    } //end of class
} //end of namespace
