﻿using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using SixLabors.ImageSharp;

namespace YnuClassificationPrediction
{
    class PredictionConsole
    {
        // 1번 여기를 먼저 셋업해 주세요
        static readonly string Endpoint = "https://chwscustomvision-prediction.cognitiveservices.azure.com/";
        static readonly string PredictionKey = "873c10b5fe6d410dbfc789f3629c260d";
        static readonly string ProjectId = "abbafae8-c681-4a27-8da0-2eddb2addb2f";
        static readonly string PublishedName = "Iteration3";

        // 2번 ObjectDtection 전 이미지의 폴더 경로 --------------------------- 사용에 따라 수정
        static readonly string TestImageFolder = @"C:\Users\chwonseok\source\repos\YnuObjectDetectionPrediction\Images\";

        // 3번 ObjectDtection 후 결과가 담길 폴더 경로 --------------------------- 사용에 따라 수정
        static readonly string ResultFolder = @"C:\Users\chwonseok\source\repos\YnuObjectDetectionPrediction\Result\result.csv";

        static async Task Main(string[] args)
        {
            // Get each image path
            var imagesPaths = Directory.GetFiles(TestImageFolder);

            var predictionCount = 0;

            //var allCsvResult = new StringBuilder();

            foreach (var imagePath in imagesPaths)
            {
                // var csvResult = new StringBuilder();

                // Get each file name
                var fileName = Path.GetFileName(imagePath);

                // Get each image size

                Image image = Image.Load($"{TestImageFolder}{fileName}");


                // Get predictions of each image
                //var imagePrediction = await GetImagePredictionsAsync(imagePath);

                //foreach (var prediction in imagePrediction.Predictions)
                //{
                //    var tagName = prediction.TagName;
                //    //var aLine = $"{fileName},{prediction.TagName},{prediction.Probability},{prediction.BoundingBox.Top},{prediction.BoundingBox.Height},{prediction.BoundingBox.Left},{prediction.BoundingBox.Width}";
                //    //csvResult.AppendLine(aLine);
                //}

                //allCsvResult.AppendLine(csvResult.ToString());
                //Console.WriteLine($"filename: {fileName}, Prediction Completed");
                predictionCount++;
            }

            //await File.WriteAllTextAsync(ResultFolder, allCsvResult.ToString());
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

                // var predictions = await predictionApi.ClassifyImageWithNoStoreAsync(new Guid(ProjectId), PublishedName, imageStream);
                var predictions = await predictionApi.DetectImageWithNoStoreAsync
                    (new Guid(ProjectId), PublishedName, imageStream);
                return predictions;
            };
        }
    }
}