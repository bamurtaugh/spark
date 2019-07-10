
using System;
using System.Collections.Generic;
using Microsoft.Spark.Sql;
using static Microsoft.Spark.Sql.Functions;
using System.Text.RegularExpressions;

namespace SampleSparkProject
{
    class Program
    {
        static void Main(string[] args)
        {
            SparkSession spark = SparkSession
                .Builder()
                .AppName("StreamingTest")
                .GetOrCreate();
            
            string hostname = "localhost";
            var port = 9999;

            DataFrame words = spark.ReadStream().Format("socket").Option("host", hostname).Option("port", port).Load();
            // Split the "value" column on space and explode to get one word per row.
            //DataFrame words = lines.Select(Explode(Split(lines["value"], " "))
                                    //.Alias("logline"));
            
            //DataFrame wordCounts = words.GroupBy("word").Count();
            // Log streaming!!!
            // If get a string (log?????), analyze with regex
            //spark.Udf().Register<string?, string?>("MyUDF", (input) => (input is string) ? StringRegTest(input) : input*2);
            spark.Udf().Register<string, bool>("MyUDF", input => StringRegTest(input));
            words.CreateOrReplaceTempView("WordsEdit");
            DataFrame sqlDf = spark.Sql("SELECT WordsEdit.value, MyUDF(WordsEdit.value) FROM WordsEdit"); // print original and edited

            // nc -vvv -l -p 9999	
            //Microsoft.Spark.Sql.Streaming.StreamingQuery query = wordCounts
            Microsoft.Spark.Sql.Streaming.StreamingQuery query = sqlDf
                                                                .WriteStream()
                                                                //.OutputMode("Append")
                                                                //.OutputMode("complete") // Complete only viable if using aggregation ***MAKES A LOT FASTER/ENTER TRIGGERS NEW BATCH
                                                                .Format("console")
                                                                .Start();
            query.AwaitTermination();

        } // main

        public static bool StringRegTest(string logLine)
        {
            //Regex rx = new Regex("\\b(?=67)\\b"); // Any BSSID that contains 67
            //Regex rx = new Regex("^(?=1c)"); // BSSID that starts with 1c (not just contains 1c)

            Regex rx = new Regex("^(\\S+) (\\S+) (\\S+) \\[([\\w:/]+\\s[+\\-]\\d{4})\\] \"(\\S+) (\\S+) (\\S+)\" (\\d{3}) (\\d+)");

            if(logLine != null && rx.IsMatch(logLine))
            {
                //Console.WriteLine(logLine);
                // Valid example: 64.242.88.10 - - [07/Mar/2004:16:47:12 -0800] "GET /robots.txt HTTP/1.1" 200 68
                Console.WriteLine("Congrats, \"" + logLine + "\" is a valid log line!");
                return true;
            }
            else
            {
                // Invalid example: 64.242.88.10 - - [07/Mar/2004:16:47:12 -0800] "GET /robots.txt HTTP/1.1" 200 aa68
                Console.WriteLine("Invalid Log Line");
                return false;
            }
        }

    }
}
                