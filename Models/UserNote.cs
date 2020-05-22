using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.Models
{
    public class UserNote
    {
        /// <summary>
        /// 文本id
        /// </summary>
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        /// <summary>
        /// 文本内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 字数
        /// </summary>
        public int wordcount { get; set; }
        /// <summary>
        /// 情感极性，0：负向；1：中性；2：正向
        /// </summary>
        public int sentiment { get; set; }
        /// <summary>
        /// 文本关注点标签
        /// </summary>
        public string tags { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime dateTime { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int userid { get; set; }
    }
}
