using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Xml;

namespace WeakCurrent1.Common
{
    public static class AnFangTools
    {


        //安防计数函数

        //计数函数一
        //修改日期230314：增加int时间戳、int电动阀的手动按钮
        public static int[] AnFang_jishu(int[] num_array, ObjectId[] id_ss)
        {


            
            //块名初始化
            //球机
            string[] Blk_QiuJi = new string[] { "$EQUIP$00002353" };
            //块统计
            num_array[5] = Jishu_BlkNum(Blk_QiuJi, id_ss);
            //枪机
            string[] Blk_QiangJi = new string[] { "$EQUIP$00002965" };
            num_array[6] = Jishu_BlkNum(Blk_QiangJi, id_ss);
            //人脸识别(预留)
            string[] Blk_Renlian = new string[] { "" };
            num_array[7] = Jishu_BlkNum(Blk_Renlian, id_ss);
            //室外预留
            string[] Blk_Shiwai = new string[] { "" };
            num_array[8] = Jishu_BlkNum(Blk_Shiwai, id_ss);
            //监控数量总和
            num_array[9] = num_array[5] + num_array[6] + num_array[7] + num_array[8];
            //信息发布
            string[] Blk_XinXifabu = new string[] { "$EQUIP$00002670" };
            num_array[10] = Jishu_BlkNum(Blk_XinXifabu, id_ss);
            //能耗监测网关
            string[] Blk_NengHao = new string[] { "$EQUIP_U$00000254(SZM-PC)" };
            num_array[11] = Jishu_BlkNum(Blk_NengHao, id_ss);
            //门禁
            string[] Blk_MenJin = new string[] { "$EQUIP_U$00000245(SZM-PC)" };
            num_array[13] = Jishu_BlkNum(Blk_MenJin, id_ss);
            //四路门禁控制器
            //string[] Blk_MenJinkongzhi = new string[] { "" };
            num_array[12] =  (int)(Math.Ceiling((decimal)(num_array[13] * 1.10 / 4.00)));

            //巡更打卡点
            string[] Blk_XunGeng = new string[] { "$EQUIP$00002618" };
            num_array[14] = Jishu_BlkNum(Blk_XunGeng, id_ss);
            //无障碍呼叫
            string[] Blk_WuZhangai = new string[] { "$EQUIP$00002479" };
            num_array[15] = Jishu_BlkNum(Blk_WuZhangai, id_ss);
            //入侵探测器
            string[] Blk_RuQintance = new string[] { "" };
            num_array[16] = Jishu_BlkNum(Blk_RuQintance, id_ss);
            //2290
            string[] Blk_DDC = new string[] { "$EQUIP$00002290" };
            num_array[19] = Jishu_BlkNum(Blk_DDC, id_ss);
            //AHD数量
            string[] Blk_AHD = new string[] { "$EQUIP_U$00000175(SZM-PC)" };
            num_array[20] = Jishu_BlkNum(Blk_AHD, id_ss);
            //无线AP数量
            string[] Blk_AP = new string[] { "$EQUIP$00003217" };
            num_array[21] = Jishu_BlkNum(Blk_AP, id_ss);
            

            //返回各种块的数量num_array
            return num_array;


        }

        //计数函数二
        /// <summary>
        /// 从ObjecId中确定指定块名的数量
        /// </summary>
        /// <param name="blk_name"></param>
        /// <param name="id_ss"></param>
        /// <returns></returns>
        private static int Jishu_BlkNum(string[] blk_name, ObjectId[] id_ss)
        {

            IEnumerable<ObjectId> blk_ids = from c in id_ss
                                            where blk_name.Contains(c.GetBlockName().ToUpper())
                                            select c;
            return blk_ids.Count();
        }
        //安防计数结束



        //计数结果转安防class
        public static AnFangClass JishuToAnFang(int[] num_blks)
        {
            //返回值 info_RD
            //新建类 AnFangClass
            AnFangClass info_RD = new AnFangClass
            {
                JianKong_qiuji = num_blks[5],
                JianKong_qiangji = num_blks[6],
                JianKong_renlian = num_blks[7],
                JianKong_shiwai = num_blks[8],
                JianKong = num_blks[9],
                XinXifabu = num_blks[10],
                NengHao = num_blks[11],
                MenJinkongzhi = num_blks[12],
                MenJin = num_blks[13],
                XunGeng = num_blks[14],
                WuZhangaihujiao = num_blks[15],
                RuQintance = num_blks[16],
                DDC = num_blks[19],
                AHD = num_blks[20],
                AP = num_blks[21],
                FenGuangqi_216 = num_blks[22],
                ONU_GC24POE = num_blks[23],

            };
            //计算弱电点位占用的端口总和
            info_RD.DianWei = info_RD.JianKong_qiuji + info_RD.JianKong_qiangji + info_RD.JianKong_renlian 
                + info_RD.JianKong_shiwai + info_RD.XinXifabu + info_RD.NengHao + info_RD.MenJinkongzhi;
            //根据端口数量计算POE交换机的数量，向上取整
            info_RD.ONU_POE24 = (int)(Math.Ceiling((decimal)(info_RD.JianKong * 1.20 / 24.00)));
            //计算普通交换机的数量 = (点位-POE点位)/24，向上取整
            info_RD.ONU_24 = (int)(Math.Ceiling((decimal)((info_RD.DianWei - info_RD.JianKong) * 1.20 / 24.00)));
            //计算综合布线系统24口POE交换机数量
            info_RD.ONU_GC24POE= (int)(Math.Ceiling((decimal)(info_RD.AP / 20.00)));
            //计算综合布线系统2:16分光器数量
            // 2024年4月29日 分光比由2:16改为2: 8; 数量计算考虑POE交换机
            info_RD.FenGuangqi_216 = (int)(Math.Ceiling((decimal)( (info_RD.AHD + info_RD.ONU_GC24POE) / 7.00)));
            return info_RD;
        } // end of JishuToFAS函数
        //计数结果转安防class结束

