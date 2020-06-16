using RecordPRO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.DTO
{
    public class UserMemorialDTO
    {
        public string type { get; set; }
        /// <summary>
        /// 纪念日时间，时、分、秒均为0
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// token
        /// </summary>
        public string token { get; set; }
    }
}
