using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Thunder
{
    public class ToolStripNumericUpDown : ToolStripControlHost
    {
        // 暴露內部的 NumericUpDown 控件
        public NumericUpDown NumericUpDownControl => Control as NumericUpDown;

        public ToolStripNumericUpDown() : base(new NumericUpDown())
        {
            // 設置默認屬性
            NumericUpDownControl.Minimum = 0;
            NumericUpDownControl.Maximum = 100;
            NumericUpDownControl.Value = 10;
            NumericUpDownControl.Width = 60; // 設置寬度
        }
    }
}
