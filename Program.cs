using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System.Drawing;
using Image = System.Drawing.Image;
using SixImage = SixLabors.ImageSharp.Image;
using YnuObjectDetectionPrediction;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Razor.Hosting;

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
        static readonly string TestImageFolder = @"C:\Users\chwonseok\source\repos\YnuObjectDetectionPrediction\Images\";

        // 3번 ObjectDetection 후 결과가 담길 폴더 경로 --------------------------- 사용에 따라 수정
        static readonly string OutputFolderPath = @"C:\Users\chwonseok\source\repos\YnuObjectDetectionPrediction\Result";

        static async Task Main(string[] args)
        {
            // Get each image path
            var imagesPaths = Directory.GetFiles(TestImageFolder);

            var predictionCount = 0;

            foreach (var imagePath in imagesPaths)
            {
                var threshhold = 0.85;

                // Set file path and name
                var fileName = Path.GetFileName(imagePath);
                var outputPath = $"{OutputFolderPath}\\{fileName}";

                // Get each image size
                SixImage image = SixImage.Load($"{imagePath}");
                var imageWidth = image.Width;
                var imageHeight = image.Height;

                // Get predictions of each image
                var predictions = await GetImagePredictionsAsync(imagePath);

                var highProbability = predictions.Predictions.AsQueryable()
                    .Where(x => x.Probability >= threshhold)
                    .ToList();
                
                var img = Image.FromFile(imagePath);

                foreach (var item in highProbability)
                {   
                    var x = (float)(item.BoundingBox.Left * imageWidth);
                    var y = (float)(item.BoundingBox.Top * imageHeight);
                    var width = (float)(item.BoundingBox.Width * imageWidth);
                    var height = (float)(item.BoundingBox.Height * imageHeight);
                    var area = width * height;
                    var diagonal = Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));

                    var marker = GetMarker(item.TagName);

                    Graphics g = Graphics.FromImage(img);

                    g.DrawRectangle(marker, x, y, width, height);

                    Console.WriteLine($"종류:{item.TagName}\n확률:{item.Probability}\n");
                }

                img.Save(outputPath);

                Console.WriteLine($"filename: {fileName}, Prediction Completed");
                predictionCount++;

                // TODO: 픽셀당길이 구하기
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

        static double GetCmPerPixel(PredictionModel item, float width)
        {
            //노란색 3 * 6 cm
            //빨간색 5 * 10 cm
            //파란색 8 * 16 cm
            double cmPerPixel = 0;

            if (item.TagName == "bluemarker")
            {
                cmPerPixel = 16 / width;
            }
            if (item.TagName == "redmarker")
            {
                cmPerPixel = 10 / width;
            }
            if (item.TagName == "yellowmarker")
            {
                cmPerPixel = 6 / width;
            }
            return cmPerPixel;
        }

        static Pen GetMarker(string marker)
        {
            var color = Color.Empty;

            if (marker == "bluemarker")
            {
                color = Color.Blue;
            }
            if (marker == "redmarker")
            {
                color = Color.Red;
            }
            if (marker == "yellowmarker")
            {
                color = Color.Yellow;
            }
            if (marker == "crack")
            {
                color = Color.Gold;
            }
            if (marker == "efflorescence")
            {
                color = Color.Green;
            }
            if (marker == "exposure")
            {
                color = Color.DarkGray;
            }

            Pen result = new(color, 3);

            return result;
        }
    }
}