using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeakCurrent1.Common
{
    public class SQLiteConn
    {


        // 静态类用于管理数据库连接信息
        public static string ConnDBName;


        //连接到SQLite数据库
        //暂时没有使用到这个方法
        public static System.Data.SQLite.SQLiteConnection ConnSQLite()
        {
            try
            {
                string folderPath = @"D:\Mycode\Database\FAS\"; // 替换为实际文件夹路径
                string fullFilePath = Path.Combine(folderPath, ConnDBName); // 获取选中的文件的完整路径
                string ConnStr = $"Data Source={fullFilePath};Version=3;"; // 更新数据库连接字符串

                //string connectionString = @"Data Source=D:\Mycode\Database\DYPD\DYPD1001.db;Version=3;";
                System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(ConnStr);
                conn.Open();
                return conn;
            }
            catch (System.Data.SQLite.SQLiteException ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 连接到存放启动信息的SQLite数据库
        /// </summary>
        /// <returns></returns>
        public static System.Data.SQLite.SQLiteConnection DefualtSQLiteName()
        {
            // 存放启动信息数据库的地址
            string connstr = @"Data Source=D:\Mycode\Database\Default\FASDEFAULT.db;Version=3;";
            // 打开数据库连接
            try
            {
                var conn = new System.Data.SQLite.SQLiteConnection(connstr);
                conn.Open();
                return conn;
            }
            catch (System.Data.SQLite.SQLiteException ex)
            {
                throw ex;
            }
        }

        public static string FasSQLGetDBName(string dwg_Name)
        {
            using (var conn = SQLiteConn.DefualtSQLiteName())
            {
                string value;
                if (true) //预留If
                {

                    //SQL查询指令
                    string sql1 = "SELECT dbname FROM currentdb WHERE  dwgname=@string2";
                    using (var cmd = new SQLiteCommand(sql1, conn))
                    {
                        //查询当前防火分区对应的主键
                        cmd.Parameters.AddWithValue("@string2", dwg_Name);
                        object result = cmd.ExecuteScalar();

                        // 判断result是否为null的正确方式
                        if (result != null)
                        {
                            // 进一步判断是否为DBNull（数据库中的NULL值）
                            if (result != DBNull.Value)
                            {
                                value = result.ToString();
                            }
                            else
                            {
                                // 处理数据库返回NULL的情况
                                value = string.Empty; // 或者其他默认值
                            }
                        }
                        else
                        {
                            // 处理result为null的情况
                            value = string.Empty; // 或者其他默认值

                        }
                    }
                } // end of if true
                  //关闭数据库连接
                return value;
            } //end of using

        } // end of GetKey

        /// <summary>
        /// 查询SQLITE数据库中是否存在指定的DWG文件名
        /// </summary>
        /// <param name="dwg_Name"></param>
        /// <returns></returns>
        public static bool FasSQLGetIfExistDwg(string dwg_Name)
        {
            using (var conn = SQLiteConn.DefualtSQLiteName())
            {

                string sql = "SELECT COUNT(*) FROM currentdb WHERE dwgname = @dwgName";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@dwgName", dwg_Name);
                    object result = cmd.ExecuteScalar();

                    // 处理可能的空值情况
                    if (result == null || result == DBNull.Value)
                    {
                        return false;
                    }

                    int count = Convert.ToInt32(result);
                    if(count == 0)
                    {
                        return false;
                    }
                    return true;
                }

            } //end of using

        } // end of GetKey


        public static string FasSQLSetAndUpdataDBName(string dwg_Name,string db_Name)
        {
            using (var conn = SQLiteConn.DefualtSQLiteName())
            {

                string value;

                // 如果数据库中存在名称,则更新，否则插入新的记录
                if(!FasSQLGetIfExistDwg(dwg_Name) && !string.IsNullOrWhiteSpace(db_Name))
                {
                    //SQL插入指令
                    string sql1 = "INSERT INTO currentdb (dwgname, dbname) VALUES (@string1, @string2)";
                    using (var cmd = new SQLiteCommand(sql1, conn))
                    {
                        //插入当前防火分区对应的主键
                        cmd.Parameters.AddWithValue("@string1", dwg_Name);
                        cmd.Parameters.AddWithValue("@string2", db_Name);
                        cmd.ExecuteNonQuery();
                    }
                    value = db_Name; // 返回新插入的数据库名称
                }
                else if(FasSQLGetIfExistDwg(dwg_Name) && !string.IsNullOrWhiteSpace(db_Name))
                {
                    //SQL更新指令
                    string sql2 = "UPDATE currentdb SET dbname=@string2 WHERE dwgname=@string1";
                    using (var cmd = new SQLiteCommand(sql2, conn))
                    {
                        //更新当前防火分区对应的主键
                        cmd.Parameters.AddWithValue("@string1", dwg_Name);
                        cmd.Parameters.AddWithValue("@string2", db_Name);
                        cmd.ExecuteNonQuery();
                    }
                    value = db_Name; // 返回更新后的数据库名称
                }
                else
                {
                    // 如果没有提供有效的数据库名称，则返回空字符串或其他默认值
                    value = string.Empty;
                    UIMessageBox.ShowError("未提供有效的数据库名称。请检查输入。");
                }

                //关闭数据库连接
                return value;
            } //end of using

        } // end of GetKey



    }
}
