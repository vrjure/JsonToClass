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

                    var result = AnalysisLine(line, index);
                    if (result.HasValue && result.Value)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            RenderProperty(sb, FormatPropertyName(line.Trim()), StringString);
                        }
                    }
                    else if (result.HasValue && !result.Value)
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

                                RenderComment(sb, CommentHandle(line));
                                sb.Append(cache);
                            }
                        }
                        else
                        {
                            RenderComment(sb, CommentHandle(line));
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

        private bool? AnalysisLine(string line, int lineIndex)
        {
            if (_lineOption == null || _lineOption.ProertyRule == LineRule.EachRow || _lineOption.ProertyRule == LineRule.None)
            {
                return true;
            }

            if (_lineOption.ProertyRule == LineRule.OddRow || _lineOption.ProertyRule == LineRule.EvenRow)
            {
                return EvenOddHandle(line, lineIndex);
            }

            return default;
        }

        private bool EvenOddHandle(string line, int lineIndex)
        {
            if (lineIndex % 2 == 0)
            {
                if (_lineOption.ProertyRule == LineRule.EvenRow)
                {
                    return true;
                }
                else if (_lineOption.CommentsRule == LineRule.EvenRow)
                {
                    return false;
                }
            }
            else
            {
                if (_lineOption.ProertyRule == LineRule.OddRow)
                {
                    return true;
                }
                else if (_lineOption.CommentsRule == LineRule.OddRow)
                {
                    return false;
                }
            }

            return default;
        }

        private string CommentHandle(string comments)
        {
            if (_lineOption == null)
            {
                return comments;
            }

            if (!string.IsNullOrEmpty(_lineOption.CommentsTrim))
            {
                comments = comments.Trim(_lineOption.CommentsTrim.ToCharArray());
            }

            return comments;
        }
    }
}
