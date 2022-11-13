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
                    .OrderByDescending(x => x.Probability)
                    .ToList();
                
                var img = Image.FromFile(imagePath);

                var predictionResult = new ImagePredictionModel()
                {
                    ImageName = fileName
                };

                foreach (var item in highProbability)
                {
                    // item이 marker가 아닐 때 어떻게 처리하는지 확인 필요
                    if (item.TagName.Substring(item.TagName.Length - 6, 6) == "marker")
                    {
                        predictionResult.CmPerPixel = GetCmPerPixel(item, item.BoundingBox.Width);
                    }

                    var singleBb = new BoundingBoxInfo()
                    {
                        Tag = item.TagName,
                        Probability = item.Probability,
                        X = (float)(item.BoundingBox.Left * imageWidth),
                        Y = (float)(item.BoundingBox.Top * imageHeight),
                        Width = (float)(item.BoundingBox.Width * imageWidth),
                        Height = (float)(item.BoundingBox.Height * imageHeight),
                        Area = (float)(item.BoundingBox.Width * item.BoundingBox.Height),
                        Diagonal = Math.Sqrt(Math.Pow(item.BoundingBox.Width, 2) + Math.Pow(item.BoundingBox.Height, 2)),
                        ActualWidth = (float)(predictionResult.CmPerPixel * item.BoundingBox.Width),
                        ActualHeight = (float)(predictionResult.CmPerPixel * item.BoundingBox.Height)
                    };

                    predictionResult.BoundingBoxes.Add(singleBb);


                    Graphics g = Graphics.FromImage(img);
                    var marker = GetMarker(singleBb.Tag);
                    g.DrawRectangle(marker, singleBb.X, singleBb.Y, singleBb.Width, singleBb.Height);

                    Console.WriteLine($"종류:{singleBb.Tag}\n확률:{singleBb.Probability}\n가로:{singleBb.ActualWidth}cm\n세로:{singleBb.ActualHeight}");
                }

                img.Save(outputPath);
                
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

        static double GetCmPerPixel(PredictionModel item, double width)
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