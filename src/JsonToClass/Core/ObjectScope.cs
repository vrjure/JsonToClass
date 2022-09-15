using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToClass
{
    internal class ObjectScope
    {
        public ObjectScope(string? className)
        {
            this.ClassName = className;
        }

        public int ArrayDepth { get; set; } = 0;
        public string? ClassName { get; }

        public Stack<PropertyObject> Properties = new Stack<PropertyObject>();
    }
}