        //2024年4月29日 分光比由2:16改为2:8
        //两个防火分区的安防点相加
        public static AnFangClass AnFangClassXiangjia(AnFangClass fas1, AnFangClass fas2)
        {
            //新建Class,将两个值合并
            AnFangClass info_RD = new AnFangClass()
            {
                IdKey = fas1.IdKey,
                Id_Dianjing = fas1.Id_Dianjing,
                Floor1 = fas1.Floor1,
                Floor2 = fas1.Floor2,
                //新增内容添加至此处
                JianKong_qiuji = fas1.JianKong_qiuji + fas2.JianKong_qiuji,
                JianKong_qiangji = fas1.JianKong_qiangji + fas2.JianKong_qiangji,
                JianKong_renlian = fas1.JianKong_renlian + fas2.JianKong_renlian,
                JianKong_shiwai = fas1.JianKong_shiwai + fas2.JianKong_shiwai,
                JianKong = fas1.JianKong + fas2.JianKong,
                NengHao = fas1.NengHao + fas2.NengHao,
                MenJin = fas1.MenJin + fas2.MenJin,
                XunGeng = fas1.XunGeng + fas2.XunGeng,
                WuZhangaihujiao = fas1.WuZhangaihujiao + fas2.WuZhangaihujiao,
                RuQintance = fas1.RuQintance + fas2.RuQintance,
                DDC = fas1.DDC + fas2.DDC
            };
            //计算四路门禁控制器数量
            info_RD.MenJinkongzhi= (int)(Math.Ceiling((decimal)(info_RD.MenJin * 1.10 / 4.00)));
            //计算弱电点位占用的交换机端口数量
            info_RD.DianWei = info_RD.JianKong_qiuji + info_RD.JianKong_qiangji + info_RD.JianKong_renlian
                + info_RD.JianKong_shiwai + info_RD.XinXifabu + info_RD.NengHao + info_RD.MenJinkongzhi;
            //根据端口数量计算POE交换机的数量，向上取整
            info_RD.ONU_POE24 = (int)(Math.Ceiling((decimal)(info_RD.JianKong * 1.20 / 24.00)));
            //计算普通交换机的数量 = (点位-POE点位)/24，向上取整
            info_RD.ONU_24 = (int)(Math.Ceiling((decimal)((info_RD.DianWei - info_RD.JianKong) * 1.20 / 24.00)));
            //计算综合布线系统24口POE交换机数量
            info_RD.ONU_GC24POE = (int)(Math.Ceiling((decimal)(info_RD.AP / 20.00)));
            //计算综合布线系统2:16分光器数量
            // 2024年4月29日 分光比由2:16改为2: 8; 数量计算考虑POE交换机
            info_RD.FenGuangqi_216 = (int)(Math.Ceiling((decimal)((info_RD.AHD + info_RD.ONU_GC24POE) / 7.00)));
            return info_RD;
        } //end of FasClassXiangjia
          //相加结束

