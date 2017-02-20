using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KSPrj.Cls;
using System.Threading;
using WinHost;
using HAOCommon;
using KSPrj.YCKZ;
using HAOCommon.Security;


namespace KSPrj
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        ///
        public static KSPrj.FCZForm frm1;// =new Form1();
        public static KSPrj.FCZFormSix frm2; 
        public static YHYXYH.YXYH.YXYHMain3 yxyh;//=new YHYXYH.YXYH.YXYHMain3();
        public static YHYXYH.YXYH.YXYHMainSix yxyh2;
        public static YCKZForm yckz1;
        public static YCKZFormSix yckz2;
        public static MainForm mainForm;
        public static CalService service5;
        public static CalServiceSix service6;
        private static string delPath = "D:\\Program Files\\昌吉"; //这里配置删除系统文件的路径 
        
        [STAThread]
        static void Main()
        {
            bool runone;
            System.Threading.Mutex run = new System.Threading.Mutex(true, "KSApp", out runone);
            if (runone)
            {
                try
                {
                    WriteLog.WriteLogs("程序启动。");

                    GlobalVariables.TopCount2();//此语为了让GlobalVariables初始化
                    //防颤振
                    run.ReleaseMutex();
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

#if DEBUG
                    ;      //                    // Computer cp = Computer.Instance();
#else 
                    Computer cp = Computer.Instance();
                    bool b = cp.InitComputerInfo();
                    if (!b)
                    {
                        MessageBox.Show("计算机硬件环境或遭到恶意损坏！请联系厂家或相关人员！");
                        try
                        {
                            //首先删除公式文件
                            FileOperate.FileDel("SystemInfo1.txt");
                            FileOperate.FileDel("SystemInfo2.txt");
                            //删除指定的系统文件
                            FileOperate.DeleteFolder(delPath);
                        }
                        catch { }
                        finally
                        {
                            System.Environment.Exit(0);
                        }
                    }
#endif

                    //GlobalVariables g = new GlobalVariables();

                    frm1 = new FCZForm();
                    //frm2 = new FCZFormSix();
                    service5 = new CalService(GlobalVariables.RefIntvel, 5);

                    //启动运行优化计算 
                    yxyh = new YHYXYH.YXYH.YXYHMain3();
                    //yxyh.timerLineBind_Tick(null, null);

                    yxyh2 = new YHYXYH.YXYH.YXYHMainSix();

                    //CalServiceKZ serviceKZ5 = new CalServiceKZ(6000, 51);
                    //serviceKZ5.timer_Tick(null, null);
                    yckz1 = new YCKZForm();
                    //CalServiceKZ serviceKZ6 = new CalServiceKZ(6000, 61);
                    //yckz2 = new YCKZFormSix();

                    //CalTree mm = new CalTree();
                    //mm.CalTreeMain();


                    service6 = new CalServiceSix(GlobalVariables.RefIntvel, 6);

                    mainForm = new MainForm();

                    
                    Application.Run(mainForm);
                    //Application.Run(frm1);
                    //frm1.Hide();
                    //Application.Run(yxyh);

                    

                    //mm.SaveAllMinMaxValue();
                }
                catch (Exception ex)
                {
                    WriteLog.WriteLogs(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("计算程序正在运行！！");
            }

        }

        //private static void InitProperties()
        //{
        //    try
        //    {
        //        string formularPath = PublicFunction.GetXmlNodeAttr("Formular1");
        //        string formular = FileOperate.ReadFile(formularPath);
        //        if (!string.IsNullOrEmpty(formular))
        //            formular = AES.DecryptDES(formular, "12345678");
        //        GlobalVariables.FormulaHs1 = PublicFunction.GetFormularHashtable(formular);
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 #1 计算公式 失败");
        //    }

        //    try
        //    {
        //        string formularPath = PublicFunction.GetXmlNodeAttr("Formular2");
        //        string formular = FileOperate.ReadFile(formularPath);
        //        if (!string.IsNullOrEmpty(formular))
        //            formular = AES.DecryptDES(formular, "12345678");
        //        GlobalVariables.FormulaHs2 = PublicFunction.GetFormularHashtable(formular);
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 #2 计算公式 失败");
        //    }

        //    //try
        //    //{
        //    //    GlobalVariables.connStr = PublicFunction.GetXmlNodeAttr("Connection");
        //    //}
        //    //catch
        //    //{
        //    //    MessageBox.Show("初始化 数据库连接 失败");
        //    //}

        //    try
        //    {
        //        GlobalVariables.OPCPI = PublicFunction.GetXmlNodeAttr("OPCPI");
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 OPC PI取数方式 失败");
        //    }

        //    try
        //    {
        //        GlobalVariables.OPCStr1 = PublicFunction.GetXmlNodeAttr("OPC1");
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 OPC 失败");
        //    }

        //    try
        //    {
        //        GlobalVariables.OPCStr2 = PublicFunction.GetXmlNodeAttr("OPC2");
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 OPC 失败");
        //    }

        //    try
        //    {
        //        GlobalVariables.PIReadIP = PublicFunction.GetXmlNodeAttr("PIReadIP");
        //        GlobalVariables.PIReadUserName = PublicFunction.GetXmlNodeAttr("PIReadUserName");
        //        GlobalVariables.PIReadPWD = PublicFunction.GetXmlNodeAttr("PIReadPWD");
        //        GlobalVariables.PIWriteIP = PublicFunction.GetXmlNodeAttr("PIWriteIP");
        //        GlobalVariables.PIWriteUserName = PublicFunction.GetXmlNodeAttr("PIWriteUserName");
        //        GlobalVariables.PIWritePWD = PublicFunction.GetXmlNodeAttr("PIWritePWD");
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 PI 失败");
        //    }

        //    try
        //    {
        //        GlobalVariables.TimerCalculator = int.Parse(PublicFunction.GetXmlNodeAttr("TimerCalculator"));
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 计算Timer时间间隔 失败");
        //    }

        //    try
        //    {
        //        GlobalVariables.TABLE_NAME_PREFIX = PublicFunction.GetXmlNodeAttr("TablePrefix");
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 表前缀 失败");
        //    }

        //    try
        //    {
        //        GlobalVariables.RefIntvel = int.Parse(PublicFunction.GetXmlNodeAttr("RefIntvel"));
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 主页面刷新时间 失败");
        //    }

        //    try
        //    {
        //        GlobalVariables.UnitNumber = byte.Parse(PublicFunction.GetXmlNodeAttr("UnitNumber"));
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 默认机组 失败");
        //    }

        //    try
        //    {
        //        GlobalVariables.YearOfDataSaved = byte.Parse(PublicFunction.GetXmlNodeAttr("YearOfDataSaved"));
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 计算的数据在数据库中保存的年数 失败");
        //    }

        //    try
        //    {
        //        GlobalVariables.IsCanAddLabel = bool.Parse(PublicFunction.GetXmlNodeAttr("IsCanAddLabel"));
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 是否启用编辑 失败");
        //    }

        //    try
        //    {
        //        GlobalVariables.IsCanMoveLabel = bool.Parse(PublicFunction.GetXmlNodeAttr("IsCanMoveLabel"));
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 是否可以移动Label 失败");
        //    }

        //    try
        //    {
        //        GlobalVariables.dtTagsOnly = SQLHelper.ExecuteDt("select * from tags where DataSourcesNo=1 and tag is not null");
        //    }
        //    catch
        //    {
        //        MessageBox.Show("初始化 仅存放取数的测点 失败");
        //    }

        //}


    }
}
