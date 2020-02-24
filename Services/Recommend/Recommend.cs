using Entities.User;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Trainers.Recommender;
using System;
using System.IO;
using System.Linq;

namespace Services.Recommend
{
    public class Recommend : IRecommend
    {
        private readonly MLContext _mlContext;

        private IDataView TestData { get; set; }
        private IDataView TrainingData { get; set; }
        private PredictionEngine<Like, PostRatingPrediction> PredictionEngine { get; set; }
        private TransformerChain<TransformerChain<MatrixFactorizationPredictionTransformer>> Model { get; set; }

        private static readonly string TrainingDataPath = Path.Combine(Environment.CurrentDirectory, "recommendation-ratings-train.csv");
        private static readonly string TestDataPath = Path.Combine(Environment.CurrentDirectory, "recommendation-ratings-test.csv");

        public Recommend(MLContext mlContext)
        {
            _mlContext = mlContext;
        }

        public void LoadData()
        {
            TrainingData = _mlContext.Data
                .LoadFromTextFile<Like>(TrainingDataPath, hasHeader: true, separatorChar: ',');

            TestData = _mlContext.Data
                .LoadFromTextFile<Like>(TestDataPath, hasHeader: true, separatorChar: ',');
        }

        public void Train()
        {
            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "UserIdEncoded",
                MatrixRowIndexColumnName = "PostIdEncoded",
                LabelColumnName = "Label",
                NumberOfIterations = 20,
                ApproximationRank = 100
            };

            // set up a training pipeline
            // step 1: map userId and movieId to keys
            var pipeline = _mlContext
                .Transforms
                .Conversion
                .MapValueToKey(
                    inputColumnName: "UserId",
                    outputColumnName: "UserIdEncoded")
                .Append(_mlContext.Transforms.Conversion
                    .MapValueToKey(
                        inputColumnName: "PostId",
                        outputColumnName: "PostIdEncoded")

                    // step 2: find recommendations using matrix factorization
                    .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(options)));

            // train the model
            Console.WriteLine("Training the model...");

            Model = pipeline.Fit(TrainingData);
        }

        public void Evaluate()
        {
            // evaluate the model performance 
            Console.WriteLine("Evaluating the model...");

            var predictions = Model.Transform(TestData);
            var metrics = _mlContext.Regression.Evaluate(predictions);

            Console.WriteLine($"  RMSE: {metrics.RootMeanSquaredError:#.##}");
            Console.WriteLine($"  LossFunction:   {metrics.LossFunction:#.##}");
            Console.WriteLine($"  MeanAbsoluteError:   {metrics.MeanAbsoluteError:#.##}");
            Console.WriteLine($"  MeanSquaredError:   {metrics.MeanSquaredError:#.##}");
            Console.WriteLine($"  RSquared:   {metrics.RSquared:#.##}");
        }

        public void Prediction()
        {
            // check if a given user likes 'GoldenEye'
            Console.WriteLine("Calculating the score for user 6 liking the movie 'GoldenEye'...");

            PredictionEngine = _mlContext.Model
                .CreatePredictionEngine<Like, PostRatingPrediction>(Model);

            var prediction = PredictionEngine.Predict(
                new Like
                {
                    UserId = 1,
                    PostId = 1 // GoldenEye
                }
            );

            Console.WriteLine($"  Score: {prediction.Score}");
        }

        public void Result()
        {
            // find the top 5 movies for a given user
            Console.WriteLine("Calculating the top 5 posts for user 1...");

            var top5 = (from m in Posts.All
                        let p = PredictionEngine.Predict(
                            new Like()
                            {
                                UserId = 1,
                                PostId = m.Id // GoldenEye
                            })
                        orderby p.Score descending
                        select (postId: m.Id, p.Score)).Take(5);

            foreach (var (postId, score) in top5)
                Console.WriteLine($"  Score:{score}\tMovie: {Posts.Get(postId)?.Title}");
        }
    }
}