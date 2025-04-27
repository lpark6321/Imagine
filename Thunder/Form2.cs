using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Thunder
{
    public partial class Form2 : Form
    {
        private Bitmap _bitmap;

        public Form2(Bitmap bitMap)
        {
            InitializeComponent();
            _bitmap = bitMap; // 將接收到的 Bitmap 儲存到類別屬性中
            this.Paint += Form2_show; // 綁定 Paint 事件  
        }

        private void Form2_show(object sender, PaintEventArgs e)
        {
            if (_bitmap != null)
            {
                e.Graphics.DrawImage(_bitmap, 0, 0); // 將 Bitmap 繪製到 Form2 上
            }
        }
        // 修正 Form2_Paint 方法的簽名以符合 PaintEventHandler 委派  
        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            // 示例：在 Paint 事件中執行繪圖邏輯  
            Bitmap background = new Bitmap(Width, Height);
            List<Color> gradientColors = new List<Color> { Color.FromArgb(0,0,0),Color.FromArgb(255,0,0)};
            int divisions = 2;
            int stepsPerDivision = 16; // 每個分區的步驟數
            bool isHorizontal = true;

            Bitmap result = GenerateSegmentedGradient(background, gradientColors, divisions, stepsPerDivision, isHorizontal);
            e.Graphics.DrawImage(result, 0, 0);
        }

        // 將原本的 Form2_Paint 方法重命名為 GenerateGradientBitmap  
        private Bitmap GenerateSegmentedGradient(Bitmap background, List<Color> gradientColors, int divisions, int stepsPerDivision, bool isHorizontal)
        {
            // 確保背景圖片大小
            int width = background.Width;
            int height = background.Height;

            Bitmap output = new Bitmap(width, height);

            // 計算每個分區的大小
            int divisionSize = isHorizontal ? width / divisions : height / divisions;

            using (Graphics g = Graphics.FromImage(output))
            {
                // 繪製背景圖片
                g.DrawImage(background, 0, 0, width, height);
            }

            // 生成分段式漸層
            for (int division = 0; division < divisions; division++)
            {
                Color startColor = gradientColors[division % gradientColors.Count];
                Color endColor = gradientColors[(division + 1) % gradientColors.Count];

                for (int step = 0; step < stepsPerDivision; step++)
                {
                    float ratio = (float)step / stepsPerDivision;

                    // 計算當前顏色
                    int r = (int)(startColor.R * (1 - ratio) + endColor.R * ratio);
                    int g = (int)(startColor.G * (1 - ratio) + endColor.G * ratio);
                    int b = (int)(startColor.B * (1 - ratio) + endColor.B * ratio);
                    int a = (int)(startColor.A * (1 - ratio) + endColor.A * ratio);

                    Color currentColor = Color.FromArgb(a, r, g, b);

                    // 繪製當前分段
                    if (isHorizontal)
                    {
                        int xStart = division * divisionSize + (step * divisionSize / stepsPerDivision);
                        int xEnd = xStart + (divisionSize / stepsPerDivision);

                        for (int y = 0; y < height; y++)
                        {
                            for (int x = xStart; x < xEnd && x < width; x++)
                            {
                                output.SetPixel(x, y, BlendWithBackground(output.GetPixel(x, y), currentColor));
                            }
                        }
                    }
                    else
                    {
                        int yStart = division * divisionSize + (step * divisionSize / stepsPerDivision);
                        int yEnd = yStart + (divisionSize / stepsPerDivision);

                        for (int x = 0; x < width; x++)
                        {
                            for (int y = yStart; y < yEnd && y < height; y++)
                            {
                                output.SetPixel(x, y, BlendWithBackground(output.GetPixel(x, y), currentColor));
                            }
                        }
                    }
                }
            }

            return output;
        }

        // 混合前景色與背景色
        private Color BlendWithBackground(Color background, Color foreground)
        {
            int r = (foreground.R * foreground.A / 255) + (background.R * (255 - foreground.A) / 255);
            int g = (foreground.G * foreground.A / 255) + (background.G * (255 - foreground.A) / 255);
            int b = (foreground.B * foreground.A / 255) + (background.B * (255 - foreground.A) / 255);

            return Color.FromArgb(r, g, b);
        }

    }
}