using System;
using System.Windows.Forms;

namespace Thunder
{
    public static class CodeFile1
    {
        public static void func1(mainWindow form)
        {
            // 操作 Form1 的控件，例如更改 Label 的文字
            if (form != null)
            {
                form.Invoke(new Action(() =>
                {
                    form.Text = "CodeFile1 控制了 Form1 的標題！"; // 修改 Form1 的標題
                    //form.showimgSize_btn.Text = "圖片大小已更新！"; // 修改某個按鈕的文字
                }));
            }
        }

        public static void func2(Button ff, string st)
        {
            // 操作 Form1 的控件，例如更改 Label 的文字
            if (ff != null)
            {
                ff.Invoke(new Action(() =>
                {
                    ff.Text = st; // 修改 Form1 的標題
                    //form.showimgSize_btn.Text = "圖片大小已更新！"; // 修改某個按鈕的文字
                }));
            }
        }
    }
}