using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

namespace JsonClassNet;

public ref struct ClassTemplate
{
    private GenerateContext _context;

    public ClassTemplate(GenerateContext context)
    {
        this._context = context;
    }
    
    public IEnumerable<TextWriter> Generate(ref Utf8JsonReader reader)
    {
        _context.Reset();
        WriteNext(ref reader);

        var list = new List<TextWriter>();
        while (_context.Outputs.TryPop(out TextWriter o))
        {
            o.Flush();
            list.Add(o);
        }
        return list;
    }

    private void WriteNext(ref Utf8JsonReader reader)
    {
        if (!reader.Read()) return;

        if (_context.ArrayDepth > 0 && (reader.TokenType == JsonTokenType.StartObject
            || reader.TokenType == JsonTokenType.Number
            || reader.TokenType == JsonTokenType.String
            || reader.TokenType == JsonTokenType.True
            || reader.TokenType == JsonTokenType.False
            || reader.TokenType == JsonTokenType.Null))
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                reader.Skip();
                _context.ArrayDepth--;
                reader.Read();
            }
            else
            {
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray);
            }

        }
        
        WriteToken(ref reader);
    }
    
    private void WriteToken(ref Utf8JsonReader reader)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.StartObject:
                WriteStartObject(ref reader);
                break;
            case JsonTokenType.EndObject:
                WriteEndObject(ref reader);
                break;
            case JsonTokenType.EndArray:
                WriteNext(ref reader);
                break;
            case JsonTokenType.PropertyName:
                WriteProperty(ref reader);
                break;
            case JsonTokenType.Comment:
            default:
                break;
        }
    }

    private void WriteStartObject(ref Utf8JsonReader reader)
    {
        _context.Writers.Push(_context.CreateWriter());
        _context.ClassCount++;
        SetClassName();
        var writer = _context.Writers.Peek();
        _context.WriteDefineValue(GenerateContext.StartObjectDef, writer);
        
        WriteNext(ref reader);
    }

    private void WriteEndObject(ref Utf8JsonReader reader)
    {
        var writer = _context.Writers.Pop();
        _context.Outputs.Push(writer);
        _context.WriteDefineValue(GenerateContext.EndObjectDef, writer);
        
        WriteNext(ref reader);
    }

    private void WriteProperty(ref Utf8JsonReader reader)
    {
        var writer = _context.Writers.Peek();
        var propertyName = reader.GetString();
        
        _context.Parameters[GenerateContext.ParamPropertyNameDef] = ReadPropertyName(ref reader);
        
        reader.Read();
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                if (DateTime.TryParse(reader.GetString(), out _))
                {
                    _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(GenerateContext.DateTimeDef);
                }
                else
                {
                    _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(GenerateContext.StringDef);
                }
                break;
            case JsonTokenType.Number:
                var number = reader.GetDouble();
                if (number - (long)number != 0)
                {
                    _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(GenerateContext.DoubleDef);
                }
                else
                {
                    _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(GenerateContext.NumberDef);
                }
                break;
            case JsonTokenType.True:
            case JsonTokenType.False:
                _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(GenerateContext.BooleanDef);
                break;
            case JsonTokenType.Null:
                _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(GenerateContext.NullDef);
                break;
            case JsonTokenType.StartArray:
                reader.Read();
                _context.ArrayDepth++;
                GetPropertyArrayType(ref reader);
                _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(GenerateContext.ArrayDef);
                break;
            case JsonTokenType.StartObject:
                _context.Parameters[GenerateContext.ParamPropertyTypeDef] = GetClassName(_context.ClassCount + 1);
                break;
        }
        
        _context.WriteDefineValue(GenerateContext.PropertyDef, writer);
        
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            WriteToken(ref reader);
        }
        
        WriteNext(ref reader);
    }

    private void GetPropertyArrayType(ref Utf8JsonReader reader)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                _context.Parameters[GenerateContext.ParamPropertyArrayTypeDef] = _context.GetDefineValue(GenerateContext.StringDef);
                break;
            case JsonTokenType.Number:
                _context.Parameters[GenerateContext.ParamPropertyArrayTypeDef] = _context.GetDefineValue(GenerateContext.NumberDef);
                break;
            case JsonTokenType.True:
            case JsonTokenType.False:
                _context.Parameters[GenerateContext.ParamPropertyArrayTypeDef] = _context.GetDefineValue(GenerateContext.BooleanDef);
                break;
            case JsonTokenType.Null:
                _context.Parameters[GenerateContext.ParamPropertyArrayTypeDef] = _context.GetDefineValue(GenerateContext.NullDef);
                break;
            case JsonTokenType.StartObject:
                _context.Parameters[GenerateContext.ParamPropertyArrayTypeDef] = GetClassName(_context.ClassCount + 1);
                break;
            default:
                _context.Parameters[GenerateContext.ParamPropertyArrayTypeDef] = "";
                break;
        }
    }

    private void SetClassName()
    {
        _context.Parameters[GenerateContext.ClassNameDef] = _context.GetDefineValue(GenerateContext.ClassNameDef) + (_context.ClassCount <= 1 ? "" : _context.ClassCount);
    }

    private string GetClassName(int index)
    {
        return _context.GetDefineValue(GenerateContext.ClassNameDef) + index;
    }

    private string? ReadPropertyName(ref Utf8JsonReader reader)
    {
        if (!_context.Parameters.TryGetValue(GenerateContext.PropertyNamePolicyDef, out string? namePolicy))
        {
            namePolicy = _context.GetDefineValue(GenerateContext.PropertyNamePolicyDef);
            _context.Parameters[GenerateContext.PropertyNamePolicyDef] = namePolicy;
        }

        if (string.IsNullOrEmpty(namePolicy)) return reader.GetString();
        return namePolicy switch
        {
            nameof(JsonNamingPolicy.CamelCase) => JsonNamingPolicy.CamelCase.ConvertName(reader.GetString()??""),
            nameof(JsonNamingPolicy.KebabCaseLower) => JsonNamingPolicy.KebabCaseLower.ConvertName(reader.GetString()??""),
            nameof(JsonNamingPolicy.KebabCaseUpper) => JsonNamingPolicy.KebabCaseUpper.ConvertName(reader.GetString()??""),
            nameof(JsonNamingPolicy.SnakeCaseLower) => JsonNamingPolicy.SnakeCaseLower.ConvertName(reader.GetString()??""),
            nameof(JsonNamingPolicy.SnakeCaseUpper) => JsonNamingPolicy.SnakeCaseUpper.ConvertName(reader.GetString()??""),
            _ => reader.GetString()??""
        };
    }
}