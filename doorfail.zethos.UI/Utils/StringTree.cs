using System;
using System.Collections.Generic;
using System.Text;

namespace doorfail.zethos.UI
{
    public class StringTree
    {
        public List<StringTree> childTree;
        private String entry;
        //only accessible if childTree is empty
        public string Entry { get => childTree.Count == 0?entry:null; set => entry = childTree.Count == 0 ? value:null; }
    }
}
