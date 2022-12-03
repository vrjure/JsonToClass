using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToClass
{
    internal class LangConfig : ClassOption
    {
        public string? Language { get; set; }

        public override string ToString()
        {
            return Language ?? "";
        }
    }
}
