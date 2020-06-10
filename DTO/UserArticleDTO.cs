using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.DTO
{

    public class UserArticleDTO
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 文本内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 字数
        /// </summary>
        public int wordcount { get; set; }
    }
}
