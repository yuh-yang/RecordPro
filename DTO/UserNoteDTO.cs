using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.DTO
{
    /// <summary>
    /// 小记通信参数
    /// </summary>
    public class UserNoteDTO
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 字数
        /// </summary>
        public int wordcount { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime dateTime { get; set; }
        /// <summary>
        /// token
        /// </summary>
        public string token { get; set; }
    }
}
