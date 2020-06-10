using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RecordPRO.DTO;
using RecordPRO.Models;
using RecordPRO.Utils;


namespace RecordPRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBillsController : ControllerBase
    {
        private readonly RecordPROContext _context;
        private readonly IUtils _utils;
        public UserBillsController(RecordPROContext context, IUtils requestVerification)
        {
            _context = context;
            _utils = requestVerification;
        }

        /// <summary>
        /// 按token获取用户账单
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="days">几天内的账单，-1表示全部账单</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<UserBill>> GetUserBill(string token, int days)
        {
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }
            if (days == -1)
            {
                return _context.UserBill.FromSqlInterpolated($"SELECT * FROM userbill WHERE userid = {userid}").ToList();
            }
            else
            {
                var requiredDateTime = DateTime.Now.Date.AddDays(-days);
                return _context.UserBill.FromSqlInterpolated($"SELECT * FROM userbill WHERE datetime > {requiredDateTime} AND userid = {userid}").ToList();
            }
        }

        /// <summary>
        /// 按账单id修改账单，需要token
        /// </summary>
        /// <param name="id">账单id</param>
        /// <param name="token">token</param>
        /// <param name="userBill">修改后的账单内容</param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult PutUserBill(int id, string token, UserBillDTO userBill)
        {
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }
            var bill = _context.UserBill.Single(u => u.id == id);
            bill.category = userBill.category;
            bill.amount = userBill.amount;
            bill.datetime = userBill.datetime;
            bill.remark = userBill.remark;
            bill.type = userBill.type;
            _context.Update<UserBill>(bill);
            _context.SaveChanges();
            return StatusCode(201);
        }

        /// <summary>
        /// 提交记账
        /// </summary>
        /// <param name="userBill"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddUserBill(UserBillDTO userBill)
        {
            //鉴权
            var userid = _utils.VerifyRequest(userBill.token);
            if(userid is null)
            {
                return StatusCode(403);
            }
            //存储
            UserBill userBill1 = new UserBill();
            userBill1.amount = userBill.amount;
            userBill1.category = userBill.category;
            userBill1.datetime = userBill.datetime;
            userBill1.type = userBill.type;
            userBill1.remark = userBill.remark;
            userBill1.userid = int.Parse(userid);
            _context.UserBill.Add(userBill1);
            _context.SaveChanges();
            return StatusCode(201);
        }

        /// <summary>
        /// 按id删除账单
        /// </summary>
        /// <param name="id">账单id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult DeleteUserBill(int id, string token) 
        {
            var userid = _utils.VerifyRequest(token);
            if(userid is null)
            {
                return StatusCode(403);
            }
            var userBill = _context.UserBill.Find(id);
            if (userBill == null)
            {
                return NotFound();
            }

            _context.UserBill.Remove(userBill);
            _context.SaveChanges();

            return StatusCode(201);
        }


        /// <summary>
        /// 用户群体统计数据
        /// </summary>
        /// <returns>平均收支/收支中位数/收支超越比例/投资收入超越比例/工资收入超越比例</returns>
        /// <param name="token"></param>
        /// <param name="period">统计周期，week或month</param>
        [HttpGet("statistics")]
        public ActionResult<BillStatisticsDTO> Analyze(string token, string period)
        {
            //鉴权
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }
            //计算时间周期
            DateTime TimePeriod;
            if ("month".Equals(period))
            {
                TimePeriod = DateTime.Now.AddDays(-30);
            }
            else
            {
                TimePeriod = DateTime.Now.AddDays(-7);
            }
            //收支均值与用户数
            var income_avg = _context.UserBill
                .Where(b=> b.datetime>TimePeriod && b.type==false)
                .Average(b=>b.amount);
            var expense_avg = _context.UserBill
                .Where(b => b.datetime > TimePeriod && b.type == true)
                .Average(b => b.amount);
            var user_count = _context.UserBill
                .Select(b=>b.userid==int.Parse(userid))
                .Count();
            //用户收支List与中位数
            float[] income_list = _context.UserBill.Where(b => b.type == false && b.datetime > TimePeriod).Select(p => p.amount).ToArray();
            float[] expense_list = _context.UserBill.Where(b => b.type == true && b.datetime > TimePeriod).Select(p => p.amount).ToArray();
            float income_median = _utils.Median(income_list);
            float expense_median = _utils.Median(expense_list);
            //各用户收入
            var users_income = _context.UserBill
                .Where(b=>b.datetime > TimePeriod && b.type == false)
                .GroupBy(b=>b.userid)
                .Select(g=>new { Userid=g.Key, Income=g.Sum(t=>t.amount)})
                .ToDictionary(g=>g.Userid,g=>g.Income);
            //收入超过多少人
            //找到排在该用户前面的用户数;
            int i = 0, UseridInIncomeDict;
            do
            {
                UseridInIncomeDict = users_income.Keys.ElementAt(i);
                i++;
            } while (UseridInIncomeDict != int.Parse(userid));
            float UserCount = _context.Users.Count();
            var  income_ratio = (float)Math.Round(i / UserCount, 3);

            //各用户支出
            var users_expense = _context.UserBill
                .Where(b => b.datetime > TimePeriod && b.type == true)
                .GroupBy(b => b.userid)
                .Select(g => new { Userid = g.Key, Expense = g.Sum(t => t.amount) })
                .ToDictionary(g => g.Userid, g => g.Expense);
            //支出超过多少人
            //找到排在该用户前面的用户数
            int j = 0, UseridInExpenseDict;
            do
            {
                UseridInExpenseDict = users_income.Keys.ElementAt(j);
                j++;
            } while (UseridInExpenseDict != int.Parse(userid));
            var expense_ratio = (float)Math.Round(j / UserCount, 3);

            //各用户投资收入
            var users_invest_income = _context.UserBill
                .Where(b => b.datetime > TimePeriod && b.type == false && b.category=="investment")
                .GroupBy(b => b.userid)
                .Select(g => new { Userid = g.Key, Income = g.Sum(t => t.amount) })
                .ToDictionary(g => g.Userid, g => g.Income);
            //投资收入超过多少人
            //找到排在该用户前面的用户数;
            int m = 0, UseridInInvestIncomeDict;
            do
            {
                UseridInInvestIncomeDict = users_income.Keys.ElementAt(m);
                m++;
            } while (UseridInInvestIncomeDict != int.Parse(userid));
            var invest_income_ratio = (float)Math.Round(m / UserCount, 3);

            //各用户工资收入
            var users_salary_income = _context.UserBill
            .Where(b => b.datetime > TimePeriod && b.type == false && b.category == "salaries")
            .GroupBy(b => b.userid)
            .Select(g => new { Userid = g.Key, Income = g.Sum(t => t.amount) })
            .ToDictionary(g => g.Userid, g => g.Income);
            //投资收入超过多少人
            //找到排在该用户前面的用户数;
            int g = 0, UseridInSalaryIncomeDict;
            do
            {
                UseridInSalaryIncomeDict = users_income.Keys.ElementAt(g);
                g++;
            } while (UseridInSalaryIncomeDict != int.Parse(userid));
            var salary_income_ratio = (float)Math.Round(g / UserCount, 3);

            //构造DTO对象
            var Stat = new BillStatisticsDTO(income_avg, expense_avg, income_median, expense_median, income_ratio, expense_ratio, invest_income_ratio, salary_income_ratio);
            return Stat;
        }

        private bool UserBillExists(int id)
        {
            return _context.UserBill.Any(e => e.id == id);
        }
    }
}
