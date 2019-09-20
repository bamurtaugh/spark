using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Spark.Sql;

namespace modelbuilderstream
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create Spark Session
            SparkSession spark = SparkSession
                .Builder()
                .AppName("Streaming Sentiment Analysis")
                .GetOrCreate();

            // Setup stream connection info
            string hostname = "localhost";
            var port = 9999;

            // Read streaming data into DataFrame
            DataFrame words = spark
               .ReadStream()
               .Format("socket")
               .Option("host", hostname)
               .Option("port", port)
               .Load();

            // Use ML.NET in a UDF to evaluate each incoming entry
            spark.Udf().Register<string, bool>("MyUDF", input => Sentiment(input));

            // Call ML.NET code and display sentiment analysis results
            words.CreateOrReplaceTempView("WordsEdit");
            DataFrame sqlDf = spark.Sql("SELECT WordsEdit.value, MyUDF(WordsEdit.value) FROM WordsEdit");
            // 

            // Handle data continuously as it arrives
            Microsoft.Spark.Sql.Streaming.StreamingQuery query = sqlDf
                                                                .WriteStream()
                                                                .Format("console")
                                                                .Start();
            query.AwaitTermination();
        }

        // Method to call ML.NET sentiment analysis model
        public static bool Sentiment(string text)
        {
            MLContext mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load("MLModel.zip", out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<Review, ReviewPrediction>(mlModel);

            var result = predEngine.Predict(new Review { Column1 = text });
            return result.Prediction;
        }

        // Class to represent each review
        public class Review
        {
            // Column names must match input file
            // Column1 is review
            [LoadColumn(0)]
            public string Column1;
        }

        // Class resulting including predictions about review
        public class ReviewPrediction : Review
        {

            [ColumnName("PredictedLabel")]
            public bool Prediction { get; set; }

            public float Probability { get; set; }

            public float Score { get; set; }
        }
    }
}
