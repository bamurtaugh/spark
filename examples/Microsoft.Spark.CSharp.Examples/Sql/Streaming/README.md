# Log Processing with Structured Streaming 

In this sample, you'll see how to use [.NET for Apache Spark](https://dotnet.microsoft.com/apps/data/spark) 
to analyze incoming log entries. In the world of big data, this is known as **stream processing**.

## Problem

Our goal here is to determine if incoming content is a valid log entry. We will be using the access log format found in the
[Apache Unix Log Samples](http://www.monitorware.com/en/logsamples/apache.php). 

This sample is an example of **stream processing**, as we are processing data in real time, and **log processing** as it
involves gaining insights from a file of log entries.

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

### 2. Establish and Connect to Data Stream

#### Establish Stream: Netcat

netcat (also known as *nc*) allows you to read from and write to network connections. We will be establishing a network
connection with netcat through a terminal window.

[Download netcat](https://sourceforge.net/projects/nc110/files/). Extract the file from the zip download, and append the 
directory you extracted to your "PATH" environment variable.
Certain downloads may ask for a password, which is nc. To make sure your download and path setting worked, open a command prompt and run the command *nc.*

To start a new connection, open a command prompt. For Linux users, run ```nc -lk 9999``` to connect to localhost on port 9999.

Windows users can run ```nc -vvv -l -p 9999``` to connect to localhost port 9999. The result should look something like this:

![NetcatConnect](https://github.com/bamurtaugh/spark/blob/StreamingLog/examples/Microsoft.Spark.CSharp.Examples/Sql/Streaming/netconnect.PNG)

Once the network connection is established, we will be able to connect our Spark program to it. Our program will listen for any input
we type into our command prompt.

#### Connect to Stream: ReadStream()

The ```ReadStream()``` method returns a DataStreamReader that can be used to read streaming data in as a DataFrame. Include the host and port 
information as options so that your Spark application knows where to expect its streaming data.

```CSharp
DataFrame words = spark
                .ReadStream()
                .Format("socket")
                .Option("host", hostname)
                .Option("port", port)
                .Load();
```

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

Choose how to display your stream, whether it's through ```DataFrame.WriteStream()``` or displaying a ```Console.WriteLine()```
within a method called by your UDF. When using ```DataFrame.WriteStream()```, you can establish some other characteristics of your output,
like where it should be printed and if all of the output or just the most recent batch's output should be displayed.

## Running Your Code

Structured streaming in Spark processes data through a series of small **batches**. In our example, when you hit *enter* after
entering data in the command prompt, Spark will consider that a batch and run the UDF.

View the full coding example to see an example of analyzing log data live as it comes in. Try modifying the regular expressions and 
reading in the Apache error log files to gain further practice with log processing in .NET for Apache Spark.
