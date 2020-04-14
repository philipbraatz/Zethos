using System;
using System.Collections.Generic;
using System.Text;

namespace doorfail.zethos.compiler
{
    public class NodeTree
    {
        string file;
        public List<NodeTree> nodes;
        string section;

        public NodeTree(String file)
        {
            this.file = file;

            foreach(char c in file)
            {

            }
        }

    }
}
