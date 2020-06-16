using RecordPRO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.DTO
{
    public class UserBodyDTO
    {
        public UserBody ToEntity(int userid)
        {
            var entity = new UserBody();
            entity.weight = this.weight;
            entity.shoulder = this.shoulder;
            entity.height = this.height;
            entity.bust = this.bust;
            entity.waist = this.waist;
            entity.hips = this.hips;
            entity.dateTime = this.dateTime;
            entity.userid = userid;
            return entity;
        }
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
        public string token { get; set; }
    }
}
