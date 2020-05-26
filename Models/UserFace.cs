using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.Models
{
    public class UserFace
    {
        /// <summary>
        /// 人脸ID
        /// </summary>
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filepath { get; set; }
        /// <summary>
        /// 男1女0
        /// </summary>
        public int gender { get; set; }
        /// <summary>
        /// 1表示在笑
        /// </summary>
        public int smile { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int age { get; set; }
        /// <summary>
        /// anger：愤怒;disgust：厌恶;fear：恐惧;happiness：高兴;neutral：平静;sadness：伤心;surprise：惊讶
        /// </summary>
        public string emotion { get; set; }
        /// <summary>
        /// 男性/女性 认为的此人脸颜值分数
        /// </summary>
        public string beauty { get; set; }
        /// <summary>
        /// 皮肤状态
        /// 健康/色斑/青春痘/黑眼圈
        /// </summary>
        public string skinstatus { get; set; }
        /// <summary>
        /// 日期时间
        /// </summary>
        public DateTime datetime { get; set; }
        /// <summary>
        /// Face++提供的人脸标识号
        /// </summary>
        public string facetoken { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int userid { get; set; }

    }
}
