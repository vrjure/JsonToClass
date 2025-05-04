using System.Text;
using System.Text.Json;

namespace JsonClassNet;

public class GenerateContext
{
    internal static readonly byte ParameterStartingPrefixChar = (byte)'$';
    internal static ReadOnlySpan<byte> ParameterStartingPrefix => "$("u8;
    internal static ReadOnlySpan<byte> ParameterEndingSuffix => ")"u8;
    internal static readonly byte DefineStartingPrefix = (byte)'@';
    internal static readonly byte DefineEndingSuffix = (byte)' ';
    internal static readonly byte DefineValueStartingPrefix = (byte)'"';
    internal static readonly byte DefineValueEndingSuffix = (byte)'"';
    internal static readonly byte Empty = (byte)' ';
    internal static ReadOnlySpan<byte> KeyChars => "@$\""u8;
    internal static readonly string NameDef ="name";
    internal static readonly string StringDef = "string";
    internal static readonly string NumberDef = "number";
    internal static readonly string BooleanDef = "boolean";
    internal static readonly string DateTimeDef = "dateTime";
    internal static readonly string DoubleDef = "double";
    internal static readonly string ArrayDef = "array";
    internal static readonly string StartObjectDef = "startObject";
    internal static readonly string EndObjectDef = "endObject";
    internal static readonly string PropertyDef = "property";
    internal static readonly string PropertyDefArray = "propertyArray";
    internal static readonly string NullDef = "null";
    internal static readonly string ClassNameDef = "className";
    internal static readonly string PropertyNamePolicyDef = "propertyNamePolicy";
    
    internal static readonly string ParamPropertyTypeDef = "propertyType";
    internal static readonly string ParamPropertyNameDef = "propertyName";
    internal static readonly string ParamPropertyArrayTypeDef = "propertyArrayType";
    
    internal int ArrayDepth = 0;
    internal int ClassCount = 0;
    internal Stack<TextWriter> Writers = new Stack<TextWriter>();
    internal Stack<TextWriter> Outputs = new Stack<TextWriter>();
    internal Dictionary<string, IList<TemplatePart>> Defines { get; } = new Dictionary<string, IList<TemplatePart>>();
    internal Dictionary<string, string?> Parameters { get; } = new Dictionary<string, string?>();
    
    private readonly Func<TextWriter> _writerFactory;
    private readonly byte[] _template;
    
    public GenerateContext(string template) : this(Encoding.UTF8.GetBytes(template))
    {
        
    }

    public GenerateContext(string template, Func<TextWriter> writerFactory) : this(Encoding.UTF8.GetBytes(template), writerFactory)
    {
        
    }
    
    public GenerateContext(byte[] template) : this(template, () => new StringWriter())
    {
        
    }

    public GenerateContext(byte[] template, Func<TextWriter> writerFactory)
    {
        _writerFactory = writerFactory;
        _template = template;
        Initialize();
    }
    
    internal TextWriter CreateWriter() => _writerFactory.Invoke() ;
    public string? GetDefineValue(string define)
    {
        var sw = new StringWriter();
        WriteDefineValue(define, sw);
        return sw.GetStringBuilder().ToString();
    }

    internal string? GetParameterValue(string parameter)
    {
        return Parameters.GetValueOrDefault(parameter);
    }

    internal void WriteDefineValue(string define, TextWriter writer)
    {
        if (string.IsNullOrEmpty(define)) return;
        if (Defines.TryGetValue(define, out var result))
        {
            foreach (var part in result)
            {
                if (part.Kind == TemplatePartKind.Parameter)
                {
                    var p = Encoding.UTF8.GetString(part.GetValue(_template));
                    var pValue = GetParameterValue(p);
                    writer.Write(pValue ?? p);
                }
                else
                {
                    writer.Write(Encoding.UTF8.GetString(part.GetValue(_template)));
                }
            }
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
                if (!Defines.TryGetValue(define, out currentParts))
                {
                    currentParts = new List<TemplatePart>();
                    Defines.Add(define, currentParts);
                }
                continue;
            }
            
            currentParts?.Add(part);
        }
    }

    internal void Reset()
    {
        ClassCount = 0;
        ArrayDepth = 0;
        Writers.Clear();
        Outputs.Clear();
        Parameters.Clear();
    }

    public override string ToString()
    {
        return Encoding.UTF8.GetString(_template);
    }
}