using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.DTO
{
    /// <summary>
    /// 账单通信参数
    /// </summary>
    public class UserBillDTO
    {
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
        /// token
        /// </summary>
        public string token { get; set; }
    }
}
