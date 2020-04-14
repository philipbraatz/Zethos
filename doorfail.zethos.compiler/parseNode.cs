using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace doorfail.zethos.compiler
{
    public class parseNode
    {
        List<parseNode> childNodes= new List<parseNode>();

        //how to read
        //each entry is followed by the next
        //index1 follows index2 follows index3
        // character '|' means it can match any characters either side if it ONLY
        //"a|b" is a OR b filled " |2" is space filled OR filled 2
        //" |" missing the second option means it can be space filled OR nothing at all
        // ( [ { " ' MUST match bracket layer
        public List<String> matchables = new List<string>();
        
        string value;
        int start;
        int end;
        public parseNode(string matchables)
        {
            this.matchables.Add(matchables);
        }
        public parseNode(List<String> matchables)
        {
            this.matchables =matchables;
        }
        public parseNode(string matchables, int start,int end)
        {
            this.matchables.Add(matchables);
            this.start = start;
            this.end = end;
        }

    }

    public class ParseEntries
    {
        List<parseNode> nodes = new List<parseNode>();
        ParseEntries()
        {
            nodes.Add(new parseNode(new List<String> { "space"," |",":"," |","* |","( * )"," |","{*}"}));
            //nodes.Add(new parseNode("//*\n"));
            //nodes.Add(new parseNode(":"));

            String file = System.IO.File.ReadAllText(@"C:\Users\philo\source\repos\doorfail.zethos\doorfail.zethos.UI\test\example.ze");

            int start = -1;
            int stop = -1;
            int counter = 0;
            foreach(char c in file)
            {
                foreach(var check in nodes[0].matchables)
                {
                    char matching;
                    int m = 0;
                    char nextC = c;

                    if (check[m] == '*')
                    {
                        int wildStopAt = m;
                        while(nextC != check[wildStopAt])
                        {
                            m++;
                            nextC = file[counter+m];
                            
                        }
                    }



                    //while(matching == c)
                    //{
                    //    m++;
                    //    nextC = file[counter + m];
                    //    matching = check[m];
                    //}
                }

                counter++;
            }
        }

    }
}
