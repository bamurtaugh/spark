# Log Processing with Batch Data

In this sample, you'll see how to use [.NET for Apache Spark](https://dotnet.microsoft.com/apps/data/spark) 
and [ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet) to determine if 
statements are positive or negative, a task known as **sentiment analysis**.

## Problem

Our goal here is to determine if online Amazon reviews are positive or negative. We will be using ML.NET to perform
**binary classification** since categorizing reviews involves choosing one of two groups: positive or negative.

## Solution

We will first train our model using ML.NET. We will then create a Spark application and incorporate our ML.NET work into
the .NET for Apache Spark application.

## ML.NET

### Download Dataset

### Download and Use Model Builder

### Generate Code and Zip File

### Add ML.NET to .NET for Apache Spark App

Add libraries/using statements 
Add project reference in Spark app's csproj
Add generated ML code to Spark project
Add classes for Review

## Spark.NET

### Create a Spark Session

In any Spark application, we must establish a new SparkSession, which is the entry point to programming Spark with the Dataset and 
DataFrame API.

```CSharp
SparkSession spark = SparkSession
                .Builder()
                .AppName("Apache User Log Processing")
                .GetOrCreate();
```

### Read Input File into a DataFrame

### Use UDF and Spark SQL

Use UDF and Spark SQL to call method using ML.NET