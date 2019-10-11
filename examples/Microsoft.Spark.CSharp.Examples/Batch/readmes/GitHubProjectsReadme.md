# GitHub Project Analysis with Batch Data

In this sample, you'll see how to use [.NET for Apache Spark](https://dotnet.microsoft.com/apps/data/spark) 
to analyze a file containing information about a set of GitHub projects. 

## Problem

Our goal here is to analyze information about GitHub projects. We start with some data prep (removing null data and unnecessary columns) 
and then perform further analysis about the number of times different projects have been forked and how recently different projects
have been updated.

This sample is an example of **batch processing** since we're analyzing data that has already been stored and is not actively growing 
or changing.

## Solution

Let's explore how we can use Spark to tackle this problem.

### 1. Create a Spark Session

In any Spark application, we need to establish a new SparkSession, which is the entry point to programming Spark with the Dataset and 
DataFrame API.

```CSharp
SparkSession spark = SparkSession
                .Builder()
                .AppName("GitHub and Spark Batch")
                .GetOrCreate();
```

By calling on the *spark* object created above, we can access Spark and DataFrame functionality throughout our program.

### 2. Read Input File into a DataFrame

Now that we have an entry point to the Spark API, let's read in our GitHub projects file. We'll store it in a DataFrame, while is a distributed collection of data organized into named columns.

```CSharp
DataFrame df = spark.Read().Text("Path to input data set");
```

### 3. Data Prep

We can use `DataFrameNaFunctions` to drop rows with NA (null) values, and the `Drop()` method to remove certain columns from our data. 
This helps prevent errors if we try to analyze null data or columns that do not matter for our final analysis.

### 4. Further Analysis

Spark SQL allows us to make SQL calls on our data. It is common to combine UDFs and Spark SQL so that we can apply a UDF to all 
rows of our DataFrame.

We can specifically call `spark.Sql` to mimic standard SQL calls seen in other types of apps, and we can also call methods like 
`GroupBy` and `Agg` to specifically combine, filter, and perform calculations on our data.

## Next Steps

View the full coding example to see an example of prepping and analyzing GitHub data.
