using System.Text.Json;

namespace JsonClassNet;

public abstract class PropertyNamingPolicy : JsonNamingPolicy
{
    public static readonly PascalCaseNamingPolicy PascalCase = new PascalCaseNamingPolicy();

}