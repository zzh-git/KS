using System;
using System.Runtime.InteropServices;


/*
   根据IFC67和IAPWS-IF84以及IAPWS-IF97编写的动态链接库的函数的调用接口声明
    专供Microsoft Visual C# .Net (C#语言) 使用

    使用方法：

    直接把本软件包提供的类文件(WASPCN.cs)加进对应项目，
    在需要调用函数的单元中引用本类文件的命名空间(using WASPCN)即可！

    把类文件(WASPCN.cs)包括进入项目的菜单路径为：
        [项目(P)]|[添加现有项(G) Shift+Alt+A]

    在需要调用函数的单元中引用本类文件的命名空间(using WASPCN)时无菜单路径，需手工输入！
*/

namespace WASPCN
{
	public class WASPCN
	{
		//动态连接库中函数的声明
		//C#中的声明

		//设定将要使用的标准
		[DllImport("WASPCN",EntryPoint="SETSTD_WASP")]
		public static extern void SETSTD_WASP(int STDID);

		//获知当前使用的标准
		[DllImport("WASPCN",EntryPoint="GETSTD_WASP")]
		public static extern void GETSTD_WASP(ref int STDID);

		//关于窗口
		[DllImport("WASPCN",EntryPoint="ABOUT_WASP")]
		public static extern void ABOUT_WASP();

		//帮助窗口
		[DllImport("WASPCN",EntryPoint="HELP_WASP")]
		public static extern void HELP_WASP();

		//版权窗口
		[DllImport("WASPCN",EntryPoint="COPYRIGHT_WASP")]
		public static extern void COPYRIGHT_WASP();

		//已知压力(MPa)，求对应饱和温度(℃)
		[DllImport("WASPCN",EntryPoint="P2T")]
		public static extern void P2T(double P,ref double T,ref int Range);
		//已知压力(MPa)，求对应饱和水比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="P2HL")]
		public static extern void P2HL(double P,ref double H,ref int Range);
		//已知压力(MPa)，求对应饱和汽比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="P2HG")]
		public static extern void P2HG(double P,ref double H,ref int Range);
		//已知压力(MPa)，求对应饱和水比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2SL")]
		public static extern void P2SL(double P,ref double S,ref int Range);
		//已知压力(MPa)，求对应饱和汽比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2SG")]
		public static extern void P2SG(double P,ref double S,ref int Range);
		//已知压力(MPa)，求对应饱和水比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="P2VL")]
		public static extern void P2VL(double P,ref double V,ref int Range);
		//已知压力(MPa)，求对应饱和汽比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="P2VL")]
		public static extern void P2VG(double P,ref double V,ref int Range);
		//已知压力(MPa)，求对应饱和温度(℃)、饱和水比焓(kJ/kg)、饱和水比熵(kJ/(kg.℃))、饱和水比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="P2L")]
		public static extern void P2L(double P,ref double T,ref double H,ref double S,ref double V,ref double X,ref int Range);
		//已知压力(MPa)，求对应饱和温度(℃)、饱和汽比焓(kJ/kg)、饱和汽比熵(kJ/(kg.℃))、饱和汽比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="P2G")]
		public static extern void P2G(double P,ref double T,ref double H,ref double S,ref double V,ref double X,ref int Range);
		//已知压力(MPa)，求对应饱和水定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2CPL")]
		public static extern void P2CPL(double P,ref double CP,ref int Range);
		//已知压力(MPa)，求对应饱和汽定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2CPG")]
		public static extern void P2CPG(double P,ref double CP,ref int Range);
		//已知压力(MPa)，求对应饱和水定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2CVL")]
		public static extern void P2CVL(double P,ref double CV,ref int Range);
		//已知压力(MPa)，求对应饱和汽定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2CVG")]
		public static extern void P2CVG(double P,ref double CV,ref int Range);
		//已知压力(MPa)，求对应饱和水内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="P2EL")]
		public static extern void P2EL(double P,ref double E,ref int Range);
		//已知压力(MPa)，求对应饱和汽内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="P2EG")]
		public static extern void P2EG(double P,ref double E,ref int Range);
		//已知压力(MPa)，求对应饱和水音速(m/s)
		[DllImport("WASPCN",EntryPoint="P2SSPL")]
		public static extern void P2SSPL(double P,ref double SSP,ref int Range);
		//已知压力(MPa)，求对应饱和汽音速(m/s)
		[DllImport("WASPCN",EntryPoint="P2SSPG")]
		public static extern void P2SSPG(double P,ref double SSP,ref int Range);
		//已知压力(MPa)，求对应饱和水定熵指数
		[DllImport("WASPCN",EntryPoint="P2KSL")]
		public static extern void P2KSL(double P,ref double KS,ref int Range);
		//已知压力(MPa)，求对应饱和汽定熵指数
		[DllImport("WASPCN",EntryPoint="P2KSG")]
		public static extern void P2KSG(double P,ref double KS,ref int Range);
		//已知压力(MPa)，求对应饱和水动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="P2ETAL")]
		public static extern void P2ETAL(double P,ref double ETA,ref int Range);
		//已知压力(MPa)，求对应饱和汽动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="P2ETAG")]
		public static extern void P2ETAG(double P,ref double ETA,ref int Range);
		//已知压力(MPa)，求对应饱和水运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="P2UL")]
		public static extern void P2UL(double P,ref double U,ref int Range);
		//已知压力(MPa)，求对应饱和汽运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="P2UG")]
		public static extern void P2UG(double P,ref double U,ref int Range);
		//已知压力(MPa)，求对应饱和水导热系数(W/(m.℃))
		[DllImport("WASPCN",EntryPoint="P2RAMDL")]
		public static extern void P2RAMDL(double P,ref double RAMD,ref int Range);
		//已知压力(MPa)，求对应饱和汽导热系数(W/(m.℃))
		[DllImport("WASPCN",EntryPoint="P2RAMDG")]
		public static extern void P2RAMDG(double P,ref double RAMD,ref int Range);
		//已知压力(MPa)，求对应饱和水普朗特数
		[DllImport("WASPCN",EntryPoint="P2PRNL")]
		public static extern void P2PRNL(double P,ref double PRN,ref int Range);
		//已知压力(MPa)，求对应饱和汽普朗特数
		[DllImport("WASPCN",EntryPoint="P2PRNG")]
		public static extern void P2PRNG(double P,ref double PRN,ref int Range);
		//已知压力(MPa)，求对应饱和水介电常数
		[DllImport("WASPCN",EntryPoint="P2EPSL")]
		public static extern void P2EPSL(double P,ref double EPS,ref int Range);
		//已知压力(MPa)，求对应饱和汽介电常数
		[DllImport("WASPCN",EntryPoint="P2EPSG")]
		public static extern void P2EPSG(double P,ref double EPS,ref int Range);
		//已知压力(MPa)，求对应饱和水折射率
		[DllImport("WASPCN",EntryPoint="P2NL")]
		public static extern void P2NL(double P,double LAMD,ref double N,ref int Range);
		//已知压力(MPa)，求对应饱和汽折射率
		[DllImport("WASPCN",EntryPoint="P2NG")]
		public static extern void P2NG(double P,double LAMD,ref double N,ref int Range);

		//已知压力(MPa)和温度(℃)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PT2H")]
		public static extern void PT2H(double P,double T,ref double H,ref int Range);
		//已知压力(MPa)和温度(℃)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PT2S")]
		public static extern void PT2S(double P,double T,ref double S,ref int Range);
		//已知压力(MPa)和温度(℃)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PT2V")]
		public static extern void PT2V(double P,double T,ref double V,ref int Range);
		//已知压力(MPa)和温度(℃)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="PT2X")]
		public static extern void PT2X(double P,double T,ref double X,ref int Range);
		//已知压力(MPa)和温度(℃)，求比焓(kJ/kg)、比熵(kJ/(kg.℃))、比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PT")]
		public static extern void PT(double P,double T,ref double H,ref double S,ref double V,ref double X,ref int Range);
		//已知压力(MPa)和温度(℃)，求定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PT2CP")]
		public static extern void PT2CP(double P,double T,ref double CP,ref int Range);
		//已知压力(MPa)和温度(℃)，求定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PT2CV")]
		public static extern void PT2CV(double P,double T,ref double CV,ref int Range);
		//已知压力(MPa)和温度(℃)，求内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PT2E")]
		public static extern void PT2E(double P,double T,ref double E,ref int Range);
		//已知压力(MPa)和温度(℃)，求音速(m/s)
		[DllImport("WASPCN",EntryPoint="PT2SSP")]
		public static extern void PT2SSP(double P,double T,ref double SSP,ref int Range);
		//已知压力(MPa)和温度(℃)，求定熵指数
		[DllImport("WASPCN",EntryPoint="PT2KS")]
		public static extern void PT2KS(double P,double T,ref double KS,ref int Range);
		//已知压力(MPa)和温度(℃)，求动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="PT2ETA")]
		public static extern void PT2ETA(double P,double T,ref double ETA,ref int Range);
		//已知压力(MPa)和温度(℃)，求运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="PT2U")]
		public static extern void PT2U(double P,double T,ref double U,ref int Range);
		//已知压力(MPa)和温度(℃)，求热传导系数 (W/(m.℃))
		[DllImport("WASPCN",EntryPoint="PT2RAMD")]
		public static extern void PT2RAMD(double P,double T,ref double RAMD,ref int Range);
		//已知压力(MPa)和温度(℃)，求普朗特数
		[DllImport("WASPCN",EntryPoint="PT2PRN")]
		public static extern void PT2PRN(double P,double T,ref double PRN,ref int Range);
		//已知压力(MPa)和温度(℃)，求介电常数
		[DllImport("WASPCN",EntryPoint="PT2EPS")]
		public static extern void PT2EPS(double P,double T,ref double EPS,ref int Range);
		//已知压力(MPa)和温度(℃)，求折射率
		[DllImport("WASPCN",EntryPoint="PT2N")]
		public static extern void PT2N(double P,double T,double LAMD,ref double N,ref int Range);

		//已知压力(MPa)和比焓(kJ/kg)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="PH2T")]
		public static extern void PH2T(double P,double H,ref double T,ref int Range);
		//已知压力(MPa)和比焓(kJ/kg)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PH2S")]
		public static extern void PH2S(double P,double H,ref double S,ref int Range);
		//已知压力(MPa)和比焓(kJ/kg)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PH2V")]
		public static extern void PH2V(double P,double H,ref double V,ref int Range);
		//已知压力(MPa)和比焓(kJ/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="PH2X")]
		public static extern void PH2X(double P,double H,ref double X,ref int Range);
		//已知压力(MPa)和比焓(kJ/kg)，求温度(℃)、比熵(kJ/(kg.℃))、比容(m^3/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="PH")]
		public static extern void PH(double P,ref double T,double H,ref double S,ref double V,ref double X,ref int Range);

		//已知压力(MPa)和比熵(kJ/(kg.℃))，求温度(℃)
		[DllImport("WASPCN",EntryPoint="PS2T")]
		public static extern void PS2T(double P,double S,ref double T,ref int Range);
		//已知压力(MPa)和比熵(kJ/(kg.℃))，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PS2H")]
		public static extern void PS2H(double P,double S,ref double H,ref int Range);
		//已知压力(MPa)和比熵(kJ/(kg.℃))，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PS2V")]
		public static extern void PS2V(double P,double S,ref double V,ref int Range);
		//已知压力(MPa)和比熵(kJ/(kg.℃))，求干度(100%)
		[DllImport("WASPCN",EntryPoint="PS2TX7")]
		public static extern void PS2X(double P,double S,ref double X,ref int Range);
		//已知压力(MPa)和比熵(kJ/(kg.℃))，求温度(℃)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="PS")]
		public static extern void PS(double P,ref double T,ref double H,double S,ref double V,ref double X,ref int Range);

