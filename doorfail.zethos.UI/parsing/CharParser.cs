using System;
using System.Collections.Generic;
using System.Text;

namespace doorfail.zethos.UI
{
    class CharParser
    {
        public List<String> list = new List<string>();
        public string current = String.Empty;
        public char previous = 'a';//set to 'a' have a starting value
        public char previous2 = 'a';

        public bool inComment = false;
        public bool inBlockComment = false;
        public bool indoubleQuote = false;
        public bool insingleQuote = false;

        public string skipNext = string.Empty;
        public bool newSection = false;


        public void Print()
        {
            foreach (String str in list)
                System.Console.WriteLine(str);
        }


        //functions ----------------------------------------
        public void push(char c)
        {
            current+=c;
        }
        public void append()
        {
            if (!string.IsNullOrWhiteSpace(current))
                list.Add(current.Trim(' '));
            current = String.Empty;
        }
        public void append(char c, bool seperate = false)
        {
            if (!seperate)
            {
                if (!string.IsNullOrWhiteSpace(current + c))
                    list.Add(current + c);
                current = String.Empty;
            }
            else
            {
                append();//seperate anything from before
                append(c);
            }
        }

        public void appendNew(char c)
        {
            append();
            current = String.Empty + c;
        }

        private bool isSpecial(char c)
        {
            return  previous >= ' ' && previous < '.' ||
                    previous == '/' ||
                    previous >= ':' && previous < '<' ||
                    previous == '=' || previous == '|' ||
                    previous > '>' && previous <= '@' ||
                    previous >= '{' && previous <= '~' ||
                    current == String.Empty;
        }
        private bool isAlphabetical(char c)
        {
            return previous >= 'A' && previous <= 'z' ||
                    previous >= '0' && previous <= '9' ||
                    previous == '.' || previous == '<' || previous == '>' ||
                    current == String.Empty || previous != '|';
        }
        //returns false if already appended c
        private bool checkForBlockComments(char c)
        {
            if (previous2 == '/' && previous == '/' && c == '*')//open block comment //*
                inBlockComment = true;
            else if (previous2 == '*' && previous == '/' && c == '/')//close block comment *//
                inBlockComment = false;
            else if (!inComment)//
                if (c == '/' && previous == '/')//check for comment //
                    inComment = true;//make comment
                else if (c == '*')
                {
                    append(c, true);//seperate special characters *
                    return false;
                }
            return true;
        }
        private void appendSeperatly(char c, bool isAlphabetCheck)
        {
            if (!inComment)//check for comment
            {
                if (isAlphabetCheck ? isAlphabetical(c) : isSpecial(c) ||
                    insingleQuote || indoubleQuote)//check for "string" 'string'
                    push(c);//add together
                else
                    appendNew(c);//switch from alphabetical to special
            }
            else
                push(c);//add comment
        }

        public List<String> groupComments(string file)
        {

            foreach (char c in file)
            {
                switch (c)
                {
                    case '\n':
                        if (!inBlockComment)//check for block comments before anything
                        {
                            if (insingleQuote || indoubleQuote)//check for comments
                                push(c);
                            else
                            {
                                inComment = false;
                                append(c);//add and print
                            }
                        }
                        else
                            push(c);//add comment in 1 line
                        break;
                    case char ch1 when ch1 == '\'' && previous != '\\':
                        if (!inBlockComment && !inComment)
                            insingleQuote = !insingleQuote;//toggle 'strings'
                        push(c);
                        break;
                    case char ch1 when ch1 == '\"' && previous != '\\':
                        if (!inBlockComment && !inComment)
                            indoubleQuote = !indoubleQuote;//toggle 'strings'
                        push(c);
                        break;
                    case ':':
                    case '|':
                    case '=':
                    case '-':
                    case '+':
                    case ',':
                    case '(':
                    case ')':
                    case '{':
                    case '}':
                    case '?':
                        if (!inBlockComment && !inComment && !indoubleQuote && !insingleQuote)
                            append(c, true);//seperate special characters
                        break;
                    case char ch1 when ch1 >= ' ' && ch1 <= '/':
                        if(checkForBlockComments(c))//only append if this is true
                            push(c);
                        break;
                    case char ch2 when ch2 >= ':' && ch2 <= '@' &&
                                        ch2 != '<' && ch2 != '>':
                    case char ch3 when ch3 >= '{' && ch3 <= '~':
                        appendSeperatly(c, false);
                        break;
                    case char ch1 when ch1 >= '0' && ch1 <= '9':
                    case char ch2 when ch2 >= 'A' && ch2 <= 'z':
                    case char ch3 when ch3 == '<' || ch3 == '>':
                        appendSeperatly(c, true);
                        break;
                }

                previous2 = previous;
                previous = c;
            }

            return fixEndlines(list);
        }

        public static String formatFile(String file)
        {
            return file.Replace("\n",";\n");
        }

        private List<String> fixEndlines(List<String> seperated)
        {
            int i = 0;
            String[] ret = new string[seperated.Count]; 
            seperated.CopyTo(ret);
            foreach (String str in seperated)
            {
                if (i != 0 && str.Length > 2)
                    if (str.Substring(0, 2) == "//" ||
                        str.Length > 3 && str.Substring(0, 3) == "//*")
                    {
                        if (seperated[i - 1].Length < 2)
                            ret[i - 1] += "\n";
                        else if (!(seperated[i - 1].Substring(0, 2) == "//" || 
                            (seperated[i - 1].Length > 3 && seperated[i - 1].Substring(0, 3) == "//*")))
                            ret[i - 1] += "\n";
                    }

                i++;
            }
            return new List<String>(ret);
        }
    }
}