using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace JsonClassNet.Tests;

public class Tests
{
    private string? _json;
    private GenerateContext? _csharpTemplate;
    private GenerateContext? _csharpObservableTemplate;
    private GenerateContext? _rustTemplate;
    private GenerateContext? _javaTemplate;
    [SetUp]
    public void Setup()
    {
        _json = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "TestJson.json"));
        _csharpTemplate = new(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "CSharp.txt")));
        _csharpObservableTemplate =new( File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "CSharpObservable.txt")));
        _rustTemplate = new(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Rust.txt")));
        _javaTemplate = new( File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Java.txt")));
    }

    [Test]
    public void TestTemplateReader()
    {
        var _templateData = Encoding.UTF8.GetBytes(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "CSharp.txt")));
        var reader = new TemplateReader(_templateData);
        while (reader.Read(out var part))
        {
            Console.WriteLine($"{part.Kind}:{Encoding.UTF8.GetString(part.GetValue(_templateData))}");
        }
    }

    [Test]
    public void TestCSharpGenerate()
    {
        ClassGenerate(_csharpTemplate!);
    }

    [Test]
    public void TextCSharpObservableGenerate()
    {
        ClassGenerate(_csharpObservableTemplate!);
    }

    [Test]
    public void TestRustGenerate()
    {
        ClassGenerate(_rustTemplate!);
    }

    [Test]
    public void TestJavaGenerate()
    {
        ClassGenerate((_javaTemplate!));
    }

    private void ClassGenerate(GenerateContext context)
    {
        var ct = new ClassTemplate(context);

        var jsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(_json!));
        var writers = ct.Generate(jsonReader);
        foreach (StringWriter writer in writers)
        {
            Console.WriteLine(writer.GetStringBuilder().ToString());
        }
    }
}