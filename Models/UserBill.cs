using RecordPRO.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.Models
{
    public class UserBill
    {
        /// <summary>
        /// 账单ID
        /// </summary>
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        /// <summary>
        /// 账单分类
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 支出类型，true表示指出，false表示收入
        /// </summary>
        public bool type { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public float amount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 格式化日期
        /// </summary>
        public DateTime datetime { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int userid { get; set; }
    }
}
