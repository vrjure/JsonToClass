using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToClass
{
    public abstract class LangConverterBase : ILangConverter
    {
        protected readonly ClassOption option;

        public LangConverterBase(ClassOption option)
        {
            this.option = option;
        }

        protected virtual string? IntegerString => option.IntegerString;
        protected virtual string? FloatString => option.FloatString;
        protected virtual string? StringString => option.StringString;
        protected virtual string? BooleanString => option.BooleanString;
        protected virtual string? DateTimeString => option.DateTimeString;
        protected virtual string? ArrayLeftString => option.ArrayOption?.StartArray;
        protected virtual string? ArrayRightString => option.ArrayOption?.EndArray;

        protected virtual void StartRender(StringBuilder? sb) { }
        protected virtual void EndRender(StringBuilder? sb) { }

        protected virtual void RenderComment(StringBuilder? sb, string? content)
        {
            sb?.AppendLine(option.CommentOption?.CommentStart);
            if (!string.IsNullOrEmpty(option?.CommentOption?.CommentContent))
            {
                sb?.AppendLine(string.Format(option?.CommentOption?.CommentContent!, content));
            }

            sb?.AppendLine(option!.CommentOption?.CommentEnd);
        }

        protected virtual void RenderEndObject(StringBuilder? sb, string? className)
        {
            if (!string.IsNullOrEmpty(option.ObjectOPtion?.EndObject))
            {
                sb?.AppendLine(string.Format(option.ObjectOPtion!.EndObject, className));
            }
        }

        protected virtual void RenderProperty(StringBuilder? sb, string? propertyName, string? propertyType)
        {
            if (!string.IsNullOrEmpty(option.PropertyString))
            {
                sb?.AppendLine(string.Format(option.PropertyString, propertyName, propertyType));
            }
        }

        protected virtual void RenderStartObject(StringBuilder? sb, string? className)
        {
            if (!string.IsNullOrEmpty(option.ObjectOPtion?.StartObject))
            {
                sb?.AppendLine(string.Format(option.ObjectOPtion!.StartObject, className));
            }
        }

        public abstract string Convert(string json);


        protected virtual string? FormatPropertyName(string? propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                switch (option!.PropertyNamePolicy)
                {
                    case PropertyNamePolicy.None:
                        break;
                    case PropertyNamePolicy.LowerCameCase:
                        propertyName = $"{propertyName?.Substring(0, 1).ToLower()}{propertyName?.Substring(1, propertyName.Length - 1)}";
                        break;
                    case PropertyNamePolicy.CamelCase:
                        propertyName = $"{propertyName?.Substring(0, 1).ToUpper()}{propertyName?.Substring(1, propertyName.Length - 1)}";
                        break;
                }

                if (option.Filters != null && option.Filters.Count > 0)
                {
                    foreach (var item in option.Filters)
                    {
                        propertyName = PropertyNameFilter(propertyName, item);
                    }
                }
            }

            return propertyName;
        }

        private string? PropertyNameFilter(string? propertyName, PropertyNameFilter filter)
        {
            if (string.IsNullOrEmpty(propertyName) || string.IsNullOrWhiteSpace(filter.Filter))
            {
                return propertyName;
            }
            return propertyName.Replace(filter.Filter, filter.Replace);
        }
    }
}
