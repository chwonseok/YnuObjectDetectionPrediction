using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YnuObjectDetectionPrediction
{
    public class Drawing
    {
        public static void DrawBoundingBox(RectangleInfo rectangleInfo, string filePath, string outputPath)
        {
            var image = Image.FromFile(filePath);
            var imageGraphics = Graphics.FromImage(image);

            Pen pen = new Pen(Color.Black, 2);

            int x = Convert.ToInt32(rectangleInfo.BoundingBoxX);
            int y = Convert.ToInt32(rectangleInfo.BoundingBoxY);
            int width = Convert.ToInt32(rectangleInfo.BoundingBoxWidth);
            int height = Convert.ToInt32(rectangleInfo.BoundingBoxHeight);

            var rect = new Rectangle(x, y, width, height);
            imageGraphics.DrawRectangle(pen, rect);

            imageGraphics.Save();
            imageGraphics.

            image.Save(outputPath);

            //using (Graphics g = Graphics.FromImage(image))
            //{
            //    g.FillRectangle(brush, x, y, width, height);
            //}



            //int thumbnailSize = imageSize;
            //int newWidth, newHeight;

            //if (image.Width > image.Height)
            //{
            //    newWidth = thumbnailSize;
            //    newHeight = image.Height * thumbnailSize / image.Width;
            //}
            //else
            //{
            //    newWidth = image.Width * thumbnailSize / image.Height;
            //    newHeight = thumbnailSize;
            //}

            //var thumbnailBitmap = new Bitmap(newWidth, newHeight);

            //var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
            //thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
            //thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
            //thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
            //thumbnailGraph.DrawImage(image, imageRectangle);

            //Color color = Color.FromArgb(50, 255, 255, 255);
            //SolidBrush brush = new SolidBrush(color);
            //Point atPoint = new Point(10, 10);
            //Pen pen = new Pen(brush);
            //thumbnailGraph.FillRectangle(brush, imageRectangle);

            //thumbnailBitmap.Save(outputPath);
            //thumbnailGraph.Dispose();
            //thumbnailBitmap.Dispose();
            //image.Dispose();
        }
    }
}
