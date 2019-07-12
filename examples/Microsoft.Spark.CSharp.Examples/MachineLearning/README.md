# Sentiment Analysis with Big Data

In this sample, you'll see how to use [.NET for Apache Spark](https://dotnet.microsoft.com/apps/data/spark) 
and [ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet) to determine if 
statements are positive or negative, a task known as **sentiment analysis**.

## Problem

Our goal here is to determine if online reviews are positive or negative. We'll be using ML.NET to perform
**binary classification** since categorizing reviews involves choosing one of two groups: positive or negative. You can read more about the problem through the [ML.NET documentation](https://docs.microsoft.com/en-us/dotnet/machine-learning/tutorials/sentiment-analysis).

## Solution

We will first train an ML model using ML.NET. We'll then create a Spark application and incorporate our ML.NET work into
the .NET for Apache Spark application.

## ML.NET

### 1. Download Datasets

Download the [UCI Sentiment Labeled Sentences dataset ZIP file](https://archive.ics.uci.edu/ml/machine-learning-databases/00331/sentiment%20labelled%20sentences.zip). You can access the specific files we'll be using in the /Datasets folder.

### 2. Download and Use Model Builder

Model Builder helps you easily train and use ML models in Visual Studio. Follow the [Model Builder Getting Started Guide](https://dotnet.microsoft.com/learn/machinelearning-ai/ml-dotnet-get-started-tutorial/intro).

Use the amazon reviews file to train your model using the sentiment analysis scenario. To easily work with the same data files in both Spark.NET and ML.NET, you may find it easier to start off using .csv rather than .txt files
since .csv files have already-defined columns.

In the last step after training your model with model builder, you'll generate a zip file containing the ML.NET code you need to use in your Spark app.

### 3. Add ML.NET to .NET for Apache Spark App

We need to make sure our Spark app has the necessary ML.NET references. 

Download the [Microsoft.ML NuGet Package](https://www.nuget.org/packages/Microsoft.ML). Make sure your Spark app has a reference to the .csproj file of your trained ML model from model builder and the Microsoft.ML API. 

![CSProject](https://github.com/bamurtaugh/spark/blob/SparkMLNet/examples/Microsoft.Spark.CSharp.Examples/MachineLearning/SparkMLPic.PNG)

As we create the logic for our Spark app, we'll paste in the code generated from model builder and include some other class definitions.

## Spark.NET

### 1. Create a Spark Session

In any Spark application, we need to establish a new SparkSession, which is the entry point to programming Spark with the Dataset and 
DataFrame API.

```CSharp
SparkSession spark = SparkSession
       .Builder()
       .AppName("Apache User Log Processing")
       .GetOrCreate();
```

### 2. Read Input File into a DataFrame

We trained our model with the amazon data, so let's test how well the model performs by testing it with the yelp dataset. 

```CSharp
DataFrame df = spark.Read().Csv(<Path to yelp data set>);
```

### 3. Use UDF to Access ML.NET

We create a User Defined Function (UDF) that calls the *Sentiment* method. 

```CSharp
spark.Udf().Register<string, bool>("MLudf", (text) => Sentiment(text));
```

The Sentiment method is where we'll call our ML.NET code. The code we're using in this method was generated from the final step of using Model Builder.

```CSharp
MLContext mlContext = new MLContext();
ITransformer mlModel = mlContext.Model.Load("MLModel.zip", out var modelInputSchema);
var predEngine = mlContext.Model.CreatePredictionEngine<Review, ReviewPrediction>(mlModel);
```
You may notice the use of *Review* and *ReviewPrediction.* These are classes we have defined to represent the data we are using for training and testing. 

```CSharp
public class Review
{
      [LoadColumn(0)]
      public string Column1;

      [LoadColumn(1), ColumnName("Column2")]
      public bool Column2;
}
```

```CSharp
public class ReviewPrediction : Review
{

       [ColumnName("PredictedLabel")]
       public bool Prediction { get; set; }

       public float Probability { get; set; }

       public float Score { get; set; }
} 
```

### 4. Spark SQL and Running Your Code

Now that you've read in your data and incorporated your ML.NET code, use Spark SQL to call the UDF that will run sentiment analysis on each row of your DataFrame:

```CSharp
DataFrame sqlDf = spark.Sql("SELECT _c0, MLudf(_c0) FROM Reviews");
```

Run your code, and you'll be performing sentiment analysis with ML.NET and Spark.NET!
