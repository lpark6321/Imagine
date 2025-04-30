using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thunder
{
    // 資料類別
    public class ImageItem
    {
        public string Type { get; set; } // 圖片檔案名稱
        public string Color { get; set; } // 圖片完整路徑
    }
    public class ImageData
    {
        public List<ImageItem> ImageList { get; set; } // 圖片列表
        public ImageData()
        {
            ImageList = new List<ImageItem>();
        }
        public void AddImage(string type, string color)
        {
            ImageList.Add(new ImageItem { Type = type, Color = color });
        }
    }
}
