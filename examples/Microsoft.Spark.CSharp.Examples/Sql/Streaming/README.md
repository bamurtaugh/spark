# Log Processing with Structured Streaming

In this sample, you'll see how to use [.NET for Apache Spark](https://dotnet.microsoft.com/apps/data/spark) 
to analyze incoming log entries. In the world of big data, this is known as **log processing**.

## Problem

Our goal here is to determine if incoming content is a valid log entry. We will be using the access log format found in the
[Apache Unix Log Samples](http://www.monitorware.com/en/logsamples/apache.php). 

This sample is an example of **stream processing**, as we are processing data in real time.

## Solution

### 1. Create a Spark Session

In any Spark application, we must establish a new SparkSession, which is the entry point to programming Spark with the Dataset and 
DataFrame API.

```CSharp
SparkSession spark = SparkSession
                .Builder()
                .AppName("StructuredLogProcessing")
                .GetOrCreate();
```

By calling on the *spark* object created above, we can access Spark and DataFrame functionality throughout our program.

### 2. Connect to Data Stream

#### Netcat

-Explain netcat and how to connect
-Explain command to run (in Windows and Linux) and what each part means
-Include screenshots

### 3. Register a UDF

A UDF is a *user-defined function.* We use UDFs in Spark applications to perform calculations or analysis on our data.

```CSharp
spark.Udf().Register<string, bool>("MyUDF", input => ValidLogTest(input));
```

In the above code snippet, we register a UDF that will pass each log entry it receives to the method *ValidLogTest.*

ValidLogTest compares the log entry to a [regular expression](https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference)
(commonly known as a "regex"), which is a sequence of characters that defines a pattern. We use regular expressions in our UDFs in
log processing to gain meaningful insights and patterns from our log data. 

### 4. Use Spark SQL

Spark SQL allows us to make SQL calls on our data. It is common to combine UDFs and Spark SQL so that we can apply a UDF to all 
rows of our DataFrame.

```CSharp
DataFrame sqlDf = spark.Sql("SELECT WordsEdit.value, MyUDF(WordsEdit.value) FROM WordsEdit"); 
```

### 5. Display Your Stream

Choose how to display your stream, whether it's through ```DataFrame.WriteStream``` or displaying a ```Console.WriteLine()```
within a method called by your UDF.

## Next Steps
View the full coding example to see an example of analyzing log data live as it comes in. Try modifying the regular expressions and 
reading in the Apache error log files to gain further practice with log processing in .NET for Apache Spark.