        //选中的安防表格转list
        public static List<AnFangClass> UIDGSecToSheBeiWangList(UIDataGridView uiDG_All)
        {
            //新建类
            List<AnFangClass> list_Xuanzhong = new List<AnFangClass>();
            for (int i = 0; i < uiDG_All.RowCount; i++)
            {

                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)uiDG_All.Rows[i].Cells[0];
                Boolean flag = Convert.ToBoolean(checkCell.Value);
                if (flag == true)     //查找被选择的数据行  
                {
                    //新建FAS类,并将数据逐格填入
                    AnFangClass anfang = new AnFangClass
                    {

                        IdKey = uiDG_All.Rows[i].Cells["IdKey"].Value.ToString().ToInt(),  //获取IdKey
                        Id_Dianjing = uiDG_All.Rows[i].Cells["Id_Dianjing"].Value.ToString(),
                        Floor1 = uiDG_All.Rows[i].Cells["Floor1"].Value.ToString(),
                        Floor2 = uiDG_All.Rows[i].Cells["Floor2"].Value.ToString().ToInt(),
                        JianKong_qiuji = uiDG_All.Rows[i].Cells["JianKong_qiuji"].Value.ToString().ToInt(),
                        JianKong_qiangji = uiDG_All.Rows[i].Cells["JianKong_qiangji"].Value.ToString().ToInt(),
                        JianKong_renlian = uiDG_All.Rows[i].Cells["JianKong_renlian"].Value.ToString().ToInt(),
                        JianKong_shiwai = uiDG_All.Rows[i].Cells["JianKong_shiwai"].Value.ToString().ToInt(),
                        XinXifabu = uiDG_All.Rows[i].Cells["XinXifabu"].Value.ToString().ToInt(),

                        NengHao = uiDG_All.Rows[i].Cells["NengHao"].Value.ToString().ToInt(),
                        MenJin = uiDG_All.Rows[i].Cells["MenJin"].Value.ToString().ToInt(),
                        MenJinkongzhi= uiDG_All.Rows[i].Cells["MenJinkongzhi"].Value.ToString().ToInt(),

                        XunGeng = uiDG_All.Rows[i].Cells["XunGeng"].Value.ToString().ToInt(),
                        WuZhangaihujiao = uiDG_All.Rows[i].Cells["WuZhangaihujiao"].Value.ToString().ToInt(),
                        RuQintance = uiDG_All.Rows[i].Cells["RuQintance"].Value.ToString().ToInt(),
                        DDC = uiDG_All.Rows[i].Cells["DDC"].Value.ToString().ToInt(),
                        gmt_create = uiDG_All.Rows[i].Cells["gmt_create"].Value.ToString(),
                        gmt_change = uiDG_All.Rows[i].Cells["gmt_change"].Value.ToString(),

                        DianWei= uiDG_All.Rows[i].Cells["DianWei"].Value.ToString().ToInt(),
                        ONU_24 = uiDG_All.Rows[i].Cells["ONU_24"].Value.ToString().ToInt(),
                        ONU_POE24 = uiDG_All.Rows[i].Cells["ONU_POE24"].Value.ToString().ToInt(),

                        AHD = uiDG_All.Rows[i].Cells["AHD"].Value.ToString().ToInt(),
                        AP = uiDG_All.Rows[i].Cells["AP"].Value.ToString().ToInt(),
                        ONU_GC24POE = uiDG_All.Rows[i].Cells["ONU_GC24POE"].Value.ToString().ToInt(),
                        FenGuangqi_216 = uiDG_All.Rows[i].Cells["FenGuangqi_216"].Value.ToString().ToInt()

                    }; //end of fas
                    anfang.JianKong = anfang.JianKong_qiangji + anfang.JianKong_qiuji + anfang.JianKong_renlian + anfang.JianKong_shiwai;
                    list_Xuanzhong.Add(anfang);
                }
            }

            //将list数据进行重排
            List<AnFangClass> listn_Paixu = list_Xuanzhong.OrderBy(d => d.Floor2).ThenBy(d => Common.MyTools.GetFenquHao(d.Id_Dianjing)).ToList();

            //返回值
            return listn_Paixu;

        }
        //转list结束

        #region 安防系统中的SQL相关模块
        /// <summary>
        /// 查询当前表中的所有IdKey,用于检验当前的IdKey是否失效
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public static List<int> AnFangSQLGetIdKeys()
        {
            //创建返回值列表
            List<int> listKey = new List<int>();
            
            //主函数
            using(var conn=new SQLiteConnection(SQLiteConn.ConnSQLite()))
            {
                //SQL查询指令
                string sql1 = "SELECT IdKey FROM anfangtable " ;
                //查询当前防火分区对应的主键
                var cmd = new SQLiteCommand(sql1, conn);
                var rdr = cmd.ExecuteReader();
                //将结果rdr转换为list
                while (rdr.Read())
                {
                    //将查询结果存入listKey
                    int IdKey = rdr[0].ToString().ToInt();
                    listKey.Add(IdKey);
                }
                rdr.Close();
                return listKey;
            } // end of using
            {
                

            } // end of try

        } // end of GetIdKeys


