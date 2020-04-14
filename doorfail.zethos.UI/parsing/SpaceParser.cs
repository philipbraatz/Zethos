using System;
using System.Collections.Generic;
using System.Text;

namespace doorfail.zethos.UI.parsing
{
    enum ClassSection
    {
        SPACE,
        PROPERTIES,
        METHODS,
        CONSTRUCTOR,
        CUSTOM
    }

    public class SpaceParser
    {
        public List<Space> spaceCollection = new List<Space>();
        private int activeSpace = -1;
        private int activeClass = -1;
        private int activeFunc = -1;
        private int activeSubFunc = -1;


        public SpaceParser(List<detectionString> file)
        {
            for (int i = 0; i < file.Count; i++)
            {

                switch (file[i].syntax)
                {
                    case SyntaxType.UNASIGNED:
                        break;
                    case SyntaxType.COMMENT:
                        break;
                    case SyntaxType.COMMENT_BLOCK:
                        break;
                    case SyntaxType.STRING_DOUBLE:
                        break;
                    case SyntaxType.STRING_SINGLE:
                        break;
                    case SyntaxType.SPACE:
                        spaceCollection.Add(new Space(file[i]));
                        activeSpace = spaceCollection.Count - 1;
                        break;
                    case SyntaxType.CLASS:
                        if (activeSpace != -1)
                        {
                            spaceCollection[activeSpace].classes.Add(new Class(file[i]));
                            activeClass = spaceCollection[activeSpace].classes.Count - 1;
                        }
                        else
                            throw new Exception("All classes have to be declared within a space");
                        activeClass = spaceCollection.Count - 1;
                        break;
                    case SyntaxType.TEMPLATE_CLASS:
                        if (activeSpace != -1)
                        {
                            spaceCollection[activeSpace].classes.Add(new Class(file[i],false));
                            activeClass = spaceCollection[activeSpace].classes.Count - 1;
                        }
                        else
                            throw new Exception("All classes have to be declared within a space");
                        break;
                    case SyntaxType.VARIBLE:
                        break;
                    case SyntaxType.EQUATION:
                        break;
                    case SyntaxType.CHILD_PROPERTY:
                        break;
                    case SyntaxType.PARAMETER:
                        break;
                    case SyntaxType.USED_CLASSES:
                        break;
                    case SyntaxType.USED_SPACES:
                        break;
                    case SyntaxType.DECLARATION:
                        break;
                    case SyntaxType.GETTER:
                        break;
                    case SyntaxType.SETTER:
                        break;
                    case SyntaxType.FUNCTION:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
