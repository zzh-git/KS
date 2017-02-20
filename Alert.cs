using System;
using System.Runtime.InteropServices;

namespace KSPrj
{
    class Alert
    {

        /*[DllImport("kernel32.dll")] 
        public static extern bool Beep(int frequency, int duration);
        */


        /*[DllImport("winmm.dll",EntryPoint="PlaySound")]
        private static extern bool PlaySound(string pszSound,IntPtr hmod,uint fdwSound);
        public static void gosound()
        {
            string strPath=AppDomain.
        }*/

        [DllImport("winmm.dll", EntryPoint = "PlaySound")]
        private static extern bool PlaySound(string pszSound, IntPtr hmod, uint fdwSound);       //调用此函数
        public static void gosound()
        {
            string strPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            strPath = strPath + "1262.wav";
            PlaySound(strPath, IntPtr.Zero, 0);
        }
        public static void goYXYHSound()
        {
            string strPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            strPath = strPath + "notify.wav";
            PlaySound(strPath, IntPtr.Zero, 0);
        }
        public static void goSoundTest()
        {
            string strPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            strPath = strPath + "声音测试.wav";
            PlaySound(strPath, IntPtr.Zero, 0);
        }
    }
}