		//已知压力(MPa)和比容(m^3/kg)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="PV2T")]
		public static extern void PV2T(double P,double V,ref double T,ref int Range);
		//已知压力(MPa)和比容(m^3/kg)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PV2H")]
		public static extern void PV2H(double P,double V,ref double H,ref int Range);
		//已知压力(MPa)和比容(m^3/kg)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PV2S")]
		public static extern void PV2S(double P,double V,ref double S,ref int Range);
		//已知压力(MPa)和比容(m^3/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="PV2X")]
		public static extern void PV2X(double P,double V,ref double X,ref int Range);
		//已知压力(MPa)和比容(m^3/kg)，求温度(℃)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="PV")]
		public static extern void PV(double P,ref double T,ref double H,ref double S,double V,ref double X,ref int Range);

		//已知压力(MPa)和干度(100%)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="PX2T")]
		public static extern void PX2T(double P,double X,ref double T,ref int Range);
		//已知压力(MPa)和干度(100%)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PX2H")]
		public static extern void PX2H(double P,double X,ref double H,ref int Range);
		//已知压力(MPa)和干度(100%)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PX2S")]
		public static extern void PX2S(double P,double X,ref double S,ref int Range);
		//已知压力(MPa)和干度(100%)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PX2V")]
		public static extern void PX2V(double P,double X,ref double V,ref int Range);
		//已知压力(MPa)和干度(100%)，求温度(℃)、比焓(kJ/kg)、比熵(kJ/(kg.℃))、比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PX")]
		public static extern void PX(double P,ref double T,ref double H,ref double S,ref double V,double X,ref int Range);

