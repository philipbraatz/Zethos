using doorfail.zethos.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace doorfail.zethos.UI
{
    public enum SyntaxType
    {
        UNASIGNED,//unknown data to be parsed
        COMMENT,//comment line
        COMMENT_BLOCK,//comment block
        STRING_DOUBLE,//string of doublequote
        STRING_SINGLE,//string of singlequote
        SPACE,//namespace
        CLASS,//class
        TEMPLATE_CLASS,//class template
        VARIBLE,//varible that holds a value
        EQUATION,//a value that can be held
        CHILD_PROPERTY,//method or property of a class
        PARAMETER,//typed and name of required parameters for a function 
        USED_CLASSES,//classes that are inherited from
        USED_SPACES,//spaces that are inherited from, used libraries
        DECLARATION,//Delcaration of a varible with a value
        GETTER,
        SETTER,
        FUNCTION,
    }
    public class detectionString
    {
        public string raw;//preparsed data
        public string entry;//actual string data
        public string errorMSG;//holds error message
        public SyntaxType syntax = 0;
        public List<detectionString> varValue = new List<detectionString>();//value of varible if isVarible
        public Operator operand;
        public int level;//how deep string is

        public string type;//type of varible

        public string ToString(int commentLevel = 0, bool rawRes = false,bool nestedRaw=false)
        {
            string ret = String.Empty;
            for (int i = 0; i < level; i++)
                ret += "\t";
            if (syntax == SyntaxType.COMMENT_BLOCK || syntax == SyntaxType.COMMENT)
            {
                ret += "Comment: ";
            }
            if (syntax == SyntaxType.SPACE)
                ret += "Space: ";
            if (syntax == SyntaxType.CLASS)
                ret += "Class: ";
            if (syntax == SyntaxType.TEMPLATE_CLASS)
                ret += "Template: ";
            if (syntax == SyntaxType.FUNCTION)
                ret += "Function: ";
            if (syntax == SyntaxType.STRING_SINGLE || syntax == SyntaxType.STRING_DOUBLE)
                ret += "String: ";
            if (syntax == SyntaxType.EQUATION)
                ret += "Equation: ";
            if (syntax == SyntaxType.DECLARATION)
                ret += "Declaration: ";

            ret += entry;

            //after entry
            if (syntax == SyntaxType.SPACE ||
                syntax == SyntaxType.CLASS ||
                syntax == SyntaxType.TEMPLATE_CLASS ||
                syntax == SyntaxType.FUNCTION
                )
            {
                ret += " Parameters: ";
                if (varValue.Count > 0)
                    varValue.ForEach(vv => ret += vv.ToString(commentLevel, nestedRaw, nestedRaw) + ", ");
                else
                    ret += "None";
                //ret = ret;
            }
            if(syntax == SyntaxType.EQUATION || syntax == SyntaxType.DECLARATION)
            {
                if(operand != Operator.NON_OPERAND)
                    ret += OperatorConversion.OperatorValues.Where(op => op.Op == operand).FirstOrDefault().Key;
                ret += " = ";
                varValue.ForEach(vv => ret += "("+vv.operand+") "+ vv.entry);
            }

            if (!(syntax == SyntaxType.COMMENT_BLOCK || syntax == SyntaxType.COMMENT))
            {
                if (rawRes)
                    ret += "\nRAW: " + raw.Replace("\n", "\\n");//return raw without endlines
            }
            else if (commentLevel == 0)
                return String.Empty;//return nothing

            //Console.WriteLine("return hit");



            return ret;//return default
        }

        public bool isString() => syntax == SyntaxType.STRING_SINGLE || syntax == SyntaxType.STRING_DOUBLE;
        public bool isComment() => syntax == SyntaxType.COMMENT || syntax == SyntaxType.COMMENT_BLOCK;

    }
}
