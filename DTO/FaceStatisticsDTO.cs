using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.DTO
{
    public class FaceStatisticsDTO
    {
        /// <summary>
        /// 颜值所处比例，例如为0.1则说明你的颜值属于前10%
        /// </summary>
        public float beauty_ratio { get; set; }
        /// <summary>
        /// 记录频次，平均每天记录几次自拍
        /// </summary>
        public float record_frequency { get; set; }
        /// <summary>
        /// 笑容比率
        /// </summary>
        public float smile_ratio { get; set; }
        /// <summary>
        /// 出现最多的三个情绪
        /// </summary>
        public object emotion_mode { get; set; }
        /// <summary>
        /// 黑眼圈比率，三个子键分别为健康、基本健康和不健康的比例
        /// </summary>
        public object black_eye_ratio { get; set; }
        /// <summary>
        /// 皮肤健康比率，三个子键分别为健康、基本健康和不健康的比例
        /// </summary>
        public object skin_health_ratio { get; set; }

    }
}