		//已知温度(℃)，求饱和压力(MPa)？
		[DllImport("WASPCN",EntryPoint="T2P")]
		public static extern void T2P(double T,ref double P,ref int Range);
		//已知温度(℃)，求饱和水比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="T2HL")]
		public static extern void T2HL(double T,ref double H,ref int Range);
		//已知温度(℃)，求饱和汽比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="T2HG")]
		public static extern void T2HG(double T,ref double H,ref int Range);
		//已知温度(℃)，求饱和水比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2SL")]
		public static extern void T2SL(double T,ref double S,ref int Range);
		//已知温度(℃)，求饱和汽比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2SG")]
		public static extern void T2SG(double T,ref double S,ref int Range);
		//已知温度(℃)，求饱和水比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="T2VL")]
		public static extern void T2VL(double T,ref double V,ref int Range);
		//已知温度(℃)，求饱和汽比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="T2VG")]
		public static extern void T2VG(double T,ref double V,ref int Range);
		//已知温度(℃)，求饱和水比焓(kJ/kg)、饱和水比熵(kJ/(kg.℃))、饱和水比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="T2L")]
		public static extern void T2L(ref double P,double T,ref double V,ref int Range);
		//已知温度(℃)，求饱和汽比焓(kJ/kg)、饱和汽比熵(kJ/(kg.℃))、饱和汽比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="T2G")]
		public static extern void T2G(ref double P,double T,ref double V,ref int Range);
		//已知温度(℃)，求饱和水定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2CPL")]
		public static extern void T2CPL(double T,ref double CP,ref int Range);
		//已知温度(℃)，求饱和汽定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2CPL")]
		public static extern void T2CPG(double T,ref double CP,ref int Range);
		//已知温度(℃)，求饱和水定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2CVL")]
		public static extern void T2CVL(double T,ref double CV,ref int Range);
		//已知温度(℃)，求饱和汽定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2CVG")]
		public static extern void T2CVG(double T,ref double CV,ref int Range);
		//已知温度(℃)，求饱和水内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="T2EL")]
		public static extern void T2EL(double T,ref double E,ref int Range);
		//已知温度(℃)，求饱和汽内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="T2EG")]
		public static extern void T2EG(double T,ref double E,ref int Range);
		//已知温度(℃)，求饱和水音速(m/s)
		[DllImport("WASPCN",EntryPoint="T2SSPL")]
		public static extern void T2SSPL(double T,ref double SSP,ref int Range);
		//已知温度(℃)，求饱和汽音速(m/s)
		[DllImport("WASPCN",EntryPoint="T2SSPG")]
		public static extern void T2SSPG(double T,ref double SSP,ref int Range);
		//已知温度(℃)，求饱和水定熵指数
		[DllImport("WASPCN",EntryPoint="T2KSL")]
		public static extern void T2KSL(double T,ref double KS,ref int Range);
		//已知温度(℃)，求饱和汽定熵指数
		[DllImport("WASPCN",EntryPoint="T2KSG")]
		public static extern void T2KSG(double T,ref double KS,ref int Range);
		//已知温度(℃)，求饱和水动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="T2ETAL")]
		public static extern void T2ETAL(double T,ref double ETA,ref int Range);
		//已知温度(℃)，求饱和汽动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="T2ETAG")]
		public static extern void T2ETAG(double T,ref double ETA,ref int Range);
		//已知温度(℃)，求饱和水运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="T2UL")]
		public static extern void T2UL(double T,ref double U,ref int Range);
		//已知温度(℃)，求饱和汽运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="T2UG")]
		public static extern void T2UG(double T,ref double U,ref int Range);
		//已知温度(℃)，求饱和水导热系数(W/(m.℃))
		[DllImport("WASPCN",EntryPoint="T2RAMDL")]
		public static extern void T2RAMDL(double T,ref double RAMD,ref int Range);
		//已知温度(℃)，求饱和汽导热系数(W/(m.℃))
		[DllImport("WASPCN",EntryPoint="T2RAMDG")]
		public static extern void T2RAMDG(double T,ref double RAMD,ref int Range);
		//已知温度(℃)，求饱和水普朗特数
		[DllImport("WASPCN",EntryPoint="T2PRNL")]
		public static extern void T2PRNL(double T,ref double PRN,ref int Range);
		//已知温度(℃)，求饱和汽普朗特数
		[DllImport("WASPCN",EntryPoint="T2PRNG")]
		public static extern void T2PRNG(double T,ref double PRN,ref int Range);
		//已知温度(℃)，求饱和水介电常数
		[DllImport("WASPCN",EntryPoint="T2EPSL")]
		public static extern void T2EPSL(double T,ref double EPS,ref int Range);
		//已知温度(℃)，求饱和汽介电常数
		[DllImport("WASPCN",EntryPoint="T2EPSG")]
		public static extern void T2EPSG(double T,ref double EPS,ref int Range);
		//已知温度(℃)，求饱和水折射率
		[DllImport("WASPCN",EntryPoint="T2NL")]
		public static extern void T2NL(double T,double LAMD,ref double N,ref int Range);
		//已知温度(℃)，求饱和汽折射率
		[DllImport("WASPCN",EntryPoint="T2NG")]
		public static extern void T2NG(double T,double LAMD,ref double N,ref int Range);
		//已知温度(℃)，求饱和水表面张力(N/m)
		[DllImport("WASPCN",EntryPoint="T2SURFT")]
		public static extern void T2SURFT(double T,ref double SURFT,ref int Range);

		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2PLP")]
		public static extern void TH2PLP(double T,double H,ref double P,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比熵(kJ/(kg.℃))(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2SLP")]
		public static extern void TH2SLP(double T,double H,ref double S,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2VLP")]
		public static extern void TH2VLP(double T,double H,ref double V,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2PHP")]
		public static extern void TH2PHP(double T,double H,ref double P,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比熵(kJ/(kg.℃))(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2SHP")]
		public static extern void TH2SHP(double T,double H,ref double S,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2VHP")]
		public static extern void TH2VHP(double T,double H,ref double V,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)、比熵(kJ/(kg.℃))、比容(m^3/kg)、干度(100%)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="THLP")]
		public static extern void THLP(ref double P,double T,double H,ref double S,ref double V,ref double X,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)、比熵(kJ/(kg.℃))、比容(m^3/kg)、干度(100%)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="THHP")]
		public static extern void THHP(ref double P,double T,double H,ref double S,ref double V,ref double X,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2P")]
		public static extern void TH2P(double T,double H,ref double P,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比熵(kJ/(kg.℃))(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2S")]
		public static extern void TH2S(double T,double H,ref double S,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比容(m^3/kg)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2V")]
		public static extern void TH2V(double T,double H,ref double V,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="TH2X")]
		public static extern void TH2X(double T,double H,ref double X,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)、比熵(kJ/(kg.℃))、比容(m^3/kg)、干度(100%)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH")]
		public static extern void TH(ref double P,double T,double H,ref double S,ref double V,ref double X,ref int Range);

		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2PLP")]
		public static extern void TS2PLP(double T,double S,ref double P,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比焓(kJ/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2HLP")]
		public static extern void TS2HLP(double T,double S,ref double H,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2VLP")]
		public static extern void TS2VLP(double T,double S,ref double V,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2PHP")]
		public static extern void TS2PHP(double T,double S,ref double P,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比焓(kJ/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2HLH")]
		public static extern void TS2HHP(double T,double S,ref double H,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2VHP")]
		public static extern void TS2VHP(double T,double S,ref double V,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2P")]
		public static extern void TS2P(double T,double S,ref double P,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比焓(kJ/kg)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2H")]
		public static extern void TS2H(double T,double S,ref double H,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比容(m^3/kg)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2V")]
		public static extern void TS2V(double T,double S,ref double V,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求干度(100%)
		[DllImport("WASPCN",EntryPoint="TS2X")]
		public static extern void TS2X(double T,double S,ref double X,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TSLP")]
		public static extern void TSLP(ref double P,double T,ref double H,double S,ref double V,ref double X,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TSHP")]
		public static extern void TSHP(ref double P,double T,ref double H,double S,ref double V,ref double X,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS")]
		public static extern void TS(ref double P,double T,ref double H,double S,ref double V,ref double X,ref int Range);

		//已知温度(℃)和比容(m^3/kg)，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="TV2P")]
		public static extern void TV2P(double T,double V,ref double P,ref int Range);
		//已知温度(℃)和比容(m^3/kg)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="TV2H")]
		public static extern void TV2H(double T,double V,ref double H,ref int Range);
		//已知温度(℃)和比容(m^3/kg)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="TV2S")]
		public static extern void TV2S(double T,double V,ref double S,ref int Range);
		//已知温度(℃)和比容(m^3/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="TV2X")]
		public static extern void TV2X(double T,double V,ref double X,ref int Range);
		//已知温度(℃)和比容(m^3/kg)，求压力(MPa)、比焓(kJ/kg)、比熵(kJ/(kg.℃))、干度(100%)
		[DllImport("WASPCN",EntryPoint="TV")]
		public static extern void TV(ref double P,double T,ref double H,ref double S,double V,ref double X,ref int Range);

		//已知温度(℃)和干度(100%)，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="TX2P")]
		public static extern void TX2P(double T,double X,ref double P,ref int Range);
		//已知温度(℃)和干度(100%)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="TX2H")]
		public static extern void TX2H(double T,double X,ref double H,ref int Range);
		//已知温度(℃)和干度(100%)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="TX2S")]
		public static extern void TX2S(double T,double X,ref double S,ref int Range);
		//已知温度(℃)和干度(100%)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="TX2V")]
		public static extern void TX2V(double T,double X,ref double V,ref int Range);
		//已知温度(℃)和干度(100%)，求压力(MPa)、比焓(kJ/kg)、比熵(kJ/(kg.℃))、比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="TX")]
		public static extern void TX(ref double P,double T,ref double H,ref double S,ref double V,double X,ref int Range);

		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="HS2P")]
		public static extern void HS2P(double H,double S,ref double P,ref int Range);
		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求温度(℃)
		[DllImport("WASPCN",EntryPoint="HS2T")]
		public static extern void HS2T(double H,double S,ref double T,ref int Range);
		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="HS2V")]
		public static extern void HS2V(double H,double S,ref double V,ref int Range);
		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求干度(100%)
		[DllImport("WASPCN",EntryPoint="HS2X")]
		public static extern void HS2X(double H,double S,ref double X,ref int Range);
		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求压力(MPa)、温度(℃)、比容(m^3/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="HS")]
		public static extern void HS(ref double P,ref double T,double H,double S,ref double V,ref double X,ref int Range);

		//已知比焓(kJ/kg)和比容(m^3/kg)，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="HV2P")]
		public static extern void HV2P(double H,double V,ref double P,ref int Range);
		//已知比焓(kJ/kg)和比容(m^3/kg)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="HV2T")]
		public static extern void HV2T(double H,double V,ref double T,ref int Range);
		//已知比焓(kJ/kg)和比容(m^3/kg)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="HV2S")]
		public static extern void HV2S(double H,double V,ref double S,ref int Range);
		//已知比焓(kJ/kg)和比容(m^3/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="HV2X")]
		public static extern void HV2X(double H,double V,ref double X,ref int Range);
		//已知比焓(kJ/kg)和比容(m^3/kg)，求压力(MPa)、温度(℃)、比熵(kJ/(kg.℃))、干度(100%)
		[DllImport("WASPCN",EntryPoint="HV")]
		public static extern void HV(ref double P,ref double T,double H,ref double S,double V,ref double X,ref int Range);

		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2PLP")]
		public static extern void HX2PLP(double H,double X,ref double P,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求温度(℃)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2TLP")]
		public static extern void HX2TLP(double H,double X,ref double T,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比熵(kJ/(kg.℃))(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2SLP")]
		public static extern void HX2SLP(double H,double X,ref double S,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2VLP")]
		public static extern void HX2VLP(double H,double X,ref double V,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2PLP")]
		public static extern void HX2PHP(double H,double X,ref double P,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求温度(℃)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2THP")]
		public static extern void HX2THP(double H,double X,ref double T,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比熵(kJ/(kg.℃))(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2SHP")]
		public static extern void HX2SHP(double H,double X,ref double S,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2VHP")]
		public static extern void HX2VHP(double H,double X,ref double V,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2P")]
		public static extern void HX2P(double H,double X,ref double P,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求温度(℃)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2T")]
		public static extern void HX2T(double H,double X,ref double T,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比熵(kJ/(kg.℃))(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2S7")]
		public static extern void HX2S7(double H,double X,ref double S,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比容(m^3/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2V7")]
		public static extern void HX2V7(double H,double X,ref double V,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)、温度(℃)、比熵(kJ/(kg.℃))、比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HXLP")]
		public static extern void HXLP(ref double P,ref double T,double H,ref double S,ref double V,double X,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)、温度(℃)、比熵(kJ/(kg.℃))、比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HXHP")]
		public static extern void HXHP(ref double P,ref double T,double H,ref double S,ref double V,double X,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)、温度(℃)、比熵(kJ/(kg.℃))、比容(m^3/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX")]
		public static extern void HX(ref double P,ref double T,double H,ref double S,ref double V,double X,ref int Range);

		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="SV2P")]
		public static extern void SV2P(double S,double V,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="SV2T")]
		public static extern void SV2T(double S,double V,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="SV2H")]
		public static extern void SV2H(double S,double V,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="SV2X")]
		public static extern void SV2X(double S,double V,ref double X,ref int Range);
		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="SV")]
		public static extern void SV(ref double P,ref double T,ref double H,double S,double V,ref double X,ref int Range);

		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2PLP")]
		public static extern void SX2PLP(double S,double X,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2PMP")]
		public static extern void SX2PMP(double S,double X,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2PHP")]
		public static extern void SX2PHP(double S,double X,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2P")]
		public static extern void SX2P(double S,double X,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求温度(℃)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2TLP")]
		public static extern void SX2TLP(double S,double X,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求温度(℃)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2TMP")]
		public static extern void SX2TMP(double S,double X,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求温度(℃)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2THP")]
		public static extern void SX2THP(double S,double X,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求温度(℃)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2T")]
		public static extern void SX2T(double S,double X,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比焓(kJ/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2HLP")]
		public static extern void SX2HLP(double S,double X,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比焓(kJ/kg)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2HMP")]
		public static extern void SX2HMP(double S,double X,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比焓(kJ/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2HHP")]
		public static extern void SX2HHP(double S,double X,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比焓(kJ/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2H")]
		public static extern void SX2H(double S,double X,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2VLP")]
		public static extern void SX2VLP(double S,double X,ref double V,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比容(m^3/kg)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2VMP")]
		public static extern void SX2VMP(double S,double X,ref double V,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2VHP")]
		public static extern void SX2VHP(double S,double X,ref double V,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比容(m^3/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2V")]
		public static extern void SX2V(double S,double X,ref double V,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SXLP")]
		public static extern void SXLP(ref double P,ref double T,ref double H,double S,ref double V,double X,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比容(m^3/kg)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SXMP")]
		public static extern void SXMP(ref double P,ref double T,ref double H,double S,ref double V,double X,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SXHP")]
		public static extern void SXHP(ref double P,ref double T,ref double H,double S,ref double V,double X,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比容(m^3/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX")]
		public static extern void SX(ref double P,ref double T,ref double H,double S,ref double V,double X,ref int Range);

		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2PLP")]
		public static extern void VX2PLP(double V,double X,ref double P,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)(低高压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2PHP")]
		public static extern void VX2PHP(double V,double X,ref double P,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2P")]
		public static extern void VX2P(double V,double X,ref double P,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求温度(℃)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2TLP")]
		public static extern void VX2TLP(double V,double X,ref double T,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求温度(℃)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2THP")]
		public static extern void VX2THP(double V,double X,ref double T,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求温度(℃)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2T")]
		public static extern void VX2T(double V,double X,ref double T,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比焓(kJ/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2HLP")]
		public static extern void VX2HLP(double V,double X,ref double H,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比焓(kJ/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2HHP")]
		public static extern void VX2HHP(double V,double X,ref double H,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比焓(kJ/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2H")]
		public static extern void VX2H(double V,double X,ref double H,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比熵(kJ/(kg.℃))(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2SLP")]
		public static extern void VX2SLP(double V,double X,ref double S,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比熵(kJ/(kg.℃))(高压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2SHP")]
		public static extern void VX2SHP(double V,double X,ref double S,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比熵(kJ/(kg.℃))(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2S")]
		public static extern void VX2S(double V,double X,ref double S,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比熵(kJ/(kg.℃))(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VXLP")]
		public static extern void VXLP(ref double P,ref double T,ref double H,ref double S,double V,double X,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比熵(kJ/(kg.℃))(高压的一个值)
		[DllImport("WASPCN",EntryPoint="VXHP")]
		public static extern void VXHP(ref double P,ref double T,ref double H,ref double S,double V,double X,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比熵(kJ/(kg.℃))(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX")]
		public static extern void VX(ref double P,ref double T,ref double H,ref double S,double V,double X,ref int Range);

		

		//已知压力(MPa)，求对应饱和温度(℃)
		[DllImport("WASPCN",EntryPoint="P2T97")]
		public static extern void P2T97(double P,ref double T,ref int Range);
		//已知压力(MPa)，求对应饱和水比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="P2HL97")]
		public static extern void P2HL97(double P,ref double H,ref int Range);
		//已知压力(MPa)，求对应饱和汽比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="P2HG97")]
		public static extern void P2HG97(double P,ref double H,ref int Range);
		//已知压力(MPa)，求对应饱和水比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2SL97")]
		public static extern void P2SL97(double P,ref double S,ref int Range);
		//已知压力(MPa)，求对应饱和汽比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2SG97")]
		public static extern void P2SG97(double P,ref double S,ref int Range);
		//已知压力(MPa)，求对应饱和水比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="P2VL97")]
		public static extern void P2VL97(double P,ref double V,ref int Range);
		//已知压力(MPa)，求对应饱和汽比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="P2VL97")]
		public static extern void P2VG97(double P,ref double V,ref int Range);
		//已知压力(MPa)，求对应饱和温度(℃)、饱和水比焓(kJ/kg)、饱和水比熵(kJ/(kg.℃))、饱和水比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="P2L97")]
		public static extern void P2L97(double P,ref double T,ref double H,ref double S,ref double V,ref double X,ref int Range);
		//已知压力(MPa)，求对应饱和温度(℃)、饱和汽比焓(kJ/kg)、饱和汽比熵(kJ/(kg.℃))、饱和汽比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="P2G97")]
		public static extern void P2G97(double P,ref double T,ref double H,ref double S,ref double V,ref double X,ref int Range);
		//已知压力(MPa)，求对应饱和水定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2CPL97")]
		public static extern void P2CPL97(double P,ref double CP,ref int Range);
		//已知压力(MPa)，求对应饱和汽定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2CPG97")]
		public static extern void P2CPG97(double P,ref double CP,ref int Range);
		//已知压力(MPa)，求对应饱和水定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2CVL97")]
		public static extern void P2CVL97(double P,ref double CV,ref int Range);
		//已知压力(MPa)，求对应饱和汽定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2CVG97")]
		public static extern void P2CVG97(double P,ref double CV,ref int Range);
		//已知压力(MPa)，求对应饱和水内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="P2EL97")]
		public static extern void P2EL97(double P,ref double E,ref int Range);
		//已知压力(MPa)，求对应饱和汽内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="P2EG97")]
		public static extern void P2EG97(double P,ref double E,ref int Range);
		//已知压力(MPa)，求对应饱和水音速(m/s)
		[DllImport("WASPCN",EntryPoint="P2SSPL97")]
		public static extern void P2SSPL97(double P,ref double SSP,ref int Range);
		//已知压力(MPa)，求对应饱和汽音速(m/s)
		[DllImport("WASPCN",EntryPoint="P2SSPG97")]
		public static extern void P2SSPG97(double P,ref double SSP,ref int Range);
		//已知压力(MPa)，求对应饱和水定熵指数
		[DllImport("WASPCN",EntryPoint="P2KSL97")]
		public static extern void P2KSL97(double P,ref double KS,ref int Range);
		//已知压力(MPa)，求对应饱和汽定熵指数
		[DllImport("WASPCN",EntryPoint="P2KSG97")]
		public static extern void P2KSG97(double P,ref double KS,ref int Range);
		//已知压力(MPa)，求对应饱和水动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="P2ETAL97")]
		public static extern void P2ETAL97(double P,ref double ETA,ref int Range);
		//已知压力(MPa)，求对应饱和汽动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="P2ETAG97")]
		public static extern void P2ETAG97(double P,ref double ETA,ref int Range);
		//已知压力(MPa)，求对应饱和水运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="P2UL97")]
		public static extern void P2UL97(double P,ref double U,ref int Range);
		//已知压力(MPa)，求对应饱和汽运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="P2UG97")]
		public static extern void P2UG97(double P,ref double U,ref int Range);
		//已知压力(MPa)，求对应饱和水导热系数(W/(m.℃))
		[DllImport("WASPCN",EntryPoint="P2RAMDL97")]
		public static extern void P2RAMDL97(double P,ref double RAMD,ref int Range);
		//已知压力(MPa)，求对应饱和汽导热系数(W/(m.℃))
		[DllImport("WASPCN",EntryPoint="P2RAMDG97")]
		public static extern void P2RAMDG97(double P,ref double RAMD,ref int Range);
		//已知压力(MPa)，求对应饱和水普朗特数
		[DllImport("WASPCN",EntryPoint="P2PRNL97")]
		public static extern void P2PRNL97(double P,ref double PRN,ref int Range);
		//已知压力(MPa)，求对应饱和汽普朗特数
		[DllImport("WASPCN",EntryPoint="P2PRNG97")]
		public static extern void P2PRNG97(double P,ref double PRN,ref int Range);
		//已知压力(MPa)，求对应饱和水介电常数
		[DllImport("WASPCN",EntryPoint="P2EPSL97")]
		public static extern void P2EPSL97(double P,ref double EPS,ref int Range);
		//已知压力(MPa)，求对应饱和汽介电常数
		[DllImport("WASPCN",EntryPoint="P2EPSG97")]
		public static extern void P2EPSG97(double P,ref double EPS,ref int Range);
		//已知压力(MPa)，求对应饱和水折射率
		[DllImport("WASPCN",EntryPoint="P2NL97")]
		public static extern void P2NL97(double P,double LAMD,ref double N,ref int Range);
		//已知压力(MPa)，求对应饱和汽折射率
		[DllImport("WASPCN",EntryPoint="P2NG97")]
		public static extern void P2NG97(double P,double LAMD,ref double N,ref int Range);

		//已知压力(MPa)和温度(℃)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PT2H97")]
		public static extern void PT2H97(double P,double T,ref double H,ref int Range);
		//已知压力(MPa)和温度(℃)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PT2S97")]
		public static extern void PT2S97(double P,double T,ref double S,ref int Range);
		//已知压力(MPa)和温度(℃)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PT2V97")]
		public static extern void PT2V97(double P,double T,ref double V,ref int Range);
		//已知压力(MPa)和温度(℃)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="PT2X97")]
		public static extern void PT2X97(double P,double T,ref double X,ref int Range);
		//已知压力(MPa)和温度(℃)，求比焓(kJ/kg)、比熵(kJ/(kg.℃))、比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PT97")]
		public static extern void PT97(double P,double T,ref double H,ref double S,ref double V,ref double X,ref int Range);
		//已知压力(MPa)和温度(℃)，求定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PT2CP97")]
		public static extern void PT2CP97(double P,double T,ref double CP,ref int Range);
		//已知压力(MPa)和温度(℃)，求定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PT2CV97")]
		public static extern void PT2CV97(double P,double T,ref double CV,ref int Range);
		//已知压力(MPa)和温度(℃)，求内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PT2E97")]
		public static extern void PT2E97(double P,double T,ref double E,ref int Range);
		//已知压力(MPa)和温度(℃)，求音速(m/s)
		[DllImport("WASPCN",EntryPoint="PT2SSP97")]
		public static extern void PT2SSP97(double P,double T,ref double SSP,ref int Range);
		//已知压力(MPa)和温度(℃)，求定熵指数
		[DllImport("WASPCN",EntryPoint="PT2KS97")]
		public static extern void PT2KS97(double P,double T,ref double KS,ref int Range);
		//已知压力(MPa)和温度(℃)，求动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="PT2ETA97")]
		public static extern void PT2ETA97(double P,double T,ref double ETA,ref int Range);
		//已知压力(MPa)和温度(℃)，求运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="PT2U97")]
		public static extern void PT2U97(double P,double T,ref double U,ref int Range);
		//已知压力(MPa)和温度(℃)，求热传导系数 (W/(m.℃))
		[DllImport("WASPCN",EntryPoint="PT2RAMD97")]
		public static extern void PT2RAMD97(double P,double T,ref double RAMD,ref int Range);
		//已知压力(MPa)和温度(℃)，求普朗特数
		[DllImport("WASPCN",EntryPoint="PT2PRN97")]
		public static extern void PT2PRN97(double P,double T,ref double PRN,ref int Range);
		//已知压力(MPa)和温度(℃)，求介电常数
		[DllImport("WASPCN",EntryPoint="PT2EPS97")]
		public static extern void PT2EPS97(double P,double T,ref double EPS,ref int Range);
		//已知压力(MPa)和温度(℃)，求折射率
		[DllImport("WASPCN",EntryPoint="PT2N97")]
		public static extern void PT2N97(double P,double T,double LAMD,ref double N,ref int Range);

		//已知压力(MPa)和比焓(kJ/kg)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="PH2T97")]
		public static extern void PH2T97(double P,double H,ref double T,ref int Range);
		//已知压力(MPa)和比焓(kJ/kg)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PH2S97")]
		public static extern void PH2S97(double P,double H,ref double S,ref int Range);
		//已知压力(MPa)和比焓(kJ/kg)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PH2V97")]
		public static extern void PH2V97(double P,double H,ref double V,ref int Range);
		//已知压力(MPa)和比焓(kJ/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="PH2X97")]
		public static extern void PH2X97(double P,double H,ref double X,ref int Range);
		//已知压力(MPa)和比焓(kJ/kg)，求温度(℃)、比熵(kJ/(kg.℃))、比容(m^3/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="PH97")]
		public static extern void PH97(double P,ref double T,double H,ref double S,ref double V,ref double X,ref int Range);

		//已知压力(MPa)和比熵(kJ/(kg.℃))，求温度(℃)
		[DllImport("WASPCN",EntryPoint="PS2T97")]
		public static extern void PS2T97(double P,double S,ref double T,ref int Range);
		//已知压力(MPa)和比熵(kJ/(kg.℃))，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PS2H97")]
		public static extern void PS2H97(double P,double S,ref double H,ref int Range);
		//已知压力(MPa)和比熵(kJ/(kg.℃))，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PS2V97")]
		public static extern void PS2V97(double P,double S,ref double V,ref int Range);
		//已知压力(MPa)和比熵(kJ/(kg.℃))，求干度(100%)
		[DllImport("WASPCN",EntryPoint="PS2TX7")]
		public static extern void PS2X97(double P,double S,ref double X,ref int Range);
		//已知压力(MPa)和比熵(kJ/(kg.℃))，求温度(℃)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="PS97")]
		public static extern void PS97(double P,ref double T,ref double H,double S,ref double V,ref double X,ref int Range);

		//已知压力(MPa)和比容(m^3/kg)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="PV2T97")]
		public static extern void PV2T97(double P,double V,ref double T,ref int Range);
		//已知压力(MPa)和比容(m^3/kg)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PV2H97")]
		public static extern void PV2H97(double P,double V,ref double H,ref int Range);
		//已知压力(MPa)和比容(m^3/kg)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PV2S97")]
		public static extern void PV2S97(double P,double V,ref double S,ref int Range);
		//已知压力(MPa)和比容(m^3/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="PV2X97")]
		public static extern void PV2X97(double P,double V,ref double X,ref int Range);
		//已知压力(MPa)和比容(m^3/kg)，求温度(℃)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="PV97")]
		public static extern void PV97(double P,ref double T,ref double H,ref double S,double V,ref double X,ref int Range);

		//已知压力(MPa)和干度(100%)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="PX2T97")]
		public static extern void PX2T97(double P,double X,ref double T,ref int Range);
		//已知压力(MPa)和干度(100%)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PX2H97")]
		public static extern void PX2H97(double P,double X,ref double H,ref int Range);
		//已知压力(MPa)和干度(100%)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PX2S97")]
		public static extern void PX2S97(double P,double X,ref double S,ref int Range);
		//已知压力(MPa)和干度(100%)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PX2V97")]
		public static extern void PX2V97(double P,double X,ref double V,ref int Range);
		//已知压力(MPa)和干度(100%)，求温度(℃)、比焓(kJ/kg)、比熵(kJ/(kg.℃))、比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PX97")]
		public static extern void PX97(double P,ref double T,ref double H,ref double S,ref double V,double X,ref int Range);

		//已知温度(℃)，求饱和压力(MPa)？
		[DllImport("WASPCN",EntryPoint="T2P97")]
		public static extern void T2P97(double T,ref double P,ref int Range);
		//已知温度(℃)，求饱和水比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="T2HL97")]
		public static extern void T2HL97(double T,ref double H,ref int Range);
		//已知温度(℃)，求饱和汽比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="T2HG97")]
		public static extern void T2HG97(double T,ref double H,ref int Range);
		//已知温度(℃)，求饱和水比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2SL97")]
		public static extern void T2SL97(double T,ref double S,ref int Range);
		//已知温度(℃)，求饱和汽比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2SG97")]
		public static extern void T2SG97(double T,ref double S,ref int Range);
		//已知温度(℃)，求饱和水比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="T2VL97")]
		public static extern void T2VL97(double T,ref double V,ref int Range);
		//已知温度(℃)，求饱和汽比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="T2VG97")]
		public static extern void T2VG97(double T,ref double V,ref int Range);
		//已知温度(℃)，求饱和水比焓(kJ/kg)、饱和水比熵(kJ/(kg.℃))、饱和水比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="T2L97")]
		public static extern void T2L97(ref double P,double T,ref double V,ref int Range);
		//已知温度(℃)，求饱和汽比焓(kJ/kg)、饱和汽比熵(kJ/(kg.℃))、饱和汽比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="T2G97")]
		public static extern void T2G97(ref double P,double T,ref double V,ref int Range);
		//已知温度(℃)，求饱和水定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2CPL97")]
		public static extern void T2CPL97(double T,ref double CP,ref int Range);
		//已知温度(℃)，求饱和汽定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2CPL97")]
		public static extern void T2CPG97(double T,ref double CP,ref int Range);
		//已知温度(℃)，求饱和水定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2CVL97")]
		public static extern void T2CVL97(double T,ref double CV,ref int Range);
		//已知温度(℃)，求饱和汽定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2CVG97")]
		public static extern void T2CVG97(double T,ref double CV,ref int Range);
		//已知温度(℃)，求饱和水内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="T2EL97")]
		public static extern void T2EL97(double T,ref double E,ref int Range);
		//已知温度(℃)，求饱和汽内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="T2EG97")]
		public static extern void T2EG97(double T,ref double E,ref int Range);
		//已知温度(℃)，求饱和水音速(m/s)
		[DllImport("WASPCN",EntryPoint="T2SSPL97")]
		public static extern void T2SSPL97(double T,ref double SSP,ref int Range);
		//已知温度(℃)，求饱和汽音速(m/s)
		[DllImport("WASPCN",EntryPoint="T2SSPG97")]
		public static extern void T2SSPG97(double T,ref double SSP,ref int Range);
		//已知温度(℃)，求饱和水定熵指数
		[DllImport("WASPCN",EntryPoint="T2KSL97")]
		public static extern void T2KSL97(double T,ref double KS,ref int Range);
		//已知温度(℃)，求饱和汽定熵指数
		[DllImport("WASPCN",EntryPoint="T2KSG97")]
		public static extern void T2KSG97(double T,ref double KS,ref int Range);
		//已知温度(℃)，求饱和水动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="T2ETAL97")]
		public static extern void T2ETAL97(double T,ref double ETA,ref int Range);
		//已知温度(℃)，求饱和汽动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="T2ETAG97")]
		public static extern void T2ETAG97(double T,ref double ETA,ref int Range);
		//已知温度(℃)，求饱和水运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="T2UL97")]
		public static extern void T2UL97(double T,ref double U,ref int Range);
		//已知温度(℃)，求饱和汽运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="T2UG97")]
		public static extern void T2UG97(double T,ref double U,ref int Range);
		//已知温度(℃)，求饱和水导热系数(W/(m.℃))
		[DllImport("WASPCN",EntryPoint="T2RAMDL97")]
		public static extern void T2RAMDL97(double T,ref double RAMD,ref int Range);
		//已知温度(℃)，求饱和汽导热系数(W/(m.℃))
		[DllImport("WASPCN",EntryPoint="T2RAMDG97")]
		public static extern void T2RAMDG97(double T,ref double RAMD,ref int Range);
		//已知温度(℃)，求饱和水普朗特数
		[DllImport("WASPCN",EntryPoint="T2PRNL97")]
		public static extern void T2PRNL97(double T,ref double PRN,ref int Range);
		//已知温度(℃)，求饱和汽普朗特数
		[DllImport("WASPCN",EntryPoint="T2PRNG97")]
		public static extern void T2PRNG97(double T,ref double PRN,ref int Range);
		//已知温度(℃)，求饱和水介电常数
		[DllImport("WASPCN",EntryPoint="T2EPSL97")]
		public static extern void T2EPSL97(double T,ref double EPS,ref int Range);
		//已知温度(℃)，求饱和汽介电常数
		[DllImport("WASPCN",EntryPoint="T2EPSG97")]
		public static extern void T2EPSG97(double T,ref double EPS,ref int Range);
		//已知温度(℃)，求饱和水折射率
		[DllImport("WASPCN",EntryPoint="T2NL97")]
		public static extern void T2NL97(double T,double LAMD,ref double N,ref int Range);
		//已知温度(℃)，求饱和汽折射率
		[DllImport("WASPCN",EntryPoint="T2NG97")]
		public static extern void T2NG97(double T,double LAMD,ref double N,ref int Range);
		//已知温度(℃)，求饱和水表面张力(N/m)
		[DllImport("WASPCN",EntryPoint="T2SURFT97")]
		public static extern void T2SURFT97(double T,ref double SURFT,ref int Range);

		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2PLP97")]
		public static extern void TH2PLP97(double T,double H,ref double P,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比熵(kJ/(kg.℃))(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2SLP97")]
		public static extern void TH2SLP97(double T,double H,ref double S,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2VLP97")]
		public static extern void TH2VLP97(double T,double H,ref double V,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2PHP97")]
		public static extern void TH2PHP97(double T,double H,ref double P,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比熵(kJ/(kg.℃))(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2SHP97")]
		public static extern void TH2SHP97(double T,double H,ref double S,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2VHP97")]
		public static extern void TH2VHP97(double T,double H,ref double V,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)、比熵(kJ/(kg.℃))、比容(m^3/kg)、干度(100%)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="THLP97")]
		public static extern void THLP97(ref double P,double T,double H,ref double S,ref double V,ref double X,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)、比熵(kJ/(kg.℃))、比容(m^3/kg)、干度(100%)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="THHP97")]
		public static extern void THHP97(ref double P,double T,double H,ref double S,ref double V,ref double X,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2P97")]
		public static extern void TH2P97(double T,double H,ref double P,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比熵(kJ/(kg.℃))(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2S97")]
		public static extern void TH2S97(double T,double H,ref double S,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比容(m^3/kg)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2V97")]
		public static extern void TH2V97(double T,double H,ref double V,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="TH2X97")]
		public static extern void TH2X97(double T,double H,ref double X,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)、比熵(kJ/(kg.℃))、比容(m^3/kg)、干度(100%)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH97")]
		public static extern void TH97(ref double P,double T,double H,ref double S,ref double V,ref double X,ref int Range);

		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2PLP97")]
		public static extern void TS2PLP97(double T,double S,ref double P,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比焓(kJ/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2HLP97")]
		public static extern void TS2HLP97(double T,double S,ref double H,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2VLP97")]
		public static extern void TS2VLP97(double T,double S,ref double V,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2PHP97")]
		public static extern void TS2PHP97(double T,double S,ref double P,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比焓(kJ/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2HLH97")]
		public static extern void TS2HHP97(double T,double S,ref double H,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2VHP97")]
		public static extern void TS2VHP97(double T,double S,ref double V,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2P97")]
		public static extern void TS2P97(double T,double S,ref double P,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比焓(kJ/kg)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2H97")]
		public static extern void TS2H97(double T,double S,ref double H,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比容(m^3/kg)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2V97")]
		public static extern void TS2V97(double T,double S,ref double V,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求干度(100%)
		[DllImport("WASPCN",EntryPoint="TS2X97")]
		public static extern void TS2X97(double T,double S,ref double X,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TSLP97")]
		public static extern void TSLP97(ref double P,double T,ref double H,double S,ref double V,ref double X,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TSHP97")]
		public static extern void TSHP97(ref double P,double T,ref double H,double S,ref double V,ref double X,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS97")]
		public static extern void TS97(ref double P,double T,ref double H,double S,ref double V,ref double X,ref int Range);

		//已知温度(℃)和比容(m^3/kg)，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="TV2P97")]
		public static extern void TV2P97(double T,double V,ref double P,ref int Range);
		//已知温度(℃)和比容(m^3/kg)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="TV2H97")]
		public static extern void TV2H97(double T,double V,ref double H,ref int Range);
		//已知温度(℃)和比容(m^3/kg)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="TV2S97")]
		public static extern void TV2S97(double T,double V,ref double S,ref int Range);
		//已知温度(℃)和比容(m^3/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="TV2X97")]
		public static extern void TV2X97(double T,double V,ref double X,ref int Range);
		//已知温度(℃)和比容(m^3/kg)，求压力(MPa)、比焓(kJ/kg)、比熵(kJ/(kg.℃))、干度(100%)
		[DllImport("WASPCN",EntryPoint="TV97")]
		public static extern void TV97(ref double P,double T,ref double H,ref double S,double V,ref double X,ref int Range);

		//已知温度(℃)和干度(100%)，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="TX2P97")]
		public static extern void TX2P97(double T,double X,ref double P,ref int Range);
		//已知温度(℃)和干度(100%)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="TX2H97")]
		public static extern void TX2H97(double T,double X,ref double H,ref int Range);
		//已知温度(℃)和干度(100%)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="TX2S97")]
		public static extern void TX2S97(double T,double X,ref double S,ref int Range);
		//已知温度(℃)和干度(100%)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="TX2V97")]
		public static extern void TX2V97(double T,double X,ref double V,ref int Range);
		//已知温度(℃)和干度(100%)，求压力(MPa)、比焓(kJ/kg)、比熵(kJ/(kg.℃))、比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="TX97")]
		public static extern void TX97(ref double P,double T,ref double H,ref double S,ref double V,double X,ref int Range);

		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="HS2P97")]
		public static extern void HS2P97(double H,double S,ref double P,ref int Range);
		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求温度(℃)
		[DllImport("WASPCN",EntryPoint="HS2T97")]
		public static extern void HS2T97(double H,double S,ref double T,ref int Range);
		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="HS2V97")]
		public static extern void HS2V97(double H,double S,ref double V,ref int Range);
		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求干度(100%)
		[DllImport("WASPCN",EntryPoint="HS2X97")]
		public static extern void HS2X97(double H,double S,ref double X,ref int Range);
		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求压力(MPa)、温度(℃)、比容(m^3/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="HS97")]
		public static extern void HS97(ref double P,ref double T,double H,double S,ref double V,ref double X,ref int Range);

		//已知比焓(kJ/kg)和比容(m^3/kg)，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="HV2P97")]
		public static extern void HV2P97(double H,double V,ref double P,ref int Range);
		//已知比焓(kJ/kg)和比容(m^3/kg)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="HV2T97")]
		public static extern void HV2T97(double H,double V,ref double T,ref int Range);
		//已知比焓(kJ/kg)和比容(m^3/kg)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="HV2S97")]
		public static extern void HV2S97(double H,double V,ref double S,ref int Range);
		//已知比焓(kJ/kg)和比容(m^3/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="HV2X97")]
		public static extern void HV2X97(double H,double V,ref double X,ref int Range);
		//已知比焓(kJ/kg)和比容(m^3/kg)，求压力(MPa)、温度(℃)、比熵(kJ/(kg.℃))、干度(100%)
		[DllImport("WASPCN",EntryPoint="HV97")]
		public static extern void HV97(ref double P,ref double T,double H,ref double S,double V,ref double X,ref int Range);

		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2PLP97")]
		public static extern void HX2PLP97(double H,double X,ref double P,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求温度(℃)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2TLP97")]
		public static extern void HX2TLP97(double H,double X,ref double T,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比熵(kJ/(kg.℃))(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2SLP97")]
		public static extern void HX2SLP97(double H,double X,ref double S,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2VLP97")]
		public static extern void HX2VLP97(double H,double X,ref double V,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2PLP97")]
		public static extern void HX2PHP97(double H,double X,ref double P,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求温度(℃)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2THP97")]
		public static extern void HX2THP97(double H,double X,ref double T,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比熵(kJ/(kg.℃))(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2SHP97")]
		public static extern void HX2SHP97(double H,double X,ref double S,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2VHP97")]
		public static extern void HX2VHP97(double H,double X,ref double V,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2P97")]
		public static extern void HX2P97(double H,double X,ref double P,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求温度(℃)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2T97")]
		public static extern void HX2T97(double H,double X,ref double T,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比熵(kJ/(kg.℃))(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2S97")]
		public static extern void HX2S97(double H,double X,ref double S,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比容(m^3/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2V97")]
		public static extern void HX2V97(double H,double X,ref double V,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)、温度(℃)、比熵(kJ/(kg.℃))、比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HXLP97")]
		public static extern void HXLP97(ref double P,ref double T,double H,ref double S,ref double V,double X,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)、温度(℃)、比熵(kJ/(kg.℃))、比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HXHP97")]
		public static extern void HXHP97(ref double P,ref double T,double H,ref double S,ref double V,double X,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)、温度(℃)、比熵(kJ/(kg.℃))、比容(m^3/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX97")]
		public static extern void HX97(ref double P,ref double T,double H,ref double S,ref double V,double X,ref int Range);

		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="SV2P97")]
		public static extern void SV2P97(double S,double V,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="SV2T97")]
		public static extern void SV2T97(double S,double V,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="SV2H97")]
		public static extern void SV2H97(double S,double V,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="SV2X97")]
		public static extern void SV2X97(double S,double V,ref double X,ref int Range);
		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="SV97")]
		public static extern void SV97(ref double P,ref double T,ref double H,double S,double V,ref double X,ref int Range);

		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2PLP97")]
		public static extern void SX2PLP97(double S,double X,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2PMP97")]
		public static extern void SX2PMP97(double S,double X,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2PHP97")]
		public static extern void SX2PHP97(double S,double X,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2P97")]
		public static extern void SX2P97(double S,double X,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求温度(℃)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2TLP97")]
		public static extern void SX2TLP97(double S,double X,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求温度(℃)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2TMP97")]
		public static extern void SX2TMP97(double S,double X,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求温度(℃)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2THP97")]
		public static extern void SX2THP97(double S,double X,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求温度(℃)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2T97")]
		public static extern void SX2T97(double S,double X,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比焓(kJ/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2HLP97")]
		public static extern void SX2HLP97(double S,double X,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比焓(kJ/kg)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2HMP97")]
		public static extern void SX2HMP97(double S,double X,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比焓(kJ/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2HHP97")]
		public static extern void SX2HHP97(double S,double X,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比焓(kJ/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2H97")]
		public static extern void SX2H97(double S,double X,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2VLP97")]
		public static extern void SX2VLP97(double S,double X,ref double V,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比容(m^3/kg)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2VMP97")]
		public static extern void SX2VMP97(double S,double X,ref double V,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2VHP97")]
		public static extern void SX2VHP97(double S,double X,ref double V,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比容(m^3/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2V97")]
		public static extern void SX2V97(double S,double X,ref double V,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SXLP97")]
		public static extern void SXLP97(ref double P,ref double T,ref double H,double S,ref double V,double X,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比容(m^3/kg)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SXMP97")]
		public static extern void SXMP97(ref double P,ref double T,ref double H,double S,ref double V,double X,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SXHP97")]
		public static extern void SXHP97(ref double P,ref double T,ref double H,double S,ref double V,double X,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比容(m^3/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX97")]
		public static extern void SX97(ref double P,ref double T,ref double H,double S,ref double V,double X,ref int Range);

		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2PLP97")]
		public static extern void VX2PLP97(double V,double X,ref double P,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)(低高压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2PHP97")]
		public static extern void VX2PHP97(double V,double X,ref double P,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2P97")]
		public static extern void VX2P97(double V,double X,ref double P,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求温度(℃)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2TLP97")]
		public static extern void VX2TLP97(double V,double X,ref double T,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求温度(℃)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2THP97")]
		public static extern void VX2THP97(double V,double X,ref double T,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求温度(℃)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2T97")]
		public static extern void VX2T97(double V,double X,ref double T,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比焓(kJ/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2HLP97")]
		public static extern void VX2HLP97(double V,double X,ref double H,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比焓(kJ/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2HHP97")]
		public static extern void VX2HHP97(double V,double X,ref double H,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比焓(kJ/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2H97")]
		public static extern void VX2H97(double V,double X,ref double H,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比熵(kJ/(kg.℃))(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2SLP97")]
		public static extern void VX2SLP97(double V,double X,ref double S,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比熵(kJ/(kg.℃))(高压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2SHP97")]
		public static extern void VX2SHP97(double V,double X,ref double S,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比熵(kJ/(kg.℃))(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2S97")]
		public static extern void VX2S97(double V,double X,ref double S,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比熵(kJ/(kg.℃))(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VXLP97")]
		public static extern void VXLP97(ref double P,ref double T,ref double H,ref double S,double V,double X,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比熵(kJ/(kg.℃))(高压的一个值)
		[DllImport("WASPCN",EntryPoint="VXHP97")]
		public static extern void VXHP97(ref double P,ref double T,ref double H,ref double S,double V,double X,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比熵(kJ/(kg.℃))(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX97")]
		public static extern void VX97(ref double P,ref double T,ref double H,ref double S,double V,double X,ref int Range);

		
		
		//已知压力(MPa)，求对应饱和温度(℃)
		[DllImport("WASPCN",EntryPoint="P2T67")]
		public static extern void P2T67(double P,ref double T,ref int Range);
		//已知压力(MPa)，求对应饱和水比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="P2HL67")]
		public static extern void P2HL67(double P,ref double H,ref int Range);
		//已知压力(MPa)，求对应饱和汽比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="P2HG67")]
		public static extern void P2HG67(double P,ref double H,ref int Range);
		//已知压力(MPa)，求对应饱和水比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2SL67")]
		public static extern void P2SL67(double P,ref double S,ref int Range);
		//已知压力(MPa)，求对应饱和汽比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2SG67")]
		public static extern void P2SG67(double P,ref double S,ref int Range);
		//已知压力(MPa)，求对应饱和水比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="P2VL67")]
		public static extern void P2VL67(double P,ref double V,ref int Range);
		//已知压力(MPa)，求对应饱和汽比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="P2VL67")]
		public static extern void P2VG67(double P,ref double V,ref int Range);
		//已知压力(MPa)，求对应饱和温度(℃)、饱和水比焓(kJ/kg)、饱和水比熵(kJ/(kg.℃))、饱和水比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="P2L67")]
		public static extern void P2L67(double P,ref double T,ref double H,ref double S,ref double V,ref double X,ref int Range);
		//已知压力(MPa)，求对应饱和温度(℃)、饱和汽比焓(kJ/kg)、饱和汽比熵(kJ/(kg.℃))、饱和汽比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="P2G67")]
		public static extern void P2G67(double P,ref double T,ref double H,ref double S,ref double V,ref double X,ref int Range);
		//已知压力(MPa)，求对应饱和水定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2CPL67")]
		public static extern void P2CPL67(double P,ref double CP,ref int Range);
		//已知压力(MPa)，求对应饱和汽定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2CPG67")]
		public static extern void P2CPG67(double P,ref double CP,ref int Range);
		//已知压力(MPa)，求对应饱和水定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2CVL67")]
		public static extern void P2CVL67(double P,ref double CV,ref int Range);
		//已知压力(MPa)，求对应饱和汽定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="P2CVG67")]
		public static extern void P2CVG67(double P,ref double CV,ref int Range);
		//已知压力(MPa)，求对应饱和水内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="P2EL67")]
		public static extern void P2EL67(double P,ref double E,ref int Range);
		//已知压力(MPa)，求对应饱和汽内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="P2EG67")]
		public static extern void P2EG67(double P,ref double E,ref int Range);
		//已知压力(MPa)，求对应饱和水音速(m/s)
		[DllImport("WASPCN",EntryPoint="P2SSPL67")]
		public static extern void P2SSPL67(double P,ref double SSP,ref int Range);
		//已知压力(MPa)，求对应饱和汽音速(m/s)
		[DllImport("WASPCN",EntryPoint="P2SSPG67")]
		public static extern void P2SSPG67(double P,ref double SSP,ref int Range);
		//已知压力(MPa)，求对应饱和水定熵指数
		[DllImport("WASPCN",EntryPoint="P2KSL67")]
		public static extern void P2KSL67(double P,ref double KS,ref int Range);
		//已知压力(MPa)，求对应饱和汽定熵指数
		[DllImport("WASPCN",EntryPoint="P2KSG67")]
		public static extern void P2KSG67(double P,ref double KS,ref int Range);
		//已知压力(MPa)，求对应饱和水动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="P2ETAL67")]
		public static extern void P2ETAL67(double P,ref double ETA,ref int Range);
		//已知压力(MPa)，求对应饱和汽动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="P2ETAG67")]
		public static extern void P2ETAG67(double P,ref double ETA,ref int Range);
		//已知压力(MPa)，求对应饱和水运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="P2UL67")]
		public static extern void P2UL67(double P,ref double U,ref int Range);
		//已知压力(MPa)，求对应饱和汽运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="P2UG67")]
		public static extern void P2UG67(double P,ref double U,ref int Range);
		//已知压力(MPa)，求对应饱和水导热系数(W/(m.℃))
		[DllImport("WASPCN",EntryPoint="P2RAMDL67")]
		public static extern void P2RAMDL67(double P,ref double RAMD,ref int Range);
		//已知压力(MPa)，求对应饱和汽导热系数(W/(m.℃))
		[DllImport("WASPCN",EntryPoint="P2RAMDG67")]
		public static extern void P2RAMDG67(double P,ref double RAMD,ref int Range);
		//已知压力(MPa)，求对应饱和水普朗特数
		[DllImport("WASPCN",EntryPoint="P2PRNL67")]
		public static extern void P2PRNL67(double P,ref double PRN,ref int Range);
		//已知压力(MPa)，求对应饱和汽普朗特数
		[DllImport("WASPCN",EntryPoint="P2PRNG67")]
		public static extern void P2PRNG67(double P,ref double PRN,ref int Range);
		//已知压力(MPa)，求对应饱和水介电常数
		[DllImport("WASPCN",EntryPoint="P2EPSL67")]
		public static extern void P2EPSL67(double P,ref double EPS,ref int Range);
		//已知压力(MPa)，求对应饱和汽介电常数
		[DllImport("WASPCN",EntryPoint="P2EPSG67")]
		public static extern void P2EPSG67(double P,ref double EPS,ref int Range);
		//已知压力(MPa)，求对应饱和水折射率
		[DllImport("WASPCN",EntryPoint="P2NL67")]
		public static extern void P2NL67(double P,double LAMD,ref double N,ref int Range);
		//已知压力(MPa)，求对应饱和汽折射率
		[DllImport("WASPCN",EntryPoint="P2NG67")]
		public static extern void P2NG67(double P,double LAMD,ref double N,ref int Range);

		//已知压力(MPa)和温度(℃)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PT2H67")]
		public static extern void PT2H67(double P,double T,ref double H,ref int Range);
		//已知压力(MPa)和温度(℃)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PT2S67")]
		public static extern void PT2S67(double P,double T,ref double S,ref int Range);
		//已知压力(MPa)和温度(℃)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PT2V67")]
		public static extern void PT2V67(double P,double T,ref double V,ref int Range);
		//已知压力(MPa)和温度(℃)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="PT2X67")]
		public static extern void PT2X67(double P,double T,ref double X,ref int Range);
		//已知压力(MPa)和温度(℃)，求比焓(kJ/kg)、比熵(kJ/(kg.℃))、比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PT67")]
		public static extern void PT67(double P,double T,ref double H,ref double S,ref double V,ref double X,ref int Range);
		//已知压力(MPa)和温度(℃)，求定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PT2CP67")]
		public static extern void PT2CP67(double P,double T,ref double CP,ref int Range);
		//已知压力(MPa)和温度(℃)，求定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PT2CV67")]
		public static extern void PT2CV67(double P,double T,ref double CV,ref int Range);
		//已知压力(MPa)和温度(℃)，求内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PT2E67")]
		public static extern void PT2E67(double P,double T,ref double E,ref int Range);
		//已知压力(MPa)和温度(℃)，求音速(m/s)
		[DllImport("WASPCN",EntryPoint="PT2SSP67")]
		public static extern void PT2SSP67(double P,double T,ref double SSP,ref int Range);
		//已知压力(MPa)和温度(℃)，求定熵指数
		[DllImport("WASPCN",EntryPoint="PT2KS67")]
		public static extern void PT2KS67(double P,double T,ref double KS,ref int Range);
		//已知压力(MPa)和温度(℃)，求动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="PT2ETA67")]
		public static extern void PT2ETA67(double P,double T,ref double ETA,ref int Range);
		//已知压力(MPa)和温度(℃)，求运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="PT2U67")]
		public static extern void PT2U67(double P,double T,ref double U,ref int Range);
		//已知压力(MPa)和温度(℃)，求热传导系数 (W/(m.℃))
		[DllImport("WASPCN",EntryPoint="PT2RAMD67")]
		public static extern void PT2RAMD67(double P,double T,ref double RAMD,ref int Range);
		//已知压力(MPa)和温度(℃)，求普朗特数
		[DllImport("WASPCN",EntryPoint="PT2PRN67")]
		public static extern void PT2PRN67(double P,double T,ref double PRN,ref int Range);
		//已知压力(MPa)和温度(℃)，求介电常数
		[DllImport("WASPCN",EntryPoint="PT2EPS67")]
		public static extern void PT2EPS67(double P,double T,ref double EPS,ref int Range);
		//已知压力(MPa)和温度(℃)，求折射率
		[DllImport("WASPCN",EntryPoint="PT2N67")]
		public static extern void PT2N67(double P,double T,double LAMD,ref double N,ref int Range);

		//已知压力(MPa)和比焓(kJ/kg)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="PH2T67")]
		public static extern void PH2T67(double P,double H,ref double T,ref int Range);
		//已知压力(MPa)和比焓(kJ/kg)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PH2S67")]
		public static extern void PH2S67(double P,double H,ref double S,ref int Range);
		//已知压力(MPa)和比焓(kJ/kg)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PH2V67")]
		public static extern void PH2V67(double P,double H,ref double V,ref int Range);
		//已知压力(MPa)和比焓(kJ/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="PH2X67")]
		public static extern void PH2X67(double P,double H,ref double X,ref int Range);
		//已知压力(MPa)和比焓(kJ/kg)，求温度(℃)、比熵(kJ/(kg.℃))、比容(m^3/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="PH67")]
		public static extern void PH67(double P,ref double T,double H,ref double S,ref double V,ref double X,ref int Range);

		//已知压力(MPa)和比熵(kJ/(kg.℃))，求温度(℃)
		[DllImport("WASPCN",EntryPoint="PS2T67")]
		public static extern void PS2T67(double P,double S,ref double T,ref int Range);
		//已知压力(MPa)和比熵(kJ/(kg.℃))，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PS2H67")]
		public static extern void PS2H67(double P,double S,ref double H,ref int Range);
		//已知压力(MPa)和比熵(kJ/(kg.℃))，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PS2V67")]
		public static extern void PS2V67(double P,double S,ref double V,ref int Range);
		//已知压力(MPa)和比熵(kJ/(kg.℃))，求干度(100%)
		[DllImport("WASPCN",EntryPoint="PS2TX7")]
		public static extern void PS2X67(double P,double S,ref double X,ref int Range);
		//已知压力(MPa)和比熵(kJ/(kg.℃))，求温度(℃)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="PS67")]
		public static extern void PS67(double P,ref double T,ref double H,double S,ref double V,ref double X,ref int Range);

		//已知压力(MPa)和比容(m^3/kg)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="PV2T67")]
		public static extern void PV2T67(double P,double V,ref double T,ref int Range);
		//已知压力(MPa)和比容(m^3/kg)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PV2H67")]
		public static extern void PV2H67(double P,double V,ref double H,ref int Range);
		//已知压力(MPa)和比容(m^3/kg)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PV2S67")]
		public static extern void PV2S67(double P,double V,ref double S,ref int Range);
		//已知压力(MPa)和比容(m^3/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="PV2X67")]
		public static extern void PV2X67(double P,double V,ref double X,ref int Range);
		//已知压力(MPa)和比容(m^3/kg)，求温度(℃)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="PV67")]
		public static extern void PV67(double P,ref double T,ref double H,ref double S,double V,ref double X,ref int Range);

		//已知压力(MPa)和干度(100%)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="PX2T67")]
		public static extern void PX2T67(double P,double X,ref double T,ref int Range);
		//已知压力(MPa)和干度(100%)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="PX2H67")]
		public static extern void PX2H67(double P,double X,ref double H,ref int Range);
		//已知压力(MPa)和干度(100%)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="PX2S67")]
		public static extern void PX2S67(double P,double X,ref double S,ref int Range);
		//已知压力(MPa)和干度(100%)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PX2V67")]
		public static extern void PX2V67(double P,double X,ref double V,ref int Range);
		//已知压力(MPa)和干度(100%)，求温度(℃)、比焓(kJ/kg)、比熵(kJ/(kg.℃))、比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="PX67")]
		public static extern void PX67(double P,ref double T,ref double H,ref double S,ref double V,double X,ref int Range);

		//已知温度(℃)，求饱和压力(MPa)？
		[DllImport("WASPCN",EntryPoint="T2P67")]
		public static extern void T2P67(double T,ref double P,ref int Range);
		//已知温度(℃)，求饱和水比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="T2HL67")]
		public static extern void T2HL67(double T,ref double H,ref int Range);
		//已知温度(℃)，求饱和汽比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="T2HG67")]
		public static extern void T2HG67(double T,ref double H,ref int Range);
		//已知温度(℃)，求饱和水比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2SL67")]
		public static extern void T2SL67(double T,ref double S,ref int Range);
		//已知温度(℃)，求饱和汽比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2SG67")]
		public static extern void T2SG67(double T,ref double S,ref int Range);
		//已知温度(℃)，求饱和水比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="T2VL67")]
		public static extern void T2VL67(double T,ref double V,ref int Range);
		//已知温度(℃)，求饱和汽比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="T2VG67")]
		public static extern void T2VG67(double T,ref double V,ref int Range);
		//已知温度(℃)，求饱和水比焓(kJ/kg)、饱和水比熵(kJ/(kg.℃))、饱和水比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="T2L67")]
		public static extern void T2L67(ref double P,double T,ref double V,ref int Range);
		//已知温度(℃)，求饱和汽比焓(kJ/kg)、饱和汽比熵(kJ/(kg.℃))、饱和汽比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="T2G67")]
		public static extern void T2G67(ref double P,double T,ref double V,ref int Range);
		//已知温度(℃)，求饱和水定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2CPL67")]
		public static extern void T2CPL67(double T,ref double CP,ref int Range);
		//已知温度(℃)，求饱和汽定压比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2CPL67")]
		public static extern void T2CPG67(double T,ref double CP,ref int Range);
		//已知温度(℃)，求饱和水定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2CVL67")]
		public static extern void T2CVL67(double T,ref double CV,ref int Range);
		//已知温度(℃)，求饱和汽定容比热(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="T2CVG67")]
		public static extern void T2CVG67(double T,ref double CV,ref int Range);
		//已知温度(℃)，求饱和水内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="T2EL67")]
		public static extern void T2EL67(double T,ref double E,ref int Range);
		//已知温度(℃)，求饱和汽内能(kJ/kg)
		[DllImport("WASPCN",EntryPoint="T2EG67")]
		public static extern void T2EG67(double T,ref double E,ref int Range);
		//已知温度(℃)，求饱和水音速(m/s)
		[DllImport("WASPCN",EntryPoint="T2SSPL67")]
		public static extern void T2SSPL67(double T,ref double SSP,ref int Range);
		//已知温度(℃)，求饱和汽音速(m/s)
		[DllImport("WASPCN",EntryPoint="T2SSPG67")]
		public static extern void T2SSPG67(double T,ref double SSP,ref int Range);
		//已知温度(℃)，求饱和水定熵指数
		[DllImport("WASPCN",EntryPoint="T2KSL67")]
		public static extern void T2KSL67(double T,ref double KS,ref int Range);
		//已知温度(℃)，求饱和汽定熵指数
		[DllImport("WASPCN",EntryPoint="T2KSG67")]
		public static extern void T2KSG67(double T,ref double KS,ref int Range);
		//已知温度(℃)，求饱和水动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="T2ETAL67")]
		public static extern void T2ETAL67(double T,ref double ETA,ref int Range);
		//已知温度(℃)，求饱和汽动力粘度(Pa.s)
		[DllImport("WASPCN",EntryPoint="T2ETAG67")]
		public static extern void T2ETAG67(double T,ref double ETA,ref int Range);
		//已知温度(℃)，求饱和水运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="T2UL67")]
		public static extern void T2UL67(double T,ref double U,ref int Range);
		//已知温度(℃)，求饱和汽运动粘度(m^2/s)
		[DllImport("WASPCN",EntryPoint="T2UG67")]
		public static extern void T2UG67(double T,ref double U,ref int Range);
		//已知温度(℃)，求饱和水导热系数(W/(m.℃))
		[DllImport("WASPCN",EntryPoint="T2RAMDL67")]
		public static extern void T2RAMDL67(double T,ref double RAMD,ref int Range);
		//已知温度(℃)，求饱和汽导热系数(W/(m.℃))
		[DllImport("WASPCN",EntryPoint="T2RAMDG67")]
		public static extern void T2RAMDG67(double T,ref double RAMD,ref int Range);
		//已知温度(℃)，求饱和水普朗特数
		[DllImport("WASPCN",EntryPoint="T2PRNL67")]
		public static extern void T2PRNL67(double T,ref double PRN,ref int Range);
		//已知温度(℃)，求饱和汽普朗特数
		[DllImport("WASPCN",EntryPoint="T2PRNG67")]
		public static extern void T2PRNG67(double T,ref double PRN,ref int Range);
		//已知温度(℃)，求饱和水介电常数
		[DllImport("WASPCN",EntryPoint="T2EPSL67")]
		public static extern void T2EPSL67(double T,ref double EPS,ref int Range);
		//已知温度(℃)，求饱和汽介电常数
		[DllImport("WASPCN",EntryPoint="T2EPSG67")]
		public static extern void T2EPSG67(double T,ref double EPS,ref int Range);
		//已知温度(℃)，求饱和水折射率
		[DllImport("WASPCN",EntryPoint="T2NL67")]
		public static extern void T2NL67(double T,double LAMD,ref double N,ref int Range);
		//已知温度(℃)，求饱和汽折射率
		[DllImport("WASPCN",EntryPoint="T2NG67")]
		public static extern void T2NG67(double T,double LAMD,ref double N,ref int Range);
		//已知温度(℃)，求饱和水表面张力(N/m)
		[DllImport("WASPCN",EntryPoint="T2SURFT67")]
		public static extern void T2SURFT67(double T,ref double SURFT,ref int Range);

		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2PLP67")]
		public static extern void TH2PLP67(double T,double H,ref double P,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比熵(kJ/(kg.℃))(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2SLP67")]
		public static extern void TH2SLP67(double T,double H,ref double S,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2VLP67")]
		public static extern void TH2VLP67(double T,double H,ref double V,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2PHP67")]
		public static extern void TH2PHP67(double T,double H,ref double P,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比熵(kJ/(kg.℃))(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2SHP67")]
		public static extern void TH2SHP67(double T,double H,ref double S,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2VHP67")]
		public static extern void TH2VHP67(double T,double H,ref double V,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)、比熵(kJ/(kg.℃))、比容(m^3/kg)、干度(100%)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="THLP67")]
		public static extern void THLP67(ref double P,double T,double H,ref double S,ref double V,ref double X,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)、比熵(kJ/(kg.℃))、比容(m^3/kg)、干度(100%)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="THHP67")]
		public static extern void THHP67(ref double P,double T,double H,ref double S,ref double V,ref double X,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2P67")]
		public static extern void TH2P67(double T,double H,ref double P,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比熵(kJ/(kg.℃))(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2S67")]
		public static extern void TH2S67(double T,double H,ref double S,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求比容(m^3/kg)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH2V67")]
		public static extern void TH2V67(double T,double H,ref double V,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="TH2X67")]
		public static extern void TH2X67(double T,double H,ref double X,ref int Range);
		//已知温度(℃)和比焓(kJ/kg)，求压力(MPa)、比熵(kJ/(kg.℃))、比容(m^3/kg)、干度(100%)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TH67")]
		public static extern void TH67(ref double P,double T,double H,ref double S,ref double V,ref double X,ref int Range);

		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2PLP67")]
		public static extern void TS2PLP67(double T,double S,ref double P,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比焓(kJ/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2HLP67")]
		public static extern void TS2HLP67(double T,double S,ref double H,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2VLP67")]
		public static extern void TS2VLP67(double T,double S,ref double V,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2PHP67")]
		public static extern void TS2PHP67(double T,double S,ref double P,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比焓(kJ/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2HLH67")]
		public static extern void TS2HHP67(double T,double S,ref double H,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2VHP67")]
		public static extern void TS2VHP67(double T,double S,ref double V,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2P67")]
		public static extern void TS2P67(double T,double S,ref double P,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比焓(kJ/kg)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2H67")]
		public static extern void TS2H67(double T,double S,ref double H,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求比容(m^3/kg)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS2V67")]
		public static extern void TS2V67(double T,double S,ref double V,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求干度(100%)
		[DllImport("WASPCN",EntryPoint="TS2X67")]
		public static extern void TS2X67(double T,double S,ref double X,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="TSLP67")]
		public static extern void TSLP67(ref double P,double T,ref double H,double S,ref double V,ref double X,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="TSHP67")]
		public static extern void TSHP67(ref double P,double T,ref double H,double S,ref double V,ref double X,ref int Range);
		//已知温度(℃)和比熵(kJ/(kg.℃))，求压力(MPa)、比焓(kJ/kg)、比容(m^3/kg)、干度(100%)(缺省为低压的一个值)
		[DllImport("WASPCN",EntryPoint="TS67")]
		public static extern void TS67(ref double P,double T,ref double H,double S,ref double V,ref double X,ref int Range);

		//已知温度(℃)和比容(m^3/kg)，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="TV2P67")]
		public static extern void TV2P67(double T,double V,ref double P,ref int Range);
		//已知温度(℃)和比容(m^3/kg)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="TV2H67")]
		public static extern void TV2H67(double T,double V,ref double H,ref int Range);
		//已知温度(℃)和比容(m^3/kg)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="TV2S67")]
		public static extern void TV2S67(double T,double V,ref double S,ref int Range);
		//已知温度(℃)和比容(m^3/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="TV2X67")]
		public static extern void TV2X67(double T,double V,ref double X,ref int Range);
		//已知温度(℃)和比容(m^3/kg)，求压力(MPa)、比焓(kJ/kg)、比熵(kJ/(kg.℃))、干度(100%)
		[DllImport("WASPCN",EntryPoint="TV67")]
		public static extern void TV67(ref double P,double T,ref double H,ref double S,double V,ref double X,ref int Range);

		//已知温度(℃)和干度(100%)，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="TX2P67")]
		public static extern void TX2P67(double T,double X,ref double P,ref int Range);
		//已知温度(℃)和干度(100%)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="TX2H67")]
		public static extern void TX2H67(double T,double X,ref double H,ref int Range);
		//已知温度(℃)和干度(100%)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="TX2S67")]
		public static extern void TX2S67(double T,double X,ref double S,ref int Range);
		//已知温度(℃)和干度(100%)，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="TX2V67")]
		public static extern void TX2V67(double T,double X,ref double V,ref int Range);
		//已知温度(℃)和干度(100%)，求压力(MPa)、比焓(kJ/kg)、比熵(kJ/(kg.℃))、比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="TX67")]
		public static extern void TX67(ref double P,double T,ref double H,ref double S,ref double V,double X,ref int Range);

		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="HS2P67")]
		public static extern void HS2P67(double H,double S,ref double P,ref int Range);
		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求温度(℃)
		[DllImport("WASPCN",EntryPoint="HS2T67")]
		public static extern void HS2T67(double H,double S,ref double T,ref int Range);
		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求比容(m^3/kg)
		[DllImport("WASPCN",EntryPoint="HS2V67")]
		public static extern void HS2V67(double H,double S,ref double V,ref int Range);
		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求干度(100%)
		[DllImport("WASPCN",EntryPoint="HS2X67")]
		public static extern void HS2X67(double H,double S,ref double X,ref int Range);
		//已知比焓(kJ/kg)和比熵(kJ/(kg.℃))，求压力(MPa)、温度(℃)、比容(m^3/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="HS67")]
		public static extern void HS67(ref double P,ref double T,double H,double S,ref double V,ref double X,ref int Range);

		//已知比焓(kJ/kg)和比容(m^3/kg)，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="HV2P67")]
		public static extern void HV2P67(double H,double V,ref double P,ref int Range);
		//已知比焓(kJ/kg)和比容(m^3/kg)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="HV2T67")]
		public static extern void HV2T67(double H,double V,ref double T,ref int Range);
		//已知比焓(kJ/kg)和比容(m^3/kg)，求比熵(kJ/(kg.℃))
		[DllImport("WASPCN",EntryPoint="HV2S67")]
		public static extern void HV2S67(double H,double V,ref double S,ref int Range);
		//已知比焓(kJ/kg)和比容(m^3/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="HV2X67")]
		public static extern void HV2X67(double H,double V,ref double X,ref int Range);
		//已知比焓(kJ/kg)和比容(m^3/kg)，求压力(MPa)、温度(℃)、比熵(kJ/(kg.℃))、干度(100%)
		[DllImport("WASPCN",EntryPoint="HV67")]
		public static extern void HV67(ref double P,ref double T,double H,ref double S,double V,ref double X,ref int Range);

		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2PLP67")]
		public static extern void HX2PLP67(double H,double X,ref double P,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求温度(℃)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2TLP67")]
		public static extern void HX2TLP67(double H,double X,ref double T,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比熵(kJ/(kg.℃))(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2SLP67")]
		public static extern void HX2SLP67(double H,double X,ref double S,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2VLP67")]
		public static extern void HX2VLP67(double H,double X,ref double V,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2PLP67")]
		public static extern void HX2PHP67(double H,double X,ref double P,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求温度(℃)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2THP67")]
		public static extern void HX2THP67(double H,double X,ref double T,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比熵(kJ/(kg.℃))(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2SHP67")]
		public static extern void HX2SHP67(double H,double X,ref double S,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2VHP67")]
		public static extern void HX2VHP67(double H,double X,ref double V,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2P67")]
		public static extern void HX2P67(double H,double X,ref double P,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求温度(℃)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2T67")]
		public static extern void HX2T67(double H,double X,ref double T,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比熵(kJ/(kg.℃))(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2S67")]
		public static extern void HX2S67(double H,double X,ref double S,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求比容(m^3/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX2V67")]
		public static extern void HX2V67(double H,double X,ref double V,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)、温度(℃)、比熵(kJ/(kg.℃))、比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="HXLP67")]
		public static extern void HXLP67(ref double P,ref double T,double H,ref double S,ref double V,double X,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)、温度(℃)、比熵(kJ/(kg.℃))、比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="HXHP67")]
		public static extern void HXHP67(ref double P,ref double T,double H,ref double S,ref double V,double X,ref int Range);
		//已知比焓(kJ/kg)和干度(100%)，求压力(MPa)、温度(℃)、比熵(kJ/(kg.℃))、比容(m^3/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="HX67")]
		public static extern void HX67(ref double P,ref double T,double H,ref double S,ref double V,double X,ref int Range);

		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求压力(MPa)
		[DllImport("WASPCN",EntryPoint="SV2P67")]
		public static extern void SV2P67(double S,double V,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求温度(℃)
		[DllImport("WASPCN",EntryPoint="SV2T67")]
		public static extern void SV2T67(double S,double V,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求比焓(kJ/kg)
		[DllImport("WASPCN",EntryPoint="SV2H67")]
		public static extern void SV2H67(double S,double V,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求干度(100%)
		[DllImport("WASPCN",EntryPoint="SV2X67")]
		public static extern void SV2X67(double S,double V,ref double X,ref int Range);
		//已知比熵(kJ/(kg.℃))和比容(m^3/kg)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、干度(100%)
		[DllImport("WASPCN",EntryPoint="SV67")]
		public static extern void SV67(ref double P,ref double T,ref double H,double S,double V,ref double X,ref int Range);

		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2PLP67")]
		public static extern void SX2PLP67(double S,double X,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2PMP67")]
		public static extern void SX2PMP67(double S,double X,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2PHP67")]
		public static extern void SX2PHP67(double S,double X,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2P67")]
		public static extern void SX2P67(double S,double X,ref double P,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求温度(℃)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2TLP67")]
		public static extern void SX2TLP67(double S,double X,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求温度(℃)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2TMP67")]
		public static extern void SX2TMP67(double S,double X,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求温度(℃)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2THP67")]
		public static extern void SX2THP67(double S,double X,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求温度(℃)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2T67")]
		public static extern void SX2T67(double S,double X,ref double T,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比焓(kJ/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2HLP67")]
		public static extern void SX2HLP67(double S,double X,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比焓(kJ/kg)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2HMP67")]
		public static extern void SX2HMP67(double S,double X,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比焓(kJ/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2HHP67")]
		public static extern void SX2HHP67(double S,double X,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比焓(kJ/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2H67")]
		public static extern void SX2H67(double S,double X,ref double H,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2VLP67")]
		public static extern void SX2VLP67(double S,double X,ref double V,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比容(m^3/kg)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2VMP67")]
		public static extern void SX2VMP67(double S,double X,ref double V,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2VHP67")]
		public static extern void SX2VHP67(double S,double X,ref double V,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求比容(m^3/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX2V67")]
		public static extern void SX2V67(double S,double X,ref double V,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比容(m^3/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="SXLP67")]
		public static extern void SXLP67(ref double P,ref double T,ref double H,double S,ref double V,double X,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比容(m^3/kg)(中压的一个值)
		[DllImport("WASPCN",EntryPoint="SXMP67")]
		public static extern void SXMP67(ref double P,ref double T,ref double H,double S,ref double V,double X,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比容(m^3/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="SXHP67")]
		public static extern void SXHP67(ref double P,ref double T,ref double H,double S,ref double V,double X,ref int Range);
		//已知比熵(kJ/(kg.℃))和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比容(m^3/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="SX67")]
		public static extern void SX67(ref double P,ref double T,ref double H,double S,ref double V,double X,ref int Range);

		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2PLP67")]
		public static extern void VX2PLP67(double V,double X,ref double P,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)(低高压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2PHP67")]
		public static extern void VX2PHP67(double V,double X,ref double P,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2P67")]
		public static extern void VX2P67(double V,double X,ref double P,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求温度(℃)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2TLP67")]
		public static extern void VX2TLP67(double V,double X,ref double T,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求温度(℃)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2THP67")]
		public static extern void VX2THP67(double V,double X,ref double T,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求温度(℃)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2T67")]
		public static extern void VX2T67(double V,double X,ref double T,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比焓(kJ/kg)(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2HLP67")]
		public static extern void VX2HLP67(double V,double X,ref double H,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比焓(kJ/kg)(高压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2HHP67")]
		public static extern void VX2HHP67(double V,double X,ref double H,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比焓(kJ/kg)(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2H67")]
		public static extern void VX2H67(double V,double X,ref double H,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比熵(kJ/(kg.℃))(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2SLP67")]
		public static extern void VX2SLP67(double V,double X,ref double S,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比熵(kJ/(kg.℃))(高压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2SHP67")]
		public static extern void VX2SHP67(double V,double X,ref double S,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求比熵(kJ/(kg.℃))(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX2S67")]
		public static extern void VX2S67(double V,double X,ref double S,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比熵(kJ/(kg.℃))(低压的一个值)
		[DllImport("WASPCN",EntryPoint="VXLP67")]
		public static extern void VXLP67(ref double P,ref double T,ref double H,ref double S,double V,double X,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比熵(kJ/(kg.℃))(高压的一个值)
		[DllImport("WASPCN",EntryPoint="VXHP67")]
		public static extern void VXHP67(ref double P,ref double T,ref double H,ref double S,double V,double X,ref int Range);
		//已知比容(m^3/kg)和干度(100%)，求压力(MPa)、温度(℃)、比焓(kJ/kg)、比熵(kJ/(kg.℃))(缺省是低压的一个值)
		[DllImport("WASPCN",EntryPoint="VX67")]
		public static extern void VX67(ref double P,ref double T,ref double H,ref double S,double V,double X,ref int Range);

		



	}
}
