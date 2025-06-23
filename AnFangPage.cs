using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeakCurrent1.Common;
using Color = Autodesk.AutoCAD.Colors.Color;

namespace WeakCurrent1
{
    public partial class AnFangPage : UIPage
    {
        Database db;
        Editor ed;
        Transaction trans;
        string databasename ;

        public AnFangPage()
        {
            InitializeComponent();   //Form初始化
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
            string fileDwgBlk = @"D:\Mycode\BlkDYPD\BlkRDXT.dwg";
            if (ImportBlocksFromDwg_RD(db, fileDwgBlk))
            {
                uiLabel1.Text = "导入成功:";
                uiTextBox_Import.Text = fileDwgBlk;
            }

            else
            {
                uiLabel1.Text = "导入失败!";
                uiTextBox_Import.Text = "";
            }
        } // end of InitializeImportDWG()

        private void FPageLouceng_Load(object sender, EventArgs e)
        {
            //打开数据库连接
            using (var conn =new SQLiteConnection(SQLiteConn.ConnSQLite()))
            {
                //Parameters参数查询
                //查询所有
                string sql1 = "SELECT *  FROM   anfangtable";
                var cmd = new SQLiteCommand(sql1, conn);
                var daAdpter = new SQLiteDataAdapter(cmd);

                DataSet dts = new DataSet();
                daAdpter.Fill(dts, "AnfangTable");
                //关闭数据库连接
                conn.Close();
                //DataGridView数据填充
                uiDG_All.DataSource = dts;
                uiDG_All.DataMember = "AnfangTable";
            }

            
        }  //end of load

        /// <summary>
        /// 图块框选，从CAD中框选平面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiSymbolButton2_Click(object sender, EventArgs e)
        {
            //查询楼层信息
            List<LouCeng> list_Louceng = Common.MyTools.ChaXunlouceng();

            //Step1 在CAD中创建筛选条件SelectionFilter
            //防火分区框对应pline线的筛选条件
            TypedValue[] acTypValAr_pl = new TypedValue[2];
            acTypValAr_pl.SetValue(new TypedValue((int)DxfCode.LayerName, "E-FAS筛选02"), 0);
            acTypValAr_pl.SetValue(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"), 1);
            SelectionFilter acSelFtr_pl = new SelectionFilter(acTypValAr_pl);


            //防火分区框对应属性块的筛选条件
            TypedValue[] acTypValAr_info = new TypedValue[2];
            acTypValAr_info.SetValue(new TypedValue((int)DxfCode.LayerName, "E-FAS筛选02"), 0);
            acTypValAr_info.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 1);
            SelectionFilter acSelFtr_info = new SelectionFilter(acTypValAr_info);


            //消防块的筛选条件
            TypedValue[] acTypValAr_blk = new TypedValue[8];
            acTypValAr_blk.SetValue(new TypedValue((int)DxfCode.Operator, "<or"), 0);
            acTypValAr_blk.SetValue(new TypedValue((int)DxfCode.LayerName, "EQUIP-消防"), 1);
            acTypValAr_blk.SetValue(new TypedValue((int)DxfCode.LayerName, "EQUIP-动力"), 2);
            acTypValAr_blk.SetValue(new TypedValue((int)DxfCode.LayerName, "EQUIP-安防"), 3);
            acTypValAr_blk.SetValue(new TypedValue((int)DxfCode.LayerName, "EQUIP-通讯"), 4);
            acTypValAr_blk.SetValue(new TypedValue((int)DxfCode.LayerName, "EQUIP-楼控"), 5);
            acTypValAr_blk.SetValue(new TypedValue((int)DxfCode.Operator, "or>"), 6);
            acTypValAr_blk.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 7);
            // Assign the filter criteria to a SelectionFilter object
            SelectionFilter acSelFtr_blk = new SelectionFilter(acTypValAr_blk);


