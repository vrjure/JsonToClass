using System.Text.Json;

namespace JsonClassNet;

public class PascalCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return string.IsNullOrEmpty(name) || char.IsUpper(name[0]) ? name : char.ToUpper(name[0]) + name[1..];
    }
}