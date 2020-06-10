using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.DTO
{
    /// <summary>
    /// 账单统计参数
    /// </summary>
    public class BillStatisticsDTO
    {

        public BillStatisticsDTO(float avg_income, float avg_expense, float med_income, float med_expense, float income_ratio, float expense_ratio, float invest_income_ratio, float salary_income_ratio)
        {
            this.avg_income = avg_income;
            this.avg_expense = avg_expense;
            this.med_income = med_income;
            this.med_expense = med_expense;
            this.income_ratio = income_ratio;
            this.expense_ratio = expense_ratio;
            this.invest_income_ratio = invest_income_ratio;
            this.salary_income_ratio = salary_income_ratio;
        }

        /// <summary>
        /// 平均收入
        /// </summary>
        public float avg_income { get; set; }
        /// <summary>
        /// 平均支出
        /// </summary>
        public float avg_expense { get; set; }
        /// <summary>
        /// 收入中位数
        /// </summary>
        public float med_income { get; set; }
        /// <summary>
        /// 支出中位数
        /// </summary>
        public float med_expense { get; set; }
        /// <summary>
        /// 收入所处比例
        /// </summary>
        public float income_ratio { get; set; }
        /// <summary>
        /// 支出所处比例
        /// </summary>
        public float expense_ratio { get; set; }
        /// <summary>
        /// 投资收入所处比例
        /// </summary>
        public float invest_income_ratio { get; set; }
        /// <summary>
        /// 工资收入所处比例
        /// </summary>
        public float salary_income_ratio { get; set; }

    }
}
