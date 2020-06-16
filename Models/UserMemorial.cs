using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.Models
{
    public class UserMemorial
    {
        /// <summary>
        /// 纪念日ID
        /// </summary>
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        /// <summary>
        /// 纪念日类型:Birth/Important/Special
        /// </summary>
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
        /// 用户ID
        /// </summary>
        public int userid { get; set; }
    }
}
