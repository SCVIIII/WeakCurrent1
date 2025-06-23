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
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


namespace WeakCurrent1.Common
{
    /// <summary>
    /// 存放数据库相关函数,暂未启用
    /// </summary>
    public static class SQLiteTools
    {
        //private SqlCommand m_Cmd = null;
        
        /// <summary>
        /// 插入新的一行数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="fasinfo"></param>
        public static void FasSQLInsertRow(FASJiSuanshuExcel fasinfo)
        {
            using(var conn = SQLiteConn.ConnSQLite())
            {
                //插入行对应的MySQL命令
                //231220新增线型光束烟感
                string sql_Insert =
                    "INSERT INTO fastable" +

                    " (Id_Dianjing , Floor1, Floor2, SI , ShouBao," +
                    "shengguang,juanliana,qiefei,dianti,gp," +
                    "zyfj,bfj,pyfj,xhsb,plb," +
                    "wyb,bec,bed,beeh,bech," +
                    "dycb,fa70,fa280,shuiliuzsq,xinhaofa," +
                    "shishibjf,liuliangkg,yalikg,wengan,xiaohuoshuan," +
                    "EXWenGan,XFDianHua,LouCengXSQ,gmt_create," + //gmt_change
                    "B,YanGan,PaiYanchuang," +
                    "HuiLu,DianWeiall,DianWeiliandong," +
                    "GuangBo,RD,RDK," +
                    "XXGSYanGan " +
                    ")" +

                    "VALUES " +
                    "(@string2,@string3,@string4,@stringSI,@string5," +
                    "@string6,@string7,@string8,@string9,@string10," +
                    "@string11,@string12,@string13,@string14,@string15," +
                    "@string16,@string17,@string18,@string19,@string20," +
                    "@string21,@string22,@string23,@string24,@string25," +
                    "@string26,@string27,@string28,@string29,@string30," +
                    "@string31,@string32,@string33,@string34," +  //@string35,
                    "@string36,@string37,@string38, " +
                    "@string39,@string40,@string41," +
                    "@string42,@string43,@string44," +
                    "@string61 " +
                    ")";


                //SQL语句中对应的变量
                var cmd_Insert = new SQLiteCommand(sql_Insert, conn);
                cmd_Insert.Parameters.AddWithValue("@string2", fasinfo.Id_Dianjing);
                cmd_Insert.Parameters.AddWithValue("@string3", fasinfo.Floor1);
                cmd_Insert.Parameters.AddWithValue("@string4", fasinfo.Floor2);
                cmd_Insert.Parameters.AddWithValue("@string5", fasinfo.ShouBao);
                cmd_Insert.Parameters.AddWithValue("@stringSI", fasinfo.SI);

                cmd_Insert.Parameters.AddWithValue("@string6", fasinfo.ShengGuang);
                cmd_Insert.Parameters.AddWithValue("@string7", fasinfo.JuanLianA);
                cmd_Insert.Parameters.AddWithValue("@string8", fasinfo.QieFei);
                cmd_Insert.Parameters.AddWithValue("@string9", fasinfo.DianTi);
                cmd_Insert.Parameters.AddWithValue("@string10", fasinfo.GP);

                cmd_Insert.Parameters.AddWithValue("@string11", fasinfo.ZYFJ);
                cmd_Insert.Parameters.AddWithValue("@string12", fasinfo.BFJ);
                cmd_Insert.Parameters.AddWithValue("@string13", fasinfo.PYFJ);
                cmd_Insert.Parameters.AddWithValue("@string14", fasinfo.XHSB);
                cmd_Insert.Parameters.AddWithValue("@string15", fasinfo.PLB);

                cmd_Insert.Parameters.AddWithValue("@string16", fasinfo.WYB);
                cmd_Insert.Parameters.AddWithValue("@string17", fasinfo.BEC);
                cmd_Insert.Parameters.AddWithValue("@string18", fasinfo.BED);
                cmd_Insert.Parameters.AddWithValue("@string19", fasinfo.BEEH);
                cmd_Insert.Parameters.AddWithValue("@string20", fasinfo.BECH);

                cmd_Insert.Parameters.AddWithValue("@string21", fasinfo.DYCB);
                cmd_Insert.Parameters.AddWithValue("@string22", fasinfo.Fa70);
                cmd_Insert.Parameters.AddWithValue("@string23", fasinfo.Fa280);
                cmd_Insert.Parameters.AddWithValue("@string24", fasinfo.ShuiLiuZSQ);
                cmd_Insert.Parameters.AddWithValue("@string25", fasinfo.XinHaofa);

                cmd_Insert.Parameters.AddWithValue("@string26", fasinfo.ShiShiBJF);
                cmd_Insert.Parameters.AddWithValue("@string27", fasinfo.LiuLiangKG);
                cmd_Insert.Parameters.AddWithValue("@string28", fasinfo.YaLiKG);
                cmd_Insert.Parameters.AddWithValue("@string29", fasinfo.WenGan);
                cmd_Insert.Parameters.AddWithValue("@string30", fasinfo.XiaoHuoShuan);
                cmd_Insert.Parameters.AddWithValue("@string31", fasinfo.EXWenGan);
                cmd_Insert.Parameters.AddWithValue("@string32", fasinfo.XFDianHua);
                cmd_Insert.Parameters.AddWithValue("@string33", fasinfo.LouCengXSQ);
                cmd_Insert.Parameters.AddWithValue("@string34", DateTime.Now.ToString("yy-MM-dd"));  //新建行时的时间戳

                cmd_Insert.Parameters.AddWithValue("@string36", fasinfo.B);
                cmd_Insert.Parameters.AddWithValue("@string37", fasinfo.YanGan);
                cmd_Insert.Parameters.AddWithValue("@string38", fasinfo.PaiYanchuang);
                if (true)
                {
                    //新增内容
                    //230409 SI

                    //新增结束

                    //230412新增:回路数量及点位数量
                    cmd_Insert.Parameters.AddWithValue("@string39", fasinfo.HuiLu);
                    cmd_Insert.Parameters.AddWithValue("@string40", fasinfo.DianWeiall);
                    cmd_Insert.Parameters.AddWithValue("@string41", fasinfo.DianWeiliandong);
                    cmd_Insert.Parameters.AddWithValue("@string42", fasinfo.GuangBo);
                    cmd_Insert.Parameters.AddWithValue("@string43", fasinfo.RD);
                    cmd_Insert.Parameters.AddWithValue("@string44", fasinfo.RDK);
                    cmd_Insert.Parameters.AddWithValue("@string61", fasinfo.XXGSYanGan);
                }


                cmd_Insert.ExecuteNonQuery();
            } //end of using


            
        } // end of SQLInsertRow