            // Request for objects to be selected in the drawing area
            PromptSelectionResult acSSPrompt_pl = ed.GetSelection(acSelFtr_pl);
            //插入开始
            //框选平面
            //先筛选出每个防火分区的pline线
            if (acSSPrompt_pl.Status == PromptStatus.OK)  //当选中实体时执行
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {


                    //初始化显示信息
                    List<AnFangClass> list2 = new List<AnFangClass>();
                    int num_acSSPrompt = acSSPrompt_pl.Value.Count;         //防火分区数量
                    ObjectId[] id_ss = acSSPrompt_pl.Value.GetObjectIds();  //将防火分区对应的pline线ObjectId存入数组id_ss'

                    //对多段线框选出的区域进行处理:
                    for (int i = 0; i < id_ss.Length; i++)
                    {
                        Polyline pline = id_ss[i].GetObject(OpenMode.ForRead) as Polyline;  //当前pline信息
                        Point3dList pts = new Point3dList(); //创建列表,用于存放多段线的顶点

                        //遍历所选多段线的顶点,并添加到Point3d类列表
                        for (int j = 0; j < pline.NumberOfVertices; j++)
                        {
                            Point3d pt = pline.GetPoint3dAt(j);
                            pts.Add(pt);
                        }

                        //筛选防火分区信息
                        PromptSelectionResult acSSPrompt_info = ed.SelectWindowPolygon(pts, acSelFtr_info); //选择pline线中的防火分区属性信息
                        PromptSelectionResult acSSPrompt_blk = ed.SelectWindowPolygon(pts, acSelFtr_blk); //选择pline线范围内的块
                        ObjectId[] id_blks = acSSPrompt_blk.Value.GetObjectIds();  //将防火分区对应块的ObjectId存入数组id_Fasbkls

                        if ((acSSPrompt_info.Status == PromptStatus.OK)&& (acSSPrompt_blk.Status == PromptStatus.OK))
                        {
                            //读取当前，第i个防火分区的信息(i从0开始)
                            ObjectId id_Info = acSSPrompt_info.Value.GetObjectIds()[0];  //获取选择集中第一个图元的ObjectId
                            string Id_Dianjing = id_Info.GetAttributeInBlockReference("电井/防火分区");
                            //string Floor1 = id_Info.GetAttributeInBlockReference("楼层");
                            string IdKey = id_Info.GetAttributeInBlockReference("KEY");

                            //第i个防火分区的各设备数量
                            int[] num_blks = new int[30];
                            num_blks=Common.AnFangTools.AnFang_jishu(num_blks, id_blks);

                            //测试用的显示信息
                            //设置相关信息
                            AnFangClass info_RD = Common.AnFangTools.JishuToAnFang(num_blks);
                            info_RD.Id_Dianjing = Id_Dianjing;
                            info_RD.Floor1 = Common.MyTools.GetLoucengming(Id_Dianjing);
                            info_RD.Floor2 = Common.MyTools.ChaXunFloor2(info_RD.Floor1, list_Louceng);
                            info_RD.IdKey = IdKey.ToInt();

                            //如果listExist中存在此防火分区
                            //说明此由多个pl线组合而成,程序会将此防火分区与现有记录相加
                            //Linq筛选电井编号相同的行
                            List<AnFangClass> listExist = (from d in list2
                                                          where d.Id_Dianjing == Id_Dianjing
                                                          select d).ToList();
                            //筛选结果的行数>0时执行
                            if(listExist.Count>0)
                            {
                                //获取重复行所在位置
                                AnFangClass infoExist = listExist[0];
                                //两行相加
                                AnFangClass info_RDAdd = Common.AnFangTools.AnFangClassXiangjia(infoExist, info_RD);
                                //删除原有的行
                                list2.Remove(infoExist);
                                info_RD = info_RDAdd;
                            }
                            //将第i个分区的信息存入list：data2中，供uidatariedview2输出校验
                            list2.Add(info_RD);
                        }

                    } //end of for i

                    trans.Commit();
                    //引入ThenBy排序，将list数据进行重排
                    List<AnFangClass> list_Pingmian = list2.OrderBy(d => d.Floor2).ThenBy(d => Common.MyTools.GetFenquHao(d.Id_Dianjing)).ToList();
                    //ThenBy结束

                    //输出,用于校验结果
                    uiDG_PingMian.DataSource = list_Pingmian;
                } //end of using trans
            } //end of if (acSSPrompt_pl.Status == PromptStatus.OK)
        } //end of 框选

   
       


        

        
        /// <summary>
        /// 修改表格内容时,CheckBox自动勾选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiDataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > 0)
            {
                int row = e.RowIndex;     //修改位置对应的行
                int col = e.ColumnIndex;  //修改位置对应的列

                if (col > 0)  //修改checkbox时不自动勾选
                {
                    uiDG_All[0, row].Value = true;  //自动勾选
                }

            }
        }

        //将平面数据存入SQL
        //当防火分区信息存在时,对该行修改
        //当防火分区信息不存在,插入新行
        private void uiSB_YingYong_Click(object sender, EventArgs e)
        {
            //从表格2中获取信息
            //List<FASJiSuanshuExcel> list1 = uiDG_PingMian.DataSource as List<FASJiSuanshuExcel>;

            //230417测试,新增勾选功能
            //从表格2中获取信息
            List<AnFangClass> list1 = Common.AnFangTools.UIDGSecToSheBeiWangList(uiDG_PingMian);
            //获取当前SQL中的所有有效主键
            List<int> listKeys =Common.AnFangTools.AnFangSQLGetIdKeys();
            //当行内容有效时进行修改SQL
            if (list1.Count > 0)
            {
                //对list3中的每行进行处理
                //添加预留变量list4，用于后续对list3的checkbox筛选
                List<AnFangClass> listInsert = (from d in list1
                                                      where d.IdKey < 1000 || !listKeys.Contains(d.IdKey)
                                                      select d).ToList();

                List<AnFangClass> listUpdate = (from d in list1
                                                      where d.IdKey > 1000 && listKeys.Contains(d.IdKey)
                                                      select d).ToList();

                //连接SQLite
                

                if(listInsert.Count>0)
                {
                    Common.AnFangTools.AnFangAddToSQL(listInsert);
                    Common.AnFangTools.AnFangUpdateBlkKeys(db, ed, listInsert);
                } // end of listInset.Count>0

                if (listUpdate.Count > 0)
                {
                    Common.AnFangTools.AnFangUpdateSQL(listUpdate);
                } // end of listUpdate.Count>0


                //重新查询SQL并加载至表格
                ReSQL();
                //弹窗提示
                MessageBox.Show("修改已完成,");

            } // end of if count>0
        } // end of UIButtonYingYgng

        //新增2024年4月30日



        

        /// <summary>
        /// 刷新SQL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiSB_ChaXun_Click(object sender, EventArgs e)
        {
            ReSQL();
        }


        /// <summary>
        /// 重新查询安防系统SQL
        /// </summary>
        public void ReSQL()
        {
            using(var conn = new SQLiteConnection(SQLiteConn.ConnSQLite()) )
            {
                //SQL查询指令
                string sql1 =
                    "SELECT *  " +
                " FROM anfangtable";
                //查询,并将结果存入
                var cmd = new SQLiteCommand(sql1, conn);
                var daAdpter = new SQLiteDataAdapter(cmd);

                DataSet dts = new DataSet();
                daAdpter.Fill(dts, "Table_Re");
                _ = dts.Tables["Table_Re"];

                //uiDataGridView1.Rows[i].Close();
                //DataGridView数据填充
                uiDG_All.DataSource = dts;
                uiDG_All.DataMember = "Table_Re";
            }
        }
        
        /// <summary>
        /// 更新图块按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiSB_GengXinBlk_Click(object sender, EventArgs e)
        {
            //设置默认对话框的打开信息
            OpenFileDialog openfiledialog = new OpenFileDialog
            {
                InitialDirectory = @"D:\Mycode\",      //默认位置
                Title = "选取火灾自动报警系统标准图块",  //窗体标题
                Filter = "dwg files (*.dwg)|*.dwg|All files (*.*)|*.*", //过滤条件
                Multiselect = false    //是否多选
            };
            //尝试从新位置导入图块文件
            try 
            {
                //显示文件选取对话框
                if(openfiledialog.ShowDialog()==DialogResult.OK)
                {
                    string fileName = openfiledialog.FileName;  //记录文件地址
                    if (ImportBlocksFromDwg_RD(db, fileName))   //校验导入结果
                    {
                        uiLabel1.Text = "图块导入成功:";
                        uiTextBox_Import.Text = fileName;
                    }
                    else  //所选文件为非DWG文件时,报错:文件类型错误
                    {
                        uiLabel1.Text = "图块导入失败!";
                        uiTextBox_Import.Text = "";
                    }
                }
                
            }
            //异常处理
            catch(Autodesk.AutoCAD.Runtime.Exception ex)
            {
                uiTextBox_Import.Text = ex.ToString();
            }

        }  //end of 更新图块


        /// <summary>
        /// 导入图块
        /// </summary>
        /// <param name="destDb"></param>
        /// <param name="sourceFileName"></param>
        /// <returns></returns>
        public static bool ImportBlocksFromDwg_RD(Database destDb, string sourceFileName)
        {
            //返回值
            bool result = false;
            //创建一个新的数据库对象，作为源数据库，以读入外部文件中的对象
            Database sourceDb = new Database(false, true);
            try
            {
                //把DWG文件读入到一个临时的数据库中
                sourceDb.ReadDwgFile(sourceFileName, System.IO.FileShare.Read, true, null);
                //创建一个变量用来存储块的ObjectId列表
                ObjectIdCollection blockIds = new ObjectIdCollection();
                //获取源数据库的事务处理管理器
                Autodesk.AutoCAD.DatabaseServices.TransactionManager tm = sourceDb.TransactionManager;
                //在源数据库中开始事务处理
                using (Transaction myT = tm.StartTransaction())
                {
                    //打开源数据库中的块表
                    BlockTable bt = (BlockTable)tm.GetObject(sourceDb.BlockTableId, OpenMode.ForRead, false);
                    //遍历每个块
                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)tm.GetObject(btrId, OpenMode.ForRead, false);
                        //只加入命名块和非布局块到复制列表中
                        if (!btr.IsAnonymous && !btr.IsLayout)
                        {
                            blockIds.Add(btrId);
                        }
                        btr.Dispose();
                    }
                    bt.Dispose();
                }
                //定义一个IdMapping对象
                IdMapping mapping = new IdMapping();
                //从源数据库向目标数据库复制块表记录
                sourceDb.WblockCloneObjects(blockIds, destDb.BlockTableId, mapping, DuplicateRecordCloning.Replace, false);
                result = true;
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {

            }
            //操作完成，销毁源数据库
            sourceDb.Dispose();
            return result;
        } //end of ImportBlocksFromDwg_RD



        //插入块
        private void uiSB_InsertBlk_Click(object sender, EventArgs e)
        {
            //插入块所需信息的初始化
            //列间距
            int distance_column = 15000;

            //将选中的行转为list
            List<AnFangClass> list_Insert = Common.AnFangTools.UIDGSecToSheBeiWangList(uiDG_All);

            //List<FASJiSuanshuExcel> list_Insert = Common.MyTools.UIDGSecToFasList(uiDG_All);

            //当行数>0时开始数据处理
            if (list_Insert.Count > 0)
            {

                //楼层号(int)的列表
                List<int> listFloor2 = (from d in list_Insert
                                        select d.Floor2).Distinct().ToList();
                //行数=楼层数(去重后)
                int num_All = listFloor2.Count;
                //地下的楼层数(地上由1F起,夹层均计入地下),此功能暂未加入,仅为预留
                int num_DiXia= (from d in list_Insert
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
                            ptRowStart[i] = ptStart.PolarPoint(Math.PI / 2, 4500 * i);
                        }
                        //地上的各行起点,间隔4500,与地下额外加1000的间距
                        for (int i = num_DiXia; i < num_All; i++)
                        {
                            ptRowStart[i] = ptStart.PolarPoint(Math.PI / 2, 4500 * i + 1000);
                        }

                        //逐行插入块
                        if(num_All>0)
                        {
                            Point3d ptColumn = new Point3d();
                            //第i行
                            for (int i=0;i< num_All;i++)
                            {
                                //第i行对应的数据
                                List<AnFangClass> list_rowi = (from d in list_Insert
                                                                     where d.Floor2 == listFloor2[i]  //所在楼层号
                                                                     select d).ToList();
                                //第i列
                                for (int j = 0; j < list_rowi.Count; j++)
                                {
                                    //第j列的插入点
                                    ptColumn = ptRowStart[i].PolarPoint(0, distance_column * j);
                                    //插入块
                                    Common.AnFangTools.Insert_AnFangblk1(db, list_rowi[j], ptColumn,"视频安防");
                                }
                            }
                        }

                    } //end of if true

                    if (trans != null)
                    {
                        trans.Commit();//提交事务处理
                        trans = null;
                    }//测试用,防止错误
                } //end of using trans

            } //end of if(list_Insert.Count > 0)
        } // end of 插入块

        

        /// <summary>
        /// 窗口关闭时,复核trans是否被清空,防止CAD崩溃
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnFangPage_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        } // end of FormClosed

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            //将选中的行转为list
            List<FASJiSuanshuExcel> listUpdateToSQL = Common.MyTools.UIDGSecToFasList(uiDG_All);
            //对选中的行进行整理,然后发送至MySQL进行处理
            //预留If
            if(true)
            {
                //数据处理函数
                try
                {
                    //UPDATE SQL
                    Common.SQLiteTools.FasUpdateSQLByKey(listUpdateToSQL);
                    MessageBox.Show("修改成功");
                    

                } // end of try
                //异常处理
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(ex.ToString());
                } //end of catch
            } // end of if true


        }

        /// <summary>
        /// 修改安防系统图1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiSB_XG_Click(object sender, EventArgs e)
        {
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {

                //将选中的行转为list
                List<AnFangClass> list_Insert = Common.AnFangTools.UIDGSecToSheBeiWangList(uiDG_All);
                //对应的块名存放于此
                List<string> listBlks = new List<string>()
                {
                    "RDXT-安防系统1"
                };
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
                    for (int i = 0; i < id_ss.Length; i++)
                    {
                        //获取防火分区信息改为获取IDKEY
                        //string Id_Dianjing = id_ss[i].GetAttributeInBlockReference("电井/防火分区");
                        string IdKey = id_ss[i].GetAttributeInBlockReference("IDKEY");

                        //获取对应防火分区的SQL信息
                        List<AnFangClass> list_XT = (from d in list_Insert
                                                     where d.IdKey == IdKey.ToInt()
                                                     select d).ToList();
                        //当SQL中存在此防火分区时继续执行
                        if (list_XT.Count == 1)
                        {
                            //
                            //根据ObjID转化为块记录
                            BlockReference blkRef = (BlockReference)acTrans.GetObject(id_ss[i], OpenMode.ForRead);
                            //修改块属性
                            Dictionary<string, string> atts = new Dictionary<string, string>
                            {
                                { "电井编号", list_XT[0].Id_Dianjing.ToString() },
                                { "IDKEY", list_XT[0].IdKey.ToString() },
                                { "枪机", list_XT[0].JianKong_qiangji.ToString() },
                                { "球机", list_XT[0].JianKong_qiuji.ToString() },
                                { "巡更", list_XT[0].XunGeng.ToString() }
                            };
                            id_ss[i].UpdateAttributesInBlock(atts);
                            

                        } // end of count==1 

                        //意外处理:SQL中查不到IDKEY
                        else if(list_XT.Count<1)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("\n当前防火分区不存在:"+ list_XT[0].Id_Dianjing + "\n主键编号:" +IdKey);
                        } // end of conut<!
                    } //end of for i
                    //提交事务处理
                    acTrans.Commit();
                } //end of if (acSSPrompt_pl.Status == PromptStatus.OK)
            } //end of using trans
        } // end of uiSyButton

        #region 没啥用但又不能删的函数
        private void AnFangPage_Initialize(object sender, EventArgs e)
        {

        }
        private void uiTextBox_Import_TextChanged(object sender, EventArgs e)
        {

        }
        private void uiDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void uiSymbolButton2_Click_1(object sender, EventArgs e)
        {

        }

        private void uiTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void uiDG_PingMian_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        #endregion


        #region 两个表格的全选按钮
        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < uiDG_All.RowCount - 1; i++)
            {
                uiDG_All.Rows[i].Cells["Check"].Value = true;   //列名为Check
            }
        } //end of UIButton 全选


        /// <summary>
        /// 全部取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < uiDG_All.RowCount - 1; i++)
            {
                uiDG_All.Rows[i].Cells["Check"].Value = false;  //列名为Check
            }
        }

        /// <summary>
        /// 平面表格全勾选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButtonCheck1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < uiDG_PingMian.RowCount; i++)
            {
                uiDG_PingMian.Rows[i].Cells[0].Value = true;   //列名为Check1
            }
        }

        /// <summary>
        /// 平面表格全部取消勾选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton_UnCheck1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < uiDG_PingMian.RowCount; i++)
            {
                uiDG_PingMian.Rows[i].Cells["Check1"].Value = false;  //列名为Check1
            }
        }

        #endregion

        private void uiLine1_Click(object sender, EventArgs e)
        {

        }

        private void uiB_YiKatong2_Click(object sender, EventArgs e)
        {
            UIBInsert_SheBeiwangBlk("一卡通");
            ////插入块所需信息的初始化
            ////列间距
            //int distance_column = 15000;

            ////将选中的行转为list
            //List<AnFangClass> list_Insert = Common.AnFangTools.UIDGSecToSheBeiWangList(uiDG_All);

            ////List<FASJiSuanshuExcel> list_Insert = Common.MyTools.UIDGSecToFasList(uiDG_All);

            ////当行数>0时开始数据处理
            //if (list_Insert.Count > 0)
            //{

            //    //楼层号(int)的列表
            //    List<int> listFloor2 = (from d in list_Insert
            //                            select d.Floor2).Distinct().ToList();
            //    //行数=楼层数(去重后)
            //    int num_All = listFloor2.Count;
            //    //地下的楼层数(地上由1F起,夹层均计入地下),此功能暂未加入,仅为预留
            //    int num_DiXia = (from d in list_Insert
            //                     where d.Floor2 < 0
            //                     select d.Floor2).Distinct().ToList().Count;

            //    //引入AutoCAD的事务处理 trans,开始插入块
            //    using (trans = db.TransactionManager.StartTransaction())
            //    {
            //        if (true) //预留if
            //        {
            //            //获取块插入点
            //            PromptPointOptions pPtOpts = new PromptPointOptions("");
            //            pPtOpts.Message = "\n选择块插入点: ";
            //            Point3d ptStart = ed.GetPoint(pPtOpts).Value;

            //            //各行的起点
            //            Point3d[] ptRowStart = new Point3d[num_All];
            //            //地下的各行起点,间隔4500
            //            for (int i = 0; i < num_DiXia; i++)
            //            {
            //                ptRowStart[i] = ptStart.PolarPoint(Math.PI / 2, 3900 * i);
            //            }
            //            //地上的各行起点,间隔4500,与地下额外加1000的间距
            //            for (int i = num_DiXia; i < num_All; i++)
            //            {
            //                ptRowStart[i] = ptStart.PolarPoint(Math.PI / 2, 3900 * i + 1000);
            //            }

            //            //逐行插入块
            //            if (num_All > 0)
            //            {
            //                Point3d ptColumn = new Point3d();
            //                //第i行
            //                for (int i = 0; i < num_All; i++)
            //                {
            //                    //第i行对应的数据
            //                    List<AnFangClass> list_rowi = (from d in list_Insert
            //                                                   where d.Floor2 == listFloor2[i]  //所在楼层号
            //                                                   select d).ToList();
            //                    //第i列
            //                    for (int j = 0; j < list_rowi.Count; j++)
            //                    {
            //                        //第j列的插入点
            //                        ptColumn = ptRowStart[i].PolarPoint(0, distance_column * j);
            //                        //插入块
            //                        Common.AnFangTools.Insert_AnFangblk1(db, list_rowi[j], ptColumn,"一卡通");
            //                    }
            //                }
            //            }

            //        } //end of if true

            //        if (trans != null)
            //        {
            //            trans.Commit();//提交事务处理
            //            trans = null;
            //        }//测试用,防止错误
            //    } //end of using trans

            //} //end of if(list_Insert.Count > 0)
        } //end of UIButton


        /// <summary>
        /// 修改一卡通系统图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiB_YiKatong1_Click(object sender, EventArgs e)
        {
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {

                //将选中的行转为list
                List<AnFangClass> list_Insert = Common.AnFangTools.UIDGSecToSheBeiWangList(uiDG_All);
                //对应的块名存放于此
                List<string> listBlks = new List<string>()
                {
                    "RDXT-一卡通门禁1"
                };
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
                    for (int i = 0; i < id_ss.Length; i++)
                    {
                        //获取防火分区信息改为获取IDKEY
                        //string Id_Dianjing = id_ss[i].GetAttributeInBlockReference("电井/防火分区");
                        string IdKey = id_ss[i].GetAttributeInBlockReference("IDKEY");

                        //获取对应防火分区的SQL信息
                        List<AnFangClass> list_XT = (from d in list_Insert
                                                     where d.IdKey == IdKey.ToInt()
                                                     select d).ToList();

                        
                        //当SQL中存在此防火分区时继续执行
                        if (list_XT.Count == 1)
                        {
                            //
                            AnFangClass row_Insert = list_XT[0];
                            //根据ObjID转化为块记录
                            BlockReference blkRef = (BlockReference)acTrans.GetObject(id_ss[i], OpenMode.ForRead);
                            //修改块属性
                            Dictionary<string, string> atts = new Dictionary<string, string>
                            {
                                { "电井编号", row_Insert.Id_Dianjing.ToString() },
                                { "IDKEY", row_Insert.IdKey.ToString() },
                                { "门禁控制器", row_Insert.MenJinkongzhi.ToString() },
                                { "门禁", row_Insert.MenJin.ToString() }
                            };
                            id_ss[i].UpdateAttributesInBlock(atts);
                        } // end of count==1 
                        //意外处理:SQL中查不到IDKEY
                        else if (list_XT.Count < 1)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("\n当前防火分区不存在:" + list_XT[0].Id_Dianjing + "\n主键编号:" + IdKey);
                        } // end of conut<!
                    } //end of for i
                    //提交事务处理
                    acTrans.Commit();
                } //end of if (acSSPrompt_pl.Status == PromptStatus.OK)
            } //end of using trans
        } // end of UIButton


        /// <summary>
        /// 生成视频安防系统图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiB_AnFang2_Click(object sender, EventArgs e)
        {
            UIBInsert_SheBeiwangBlk("视频安防");
            ////插入块所需信息的初始化
            ////列间距
            //int distance_column = 15000;

            ////将选中的行转为list
            //List<AnFangClass> list_Insert = Common.AnFangTools.UIDGSecToSheBeiWangList(uiDG_All);

            ////List<FASJiSuanshuExcel> list_Insert = Common.MyTools.UIDGSecToFasList(uiDG_All);

            ////当行数>0时开始数据处理
            //if (list_Insert.Count > 0)
            //{

            //    //楼层号(int)的列表
            //    List<int> listFloor2 = (from d in list_Insert
            //                            select d.Floor2).Distinct().ToList();
            //    //行数=楼层数(去重后)
            //    int num_All = listFloor2.Count;
            //    //地下的楼层数(地上由1F起,夹层均计入地下),此功能暂未加入,仅为预留
            //    int num_DiXia = (from d in list_Insert
            //                     where d.Floor2 < 0
            //                     select d.Floor2).Distinct().ToList().Count;

            //    //引入AutoCAD的事务处理 trans,开始插入块
            //    using (trans = db.TransactionManager.StartTransaction())
            //    {
            //        if (true) //预留if
            //        {
            //            //获取块插入点
            //            PromptPointOptions pPtOpts = new PromptPointOptions("");
            //            pPtOpts.Message = "\n选择块插入点: ";
            //            Point3d ptStart = ed.GetPoint(pPtOpts).Value;

            //            //各行的起点
            //            Point3d[] ptRowStart = new Point3d[num_All];
            //            //地下的各行起点,间隔4500
            //            for (int i = 0; i < num_DiXia; i++)
            //            {
            //                ptRowStart[i] = ptStart.PolarPoint(Math.PI / 2, 3900 * i);
            //            }
            //            //地上的各行起点,间隔4500,与地下额外加1000的间距
            //            for (int i = num_DiXia; i < num_All; i++)
            //            {
            //                ptRowStart[i] = ptStart.PolarPoint(Math.PI / 2, 3900 * i + 1000);
            //            }

            //            //逐行插入块
            //            if (num_All > 0)
            //            {
            //                Point3d ptColumn = new Point3d();
            //                //第i行
            //                for (int i = 0; i < num_All; i++)
            //                {
            //                    //第i行对应的数据
            //                    List<AnFangClass> list_rowi = (from d in list_Insert
            //                                                   where d.Floor2 == listFloor2[i]  //所在楼层号
            //                                                   select d).ToList();
            //                    //第i列
            //                    for (int j = 0; j < list_rowi.Count; j++)
            //                    {
            //                        //第j列的插入点
            //                        ptColumn = ptRowStart[i].PolarPoint(0, distance_column * j);
            //                        //插入块
            //                        Common.AnFangTools.Insert_AnFangblk1(db, list_rowi[j], ptColumn, "视频安防");
            //                    }
            //                }
            //            }

            //        } //end of if true

            //        if (trans != null)
            //        {
            //            trans.Commit();//提交事务处理
            //            trans = null;
            //        }//测试用,防止错误
            //    } //end of using trans

            //} //end of if(list_Insert.Count > 0)
        } //end of uiB_AnFang2_Click 生成安防

        private void uiB_AnFang1_Click(object sender, EventArgs e)
        {
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {

                //将选中的行转为list
                List<AnFangClass> list_Insert = Common.AnFangTools.UIDGSecToSheBeiWangList(uiDG_All);
                //对应的块名存放于此
                List<string> listBlks = new List<string>()
                {
                    "RDXT-安防系统1"
                };
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
                    for (int i = 0; i < id_ss.Length; i++)
                    {
                        //获取防火分区信息改为获取IDKEY
                        //string Id_Dianjing = id_ss[i].GetAttributeInBlockReference("电井/防火分区");
                        string IdKey = id_ss[i].GetAttributeInBlockReference("IDKEY");

                        //获取对应防火分区的SQL信息
                        List<AnFangClass> list_XT = (from d in list_Insert
                                                     where d.IdKey == IdKey.ToInt()
                                                     select d).ToList();
                        //当SQL中存在此防火分区时继续执行
                        if (list_XT.Count == 1)
                        {
                            //
                            //根据ObjID转化为块记录
                            BlockReference blkRef = (BlockReference)acTrans.GetObject(id_ss[i], OpenMode.ForRead);
                            //修改块属性
                            Dictionary<string, string> atts = new Dictionary<string, string>
                            {
                                { "电井编号", list_XT[0].Id_Dianjing.ToString() },
                                { "IDKEY", list_XT[0].IdKey.ToString() },
                                { "枪机", list_XT[0].JianKong_qiangji.ToString() },
                                { "球机", list_XT[0].JianKong_qiuji.ToString() },
                                { "巡更", list_XT[0].XunGeng.ToString() }
                            };
                            id_ss[i].UpdateAttributesInBlock(atts);


                        } // end of count==1 

                        //意外处理:SQL中查不到IDKEY
                        else if (list_XT.Count < 1)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("\n当前防火分区不存在:" + list_XT[0].Id_Dianjing + "\n主键编号:" + IdKey);
                        } // end of conut<!
                    } //end of for i
                    //提交事务处理
                    acTrans.Commit();
                } //end of if (acSSPrompt_pl.Status == PromptStatus.OK)
            } //end of using trans
        } //end of uiB_AnFang1_Click 修改安防系统图

        private void uiB_SBW2_Click(object sender, EventArgs e)
        {
            UIBInsert_SheBeiwangBlk("设备网");
        }



        //安防系统通用插入函数
        private void UIBInsert_SheBeiwangBlk(string purpose)
        {
            //插入块所需信息的初始化
            //列、列间距
            int distance_row , distance_column ;


            //将选中的行转为list
            List<AnFangClass> list_Insert = Common.AnFangTools.UIDGSecToSheBeiWangList(uiDG_All);


            //当行数>0时开始数据处理
            if (list_Insert.Count > 0)
            {

                //根据系统对象purpose,确定插入的参数
                if(purpose == "视频安防")
                {
                    //列间距
                    distance_column = 10600;
                    //行间距
                     distance_row = 3900;
                }

                else if (purpose == "一卡通")
                {
                    //列间距
                    distance_column = 10600;
                    //行间距
                    distance_row = 3900;
                }

                else if (purpose == "设备网")
                {
                    //列间距
                    distance_column = 17000;
                    //行间距
                    distance_row = 5300;
                }
                else if (purpose == "综合布线")
                {
                    //列间距
                    distance_column = 14500;
                    //行间距
                    distance_row = 4000;
                }
                else 
                {
                    MessageBox.Show("\n函数功能未找到:"+purpose+"\n请复核程序");
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
                                List<AnFangClass> list_rowi = (from d in list_Insert
                                                               where d.Floor2 == listFloor2[i]  //所在楼层号
                                                               select d).ToList();
                                //第i列
                                for (int j = 0; j < list_rowi.Count; j++)
                                {
                                    //第j列的插入点
                                    ptColumn = ptRowStart[i].PolarPoint(0, distance_column * j);
                                    //插入块
                                    Common.AnFangTools.Insert_AnFangblk1(db, list_rowi[j], ptColumn, purpose);
                                }
                            }
                        }// end of 插入块

                    } //end of if true

                    if (trans != null)
                    {
                        trans.Commit();//提交事务处理
                        trans = null;
                    }//测试用,防止错误
                } //end of using trans

            } //end of if(list_Insert.Count > 0)

        }//end of 安防系统通用插入函数

        /// <summary>
        /// 2024年4月30日新增:从SQL中删除选中的行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiSymbolButton2_Click_2(object sender, EventArgs e)
        {
            //230417测试,新增勾选功能
            //从表格2中获取信息
            List<AnFangClass> list1 = Common.AnFangTools.UIDGSecToSheBeiWangList(uiDG_PingMian);
            //获取当前SQL中的所有有效主键
            List<int> listKeys = Common.AnFangTools.AnFangSQLGetIdKeys();
            
            //当行内容有效时进行修改SQL
            if (list1.Count > 0)
            {
                List<int> listDeleteIDKey = (from d in list1
                                                where d.IdKey > 1000 && listKeys.Contains(d.IdKey)
                                                select d.IdKey).ToList();
                // 构建包含行 IDKEY 的字符串
                string idsString = string.Join(",", listDeleteIDKey);

                
                using(var conn = new SQLiteConnection(SQLiteConn.ConnSQLite()))
                {
                    //删除行
                    string sqldelete = "DELETE FROM anfangtable WHERE IDKEY IN ({idsString})";
                    using (var command = new SQLiteCommand(sqldelete, conn))
                    {
                        // 执行 SQL 命令
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }
                

                //重新查询SQL并加载至表格
                ReSQL();
                //弹窗提示
                MessageBox.Show("所选行已删除,删除数量"+ listDeleteIDKey.Count);
            } // end of if count>0

        }
        /// <summary>
        /// 修改设备网
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiB_SBW1_Click(object sender, EventArgs e)
        {
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {

                //将选中的行转为list
                List<AnFangClass> list_Insert = Common.AnFangTools.UIDGSecToSheBeiWangList(uiDG_All);
                //对应的块名存放于此
                List<string> listBlks = new List<string>()
                {
                    "RDXT-设备网1"
                };
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
                    for (int i = 0; i < id_ss.Length; i++)
                    {
                        //获取防火分区信息改为获取IDKEY
                        //string Id_Dianjing = id_ss[i].GetAttributeInBlockReference("电井/防火分区");
                        string IdKey = id_ss[i].GetAttributeInBlockReference("IDKEY");

                        //获取对应防火分区的SQL信息
                        List<AnFangClass> list_XT = (from d in list_Insert
                                                     where d.IdKey == IdKey.ToInt()
                                                     select d).ToList();


                        //当SQL中存在此防火分区时继续执行
                        if (list_XT.Count == 1)
                        {
                            //
                            AnFangClass row_Insert = list_XT[0];
                            //根据ObjID转化为块记录
                            BlockReference blkRef = (BlockReference)acTrans.GetObject(id_ss[i], OpenMode.ForRead);
                            //修改块属性
                            Dictionary<string, string> atts = new Dictionary<string, string>
                            {
                                { "电井编号", row_Insert.Id_Dianjing.ToString() },
                                { "IDKEY", row_Insert.IdKey.ToString() },
                                { "门禁控制器", "x" + row_Insert.MenJinkongzhi.ToString() },
                                { "POE交换机", "x" + row_Insert.ONU_POE24.ToString() },
                                { "普通交换机", "x" + row_Insert.ONU_24.ToString() },
                                { "能耗网关", "x" + row_Insert.NengHao.ToString() },
                                { "信息发布", "x" + row_Insert.XinXifabu.ToString() },
                                { "摄像头", "x" + row_Insert.JianKong.ToString() },
                                { "进线光缆", "6芯光缆" }
                            };
                            id_ss[i].UpdateAttributesInBlock(atts);
                        } // end of count==1 
                        //意外处理:SQL中查不到IDKEY
                        else if (list_XT.Count < 1)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("\n当前防火分区不存在:" + list_XT[0].Id_Dianjing + "\n主键编号:" + IdKey);
                        } // end of conut<!
                    } //end of for i
                    //提交事务处理
                    acTrans.Commit();
                } //end of if (acSSPrompt_pl.Status == PromptStatus.OK)
            } //end of using trans
        }

        private void uiB_GC2_Click(object sender, EventArgs e)
        {
            UIBInsert_SheBeiwangBlk("综合布线");
        }

        private void uiB_GC1_Click(object sender, EventArgs e)
        {
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {

                //将选中的行转为list
                List<AnFangClass> list_Insert = Common.AnFangTools.UIDGSecToSheBeiWangList(uiDG_All);
                //对应的块名存放于此
                List<string> listBlks = new List<string>()
                {
                    "RDXT-综合布线-240429"
                };
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
                    for (int i = 0; i < id_ss.Length; i++)
                    {
                        //获取防火分区信息改为获取IDKEY
                        string Id_Dianjing = id_ss[i].GetAttributeInBlockReference("电井/防火分区");
                        string IdKey = id_ss[i].GetAttributeInBlockReference("IDKEY");
                        

                        //获取对应防火分区的SQL信息
                        List<AnFangClass> list_XT = (from d in list_Insert
                                                     where d.IdKey == IdKey.ToInt()
                                                     select d).ToList();


                        //当SQL中存在此防火分区时继续执行
                        if (list_XT.Count == 1)
                        {
                            //
                            AnFangClass row_Insert = list_XT[0];
                            //2024年4月29日 新增分光器的进线光缆计算:每根12芯光缆最多带5个分光器
                            //默认TYPEB保护(每个分光器有两根进线),
                            //默认SC接口,但为LC接口改造预留条件,所以最多使用5口10芯,预留1口2芯,合计12芯
                            int num_GuangXian = 2 * (int)(Math.Ceiling((decimal)(row_Insert.FenGuangqi_216 / 10.00)));
                            //根据ObjID转化为块记录
                            BlockReference blkRef = (BlockReference)acTrans.GetObject(id_ss[i], OpenMode.ForRead);
                            //修改块属性
                            Dictionary<string, string> atts = new Dictionary<string, string>
                            {
                                { "电井编号", row_Insert.Id_Dianjing.ToString() },
                                { "IDKEY", row_Insert.IdKey.ToString() },
                                { "FLOOR1", row_Insert.Floor1.ToString() },
                                { "进线光缆", "12芯光缆x" + num_GuangXian.ToString() },
                                { "配线光缆", "4芯光缆x" + row_Insert.AHD.ToString() },
                                { "分光器", row_Insert.FenGuangqi_216.ToString() },
                                { "ONU", row_Insert.ONU_GC24POE.ToString() },
                                { "AHD", row_Insert.AHD.ToString() },
                                { "AP", row_Insert.AP.ToString() }
                            };
                            id_ss[i].UpdateAttributesInBlock(atts);
                        } // end of count==1 
                        //意外处理:SQL中查不到IDKEY
                        else if (list_XT.Count < 1)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("\n当前防火分区不存在:" + Id_Dianjing + "\n主键编号:" + IdKey);
                        } // end of conut<!
                    } //end of for i
                    //提交事务处理
                    acTrans.Commit();
                } //end of if (acSSPrompt_pl.Status == PromptStatus.OK)
            } //end of using trans
        }

        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        
    } // end of class

}// end of namespace
