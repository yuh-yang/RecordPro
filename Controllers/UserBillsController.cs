using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IUtils _requestVerification;
        public UserBillsController(RecordPROContext context, IUtils requestVerification)
        {
            _context = context;
            _requestVerification = requestVerification;
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
            var userid = _requestVerification.VerifyRequest(token);
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
            var userid = _requestVerification.VerifyRequest(token);
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
            var userid = _requestVerification.VerifyRequest(userBill.token);
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
        /// <returns></returns>
        [HttpDelete]
        public IActionResult DeleteUserBill(int id, string token) 
        {
            var userid = _requestVerification.VerifyRequest(token);
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

        private bool UserBillExists(int id)
        {
            return _context.UserBill.Any(e => e.id == id);
        }
    }
}