        //新建行信息,并插入数据库
        public static bool AnFangAddToSQL(List<AnFangClass> listInsert)
        {

            //当行内容有效时进行修改SQL
            if (listInsert.Count > 0)
            {
                //对list3中的每行进行处理
                //添加预留变量list4，用于后续对list3的checkbox筛选
                List<AnFangClass> list4 = listInsert;
                //表名
                string table = "anfangtable";

                using (var conn = new SQLiteConnection(SQLiteConn.ConnSQLite()))
                {
                    // 对各行依次对SQL进行检索,检索条件: 电井编号
                    for (int i = 0; i < list4.Count; i++)
                    {

                        //获取电井/防火分区号
                        string Id_Dianjing = list4[i].Id_Dianjing;

                        //SQL筛选条件
                        string sql1 =
                        "SELECT IdKey " +
                        " FROM " + table +
                        " WHERE Id_Dianjing =@Id_Dianjing";
                        //" WHERE Id_Dianjing=" + Id_Dianjing;

                        //获取IdKey
                        var cmd = new SQLiteCommand(sql1, conn);
                        cmd.Parameters.AddWithValue("@Id_Dianjing", Id_Dianjing);
                        object result = cmd.ExecuteScalar();

                        //当前电井/防火分区是否存在
                        if (result == null) //此防火分区不存在
                        {
                            //插入第i行
                            AnFangSQLAddRow(table, list4[i]);

                        } //end of if
                          //当防火分区重名时弹窗提示
                        else
                        {
                            MessageBox.Show("此防火分区已存在,请复核: " + Id_Dianjing);
                        }
                    } //end of for
                }
            } // end of if (count>0)

            return true;
        } //end of Insert
        //新行插入数据库结束

        /// <summary>
        /// 功能子函数1/3:在SQL中插入行
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="table"></param>
        /// <param name="fasinfo"></param>
        private static void AnFangSQLAddRow(string table, AnFangClass fasinfo)
        {
            //在SQL中插入行
            using (var conn = new SQLiteConnection(SQLiteConn.ConnSQLite()))
            {
                //插入行对应的MySQL命令
                string sql_Insert =
                    "INSERT INTO " + table +

                    "( Id_Dianjing , Floor1, Floor2, " +
                    "JianKong_qiuji,JianKong_qiangji,JianKong_renlian,JianKong_shiwai,JianKong,XinXifabu," +
                    "NengHao,MenJinkongzhi,MenJin,XunGeng,WuZhangaihujiao," +
                    "RuQintance,gmt_create,DDC, " +
                    "YuLiu1,YuLiu2,YuLiu3,YuLiu4,YuLiu5,YuLiu6, " +
                    "DianWei , ONU_24 ,ONU_POE24," +
                    "ONU_GC24POE, FenGuangqi_216, AHD, AP ) " +

                    "VALUES " +
                    "(@string2,@string3,@string4," +
                    "@string5,@string6,@string7,@string8,@string9,@string10," +
                    "@string11,@string12,@string13,@string14,@string15," +
                    "@string16,@string17,@string19," +
                    "@string20," +
                    "@string21,@string22,@string23,@string24,@string25, " +
                    "@string26,@string27,@string28, @string29, @string30, @string31, @string32)";

                //SQL语句中对应的变量
                var cmd_Insert = new SQLiteCommand(sql_Insert, conn);
                cmd_Insert.Parameters.AddWithValue("@string2", fasinfo.Id_Dianjing);
                cmd_Insert.Parameters.AddWithValue("@string3", fasinfo.Floor1);
                cmd_Insert.Parameters.AddWithValue("@string4", fasinfo.Floor2);

                //5~10
                cmd_Insert.Parameters.AddWithValue("@string5", fasinfo.JianKong_qiuji);
                cmd_Insert.Parameters.AddWithValue("@string6", fasinfo.JianKong_qiangji);
                cmd_Insert.Parameters.AddWithValue("@string7", fasinfo.JianKong_renlian);
                cmd_Insert.Parameters.AddWithValue("@string8", fasinfo.JianKong_shiwai);
                cmd_Insert.Parameters.AddWithValue("@string9", fasinfo.JianKong);
                cmd_Insert.Parameters.AddWithValue("@string10", fasinfo.XinXifabu);
                //11~15
                cmd_Insert.Parameters.AddWithValue("@string11", fasinfo.NengHao);
                cmd_Insert.Parameters.AddWithValue("@string12", fasinfo.MenJinkongzhi);
                cmd_Insert.Parameters.AddWithValue("@string13", fasinfo.MenJin);
                cmd_Insert.Parameters.AddWithValue("@string14", fasinfo.XunGeng);
                cmd_Insert.Parameters.AddWithValue("@string15", fasinfo.WuZhangaihujiao);

                //16~19
                cmd_Insert.Parameters.AddWithValue("@string16", fasinfo.RuQintance);
                cmd_Insert.Parameters.AddWithValue("@string17", DateTime.Now.ToString("yy-MM-dd"));  //新建行时的日期
                cmd_Insert.Parameters.AddWithValue("@string19", fasinfo.DDC);

                //20~25
                cmd_Insert.Parameters.AddWithValue("@string20", fasinfo.YuLiu1);
                cmd_Insert.Parameters.AddWithValue("@string21", fasinfo.YuLiu2);
                cmd_Insert.Parameters.AddWithValue("@string22", fasinfo.YuLiu3);
                cmd_Insert.Parameters.AddWithValue("@string23", fasinfo.YuLiu4);
                cmd_Insert.Parameters.AddWithValue("@string24", fasinfo.YuLiu5);
                cmd_Insert.Parameters.AddWithValue("@string25", fasinfo.YuLiu6);

                //26~28
                cmd_Insert.Parameters.AddWithValue("@string26", fasinfo.DianWei);
                cmd_Insert.Parameters.AddWithValue("@string27", fasinfo.ONU_24);
                cmd_Insert.Parameters.AddWithValue("@string28", fasinfo.ONU_POE24);

                // 新添加的列
                cmd_Insert.Parameters.AddWithValue("@string29", fasinfo.ONU_GC24POE);
                cmd_Insert.Parameters.AddWithValue("@string30", fasinfo.FenGuangqi_216);
                cmd_Insert.Parameters.AddWithValue("@string31", fasinfo.AHD);
                cmd_Insert.Parameters.AddWithValue("@string32", fasinfo.AP);
                cmd_Insert.ExecuteNonQuery();

            } // end of using

            
        } // end of SQLInsertRow
        //插入行结束

