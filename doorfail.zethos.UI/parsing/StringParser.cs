using doorfail.zethos.UI.parsing.scanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace doorfail.zethos.UI
{

    static class StringParser
    {
        public static List<detectionString> removeBrackets(List<String> sepFile)
        {
            List<detectionString> ret = new List<detectionString>();
            detectionString current;
            detectionString prev;

            int depth = 0;

            current = new detectionString();

            foreach (String str in sepFile)
            {
                current.raw += str;//RAW: original value \n

                if (str.Trim() == "{")//hide depth and track
                {
                    depth++;
                    continue;
                }
                else if (str.Trim() == "}")//hide depth and track
                {
                    depth--;
                    continue;
                }
                else if (str.Length >= 1 && str.Substring(0, 1) == "\"")//toggle double string
                {
                    if (current.syntax != SyntaxType.STRING_SINGLE)//dont switch if inside a alternate string
                        current.syntax = current.syntax == SyntaxType.STRING_DOUBLE ? SyntaxType.UNASIGNED : SyntaxType.STRING_DOUBLE;
                }
                else if (str.Length >= 1 && str.Substring(0, 1) == "\'")
                {
                    if (current.syntax != SyntaxType.STRING_DOUBLE)//dont switch if inside a alternate string
                        current.syntax = current.syntax == SyntaxType.STRING_SINGLE ? SyntaxType.UNASIGNED : SyntaxType.STRING_SINGLE;
                }
                else if (str.Trim().Length >= 2 && str.Trim().Substring(0, 2) == "//")//toggle single string
                {
                    if (str.Length >= 3 && str.Substring(2, 1) == "*")
                    {
                        current.syntax = SyntaxType.COMMENT_BLOCK;
                        if (str.EndsWith("\n"))
                            current.entry = str.Remove(str.Length-5, 4).Remove(0,3);
                        else
                            current.entry = str.Remove(str.Length - 4, 3).Remove(0, 3);
                    }
                    else
                    {
                        current.syntax = SyntaxType.COMMENT;
                        current.entry = str.Trim('/');
                    }
                }

                current.level = depth;
                if (!(current.isComment()))//text already in entry or text to be skipped
                    current.entry += str;
                else if(ret.Count > 0 && !ret.Last().isComment())//if last DetectionString was not a comment
                {
                    ret.Last().entry += "\n";//recover endline stolen from comments
                    ret.Last().raw += "\n";
                }
                ret.Add(current);
                prev = current;
                current = new detectionString();

            }


            return formatFile( ret);
        }

        private static List<detectionString> formatFile(List<detectionString> file)
        {
            List<detectionString> ret = new List<detectionString>();
            foreach (detectionString item in file)
            {
                detectionString temp = item;
                temp.entry = temp.entry.Replace(";\n", "\n");
                ret.Add(temp);
            }
            return ret;
        }

        public static List<detectionString> analyzeDeclarations(List<detectionString> lineChunks)
        {
            List<detectionString> ret = new List<detectionString>();
            detectionString current;

            List<DeclarationScanner> declarationScannerCollection = new List<DeclarationScanner> { 
                new DeclarationScanner("space", SyntaxType.SPACE, false),
                new DeclarationScanner("class", SyntaxType.CLASS, false),
                new DeclarationScanner("template", SyntaxType.TEMPLATE_CLASS, false),
                new DeclarationScanner("func", SyntaxType.FUNCTION, true)
            };
            EquationScanner equationScanner = new EquationScanner();

            int lastlvl=0;
            foreach ( detectionString dtstr in lineChunks)
            {
                if (!dtstr.isComment())//ignore comments
                {
                    bool externalContinue = false;
                    bool isFound = false;
                    //FINISHED, add when finished from previous
                    declarationScannerCollection.ForEach(ds =>
                    {
                        if (!externalContinue)//continue; //inside of this function
                        {
                            if (ds.isFinished())
                                ret.Add(ds.GetDetection(dtstr.level, true));

                            //ACTIVE, only run through active scanners
                            if (ds.isActive())
                            {
                                ds.Next(dtstr.entry);
                                externalContinue = true;
                                isFound = true;
                            }
                        }
                    });

                    if (equationScanner.isFinished())
                    {
                        ret.Add(equationScanner.GetDetection(dtstr.level));
                    }
                    if(equationScanner.isActive())
                    {
                        equationScanner.Next(dtstr.entry,dtstr.level);
                        isFound = true;
                        continue;
                    }

                    declarationScannerCollection.ForEach(ds => {
                        if (ds.Status == StatusDec.ERROR)
                            throw new Exception(ds.GetDetection(-1, false).raw+ "\n"+ ds.GetError());
                    });
                    if (equationScanner.Status == StatusEq.ERROR)
                        throw new Exception(equationScanner.GetDetection(-1,false).raw+"\n"+ equationScanner.GetError());

                    //nothing active currently
                    //check all scanners
                    bool addMissing = true;
                    if(!isFound)
                    {
                        declarationScannerCollection.ForEach(ds => ds.Next(dtstr.entry.Trim()));
                        if (declarationScannerCollection.Where(ds => ds.isActive()).Any())//if nothing was found add untouched
                            addMissing = false; 

                        equationScanner.Next(dtstr.entry, dtstr.level);
                        if (equationScanner.isActive())
                        {
                            addMissing = false;
                        }

                        if(addMissing)
                            ret.Add(dtstr);
                    }
                }
                else
                    ret.Add(dtstr);//add comments back in
                lastlvl = dtstr.level;
            }

            //add if finished at end of loop VERY UNLIKELY
            declarationScannerCollection.ForEach(ds=> {
                if (ds.isFinished())
                    ret.Add(ds.GetDetection(lastlvl));
            });
            if (equationScanner.isFinished())
                ret.Add(equationScanner.GetDetection(lastlvl));
            return ret;
        }
    }
}
