// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.Spark.Sql;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Microsoft.Spark.Examples.MachineLearning
{
    /// <summary>
    /// Example of using ML.NET for sentiment analysis in a .NET for Apache Spark program.
    /// </summary>
    internal sealed class SentimentAnalysis : IExample 
    {
        public void Run(string[] args)
        {
            if(args.Length != 1)
            {
                Console.Error.WriteLine(
                    "Usage: Datasource <path to review dataset>");
                
                Environment.Exit(1);
            }

            SparkSession spark = SparkSession
                .Builder()
                .AppName(".NET Spark Sentiment Analysis")
                .GetOrCreate();

            DataFrame df = spark
                .Read()
                .Option("header", true)
                .Option("inferSchema", true)
                .Csv(args[0]);
            df.Show();

            // Use ML.NET to evaluate each review 
            spark.Udf().Register<string, bool>("MLudf", (text) => Sentiment(text));
            df.CreateOrReplaceTempView("Reviews");
            DataFrame sqlDf = spark.Sql("SELECT Column1, MLudf(Column1) FROM Reviews");
            sqlDf.Show();

            // Print out first 20 rows of data
            // Prevents data getting cut off (as it is when we print a DF)
            IEnumerable<Row> rows = sqlDf.Collect();
            int counter = 0;
            foreach (Row row in rows)
            {
                counter++;
                if (counter < 20)
                    Console.WriteLine(row);
                else
                    break;
            }

            spark.Stop();
        }

        // To use ML.NET sure have ProjectReference in .csproj file:
        // Include="<path to sentimentML.Model.csproj>"
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

            // Column2 is sentiment (1 = positive, 0 = negative)
            // [LoadColumn(1), ColumnName("Column2")]
            // public bool Column2;
        }

        // Class resulting from ML.NET code including predictions about review
        public class ReviewPrediction : Review
        {

            [ColumnName("PredictedLabel")]
            public bool Prediction { get; set; }

            public float Probability { get; set; }

            public float Score { get; set; }
        } 
    }
}
                
