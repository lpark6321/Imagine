using System;
using System.Windows.Forms;

namespace Natural
{
    internal static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 創建 Mainwindow 的實例並啟動
            Application.Run(new Mainwindow());
        }
    }
}
