namespace YnuObjectDetectionPrediction
{
    public class BoundingBoxInfo
    {
        public string Tag { get; set; } = string.Empty;
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public float Area { get; set; }
        public float Diagonal { get; set; }
    }
}
