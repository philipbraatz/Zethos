using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace doorfail.zethos.UI
{
    public static class TypeConversion
    {
        private static Dictionary<String, Type> typeValuePairs = new Dictionary<string, Type>()
        {
            { "void", typeof(void)},
            { "int", typeof(int)},
            {"string",typeof(string) }
        };

        public static Type GetType(string value)
        {
            Type t = typeValuePairs.Where(c => c.Key == value).FirstOrDefault().Value;
            if (t is null)
                throw new NullReferenceException();
            return t;
        }
    }
}
