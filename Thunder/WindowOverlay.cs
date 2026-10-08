using System;
using System.Drawing;
using System.Windows.Forms;

namespace Thunder
{
    public enum OverlayKind
    {
        Window,  // 窗口：X、Y、寬、高
        HLine,   // 水平線：Y、粗細
        VLine,   // 垂直線：X、粗細
        Circle   // 圓形：圓心 X、Y、直徑
    }

    // ImgEditor Window 額外疊加的窗口或線條（與外框、十字分開）
    public class WindowOverlay
    {
        public OverlayKind Kind;
        public int X, Y;             // 窗口左上角；圓形為圓心；水平線用 Y、垂直線用 X
        public int Width, Height;    // 窗口大小；線條的粗細、圓形的直徑存在 Width
        public Color Color;

        public static string KindText(OverlayKind kind) =>
            kind == OverlayKind.Window ? "窗口" : kind == OverlayKind.HLine ? "水平線" : kind == OverlayKind.VLine ? "垂直線" : "圓形";

        public static string ColorText(Color c) => c.IsNamedColor ? c.Name : $"{c.R}.{c.G}.{c.B}";

        public string Description()
        {
            switch (Kind)
            {
                case OverlayKind.Window: return $"({X},{Y}) {Width}x{Height} {ColorText(Color)}";
                case OverlayKind.HLine: return $"Y={Y} 粗{Width} {ColorText(Color)}";
                case OverlayKind.Circle: return $"圓心({X},{Y}) 直徑{Width} {ColorText(Color)}";
                default: return $"X={X} 粗{Width} {ColorText(Color)}";
            }
        }

        public void Draw(Graphics g, int imageWidth, int imageHeight)
        {
            // 用 FillRectangle 畫，確保線條與窗口剛好落在整數像素上
            using (Brush brush = new SolidBrush(Color))
            {
                switch (Kind)
                {
                    case OverlayKind.Window: g.FillRectangle(brush, X, Y, Width, Height); break;
                    case OverlayKind.HLine: g.FillRectangle(brush, 0, Y, imageWidth, Width); break;
                    case OverlayKind.VLine: g.FillRectangle(brush, X, 0, Width, imageHeight); break;
                    case OverlayKind.Circle: g.FillEllipse(brush, X - Width / 2f, Y - Width / 2f, Width, Width); break;  // 不反鋸齒，邊緣保持純色
                }
            }
        }
    }

    // 新增 / 修改疊加項目的小視窗
    public class WindowOverlayDialog : Form
    {
        private readonly ComboBox kind_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 90 };
        private readonly NumericUpDown x_nud = NewNud(), y_nud = NewNud(), w_nud = NewNud(), h_nud = NewNud();
        private readonly Label x_lbl = NewLabel("X"), y_lbl = NewLabel("Y"), w_lbl = NewLabel("寬"), h_lbl = NewLabel("高");
        private readonly Button color_btn = new Button { Width = 60, Height = 24, FlatStyle = FlatStyle.Flat };
        public WindowOverlay Result { get; private set; }

        private static NumericUpDown NewNud() => new NumericUpDown { Maximum = 16384, Width = 70, ImeMode = ImeMode.Disable };
        private static Label NewLabel(string text) => new Label { Text = text, AutoSize = true, Margin = new Padding(3, 6, 0, 0) };

        public WindowOverlayDialog(WindowOverlay initial)
        {
            Text = "窗口 / 線條";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            kind_cmb.Items.AddRange(new object[] { WindowOverlay.KindText(OverlayKind.Window), WindowOverlay.KindText(OverlayKind.HLine), WindowOverlay.KindText(OverlayKind.VLine), WindowOverlay.KindText(OverlayKind.Circle) });
            kind_cmb.SelectedIndexChanged += (s, e) => UpdateFields();
            color_btn.Click += (s, e) =>
            {
                using (ColorDialog dialog = new ColorDialog { Color = color_btn.BackColor, FullOpen = true })
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK) color_btn.BackColor = dialog.Color;
                }
            };
            var ok = new Button { Text = "確定", DialogResult = DialogResult.OK, Width = 70 };
            var cancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Width = 70 };
            ok.Click += (s, e) => Result = BuildResult();
            AcceptButton = ok;
            CancelButton = cancel;

            var table = new TableLayoutPanel { ColumnCount = 4, AutoSize = true, Padding = new Padding(8), Dock = DockStyle.Fill };
            table.Controls.Add(NewLabel("類型"), 0, 0); table.Controls.Add(kind_cmb, 1, 0);
            table.Controls.Add(NewLabel("顏色"), 2, 0); table.Controls.Add(color_btn, 3, 0);
            table.Controls.Add(x_lbl, 0, 1); table.Controls.Add(x_nud, 1, 1);
            table.Controls.Add(y_lbl, 2, 1); table.Controls.Add(y_nud, 3, 1);
            table.Controls.Add(w_lbl, 0, 2); table.Controls.Add(w_nud, 1, 2);
            table.Controls.Add(h_lbl, 2, 2); table.Controls.Add(h_nud, 3, 2);
            var buttons = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, AutoSize = true, Dock = DockStyle.Fill };
            buttons.Controls.Add(cancel); buttons.Controls.Add(ok);
            table.Controls.Add(buttons, 0, 3); table.SetColumnSpan(buttons, 4);
            Controls.Add(table);

            w_nud.Minimum = h_nud.Minimum = 1;
            kind_cmb.SelectedIndex = (int)initial.Kind;
            SetValue(x_nud, initial.X); SetValue(y_nud, initial.Y);
            SetValue(w_nud, initial.Width); SetValue(h_nud, initial.Height);
            color_btn.BackColor = initial.Color;
            UpdateFields();
        }

        private static void SetValue(NumericUpDown nud, int value) => nud.Value = Math.Max(nud.Minimum, Math.Min(nud.Maximum, value));

        private void UpdateFields()
        {
            // 依類型切換欄位：窗口用 X/Y/寬/高；水平線用 Y/粗細；垂直線用 X/粗細；圓形用圓心 X/Y/直徑
            OverlayKind kind = (OverlayKind)Math.Max(0, kind_cmb.SelectedIndex);
            bool window = kind == OverlayKind.Window, circle = kind == OverlayKind.Circle;
            x_nud.Enabled = kind != OverlayKind.HLine;
            y_nud.Enabled = kind != OverlayKind.VLine;
            h_nud.Enabled = h_lbl.Enabled = window;
            x_lbl.Text = circle ? "圓心X" : "X";
            y_lbl.Text = circle ? "圓心Y" : "Y";
            w_lbl.Text = window ? "寬" : circle ? "直徑" : "粗細";
            if (!window && !circle && w_nud.Value > 100) w_nud.Value = 1;
        }

        private WindowOverlay BuildResult() => new WindowOverlay
        {
            Kind = (OverlayKind)kind_cmb.SelectedIndex,
            X = (int)x_nud.Value,
            Y = (int)y_nud.Value,
            Width = (int)w_nud.Value,
            Height = (int)h_nud.Value,
            Color = color_btn.BackColor,
        };
    }
}
