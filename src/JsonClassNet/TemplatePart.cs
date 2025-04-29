namespace JsonClassNet;

public readonly struct TemplatePart(TemplatePartKind kind, int startIndex, int endIndex)
{
    public TemplatePartKind Kind { get; } = kind;
    public int StartIndex { get; } = startIndex;
    public int EndIndex { get; } = endIndex;

    public ReadOnlySpan<byte> GetValue(ReadOnlySpan<byte> template)
    {
        return template.Slice(StartIndex, EndIndex - StartIndex);
    }

    public ReadOnlySpan<byte> GetValueWithoutPrefix(ReadOnlySpan<byte> template)
    {
        var start = StartIndex + 1;
        return template.Slice(start, EndIndex - start);
    }
}

public enum TemplatePartKind
{
    None = 0,
    Define,
    Parameter,
    Text
}