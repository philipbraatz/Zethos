using System;
using System.Collections.Generic;
using doorfail.zethos.compiler;

namespace doorfail.zethos.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Start hit");
            CharParser Charparser = new CharParser();
            List<String> seperated = Charparser.groupComments(CharParser.formatFile(
                System.IO.File.ReadAllText(@"C:\Users\philo\source\repos\Zethos\doorfail.zethos.UI\test\example.ze")
                ));
            List<detectionString> sortLevels = StringParser.removeBrackets(seperated);
            List<detectionString> declarationLevels = StringParser.analyzeDeclarations(sortLevels);


            foreach(String lstr in seperated)
            {
                Console.WriteLine("SortLevels: " + lstr.Replace("\n","\\n"));
            }
            Console.WriteLine("-------------------------------------------------------------");
            foreach (detectionString lstr in declarationLevels)
            {
                //Console.WriteLine("Main hit");
                var temp = lstr.ToString(0, false);
                if(temp != String.Empty)
                    Console.WriteLine("SortLevels: " + temp);
            }

        }

        

    }
}
