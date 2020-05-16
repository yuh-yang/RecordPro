using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.Models
{
    public class User
    {
        /// <summary>
        /// ID主键
        /// </summary>
        /// 
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        /// 
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 登陆凭证
        /// </summary>
        public string Token { get; set; }
    }
}
