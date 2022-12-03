using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToClass
{
    public class ClassOption
    {
        public bool CommentInclude { get; set; } = true;
        public PropertyNamePolicy PropertyNamePolicy { get; set; } = PropertyNamePolicy.CamelCase;
        public string? IntegerString { get; set; }
        public string? FloatString { get; set; }
        public string? StringString { get; set; }
        public string? BooleanString { get; set; }
        public string? DateTimeString { get; set; }
        public string? PropertyString { get; set; }
        public CommentOption? CommentOption { get; set; }
        public ObjectOption? ObjectOPtion { get; set; }
        public ArrayOption? ArrayOption { get; set; }
        public ICollection<PropertyNameFilter>? Filters { get; set; }
    }
}
