using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToClass
{
    internal class LangLineConverter : LangConverterBase
    {
        private readonly string _className = "SampleClass";
        public LangLineConverter(ClassOption option) : base(option)
        {
        }

        public override string Convert(string lines)
        {
            var sb = new StringBuilder();

            StartRender(sb);

            RenderStartObject(sb, _className);

            using (var sr = new StringReader(lines))
            {
                var line = sr.ReadLine();
                while (line != null)
                {
                    var propertyName = line.Trim();

                    if (!string.IsNullOrEmpty(propertyName))
                    {
                        RenderProperty(sb, FormatPropertyName(propertyName), StringString);

                    }

                    line = sr.ReadLine();
                }
            }

            RenderEndObject(sb, _className);

            EndRender(sb);

            return sb.ToString();
        }
    }
}
