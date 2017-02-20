using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CJPrj.Cls
{
    /*  * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
   *   文 件 名:CalTree
   *   功能描述:
   *   创 建 人:张传昀
   *   日    期:2014-5-20
   *   * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
    internal class ExtendPressMath
    {
        //-------------------------
        private static Regex inBracket = new Regex(@"\(([0-9\+\-\*\/\.\^]+)\)");
        private static Regex twoNumberPow = new Regex(@"\(?(-?\d+(\.\d+)?)\)?([\^])\(?(-?\d+(\.\d+)?)\)?");
        private static Regex twoNumberMD = new Regex(@"\(?(-?\d+(\.\d+)?)\)?([\*\/])\(?(-?\d+(\.\d+)?)\)?");
        private static Regex twoNumberAE = new Regex(@"\(?(-?\d+(\.\d+)?)\)?([+-])\(?(-?\d+(\.\d+)?)\)?");
        private static Regex snRegex = new Regex(@"(-?\d+(\.\d+)?[Ee]\d+)");
        private IList<FunctionReflect> functions = new List<FunctionReflect>();
        private delegate string runFunction(string[] args);
        #region 自定义函数的函数体
        //private string runFunctionCos(string[] args)
        //{
        //    return Math.Cos(Convert.ToDouble(args[0])).ToString();
        //}
        //private string runFunctionTan(string[] args)
        //{
        //    return Math.Tan(Convert.ToDouble(args[0])).ToString();
        //}

        //在构造函数里调用函数注册的方法，在计算时的调用去掉（每次计算都添加，致命的重复内存分配（错误），程序内存会无限制增长！），add by hlt 2015-8-7
        public ExtendPressMath()
        {
            functionRegxRegister();
        }
        private string runFunctionSqrt(string[] args)
        {
            return Math.Sqrt(Convert.ToDouble(args[0])).ToString();
        }
        private string runFunctionAbs(string[] args)
        {
            return Math.Abs(Convert.ToDouble(args[0])).ToString();
        }
        #endregion
        #region 内部类，定义自定义方法两个参数：1、正则表达式或者方法名的；2、委托方法
        private class FunctionReflect
        {
            public FunctionReflect(Regex regx, runFunction runFun)
            {
                this.funRegex = regx;
                this.funDelegate = runFun;
            }

            public FunctionReflect(string funname, runFunction runFun)
            {
                this.funRegex = buildFunctionRegx(funname);
                this.funDelegate = runFun;
            }
            public Regex funRegex { get; set; }
            public runFunction funDelegate { get; set; }

            private Regex buildFunctionRegx(string funName)
            {
                string regex = funName + @"\(([0-9\+\-\*\/\.\^\(\)]+?)\)";
                return new Regex(regex);
            }
        }
        #endregion
        //自定义函数的注册与添加
        private void functionRegxRegister()
        {
            FunctionReflect funRef = null;
            //funRef = new FunctionReflect("cos", runFunctionCos);
            //functions.Add(funRef);
            //funRef = new FunctionReflect("tan", runFunctionTan);
            //functions.Add(funRef);
            funRef = new FunctionReflect("sqrt", runFunctionSqrt);
            functions.Add(funRef);
            funRef = new FunctionReflect("Abs", runFunctionAbs);
            functions.Add(funRef);
        }
        //判断表达式中是否含有自定义函数
        private bool hasFunction(string exp)
        {
            bool result = false;
            foreach (FunctionReflect fr in functions)
            {
                if (fr.funRegex.Match(exp).Success)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        private string calcFunction(string exp)
        {
            Match m = null;
            string express = exp;

            while (true)
            {
                if (!hasFunction(express))
                    break;

                foreach (FunctionReflect fr in functions)
                {
                    while (true)
                    {
                        m = fr.funRegex.Match(express);
                        if (m.Success)
                        {
                            string repExp = m.Groups[0].Value;
                            string[] calcExp = m.Groups[1].Value.Split(',');
                            IList<string> args = new List<string>();
                            foreach (string param in calcExp)
                            {
                                args.Add(CalcExpress(param));
                            }
                            string result = fr.funDelegate(args.ToArray());
                            express = express.Replace(repExp, result);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return express;
        }
        private string calcTwoNumber(string left, string oper, string right)
        {
            double leftValue = Convert.ToDouble(left);
            double rightValue = Convert.ToDouble(right);
            switch (oper)
            {
                case "+":
                    return (leftValue + rightValue).ToString();
                case "-":
                    return (leftValue - rightValue).ToString();
                case "*":
                    return (leftValue*rightValue).ToString();
                case "/":
                    return (leftValue/rightValue).ToString();
                case "^":
                    return Math.Pow(leftValue, rightValue).ToString();
                default:
                    return string.Empty;
            }
        }
        //private string calcTwoNumberSingle(string left, string oper, string right)
        //{
        //    switch (oper)
        //    {
        //        case "+":
        //            return (Convert.ToSingle(left) + Convert.ToSingle(right)).ToString();
        //        case "-":
        //            return (Convert.ToSingle(left) - Convert.ToSingle(right)).ToString();
        //        case "*":
        //            return (Convert.ToSingle(left)*Convert.ToSingle(right)).ToString();
        //        case "/":
        //            return (Convert.ToSingle(left)/Convert.ToSingle(right)).ToString();
        //        default:
        //            return string.Empty;
        //    }
        //}
        //转换科学计算法计数的参数
        private string snToNormal(string sn)
        {
            sn = sn.ToLower().Trim();
            string[] temp = sn.Split('e');
            double l = Convert.ToDouble(temp[0]);
            double r = Convert.ToDouble(temp[1]);
            string result = (Math.Pow(10, r)*l).ToString();
            return result;
        }
        //判断是否有科学计算法的书写方式
        public string snReplace(string exp)
        {
            string express = exp.Trim();
            Match m = snRegex.Match(express);
            while (m.Success)
            {
                string sn = m.Groups[0].Value;
                express = express.Replace(sn, snToNormal(sn));
                m = m.NextMatch();
            }
            return express;
        }
        /**
         * 计算两个数字加减乘除
         * 参数为X+Y类似的表达式
         */
        #region
        private string calcExpressNoBracket(String exp)
        {
            Match m = null;
            string express = exp;

            operationReplace(ref m, ref express, twoNumberPow);
            operationReplace(ref m, ref express, twoNumberMD);
            operationReplace(ref m, ref express, twoNumberAE);

            return express;

        }
        private void operationReplace(ref Match m, ref string express, Regex reg)
        {
            while (true)
            {
                m = reg.Match(express);
                if (m.Success)
                {
                    express = calcReplace(m, express);
                }
                else
                {
                    break;
                }

            }
        }
        private string calcReplace(Match m, string express)
        {
            string twoNumberExp = m.Groups[0].Value;
            string leftValue = m.Groups[1].Value;
            string operatorStr = m.Groups[3].Value;
            string rightValue = m.Groups[4].Value;
            string result = calcTwoNumber(leftValue, operatorStr, rightValue);
            express = express.Replace(twoNumberExp, result);
            return express;
        }
        #endregion
        //去括号，并计算括号中数据
        private string clearBracket(string exp)
        {
            Match m = null;
            string express = exp;
            while (true)
            {
                m = inBracket.Match(express);
                if (m.Success)
                    express = express.Replace(m.Groups[0].Value, calcExpressNoBracket(m.Groups[1].Value));
                else
                    break;
            }
            return express;

        }
        public string CalcExpress(string exp)
        {
            string express = exp;
            //去括号
            express = clearBracket(express);
            return calcExpressNoBracket(express);
        }
        //对外的最终方法。
        public string runExpress(string exp)
        {
            //每次计算都添加，致命的重复内存分配（错误），程序内存会无限制增长！edit by hlt 2015-8-1
            //functionRegxRegister();

            string express = exp.Trim();
            //转换科学计数法
            express = snReplace(express);
            //去函数
            express = calcFunction(express);
            return CalcExpress(express);
        }
        //--------------------------
    }
}
