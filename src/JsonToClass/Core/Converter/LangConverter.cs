using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToClass
{
    internal class LangConverter : LangConverterBase
    {
        public LangConverter(JsonToClassOption option) : base(option)
        {
        }

        protected override string? IntegerString => option.IntegerString;

        protected override string? FloatString => option.FloatString;

        protected override string? StringString => option.StringString;

        protected override string? BooleanString => option.BooleanString;

        protected override string? DateTimeString => option.DateTimeString;

        protected override string? ArrayLeftString => option.ArrayOption?.StartArray;

        protected override string? ArrayRightString => option.ArrayOption?.EndArray;

        protected override void RenderComment(StringBuilder? sb, string? content)
        {
            sb?.AppendLine(option.CommentOption?.CommentStart);
            if (!string.IsNullOrEmpty(option?.CommentOption?.CommentContent))
            {
                sb?.AppendLine(string.Format(option?.CommentOption?.CommentContent!, content));
            }

            sb?.AppendLine(option!.CommentOption?.CommentEnd);
        }

        protected override void RenderEndObject(StringBuilder? sb, string? className)
        {
            if (!string.IsNullOrEmpty(option.ObjectOPtion?.EndObject))
            {
                sb?.AppendLine(string.Format(option.ObjectOPtion!.EndObject, className));
            }
        }

        protected override void RenderProperty(StringBuilder? sb, string? propertyName, string? propertyType)
        {
            if (!string.IsNullOrEmpty(option.PropertyString))
            {
                sb?.AppendLine(string.Format(option.PropertyString, propertyName, propertyType));
            }
        }

        protected override void RenderStartObject(StringBuilder? sb, string? className)
        {
            if (!string.IsNullOrEmpty(option.ObjectOPtion?.StartObject))
            {
                sb?.AppendLine(string.Format(option.ObjectOPtion!.StartObject, className));
            }
        }
    }
}
