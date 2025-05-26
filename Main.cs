// (C) Copyright 2023 by  
//
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;
using System;
using System.Linq;


using System.Diagnostics;
using System.Collections.Generic;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;


namespace WeakCurrent1
{






    // This class is instantiated by AutoCAD for each document when
    // a command is called by the user the first time in the context
    // of a given document. In other words, non static data in this class
    // is implicitly per-document!
    public partial class MyCommands
    {


        #region AutoCAD测试
        //测试方式二
        [CommandMethod("d2")]
        public void ShowCADDialog()
        {

            FMain form = new FMain();
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form);

        }
        #endregion












    }

    /// <summary>
    /// 自定义class:用于统计块的名称,数量
    /// </summary>
    public class BLK_NUMS
    {
        public string BlockName { get; set; } //块名
        public int Count { get; set; } //数量
    }

    /// <summary>
    /// 自定义class:用于存放楼层信息
    /// </summary>
    public class LouCeng
    {
        public int IdKey { get; set; } //主键
        public string Floor1 { get; set; } //楼层名称
        public int Floor2 { get; set; }  //楼层数字索引
    }

    public class RDJiSuanshuExcel
    {
        public string Key { get; set; } //主键
        public string Id_Dianjing { get; set; }  //电井编号
        public string Floor { get; set; }  //楼层
        public string Floor_ODN { get; set; }  //分光器所在楼层
        public string Floor_PubONU { get; set; }  //公区ONU所在楼层
        public string Num_OfficeONU { get; set; }  //办公室ONU总量
        public string Num_ODN { get; set; }  //分光器数量
        public string Num_PubONU { get; set; }  //公区ONU数量
        public string Num_PubTO { get; set; }  //公区TO信息点数量
        public string Num_PubAP { get; set; }  //公区AP点数量
        public string Num_PubTP { get; set; }  //公区TP电话点数量
        public string Num_Pub2TO { get; set; }  //公区2TO信息点数量
        public string Num_PubTV { get; set; }  //公区TV点数量
        public string Info1 { get; set; }  //备用
    }

    public class AnFangClass
    {
        public int IdKey { get; set; } //主键
        public string Id_Dianjing { get; set; }  //电井编号
        public string Floor1 { get; set; }  //楼层名称
        public int Floor2 { get; set; }     //楼层对应数字层号,1F为1,向下依次为0,-1,-2  // -1 不等于 -B1F
        public int JianKong_qiuji { get; set; }  //球机
        public int JianKong_qiangji { get; set; }  //枪机
        public int JianKong_renlian { get; set; }  //人脸识别摄像机
        public int JianKong_shiwai { get; set; }  //室外摄像机
        public int JianKong { get; set; }  //监控总数
        public int XinXifabu { get; set; }  //信息发布点
        public int NengHao { get; set; }  //能耗点
        public int MenJinkongzhi { get; set; }  //门禁控制器
        public int MenJin { get; set; }  //门禁
        public int XunGeng { get; set; }  //巡更打卡点
        public int WuZhangaihujiao { get; set; }  //无障碍呼叫
        public int RuQintance { get; set; }  //入侵探测
        public int DDC { get; set; }  //DDC
        public int DianWei { get; set; }  //弱电点位信息
        public int ONU_24 { get; set; }  //24口交换机数量
        public int ONU_POE24 { get; set; }  //24口POE交换机数量
        public string gmt_create { get; set; }  //创建时间
        public string gmt_change { get; set; }  //修改时间
        public int AHD { get; set; }  //房间AHD数量
        public int ONU_GC24POE { get; set; }  //综合布线系统24口POE交换机
        public int FenGuangqi_216 { get; set; }  //综合布线系统2:16分光器
        public int AP { get; set; }  //综合布线系统无线AP数量

        public int YuLiu1 { get; set; }  //DDC
        public int YuLiu2 { get; set; }  //DDC
        public int YuLiu3 { get; set; }  //DDC
        public int YuLiu4 { get; set; }  //DDC
        public int YuLiu5 { get; set; }  //DDC
        public int YuLiu6 { get; set; }  //DDC

        //初始化
        public AnFangClass()
        { 
            YuLiu1 = 0;
            YuLiu2 = 0;
            YuLiu3 = 0;
            YuLiu4 = 0;
            YuLiu5 = 0;
            YuLiu6 = 0;
            gmt_change = ""; // 初始化为""
            gmt_create = ""; // 初始化为""

        }



    }


    public class FASJiSuanshuExcel
    {
        public int IdKey { get; set; } //主键
        public string Id_Dianjing { get; set; }  //电井编号
        public string Floor1 { get; set; }  //楼层名称
        public int Floor2 { get; set; }     //楼层对应数字层号,1F为1,向下依次为0,-1,-2  // -1 不等于 -B1F
        public int DianWeiall { get; set; }  //总点位数量
        public int DianWeiliandong { get; set; }  //联动点位数量
        public int SI { get; set; }  //SI数量
        public int HuiLu { get; set; }  //回路数量
        public int ShouBao { get; set; }  //手报数量
        public int ShengGuang { get; set; }  //声光数量
        public int JuanLianA { get; set; }  //疏散通道的防火卷帘数量
        //public int JuanLianB { get; set; }  //非疏散通道的防火卷帘数量
        public int QieFei { get; set; }  //切非模块数量
        public int DianTi { get; set; }  //电梯模块数量
        public int GP { get; set; }  //GP数量
        public int ZYFJ { get; set; }  //正压风机数量
        public int BFJ { get; set; }  //补风机数量
        public int PYFJ { get; set; }  //排烟风机数量
        public int XHSB { get; set; }  //消防水泵--消火栓泵数量
        public int PLB { get; set; }  //消防水泵--喷淋泵数量
        public int WYB { get; set; }  //消防水泵--稳压泵数量
        public int BEC { get; set; }  //BEC数量
        public int BED { get; set; }  //BED数量
        public int BEEH { get; set; }  //BEEH数量
        public int BECH { get; set; }  //BECH数量
        public int DYCB { get; set; }  //BYCB数量
        public int Fa70 { get; set; }  //70度防火阀数量
        public int Fa280 { get; set; }  //280度防火阀数量
        public int ShuiLiuZSQ { get; set; }  //水流指示器数量
        public int ShiShiBJF { get; set; }  //湿式报警阀组数量
        public int XinHaofa { get; set; }  //信号阀数量
        public int LiuLiangKG { get; set; }  //流量开关数量
        public int YaLiKG { get; set; }  //压力开关数量
        public int WenGan { get; set; }  //温感数量
        public int XiaoHuoShuan { get; set; }  //温感数量
        public int EXWenGan { get; set; }  //温感数量
        public int XFDianHua { get; set; }  //消防电话数量
        public int LouCengXSQ { get; set; }  //楼层显示器数量
        public string gmt_create { get; set; }  //此行的创建时间戳
        public string gmt_change { get; set; }  //此行的修改时间戳
        public int B { get; set; }  //电动阀的手动控制按钮 
        public int YanGan { get; set; }  //电动阀的手动控制按钮 
        public int PaiYanchuang { get; set; }  //排烟窗模块数量 
        public int GuangBo { get; set; }  //消防广播 
        public int RD { get; set; }  //常闭防火门 
        public int RDK { get; set; }  //常开防火门
        public int JuanlianA { get; set; }
        public int XXGSYanGan { get; set; }  //预留1 线型光束感烟火灾探测器（接收端）
        public int YuLiu2 { get; set; }  //预留2
        public int YuLiu3 { get; set; }  //预留3
        public int YuLiu4 { get; set; }  //预留4
        public int YuLiu5 { get; set; }  //预留5

        public FASJiSuanshuExcel()
        {
            gmt_change = ""; // 初始化为""
            gmt_create = ""; // 初始化为""
        }


    }

    public class FasRDClass
    {
        public int IdKey { get; set; } //主键
        public string Id_Dianjing { get; set; }  //电井编号
        public string Floor1 { get; set; }  //楼层名称
        public int Floor2 { get; set; }  //楼层名称
        public int GuangBo { get; set; }  //消防广播 
        public int RD { get; set; }  //常闭防火门 
        public int RDK { get; set; }  //常开防火门
 
    }


    public class FASInfo
    {
        public string Id_Dianjing { get; set; }  //电井编号
        public string Floor1 { get; set; }  //楼层名称
    }

    /// <summary>
    /// 重复、未找到的防火分区名称及数量
    /// </summary>
    public class AreaInfo
    {
        public string Id_Dianjing { get; set; }  //电井编号
        public int NUM_EXIST { get; set; }  //楼层名称
    }

}
