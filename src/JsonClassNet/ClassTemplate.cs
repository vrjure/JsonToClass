using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

namespace JsonClassNet;

public ref struct ClassTemplate
{
    private ReadOnlySpan<byte> _template;
    private GenerateContext _context;

    public ClassTemplate(GenerateContext context)
    {
        this._template = context.GetTemplate();
        this._context = context;
        Initialize();
    }


    public string? GetName() => _context.GetDefineValue(_template, GenerateContext.NameDef);

    public IEnumerable<TextWriter> Generate(Utf8JsonReader reader)
    {
        if (!_context._initialized)
        {
            throw new Exception("Template not initialized");
        }
        WriteNext(ref reader);

        var list = new List<TextWriter>();
        while (_context._outputs.TryPop(out TextWriter o))
        {
            o.Flush();
            list.Add(o);
        }
        return list;
    }

    private void WriteNext(ref Utf8JsonReader reader)
    {
        if (!reader.Read()) return;

        if (_context._arrayDepth > 0 && (reader.TokenType == JsonTokenType.StartObject
            || reader.TokenType == JsonTokenType.Number
            || reader.TokenType == JsonTokenType.String
            || reader.TokenType == JsonTokenType.True
            || reader.TokenType == JsonTokenType.False
            || reader.TokenType == JsonTokenType.Null))
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                reader.Skip();
                _context._arrayDepth--;
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
        _context._writers.Push(_context._writerFactory.Invoke());
        _context._classCount++;
        SetClassName();
        var writer = _context._writers.Peek();
        _context.WriteDefineValue(_template, GenerateContext.StartObjectDef, writer);
        
        WriteNext(ref reader);
    }

    private void WriteEndObject(ref Utf8JsonReader reader)
    {
        var writer = _context._writers.Pop();
        _context._outputs.Push(writer);
        _context.WriteDefineValue(_template, GenerateContext.EndObjectDef, writer);
        
        WriteNext(ref reader);
    }

    private void WriteProperty(ref Utf8JsonReader reader)
    {
        var writer = _context._writers.Peek();
        var propertyName = reader.GetString();
        
        _context.Parameters[GenerateContext.ParamPropertyNameDef] = ReadPropertyName(ref reader);
        
        reader.Read();
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                if (DateTime.TryParse(reader.GetString(), out _))
                {
                    _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(_template, GenerateContext.DateTimeDef);
                }
                else
                {
                    _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(_template, GenerateContext.StringDef);
                }
                break;
            case JsonTokenType.Number:
                var number = reader.GetDouble();
                if (number - (long)number != 0)
                {
                    _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(_template, GenerateContext.DoubleDef);
                }
                else
                {
                    _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(_template, GenerateContext.NumberDef);
                }
                break;
            case JsonTokenType.True:
            case JsonTokenType.False:
                _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(_template, GenerateContext.BooleanDef);
                break;
            case JsonTokenType.Null:
                _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(_template, GenerateContext.NullDef);
                break;
            case JsonTokenType.StartArray:
                reader.Read();
                _context._arrayDepth++;
                GetPropertyArrayType(ref reader);
                _context.Parameters[GenerateContext.ParamPropertyTypeDef] = _context.GetDefineValue(_template, GenerateContext.ArrayDef);
                break;
            case JsonTokenType.StartObject:
                _context.Parameters[GenerateContext.ParamPropertyTypeDef] = GetClassName(_context._classCount + 1);
                break;
        }
        
        _context.WriteDefineValue(_template, GenerateContext.PropertyDef, writer);
        
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
                _context.Parameters[GenerateContext.ParamPropertyArrayTypeDef] = _context.GetDefineValue(_template, GenerateContext.StringDef);
                break;
            case JsonTokenType.Number:
                _context.Parameters[GenerateContext.ParamPropertyArrayTypeDef] = _context.GetDefineValue(_template, GenerateContext.NumberDef);
                break;
            case JsonTokenType.True:
            case JsonTokenType.False:
                _context.Parameters[GenerateContext.ParamPropertyArrayTypeDef] = _context.GetDefineValue(_template, GenerateContext.BooleanDef);
                break;
            case JsonTokenType.Null:
                _context.Parameters[GenerateContext.ParamPropertyArrayTypeDef] = _context.GetDefineValue(_template, GenerateContext.NullDef);
                break;
            case JsonTokenType.StartObject:
                _context.Parameters[GenerateContext.ParamPropertyArrayTypeDef] = GetClassName(_context._classCount + 1);
                break;
            default:
                _context.Parameters[GenerateContext.ParamPropertyArrayTypeDef] = "";
                break;
        }
    }

    private void SetClassName()
    {
        _context.Parameters[GenerateContext.ClassNameDef] = _context.GetDefineValue(_template, GenerateContext.ClassNameDef) + (_context._classCount <= 1 ? "" : _context._classCount);
    }

    private string GetClassName(int index)
    {
        return _context.GetDefineValue(_template, GenerateContext.ClassNameDef) + index;
    }

    private string? ReadPropertyName(ref Utf8JsonReader reader)
    {
        if (!_context.Parameters.TryGetValue(GenerateContext.PropertyNamePolicyDef, out string? namePolicy))
        {
            namePolicy = _context.GetDefineValue(_template, GenerateContext.PropertyNamePolicyDef);
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

    private void Initialize()
    {
        var reader = new TemplateReader(_template);
        IList<TemplatePart>? currentParts = null;
        while (reader.Read(out TemplatePart part))
        {
            if (part.Kind == TemplatePartKind.Define)
            {
                var define = Encoding.UTF8.GetString(part.GetValueWithoutPrefix(_template));
                if (!_context.Defines.TryGetValue(define, out currentParts))
                {
                    currentParts = new List<TemplatePart>();
                    _context.Defines.Add(define, currentParts);
                }
                continue;
            }
            
            currentParts?.Add(part);
        }
        
        _context._initialized = true;
    }
}