        //子函数:在平面插入IDKEY
        /// <summary>
        /// 子函数2/3:在平面块中插入IDKEY
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="db"></param>
        /// <param name="ed"></param>
        /// <param name="database"></param>
        /// <param name="listInsert"></param>
        public static void AnFangUpdateBlkKeys(Database db, Editor ed, List<AnFangClass> listInsert)
        {
            //主函数
            try
            {
                //引入trans
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    //在CAD中自动选取防火分区信息的块
                    //图框对应的块名存放于此
                    List<string> listRDblk1 = new List<string>() { "RD-防火分区信息1" };

                    //筛选条件:块名
                    string rdblkNames1 = ARXTools.GetAnonymousBlk(db, listRDblk1);
                    //防火分区框对应属性块的筛选条件
                    TypedValue[] acTypValAr_info = new TypedValue[3];
                    acTypValAr_info.SetValue(new TypedValue((int)DxfCode.BlockName, rdblkNames1), 0);  //块名
                    acTypValAr_info.SetValue(new TypedValue((int)DxfCode.LayerName, "E-FAS筛选02"), 1);  //图层
                    acTypValAr_info.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 2);   //图元类型(INSERT代表块参照)
                    SelectionFilter acSelFtr_info = new SelectionFilter(acTypValAr_info);
                    //选择集结果
                    PromptSelectionResult acSSPrompt_info = ed.SelectAll(acSelFtr_info);
                    //将选择集结果转换为ObjectId[]存入id_Info
                    ObjectId[] id_Info = acSSPrompt_info.Value.GetObjectIds();

                    //异常的块信息列表
                    //重复的块(可能是错误同名,也可能是由多个子分区组成)
                    List<AreaInfo> AREA_EXIST_LIST = new List<AreaInfo>();
                    //未找到的块(可能是由于块名错误,也可能是由于块不存在)
                    List<AreaInfo> AREA_NOTFOUND_LIST = new List<AreaInfo>();

                    //平面选择完成后,根据返回值信息,为防火分区的块添加主键
                    if ((acSSPrompt_info.Status == PromptStatus.OK) && (id_Info.Count() > 0))
                    {

                        for (int i = 0; i < listInsert.Count; i++)
                        {
                            //查询主键
                            string Id_Dianjing = listInsert[i].Id_Dianjing;
                            string FLoor1 = listInsert[i].Floor1;
                            int IdKey = AnFangQueryKey(Id_Dianjing);

                            //筛选出与电井编号相同的块
                            List<ObjectId> listObjIds = (from d in id_Info
                                                         where d.GetAttributeInBlockReference("电井/防火分区") == Id_Dianjing
                                                         select d).ToList();
                            //当主键有效且属性块唯一时进行修改
                            if ((listObjIds.Count == 1))
                            {
                                //设置主键信息
                                Dictionary<string, string> atts = new Dictionary<string, string>
                                {
                                    { "KEY", IdKey.ToString() },
                                    { "楼层", FLoor1 }
                                };
                                //listObjIds应只有一个元素
                                listObjIds[0].UpdateAttributesInBlock(atts);

                            } // end of if (count>!)
                            //异常处理: 同名(Count>0)
                            else if (listObjIds.Count > 1)
                            {


                                //设置主键信息
                                Dictionary<string, string> atts = new Dictionary<string, string>();
                                atts.Add("KEY", IdKey.ToString());
                                //listObjIds有多个元素时,均修改
                                foreach (var objid in listObjIds)
                                {
                                    objid.UpdateAttributesInBlock(atts);
                                }// end of foreach

                                //将重复的块加入AREA_EXSIT_LIST
                                AreaInfo AREAINFO = new AreaInfo()
                                {
                                    Id_Dianjing = Id_Dianjing,
                                    NUM_EXIST = listObjIds.Count
                                };
                                AREA_EXIST_LIST.Add(AREAINFO);
                            }
                            //异常处理: Count为0
                            else
                            {
                                //将未找到的块加入AREA_NOTFOUND_LIST
                                AreaInfo AREAINFO = new AreaInfo()
                                {
                                    Id_Dianjing = Id_Dianjing,
                                    NUM_EXIST = listObjIds.Count
                                };
                                AREA_NOTFOUND_LIST.Add(AREAINFO);
                            }

                        } //end of for

                        //弹窗提示AREA_EXSIT_LIST
                        string STR_EXIST = "以下防火分区存在多个块,请检查:\n";
                        if (AREA_EXIST_LIST.Count > 0)
                        {
                            
                            foreach (var item in AREA_EXIST_LIST)
                            {
                                STR_EXIST += item.Id_Dianjing + " " + item.NUM_EXIST + "个\n";
                            }
                        }
                        //弹窗提示AREA_NOTFOUND_LIST
                        string STR_NOTFOUND = "以下防火分区未找到块,请检查:\n";
                        if (AREA_NOTFOUND_LIST.Count > 0)
                        {
                            
                            foreach (var item in AREA_NOTFOUND_LIST)
                            {
                                STR_NOTFOUND += item.Id_Dianjing + " " + item.NUM_EXIST + "个\n";
                            }
                        }
                        //弹窗提示AREA_EXSIT_LIST及AREA_NOTFOUND_LIST
                        if(AREA_EXIST_LIST.Count+ AREA_NOTFOUND_LIST.Count > 0)
                        {
                            MessageBox.Show(STR_EXIST + "\n" +STR_NOTFOUND);
                        }

                        trans.Commit();
                    } //end of if(acSSPrompt_info.Status == PromptStatus.OK)
                    else
                    {
                        MessageBox.Show("未检索到防火分区的对应块 ");
                        trans.Abort();
                    }
                    //结束前校验trans


                } //end of using
            } //end of try

            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(ex.ToString());
            } //end of catch

        }

        //插入KEY结束

        /// <summary>
        /// 子函数3/3:查询防火分区编号对应的主键值
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="Id_Dianjing"></param>
        /// <param name="name_table"></param>
        /// <returns></returns>
        private static int AnFangQueryKey(string Id_Dianjing)
        {
            int IdKey = 0;

            using (var conn = new SQLiteConnection(SQLiteConn.ConnSQLite()))
            {

                if (true) //预留If
                {

                    //SQL查询指令
                    string sql1 = "SELECT IdKey FROM anfangtable WHERE  Id_Dianjing=@string2";
                    //查询当前防火分区对应的主键
                    var cmd = new SQLiteCommand(sql1, conn);
                    cmd.Parameters.AddWithValue("@string2", Id_Dianjing);
                    object result = cmd.ExecuteScalar();
                    IdKey = result.ToString().ToInt();

                } //end of if true
                return IdKey;

            } // end of using


        } // end of GetKey


        //修改SQL行信息

        public static bool AnFangUpdateSQL(List<AnFangClass> list3)
        {
            if (list3.Count > 0)
            {
                //对list3中的每行进行处理
                //添加预留变量list4，用于后续对list3的checkbox筛选
                List<AnFangClass> list4 = list3;
                

                using(var conn= new SQLiteConnection(SQLiteConn.ConnSQLite()))
                {
                    //对各行依次对SQL进行检索,检索条件:电井编号
                    for (int i = 0; i < list4.Count; i++)
                    {

                        //获取电井/防火分区号
                        int IdKey = list4[i].IdKey;

                        //SQL筛选条件
                        string sql1 =
                        "SELECT COUNT(*) " +
                        " FROM anfangtable" +
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
                                "UPDATE anfangtable"  +

                                " SET " +
                                " Id_Dianjing=@string2 , Floor1=@string3,  Floor2=@string4, " +

                                "JianKong_qiuji=@string5, " +  //Id_Dianjing=@string2,
                                "JianKong_qiangji=@string6, JianKong_renlian=@string7, JianKong_shiwai=@string8, JianKong=@string9, XinXifabu=@string10, " +
                                "NengHao=@string11, MenJinkongzhi=@string12, MenJin=@string13, XunGeng=@string14, WuZhangaihujiao=@string15, " +
                                "RuQintance=@string16, DDC=@string19, gmt_change=@string18, " +


                                " YuLiu1=@string20,YuLiu2=@string21,YuLiu3=@string22,YuLiu4=@string23,YuLiu5=@string24,YuLiu6=@string25, " +
                                " DianWei=@string26,ONU_24=@string27,ONU_POE24=@string28, ONU_GC24POE=@string29, FenGuangqi_216=@string30, AHD=@string31, AP=@string32" +
                                " WHERE IdKey=@string1";
                            //

                            //SQL语句中对应的变量
                            var cmd_Update = new SQLiteCommand(sql_Update, conn);
                            cmd_Update.Parameters.AddWithValue("@string1", list4[i].IdKey);
                            cmd_Update.Parameters.AddWithValue("@string2", list4[i].Id_Dianjing);
                            cmd_Update.Parameters.AddWithValue("@string3", list4[i].Floor1);
                            cmd_Update.Parameters.AddWithValue("@string4", list4[i].Floor2);
                            cmd_Update.Parameters.AddWithValue("@string5", list4[i].JianKong_qiuji);


                            cmd_Update.Parameters.AddWithValue("@string6", list4[i].JianKong_qiangji);
                            cmd_Update.Parameters.AddWithValue("@string7", list4[i].JianKong_renlian);
                            cmd_Update.Parameters.AddWithValue("@string8", list4[i].JianKong_shiwai);
                            cmd_Update.Parameters.AddWithValue("@string9", list4[i].JianKong);
                            cmd_Update.Parameters.AddWithValue("@string10", list4[i].XinXifabu);

                            cmd_Update.Parameters.AddWithValue("@string11", list4[i].NengHao);
                            cmd_Update.Parameters.AddWithValue("@string12", list4[i].MenJinkongzhi);
                            cmd_Update.Parameters.AddWithValue("@string13", list4[i].MenJin);
                            cmd_Update.Parameters.AddWithValue("@string14", list4[i].XunGeng);
                            cmd_Update.Parameters.AddWithValue("@string15", list4[i].WuZhangaihujiao);

                            cmd_Update.Parameters.AddWithValue("@string16", list4[i].RuQintance);
                            cmd_Update.Parameters.AddWithValue("@string19", list4[i].DDC);
                            cmd_Update.Parameters.AddWithValue("@string18", DateTime.Now.ToString("yy-MM-dd"));

                            cmd_Update.Parameters.AddWithValue("@string20", list4[i].YuLiu1);

                            cmd_Update.Parameters.AddWithValue("@string21", list4[i].YuLiu2);
                            cmd_Update.Parameters.AddWithValue("@string22", list4[i].YuLiu3);
                            cmd_Update.Parameters.AddWithValue("@string23", list4[i].YuLiu4);
                            cmd_Update.Parameters.AddWithValue("@string24", list4[i].YuLiu5);
                            cmd_Update.Parameters.AddWithValue("@string25", list4[i].YuLiu6);

                            //26~28
                            cmd_Update.Parameters.AddWithValue("@string26", list4[i].DianWei);
                            cmd_Update.Parameters.AddWithValue("@string27", list4[i].ONU_24);
                            cmd_Update.Parameters.AddWithValue("@string28", list4[i].ONU_POE24);

                            //230506新添加的列
                            cmd_Update.Parameters.AddWithValue("@string29", list4[i].ONU_GC24POE);
                            cmd_Update.Parameters.AddWithValue("@string30", list4[i].FenGuangqi_216);
                            cmd_Update.Parameters.AddWithValue("@string31", list4[i].AHD);
                            cmd_Update.Parameters.AddWithValue("@string32", list4[i].AP);
                            cmd_Update.ExecuteNonQuery();

                        }
                    }
                } // end of using
            }
            return true;

        }
        //SQL行信息修改结束


        #endregion

        #region 安防相关的插入块模块
        //插入块1
        public static void Insert_AnFangblk1(Database db, AnFangClass row_Insert, Point3d ptStart,string purpose)
        {
            //
            //int a = ptRowStart.Length;

            ObjectId spaceid = db.CurrentSpaceId;

            //Point3d ptStart = Point3d.Origin;

            //以下信息初始化:
            //插入比例、图层、块间距
            Scale3d InsertScale = new Scale3d(1);
            string sInsertLayer = "E-火灾报警系统图专用图层";

            //
            ObjectId layerid = ARXTools.AddLayer(db, sInsertLayer);
            if (ARXTools.SetLayerColor(db, sInsertLayer, 3))
            {
                #region 主函数


                //插入安防系统图,设置摄像头、巡更打卡点数量
                if (purpose == "视频安防")
                {
                    Dictionary<string, string> atts = new Dictionary<string, string>();
                    atts.Add("电井编号", row_Insert.Id_Dianjing.ToString());
                    atts.Add("IDKEY", row_Insert.IdKey.ToString());
                    atts.Add("枪机", row_Insert.JianKong_qiangji.ToString());
                    atts.Add("球机", row_Insert.JianKong_qiuji.ToString());
                    atts.Add("巡更", row_Insert.XunGeng.ToString());
                    spaceid.InsertBlockReference(sInsertLayer, "RDXT-安防系统1", ptStart, InsertScale, 0, atts);
                }
                //插入一卡通系统图,设置摄像头、巡更打卡点数量
                else if (purpose == "一卡通")
                {
                    Dictionary<string, string> atts = new Dictionary<string, string>();
                    atts.Add("电井编号", row_Insert.Id_Dianjing.ToString());
                    atts.Add("IDKEY", row_Insert.IdKey.ToString());
                    atts.Add("门禁控制器", row_Insert.MenJinkongzhi.ToString());
                    atts.Add("门禁", row_Insert.MenJin.ToString());
                    spaceid.InsertBlockReference(sInsertLayer, "RDXT-一卡通门禁1", ptStart, InsertScale, 0, atts);
                }
                else if (purpose == "设备网")
                {
                    Dictionary<string, string> atts = new Dictionary<string, string>();
                    atts.Add("电井编号", row_Insert.Id_Dianjing.ToString());
                    atts.Add("IDKEY", row_Insert.IdKey.ToString());
                    atts.Add("门禁控制器", "x" + row_Insert.MenJinkongzhi.ToString());
                    atts.Add("POE交换机", "x" + row_Insert.ONU_POE24.ToString());
                    atts.Add("普通交换机", "x" + row_Insert.ONU_24.ToString());
                    atts.Add("能耗网关", "x" + row_Insert.NengHao.ToString());
                    atts.Add("信息发布", "x" + row_Insert.XinXifabu.ToString());
                    atts.Add("摄像头", "x" + row_Insert.JianKong.ToString());
                    atts.Add("进线光缆", "6芯光缆");
                    spaceid.InsertBlockReference(sInsertLayer, "RDXT-设备网1", ptStart, InsertScale, 0, atts);
                }
                //插入综合布线系统图
                else if (purpose == "综合布线")
                {
                    //增加交验条件:此防火分区内的设备数>0时添加
                    if(row_Insert.FenGuangqi_216+row_Insert.ONU_GC24POE > 0)
                    {
                        //2024年4月29日 新增分光器的进线光缆计算:每根12芯光缆最多带5个分光器
                        //默认TYPEB保护(每个分光器有两根进线),
                        //默认SC接口,但为LC接口改造预留条件,所以最多使用5口10芯,预留1口2芯,合计12芯
                        int num_GuangXian = 2 * (int)(Math.Ceiling((decimal)(row_Insert.FenGuangqi_216 / 10.00)));
                        Dictionary<string, string> atts = new Dictionary<string, string>();
                        atts.Add("电井编号", row_Insert.Id_Dianjing.ToString());
                        atts.Add("IDKEY", row_Insert.IdKey.ToString());
                        atts.Add("FLOOR1", row_Insert.Floor1.ToString());
                        atts.Add("进线光缆", "12芯光缆x" + num_GuangXian.ToString());
                        atts.Add("配线光缆", "4芯光缆x" + row_Insert.AHD.ToString());
                        atts.Add("分光器", row_Insert.FenGuangqi_216.ToString());
                        atts.Add("ONU", row_Insert.ONU_GC24POE.ToString());
                        atts.Add("AHD", row_Insert.AHD.ToString());
                        atts.Add("AP", row_Insert.AP.ToString());
                        //2024年4月29日修改块名
                        spaceid.InsertBlockReference(sInsertLayer, "RDXT-综合布线-240429", ptStart, InsertScale, 0, atts);
                    } // end of if >0
                }


                //插入广播系统图
                else if (purpose == "广播")
                {
                    //增加交验条件:此防火分区内的设备数>0时添加
                    if (row_Insert.FenGuangqi_216 + row_Insert.ONU_GC24POE > 0)
                    {
                        Dictionary<string, string> atts = new Dictionary<string, string>();
                        atts.Add("电井编号", row_Insert.Id_Dianjing.ToString());
                        atts.Add("IDKEY", row_Insert.IdKey.ToString());
                        atts.Add("FLOOR1", row_Insert.Floor1.ToString());
                        atts.Add("进线光缆", "6芯光缆x" + row_Insert.FenGuangqi_216.ToString());
                        atts.Add("配线光缆", "4芯光缆x" + row_Insert.AHD.ToString());
                        atts.Add("分光器", row_Insert.FenGuangqi_216.ToString());
                        atts.Add("ONU", row_Insert.ONU_GC24POE.ToString());
                        atts.Add("AHD", row_Insert.AHD.ToString());
                        atts.Add("AP", row_Insert.AP.ToString());
                        spaceid.InsertBlockReference(sInsertLayer, "RDXT-综合布线1", ptStart, InsertScale, 0, atts);
                    } // end of if >0
                }
                //调试用的报错函数
                else
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("\n程序设计错误,未找到所需流程函数");
                }
               
                //01插入并设置手报声光警报器
                //if (row_Insert.ShengGuang >= 1)
                //{

                //    Dictionary<string, string> atts = new Dictionary<string, string>();
                //    atts.Add("IO", "1");
                //    atts.Add("设备数量", row_Insert.ShengGuang.ToString());
                //    spaceid.InsertBlockReference(sInsertLayer, "FAS_声光", ptBlk, InsertScale, 0, atts);
                //    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                //    i++;
                //}
                #endregion
            }


        }
        //插入块1结束


        #endregion





    } //end of class
}
