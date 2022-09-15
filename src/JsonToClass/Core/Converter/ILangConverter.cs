using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToClass
{
    public interface ILangConverter
    {
        string Convert(string json);
    }
}
