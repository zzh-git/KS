using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CJPrj.Cls;
using WinHost;
using HAOCache;

namespace CJPrj
{
    public class DQY
    {
        //求大气压力
        public static double GetDQY(DataTable dt)
        {
            DataRow[] drDQ= dt.Select("tag='1MG80CP101'");//大气压力
            double dqy=Convert.ToDouble(drDQ[0]["tagvalue"])+0.35;
            return dqy;
        }
        public static double PreChangeValue(int colNum)
        {
            double dqy = Convert.ToDouble(CalTree.CalDataTable.Rows[0]["F"+colNum.ToString()].ToString());
            if (Convert.ToDouble(dqy) == GlobalVariables.ReplaceValue)
                {
                    DataRow[] dtrarr = GlobalVariables.dtTagsSet.Select("ExcelCell='F" + colNum + "'");
                    if (string.IsNullOrEmpty(dtrarr[0]["setvalue"].ToString()))
                    {
                        return GlobalVariables.ReplaceValue;
                    }
                    dqy = Convert.ToDouble(dtrarr[0]["setvalue"]);
                }
            return dqy;
        }
    }
}
