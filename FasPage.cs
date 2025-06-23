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

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;

using Color = Autodesk.AutoCAD.Colors.Color;
using System.Data.SQLite;
using WeakCurrent1.Common;

namespace WeakCurrent1
{
    public partial class FasPage : UIPage
    {
        Database db;
        Editor ed;
        Transaction trans;
        //MySqlConnection conn;

        public FasPage()
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
            string fileDwgBlk = @"D:\Mycode\BlkDYPD\BlkFAS.dwg";
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
            //加载数据库
            re_FASTABLE_FROM_SQL();
            
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


            //CAD所需变量初始化
            //防火分区框对应pline线的筛选条件
            SelectionFilter acSelFtr_pl = new SelectionFilter(
            new TypedValue[] {
                new TypedValue((int)DxfCode.LayerName, "E-FAS筛选02"),
                new TypedValue((int)DxfCode.Start, "LWPOLYLINE")
            });


            //防火分区框对应属性块的筛选条件
            SelectionFilter acSelFtr_info = new SelectionFilter(
            new TypedValue[] {
                new TypedValue((int)DxfCode.LayerName, "E-FAS筛选02"),
                new TypedValue((int)DxfCode.Start, "INSERT")
            });

            //消防块的筛选条件
            SelectionFilter acSelFtr_blk = new SelectionFilter(
            new TypedValue[] {
                new TypedValue((int)DxfCode.Operator, "<or"),
                new TypedValue((int)DxfCode.LayerName, "EQUIP-消防"),
                new TypedValue((int)DxfCode.LayerName, "EQUIP-动力"),
                new TypedValue((int)DxfCode.LayerName, "EQUIP-安防"),
                new TypedValue((int)DxfCode.LayerName, "EQUIP-通讯"),
                new TypedValue((int)DxfCode.LayerName, "EQUIP-楼控"),
                new TypedValue((int)DxfCode.LayerName, "EQUIP-广播"),
                new TypedValue((int)DxfCode.Operator, "or>"),
                new TypedValue((int)DxfCode.Start, "INSERT")  
            });

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
                    List<FASJiSuanshuExcel> list2 = new List<FASJiSuanshuExcel>();
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
                            Point3d pt  = pline.GetPoint3dAt(j);
                            pts.Add(pt);
                        }

                        //筛选防火分区信息
                        PromptSelectionResult acSSPrompt_info = ed.SelectWindowPolygon(pts, acSelFtr_info); //选择pline线中的防火分区属性信息
                        //2024年12月9日修改: 落在边界上的也会被读入
                        //PromptSelectionResult acSSPrompt_blk = ed.SelectWindowPolygon(pts, acSelFtr_blk); //选择pline线范围内的块
                        PromptSelectionResult acSSPrompt_blk = ed.SelectCrossingPolygon(pts, acSelFtr_blk); //选择pline线范围内的块



