using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Buffers;
using System.Data;

namespace JsonToClass
{
    public abstract class LangConverterBase : ILangConverter
    {
        protected readonly JsonToClassOption option;
        public LangConverterBase(JsonToClassOption option)
        {
            this.option = option;
        }

        private string? currentPropertyName = string.Empty;
        private string? currentObjectPropertyName = string.Empty;
        private StringBuilder? currentComment = new StringBuilder();
        private JsonTokenType lastTokenType = JsonTokenType.None;
        private bool isInObject = false;

        private Stack<ObjectScope> _classStack = new Stack<ObjectScope>();


        protected abstract string? IntegerString { get; }
        protected abstract string? FloatString { get; }
        protected abstract string? StringString { get; }
        protected abstract string? BooleanString { get; }
        protected abstract string? DateTimeString { get; }
        protected abstract string? ArrayLeftString { get; }
        protected abstract string? ArrayRightString { get; }

        public string Convert(string json)
        {
            JsonReaderOptions readOption = new JsonReaderOptions();
            if (option != default)
            {
                if (option.CommentInclude)
                {
                    readOption.CommentHandling = JsonCommentHandling.Allow;
                }
            }
            var reader = new Utf8JsonReader(new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(json)), readOption);

            ObjectScope? oc = default;

            var sb = new StringBuilder();

            while (reader.Read())
            {
                if (_classStack.Count > 0)
                {
                    oc = _classStack.Peek();
                }
                
                switch (reader.TokenType)
                {
                    case JsonTokenType.None:
                        break;
                    case JsonTokenType.StartObject:
                        if (IsStartObjectInArrayNotFirst())//}{
                        {
                            reader.Skip();
                            continue;
                        }
                        isInObject = true;

                        if (lastTokenType == JsonTokenType.PropertyName)
                        {
                            currentObjectPropertyName = currentPropertyName;
                        }

                        oc = GetObject();
                        break;
                    case JsonTokenType.EndObject:
                        isInObject = false;
                        var pop = _classStack.Pop();
                        Render(sb, pop);
                        if (_classStack.Count > 0)
                        {
                            var last = _classStack.Peek();
                            last.Properties.Peek().PropertyType = GetArrayStr(last, pop.ClassName);
                        }
                        break;
                    case JsonTokenType.StartArray:
                        if (IsStartArrayInArrayNotFirst())//][
                        {
                            reader.Skip();
                            continue;
                        }
                        oc!.ArrayDepth++;
                        isInObject = false;
                        if (lastTokenType == JsonTokenType.PropertyName)
                        {
                            currentObjectPropertyName = currentPropertyName;
                        } 
                        break;
                    case JsonTokenType.EndArray:
                        oc!.ArrayDepth--;
                        break;
                    case JsonTokenType.PropertyName:
                        currentPropertyName = FormatPropertyName(reader.GetString());
                        var property = new PropertyObject()
                        {
                            PropertyName = currentPropertyName
                        };
                        if (lastTokenType == JsonTokenType.Comment)
                        {
                            property.PropertyComment = currentComment?.ToString();
                            currentComment?.Clear();
                        }
                        oc!.Properties.Push(property);
                        break;
                    case JsonTokenType.Comment:
                        currentComment?.Append(reader.GetComment());
                        break;
                    case JsonTokenType.Number:
                        if (lastTokenType == JsonTokenType.Number)
                        {
                            break;
                        }

                        string? typeString = string.Empty;
                        if (reader.TryGetInt64(out long _))
                        {
                            typeString = GetArrayStr(oc, IntegerString);
                        }
                        else if (reader.TryGetDouble(out double _))
                        {
                            typeString = GetArrayStr(oc, FloatString);
                        }

                        oc!.Properties.Peek().PropertyType = typeString;
                        break;
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                        if (lastTokenType == reader.TokenType)
                        {
                            break;
                        }
                        oc!.Properties.Peek().PropertyType = GetArrayStr(oc, BooleanString);
                        break;
                    case JsonTokenType.Null:
                        if (lastTokenType == reader.TokenType)
                        {
                            break;
                        }
                        oc!.Properties.Peek().PropertyType = GetArrayStr(oc, StringString);
                        break;
                    case JsonTokenType.String:
                        if (lastTokenType == reader.TokenType)
                        {
                            break;
                        }
                        if (reader.TryGetDateTime(out DateTime _))
                        {
                            oc!.Properties.Peek().PropertyType = GetArrayStr(oc, DateTimeString);
                        }
                        else
                        {
                            oc!.Properties.Peek().PropertyType = GetArrayStr(oc, StringString);
                        }
                        break;
                    default:
                        break;
                }

                lastTokenType = reader.TokenType;
            }

