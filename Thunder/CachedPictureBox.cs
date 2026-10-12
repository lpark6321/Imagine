using System;
using System.Drawing;
using System.Windows.Forms;

namespace Thunder
{
    // 主視窗右側的預覽：Image 仍是原始的全解析度圖（匯出、縮圖等功能直接取用），
    // 但只在換圖或預覽框大小改變時縮小一次並快取，之後重畫只貼快取，不必每次把整張大圖縮放
    public class CachedPictureBox : PictureBox
    {
        private Bitmap _cache;
        private Image _cacheSource;
        private Size _cacheSize;

        protected override void OnPaint(PaintEventArgs pe)
        {
            Image image = Image;
            if (image == null || SizeMode != PictureBoxSizeMode.Zoom)
            {
                base.OnPaint(pe);
                return;
            }
            Size imageSize;
            try
            {
                imageSize = image.Size;  // 圖片已被釋放時會丟 ArgumentException
            }
            catch (ArgumentException)
            {
                return;  // 已釋放的圖不畫（之後換上新圖會再重畫）
            }
            Rectangle rect = ZoomRectangle(imageSize);
            if (rect.Width <= 0 || rect.Height <= 0) return;
            if (_cache == null || !ReferenceEquals(image, _cacheSource) || rect.Size != _cacheSize)
            {
                _cache?.Dispose();
                _cache = new Bitmap(rect.Width, rect.Height);
                using (Graphics g = Graphics.FromImage(_cache))
                {
                    g.DrawImage(image, 0, 0, rect.Width, rect.Height);  // 與 PictureBox 原本的縮放方式相同（預設內插）
                }
                _cacheSource = image;
                _cacheSize = rect.Size;
            }
            pe.Graphics.DrawImageUnscaled(_cache, rect.Location);
        }  // 畫快取的縮圖

        private Rectangle ZoomRectangle(Size imageSize)
        {
            // 與 PictureBoxSizeMode.Zoom 相同：等比例置中
            Rectangle client = ClientRectangle;
            float ratio = Math.Min((float)client.Width / imageSize.Width, (float)client.Height / imageSize.Height);
            int width = (int)(imageSize.Width * ratio), height = (int)(imageSize.Height * ratio);
            return new Rectangle(client.X + (client.Width - width) / 2, client.Y + (client.Height - height) / 2, width, height);
        }  // 縮放後的位置

        public void InvalidateCache()
        {
            // 同一張圖的內容被改過時呼叫（換成新的 Bitmap 物件時會自動更新）
            _cacheSource = null;
            Invalidate();
        }  // 重建快取

        protected override void Dispose(bool disposing)
        {
            if (disposing) _cache?.Dispose();
            base.Dispose(disposing);
        }
    }
}
