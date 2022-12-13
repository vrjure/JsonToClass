using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToClass
{
    internal class LangLineConverter : LangConverterBase
    {
        private readonly string _className = "SampleClass";
        private readonly LineOption _lineOption;

        public LangLineConverter(ClassOption option):this(option, null)
        {

        }

        public LangLineConverter(ClassOption option, LineOption lineOption) : base(option)
        {
            _lineOption = lineOption;
        }

        public override string Convert(string lines)
        {
            var sb = new StringBuilder();

            StartRender(sb);

            RenderStartObject(sb, _className);

            using (var sr = new StringReader(lines))
            {
                var index = 0;
                var line = sr.ReadLine();
                while (line != null)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        line = sr.ReadLine();
                        continue;
                    }

                    if (AnalysisLine(line, index, out string value))
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            RenderProperty(sb, FormatPropertyName(value), StringString);
                        }
                    }
                    else if (!string.IsNullOrEmpty(value))
                    {
                        if (_lineOption != null && _lineOption.CommentsRule == LineRule.OddRow)
                        {
                            var len = sb.Length;
                            var lastLineIndex = -1;
                            for (int i = len - 3; i > 0; i--)
                            {
                                if (sb[i] == '\n' && i - 1 >= 0 && sb[i - 1] == '\r')
                                {
                                    lastLineIndex = i + 1;
                                    break;
                                }
                            }
                            if (lastLineIndex < sb.Length)
                            {
                                var cache = new char[len - lastLineIndex];
                                sb.CopyTo(lastLineIndex, cache, cache.Length);
                                sb.Remove(lastLineIndex, len - lastLineIndex);
                                RenderComment(sb, value);
                                sb.Append(cache);
                            }
                        }
                        else
                        {
                            RenderComment(sb, value);
                        }
                        
                    }

                    line = sr.ReadLine();
                    index++;
                }
            }

            RenderEndObject(sb, _className);

            EndRender(sb);

            return sb.ToString();
        }

        private bool AnalysisLine(string line, int lineIndex, out string value)
        {
            if (_lineOption == null || _lineOption.ProertyRule == LineRule.EachRow || _lineOption.ProertyRule == LineRule.None)
            {
                value = line;
                return true;
            }

            if (_lineOption.ProertyRule == LineRule.OddRow || _lineOption.ProertyRule == LineRule.EvenRow)
            {
                return EvenOddHandle(line, lineIndex, out value);
            }

            value = line;
            return true;
        }

        private bool EvenOddHandle(string line, int lineIndex, out string value)
        {
            if (lineIndex % 2 == 0)
            {
                if (_lineOption.ProertyRule == LineRule.EvenRow)
                {
                    value = line.Trim();
                    return true;
                }
                else if (_lineOption.CommentsRule == LineRule.EvenRow)
                {
                    value = CommentHandle(line);
                    return false;
                }
            }
            else
            {
                if (_lineOption.ProertyRule == LineRule.OddRow)
                {
                    value = line.Trim();
                    return true;
                }
                else if (_lineOption.CommentsRule == LineRule.OddRow)
                {
                    value = CommentHandle(line);
                    return false;
                }
            }

            value = string.Empty;
            return true;
        }

        private string CommentHandle(string comments)
        {
            if (_lineOption == null || string.IsNullOrEmpty(_lineOption.CommentsTrim))
            {
                return comments;
            }

            return comments.Trim(_lineOption.CommentsTrim.ToCharArray());
        }
    }
}
