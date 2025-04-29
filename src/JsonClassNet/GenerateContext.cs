using System.Text;

namespace JsonClassNet;

public class GenerateContext(byte[] template, Func<TextWriter> writerFactory)
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
    

    internal bool _initialized = false;
    internal int _arrayDepth = 0;
    internal int _classCount = 0;
    internal Stack<TextWriter> _writers = new Stack<TextWriter>();
    internal Stack<TextWriter> _outputs = new Stack<TextWriter>();
    internal Dictionary<string, IList<TemplatePart>> Defines { get; } = new Dictionary<string, IList<TemplatePart>>();
    internal Dictionary<string, string?> Parameters { get; } = new Dictionary<string, string?>();
    
    internal readonly Func<TextWriter> _writerFactory = writerFactory;
    
    public GenerateContext(string template) : this(Encoding.UTF8.GetBytes(template))
    {
        
    }

    public GenerateContext(string template, Func<TextWriter> writerFactory) : this(Encoding.UTF8.GetBytes(template), writerFactory)
    {
        
    }
    
    public GenerateContext(byte[] template) : this(template, () => new StringWriter())
    {
        
    }
    
    
    internal ReadOnlySpan<byte> GetTemplate() => template.AsSpan();
    
    internal string? GetDefineValue(ReadOnlySpan<byte> template, string define)
    {
        var sw = new StringWriter();
        WriteDefineValue(template, define, sw);
        return sw.GetStringBuilder().ToString();
    }

    internal string? GetParameterValue(string parameter)
    {
        if (Parameters == null || !Parameters.TryGetValue(parameter, out var value)) return null;
        return value;
    }

    internal void WriteDefineValue(ReadOnlySpan<byte> template, string define, TextWriter writer)
    {
        if (Defines == null || string.IsNullOrEmpty(define)) return;
        if (Defines.TryGetValue(define, out var result))
        {
            foreach (var part in result)
            {
                if (part.Kind == TemplatePartKind.Parameter)
                {
                    var p = Encoding.UTF8.GetString(part.GetValue(template));
                    var pValue = GetParameterValue(p);
                    writer.Write(pValue ?? p);
                }
                else
                {
                    writer.Write(Encoding.UTF8.GetString(part.GetValue(template)));
                }
            }
        };
    }
}