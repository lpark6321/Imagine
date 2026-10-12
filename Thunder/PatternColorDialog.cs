using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Thunder
{
    // 會記住顏色的選色器：預設選上次選的顏色，「自訂色彩」列出最近選過的 16 個顏色
    // 只記在記憶體中（不寫檔），程式關閉後清空
    public class PatternColorDialog : ColorDialog
    {
        private const int MaxRecent = 16;  // ColorDialog 的自訂色彩剛好 16 格
        private static readonly List<Color> Recent = new List<Color>();

        public PatternColorDialog()
        {
            FullOpen = true;  // 顯示進階選項
            if (Recent.Count > 0) Color = Recent[0];
            // 自訂色彩格式為 0x00BBGGRR；空格補白色
            CustomColors = Enumerable.Range(0, MaxRecent)
                .Select(i => i < Recent.Count ? ColorTranslator.ToWin32(Recent[i]) : 0xFFFFFF).ToArray();
        }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            bool ok = base.RunDialog(hwndOwner);
            if (ok) Remember(Color);
            return ok;
        }  // 按確定後記住顏色

        private static void Remember(Color color)
        {
            color = Color.FromArgb(color.R, color.G, color.B);  // 去掉具名顏色與透明度，只比 RGB
            Recent.RemoveAll(c => c.ToArgb() == color.ToArgb());
            Recent.Insert(0, color);
            if (Recent.Count > MaxRecent) Recent.RemoveRange(MaxRecent, Recent.Count - MaxRecent);
        }  // 記住顏色（最新的排第一）
    }
}
