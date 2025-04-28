using System;
using System.Windows.Forms;

namespace Thunder
{
    internal static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 啟用視覺化樣式
            Application.EnableVisualStyles();
            // 設定文字渲染為預設
            Application.SetCompatibleTextRenderingDefault(false);

            // 啟動主視窗
            Application.Run(new mainWindow());
        }
    }
}
