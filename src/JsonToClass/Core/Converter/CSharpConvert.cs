using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonToClass
{
    public class CSharpConvert : LangConverterBase
    {      
        public CSharpConvert(JsonToClassOption option) : base(option)
        {

        }

        protected override string IntegerString => "long";

        protected override string FloatString => "double";

        protected override string StringString => "string?";

        protected override string BooleanString => "bool";

        protected override string DateTimeString => "DateTime";

        protected override string ArrayLeftString => "ICollection<";

        protected override string ArrayRightString => ">?";

        protected override void RenderComment(StringBuilder? sb, string? content)
        {
            sb?.AppendLine("    /// <summary>");
            sb?.AppendLine($"    /// {content}");
            sb?.AppendLine("    /// </summary>");
        }

        protected override void RenderEndObject(StringBuilder? sb, string? className)
        {
            sb?.AppendLine("}");
        }

        protected override void RenderProperty(StringBuilder? sb, string? propertyName, string? propertyType)
        {
            sb?.AppendLine($"    public {propertyType} {propertyName} {{ get; set; }}");
        }

        protected override void RenderStartObject(StringBuilder? sb, string? className)
        {
            sb?.AppendLine($"public class {className}");
            sb?.AppendLine("{");
        }
    }
}
