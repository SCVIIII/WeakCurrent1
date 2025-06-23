using Autodesk.AutoCAD.Geometry;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeakCurrent1.Common
{

    public class OperatorFile
    {



        [DllImport("kernel32")] //引入“shell32.dll”API文件
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 从INI文件中读取指定节点的内容
        /// </summary>
        /// <param name="section">INI节点</param>
        /// <param name="key">节点下的项</param>
        /// <param name="def">没有找到内容时返回的默认值</param>
        /// <param name="filePath">要读取的INI文件</param>
        /// <returns>读取的节点内容</returns>
        public static string GetIniFileString(string section, string key, string def, string filePath)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(section, key, def, temp, 1024, filePath);
            return temp.ToString();
        }


    }

    /// <summary>
    /// 存放函数
    /// </summary>
    public static class MyTools
    {

        public static bool ceshi3()
        {
            return true;
        }

       
        /// <summary>
        /// 计算此防火分区的点位总数
        /// </summary>
        /// <param name="info_RD"></param>
        /// <returns></returns>
        public static int CalDianWeiall(FASJiSuanshuExcel info_RD)
        {
            int Num_DianWeiall =
                info_RD.ShouBao + info_RD.ShengGuang + 2 * info_RD.JuanLianA + info_RD.QieFei + info_RD.DianTi + info_RD.GP +
                6 * info_RD.ZYFJ + 6 * info_RD.BFJ + 6 * info_RD.PYFJ + 8 * info_RD.XHSB + 8 * info_RD.PLB + info_RD.WYB +
                info_RD.BEC + info_RD.BED + info_RD.BEEH + info_RD.BECH + info_RD.DYCB + info_RD.Fa70 + info_RD.Fa280 +
                info_RD.ShuiLiuZSQ + 3 * info_RD.XinHaofa + info_RD.ShiShiBJF + info_RD.LiuLiangKG + info_RD.YaLiKG + info_RD.WenGan + info_RD.XiaoHuoShuan + info_RD.EXWenGan + info_RD.B + info_RD.YanGan + info_RD.PaiYanchuang;
            return Num_DianWeiall;
        }

        /// <summary>
        /// 计算此防火分区联动点位数量
        /// </summary>
        /// <param name="info_RD"></param>
        /// <returns></returns>
        public static int CalDianWeiLianDong(FASJiSuanshuExcel info_RD)
        {
            int Num_DianWeiliandong =
                2 * info_RD.JuanLianA + info_RD.QieFei + info_RD.DianTi + info_RD.GP +
                6 * info_RD.ZYFJ + 6 * info_RD.BFJ + 6 * info_RD.PYFJ + 8 * info_RD.XHSB + 8 * info_RD.PLB + info_RD.WYB +
                info_RD.BEC + info_RD.BED + info_RD.BEEH + info_RD.BECH + info_RD.DYCB + info_RD.Fa70 + info_RD.Fa280 +
                info_RD.ShuiLiuZSQ + 3 * info_RD.XinHaofa + info_RD.ShiShiBJF + info_RD.LiuLiangKG + info_RD.YaLiKG + info_RD.XiaoHuoShuan + info_RD.B  + info_RD.PaiYanchuang;
            return Num_DianWeiliandong;
        }

        /// <summary>
        /// 计算此防火分区对应的回路数量(至少为1条)
        /// </summary>
        /// <param name="info_RD"></param>
        /// <returns></returns>
        public static int CalDianWeiHuilu(int Num_DianWeiall, int Num_DianWeiliandong)
        {
            int Num_HuiLu = (int)Math.Max(Math.Ceiling((decimal)(Num_DianWeiall / 180.00)),
                                          Math.Ceiling((decimal)(Num_DianWeiliandong / 90.00))
                                              );
            return Num_HuiLu;
        }



        //Num_DianWeiall= num_Fasblks[5]+ num_Fasblks[6]+ num_Fasblks[7]+ num_Fasblks[8] + num_Fasblks[9] + num_Fasblks[10] 
        //      + num_Fasblks[11] + num_Fasblks[12] + num_Fasblks[13] + num_Fasblks[14] + num_Fasblks[15]
        //


        #region 查询火灾报警表Dapper版本
        public static List<FASJiSuanshuExcel> UIDGSecToFasList2(UIDataGridView uiDG_All)
        {
            //新建类
            List<FASJiSuanshuExcel> list_Xuanzhong = new List<FASJiSuanshuExcel>();

            

            return list_Xuanzhong;
        }


        #endregion


        /// <summary>
        /// 将DataGridView中的选中行,转为List<FASJiSuanshuExcel>
        /// </summary>
        /// <param name="uiDG_All"></param>
        /// <returns></returns>
        public static List<FASJiSuanshuExcel> UIDGSecToFasList(UIDataGridView uiDG_All)
        {
            //新建类
            List<FASJiSuanshuExcel> list_Xuanzhong = new List<FASJiSuanshuExcel>();
            for (int i=0;i<uiDG_All.RowCount;i++)
            {
                DataGridViewRow row = uiDG_All.Rows[i];
                
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)uiDG_All.Rows[i].Cells[0];
                Boolean flag = Convert.ToBoolean(checkCell.Value);
                if (flag == true)     //查找被选择的数据行  
                {
                    //新建FAS类,并将数据逐格填入
                    FASJiSuanshuExcel fas = new FASJiSuanshuExcel
                    {

                        IdKey = row.Cells["IdKey"].Value.ToString().ToInt(),  //获取IdKey
                        Id_Dianjing = row.Cells["Id_Dianjing"].Value.ToString(),
                        Floor1 = row.Cells["Floor1"].Value.ToString(),
                        Floor2 = row.Cells["Floor2"].Value.ToString().ToInt(),
                        ShouBao = row.Cells["ShouBao"].Value.ToString().ToInt(),
                        ShengGuang = row.Cells["ShengGuang"].Value.ToString().ToInt(),
                        JuanLianA = row.Cells["JuanLianA"].Value.ToString().ToInt(),
                        QieFei = row.Cells["QieFei"].Value.ToString().ToInt(),
                        DianTi = row.Cells["DianTi"].Value.ToString().ToInt(),

                        GP = row.Cells["GP"].Value.ToString().ToInt(),
                        ZYFJ = row.Cells["ZYFJ"].Value.ToString().ToInt(),
                        BFJ = row.Cells["BFJ"].Value.ToString().ToInt(),
                        PYFJ = row.Cells["PYFJ"].Value.ToString().ToInt(),
                        XHSB = row.Cells["XHSB"].Value.ToString().ToInt(),
                        PLB = row.Cells["PLB"].Value.ToString().ToInt(),
                        WYB = row.Cells["WYB"].Value.ToString().ToInt(),
                        BEC = row.Cells["BEC"].Value.ToString().ToInt(),
                        BED = row.Cells["BED"].Value.ToString().ToInt(),
                        BEEH = row.Cells["BEEH"].Value.ToString().ToInt(),

                        BECH = row.Cells["BECH"].Value.ToString().ToInt(),
                        DYCB = row.Cells["DYCB"].Value.ToString().ToInt(),
                        Fa70 = row.Cells["Fa70"].Value.ToString().ToInt(),
                        Fa280 = row.Cells["Fa280"].Value.ToString().ToInt(),
                        ShuiLiuZSQ = row.Cells["ShuiLiuZSQ"].Value.ToString().ToInt(),
                        XinHaofa = row.Cells["XinHaofa"].Value.ToString().ToInt(),
                        ShiShiBJF = row.Cells["ShiShiBJF"].Value.ToString().ToInt(),
                        LiuLiangKG = row.Cells["LiuLiangKG"].Value.ToString().ToInt(),
                        YaLiKG = row.Cells["YaLiKG"].Value.ToString().ToInt(),
                        WenGan = row.Cells["WenGan"].Value.ToString().ToInt(),

                        XiaoHuoShuan = row.Cells["XiaoHuoshuan"].Value.ToString().ToInt(),
                        EXWenGan = row.Cells["EXWenGan"].Value.ToString().ToInt(),
                        XFDianHua = row.Cells["XFDianHua"].Value.ToString().ToInt(),
                        LouCengXSQ = row.Cells["LouCengXSQ"].Value.ToString().ToInt(),
                        gmt_create = row.Cells["gmt_create"].Value.ToString(),
                        gmt_change = row.Cells["gmt_change"].Value.ToString(),
                        B = row.Cells["B"].Value.ToString().ToInt(),
                        YanGan = row.Cells["YanGan"].Value.ToString().ToInt(),

                        SI = row.Cells["SI"].Value.ToString().ToInt(),
                        HuiLu = row.Cells["HuiLu"].Value.ToString().ToInt(),
                        DianWeiliandong = row.Cells["DianWeiliandong"].Value.ToString().ToInt(),
                        DianWeiall = row.Cells["DianWeiall"].Value.ToString().ToInt(),

                        GuangBo = row.Cells["GuangBo"].Value.ToString().ToInt(),
                        RD = row.Cells["RD"].Value.ToString().ToInt(),
                        RDK = row.Cells["RDK"].Value.ToString().ToInt(),
                        PaiYanchuang = row.Cells["PaiYanchuang"].Value.ToString().ToInt(),
                        XXGSYanGan = row.Cells["XXGSYanGan"].Value.ToString().ToInt(),
                    }; //end of fas
                    list_Xuanzhong.Add(fas);
                    
                }
            }

            //将list数据进行重排
            List<FASJiSuanshuExcel> list_Paixu = list_Xuanzhong.OrderBy(d => d.Floor2).ThenBy(d => GetFenquHao(d.Id_Dianjing)).ToList();
            return list_Paixu;

        }

        //测试函数:UIDGtoList
        public static List<FASJiSuanshuExcel> ConvertDataGridViewToClass<FASJiSuanshuExcel>(UIDataGridView dataGridView) where FASJiSuanshuExcel : new()
        {
            List<FASJiSuanshuExcel> dataList = new List<FASJiSuanshuExcel>();

            // 获取类的所有公共属性
            PropertyInfo[] properties = typeof(FASJiSuanshuExcel).GetProperties();

            // 遍历 DataGridView 的行数据
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                // 创建类对象实例
                FASJiSuanshuExcel obj = new FASJiSuanshuExcel();

                // 遍历属性，并将对应的单元格数据赋值给属性
                foreach (PropertyInfo property in properties)
                {

                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)row.Cells[0];
                    Boolean flag = Convert.ToBoolean(checkCell.Value);
                    if(flag)
                    {
                        // 根据属性名称获取对应的单元格值
                        object cellValue = row.Cells[property.Name]?.Value;

                        // 如果单元格值不为空，则将其转换为属性类型并赋值给属性
                        if (cellValue != null)
                        {
                            object value = Convert.ChangeType(cellValue, property.PropertyType);
                            property.SetValue(obj, value);
                        }
                    }
                    
                }

                // 将对象添加到列表中
                dataList.Add(obj);
            }

            return dataList;
        }

        //end of UIDGtoList


        /// <summary>
        /// 根据电井编号,返回所在的序号,如B1F-12返回 12
        /// </summary>
        /// <param name="Id_Dianjing"></param>
        /// <returns></returns>
        public static string GetFenquHao(string Id_Dianjing)
        {
            //检索 '-' 所在的位置,从0起
            int index1 = Id_Dianjing.IndexOf("-");
            //字符串长度
            int index2 = Id_Dianjing.Length;
            //返回值
            //若检索条件为 'F-' ,则应为index1 + 2
            string d = Id_Dianjing.Substring(index1 + 1);  
            // This line prints "Hello, World" 
            return d;
           
        }

        /// <summary>
        /// 获取楼层名称
        /// </summary>
        /// <param name="Id_Dianjing"></param>
        /// <returns></returns>
        public static string GetLoucengming(string Id_Dianjing)
        {
            //检索 '-' 所在的位置,从0起
            int index1 = Id_Dianjing.IndexOf("-");
            //字符串长度
            int index2 = Id_Dianjing.Length;
            //返回值
            //若检索条件为 'F-' ,则应为index1 + 2
            string d = Id_Dianjing.Substring(0,index1);
            // This line prints "Hello, World" 
            return d;

        }


        /// <summary>
        /// 从MySQL中查询所有的楼层表信息
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<LouCeng> ChaXunlouceng()
        {

            //查询楼层表格
            //新建返回值的list
            List<LouCeng> list_Louceng = new List<LouCeng>();
            //查询指令
            string sql = "SELECT * FROM  louceng" ;
            //打开数据库连接
            var conn = SQLiteConn.ConnSQLite();
            //数据库查询
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                using(SQLiteDataAdapter daadpter = new SQLiteDataAdapter(cmd))
                {
                    DataSet dts2 = new DataSet();
                    //将查询信息存入dt2的对应表
                    daadpter.Fill(dts2, "TableLouceng");
                    System.Data.DataTable t2 = dts2.Tables["TableLouceng"];
                    //关闭数据库连接
                    conn.Close();
                    //datatable转为list
                    for (int i = 0; i < t2.Rows.Count; i++)
                    {
                        LouCeng info = new LouCeng
                        {
                            IdKey = t2.Rows[i]["IdKey"].ToString().ToInt(),
                            Floor1 = t2.Rows[i]["Floor1"].ToString(),
                            Floor2 = t2.Rows[i]["Floor2"].ToString().ToInt()
                        };
                        list_Louceng.Add(info);
                    }
                    //返回值
                    conn.Close();
                    return list_Louceng;
                }

            } //end of using (var command = new MySqlCommand(query, conn))
        }

        /// <summary>
        /// 查询楼层的索引号
        /// </summary>
        /// <param name="Floor1"></param>
        /// <param name="list_Louceng"></param>
        /// <returns></returns>
        public static int ChaXunFloor2(string Floor1, List<LouCeng> list_Louceng)
        {
            //返回值默认为99999
            int Floor2 = 99999;
            //根据楼层名称查询
            List<int> t1 = (from d in list_Louceng
                            where d.Floor1 == Floor1
                            select d.Floor2).ToList();
            //若楼层名称存在,返回对应的楼层索引号
            if (t1.Count > 0)
            {
                Floor2 = t1[0];
            }
            //返回值
            return Floor2;

        }

        //块数量计数函数
        public static int count_Blks_Num(List<BLK_NUMS>  BLK_NUMS ,List<string > BLK_NAMES)
        {
            //完整语句
            return (from block in BLK_NUMS
                      where BLK_NAMES.Contains(block.BlockName)
                      select block.Count).Sum();
            //缩写
            //return BLK_NUMS.Where(x => BLK_NAMES.Contains(x.BlockName)).Sum(x => x.Count);
        }





    }
}