        /// <summary>
        /// 在MYSQL中新建行
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="nametable"></param>
        /// <param name="listInsert"></param>
        /// <returns></returns>
        public static bool FasSQLInsert(List<FASJiSuanshuExcel> listInsert)
        {

            using (var conn = SQLiteConn.ConnSQLite())
            {
                //当行内容有效时进行修改SQL
                if (listInsert.Count > 0)
                {
                    //对list3中的每行进行处理
                    //添加预留变量list4，用于后续对list3的checkbox筛选
                    List<FASJiSuanshuExcel> list4 = listInsert;

                    //连接MySQL
                    if (conn.State == System.Data.ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    //已存在的分区存入LIST_EXIST
                    List<string> AREA_EXIST = new List<string>();

                    //对各行依次对SQL进行检索,检索条件:电井编号
                    for (int i = 0; i < list4.Count; i++)
                    {

                        //获取电井/防火分区号
                        string Id_Dianjing = list4[i].Id_Dianjing;

                        //SQL筛选条件
                        string sql1 =
                        "SELECT IdKey " +
                        " FROM fastable" +
                        " WHERE Id_Dianjing =@Id_Dianjing";



                        //获取IdKey
                        var cmd = new SQLiteCommand(sql1, conn);
                        cmd.Parameters.AddWithValue("@Id_Dianjing", Id_Dianjing);
                        object result = cmd.ExecuteScalar();

                        //当前电井/防火分区是否存在
                        if (result == null) //此防火分区不存在
                        {
                            //插入第i行
                            FasSQLInsertRow(list4[i]);

                        } //end of if
                          //当防火分区重名时弹窗提示
                        else
                        {
                            //已存在的分区存入LIST_EXIST
                            AREA_EXIST.Add(Id_Dianjing);
                        }
                    } //end of for

                    //弹窗提示已存在的防火分区
                    if (AREA_EXIST.Count > 0)
                    {
                        string str = "";
                        for (int i = 0; i < AREA_EXIST.Count; i++)
                        {
                            str += AREA_EXIST[i] + "\n";
                        }
                        MessageBox.Show("以下防火分区已存在,请复核: \n" + str);
                    } //end of if (AREA_EXIST.Count > 0)

                    //此批次数据处理全部完成后关闭数据库连接
                    conn.Close();

                } // end of if (count>0)
                return true;
            }

        } //end of FasSQLInsert



        public static bool FasUpdate(List<FASJiSuanshuExcel> list3)
        {
            if (list3.Count > 0)
            {
                //对list3中的每行进行处理
                //添加预留变量list4，用于后续对list3的checkbox筛选
                List<FASJiSuanshuExcel> list4 = list3;
                using (var conn = SQLiteConn.ConnSQLite())
                {

                    //对各行依次对SQL进行检索,检索条件:电井编号
                    for (int i = 0; i < list4.Count; i++)
                    {

                        //获取电井/防火分区号
                        int IdKey = list4[i].IdKey;

                        //SQL筛选条件
                        string sql1 =
                        "SELECT COUNT(*) " +
                        " FROM fastable" +
                        " WHERE IdKey =@IdKey";
                        //" WHERE Id_Dianjing=" + Id_Dianjing;

                        //获取IdKey
                        var cmd = new SQLiteCommand(sql1, conn);
                        cmd.Parameters.AddWithValue("@IdKey", IdKey);
                        object result = cmd.ExecuteScalar();

                        //当前电井/防火分区是否存在
                        if (result != null) //此防火分区 信息存在时,进行修改
                        {
                            //更新行对应的MySQL命令
                            string sql_Update =
                                "UPDATE fastable" + 

                                " SET " +
                                " Id_Dianjing=@string2 , Floor1=@string3,  Floor2=@string4, SI=@stringSI, ShouBao=@string5, " +  //Id_Dianjing=@string2,
                                "ShengGuang=@string6, JuanLianA=@string7, QieFei=@string8, DianTi=@string9, GP=@string10, " +
                                "ZYFJ=@string11, BFJ=@string12, PYFJ=@string13, XHSB=@string14, PLB=@string15, " +
                                "WYB=@string16, BEC=@string17, BED=@string18, BEEH=@string19, BECH=@string20, " +
                                "DYCB=@string21, Fa70=@string22, Fa280=@string23, ShuiLiuZSQ=@string24, XinHaofa=@string25, " +
                                "ShiShiBJF=@string26, LiuLiangKG=@string27, YaLiKG=@string28, WenGan=@string29, XiaoHuoshuan=@string30, " +
                                "EXWenGan=@string31, XFDianHua=@string32, LouCengXSQ=@string33, gmt_change=@string35, " + //gmt_change
                                "B=@string36, YanGan=@string37 , PaiYanchuang=@string38 ," +
                                " HuiLu=@string39 , DianWeiall =@string40 , DianWeiliandong=@string41," +
                                "GuangBo=@string42,RD=@string43,RDK=@string44," +
                                "XXGSYanGan=@string61" +

                                " WHERE IdKey=@string1";

                            //SQL语句中对应的变量
                            var cmd_Update = new SQLiteCommand(sql_Update, conn);
                            cmd_Update.Parameters.AddWithValue("@string1", list4[i].IdKey);
                            cmd_Update.Parameters.AddWithValue("@string2", list4[i].Id_Dianjing);
                            cmd_Update.Parameters.AddWithValue("@string3", list4[i].Floor1);
                            cmd_Update.Parameters.AddWithValue("@string4", list4[i].Floor2);
                            cmd_Update.Parameters.AddWithValue("@string5", list4[i].ShouBao);

                            //新增
                            cmd_Update.Parameters.AddWithValue("@stringSI", list4[i].SI);
                            //新增结束

                            cmd_Update.Parameters.AddWithValue("@string6", list4[i].ShengGuang);
                            cmd_Update.Parameters.AddWithValue("@string7", list4[i].JuanLianA);
                            cmd_Update.Parameters.AddWithValue("@string8", list4[i].QieFei);
                            cmd_Update.Parameters.AddWithValue("@string9", list4[i].DianTi);
                            cmd_Update.Parameters.AddWithValue("@string10", list4[i].GP);

                            cmd_Update.Parameters.AddWithValue("@string11", list4[i].ZYFJ);
                            cmd_Update.Parameters.AddWithValue("@string12", list4[i].BFJ);
                            cmd_Update.Parameters.AddWithValue("@string13", list4[i].PYFJ);
                            cmd_Update.Parameters.AddWithValue("@string14", list4[i].XHSB);
                            cmd_Update.Parameters.AddWithValue("@string15", list4[i].PLB);

                            cmd_Update.Parameters.AddWithValue("@string16", list4[i].WYB);
                            cmd_Update.Parameters.AddWithValue("@string17", list4[i].BEC);
                            cmd_Update.Parameters.AddWithValue("@string18", list4[i].BED);
                            cmd_Update.Parameters.AddWithValue("@string19", list4[i].BEEH);
                            cmd_Update.Parameters.AddWithValue("@string20", list4[i].BECH);

                            cmd_Update.Parameters.AddWithValue("@string21", list4[i].DYCB);
                            cmd_Update.Parameters.AddWithValue("@string22", list4[i].Fa70);
                            cmd_Update.Parameters.AddWithValue("@string23", list4[i].Fa280);
                            cmd_Update.Parameters.AddWithValue("@string24", list4[i].ShuiLiuZSQ);
                            cmd_Update.Parameters.AddWithValue("@string25", list4[i].XinHaofa);

                            cmd_Update.Parameters.AddWithValue("@string26", list4[i].ShiShiBJF);
                            cmd_Update.Parameters.AddWithValue("@string27", list4[i].LiuLiangKG);
                            cmd_Update.Parameters.AddWithValue("@string28", list4[i].YaLiKG);
                            cmd_Update.Parameters.AddWithValue("@string29", list4[i].WenGan);
                            cmd_Update.Parameters.AddWithValue("@string30", list4[i].XiaoHuoShuan);

                            cmd_Update.Parameters.AddWithValue("@string31", list4[i].EXWenGan);
                            cmd_Update.Parameters.AddWithValue("@string32", list4[i].XFDianHua);
                            cmd_Update.Parameters.AddWithValue("@string33", list4[i].LouCengXSQ);
                            cmd_Update.Parameters.AddWithValue("@string35", DateTime.Now.ToString("yy-MM-dd"));  //修改行时的时间戳

                            cmd_Update.Parameters.AddWithValue("@string36", list4[i].B);
                            cmd_Update.Parameters.AddWithValue("@string37", list4[i].YanGan);
                            cmd_Update.Parameters.AddWithValue("@string38", list4[i].PaiYanchuang);

                            //230412新增回路数量及点位总数
                            cmd_Update.Parameters.AddWithValue("@string39", list4[i].HuiLu);
                            cmd_Update.Parameters.AddWithValue("@string40", list4[i].DianWeiall);
                            cmd_Update.Parameters.AddWithValue("@string41", list4[i].DianWeiliandong);
                            cmd_Update.Parameters.AddWithValue("@string42", list4[i].GuangBo);
                            cmd_Update.Parameters.AddWithValue("@string43", list4[i].RD);
                            cmd_Update.Parameters.AddWithValue("@string44", list4[i].RDK);
                            cmd_Update.Parameters.AddWithValue("@string61", list4[i].XXGSYanGan);
                            cmd_Update.ExecuteNonQuery();

                        }
                    }

                }

                



            }
            return true;

        }


        /// <summary>
        /// 根据IdKey,修改对应的一行数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="fasinfo"></param>
        public static void FasUpdateSQLRow(FASJiSuanshuExcel fasinfo)
        {
            using (var conn = SQLiteConn.ConnSQLite())
            {
                //更新行对应的MySQL命令
                string sql_Update =
                "UPDATE fastable"  +

                " SET " +
                "Id_Dianjing=@string2 , Floor1=@string3,  Floor2=@string4,  ShouBao=@string5, " +  //Id_Dianjing=@string2,
                "ShengGuang=@string6, JuanLianA=@string7, QieFei=@string8, DianTi=@string9, GP=@string10, " +
                "ZYFJ=@string11, BFJ=@string12, PYFJ=@string13, XHSB=@string14, PLB=@string15, " +
                "WYB=@string16, BEC=@string17, BED=@string18, BEEH=@string19, BECH=@string20, " +
                "DYCB=@string21, Fa70=@string22, Fa280=@string23, ShuiLiuZSQ=@string24, XinHaofa=@string25, " +
                "ShiShiBJF=@string26, LiuLiangKG=@string27, YaLiKG=@string28, WenGan=@string29, XiaoHuoshuan=@string30, " +
                "EXWenGan=@string31, XFDianHua=@string32, LouCengXSQ=@string33, gmt_change=@string35, " + //gmt_change
                "B=@string36, YanGan=@string37 , PaiYanchuang=@string38,XXGSYanGan=@string61" +

                " WHERE IdKey=@string1";

                //SQL语句中对应的变量
                var cmd_Update = new SQLiteCommand(sql_Update, conn);
                cmd_Update.Parameters.AddWithValue("@string1", fasinfo.IdKey);  //防火分区编号在修改行内容时为筛选项,不做修改
                cmd_Update.Parameters.AddWithValue("@string2", fasinfo.Id_Dianjing);  //防火分区编号在修改行内容时为筛选项,不做修改
                cmd_Update.Parameters.AddWithValue("@string3", fasinfo.Floor1);
                cmd_Update.Parameters.AddWithValue("@string4", fasinfo.Floor2);
                cmd_Update.Parameters.AddWithValue("@string5", fasinfo.ShouBao);

                cmd_Update.Parameters.AddWithValue("@string6", fasinfo.ShengGuang);
                cmd_Update.Parameters.AddWithValue("@string7", fasinfo.JuanLianA);
                cmd_Update.Parameters.AddWithValue("@string8", fasinfo.QieFei);
                cmd_Update.Parameters.AddWithValue("@string9", fasinfo.DianTi);
                cmd_Update.Parameters.AddWithValue("@string10", fasinfo.GP);

                cmd_Update.Parameters.AddWithValue("@string11", fasinfo.ZYFJ);
                cmd_Update.Parameters.AddWithValue("@string12", fasinfo.BFJ);
                cmd_Update.Parameters.AddWithValue("@string13", fasinfo.PYFJ);
                cmd_Update.Parameters.AddWithValue("@string14", fasinfo.XHSB);
                cmd_Update.Parameters.AddWithValue("@string15", fasinfo.PLB);

                cmd_Update.Parameters.AddWithValue("@string16", fasinfo.WYB);
                cmd_Update.Parameters.AddWithValue("@string17", fasinfo.BEC);
                cmd_Update.Parameters.AddWithValue("@string18", fasinfo.BED);
                cmd_Update.Parameters.AddWithValue("@string19", fasinfo.BEEH);
                cmd_Update.Parameters.AddWithValue("@string20", fasinfo.BECH);

                cmd_Update.Parameters.AddWithValue("@string21", fasinfo.DYCB);
                cmd_Update.Parameters.AddWithValue("@string22", fasinfo.Fa70);
                cmd_Update.Parameters.AddWithValue("@string23", fasinfo.Fa280);
                cmd_Update.Parameters.AddWithValue("@string24", fasinfo.ShuiLiuZSQ);
                cmd_Update.Parameters.AddWithValue("@string25", fasinfo.XinHaofa);

                cmd_Update.Parameters.AddWithValue("@string26", fasinfo.ShiShiBJF);
                cmd_Update.Parameters.AddWithValue("@string27", fasinfo.LiuLiangKG);
                cmd_Update.Parameters.AddWithValue("@string28", fasinfo.YaLiKG);
                cmd_Update.Parameters.AddWithValue("@string29", fasinfo.WenGan);
                cmd_Update.Parameters.AddWithValue("@string30", fasinfo.XiaoHuoShuan);

                cmd_Update.Parameters.AddWithValue("@string31", fasinfo.EXWenGan);
                cmd_Update.Parameters.AddWithValue("@string32", fasinfo.XFDianHua);
                cmd_Update.Parameters.AddWithValue("@string33", fasinfo.LouCengXSQ);
                cmd_Update.Parameters.AddWithValue("@string35", DateTime.Now.ToString("yy-MM-dd"));  //修改行时的时间戳

                cmd_Update.Parameters.AddWithValue("@string36", fasinfo.B);
                cmd_Update.Parameters.AddWithValue("@string37", fasinfo.YanGan);
                cmd_Update.Parameters.AddWithValue("@string38", fasinfo.PaiYanchuang);
                cmd_Update.Parameters.AddWithValue("@string61", fasinfo.XXGSYanGan);
                cmd_Update.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 对输入的list,按主键,修改所有信息
        /// 230320
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="nametable"></param>
        /// <param name="listUpdateToSQL"></param>
        public static void FasUpdateSQLByKey(List<FASJiSuanshuExcel> listUpdateToSQL)
        {
            using(var conn = SQLiteConn.ConnSQLite())
            {
                try
                {
                    //先判断输入的列表是否有效
                    if (listUpdateToSQL.Count > 0)
                    {

                        //连接MySQL
                        if (conn.State == System.Data.ConnectionState.Closed)
                        {
                            conn.Open();
                        }


                        //对各行依次对SQL进行检索,检索条件:电井编号
                        for (int i = 0; i < listUpdateToSQL.Count; i++)
                        {

                            //获取电井/防火分区号
                            int IdKey = listUpdateToSQL[i].IdKey;

                            //从SQL中筛选,当前主键是否存在
                            string sql1 = "SELECT COUNT(*)  FROM fastable  WHERE IdKey =@IdKey ";
                            var cmd = new SQLiteCommand(sql1, conn);
                            cmd.Parameters.AddWithValue("@IdKey", IdKey);
                            object result = cmd.ExecuteScalar();

                            //当IdKey存在时进行修改
                            if (result.ToString().ToInt() > 0)
                            {
                                //第i行的修改
                                FasUpdateSQLRow(listUpdateToSQL[i]);
                            }
                        } //end of for
                          //此批次数据处理全部完成后关闭数据库连接
                        conn.Close();
                    } // end of if
                } //end of try
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(ex.ToString());
                } //end of catch

            } //end of using



           
            

        }

        /// <summary>
        /// 查询防火分区编号对应的主键值
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="Id_Dianjing"></param>
        /// <param name="name_table"></param>
        /// <returns></returns>
        public static int FasSQLGetKey(string Id_Dianjing)
        {
            using (var conn = SQLiteConn.ConnSQLite())
            {
                int IdKey = 0;
                if (true) //预留If
                {

                    //SQL查询指令
                    string sql1 = "SELECT IdKey FROM fastable WHERE  Id_Dianjing=@string2";
                    using (var cmd = new SQLiteCommand(sql1, conn))
                    {
                        //查询当前防火分区对应的主键
                        cmd.Parameters.AddWithValue("@string2", Id_Dianjing);
                        object result = cmd.ExecuteScalar();
                        IdKey = result.ToString().ToInt();
                    }
                } // end of if true
                  //关闭数据库连接
                return IdKey;
            } //end of using

        } // end of GetKey

        /// <summary>
        /// 查询当前表中的所有IdKey,用于检验当前的IdKey是否失效
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<int> FasSQLGetIdKeys()
        {
            //创建返回值列表
            List<int> listKey = new List<int>();
            // 链接sqlite数据库
            using (var conn = new SQLiteConnection(SQLiteConn.ConnSQLite()))
            {
                //SQL查询指令
                string sql1 = "SELECT IdKey FROM fastable ";
                using (var cmd = new SQLiteCommand(sql1, conn))
                {
                    //查询当前防火分区对应的主键
                    var rdr = cmd.ExecuteReader();
                    //将结果rdr转换为list
                    while (rdr.Read())
                    {
                        //将查询结果存入listKey
                        int IdKey = rdr[0].ToString().ToInt();
                        listKey.Add(IdKey);
                    }
                    rdr.Close();
                }
                // 关闭数据库连接
                conn.Close();
            } //end of using
            //返回值
            return listKey;
        }


    } //end of class

} //end of namespace
