using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using SixLabors.ImageSharp;
using YnuObjectDetectionPrediction;

namespace YnuClassificationPrediction
{
    
    class PredictionConsole
    {
        // 1번 여기를 먼저 셋업해 주세요
        static readonly string Endpoint = "https://chwscustomvision-prediction.cognitiveservices.azure.com/";
        static readonly string PredictionKey = "873c10b5fe6d410dbfc789f3629c260d";
        static readonly string ProjectId = "abbafae8-c681-4a27-8da0-2eddb2addb2f";
        static readonly string PublishedName = "Iteration3";

        // 2번 ObjectDetection 전 이미지의 폴더 경로 --------------------------- 사용에 따라 수정
        static readonly string TestImageFolder = @"C:\Users\최원석\Source\Repos\YnuObjectDetectionPrediction\Images\";

        // 3번 ObjectDetection 후 결과가 담길 폴더 경로 --------------------------- 사용에 따라 수정
        static readonly string ResultFolder = @"C:\Users\chwonseok\source\repos\YnuObjectDetectionPrediction\Result\";

        static async Task Main(string[] args)
        {
            // Get each image path
            var imagesPaths = Directory.GetFiles(TestImageFolder);

            var predictionCount = 0;

            var outputPath = @"C:\Users\chwonseok\source\repos\YnuObjectDetectionPrediction\Result\";

            foreach (var imagePath in imagesPaths)
            {
                // Get each file name
                var fileName = Path.GetFileName(imagePath);

                // Get each image size
                Image image = Image.Load($"{imagePath}");
                var imageWidth = image.Width;
                var imageHeight = image.Height;

                // Setup boundingBox dimension
                double x = 0;
                double y = 0;
                double boundingBoxWidth = 0;
                double boundingBoxHeight = 0;

                // Get predictions of each image
                var predictions = await GetImagePredictionsAsync(imagePath);

                // 고도화 필요
                var threshhold = 0.80;

                var highProbability = predictions.Predictions.AsQueryable()
                    .Where(x => x.Probability >= threshhold)
                    .ToList();

                foreach (var item in highProbability)
                {
                    RectangleInfo newBoundingBox = new RectangleInfo()
                    {
                        BoundingBoxTag = item.TagName,
                        BoundingBoxX = item.BoundingBox.Left * imageWidth,
                        BoundingBoxY = item.BoundingBox.Top * imageHeight,
                        BoundingBoxWidth = item.BoundingBox.Width * imageWidth,
                        BoundingBoxHeight= item.BoundingBox.Height * imageHeight
                    };

                    Console.WriteLine($"rectangle starting position is ({x},{y})\n{boundingBoxWidth}, {boundingBoxHeight}");
                    Console.WriteLine($"tagname:{item.TagName}, probability:{item.Probability}");

                    // Draw boundingBox on the image
                    Drawing.DrawBoundingBox(newBoundingBox, imagePath, outputPath);
                    
                }


                Console.WriteLine($"filename: {fileName}, Prediction Completed");
                predictionCount++;
            }
            Console.WriteLine($"{predictionCount}개 이미지 대상 Prediction 완료");
        }

        // Authenticate the prediction
        private static CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
        {
            CustomVisionPredictionClient predictionApi = new(new ApiKeyServiceClientCredentials(predictionKey))
            {
                Endpoint = endpoint
            };
            return predictionApi;
        }

        static async Task<ImagePrediction> GetImagePredictionsAsync(string imagePath)
        {
            // Get predictions from the image
            using (var imageStream = new FileStream(imagePath, FileMode.Open))
            {
                var predictionApi = AuthenticatePrediction(Endpoint, PredictionKey);

                var predictions = await predictionApi.DetectImageWithNoStoreAsync
                    (new Guid(ProjectId), PublishedName, imageStream);
                return predictions;
            };
        }
    }
}