            return sb.ToString();
        }

        protected abstract void RenderStartObject(StringBuilder? sb, string? className);
        protected abstract void RenderEndObject(StringBuilder? sb, string? className);
        protected abstract void RenderComment(StringBuilder? sb, string? content);
        protected abstract void RenderProperty(StringBuilder? sb, string? propertyName, string? propertyType);

        protected virtual string? FormatPropertyName(string? propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                switch (option!.PropertyNamePolicy)
                {
                    case PropertyNamePolicy.None:
                        return propertyName;
                    case PropertyNamePolicy.LowerCameCase:
                        return $"{propertyName?.Substring(0, 1).ToLower()}{propertyName?.Substring(1, propertyName.Length - 1)}";
                    case PropertyNamePolicy.CamelCase:
                        return $"{propertyName?.Substring(0, 1).ToUpper()}{propertyName?.Substring(1, propertyName.Length - 1)}";
                }
            }
            return propertyName;
        }

        protected virtual void StartRender(StringBuilder? sb) { }
        protected virtual void EndRender(StringBuilder? sb)
        {
            
        }

        private void Render(StringBuilder sb, ObjectScope oc)
        {
            RenderStartObject(sb, oc.ClassName);
            Stack<PropertyObject> stack = new Stack<PropertyObject>();
            while (oc.Properties.TryPop(out PropertyObject? pro))
            {
                stack.Push(pro);
            }

            while (stack.TryPop(out PropertyObject? pro))
            {
                if (!string.IsNullOrEmpty(pro.PropertyComment))
                {
                    RenderComment(sb, pro.PropertyComment);
                }
                RenderProperty(sb, pro.PropertyName, pro.PropertyType);
            }
            RenderEndObject(sb, oc.ClassName);

        }

        protected bool IsStartObjectInArray()
        {
            return lastTokenType == JsonTokenType.EndObject || lastTokenType == JsonTokenType.StartArray;
        }

        protected bool IsStartObjectInArrayNotFirst()
        {
            return lastTokenType == JsonTokenType.EndObject;
        }

        protected bool IsStartObjectInArrayFirst()
        {
            return lastTokenType == JsonTokenType.StartArray;
        }

        protected bool IsStartArrayInArrayFirst()
        {
            return lastTokenType == JsonTokenType.StartArray;
        }

        protected bool IsStartArrayInArrayNotFirst()
        {
            return lastTokenType == JsonTokenType.EndArray;
        }

        private string? GetArrayStr(ObjectScope? oc, string? type)
        {
            if (oc?.ArrayDepth <= 0 || isInObject)
            {
                return type;
            }
            var sb = new StringBuilder();
            for (int i = 0; i < oc?.ArrayDepth; i++)
            {
                sb.Append(ArrayLeftString);
                if (i == oc.ArrayDepth - 1)
                {
                    sb.Append(type);
                }
            }
            for (int i = 0; i < oc?.ArrayDepth; i++)
            {
                sb.Append(ArrayRightString);
            }

            return sb.ToString();
        }

        private ObjectScope GetObject()
        {           
            var oc = new ObjectScope(string.IsNullOrEmpty(currentObjectPropertyName) ? $"SampleClass" : currentObjectPropertyName);
            _classStack.Push(oc);
            return oc;
        }
    }
}
