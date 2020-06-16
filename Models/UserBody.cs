using RecordPRO.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.Models
{
    public class UserBody
    {
        /// <summary>
        /// 身体数据组ID
        /// </summary>
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        /// <summary>
        /// 体重
        /// </summary>
        public float weight { get; set; }
        /// <summary>
        /// 肩宽
        /// </summary>
        public float shoulder { get; set; }
        /// <summary>
        /// 身高
        /// </summary>
        public float height { get; set; }
        /// <summary>
        /// 胸围
        /// </summary>
        public float bust { get; set; }
        /// <summary>
        /// 腰围
        /// </summary>
        public float waist { get; set; }
        /// <summary>
        /// 臀围
        /// </summary>
        public float hips { get; set; }
        /// <summary>
        /// 日期时间
        /// </summary>
        public DateTime dateTime { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int userid { get; set; }

    }
}
