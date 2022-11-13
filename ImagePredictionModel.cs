namespace YnuObjectDetectionPrediction
{
    public class ImagePredictionModel
    {
        public string ImageName { get; set; } = string.Empty;
        public string Marker { get; set; } = string.Empty;
        public double CmPerPixel { get; set; }
        public List<BoundingBoxInfo> BoundingBoxes { get; set; } = new();
    }
}
