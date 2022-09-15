using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToClass
{
    internal sealed class LanguageDefaultConfig
    {
        public static ICollection<LangConfig> DefaultConfigs { get; set; } = new LangConfig[]
        {
            new LangConfig
            {
                Language = "C#",
                IntegerString = "int",
                StringString = "string",
                BooleanString = "boolean",
                DateTimeString = "DateTime",
                FloatString = "double",
                ArrayOption = new ArrayOption
                {
                    EndArray = ">?",
                    StartArray = "ICollection<"
                },
                ObjectOPtion = new ObjectOption
                {
                    StartObject = "public class {0}\r\n{{",
                    EndObject = "}}"
                },
                CommentOption = new CommentOption
                {
                    CommentStart = "    /// <summary>",
                    CommentContent ="    /// {0}",
                    CommentEnd = "    /// </summary>"
                },
                PropertyString = "    public {1} {0} {{ get; set; }}",
                CommentInclude = true,
                PropertyNamePolicy = PropertyNamePolicy.CamelCase
            },
            new LangConfig
            {
                Language = "Java",
                IntegerString = "int",
                StringString = "String",
                BooleanString = "boolean",
                DateTimeString = "String",
                FloatString = "double",
                ArrayOption = new ArrayOption
                {
                    EndArray = ">",
                    StartArray = "IList<"
                },
                ObjectOPtion = new ObjectOption
                {
                    StartObject = "public class {0}\r\n{{",
                    EndObject = "}}"
                },
                CommentOption = new CommentOption
                {
                    CommentStart = "    /**",
                    CommentContent ="    * {0}",
                    CommentEnd = "    */"
                },
                PropertyString = "    public {1} {0};",
                CommentInclude = true,
                PropertyNamePolicy = PropertyNamePolicy.LowerCameCase
            }
        };
    }
}
