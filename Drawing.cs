using System.Drawing;

namespace YnuObjectDetectionPrediction
{
    public class Drawing
    {
        public static void DrawBoundingBox(BoundingBoxInfo rectangleInfo, string filePath, string outputPath)
        {
            var x = rectangleInfo.X;
            var y = rectangleInfo.Y;
            var width = rectangleInfo.Width;
            var height = rectangleInfo.Height;

            var img = Image.FromFile(filePath);

            // test
            Graphics g = Graphics.FromImage(img);
            Pen black = new(Color.Black, 3);

            g.DrawRectangle(black, x, y, width, height);
            img.Save(outputPath);
        }
    }
}
