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
        // 圖片處理 groupbox 功能--------------------------------------------------------------------------------------
        private void SetButtonBackgroundColor(object sender, EventArgs e)
        {
            // 將 targetButton 修改為事件的觸發按鈕  
            if (sender is Button targetButton)
            {
                using (ColorDialog colorDialog = new PatternColorDialog())
                {
                    colorDialog.FullOpen = true; // 顯示進階選項  
                                                 // 顯示顏色選擇對話框  
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        // 將選擇的顏色設置為目標按鈕的背景顏色  
                        targetButton.BackColor = colorDialog.Color;
                        targetButton.Image = null; // 清除按鈕上的圖片
                        mypicture.tag = "color";
                        //if (targetButton.Name.Contains("tabiefBack_btn"))
                        //{
                        //}
                    }
                }
            }
        }  // 設定按鈕顏色
        private void GetButtonBackgroundColor(object sender, EventArgs e)
        {
            if (sender is Button targetButton)
            {
                if (targetButton.Text == "")
                {
                    SetButtonBackgroundColor(sender, e);
                }
                if ((targetButton.BackColor.R + targetButton.BackColor.G + targetButton.BackColor.B) / 3 < 127)
                {
                    tabiecGray_lbl.ForeColor = Color.White;
                }
                else
                {
                    tabiecGray_lbl.ForeColor = Color.Black;
                }
                tabiecGray_lbl.BackColor = targetButton.BackColor;
                tabiecGray_hsc.Maximum = Math.Max(targetButton.BackColor.R, Math.Max(targetButton.BackColor.G, targetButton.BackColor.B));
                tabiecGray_hsc.Value = tabiecGray_hsc.Maximum;
            }
        }  //Chess按鈕顏色
        private void GradientColor_Click(object sender, EventArgs e)
        {
            if (sender is Button targetbutton)
            {
                Color color = targetbutton.BackColor;
                if (targetbutton.Name.Contains("tabigBaseColor"))
                {

                    if (targetbutton.Text == "")
                    {
                        GetButtonBackgroundColor(sender, e);
                    }
                    tabigBaseColor_lbl.BackColor = color;
                    tabigBaseColor_lbl.ForeColor = (color.R + color.G + color.B) / 3 < 127 ? Color.White : Color.Black;
                }
                else if (targetbutton.Name.Contains("tabigOtherColor"))
                {
                    if (targetbutton.Text == "")
                    {
                        GetButtonBackgroundColor(sender, e);
                    }
                    tabigOtherColor_lbl.BackColor = color;
                    tabigOtherColor_lbl.ForeColor = (color.R + color.G + color.B) / 3 < 127 ? Color.White : Color.Black;
                }
                else if (targetbutton.Name.Contains("tabiecBaseColor"))
                {
                    if (targetbutton.Text == "")
                    {
                        GetButtonBackgroundColor(sender, e);
                    }
                    tabiecGray_lbl.BackColor = color;
                    tabiecGray_lbl.ForeColor = (color.R + color.G + color.B) / 3 < 127 ? Color.White : Color.Black;
                }
                else if (targetbutton.Name.Contains("tabiwCGradBaseColor"))
                {
                    if (targetbutton.Text == "")
                    {
                        GetButtonBackgroundColor(sender, e);
                    }
                    tabiwCGradBaseColor_lbl.BackColor = color;
                    tabiwCGradBaseColor_lbl.ForeColor = (color.R + color.G + color.B) / 3 < 127 ? Color.White : Color.Black;
                }
                else if (targetbutton.Name.Contains("tabiwCGradOtherColor"))
                {
                    if (targetbutton.Text == "")
                    {
                        GetButtonBackgroundColor(sender, e);
                    }
                    tabiwCGradOtherColor_lbl.BackColor = color;
                    tabiwCGradOtherColor_lbl.ForeColor = (color.R + color.G + color.B) / 3 < 127 ? Color.White : Color.Black;
                }
            }
        }  // 按鈕設定Label設定顏色
        private void gbxfHsblocation_ValueChanged(object sender, EventArgs e)
        {
            tabiefOtherLoc_lbl.Text = tabiefOtherLoc_hsc.Value.ToString();  // frame 的卷軸值變更
        }  // Frame 其他線的卷軸值變更
        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            tabiecGray_lbl.Text = $"Gray: {tabiecGray_hsc.Value}"; // 更新 Label 顯示當前值
        }  // Chess卷軸值變更

        private void Framechkother_CheckStateChanged(object sender, EventArgs e)
        {
            if (tabiefOther_chk.Checked)  // frame 的卷軸值顯示
            {
                tabiefOther_pnl.Enabled = true;
                //tabiefOtherLoc_lbl.Text = tabiefOtherLoc_hsc.Value.ToString();
            }
            else
            {
                tabiefOther_pnl.Enabled = false;
                //tabiefOtherLoc_lbl.Text = "";
            }
        }  // Frame 其他線的顯示
        private void gbxfBtnaddclear_Click(object sender, EventArgs e)
        {
            if (sender is Button targetbutton)
            {
                if (targetbutton.Text == "Add")
                {
                    // Frame 增加其他線 
                    if (!string.IsNullOrEmpty(tabiefOther_cmb.Text))
                    {
                        string itemText = $"{tabiefOther_cmb.Text.Substring(0, 1)}:{tabiefOtherLoc_lbl.Text},{tabiefOtherColor_btn.BackColor.R},{tabiefOtherColor_btn.BackColor.G},{tabiefOtherColor_btn.BackColor.B}";
                        if (!tabiefOther_lst.Items.Contains(itemText))
                        {
                            // 將格式化字串新增到 gbxfLst  
                            tabiefOther_lst.Items.Add(itemText);
                        }
                    }
                }
                else if (targetbutton.Text == "Clear")
                {
                    //清空Framelst的內容
                    tabiefOther_lst.Items.Clear();
                }
            }
        }  // Frame 其他線listbox的增加與清除
        private void tabiefOther_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabiefOther_cmb.Text == "HLine")
            {
                tabiefOtherLoc_hsc.Maximum = (int.Parse(mnsH_txt.Text)-1); // 將 msTxtH.Text 轉換為 int  
            }
            else if (tabiefOther_cmb.Text == "VLine")
            {
                tabiefOtherLoc_hsc.Maximum = (int.Parse(mnsW_txt.Text)-1); // 將 msTxtW.Text 轉換為 int  
            }
        }  // Frame 其他線的選擇HLine/VLine
        private Bitmap ApplyTransparency(Bitmap originalBitmap, int startAlpha, int endAlpha, bool isH, int steps, int segments, bool reverse)
        {
            // 複製原始圖片，避免直接修改
            //Bitmap resultBitmap = new Bitmap(originalBitmap);
            Bitmap resultBitmap = originalBitmap;

            // 鎖定圖片的像素資料
            Rectangle rect = new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height);
            BitmapData bitmapData = resultBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // 取得像素資料的指標
            IntPtr ptr = bitmapData.Scan0;
            int bytes = Math.Abs(bitmapData.Stride) * resultBitmap.Height;
            byte[] pixelValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, pixelValues, 0, bytes);


            if (reverse)
            {
                // 計算每個分區的長度
                int dimension = isH ? resultBitmap.Width : resultBitmap.Height;
                int segmentLength = isH ? resultBitmap.Height : resultBitmap.Width;
                // 處理分階數大於分區長度的情況
                steps = Math.Min(steps, segmentLength);

                // 計算每個階段的透明度變化量
                int alphaRange = endAlpha - startAlpha;
                float alphaStep = (float)alphaRange / (steps - 1);


                // 遍歷每個像素
                for (int y = 0; y < resultBitmap.Height; y++)
                {
                    for (int x = 0; x < resultBitmap.Width; x++)
                    {
                        int index = (y * bitmapData.Stride) + (x * 4); // 每個像素佔 4 個位元組 (BGRA)

                        // 計算當前像素所在的分區
                        int position = isH ? y : x;
                        int positionIndex = isH ? x / (resultBitmap.Width / segments) : y / (resultBitmap.Height / segments);

                        // 計算當前像素在分區內的位置
                        int positionInSegment = position % segmentLength;
                        // 如果分區是奇數，反向透明度
                        int stepIndex;
                        byte alpha;
                        if (positionIndex % 2 == 1)
                        {
                            stepIndex = (int)((float)positionInSegment / segmentLength * (steps));
                            alpha = (byte)(endAlpha - alphaStep * stepIndex);
                        }
                        else // 計算透明度
                        {
                            stepIndex = (int)((float)positionInSegment / segmentLength * (steps));
                            alpha = (byte)(startAlpha + alphaStep * stepIndex);
                        }

                        // 修改 Alpha 值
                        pixelValues[index + 3] = alpha; // Alpha 通道
                    }
                }
            }
            else
            {
                // 計算每個分區的長度
                int dimension = isH ? resultBitmap.Height : resultBitmap.Width;
                int segmentLength = dimension / segments;
                // 處理分階數大於分區長度的情況
                steps = Math.Min(steps, segmentLength);

                // 計算每個階段的透明度變化量
                int alphaRange = endAlpha - startAlpha;
                float alphaStep = (float)alphaRange / (steps - 1);

                // 遍歷每個像素
                for (int y = 0; y < resultBitmap.Height; y++)
                {
                    for (int x = 0; x < resultBitmap.Width; x++)
                    {
                        int index = (y * bitmapData.Stride) + (x * 4); // 每個像素佔 4 個位元組 (BGRA)

                        // 計算當前像素所在的分區
                        int position = isH ? y : x;

                        // 計算當前像素在分區內的位置
                        int positionInSegment = position % segmentLength;

                        // 計算透明度
                        int stepIndex = (int)((float)positionInSegment / segmentLength * (steps));
                        byte alpha = (byte)(startAlpha + alphaStep * stepIndex);

                        // 修改 Alpha 值
                        pixelValues[index + 3] = alpha; // Alpha 通道
                    }
                }
            }
            

            // 將修改後的像素資料寫回圖片
            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, ptr, bytes);
            resultBitmap.UnlockBits(bitmapData);

            return resultBitmap;
        }  // Grad 透明度處理
        private Bitmap ConvertARGBToRGB(Bitmap originalBitmap)
        {
            // 建立與原始圖片相同大小的 RGB Bitmap
            Bitmap rgbBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height, PixelFormat.Format24bppRgb);

            // 鎖定原始圖片的像素資料
            Rectangle rect = new Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height);
            BitmapData originalData = originalBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // 鎖定新圖片的像素資料
            BitmapData rgbData = rgbBitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            // 取得像素資料的指標
            IntPtr originalPtr = originalData.Scan0;
            IntPtr rgbPtr = rgbData.Scan0;

            // 計算像素資料的大小
            int originalBytes = Math.Abs(originalData.Stride) * originalBitmap.Height;
            int rgbBytes = Math.Abs(rgbData.Stride) * rgbBitmap.Height;

            // 建立緩衝區
            byte[] originalPixels = new byte[originalBytes];
            byte[] rgbPixels = new byte[rgbBytes];

            // 複製像素資料到緩衝區
            System.Runtime.InteropServices.Marshal.Copy(originalPtr, originalPixels, 0, originalBytes);

            // 遍歷每個像素
            for (int y = 0; y < originalBitmap.Height; y++)
            {
                for (int x = 0; x < originalBitmap.Width; x++)
                {
                    // 計算像素索引
                    int originalIndex = y * originalData.Stride + x * 4; // ARGB 每像素 4 bytes
                    int rgbIndex = y * rgbData.Stride + x * 3; // RGB 每像素 3 bytes

                    // 取得原始像素的 ARGB 值
                    byte b = originalPixels[originalIndex];
                    byte g = originalPixels[originalIndex + 1];
                    byte r = originalPixels[originalIndex + 2];
                    byte a = originalPixels[originalIndex + 3];

                    // 計算與黑色背景混合後的 RGB 值
                    byte finalR = (byte)((r * a) / 255);
                    byte finalG = (byte)((g * a) / 255);
                    byte finalB = (byte)((b * a) / 255);

                    // 寫入 RGB 像素資料
                    rgbPixels[rgbIndex] = finalB;
                    rgbPixels[rgbIndex + 1] = finalG;
                    rgbPixels[rgbIndex + 2] = finalR;
                }
            }

            // 將修改後的像素資料寫回新圖片
            System.Runtime.InteropServices.Marshal.Copy(rgbPixels, 0, rgbPtr, rgbBytes);

            // 解鎖像素資料
            originalBitmap.UnlockBits(originalData);
            rgbBitmap.UnlockBits(rgbData);

            return rgbBitmap;
        }  // Grad ARGB 轉 RGB
        private void SplitPanelCreat(object sender, EventArgs e)
        {
            if (sender is Control control && control != null)
            {
                Panel targetPanel;
                int splitnum;
                int blockWidth;
                int blockHeight;
                string splitType;
                if (control.Parent != null && control.Parent.Name == "tabigOther_grp")
                {
                    // 取得並清空面板
                    targetPanel = tabigOtherSplit_pnl;
                    targetPanel.Controls.Clear();
                    // 取得寬度與高度 
                    splitnum = (int)tabigOtherSplit_nud.Value;
                    if (tabigOtherVSplit_rdo.Checked)
                    {
                        blockWidth = targetPanel.Width / splitnum;
                        blockHeight = targetPanel.Height;
                    }
                    else
                    {
                        blockWidth = targetPanel.Width;
                        blockHeight = targetPanel.Height / splitnum;
                    }
                }
                else
                {
                    // 取得並清空面板
                    targetPanel = tabiwCGradOtherSplit_pnl;
                    targetPanel.Controls.Clear();
                    // 取得寬度與高度
                    splitnum = (int)tabiwCGradOtherSplit_nud.Value;
                    splitType = tabiwCGradOtherVSplit_rdo.Checked ? "Vsplit" : "Hplit";
                    if (tabiwCGradOtherVSplit_rdo.Checked)
                    {
                        blockWidth = targetPanel.Width / splitnum;
                        blockHeight = targetPanel.Height;
                    }
                    else
                    {
                        blockWidth = targetPanel.Width;
                        blockHeight = targetPanel.Height / splitnum;
                    }
                }

                Control box = targetPanel.Parent;  // 先記下群組：放大編輯時面板會暫時移到其他視窗
                RadioButton targetRdo = box.Controls.OfType<RadioButton>()
                                    .FirstOrDefault(rdo => rdo.Name.Contains("HSplit_rdo")); // 替換為目標 Label 的名稱

                // 生成分割區塊
                for (int i = 0; i < splitnum; i++)
                {
                    Panel block = new Panel
                    {
                        Size = new Size(blockWidth, blockHeight),
                        Location = targetRdo.Checked
                            ? new Point(0, i * blockHeight)
                            : new Point(i * blockWidth, 0),
                        BorderStyle = BorderStyle.FixedSingle,
                        Tag = $"{(targetRdo.Checked ? "H" : "V")},{i}" // 儲存區塊的方向與索引
                    };

                    // 設定點擊事件以填充顏色
                    block.MouseDown += (s, args) =>
                    {
                        if (args.Button == MouseButtons.Right)
                        {
                            block.BackColor = Color.Transparent;
                        }
                        else if (args.Button == MouseButtons.Left)
                        {
                            Label targetLabel = box.Controls.OfType<Label>()
                            .FirstOrDefault(label => label.Name.Contains("Color_lbl"));
                            block.BackColor = targetLabel.BackColor;
                        }
                    };
                    // 設定滑鼠移入事件以顯示座標
                    block.MouseEnter += (s, args) =>
                    {
                        Label targetLabel = box.Controls.OfType<Label>()
                        .FirstOrDefault(label => label.Name.Contains("Loc_lbl"));
                        targetLabel.Text = $"{block.Tag}"; // 更新 Label 顯示區塊座標
                    };

                    targetPanel.Controls.Add(block);
                }
            }
        }  // Grad Colorbar生成區塊
        private void otherColor_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is Control target)
            {
                if (target.Name.Contains("tabig"))
                {
                    if (tabigColorBar_rdo.Checked)
                    {
                        tabigOther_chk.Checked = true;
                    }
                    if (tabigOther_chk.Checked)
                    {
                        tabigBaseColor_pnl.Enabled = false;
                        tabigOther_grp.Enabled = true; // 顯示顏色選擇面板
                    }
                    else
                    {
                        tabigBaseColor_pnl.Enabled = true;
                        tabigOther_grp.Enabled = false; // 隱藏顏色選擇面板
                    }
                }
                else if (target.Name.Contains("tabiwCGrad"))//
                {
                    //MessageBox.Show("123");
                    if (tabiwCGradColorBar_rdo.Checked)
                    {
                        tabiwCGradOther_chk.Checked = true;
                    }
                    if (tabiwCGradOther_chk.Checked)
                    {
                        tabiwCGradBaseColor_pnl.Enabled = false;
                        tabiwCGradOther_grp.Enabled = true; // 顯示顏色選擇面板
                    }
                    else
                    {
                        tabiwCGradBaseColor_pnl.Enabled = true;
                        tabiwCGradOther_grp.Enabled = false; // 隱藏顏色選擇面板
                    }
                }
            }
        }  // Grad Colorbar顏色選擇
        private void ColorBar_btn_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Name.Contains("tabig"))
                {
                    tabigOtherSplit_nud.Value = button.Name.Contains("FourColor_btn") ? 4 : 8;
                }
                else if (button.Name.Contains("tabiwCGrad"))
                {
                    tabiwCGradOtherSplit_nud.Value = button.Name.Contains("FourColor_btn") ? 4 : 8;
                }
                if (button.Parent is GroupBox parent)
                {
                    Color[] colors = { Color.White, Color.Red, Color.Lime, Color.Blue, Color.Black, Color.Aqua, Color.Yellow, Color.Magenta };
                    int colorIndex = 0;
                    Panel targetpanel = parent.Controls.OfType<Panel>()
                    .FirstOrDefault(panel => panel.Name.Contains("OtherSplit_pnl")); // 替換為目標 Panel 的名稱
                    foreach (Panel panel in targetpanel.Controls)
                    {
                        panel.BackColor = colors[colorIndex % colors.Length];
                        colorIndex++;
                    }
                }
            }

        }  // Grad ColorBar 標準四色/八色
        private async void tabieaTrans_btn_Click(object sender, EventArgs e)
        {
            if (tabieaDiff_rdo.Checked)
            {
                RunImageDiff();  // 差異比對：勾選的兩張圖
                return;
            }
            ssrProgressbar_prg.Visible = true;
            ssrProgressbar_prg.Maximum = tabieaPatternList_chl.CheckedItems.Count;
            ssrProgressbar_prg.Value = 0; // 初始化進度條值

            using (Bitmap bitmap = new Bitmap(int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text)))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    if (tabieaUpDown_rdo.Checked)
                    {
                        // 取得上下圖片
                        Bitmap upperImage = new Bitmap(tabieaUpDownU_pic.Image);
                        Bitmap lowerImage = new Bitmap(tabieaUpDownD_pic.Image);
                        // 建立組合後的圖片
                        // 繪製上方圖片
                        g.DrawImage(upperImage, 0, 0, bitmap.Width, bitmap.Height / 2);
                        // 繪製下方圖片
                        g.DrawImage(lowerImage, 0, bitmap.Height / 2, bitmap.Width, bitmap.Height / 2);
                        // 更新檔案儲存路徑
                        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                        {
                            saveFileDialog.Filter = "BMP|*.bmp|PNG|*.png|JPEG|*.jpg";
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                try
                                {
                                    if (upperImage == null || lowerImage == null)
                                    {
                                        throw new InvalidOperationException("沒有圖片可以儲存！");
                                    }
                                    string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                                    // C# 8.0支援
                                    //ImageFormat imageFormat = extension switch
                                    //{
                                    //    ".bmp" => ImageFormat.Bmp,
                                    //    ".png" => ImageFormat.Png,
                                    //    ".jpg" => ImageFormat.Jpeg,
                                    //    _ => throw new NotSupportedException("不支援的檔案格式！")
                                    //};
                                    // Replace the switch expression with a switch statement to make it compatible with C# 7.3.  
                                    // Original code:  
                                    // ImageFormat imageFormat = extension switch  
                                    // {  
                                    //     ".bmp" => ImageFormat.Bmp,  
                                    //     ".png" => ImageFormat.Png,  
                                    //     ".jpg" => ImageFormat.Jpeg,  
                                    //     _ => throw new NotSupportedException("不支援的檔案格式！")  
                                    // };  

                                    ImageFormat imageFormat;
                                    switch (extension)
                                    {
                                        case ".bmp":
                                            imageFormat = ImageFormat.Bmp;
                                            break;
                                        case ".png":
                                            imageFormat = ImageFormat.Png;
                                            break;
                                        case ".jpg":
                                            imageFormat = ImageFormat.Jpeg;
                                            break;
                                        default:
                                            throw new NotSupportedException("不支援的檔案格式！");
                                    }
                                    bitmap.Save(saveFileDialog.FileName, imageFormat);
                                    //MessageBox.Show("圖片已儲存成功！");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"圖片儲存失敗：{ex.Message}");
                                }
                            }
                        }

                    }
                    else if (tabieaLeftRight_rdo.Checked)
                    {
                        // 取得上下圖片
                        Bitmap leftImage = new Bitmap(tabieaLeftRightL_pic.Image);
                        Bitmap rightImage = new Bitmap(tabieaLeftRightR_pic.Image);
                        // 建立組合後的圖片
                        // 繪製左方圖片
                        g.DrawImage(leftImage, 0, 0, bitmap.Width / 2, bitmap.Height);
                        // 繪製右方圖片
                        g.DrawImage(rightImage, bitmap.Width / 2, 0, bitmap.Width / 2, bitmap.Height);
                        // 更新檔案儲存路徑
                        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                        {
                            saveFileDialog.Filter = "BMP|*.bmp|PNG|*.png|JPEG|*.jpg";
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                try
                                {
                                    if (leftImage == null || rightImage == null)
                                    {
                                        throw new InvalidOperationException("沒有圖片可以儲存！");
                                    }
                                    string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                                    ImageFormat imageFormat;
                                    switch (extension)
                                    {
                                        case ".bmp":
                                            imageFormat = ImageFormat.Bmp;
                                            break;
                                        case ".png":
                                            imageFormat = ImageFormat.Png;
                                            break;
                                        case ".jpg":
                                            imageFormat = ImageFormat.Jpeg;
                                            break;
                                        default:
                                            throw new NotSupportedException("不支援的檔案格式！");
                                    }
                                    ;
                                    //ImageFormat imageFormat = extension switch
                                    //{
                                    //    ".bmp" => ImageFormat.Bmp,
                                    //    ".png" => ImageFormat.Png,
                                    //    ".jpg" => ImageFormat.Jpeg,
                                    //    _ => throw new NotSupportedException("不支援的檔案格式！")
                                    //};
                                    bitmap.Save(saveFileDialog.FileName, imageFormat);
                                    //MessageBox.Show("圖片已儲存成功！");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"圖片儲存失敗：{ex.Message}");
                                }
                            }
                        }
                    }
                    else
                    {
                        int outW = bitmap.Width, outH = bitmap.Height;
                        var done = new List<object>();
                        var failed = new List<string>();
                        foreach (object iteamChecked in tabieaPatternList_chl.CheckedItems)
                        {
                            string sourcePath = iteamChecked.ToString();
                            // 用 Path 取副檔名（資料夾名稱含 . 也不會切錯，大小寫皆可）
                            string extension = Path.GetExtension(sourcePath).ToLower();
                            ImageFormat format = extension == ".bmp" ? ImageFormat.Bmp
                                : extension == ".png" ? ImageFormat.Png
                                : extension == ".jpg" || extension == ".jpeg" ? ImageFormat.Jpeg : null;
                            if (format == null)
                            {
                                failed.Add($"{Path.GetFileName(sourcePath)}：不支援的格式 {extension}");
                                ssrProgressbar_prg.Value++;
                                continue;
                            }
                            try
                            {
                                Bitmap source;
                                using (FileStream stream = File.OpenRead(sourcePath))  // 讀進記憶體後即釋放檔案，不鎖檔
                                using (Image image = Image.FromStream(stream))
                                {
                                    source = new Bitmap(image);
                                }
                                string suffix;
                                using (source)
                                {
                                    g.Clear(Color.Black);  // 每張先清空，避免殘留上一張；留邊處為黑色
                                    if (tabieaResize_rdo.Checked)
                                    {
                                        g.DrawImage(source, ResizeRectangle(source.Size, outW, outH, tabieaResizeMode_cmb.SelectedIndex));
                                        string[] modeSuffix = { "", "Fit_", "Fill_", "1to1_" };
                                        suffix = $"{modeSuffix[Math.Max(0, tabieaResizeMode_cmb.SelectedIndex)]}{outW}x{outH}";
                                    }
                                    else
                                    {
                                        var (rotate, rotateName) =
                                              tabieaRotate90_rdo.Checked ? (RotateFlipType.Rotate90FlipNone, "Rotate90")
                                            : tabieaRotate180_rdo.Checked ? (RotateFlipType.Rotate180FlipNone, "Rotate180")
                                            : tabieaRotate270_rdo.Checked ? (RotateFlipType.Rotate270FlipNone, "Rotate270")
                                            : tabieaHFlip_rdo.Checked ? (RotateFlipType.RotateNoneFlipX, "FlipX")
                                            : (RotateFlipType.RotateNoneFlipY, "FlipY");
                                        source.RotateFlip(rotate);
                                        g.DrawImage(source, 0, 0, outW, outH);
                                        suffix = $"{rotateName}_{outW}x{outH}";
                                    }
                                }
                                string outputPath = Path.Combine(Path.GetDirectoryName(sourcePath), $"{Path.GetFileNameWithoutExtension(sourcePath)}_{suffix}{extension}");
                                bitmap.Save(outputPath, format);
                                done.Add(iteamChecked);
                            }
                            catch (Exception ex)
                            {
                                failed.Add($"{Path.GetFileName(sourcePath)}：{ex.Message}");
                            }
                            ssrProgressbar_prg.Value++;
                        }
                        // 只移除轉換成功的項目，失敗的留在清單上
                        foreach (object item in done)
                        {
                            tabieaPatternList_chl.Items.Remove(item);
                        }
                        if (failed.Count > 0)
                        {
                            MessageBox.Show($"以下 {failed.Count} 個檔案轉換失敗：\r\n\r\n" + string.Join("\r\n", failed), "ImageAdjust", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            ssrProgressbar_prg.Visible = false; // 在迴圈完成後隱藏進度條
        }  // Adjust 調整圖片
        private static Rectangle ResizeRectangle(Size source, int width, int height, int mode)
        {
            // 0 拉伸填滿、1 等比例（留黑邊）、2 填滿裁切（等比例，超出裁掉）、3 1:1 置中
            if (mode == 3) return new Rectangle((width - source.Width) / 2, (height - source.Height) / 2, source.Width, source.Height);
            if (mode == 1 || mode == 2)
            {
                double sx = (double)width / source.Width, sy = (double)height / source.Height;
                double scale = mode == 1 ? Math.Min(sx, sy) : Math.Max(sx, sy);
                int w = (int)Math.Round(source.Width * scale), h = (int)Math.Round(source.Height * scale);
                return new Rectangle((width - w) / 2, (height - h) / 2, w, h);
            }
            return new Rectangle(0, 0, width, height);
        }  // Resize 縮放模式的繪製範圍
        private void InitQuickButtons()
        {
            // Gradient：FirstLevel / LastLevel 右側，數值對調（例 0/255 → 255/0）
            var swap = new Button { Text = "對調", Location = new Point(368, 45), Size = new Size(44, 24) };
            swap.Click += (s, e) =>
            {
                string first = tabigFirstLevel_cmb.Text;
                tabigFirstLevel_cmb.Text = tabigLastLevel_cmb.Text;
                tabigLastLevel_cmb.Text = first;
            };
            toolTip.SetToolTip(swap, "FirstLevel 與 LastLevel 數值對調（例：0 / 255 → 255 / 0）");
            tabimgeGradient.Controls.Add(swap);
            swap.BringToFront();

            // Chess：灰階捲軸下方，快速設定 水平×垂直 格數
            var sizes = new (int h, int v)[] { (2, 2), (5, 5), (8, 8), (32, 32), (8, 5) };
            for (int i = 0; i < sizes.Length; i++)
            {
                var (h, v) = sizes[i];
                var button = new Button { Text = $"{h}×{v}", Location = new Point(85 + i * 52, 148), Size = new Size(50, 24) };
                button.Click += (s, e) =>
                {
                    tabiecHNum_nud.Value = Math.Min(h, tabiecHNum_nud.Maximum);
                    tabiecVNum_nud.Value = Math.Min(v, tabiecVNum_nud.Maximum);
                };
                toolTip.SetToolTip(button, $"水平 {h} 格、垂直 {v} 格");
                tabimgeChess.Controls.Add(button);
                button.BringToFront();
            }
        }  // Gradient 對調、Chess 快速格數
        private ComboBox tabieaResizeMode_cmb;
        private void InitResizeMode()
        {
            // ReSize 右側：縮放模式（選 ReSize 時才可用）
            tabieaResizeMode_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(75, 42), Width = 85, ImeMode = ImeMode.Disable };
            tabieaResizeMode_cmb.Items.AddRange(new object[] { "拉伸填滿", "等比例", "填滿裁切", "1:1 置中" });
            tabieaResizeMode_cmb.SelectedIndex = 0;
            toolTip.SetToolTip(tabieaResizeMode_cmb, "拉伸填滿：拉成目前寬高\r\n等比例：完整顯示，留黑邊\r\n填滿裁切：等比例填滿，超出裁掉\r\n1:1 置中：原尺寸置中，不縮放");
            tabimgeAdjust.Controls.Add(tabieaResizeMode_cmb);
            tabieaResizeMode_cmb.BringToFront();
            void Update() => tabieaResizeMode_cmb.Enabled = tabieaResize_rdo.Checked;
            tabieaResize_rdo.CheckedChanged += (s, e) => Update();
            Update();
        }  // 建立縮放模式選單
        private void windowPictureBox_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox targetPictureBox)
            {
                if (targetPictureBox.Name == "tabiwBack_pic")
                {
                    if (tabiwBackColor_rdo.Checked)
                    {
                        using (ColorDialog colorDialog = new PatternColorDialog())
                        {
                            colorDialog.FullOpen = true; // 顯示進階選項  
                                                         // 顯示顏色選擇對話框  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.BackColor = colorDialog.Color;
                                targetPictureBox.Image = null; // 清除按鈕上的圖片
                            }
                        }
                    }
                    else if (tabiwBackImg_rdo.Checked)
                    {
                        using (OpenFileDialog openFileDialog = new OpenFileDialog())
                        {
                            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.Image = Image.FromFile(openFileDialog.FileName);
                                targetPictureBox.BackColor = Color.Transparent;
                                targetPictureBox.Text = "";
                            }
                        }
                    }
                    else if (tabiwBackCustom_rdo.Checked)
                    {
                        Custom_btn_Click(sender, e); // 觸發 Custom 按鈕的事件
                    }
                }
                else if (targetPictureBox.Name == "tabiwWin_pic")
                {
                    if (tabiwWinColor_rdo.Checked)
                    {
                        using (ColorDialog colorDialog = new PatternColorDialog())
                        {
                            colorDialog.FullOpen = true; // 顯示進階選項  
                                                         // 顯示顏色選擇對話框  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.BackColor = colorDialog.Color;
                                targetPictureBox.Image = null; // 清除按鈕上的圖片
                            }
                        }
                    }
                    else if (tabiwWinImg_rdo.Checked)
                    {
                        using (OpenFileDialog openFileDialog = new OpenFileDialog())
                        {
                            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.Image = Image.FromFile(openFileDialog.FileName);
                                targetPictureBox.BackColor = Color.Transparent;
                                targetPictureBox.Text = "";
                            }
                        }
                    }
                    else if (tabiwBackCustom_rdo.Checked)
                    {
                        Custom_btn_Click(sender, e); // 觸發 Custom 按鈕的事件
                    }
                }
            }
            else if (sender is RadioButton targetRadio)
            {
                if (targetRadio.Name.Contains("Color_rdo"))
                {
                    using (ColorDialog colorDialog = new PatternColorDialog())
                    {
                        colorDialog.FullOpen = true; // 顯示進階選項  
                                                     // 顯示顏色選擇對話框  
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            if (targetRadio.Name.Contains("Back"))
                            {
                                tabiwBack_pic.BackColor = colorDialog.Color;
                                tabiwBack_pic.Image = null; // 清除按鈕上的圖片
                            }
                            else
                            {
                                tabiwWin_pic.BackColor = colorDialog.Color;
                                tabiwWin_pic.Image = null; // 清除按鈕上的圖片
                            }
                        }
                    }
                    tabiwCustom_grp.Enabled = false;
                }
                else if (targetRadio.Name.Contains("Img_rdo"))
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            if (targetRadio.Name.Contains("Back"))
                            {
                                tabiwBack_pic.Image = Image.FromFile(openFileDialog.FileName);
                                tabiwBack_pic.BackColor = Color.Transparent;
                            }
                            else
                            {
                                tabiwWin_pic.Image = Image.FromFile(openFileDialog.FileName);
                                tabiwWin_pic.BackColor = Color.Transparent;
                            }
                        }
                    }
                    tabiwCustom_grp.Enabled = false;
                }
                else if (targetRadio.Name.Contains("Custom_rdo"))
                {
                    tabiwCustom_grp.Enabled = true;
                    tabiwCustom_btn.Text = targetRadio.Name.Contains("Back") ? "背景生成" : "窗口生成";
                }
            }
        }  // Window 純色、圖片、自製選擇
        private void tabieaPatternList_chL_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (sender is CheckedListBox targetcheck && targetcheck.Text != "")
            {
                if (showimgPicture_pic.Image != null)
                {
                    //showimgPicture_pic.Image.Dispose();
                    showimgPicture_pic.Image = null;
                }
                showimgPicture_pic.Image = Image.FromFile(targetcheck.Text);
                showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }  // Adjust 圖片列表選擇
        private void tabieaClear_btn_Click(object sender, EventArgs e)
        {
            tabieaPatternList_chl.Items.Clear();
        }  // Adjust 清除圖片列表
        private void tabiea_rdo_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton targetradio)
            {
                if (targetradio.Name == "tabieaUpDown_rdo")
                {
                    tabieaImport_btn.Enabled = false;
                    tabieaClear_btn.Enabled = false;
                    tabieaPatternList_chl.Visible = false;
                    tabieaLeftRight_pnl.Visible = false;
                    tabieaUpDown_pnl.Dock = DockStyle.Fill;
                    tabieaUpDown_pnl.Visible = true;

                }
                else if (targetradio.Name == "tabieaLeftRight_rdo")
                {
                    tabieaImport_btn.Enabled = false;
                    tabieaClear_btn.Enabled = false;
                    tabieaPatternList_chl.Visible = false;
                    tabieaUpDown_pnl.Visible = false;
                    tabieaLeftRight_pnl.Dock = DockStyle.Fill;
                    tabieaLeftRight_pnl.Visible = true;
                }
                else
                {
                    tabieaImport_btn.Enabled = true;
                    tabieaClear_btn.Enabled = true;
                    tabieaUpDown_pnl.Visible = false;
                    tabieaLeftRight_pnl.Visible = false;
                    tabieaPatternList_chl.Dock = DockStyle.Fill;
                    tabieaPatternList_chl.Visible = true;
                }
            }
        }  // Adjust 調整功能選擇
        private void tabiea_pic_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox targetPicture)
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        targetPicture.Image = Image.FromFile(openFileDialog.FileName);
                    }
                }
        }  // Adjust 圖片選擇
        private void MaskPanelCreat(object sender, EventArgs e)
        {
            if (sender is Control control && control != null)
            {
                Panel targetPanel;
                int width;
                int height;
                int blockWidth;
                int blockHeight;
                string pixelType;
                VScrollBar brightnessvsc;
                GroupBox box;  // 顏色按鈕與座標標籤所在的群組（面板放大編輯時 Parent 會暫時改變，不能用 Parent 找）
                if (control.Parent != null && control.Parent.Name == "tabimgeMask")
                {
                    box = tabimgeMask;
                    // 取得並清空面板
                    targetPanel = tabiemPixelPanel_pnl;
                    targetPanel.Controls.Clear();
                    // 取得寬度與高度
                    width = (int)tabiemWNum_nud.Value;
                    height = (int)tabiemHNum_nud.Value;
                    pixelType = tabiemPixel_rdo.Checked ? "Pixel" : "SubPixel";
                    // 設定亮度來源
                    brightnessvsc = tabiemPixelGray_vsc;
                }
                else
                {
                    box = tabiwCustomMask_grp;
                    // 取得並清空面板
                    targetPanel = tabiwCMaskPixelPanel_pnl;
                    targetPanel.Controls.Clear();
                    // 取得寬度與高度
                    width = (int)tabiwCMaskWNum_nud.Value;
                    height = (int)tabiwCMaskHNum_nud.Value;
                    pixelType = tabiwCMaskPixel_rdo.Checked ? "Pixel" : "SubPixel";
                    // 設定亮度來源
                    brightnessvsc = tabiwCMaskPixelGray_vsc;
                }
                // 設定每個區塊的H
                blockHeight = targetPanel.Height / height;

                if (pixelType == "Pixel")
                {
                    // 設定每個區塊的W
                    blockWidth = targetPanel.Width / width;
                    // 生成Pixel區塊
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Panel block = new Panel
                            {
                                Size = new Size(blockWidth, blockHeight),
                                Location = new Point(x * blockWidth, y * blockHeight),
                                BorderStyle = BorderStyle.FixedSingle,
                                Tag = $"{x},{y}" // 儲存區塊的座標
                            };

                            // 設定點擊事件以填充色 設定滑鼠右鍵事件以清除顏色
                            block.MouseDown += (s, args) =>
                            {
                                if (args.Button == MouseButtons.Right)
                                {
                                    block.BackColor = Color.Transparent;
                                }
                                else if (args.Button == MouseButtons.Left)
                                {
                                    Button targetButton = box.Controls.OfType<Button>()
                                    .FirstOrDefault(button => button.Name.Contains("PixelColor"));
                                    block.BackColor = targetButton.BackColor;
                                }
                            };
                            block.MouseEnter += (s, args) =>
                            {
                                Label targetLabel = box.Controls.OfType<Label>()
                                .FirstOrDefault(label => label.Name.Contains("Loc_lbl"));
                                targetLabel.Text = $"{block.Tag}"; // 更新 Label 顯示區塊座標
                            };

                            targetPanel.Controls.Add(block);
                        }
                    }
                }
                else if (pixelType == "SubPixel")
                {
                    width = width * 3;
                    // 設定每個區塊的W
                    blockWidth = targetPanel.Width / width;

                    // 生成SubPixel區塊
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Panel block = new Panel
                            {
                                Size = new Size(blockWidth, blockHeight),
                                Location = new Point(x * blockWidth, y * blockHeight),
                                BackColor = SubPixelColumnColor(x, 255 - brightnessvsc.Value),
                                BorderStyle = BorderStyle.FixedSingle,
                                Tag = $"{x},{y}" // 儲存區塊的座標
                            };

                            // 設定右鍵點擊事件以填充色 設定滑鼠左鍵事件以清除顏色
                            block.MouseDown += (s, args) =>
                            {
                                if (args.Button == MouseButtons.Right)
                                {
                                    int locx = int.Parse(block.Tag.ToString().Split(',')[0]);
                                    block.BackColor = SubPixelColumnColor(locx, 255 - brightnessvsc.Value);
                                }
                                else if (args.Button == MouseButtons.Left)
                                {
                                    block.BackColor = Color.Black;
                                }
                            };
                            block.MouseEnter += (s, args) =>
                            {
                                Label targetLabel = box.Controls.OfType<Label>()
                                .FirstOrDefault(label => label.Name.Contains("PixelLoc_lbl"));
                                targetLabel.Text = $"{block.Tag}"; // 更新 Label 顯示區塊座標
                            };
                            targetPanel.Controls.Add(block);
                        }
                    }
                }
            }
        }  // Mask 生成區塊
        private void Pixel_rdo_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton targetradio)
            {
                if (targetradio.Parent is GroupBox parentbox)
                {
                    VScrollBar targetVsc = parentbox.Controls.OfType<VScrollBar>()
                    .FirstOrDefault(vsc => vsc.Name.Contains("PixelGray"));
                    targetVsc.Visible = targetradio.Name.Contains("SubPixel");

                    Label targetLbl = parentbox.Controls.OfType<Label>()
                    .FirstOrDefault(lbl => lbl.Name.Contains("PixelGray"));
                    targetLbl.Visible = targetradio.Name.Contains("SubPixel");

                    targetLbl = parentbox.Controls.OfType<Label>()
                    .FirstOrDefault(lbl => lbl.Name.Contains("PixelColor"));
                    targetLbl.Visible = !targetradio.Name.Contains("SubPixel");

                    Button targetBtn = parentbox.Controls.OfType<Button>()
                    .FirstOrDefault(btn => btn.Name.Contains("PixelColor"));
                    targetBtn.Visible = !targetradio.Name.Contains("SubPixel");
                }
            }
            MaskPanelCreat(sender, e);
        }  // Mask Pixel/SubPixel選擇 切換控件
        private void PixelGray_vsc_ValueChanged(object sender, EventArgs e)
        {
            if (sender is VScrollBar targetvsc)
            {
                if (targetvsc.Parent is GroupBox parentbox)
                {
                    Label targetLbl = parentbox.Controls.OfType<Label>()
                    .FirstOrDefault(lbl => lbl.Name.Contains("PixelGray"));
                    targetLbl.Text = $"Gray: {255 - targetvsc.Value}"; // 更新 Label 顯示當前值

                    Panel targetPnl = parentbox.Controls.OfType<Panel>()
                    .FirstOrDefault(pnl => pnl.Name.Contains("PixelPanel"));
                    foreach (Control control in targetPnl.Controls)
                    {
                        if (control is Panel panel && panel.BackColor != Color.Black)
                        {
                            int x = int.Parse(panel.Tag.ToString().Split(',')[0]);
                            panel.BackColor = SubPixelColumnColor(x, 255 - targetvsc.Value);
                        }
                    }
                }
            }
        }  // Mask 亮度調整
        private void tabiwCustom_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox targetcombo)
            {
                if (targetcombo.Name == "tabiwCustom_cmb")
                {
                    if (targetcombo.Text == "Mask")
                    {
                        tabiwCustomGrad_grp.Visible = false;
                        tabiwCustomMask_grp.Visible = true;
                    }
                    else
                    {
                        tabiwCustomMask_grp.Visible = false;
                        tabiwCustomGrad_grp.Visible = true;
                    }
                }
            }
        }  // Window Mask/Grad 選擇
        private void Custom_btn_Click(object sender, EventArgs e)
        {
            int width = 0;
            int height = 0;
            if (!int.TryParse(mnsW_txt.Text, out width) || !int.TryParse(mnsH_txt.Text, out height))
            {
                MessageBox.Show("請輸入有效的寬度與高度！");
                return;
            }
            if (width != 0 || height != 0)
            {
                if (tabiwCustom_btn.Text == "窗口生成")
                {
                    if (tabiwWinSizePercent_rdo.Checked)
                    {
                        width = width * (int)tabiwWinSizePercent_nud.Value / 100;
                        height = height * (int)tabiwWinSizePercent_nud.Value / 100;
                    }
                    else
                    {
                        width = (int)tabiwWinSizePixelW_nud.Value;
                        height = (int)tabiwWinSizePixelH_nud.Value;
                    }
                }
            }
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                if (tabiwCustom_cmb.Text == "Mask")
                {
                    int pixelW = (int)tabiwCMaskWNum_nud.Value;
                    int pixelH = (int)tabiwCMaskHNum_nud.Value;
                    if (pixelW * pixelH <= 9216)  //太小的像素要加速
                    {
                        int minhw = Math.Min(256 / pixelW, 144 / pixelH);
                        pixelW = minhw * pixelW;
                        pixelH = minhw * pixelH;
                        if (tabiwCMaskPixel_rdo.Checked)
                        {
                            using (Bitmap pixelimage = new Bitmap(pixelW, pixelH))
                            {
                                // 鎖定位圖的像素數據
                                BitmapData pixelData = pixelimage.LockBits(
                                    new Rectangle(0, 0, pixelimage.Width, pixelimage.Height),
                                    ImageLockMode.WriteOnly,
                                    PixelFormat.Format32bppArgb);

                                int stride = pixelData.Stride;
                                IntPtr ptr = pixelData.Scan0;
                                byte[] pixelBuffer = new byte[stride * pixelH];

                                // 初始化像素數據
                                foreach (Control control in tabiwCMaskPixelPanel_pnl.Controls)
                                {
                                    if (control is Panel panel)
                                    {
                                        // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                        string[] coordinates = panel.Tag.ToString().Split(',');
                                        int x = int.Parse(coordinates[0]);
                                        int y = int.Parse(coordinates[1]);

                                        for (int i = 0; i < pixelH; i += (int)tabiwCMaskHNum_nud.Value)
                                        {
                                            for (int j = 0; j < pixelW; j += (int)tabiwCMaskWNum_nud.Value)
                                            {
                                                // 計算像素在數組中的索引
                                                //int index = (y * stride) + (x * 4);
                                                int index = (y * stride) + (x * 4) + i * stride + j * 4;

                                                // 設置像素顏色
                                                Color color = panel.BackColor;
                                                pixelBuffer[index] = color.B;       // 藍色
                                                pixelBuffer[index + 1] = color.G;  // 綠色
                                                pixelBuffer[index + 2] = color.R;  // 紅色
                                                pixelBuffer[index + 3] = color.A;  // 透明度
                                            }
                                        }
                                    }
                                }

                                // 將像素數據寫回位圖
                                System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, 0, ptr, pixelBuffer.Length);
                                pixelimage.UnlockBits(pixelData);

                                // 將 pixelimage 填滿 bitmap
                                for (int j = 0; j < bitmap.Height; j += pixelH)
                                {
                                    for (int i = 0; i < bitmap.Width; i += pixelW)
                                    {
                                        g.DrawImage(pixelimage, i, j);
                                    }
                                }
                            }
                        }
                        else if (tabiwCMaskSubPixel_rdo.Checked)
                        {
                            using (Bitmap pixelimage = new Bitmap(pixelW, pixelH))
                            {
                                // 鎖定位圖的像素數據
                                BitmapData pixelData = pixelimage.LockBits(
                                    new Rectangle(0, 0, pixelimage.Width, pixelimage.Height),
                                    ImageLockMode.WriteOnly,
                                    PixelFormat.Format32bppArgb);

                                int stride = pixelData.Stride;
                                IntPtr ptr = pixelData.Scan0;
                                byte[] pixelBuffer = new byte[stride * pixelH];

                                // 初始化像素數據
                                for (int k = 0; k < tabiwCMaskPixelPanel_pnl.Controls.Count; k += 3)
                                {
                                    if (k + 2 < tabiwCMaskPixelPanel_pnl.Controls.Count)
                                    {
                                        // 獲取三個 Panel
                                        Panel panel1 = tabiwCMaskPixelPanel_pnl.Controls[k] as Panel;
                                        Panel panel2 = tabiwCMaskPixelPanel_pnl.Controls[k + 1] as Panel;
                                        Panel panel3 = tabiwCMaskPixelPanel_pnl.Controls[k + 2] as Panel;

                                        if (panel1 != null && panel2 != null && panel3 != null)
                                        {
                                            //// 根據 Panel 的背景顏色組合成 Color
                                            //int subpixelr = panel1.BackColor.R;
                                            //int subpixelg = panel2.BackColor.G;
                                            //int subpixelb = panel3.BackColor.B;

                                            // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                            string[] coordinates = panel1.Tag.ToString().Split(',');
                                            int x = int.Parse(coordinates[0]) / 3;
                                            int y = int.Parse(coordinates[1]);
                                            for (int i = 0; i < pixelH; i += (int)tabiemHNum_nud.Value)
                                            {
                                                for (int j = 0; j < pixelW; j += (int)tabiemWNum_nud.Value)
                                                {
                                                    // 計算像素在數組中的索引
                                                    //int index = (y * stride) + (x * 4);
                                                    int index = (y * stride) + (x * 4) + i * stride + j * 4;

                                                    // 設置像素顏色
                                                    Color combinedColor = CombineSubPixels(panel1, panel2, panel3);  // 三格各自的色版合成一個像素（RGB / BGR 皆可）
                                                    pixelBuffer[index] = combinedColor.B;      // 藍色
                                                    pixelBuffer[index + 1] = combinedColor.G;  // 綠色
                                                    pixelBuffer[index + 2] = combinedColor.R;  // 紅色
                                                    pixelBuffer[index + 3] = 255;  // 透明度不透明
                                                }
                                            }
                                        }
                                    }
                                }

                                // 將像素數據寫回位圖
                                System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, 0, ptr, pixelBuffer.Length);
                                pixelimage.UnlockBits(pixelData);

                                // 將 pixelimage 填滿 bitmap
                                for (int j = 0; j < bitmap.Height; j += pixelH)
                                {
                                    for (int i = 0; i < bitmap.Width; i += pixelW)
                                    {
                                        g.DrawImage(pixelimage, i, j);
                                    }
                                }
                            }
                        }
                    }
                    else  //不加速
                    { 
                        pixelW = (int)tabiwCMaskWNum_nud.Value;
                        pixelH = (int)tabiwCMaskHNum_nud.Value;
                        Panel targetPanel = tabiwCMaskPixelPanel_pnl;
                        // 使用遮罩圖片
                        using (Bitmap pixelimage = new Bitmap(pixelW, pixelH))
                        {
                            if (tabiwCMaskPixel_rdo.Checked)
                            {
                                // 遍歷 tabiemPixelPanel_pnl 的子 Panel  
                                foreach (Control control in targetPanel.Controls)
                                {
                                    if (control is Panel panel)
                                    {
                                        // 解析 Panel 的座標 (Tag 格式為 "x,y")  
                                        string[] coordinates = panel.Tag.ToString().Split(',');
                                        int x = int.Parse(coordinates[0]);
                                        int y = int.Parse(coordinates[1]);

                                        // 使用 Panel 的背景顏色填充對應的像素  
                                        pixelimage.SetPixel(x, y, panel.BackColor);
                                    }
                                }
                            }
                            else if (tabiwCMaskSubPixel_rdo.Checked)
                            {
                                // 遍歷 tabiemPixelPanel_pnl 的子 Panel，每三個一組組合成 Color.FromArgb() 的三個參數  
                                for (int i = 0; i < targetPanel.Controls.Count; i += 3)
                                {
                                    if (i + 2 < targetPanel.Controls.Count)
                                    {
                                        // 獲取三個 Panel  
                                        Panel panel1 = targetPanel.Controls[i] as Panel;
                                        Panel panel2 = targetPanel.Controls[i + 1] as Panel;
                                        Panel panel3 = targetPanel.Controls[i + 2] as Panel;

                                        if (panel1 != null && panel2 != null && panel3 != null)
                                        {
                                            // 根據 Panel 的背景顏色組合成 Color  
                                            Color combinedColor = CombineSubPixels(panel1, panel2, panel3);  // 三格各自的色版合成一個像素（RGB / BGR 皆可）

                                            // 解析 Panel 的座標 (Tag 格式為 "x,y")  
                                            string[] coordinates = panel1.Tag.ToString().Split(',');
                                            int x = int.Parse(coordinates[0]) / 3;
                                            int y = int.Parse(coordinates[1]);
                                            // 在這裡可以使用 combinedColor，例如：  
                                            pixelimage.SetPixel(x, y, combinedColor);
                                        }
                                    }
                                }
                            }
                            // 將 pixelimage 填滿 currentBitmap  
                            for (int j = 0; j < bitmap.Height; j += pixelH)
                            {
                                for (int i = 0; i < bitmap.Width; i += pixelW)
                                {
                                    g.DrawImage(pixelimage, i, j);
                                }
                            }
                        }
                    }
                }
                else if (tabiwCustom_cmb.Text == "Gradient")
                {
                    int divisions = int.Parse(tabiwCGradDivid_cmb.Text); // 獲取畫面等分數  
                    int divisionSize = tabiwCGradHWay_rdo.Checked ? width / divisions : height / divisions; // 計算每個分區的寬度或高度
                    if (tabiwCGradOther_chk.Checked) // 如果選擇了colorbar  
                    {
                        int blockHeight = height / divisions;
                        int blockWidth = width / divisions;
                        int split = (int)tabiwCGradOtherSplit_nud.Value;
                        for (int i = 0; i < divisions; i++)
                        {
                            foreach (Control control in tabiwCGradOtherSplit_pnl.Controls)
                            {
                                if (control is Panel panel && panel.BackColor != Color.Transparent)
                                {
                                    string[] tagParts = panel.Tag.ToString().Split(',');
                                    if (tagParts.Length == 2)
                                    {
                                        string direction = tagParts[0];
                                        int index = int.Parse(tagParts[1]);
                                        if (direction == "H") // 水平分割  
                                        {
                                            if (tabiwCGradHWay_rdo.Checked)
                                            {
                                                g.FillRectangle(new SolidBrush(panel.BackColor), 0, index * blockHeight / split + i * blockHeight, width, blockHeight / split);
                                            }
                                            else
                                            {
                                                g.FillRectangle(new SolidBrush(panel.BackColor), 0, index * height / split, width, height / split);
                                            }
                                        }
                                        else if (direction == "V") // 垂直分割  
                                        {
                                            if (tabiwCGradVWay_rdo.Checked)
                                            {
                                                g.FillRectangle(new SolidBrush(panel.BackColor), index * blockWidth / split + i * blockWidth, 0, blockWidth / split, height);
                                            }
                                            else
                                            {
                                                //MessageBox.Show("123");
                                                g.FillRectangle(new SolidBrush(panel.BackColor), index * width / split, 0, width / split, height);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        g.Clear(tabiwCGradBaseColor_lbl.BackColor); // 填滿整個backBitmap  
                    }
                    if (!tabiwCGradColorBar_rdo.Checked)
                    {
                        int stepNum = Math.Min(int.Parse(tabiwCGradStep_cmb.Text), divisionSize); // 獲取漸層階數
                        int startColorValue = int.Parse(tabiwCGradFirstLevel_cmb.Text); // 開始顏色值  
                        int endColorValue = int.Parse(tabiwCGradLastLevel_cmb.Text); // 結束顏色值  
                        bitmap = ApplyTransparency(bitmap, startColorValue, endColorValue, tabiwCGradHWay_rdo.Checked, stepNum, divisions, tabiwCGradReverse_chk.Checked);
                        bitmap = ConvertARGBToRGB(bitmap);
                    }

                    // 使用漸層顏色
                }
            }
            if (tabiwCustom_btn.Text == "背景生成")
            {
                tabiwBack_pic.Image = bitmap;
            }
            else
            {
                tabiwWin_pic.Image = bitmap;
            }
        }  // Window 自製圖
        private void tabiwDiagonal_btn_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox targetchk)
            {
                if (targetchk.Checked)
                {
                    tabiwWinLoc_grp.Enabled = false;
                    tabiwWinImg_rdo.Enabled = false;
                    tabiwWinCustom_rdo.Enabled = false;
                }
                else
                {
                    tabiwWinLoc_grp.Enabled = true;
                    tabiwWinImg_rdo.Enabled = true;
                    tabiwWinCustom_rdo.Enabled = true;
                }
            }
        }  // Window對角線選擇
        private void tabiedObjectStrip_rdo_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton targetRdo)
            {
                if (targetRdo.Checked)
                {
                    tabiedMoveH_rdo.Checked = true;
                    tabiedMoveBevel_rdo.Enabled = false;
                }
                else
                {
                    tabiedMoveBevel_rdo.Enabled = true;
                }
            }
        }  //Dymenic物件移動方式選擇
        private void DrawDynamicObject(Graphics g, int width, int height)
        {
            // 依 Dynamic 設定在目前位置畫出物件（動畫與預覽共用）
            using (Brush brush = new SolidBrush(tabiedObjectColor_btn.BackColor))
            {
                if (tabiedObjectBall_rdo.Checked)
                {
                    g.FillEllipse(brush, mypicture.startX, mypicture.startY, mypicture.objectSize * 2, mypicture.objectSize * 2);
                }
                else if (tabiedObjectCube_rdo.Checked)
                {
                    g.FillRectangle(brush, mypicture.startX, mypicture.startY, mypicture.objectSize * 2, mypicture.objectSize * 2);
                }
                else if (tabiedObjectBlur_rdo != null && tabiedObjectBlur_rdo.Checked)
                {
                    DrawBlurBar(g, brush, width, height);  // 模糊測試條
                }
                else if (tabiedObjectStrip_rdo.Checked)
                {
                    if (tabiedMoveH_rdo.Checked)
                    {
                        g.FillRectangle(brush, mypicture.startX, 0, mypicture.objectSize, height);
                    }
                    else if (tabiedMoveV_rdo.Checked)
                    {
                        g.FillRectangle(brush, 0, mypicture.startY, width, mypicture.objectSize);
                    }
                }
            }
        }  // 畫動態物件
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // 更新球體位置
            string direction = "b";
            if (tabiedMoveH_rdo.Checked)
            {
                direction = "h";
            }
            else if (tabiedMoveV_rdo.Checked)
            {
                direction = "v";
            }
            else if (tabiedMoveBevel_rdo.Checked)
            {
                direction = "b";
            }
            mypicture.Move(direction, DynamicIsLine);  // 長條與模糊條在邊緣反彈的方式相同;

            Bitmap bitmap;
            if (timerDynamic.Tag != null && timerDynamic.Tag.ToString() == "openpic")
            {
                if (showimgPicture_pic.Image == null)
                {
                    mainWindow.Instance.timerDynamic.Stop();
                    return;
                }
                bitmap = new Bitmap(showimgPicture_pic.Image);
            }
            else
            {
                if (mypicture.Getpicture() != null)
                {
                    // 繪製動畫
                    //Bitmap bitmap = (Bitmap)mypicture.Getpicture().Clone();
                    bitmap = new Bitmap(mypicture.Getpicture());
                    //bitmap = _pictureWindow.getpicturebox();
                }
                else
                { 
                    bitmap = new Bitmap(int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text));
                }
            }

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                DrawDynamicObject(g, bitmap.Width, bitmap.Height);
            }

            // 更新到 PictureBox
            if (_pictureWindow != null)
            {
                _pictureWindow.UpdatePictureBox(bitmap);
                timerDynamic.Tag = "run";
            }
            else
            {
                throw new InvalidOperationException("沒有圖片可以顯示！");
            }
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
        }  // AnimationTimer_Tick
    }
}
