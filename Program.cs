using System;
using System.Windows.Forms;

namespace Natural
{
    internal static class Program
    {
        /// <summary>
        /// ���ε{�����D�n�i�J�I�C
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // �Ы� Mainwindow ����ҨñҰ�
            Application.Run(new Mainwindow());
        }
    }
}
