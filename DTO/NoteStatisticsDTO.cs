using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.DTO
{
    /// <summary>
    /// 小记统计参数
    /// </summary>
    public class NoteStatisticsDTO
    {
        public NoteStatisticsDTO(float negative_ratio, float positive_ratio, float neutral_ratio, float sentiment_mean, object sentiment_mode, int wordcount_mean, object wordcount_least, object topthree)
        {
            this.negative_ratio = negative_ratio;
            this.positive_ratio = positive_ratio;
            this.neutral_ratio = neutral_ratio;
            this.sentiment_mean = sentiment_mean;
            this.sentiment_mode = sentiment_mode;
            this.wordcount_mean = wordcount_mean;
            this.wordcount_least = wordcount_least;
            this.topthree = topthree;
        }

        /// <summary>
        /// 负面情感比例
        /// </summary>
        public float negative_ratio { get; set; }
        /// <summary>
        /// 正面情感比例
        /// </summary>
        public float positive_ratio { get; set; }
        /// <summary>
        /// 中性情感比例
        /// </summary>
        public float neutral_ratio { get; set; }
        /// <summary>
        /// 平均情感
        /// </summary>
        public float sentiment_mean { get; set; }
        /// <summary>
        /// 情感众数
        /// </summary>
        public object sentiment_mode { get; set; }
        /// <summary>
        /// 平均字数
        /// </summary>
        public int wordcount_mean { get; set; }
        /// <summary>
        /// 最少字数及其内容
        /// </summary>
        public object wordcount_least { get; set; }
        /// <summary>
        /// 最常出现的三个关键词
        /// </summary>
        public object topthree { get; set; }
    }
}
