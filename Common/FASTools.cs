using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;


using MySql.Data.MySqlClient;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WeakCurrent1.Common
{
    public static class FASTools
    {
        /// <summary>
        /// 在CAD中插入当前防火分区的系统图
        /// </summary>
        /// <param name="db"></param>
        /// <param name="row_Insert"></param>
        /// <param name="ptStart"></param>
        public static void Insert_Xiaofangblk2(Database db, FASJiSuanshuExcel row_Insert, Point3d ptStart)
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
                int i = 0;
                //普通块的间隔
                int blk_distance = 750;
                //水泵,风机的间距
                int blk_distance2 = 850;
                Point3d pt_XD = ptStart.PolarPoint(0, 6500).PolarPoint((Math.PI / 2), 1300);
                Point3d ptBlk = ptStart.PolarPoint(0, 7300).PolarPoint((Math.PI / 2), 1700);
                Point3d ptKxian1 = ptStart.PolarPoint(0, 6500).PolarPoint((Math.PI / 2), 3150);
                Point3d ptKxian2 = new Point3d();

                //插入楼层框,并自动填写楼层,防火分区,主键,点位信息
                if (true)
                {
                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "防火分区", row_Insert.Id_Dianjing.ToString() },
                        { "楼层", row_Insert.Floor1.ToString() },
                        { "KEY", row_Insert.IdKey.ToString() },
                        { "点位总数", "点位总数: " + row_Insert.DianWeiall.ToString() },
                        { "联动点位", "联动点位: " + row_Insert.DianWeiliandong.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS-楼层1", ptStart, InsertScale, 0, atts);
                }

                //00插入并设置手报数量
                if (row_Insert.ShouBao >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "设备数量", row_Insert.ShouBao.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_手报", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //01插入并设置手报声光警报器
                if (row_Insert.ShengGuang >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", "1" },
                        { "设备数量", row_Insert.ShengGuang.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_声光", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //02插入并设置防火卷帘数量
                if (row_Insert.JuanLianA >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", (2 * row_Insert.JuanLianA).ToString() },
                        { "设备数量", row_Insert.JuanLianA.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_卷帘", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //03插入并设置切非模块数量 FAS_切非
                if (row_Insert.QieFei >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.QieFei.ToString() },
                        { "设备数量", row_Insert.QieFei.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_切非", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //04插入并设置电梯模块数量
                if (row_Insert.DianTi >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.DianTi.ToString() },
                        { "设备数量", row_Insert.DianTi.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_电梯", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //05插入并设置GP风口数量
                if (row_Insert.GP >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.GP.ToString() },
                        { "设备数量", row_Insert.GP.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_GP", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //06正压风机 FAS_正压风机
                if (row_Insert.ZYFJ >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.ZYFJ.ToString() },
                        { "设备数量", row_Insert.ZYFJ.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_正压风机", ptBlk, InsertScale, 0, atts);
                    //K线的终点
                    ptKxian2 = ptBlk.PolarPoint(0, 500).PolarPoint((Math.PI / 2), 1450);
                    //指向下一个块的插入点
                    ptBlk = ptBlk.PolarPoint(0, blk_distance2);

                    i++;
                }

                //07补风机 FAS_补风机
                if (row_Insert.BFJ >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.BFJ.ToString() },
                        { "设备数量", row_Insert.BFJ.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_补风机", ptBlk, InsertScale, 0, atts);
                    //K线的终点
                    ptKxian2 = ptBlk.PolarPoint(0, 500).PolarPoint((Math.PI / 2), 1450);
                    //指向下一个块的插入点
                    ptBlk = ptBlk.PolarPoint(0, blk_distance2);
                    i++;
                }

                //07+1 排烟风机 FAS_排烟风机
                if (row_Insert.PYFJ >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.PYFJ.ToString() },
                        { "设备数量", row_Insert.PYFJ.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_排烟风机", ptBlk, InsertScale, 0, atts);
                    //K线的终点
                    ptKxian2 = ptBlk.PolarPoint(0, 500).PolarPoint((Math.PI / 2), 1450);
                    //指向下一个块的插入点
                    ptBlk = ptBlk.PolarPoint(0, blk_distance2);
                    i++;
                }


                //08消火栓泵 FAS_消防泵
                if (row_Insert.XHSB >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.XHSB.ToString() },
                        { "设备数量", row_Insert.XHSB.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_XHSB1", ptBlk, InsertScale, 0, atts);
                    //K线的终点
                    ptKxian2 = ptBlk.PolarPoint(0, 500).PolarPoint((Math.PI / 2), 1450);
                    //指向下一个块的插入点
                    ptBlk = ptBlk.PolarPoint(0, blk_distance2);
                    i++;
                }

                //09喷淋泵 FAS_消防泵
                if (row_Insert.PLB >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.PLB.ToString() },
                        { "设备数量", row_Insert.PLB.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_PLB1", ptBlk, InsertScale, 0, atts);
                    //K线的终点
                    ptKxian2 = ptBlk.PolarPoint(0, 500).PolarPoint((Math.PI / 2), 1450);
                    //指向下一个块的插入点
                    ptBlk = ptBlk.PolarPoint(0, blk_distance2);
                    i++;
                }

                //10稳压泵 FAS_稳压泵
                if (row_Insert.WYB >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.WYB.ToString() },
                        { "设备数量", row_Insert.WYB.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_稳压泵", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance2);
                    i++;
                }

                //11 BEC FAS_BEC
                if (row_Insert.BEC >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.BEC.ToString() },
                        { "设备数量", row_Insert.BEC.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_BEC", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //12 BED FAS_BED
                if (row_Insert.BED >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.BED.ToString() },
                        { "设备数量", row_Insert.BED.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_BED", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //13 BEEH FAS_BEEH
                if (row_Insert.BEEH >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.BEEH.ToString() },
                        { "设备数量", row_Insert.BEEH.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_BEEH", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //14 BECH FAS_BECH
                if (row_Insert.BECH >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.BECH.ToString() },
                        { "设备数量", row_Insert.BECH.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_BECH", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //15挡烟垂壁 FAS_挡烟垂壁
                if (row_Insert.DYCB >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.DYCB.ToString() },
                        { "设备数量", row_Insert.DYCB.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_挡烟垂壁", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //排烟窗
                if (row_Insert.PaiYanchuang >= 1)
                {
                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.PaiYanchuang.ToString() },
                        { "设备数量", row_Insert.PaiYanchuang.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_排烟窗1", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //线型光束烟感（接收端）
                if (row_Insert.XXGSYanGan >= 1)
                {
                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.XXGSYanGan.ToString() },
                        { "设备数量", row_Insert.XXGSYanGan.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_线型烟感-接收1", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //16 70度防火阀 FAS_70度阀
                if (row_Insert.Fa70 >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.Fa70.ToString() },
                        { "设备数量", row_Insert.Fa70.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_70度阀", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //17 280度排烟防火阀 FAS_280度阀
                if (row_Insert.Fa280 >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.Fa280.ToString() },
                        { "设备数量", row_Insert.Fa280.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_280度阀", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //18 FAS_水流+信号阀 FAS_水流+信号阀
                if (row_Insert.XinHaofa >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.XinHaofa.ToString() },
                        { "设备数量", row_Insert.XinHaofa.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_水流+信号阀", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance+290);
                    i++;
                }

                //19 湿式报警阀组 FAS_湿式报警阀
                if (row_Insert.ShiShiBJF >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", (3 * row_Insert.ShiShiBJF).ToString() },
                        { "设备数量", row_Insert.ShiShiBJF.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_湿式报警阀", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //20 流量开关 FAS_流量开关
                if (row_Insert.LiuLiangKG >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", (2 *row_Insert.LiuLiangKG).ToString() },
                        { "设备数量", row_Insert.LiuLiangKG.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_流量开关", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //21 压力开关 FAS_压力开关
                if (row_Insert.YaLiKG >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.YaLiKG.ToString() },
                        { "设备数量", row_Insert.YaLiKG.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_压力开关", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //22 线型光束烟感（发射端）
                if (row_Insert.XXGSYanGan >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.XXGSYanGan.ToString() },
                        { "设备数量", row_Insert.XXGSYanGan.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_线型烟感-发射1", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //22 烟感 FAS_烟感
                if (row_Insert.YanGan >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.YanGan.ToString() },
                        { "设备数量", row_Insert.YanGan.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_烟感", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //23 温感 FAS_温感
                if (row_Insert.WenGan >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.WenGan.ToString() },
                        { "设备数量", row_Insert.WenGan.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_温感", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //24 消火栓 FAS_消火栓
                if (row_Insert.XiaoHuoShuan >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.XiaoHuoShuan.ToString() },
                        { "设备数量", row_Insert.XiaoHuoShuan.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_消火栓", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //25 防爆温感 FAS_防爆温感
                if (row_Insert.EXWenGan >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "IO", row_Insert.EXWenGan.ToString() },
                        { "设备数量", row_Insert.EXWenGan.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_防爆温感", ptBlk, InsertScale, 0, atts);
                    ptBlk = ptBlk.PolarPoint(0, blk_distance);
                    i++;
                }

                //27 消防电话
                if (row_Insert.XFDianHua >= 1)
                {
                    ptBlk = pt_XD.PolarPoint(0, -2360).PolarPoint((Math.PI / 2), 910);
                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "设备数量", row_Insert.XFDianHua.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_消防电话", ptBlk, InsertScale, 0, atts);
                    i++;
                }


                //28 楼层显示器
                if (row_Insert.LouCengXSQ >= 1)
                {
                    ptBlk = pt_XD.PolarPoint(0, -2600).PolarPoint((Math.PI / 2), 1820);
                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        { "设备数量", row_Insert.LouCengXSQ.ToString() }
                    };
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_楼层显示器", ptBlk, InsertScale, 0, atts);
                    i++;
                }

                //28 插入模块箱 FAS_XD
                if (i >= 1)
                {

                    Dictionary<string, string> atts = new Dictionary<string, string>
                    {
                        //默认设置SI数量为空
                        { "SI", "x" + row_Insert.SI.ToString() },
                        { "回路数量", row_Insert.HuiLu.ToString() + "个回路" }
                    };
                    //插入块
                    spaceid.InsertBlockReference(sInsertLayer, "FAS_端子箱", pt_XD, InsertScale, 0, atts);
                    i++;
                }



                //连接K线
                if (ptKxian2.X > 300)
                {
                    ARXTools.AddLine(db, ptKxian1, ptKxian2, "WIRE-照明");
                }

                #endregion
            } //end foreach


        } //end of Insert_Xiaofangblk2

        /// <summary>
        /// 加两个fas Class相加,合并为一条数据
        /// </summary>
        /// <param name="fas1"></param>
        /// <param name="fas2"></param>
        /// <returns></returns>
        public static FASJiSuanshuExcel FasClassXiangjia(FASJiSuanshuExcel fas1, FASJiSuanshuExcel fas2)
        {

            //新建Class,将两个值合并
            FASJiSuanshuExcel fas = new FASJiSuanshuExcel()
            {
                IdKey = fas1.IdKey,
                Id_Dianjing = fas1.Id_Dianjing,
                Floor1 = fas1.Floor1,
                Floor2 = fas1.Floor2,

                //新增内容添加至此处
                //230409 SI
                SI = fas1.SI + fas2.SI,
                //新增结束

                ShouBao = fas1.ShouBao + fas2.ShouBao,
                ShengGuang = fas1.ShengGuang + fas2.ShengGuang,
                JuanLianA = fas1.JuanLianA + fas2.JuanLianA,
                QieFei = fas1.QieFei + fas2.QieFei,
                DianTi = fas1.DianTi + fas2.DianTi,
                GP = fas1.GP + fas2.GP,

                ZYFJ = fas1.ZYFJ + fas2.ZYFJ,
                BFJ = fas1.BFJ + fas2.BFJ,
                PYFJ = fas1.PYFJ + fas2.PYFJ,
                XHSB = fas1.XHSB + fas2.XHSB,
                PLB = fas1.PLB + fas2.PLB,

                WYB = fas1.WYB + fas2.WYB,
                BEC = fas1.BEC + fas2.BEC,
                BED = fas1.BED + fas2.BED,
                BEEH = fas1.BEEH,
                BECH = fas1.BECH + fas2.BECH,

                DYCB = fas1.DYCB + fas2.DYCB,
                Fa70 = fas1.Fa70 + fas2.Fa70,
                Fa280 = fas1.Fa280 + fas2.Fa280,
                ShuiLiuZSQ = fas1.ShuiLiuZSQ + fas2.ShuiLiuZSQ,
                XinHaofa = fas1.XinHaofa + fas2.XinHaofa,

                ShiShiBJF = fas1.ShiShiBJF + fas2.ShiShiBJF,
                LiuLiangKG = fas1.LiuLiangKG + fas2.LiuLiangKG,
                YaLiKG = fas1.YaLiKG + fas2.YaLiKG,
                WenGan = fas1.WenGan + fas2.WenGan,
                XiaoHuoShuan = fas1.XiaoHuoShuan + fas2.XiaoHuoShuan,

                EXWenGan = fas1.EXWenGan + fas2.EXWenGan,
                XFDianHua = fas1.XFDianHua + fas2.XFDianHua,
                LouCengXSQ = fas1.LouCengXSQ + fas2.LouCengXSQ,

                B = fas1.B + fas2.B,
                YanGan = fas1.YanGan + fas2.YanGan,
                PaiYanchuang = fas1.PaiYanchuang + fas2.PaiYanchuang,
                XXGSYanGan=fas1.XXGSYanGan+fas2.XXGSYanGan,

                DianWeiliandong = fas1.DianWeiliandong + fas2.DianWeiliandong,
                DianWeiall = fas1.DianWeiall + fas2.DianWeiall,

                GuangBo = fas1.GuangBo + fas2.GuangBo,
                RD = fas1.RD + fas2.RD,
                RDK = fas1.RDK + fas2.RDK


            };

            //回路数量
            fas.HuiLu = (int)Math.Max(Math.Ceiling((decimal)(fas.DianWeiall / 180.00)),
                                          Math.Ceiling((decimal)(fas.DianWeiliandong / 90.00)));


            return fas;
        } //end of FasClassXiangjia

        


        //230512测试函数:优化计数
        //320810新增:计数子函数
        //public static

        /// <summary>
        /// 返回当前防火分区的火灾报警点位数量
        /// 当前防火分区内所有图元的objId输出FASJiSuanshuExcel
        /// </summary>
        /// <param name="id_ss">当前防火分区内所有图元的objID</param>
        /// <returns>FASJiSuanshuExcel</returns>
        public static FASJiSuanshuExcel ParCal_FasBlkToClass(List<LouCeng> list_Louceng,ObjectId id_Info,ObjectId[] id_ss) //Transaction trans,
        {
            //GPT优化代码,原理及输出形式未拆解
            //将ObjectId按图块名称分组,用于后续数据计算
            List<BLK_NUMS> num_blks = (from id_blk in id_ss
                           group id_blk by id_blk.GetBlockName().ToUpper() into g
                           select new BLK_NUMS { BlockName = g.Key, Count = g.Count() }).ToList();



            string Id_Dianjing = id_Info.GetAttributeInBlockReference("电井/防火分区");
            string Floor1 = Common.MyTools.GetLoucengming(Id_Dianjing);
            int IdKey = id_Info.GetAttributeInBlockReference("KEY").ToInt();
            int Floor2 = Common.MyTools.ChaXunFloor2(Floor1, list_Louceng);

            //第i个防火分区的各设备数量
            //测试用的显示信息
            //设置相关信息
            //将num_blks按名称填入fas的对应属性
            FASJiSuanshuExcel fas = new FASJiSuanshuExcel()
            {
                //防火分区/电井编号
                Id_Dianjing = Id_Dianjing,
                //楼层名称
                Floor1 = Floor1,
                //楼层索引号
                Floor2 = Floor2,
                //主键
                IdKey = IdKey,
                // 手报(带电话插孔)
                ShouBao = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002655" }),
                // 声光
                //"$EQUIP$00002679"
                ShengGuang = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002679" }),
                //num_blks.FirstOrDefault(x => new string[] { "$EQUIP$00002679" }.Contains(x.BlockName))?.Count ?? 0,
                // 防火卷帘(疏散通道)
                //"$EQUIP$00002770"
                //"$EQUIP_U$00000248"
                JuanLianA = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002770", "$EQUIP_U$00000248(SY3SUNZM)" }),
                // 切非模块
                QieFei = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP_U$00000176(SY3SUNZM)", "$EQUIP_U$00000250(SY3SUNZM)" }),
                // 电梯模块
                DianTi = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP_U$00000177(SY3SUNZM)" }),
                // 烟感
                YanGan = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002649" }),
                // 温感
                WenGan = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002648" }),
                // 防爆温感
                EXWenGan = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002396" }),
                // 消防电话
                XFDianHua = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002658" }),
                // 楼层显示器
                LouCengXSQ = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002779" }),
                // 短路隔离器
                SI = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002776" }),
                // 消火栓
                XiaoHuoShuan = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002656" }),
                // 压力开关
                YaLiKG = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002755" }),
                // 流量开关
                LiuLiangKG = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00003128" }),
                // 水流指示器
                //num_blks.FirstOrDefault(x => new string[] { "$EQUIP$00002792", "$EQUIP$00002760" }.Contains(x.BlockName))?.Count ?? 0,
                ShuiLiuZSQ = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002792", "$EQUIP$00002760" }),
                // 信号阀
                XinHaofa = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002794" }),
                // 湿式报警阀组
                ShiShiBJF = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00003103" }),
                // 消火栓泵
                //$equip_U$00000178(SY3SUNZM)
                XHSB = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP_U$00000178(SY3SUNZM)" }),
                // 喷淋泵
                PLB = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP_U$00000179(SY3SUNZM)" }),
                // 稳压泵
                WYB = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP_U$00000180(SY3SUNZM)" }),
                // GP 楼梯间正压送风口
                GP = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00003100", "FASPM-GP1", "FASPM-GP2" }),
                //num_blks.FirstOrDefault(x => new string[] { "$EQUIP$00003100", "FASPM-GP1" }.Contains(x.BlockName))?.Count ?? 0,
                // 挡烟垂壁
                DYCB = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP_U$00000141(SY3SUNZM)", "$EQUIP_U$00000184(SY3SUNZM)" }),
                // 正压风机
                ZYFJ = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP_U$00000182(SY3SUNZM)", "$EQUIP_U$00000161(SY3SUNZM)" }),
                // 消防补风机
                BFJ = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP_U$00000183(SY3SUNZM)", "$EQUIP_U$00000162(SY3SUNZM)" }),
                // 排烟风机
                PYFJ = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP_U$00000181(SY3SUNZM)", "$EQUIP_U$00000160(SY3SUNZM)" }),
                // BEC 70℃
                BEC = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP_U$00000172(SY3SUNZM)", "FASPM-BEC1", "FASPM-BEC2" }),
                // BED 70℃动作的常开防火阀
                BED = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002402", "FASPM-BED1" }),
                //BEEH 280℃动作的常开排烟阀
                BEEH = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002403", "FASPM-BEEH1" }),
                //BECH 
                // num_blks.FirstOrDefault(x => new string[] { "$EQUIP$00002413", "FASPM-BECH1", "FASPM-BECH2" }.Contains(x.BlockName))?.Count ?? 0,
                //BECH = num_blks.FirstOrDefault(x => new string[] { "$EQUIP$00002413", "FASPM-BECH1", "FASPM-BECH2" }.Contains(x.BlockName))?.Count ?? 0,
                BECH = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002413", "FASPM-BECH1", "FASPM-BECH2" }),
                //70度防火阀

                Fa70 = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002859", "FASPM-70度防火阀1" }),
                //280度防火阀
                Fa280 = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002858", "FASPM-280度防火阀1" }),
                // 待添加：BECH的就地手动控制按钮E
                B = MyTools.count_Blks_Num(num_blks, new List<string> { "待添加" }),
                //电动排烟窗
                PaiYanchuang = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP_U$00000249(SY3SUNZM)" }),
                //线型光束感烟探测器（接收端）
                XXGSYanGan = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002763" }),
                //num_blks.FirstOrDefault(x => new string[] { "$EQUIP_U$00000249(SY3SUNZM)" }.Contains(x.BlockName))?.Count ?? 0,
                //消防广播
                GuangBo = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002341", "$EQUIP_U$00000114(SY3SUNZM)", "$EQUIP$00002544", "$EQUIP$00002984" }),
                //卷帘
                //JuanLianA = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00000248" }),
                //常闭防火门监控模块
                RD = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP$00002771", "A$C54AB6E89", "$EQUIP_U$00000081(SY3SUNZM)" }),
                //常开防火门监控模块
                RDK = MyTools.count_Blks_Num(num_blks, new List<string> { "$EQUIP_U$00000126(SY3SUNZM)" }),
                
            }; //end of  fas

            //fas.DianWeiall等于以上所有属性数量总和
            //待修正：火灾报警点位统计
            fas.DianWeiall = fas.ShouBao + fas.ShengGuang + 2 * fas.JuanLianA + fas.QieFei + fas.DianTi + fas.YanGan + fas.WenGan + fas.EXWenGan +
                +fas.XiaoHuoShuan + fas.YaLiKG + fas.LiuLiangKG + fas.ShuiLiuZSQ + fas.XinHaofa +
                3 * fas.ShiShiBJF + 8 * fas.XHSB + 8 * fas.PLB + fas.WYB + fas.GP + 1 * fas.DYCB + 6 * fas.ZYFJ + 6 * fas.BFJ + 6 * fas.PYFJ + fas.BEC
                + fas.BED +  fas.BEEH +  fas.BECH + fas.Fa70 + fas.Fa280 + fas.B + fas.PaiYanchuang + 2 * fas.XXGSYanGan;
            //联动点位数量
            fas.DianWeiliandong = 2 * fas.JuanLianA + fas.QieFei + fas.DianTi
                 + fas.YaLiKG + fas.LiuLiangKG + fas.ShuiLiuZSQ + fas.XinHaofa +
                3 * fas.ShiShiBJF + 8 * fas.XHSB + 8 * fas.PLB + fas.WYB + fas.GP + fas.DYCB + 6 * fas.ZYFJ + 6 * fas.BFJ + 6 * fas.PYFJ + fas.BEC
                + fas.BED + fas.BEEH + 2*fas.BECH + fas.Fa70 + fas.Fa280 + fas.B + fas.PaiYanchuang+fas.XXGSYanGan;
            //回路数量
            fas.HuiLu = (int)Math.Max(Math.Ceiling((decimal)(fas.DianWeiall / 180.00)),
                                          Math.Ceiling((decimal)(fas.DianWeiliandong / 90.00)));
            //返回值
            return fas;
        } // end of 新计数函数Par


        /// <summary>
        /// MySQL信息处理完成后,返回到CAD文件中,为图块添加块属性
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="db"></param>
        /// <param name="ed"></param>
        /// <param name="listInsert"></param>
        public static void UpdateBlkKeys(MySqlConnection conn, Database db, Editor ed, string tablename, List<FASJiSuanshuExcel> listInsert)
        {
            //主函数
            try
            {
                //引入trans
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    //在CAD中自动选取防火分区信息的块
                    //图框对应的块名存放于此
                    List<string> listTKblk = new List<string>() { "RD-防火分区信息1" };

                    //筛选条件:k块名
                    string blkTKNames = ARXTools.GetAnonymousBlk(db, listTKblk);

                    //防火分区框对应属性块的筛选条件
                    TypedValue[] acTypValAr_info = new TypedValue[3];
                    acTypValAr_info.SetValue(new TypedValue((int)DxfCode.BlockName, blkTKNames), 0);  //块名
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
                            int IdKey = Common.MySQLTools.FasSQLGetKey(conn, Id_Dianjing, tablename);

                            //筛选出与电井编号相同的块
                            List<ObjectId> listObjIds = (from d in id_Info
                                                         where d.GetAttributeInBlockReference("电井/防火分区") == Id_Dianjing
                                                         select d).ToList();
                            //当主键有效且属性块唯一时进行修改
                            if ((IdKey > 1000) && (listObjIds.Count == 1))
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
                                Dictionary<string, string> atts = new Dictionary<string, string>
                                {
                                    { "KEY", IdKey.ToString() }
                                };
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
                                    NUM_EXIST = 0
                                };
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
                        if (AREA_EXIST_LIST.Count + AREA_NOTFOUND_LIST.Count > 0)
                        {
                            MessageBox.Show(STR_EXIST + "\n" + STR_NOTFOUND);
                        }


                        //提交事务
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


        /// <summary>
        /// 函数功能:插入一个防火分区广播及防火门的图块
        /// </summary>
        /// <param name="db"></param>
        /// <param name="row_Insert"></param>
        /// <param name="ptStart"></param>
        /// <param name="purpose"></param>
        public static void Insert_FasRuoDianblk1(Database db, FasRDClass row_Insert, Point3d ptStart, string purpose)
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
                //插入广播
                if (purpose == "广播")
                {
                    //增加校验条件:此防火分区内的设备数>0时添加
                    if (row_Insert.GuangBo > 0)
                    {
                        Dictionary<string, string> atts = new Dictionary<string, string>
                        {
                            { "电井编号", row_Insert.Id_Dianjing.ToString() },
                            { "IDKEY", row_Insert.IdKey.ToString() },
                            { "FLOOR1", row_Insert.Floor1.ToString() },
                            { "广播", row_Insert.GuangBo.ToString() }
                        };
                        spaceid.InsertBlockReference(sInsertLayer, "FAS-广播1", ptStart, InsertScale, 0, atts);
                    } // end of if >0
                }
                //插入防火门
                else if (purpose == "防火门")
                {
                    //增加校验条件:此防火分区内的设备数>0时添加
                    if (row_Insert.GuangBo > 0)
                    {
                        Dictionary<string, string> atts = new Dictionary<string, string>
                        {
                            { "电井编号", row_Insert.Id_Dianjing.ToString() },
                            { "IDKEY", row_Insert.IdKey.ToString() },
                            { "FLOOR1", row_Insert.Floor1.ToString() },
                            { "常闭门", row_Insert.RD.ToString() },
                            { "常开门", row_Insert.RDK.ToString() }
                        };
                        spaceid.InsertBlockReference(sInsertLayer, "FAS-防火门系统图1", ptStart, InsertScale, 0, atts);
                    } // end of if >0
                }


                //调试用的报错函数
                else
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("\n程序设计错误,未找到所需流程函数");
                }
                #endregion
            }


        }
        //插入块1结束

        //

        //

        /// <summary>
        /// 将DataGridView中的选中行,转为List<FASJiSuanshuExcel>
        /// </summary>
        /// <param name="uiDG_All"></param>
        /// <returns></returns>
        public static List<FasRDClass> UIDGSecToFasRDClassList(UIDataGridView uiDG_All)
        {
            //新建类
            List<FasRDClass> list_Xuanzhong = new List<FasRDClass>();
            for (int i = 0; i < uiDG_All.RowCount; i++)
            {

                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)uiDG_All.Rows[i].Cells[0];
                Boolean flag = Convert.ToBoolean(checkCell.Value);
                if (flag == true)     //查找被选择的数据行  
                {
                    //新建FAS类,并将数据逐格填入
                    FasRDClass fas = new FasRDClass
                    {

                        IdKey = uiDG_All.Rows[i].Cells["IdKey"].Value.ToString().ToInt(),  //获取IdKey
                        Id_Dianjing = uiDG_All.Rows[i].Cells["Id_Dianjing"].Value.ToString(),
                        Floor1 = uiDG_All.Rows[i].Cells["Floor1"].Value.ToString(),
                        Floor2 = uiDG_All.Rows[i].Cells["Floor2"].Value.ToString().ToInt(),
                        GuangBo = uiDG_All.Rows[i].Cells["GuangBo"].Value.ToString().ToInt(),
                        RD = uiDG_All.Rows[i].Cells["RD"].Value.ToString().ToInt(),
                        RDK = uiDG_All.Rows[i].Cells["RDK"].Value.ToString().ToInt(),

                    }; //end of fas
                    list_Xuanzhong.Add(fas);
                } //end of if
            } //end of for

            //将list数据进行重排
            List<FasRDClass> list_Paixu = list_Xuanzhong.OrderBy(d => d.Floor2).ThenBy(d => Common.MyTools.GetFenquHao(d.Id_Dianjing)).ToList();
            return list_Paixu;

        } //end of UIDGSecToFasRDClassList

    } //end of class
} // end of namespace
