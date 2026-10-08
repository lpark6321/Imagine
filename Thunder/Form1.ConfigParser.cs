using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Thunder.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Object = System.Object;
using String = System.String;

namespace Thunder
{
    public partial class mainWindow
    {
        // listbox功能-----------------------------------------------------------------------------------
        private Color switchcolor(string sc)
        {
            if (sc.Contains("."))
            {
                // 這裡的 Name 是從 ListBox 中獲取的顏色名稱
                string[] colorParts = sc.Split('.');
                return Color.FromArgb(int.Parse(colorParts[0]), int.Parse(colorParts[1]), int.Parse(colorParts[2])); // 修改 Form1 的背景顏色
            }
            else if (sc.EndsWith("%gray") || sc.EndsWith("%gary"))  // 例如 50%Gray（相容舊的拼法 Gary）
            {
                int level = Math.Max(0, Math.Min(255, (int)(float.Parse(sc.Split('%')[0]) / 100 * 256) - 1));
                return Color.FromArgb(level, level, level);
            }
            else
            {
                // 根據傳入的顏色名稱來設置顏色；Color.FromName 遇到不認識的名稱不會丟例外，會回傳透明黑
                Color color = Color.FromName(sc);
                if (!color.IsKnownColor)
                {
                    throw new FormatException($"無法識別顏色「{sc}」");
                }
                return color;
            }
        }
        private void funcFrame(string color, string line)
        {
            // 先重設 config 沒有指定的 ImgEditor 狀態，避免沿用使用者手動的設定
            tabiefOther_chk.Checked = false;  // 額外的 H/V 線
            tabiefText_chk.Checked = false;   // 文字模式
            tabiefLineSize_nud.Value = 1;
            mypicture.tag = "";  // 匯入背景圖後 tag 會是 "image"，會畫出匯入的圖而不是純色
            if (color.EndsWith("%gary"))
            {
                int cc = (int)((float.Parse(color.Split('%')[0]) / 100) * 256) - 1;
                tabiefBack_btn.BackColor = Color.FromArgb(cc, cc, cc);
            }
            else
            {
                // 操作 Form1 的控件，例如更改 Label 的文字
                tabiefBack_btn.BackColor = switchcolor(color);
            }
            // 根據傳入的 line 參數來設置其他控件的狀態
            // 格式：線種-線色；線種可組合 c 中心、o 外框、n 九點、r 左紅右藍（r 不用線色，只有 r 時可省略線色）
            string[] li = line.Split('-');
            string kinds = li[0].Trim();
            tabiefCenter_chk.Checked = kinds.Contains("c");
            tabiefOutside_chk.Checked = kinds.Contains("o");
            tabiefNine_chk.Checked = kinds.Contains("n");
            tabiefRedBlue_chk.Checked = kinds.Contains("r");
            tabiefLineColor_btn.BackColor = li.Length >= 2 ? switchcolor(li[1]) : Color.White;
        } // listbox控制Frame
        private void funcChess(string num, string option)
        {
            // 選項以 - 分隔：h / v / hv 為翻轉，其他視為格子顏色（預設白色，另一色固定黑色）
            // 例如 "v"、"red"、"255.0.0-v"、"50%gray"
            Color color = Color.White;
            bool hFlip = false, vFlip = false;
            foreach (string token in option.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string t = token.Trim();
                if (t == "h" || t == "v" || t == "hv" || t == "vh")
                {
                    hFlip |= t.Contains("h");
                    vFlip |= t.Contains("v");
                }
                else
                {
                    color = switchcolor(t);
                }
            }
            string[] numParts = num.Split('*');
            if (numParts.Length < 2)
            {
                throw new FormatException($"格數「{num}」格式應為 寬*高，例如 8*8");
            }
            tabiecHNum_nud.Value = int.Parse(numParts[0]);
            tabiecVNum_nud.Value = int.Parse(numParts[1]);
            tabiecHFlip_chk.Checked = hFlip;
            tabiecVFlip_chk.Checked = vFlip;
            // 產生棋盤格時讀的是 tabiecGray_lbl 與灰階捲軸，兩者都要設定，否則會沿用 ImgEditor 的狀態
            tabiecBaseColor_lbl.BackColor = color;
            tabiecGray_lbl.BackColor = color;
            tabiecGray_lbl.ForeColor = (color.R + color.G + color.B) / 3 < 127 ? Color.White : Color.Black;
            tabiecGray_hsc.Maximum = Math.Max(color.R, Math.Max(color.G, color.B));
            tabiecGray_hsc.Value = tabiecGray_hsc.Maximum;
        }  // listbox控制棋盤格
        private void funcText(string colors, string option)
        {
            // 參數：背景色-文字色；選項：文字/大小/字距/行距/串距（大小以後可省略，用預設值）
            string[] colorParts = colors.Split('-');
            funcFrame(colorParts[0], "");  // 背景與框線（重設為無線條、關閉文字模式）
            string[] parts = option.Split('/');
            string text = parts[0].Trim();
            if (text == "") throw new FormatException("沒有文字（選項第一段為文字）");
            if (text.Length > 10) throw new FormatException($"文字最多 10 個字，目前 {text.Length} 個");
            decimal Number(int index, string name, NumericUpDown range)
            {
                if (!decimal.TryParse(parts[index].Trim(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal value))
                {
                    throw new FormatException($"{name}「{parts[index]}」不是數字");
                }
                if (value < range.Minimum || value > range.Maximum) throw new FormatException($"{name}需在 {range.Minimum}~{range.Maximum}");
                return value;
            }
            tabiefTextSize_nud.Value = parts.Length > 1 ? Number(1, "大小", tabiefTextSize_nud) : 12;  // 會依比例重設間距
            if (parts.Length > 2) tabiefTextCharGap_nud.Value = Number(2, "字距", tabiefTextCharGap_nud);
            if (parts.Length > 3) tabiefTextLineGap_nud.Value = Number(3, "行距", tabiefTextLineGap_nud);
            if (parts.Length > 4) tabiefTextStringGap_nud.Value = Number(4, "串距", tabiefTextStringGap_nud);
            tabiefTextColor_btn.BackColor = colorParts.Length > 1 ? switchcolor(colorParts[1]) : Color.White;
            tabiefText_txt.Text = text;
            tabiefText_chk.Checked = true;
        }  // listbox控制文字模式
        private void funcDynamic(string colors, string option)
        {
            // 參數：背景色-物件色；選項：形狀(ball/cube/strip)、方向(h/v/b)、size數字、speed數字，以 - 連接
            mypicture.tag = "";  // 不使用匯入的背景圖
            string[] colorParts = colors.Split('-');
            tabiedBackColor_btn.BackColor = switchcolor(colorParts[0]);
            tabiedObjectColor_btn.BackColor = colorParts.Length > 1 ? switchcolor(colorParts[1]) : Color.White;
            RadioButton shape = tabiedObjectBall_rdo;
            RadioButton direction = tabiedMoveBevel_rdo;
            decimal size = 50, speed = 50;
            foreach (string token in option.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string t = token.Trim();
                if (t == "ball") shape = tabiedObjectBall_rdo;
                else if (t == "cube") shape = tabiedObjectCube_rdo;
                else if (t == "strip") shape = tabiedObjectStrip_rdo;
                else if (t == "h") direction = tabiedMoveH_rdo;
                else if (t == "v") direction = tabiedMoveV_rdo;
                else if (t == "b") direction = tabiedMoveBevel_rdo;
                else if (t.StartsWith("size") && decimal.TryParse(t.Substring(4), out decimal s)) size = s;
                else if (t.StartsWith("speed") && decimal.TryParse(t.Substring(5), out decimal v)) speed = v;
                else throw new FormatException($"無法識別選項「{t}」");
            }
            if (size < tabiedObjectSize_nud.Minimum || size > tabiedObjectSize_nud.Maximum)
            {
                throw new FormatException($"size 需在 {tabiedObjectSize_nud.Minimum}~{tabiedObjectSize_nud.Maximum}");
            }
            if (speed < tabiedMoveSpeed_nud.Minimum || speed > tabiedMoveSpeed_nud.Maximum)
            {
                throw new FormatException($"speed 需在 {tabiedMoveSpeed_nud.Minimum}~{tabiedMoveSpeed_nud.Maximum}");
            }
            shape.Checked = true;  // 先設形狀：選長條時會強制水平並停用斜角
            if (shape == tabiedObjectStrip_rdo && direction == tabiedMoveBevel_rdo)
            {
                direction = tabiedMoveH_rdo;  // 長條不能斜角移動
            }
            direction.Checked = true;
            tabiedObjectSize_nud.Value = size;
            tabiedMoveSpeed_nud.Value = speed;
        }  // listbox控制動態
        private void funcGrad(string color, string hv)
        {
            // 先重設 config 沒有指定的 ImgEditor 狀態（起訖灰階、反向）
            tabigFirstLevel_cmb.Text = "0";
            tabigLastLevel_cmb.Text = "255";
            tabigReverse_chk.Checked = false;
            // 格式：[r-]方向-階數-分割[-起始.結束]；r- 為反向（分割需 ≥ 2），起始.結束 為灰階（例：255.0 由亮到暗）
            bool reverse = hv.StartsWith("r-");
            if (reverse) hv = hv.Substring(2);
            string[] hvs = hv.Split('-');
            if (hv.Trim() != "" && hvs.Length < 3)
            {
                throw new FormatException($"漸層選項「{hv}」格式應為 方向-階數-分割，例如 v-256-1");
            }
            if (hvs.Length >= 3)
            {
                tabigHWay_rdo.Checked = hvs[0].Contains("h") ? true : false;
                tabigVWay_rdo.Checked = hvs[0].Contains("v") ? true : false;
                tabigStep_cmb.Text = hvs[1];
                tabigDivid_cmb.Text = hvs[2];
                if (hvs.Length >= 5)
                {
                    throw new FormatException($"起始、結束灰階請用 . 連接，例如 {hvs[0]}-{hvs[1]}-{hvs[2]}-{hvs[3]}.{hvs[4]}");
                }
                if (hvs.Length == 4)
                {
                    // 第 4 段為「起始.結束」灰階（例：255.0 由亮到暗）
                    string[] levels = hvs[3].Split('.');
                    if (levels.Length != 2 || !int.TryParse(levels[0], out int first) || !int.TryParse(levels[1], out int last)
                        || first < 0 || first > 255 || last < 0 || last > 255)
                    {
                        throw new FormatException($"起始.結束灰階「{hvs[3]}」需為兩個 0~255 的整數，用 . 連接，例如 255.0");
                    }
                    tabigFirstLevel_cmb.Text = first.ToString();
                    tabigLastLevel_cmb.Text = last.ToString();
                }
                if (reverse)
                {
                    if (!int.TryParse(hvs[2], out int divisions) || divisions < 2)
                    {
                        throw new FormatException("反向 (r-) 需要畫面等分 ≥ 2");
                    }
                    tabigReverse_chk.Checked = true;
                }
            }
            else if (reverse)
            {
                throw new FormatException("反向 (r-) 後面需要漸層設定，例如 r-h-256-2");
            }
            else
            {
                tabigHWay_rdo.Checked = false;
                tabigVWay_rdo.Checked = false;
                tabigColorBar_rdo.Checked = true;
                tabigStep_cmb.Text = "256";
                tabigDivid_cmb.Text = "1";
            }

            if (color.StartsWith("colorbar"))
            {
                tabigOther_chk.Checked = true;
                if (color.Contains("-h"))
                {
                    tabigOtherHSplit_rdo.Checked = true;
                }
                else if (color.Contains("-v"))
                {
                    tabigOtherVSplit_rdo.Checked = true;
                }
                SplitPanelCreat(tabigOtherSplit_nud, EventArgs.Empty); // 修正 'e' 為 'EventArgs.Empty'

                if (color.Contains("four"))
                {
                    ColorBar_btn_Click(tabigFourColor_btn, EventArgs.Empty); // 修正 'e' 為 'EventArgs.Empty'  
                }
                else if(color.Contains("eight"))
                {
                    ColorBar_btn_Click(tabigEightColor_btn, EventArgs.Empty); // 修正 'e' 為 'EventArgs.Empty'  
                }
                else
                {
                    string[] colorbars = color.Split('-');
                    if (colorbars.Length >= 3)
                    {
                        tabigOtherVSplit_rdo.Checked = colorbars[1].Contains("v");
                        string[] barcolor = colorbars[2].Split('.');
                        tabigOtherSplit_nud.Value = int.Parse(barcolor[0]);
                        for (int i = 0; i < barcolor.Length - 1; i++)
                        {
                            tabigOtherSplit_pnl.Controls[i].BackColor = switchcolor(barcolor[i + 1]);
                        }
                    }
                }
            }
            else
            {
                tabigOther_chk.Checked = false;
                tabigBaseColor_lbl.BackColor = switchcolor(color); // 修改 Form1 的背景顏色  
            }
        }  // listbox控制漸層
        private static bool IsColorMaskOption(string option)
        {
            // 舊寫法只有 0~2 與 . （例：10.01）；出現 / 、空格、英文字母或 % 就是顏色寫法
            return option.IndexOfAny(new[] { '/', ' ', '%' }) >= 0 || option.Any(char.IsLetter);
        }  // 判斷 Mask 選項是否為顏色寫法
        private void funcMask(string num, string onoff)
        {
            string[] numParts = num.Split('-');
            if (numParts.Length >= 2)
            {
                string[] numhw = numParts[1].Split('*');
                if (numhw.Length >= 2)
                {
                    tabiemWNum_nud.Value = int.Parse(numhw[0]);
                    tabiemHNum_nud.Value = int.Parse(numhw[1]);
                }
                else
                {
                    tabiemWNum_nud.Value = int.Parse(numParts[1]);
                    tabiemHNum_nud.Value = int.Parse(numParts[1]);
                }

                MaskPanelCreat(tabiemPixelClear_btn, EventArgs.Empty);
                if (numParts[0].Contains("p") && IsColorMaskOption(onoff))
                {
                    // 顏色寫法：每格一個顏色（名稱、R.G.B、N%Gray，或 0/1/2），格之間空格、列之間 /
                    tabiemPixel_rdo.Checked = true;
                    int w = (int)tabiemWNum_nud.Value, h = (int)tabiemHNum_nud.Value;
                    string[] rows = onoff.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (rows.Length != h)
                    {
                        throw new FormatException($"高度為 {h}，選項需要 {h} 列（以 / 分隔），實際 {rows.Length} 列");
                    }
                    var colors = new Color[w * h];
                    for (int y = 0; y < h; y++)
                    {
                        string[] cells = rows[y].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (cells.Length != w)
                        {
                            throw new FormatException($"寬度為 {w}，第 {y + 1} 列需要 {w} 格（以空格分隔），實際 {cells.Length} 格");
                        }
                        for (int x = 0; x < w; x++)
                        {
                            colors[y * w + x] = cells[x] == "0" ? Color.Black : cells[x] == "1" ? Color.White : cells[x] == "2" ? Color.Gray : switchcolor(cells[x]);
                        }
                    }
                    foreach (Panel targetPanel in tabiemPixelPanel_pnl.Controls.OfType<Panel>())
                    {
                        string[] coordinates = targetPanel.Tag.ToString().Split(',');
                        targetPanel.BackColor = colors[int.Parse(coordinates[1]) * w + int.Parse(coordinates[0])];
                    }
                    return;
                }
                onoff = onoff.Replace(".", "");
                if (numParts[0].Contains("p"))
                {
                    tabiemPixel_rdo.Checked = true;
                    for (int i = 0; i < tabiemHNum_nud.Value; i++)
                    {
                        for (int j = 0; j < tabiemWNum_nud.Value; j++)
                        {
                            Panel targetPanel =
                                tabiemPixelPanel_pnl.Controls[i * (int)tabiemWNum_nud.Value + j] as Panel;
                            if (targetPanel != null)
                            {
                                // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                string[] coordinates = targetPanel.Tag.ToString().Split(',');
                                // 計算在 onoff 字符串中的索引
                                int index = (int.Parse(coordinates[1]) * (int)tabiemWNum_nud.Value) + int.Parse(coordinates[0]);
                                // 設置 Panel 的顏色
                                targetPanel.BackColor = onoff[index] == '0' ? Color.Black : 
                                    onoff[index] == '1' ? Color.White : Color.Gray;
                            }
                        }
                    }
                }
                else if (numParts[0].Contains("s"))
                {
                    tabiemSubPixel_rdo.Checked = true;
                    //MaskPanelCreat(tabiemPixelClear_btn, EventArgs.Empty);
                    if (numParts.Length >= 3)
                    {
                        tabiemPixelGray_vsc.Value = 255 - int.Parse(numParts[2]);
                    }

                    for (int i = 0; i < tabiemHNum_nud.Value; i++)
                    {
                        for (int j = 0; j < tabiemWNum_nud.Value * 3; j++)
                        {
                            Panel targetPanel =
                                tabiemPixelPanel_pnl.Controls[i * (int)tabiemWNum_nud.Value * 3 + j] as Panel;
                            if (targetPanel != null)
                            {
                                // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                string[] coordinates = targetPanel.Tag.ToString().Split(',');
                                // 計算在 onoff 字符串中的索引
                                int index = (int.Parse(coordinates[1]) * (int)tabiemWNum_nud.Value * 3) + int.Parse(coordinates[0]);
                                // 設置 Panel 的顏色
                                if (onoff[index] == '0')
                                {
                                    targetPanel.BackColor = Color.Black;
                                }
                            }
                        }
                    }
                }
            }
        }  // listbox控制遮罩
        private void funcWind(string color, string sizeloc)
        {
            // 先重設 config 沒有指定的 ImgEditor 狀態，避免沿用使用者手動的設定
            tabiwDiagonal_cmb.SelectedIndex = 0;  // 斜角：無
            tabiwDiagonal_btn.Checked = false;    // 選單原本就是「無」時不會觸發同步，直接重設
            tabiwLineOutside_btn.Checked = false;
            tabiwLineCross_btn.Checked = false;
            tabiwCMaskPixel_rdo.Checked = true;  // 自製圖固定用 Pixel 格，下面的 onoff 以像素排列
            // 視窗大小要在 Custom_btn_Click 產生窗口圖之前設定，否則窗口圖會用舊尺寸產生再被縮放
            tabiwWinSizePercent_rdo.Checked = true;
            tabiwWinSizePercent_nud.Value = sizeloc.Contains("%") ? int.Parse(sizeloc.Split('%')[0]) : 50;
            if (color.Contains("excel"))
            {
                tabiwBackColor_rdo.Checked = true;
                tabiwWinCustom_rdo.Checked = true;
                windowPictureBox_Click(tabiwWinCustom_rdo, EventArgs.Empty);
                tabiwCustom_cmb.SelectedIndex = 0;
                tabiwBack_pic.BackColor = Color.Gray;
                int wnum = 2;
                int hnum = 2;
                string onoff = "10.01";
                switch (color)
                {
                    case "excel1":
                        onoff = "10.22";
                        wnum = hnum = 2;
                        break;
                    case "excel2":
                        onoff = "0011.2222";
                        wnum = 4;
                        hnum = 2;
                        break;
                    case "excel3":
                        onoff = "0001.2222";
                        wnum = 4;
                        hnum = 2;
                        break;
                    case "excel4":
                        onoff = "000000000111.222222222222";
                        wnum = 12;
                        hnum = 2;
                        break;
                    case "excel5":
                        onoff = "000111000111000000.222222222222222222";
                        wnum = 18;
                        hnum = 2;
                        break;
                    case "excel6":
                        onoff = "000111000111000000000111.222222222222222222222222";
                        wnum = 24;
                        hnum = 2;
                        break;
                    case "excel7":
                        onoff = "000000100000100000.222222222222222222.222222222222222222.000011000011000000";
                        wnum = 18;
                        hnum = 4;
                        break;
                    default:
                        break;
                }

                onoff = onoff.Replace(".", "");
                tabiwCMaskWNum_nud.Value = wnum;
                tabiwCMaskHNum_nud.Value = hnum;
                MaskPanelCreat(tabiwCMaskPixelClear_btn, EventArgs.Empty);
                for (int i = 0; i < hnum; i++)
                {
                    for (int j = 0; j < wnum; j++)
                    {
                        Panel targetPanel =
                            tabiwCMaskPixelPanel_pnl.Controls[i * wnum + j] as Panel;
                        if (targetPanel != null)
                        {
                            // 解析 Panel 的座標 (Tag 格式為 "x,y")
                            string[] coordinates = targetPanel.Tag.ToString().Split(',');
                            // 計算在 onoff 字符串中的索引
                            int index = (int.Parse(coordinates[1]) * wnum + int.Parse(coordinates[0]));
                            // 設置 Panel 的顏色
                            if (onoff[index] == '1')
                            {
                                targetPanel.BackColor = Color.White;
                            }
                            else if (onoff[index] == '2')
                            {
                                targetPanel.BackColor = Color.Gray;
                            }
                            else
                            {
                                targetPanel.BackColor = Color.Black;
                            }
                        }
                    }
                }
                Custom_btn_Click(tabiwCustom_btn, EventArgs.Empty);
            }
            else if (color.Contains("custom"))
            {
                string[] colorParts = color.Split('-');
                if (colorParts.Length >= 3)
                {
                    int wnum = 0;
                    int hnum = 0;
                    string onoff = "";
                    if (colorParts[2] == "black")
                    {
                        tabiwWinColor_rdo.Checked = true;
                        tabiwWin_pic.Image = null;
                        tabiwWin_pic.BackColor = Color.Black; // 修改 Form1 的背景顏色
                    }
                    else if (colorParts[2] == "gray")
                    {
                        tabiwWinColor_rdo.Checked = true;
                        tabiwWin_pic.Image = null;
                        tabiwWin_pic.BackColor = Color.Gray; // 修改 Form1 的背景顏色
                    }
                    else if (colorParts[2] == "dotw")
                    {
                        tabiwWinCustom_rdo.Checked = true;
                        tabiwWin_pic.Image = null;
                        windowPictureBox_Click(tabiwWinCustom_rdo, EventArgs.Empty);
                        tabiwCustom_cmb.SelectedIndex = 0;
                        wnum = 2;
                        hnum = 2;
                        onoff = "10.01";

                        onoff = onoff.Replace(".", "");
                        tabiwCMaskWNum_nud.Value = wnum;
                        tabiwCMaskHNum_nud.Value = hnum;
                        MaskPanelCreat(tabiwCMaskPixelClear_btn, EventArgs.Empty);
                        for (int i = 0; i < hnum; i++)
                        {
                            for (int j = 0; j < wnum; j++)
                            {
                                Panel targetPanel =
                                    tabiwCMaskPixelPanel_pnl.Controls[i * wnum + j] as Panel;
                                if (targetPanel != null)
                                {
                                    // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                    string[] coordinates = targetPanel.Tag.ToString().Split(',');
                                    // 計算在 onoff 字符串中的索引
                                    int index = (int.Parse(coordinates[1]) * wnum + int.Parse(coordinates[0]));
                                    // 設置 Panel 的顏色
                                    if (onoff[index] == '1')
                                    {
                                        targetPanel.BackColor = Color.White;
                                    }
                                    else
                                    {
                                        targetPanel.BackColor = Color.Black;
                                    }
                                }
                            }
                        }
                        Custom_btn_Click(tabiwCustom_btn, EventArgs.Empty);
                    }
                    else
                    {
                        tabiwWinColor_rdo.Checked = true;
                        tabiwWin_pic.Image = null;
                        tabiwWin_pic.BackColor = switchcolor(colorParts[2]); // Custom-背景-窗口：第 3 段為窗口顏色
                    }


                    if (colorParts[1] == "dotg")
                    {
                        tabiwBackCustom_rdo.Checked = true;
                        tabiwBack_pic.Image = null;
                        windowPictureBox_Click(tabiwBackCustom_rdo, EventArgs.Empty);
                        tabiwCustom_cmb.SelectedIndex = 0;
                        wnum = 2;
                        hnum = 2;
                        onoff = "20.02";
                    }
                    else if(colorParts[1] == "vstripeg")
                    {
                        tabiwBackCustom_rdo.Checked = true;
                        tabiwBack_pic.Image = null;
                        windowPictureBox_Click(tabiwBackCustom_rdo, EventArgs.Empty);
                        tabiwCustom_cmb.SelectedIndex = 0;
                        wnum = 2;
                        hnum = 2;
                        onoff = "45.54";
                    }
                    else if(colorParts[1] == "vstripew")
                    {
                        tabiwBackCustom_rdo.Checked = true;
                        tabiwBack_pic.Image = null;
                        windowPictureBox_Click(tabiwBackCustom_rdo, EventArgs.Empty);
                        tabiwCustom_cmb.SelectedIndex = 0;
                        wnum = 2;
                        hnum = 2;
                        onoff = "67.76";
                    }
                    else if (colorParts[1] == "dotdell")
                    {
                        tabiwBackCustom_rdo.Checked = true;
                        tabiwBack_pic.Image = null;
                        windowPictureBox_Click(tabiwBackCustom_rdo, EventArgs.Empty);
                        tabiwCustom_cmb.SelectedIndex = 0;
                        wnum = 2;
                        hnum = 2;
                        onoff = "30.03";
                    }
                    else if (colorParts[1] == "roww")
                    {
                        tabiwBackCustom_rdo.Checked = true;
                        tabiwBack_pic.Image = null;
                        windowPictureBox_Click(tabiwBackCustom_rdo, EventArgs.Empty);
                        tabiwCustom_cmb.SelectedIndex = 0;
                        wnum = 2;
                        hnum = 2;
                        onoff = "11.00";
                    }
                    else
                    {
                        tabiwBackColor_rdo.Checked = true;
                        tabiwBack_pic.Image = null;
                        tabiwBack_pic.BackColor = switchcolor(colorParts[1]); // Custom-背景-窗口：第 2 段為背景顏色
                    }
                    if (tabiwBackCustom_rdo.Checked == true && onoff != "")
                    {
                        onoff = onoff.Replace(".", "");
                        tabiwCMaskWNum_nud.Value = wnum;
                        tabiwCMaskHNum_nud.Value = hnum;
                        MaskPanelCreat(tabiwCMaskPixelClear_btn, EventArgs.Empty);
                        for (int i = 0; i < hnum; i++)
                        {
                            for (int j = 0; j < wnum; j++)
                            {
                                Panel targetPanel =
                                    tabiwCMaskPixelPanel_pnl.Controls[i * wnum + j] as Panel;
                                if (targetPanel != null)
                                {
                                    // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                    string[] coordinates = targetPanel.Tag.ToString().Split(',');
                                    // 計算在 onoff 字符串中的索引
                                    int index = (int.Parse(coordinates[1]) * wnum + int.Parse(coordinates[0]));
                                    // 設置 Panel 的顏色
                                    switch (onoff[index])
                                    {
                                        case '0':
                                            targetPanel.BackColor = Color.Black;
                                            break;
                                        case '1':
                                            targetPanel.BackColor = Color.White;
                                            break;
                                        case '2':
                                            targetPanel.BackColor = Color.Gray;
                                            break;
                                        case '3':
                                            targetPanel.BackColor = Color.FromArgb(0, 127, 127);
                                            break;
                                        case '4':
                                            targetPanel.BackColor = Color.FromArgb(127, 0, 127);
                                            break;
                                        case '5':
                                            targetPanel.BackColor = Color.FromArgb(0, 127, 0);
                                            break;
                                        case '6':
                                            targetPanel.BackColor = Color.FromArgb(255, 0, 255);
                                            break;
                                        case '7':
                                            targetPanel.BackColor = Color.FromArgb(0, 255, 0);
                                            break;
                                        default:
                                            targetPanel.BackColor = Color.Black;
                                            break;
                                    }
                                }
                            }
                        }
                        Custom_btn_Click(tabiwCustom_btn, EventArgs.Empty);
                    }
                }
            }
            else
            {
                string[] colorParts = color.Split('-');
                if (colorParts.Length >= 2)
                {
                    tabiwBackColor_rdo.Checked = true;
                    tabiwWinColor_rdo.Checked = true;
                    tabiwBack_pic.Image = null;
                    tabiwBack_pic.BackColor = switchcolor(colorParts[0]); // 修改 Form1 的背景顏色
                    tabiwWin_pic.Image = null;
                    tabiwWin_pic.BackColor = switchcolor(colorParts[1]); // 修改 Form1 的背景顏色
                }
            }

            if (sizeloc.Contains("xy"))  //loc center
            {
                tabiwWinLocPixcel_rdo.Checked = true;
                int x = 0;
                int y = 0;
                if (sizeloc.Contains("q"))  //loc center
                {
                    if (sizeloc.Contains("++"))
                    {
                        x = int.Parse(mnsW_txt.Text) / 2;
                        y = 0;
                    }
                    else if (sizeloc.Contains("-+"))
                    {
                        x = 0;
                        y = 0;
                    }
                    else if (sizeloc.Contains("--"))
                    {
                        x = 0;
                        y = int.Parse(mnsH_txt.Text) / 2;
                    }
                    else if (sizeloc.Contains("+-"))
                    {
                        x = int.Parse(mnsW_txt.Text) / 2;
                        y = int.Parse(mnsH_txt.Text) / 2;
                    }
                } 
                else
                {
                    x = int.Parse(sizeloc.Split('_')[1].Split('.')[0]);
                    y = int.Parse(sizeloc.Split('_')[1].Split('.')[1]);
                }
                tabiwWinLocPixcelX_nud.Value = x;
                tabiwWinLocPixcelY_nud.Value = y;
            }
            else if(sizeloc.Contains("two"))  //loc outside
            {
                tabiwWinLocTwo_rdo.Checked = true;
            }
            else  //loc center
            {
                tabiwWinLocCenter_rdo.Checked = true;
            }
        } // listbox控制窗口
    }
}
