using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YnuObjectDetectionPrediction
{
    public class RectangleInfo
    {
        public string BoundingBoxTag { get; set; } = string.Empty;
        public double BoundingBoxX { get; set; }
        public double BoundingBoxY { get; set; }
        public double BoundingBoxWidth { get; set; }
        public double BoundingBoxHeight { get; set; }
    }
}
