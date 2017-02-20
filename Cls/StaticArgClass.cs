using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinHost;

namespace CJPrj.Cls
{
    public class StaticArgClass
    {
        public static Type type = typeof(WASPCNTrancate); //获取同名函数的方法，先获取该类的type
        public static object typeObj = Activator.CreateInstance(type); //创建类的方法对象
    }
}
