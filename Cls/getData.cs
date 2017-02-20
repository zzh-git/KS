using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace CJPrj
{
    class getData
    {//获取数据(目前是模拟以opc client方式输入的数据)
        /*public static DataTable getInData(string sFlag,DataTable dtBase)
        {
            if (sFlag == "0")//采集并计算数据，不返回
            {
                for (int i = 0; i < dtBase.Rows.Count; i++)
                {
                    Random tagV = new Random();
                    double _tagV = Convert.ToDouble(tagV.Next(Convert.ToInt32(dtBase.Rows[i]["down"]), Convert.ToInt32(dtBase.Rows[i]["up"])));
                    dtBase.Rows[i]["value"] = Convert.ToDouble(dtBase.Rows[i]["value"]) + _tagV;
                }
            }
            else
            {
                for (int i = 0; i < dtBase.Rows.Count; i++)
                {
                    dtBase.Rows[i]["value"] = Convert.ToDouble(dtBase.Rows[i]["value"]) / 5;
                }
                    //存储数据
                //saveData.saveTagData(dtBase);
            }
            return dtBase;
        }*/

        public static DataTable getInData(DataTable dtBase)
        {
            //if (sFlag == "0")//采集并计算数据，不返回
            //{
                for (int i = 0; i < dtBase.Rows.Count; i++)
                {
                    Random tagV = new Random();
                    double _tagV = Convert.ToDouble(tagV.Next(Convert.ToInt32(dtBase.Rows[i]["down"]), Convert.ToInt32(dtBase.Rows[i]["up"])));
                    //dtBase.Rows[i]["value"] = Convert.ToDouble(dtBase.Rows[i]["value"]) + _tagV;
                    dtBase.Rows[i]["value"] = _tagV;
                }
                
            //}
            /*else
            {
                for (int i = 0; i < dtBase.Rows.Count; i++)
                {
                    dtBase.Rows[i]["value"] = Convert.ToDouble(dtBase.Rows[i]["value"]) / 5;
                }
                //存储数据
                //saveData.saveTagData(dtBase);
            }*/
            return dtBase;
        }
    }
}
