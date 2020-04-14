using System;
using System.Collections.Generic;
using System.Text;

namespace doorfail.zethos.UI
{
    public enum Scope
    {
        PUBLIC,
        PRIVATE,
        PROTECTED,
        PUBLIC_STATIC,
        PRIVATE_STATIC,
        PROTECTED_STATIC
    }

    public class Space
    {
        public string name;
        public List<Class> classes = new List<Class>();
        public List<Function> functions = new List<Function>();
        public List<Property> properties = new List<Property>();
        public List<Space> inheritance = new List<Space>();

        public Space(detectionString detectionString)
        {
            name = detectionString.entry;
            foreach (var item in detectionString.varValue)
                inheritance.Add(new Space(item));
        }
    }

    public class Class
    {
        public string name;
        public List<Property> properties =new List<Property>();
        public List<Function> methods = new List<Function>();
        public List<Function> constructors = new List<Function>();
        public List<Class> inheritance = new List<Class>();
        public Scope scope;
        bool template;

        public Class(detectionString detectionString, bool template =false)
        {
            name = detectionString.entry;
            this.template = template;
        }
    }
    public class Function
    {
        public string name;
        public List<Property> parameters = new List<Property>();
        public List<detectionString> lines = new List<detectionString>();
        public Scope scope;
    }
    public class Property
    {
        public string name;
        public Scope scope;
        public string type;
        public List<detectionString> value;
    }
}