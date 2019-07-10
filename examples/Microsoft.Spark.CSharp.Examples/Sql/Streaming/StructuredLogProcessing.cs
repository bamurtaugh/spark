// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.Spark.Sql;
using static Microsoft.Spark.Sql.Functions;
using System.Text.RegularExpressions;

namespace Microsoft.Spark.Examples.Sql.Streaming
{
    /// <summary>
    /// Example of analyzing log entries as they are entered live.
    /// Basis for streaming taken/modified from
    /// spark/examples/src/main/python/sql/streaming/structured_kafka_wordcount.py
    /// </summary>
    internal sealed class StructuredLogProcessing : IExample 
    {
        public void Run(string[] args)
        {
            string hostname = "localhost";
            var port = 9999;

            // If user entered host and port info of their updating logs in the command line, update the variables
            if(args.Length == 2)
            {
                hostname = args[0];
                port = int.Parse(args[1]);
            }
            else 
            {
                Console.WriteLine("Usage: StructuredLogProcessing <hostname> <port>. Will use hostname = localhost, port = 9999.");
            }

            SparkSession spark = SparkSession
                .Builder()
                .AppName("StructuredLogProcessing")
                .GetOrCreate();

            DataFrame words = spark
                .ReadStream()
                .Format("socket")
                .Option("host", hostname)
                .Option("port", port)
                .Load();
            
            // Register a UDF to be run on each incoming entry from the stream 
            spark.Udf().Register<string, bool>("MyUDF", input => ValidLogTest(input));
            words.CreateOrReplaceTempView("WordsEdit");
            DataFrame sqlDf = spark.Sql("SELECT WordsEdit.value, MyUDF(WordsEdit.value) FROM WordsEdit"); 

            // With each incoming line, test if it's a valid log entry and output result
            Microsoft.Spark.Sql.Streaming.StreamingQuery query = sqlDf
                                                                .WriteStream()
                                                                .Format("console")
                                                                .Start();
            query.AwaitTermination();

        } 

        public static bool ValidLogTest(string logLine)
        {
            // Regex for user logs taken from Databricks Spark Reference Applications: https://databricks.gitbooks.io 
            Regex rx = new Regex("^(\\S+) (\\S+) (\\S+) \\[([\\w:/]+\\s[+\\-]\\d{4})\\] \"(\\S+) (\\S+) (\\S+)\" (\\d{3}) (\\d+)");

            if(logLine != null && rx.IsMatch(logLine))
            {
                // Valid entry example: 64.242.88.10 - - [07/Mar/2004:16:47:12 -0800] "GET /robots.txt HTTP/1.1" 200 68
                Console.WriteLine("Congrats, \"" + logLine + "\" is a valid log entry!");
                return true;
            }
            else
            {
                // Invalid entry example: 64.242.88.10 - - [07/Mar/2004:16:47:12 -0800] "GET /robots.txt HTTP/1.1" 200 aa68
                Console.WriteLine("Invalid Log Entry");
                return false;
            }
        }

    }
}
                