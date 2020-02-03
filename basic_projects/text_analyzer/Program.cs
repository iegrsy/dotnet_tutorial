using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

namespace text_analyzer
{
    class Program
    {
        public class Options
        {
            [Option('f', "file", Required = true, HelpText = "analyze file")]
            public string FilePath { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o => RunWithOpts(o))
            .WithNotParsed<Options>(e => ErrorHandle(e));
        }

        static void ErrorHandle(IEnumerable<Error> e)
        {
            foreach (var item in e)
                Console.WriteLine(item.ToString(), null);
        }

        static void RunWithOpts(Options o)
        {
            try
            {
                var text = System.IO.File.ReadAllText(o.FilePath);

                // Count words in text
                var map = common_utils.Utils.CountWords(text);

                // Print
                System.Console.WriteLine("-----------------Print-----------------");
                foreach (var i in map)
                    System.Console.WriteLine($"count:{i.Value}\tword: {i.Key}");

                // Short for words cout
                var shortMap = common_utils.Utils.ShortDictionary4Value(map);

                // Print
                System.Console.WriteLine("-----------------Shorted for count-----------------");
                foreach (var i in shortMap)
                    System.Console.WriteLine($"count:{i.Value}\tword: {i.Key}");

                // Short for words cout
                var shortMap1 = common_utils.Utils.ShortDictionary4Key(map);

                // Print
                System.Console.WriteLine("-----------------Shorted for Word-----------------");
                foreach (var i in shortMap1)
                    System.Console.WriteLine($"count:{i.Value}\tword: {i.Key}");
            }
            catch (FileNotFoundException)
            {
                System.Console.WriteLine("File not found! Check file path.");
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }
    }
}
