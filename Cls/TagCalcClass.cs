using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WinHost; 

namespace KSPrj
{
    //计算点数据计算，并存储
    public class TagCalcClass
    {
        //临界值时的实时理论排汽流量值（吨/小时）
        public static double PqLl(DataTable dt)
        {

            DataRow[] dr1 = dt.Select("tag='1MAB90CP101'");//低压缸进汽压力A（表压）
            DataRow[] dr2 = dt.Select("tag='1MAB90CP102'");//低压缸进汽压力B（表压）
            DataRow[] drDQ = dt.Select("tag='1MG80CP101'");//大气压力

            //低压缸进汽压力为两个点取平均值
            double abs1 = Convert.ToDouble(dr1[0]["tagvalue"]) + Convert.ToDouble(drDQ[0]["tagvalue"]);//绝对压力
            double abs2 = Convert.ToDouble(dr2[0]["tagvalue"]) + Convert.ToDouble(drDQ[0]["tagvalue"]);//绝对压力
            double dou_jqyl = (abs1 + abs2) / 2000;

            double douRet = 862.84 * dou_jqyl + 124.1;

            //保存数据
            DataRow dr = dt.NewRow();//tag,tagdesc,up,down,0.00 as value,unit 
            dr["tag"] = "Calc_pqll";
            dr["tagdesc"] = "预估排气流量";
            dr["tagvalue"] = douRet.ToString();
            dr["tagunit"] = "t/h";
            DataTable dt1=dt.Clone();
            dt1.Rows.Add(dr.ItemArray);

            DateTime dtSt = Convert.ToDateTime(dt.Rows[0]["time"]);
            //saveData.saveCalcData(dt1, dtSt);
            return douRet;
        }

        //临界值时理论排汽的容积流量值（立方米/小时）
        public static double PqRjLl(DataTable dt)
        {
            double D184 = 220;
            double D182 = GetD182();
            double D181 = GetD181();
            double douRet = D184 * D182 / D181;


            //保存数据
            DataRow dr = dt.NewRow();//tag,tagdesc,up,down,0.00 as value,unit 
            dr["tag"] = "Calc_PqRjLl";
            dr["tagdesc"] = "临界值时理论排汽的容积流量值";
            dr["tagvalue"] = douRet.ToString();
            dr["tagunit"] = "t/h";
            DataTable dt1 = dt.Clone();
            dt1.Rows.Add(dr.ItemArray);

            DateTime dtSt = Convert.ToDateTime(dt.Rows[0]["time"]);
            //saveData.saveCalcData(dt1, dtSt);
            return douRet;
        }

        public static double GetD182()
        {
            double dVRet = 0;
            int iRange = 0;
            WASPCN.P2VG(0.028, ref dVRet, ref iRange);
            return dVRet;
        }

        public static double GetD181()
        {
            double dVRet = 0;
            int iRange = 0;
            WASPCN.P2VG(0.033, ref dVRet, ref iRange);
            return dVRet;
        }
    }
}
