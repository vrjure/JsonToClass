using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToClass
{
    internal class LineOption
    {
        /// <summary>
        /// 属性名规则
        /// </summary>
        public LineRule ProertyRule { get; set; }
        /// <summary>
        /// 属性注释规则
        /// </summary>
        public LineRule CommentsRule { get; set; }
        public string? CommentsTrim { get; set; }
    }

    internal enum LineRule
    {
        None,
        /// <summary>
        /// 每一行
        /// </summary>
        EachRow,
        /// <summary>
        /// 奇数行
        /// </summary>
        OddRow,
        /// <summary>
        /// 偶数行
        /// </summary>
        EvenRow,
        /// <summary>
        /// 以...开始
        /// </summary>
        StartWith,
        /// <summary>
        /// 以...结束
        /// </summary>
        EndWith
    }
}