                        if ((acSSPrompt_info.Status == PromptStatus.OK) && (acSSPrompt_blk.Status == PromptStatus.OK) && (acSSPrompt_blk.Value.Count > 0))
                        {
                            //id_Fasbkls移入if内
                            ObjectId[] id_Fasbkls = acSSPrompt_blk.Value.GetObjectIds();  //将防火分区对应块的ObjectId存入数组id_Fasbkls
                            //读取当前，第i个防火分区的信息(i从0开始)
                            ObjectId id_Info = acSSPrompt_info.Value.GetObjectIds()[0];  //获取选择集中第一个图元的ObjectId

                            string Id_Dianjing = id_Info.GetAttributeInBlockReference("电井/防火分区");
                            string IdKey = id_Info.GetAttributeInBlockReference("KEY");

                            //第i个防火分区的各设备数量
                            FASJiSuanshuExcel info_RD = Common.FASTools.ParCal_FasBlkToClass(list_Louceng,id_Info,id_Fasbkls);
                            

                            //如果listExist中存在此防火分区
                            //说明此由多个pl线组合而成,程序会将此防火分区与现有记录相加
                            //Linq筛选电井编号相同的行
                            List<FASJiSuanshuExcel> listExist = (from d in list2
                                                          where d.Id_Dianjing == Id_Dianjing
                                                          select d).ToList();
                            //筛选结果的行数>0时执行
                            if(listExist.Count>0)
                            {
                                //获取重复行所在位置
                                FASJiSuanshuExcel infoExist = listExist[0];
                                //两行相加
                                FASJiSuanshuExcel info_RDAdd = Common.FASTools.FasClassXiangjia(infoExist, info_RD);
                                //删除原有的行
                                list2.Remove(infoExist);
                                info_RD = info_RDAdd;
                            }

                            //将第i个分区的信息存入list：data2中，供uidatariedview2输出校验
                            list2.Add(info_RD);
                        }

                    } //end of for i

                    trans.Commit();
                    //显示前,将list进行简单排序
                    List<FASJiSuanshuExcel> list_Pingmian = (from d in list2
                                        orderby d.Id_Dianjing
                                        select d).ToList();

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
            List<FASJiSuanshuExcel> list1 = Common.MyTools.UIDGSecToFasList(uiDG_PingMian);
            //获取当前SQL中的所有有效主键
            List<int> listKeys = Common.SQLiteTools.FasSQLGetIdKeys();
            //当行内容有效时进行修改SQL
            if (list1.Count > 0)
            {
                //对list3中的每行进行处理
                //添加预留变量list4，用于后续对list3的checkbox筛选
                List<FASJiSuanshuExcel> listInsert = (from d in list1
                                                      where !listKeys.Contains(d.IdKey)
                                                      select d).ToList();

                List<FASJiSuanshuExcel> listUpdate = (from d in list1
                                                      where listKeys.Contains(d.IdKey)
                                                      select d).ToList();

                

                if(listInsert.Count>0)
                {
                    //在MYSQL中新建行
                    Common.SQLiteTools.FasSQLInsert(listInsert);
                    //在AutoCAD中更新主键
                    Common.FASTools.UpdateBlkKeys(db,ed,listInsert);
                } // end of listInset.Count>0

                if(listUpdate.Count>0)
                {
                    Common.SQLiteTools.FasUpdate(listUpdate);
                } // end of listUpdate.Count>0


                //重新查询SQL并加载至表格
                re_FASTABLE_FROM_SQL();
                //弹窗提示
                MessageBox.Show("修改已完成,");

            } // end of if count>0
        } // end of UIButtonYingYgng


        

        /// <summary>
        /// 刷新SQL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiSB_ChaXun_Click(object sender, EventArgs e)
        {
            re_FASTABLE_FROM_SQL();
        }


        /// <summary>
        /// 重新查询SQL
        /// </summary>
        public void re_FASTABLE_FROM_SQL()
        {
            using (var conn = new SQLiteConnection(SQLiteConn.ConnSQLite()))
            {
                    //SQL查询指令
                    string sql1 =
                        "SELECT IdKey, Id_Dianjing ,Floor1,Floor2,SI, " +
                    "DianWeiall,DianWeiliandong, HuiLu, " +
                    "ShouBao,ShengGuang,JuanLianA,JuanLianB,QieFei,DianTi,GP," +
                    "ZYFJ,BFJ,PYFJ,XHSB,PLB," +
                    "WYB,BEC,BED,BEEH,BECH," +
                    "DYCB,Fa70,Fa280,ShuiLiuZSQ,XinHaofa," +
                    "ShiShiBJF,LiuLiangKG,YaLiKG,WenGan,XiaoHuoshuan," +
                    "EXWenGan,XFDianHua,LouCengXSQ,gmt_create,gmt_change," +
                    "B,YanGan,PaiYanchuang,GuangBo,RD,RDK,XXGSYanGan  " +
                    " FROM fastable" ;
                    //查询,并将结果存入
                    var cmd = new SQLiteCommand(sql1, conn);
                    var daAdpter = new SQLiteDataAdapter(cmd);

                    DataSet dts = new DataSet();
                    daAdpter.Fill(dts, "Table_Re");

                    System.Data.DataTable dt1 = dts.Tables["Table_Re"];

                    //uiDataGridView1.Rows[i].Close();
                    //DataGridView数据填充
                    uiDG_All.DataSource = dts;
                    uiDG_All.DataMember = "Table_Re";

            } //end of using conn
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
            int distance2 = 26000;

            //将选中的行转为list
            List<FASJiSuanshuExcel> list_Insert = Common.MyTools.UIDGSecToFasList(uiDG_All);

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
                            Message = "\nEnter the start point of the line: "
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
                                List<FASJiSuanshuExcel> list_rowi = (from d in list_Insert
                                                                     where d.Floor2 == listFloor2[i]  //所在楼层号
                                                                     select d).ToList();
                                //第i列
                                for (int j = 0; j < list_rowi.Count; j++)
                                {
                                    //第j列的插入点
                                    ptColumn = ptRowStart[i].PolarPoint(0, distance2 * j);
                                    //插入块
                                    Common.FASTools.Insert_Xiaofangblk2(db, list_rowi[j], ptColumn);
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
        private void FasPage_FormClosed(object sender, FormClosedEventArgs e)
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
        /// 修改系统图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiSB_XG_Click(object sender, EventArgs e)
        {
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {

                //将选中的行转为list
                List<FASJiSuanshuExcel> list_Insert = Common.MyTools.UIDGSecToFasList(uiDG_All);
                //对应的块名存放于此
                List<string> listBlks = new List<string>()
                {
                    "FAS-楼层1"
                };
                //此功能需要在定义trans的前提下使用
                string blkTKNames = ARXTools.GetAnonymousBlk(db, listBlks);

                //防火分区框的筛选条件
                TypedValue[] acTypValAr_XT = new TypedValue[2];
                //acTypValAr_XT.SetValue(new TypedValue((int)DxfCode.LayerName, "E-火灾报警系统图专用图层"), 0);
                acTypValAr_XT.SetValue(new TypedValue((int)DxfCode.BlockName, blkTKNames), 0);
                acTypValAr_XT.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 1);
                SelectionFilter acSelFtr_XT = new SelectionFilter(acTypValAr_XT);
                // Request for objects to be selected in the drawing area
                PromptSelectionResult acSSPrompt_XT = ed.GetSelection(acSelFtr_XT);

                //系统图的筛选条件
                TypedValue[] acTypValAr_Fas = new TypedValue[4];
                acTypValAr_Fas.SetValue(new TypedValue((int)DxfCode.Operator, "<or"), 0);
                acTypValAr_Fas.SetValue(new TypedValue((int)DxfCode.LayerName, "E-火灾报警系统图专用图层"), 1);
                acTypValAr_Fas.SetValue(new TypedValue((int)DxfCode.LayerName, "WIRE-照明"), 2);
                acTypValAr_Fas.SetValue(new TypedValue((int)DxfCode.Operator, "or>"), 3);
                SelectionFilter acSelFtr_Fas = new SelectionFilter(acTypValAr_Fas);
                // Request for objects to be selected in the drawing area
                //PromptSelectionResult acSSPrompt_Fas = ed.GetSelection(acSelFtr_Fas);



                //获取防火分区信息
                if (acSSPrompt_XT.Status == PromptStatus.OK)  //当选中实体时执行
                {


                    int num_acSSPrompt = acSSPrompt_XT.Value.Count;         //防火分区框数量
                    ObjectId[] id_ss = acSSPrompt_XT.Value.GetObjectIds();  //将防火分区框块对应的ObjectId存入数组id_ss'

                    //对多段线框选出的区域进行处理:
                    for (int i = 0; i < id_ss.Length; i++)
                    {
                        //获取防火分区信息
                        string Id_Dianjing = id_ss[i].GetAttributeInBlockReference("电井/防火分区");
                        string Floor1 = id_ss[i].GetAttributeInBlockReference("楼层");
                        string IdKey = id_ss[i].GetAttributeInBlockReference("KEY");

                        //获取对应防火分区的SQL信息
                        List<FASJiSuanshuExcel> list_XT = (from d in list_Insert
                                       where d.IdKey == IdKey.ToInt()
                                       select d).ToList();
                        //当SQL中存在此防火分区时继续执行
                        if(list_XT.Count == 1)
                        {
                            //
                            //根据ObjID转化为块记录
                            BlockReference blkRef = (BlockReference)acTrans.GetObject(id_ss[i], OpenMode.ForRead);
                            //获取防火分区块的插入点
                            Point3d pt_Insert = blkRef.Position.PolarPoint(0, -10).PolarPoint(Math.PI / 2, -10);
                            //根据插入点确定矩形筛选框的顶点
                            Point3dList pts = new Point3dList()
                        {
                            pt_Insert,
                            pt_Insert.PolarPoint(0,26020),
                            pt_Insert.PolarPoint(0,26020).PolarPoint(Math.PI/2,4520),
                            pt_Insert.PolarPoint(Math.PI/2,4520)
                        }; //创建列表,用于存放多段线的顶点



                            //筛选防火分区信息
                            PromptSelectionResult acSSPrompt_Fas = ed.SelectWindowPolygon(pts, acSelFtr_Fas); //选择pline线范围内的块
                            ObjectId[] id_Fasbkls = acSSPrompt_Fas.Value.GetObjectIds();  //将防火分区对应块的ObjectId存入数组id_Fasbkls
                                                                                          //当选择集有效时,删除框内所有图元
                            if ((acSSPrompt_XT.Status == PromptStatus.OK) && (acSSPrompt_Fas.Status == PromptStatus.OK))
                            {

                                //删除筛选内的所有图元
                                foreach (ObjectId id in id_Fasbkls)
                                {
                                    DBObject ent = id.GetObject(OpenMode.ForWrite);
                                    ent.Erase();
                                } //删除完毕
                                  //在CAD中重新插入该防火分区的系统图
                                Common.FASTools.Insert_Xiaofangblk2(db, list_XT[0], pt_Insert);

                            }
                        } // end of count==1 

                        //意外处理:SQL中查不到IDKEY
                        else if(list_XT.Count<1)
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("\n当前防火分区不存在:"+Id_Dianjing+"\n主键编号:" +IdKey);
                        } // end of conut<!
                        //当有多个分区时 
                        else
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("\nSQL中存在多条信息:" + Id_Dianjing + "\n主键编号:" + IdKey);
                        } // end of else count>!


                    } //end of for i
                    //提交事务处理
                    acTrans.Commit();

                } //end of if (acSSPrompt_pl.Status == PromptStatus.OK)
            } //end of using trans
            
            
        }

        #region 没啥用但又不能删的函数
        private void FasPage_Initialize(object sender, EventArgs e)
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

        private void uiSTest_Click(object sender, EventArgs e)
        {
            Common.MyTools.ConvertDataGridViewToClass<FASJiSuanshuExcel>(uiDG_PingMian);
        }

        private void uiLine1_Click(object sender, EventArgs e)
        {

        }
    }
}
