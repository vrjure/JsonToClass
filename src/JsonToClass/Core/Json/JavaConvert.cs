using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToClass
{
    internal class JavaConvert : LangJsonConverter
    {
        public JavaConvert(ClassOption option) : base(option)
        {
        }

        protected override string IntegerString => "int";

        protected override string FloatString => "double";

        protected override string StringString => "String";

        protected override string BooleanString => "boolean";

        protected override string DateTimeString => "Date";

        protected override string ArrayLeftString => "IList<";

        protected override string ArrayRightString => ">";

        protected override void RenderComment(StringBuilder? sb, string? content)
        {
            sb?.AppendLine("    /**");
            sb?.AppendLine($"     *{content}");
            sb?.AppendLine("     */");
        }

        protected override void RenderEndObject(StringBuilder? sb, string? classsName)
        {
            sb?.AppendLine("}");
        }

        protected override void RenderProperty(StringBuilder? sb, string? propertyName, string? propertyType)
        {
            sb?.AppendLine($"    public {propertyType} {propertyName};");
        }

        protected override void RenderStartObject(StringBuilder? sb, string? classsName)
        {
            sb?.AppendLine("{");
        }
    }
}
