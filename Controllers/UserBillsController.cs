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

        // GET: api/UserBills
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserBill>>> GetUserBill()
        {
            return await _context.UserBill.ToListAsync();
        }

        // GET: api/UserBills/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserBill>> GetUserBill(int id)
        {
            var userBill = await _context.UserBill.FindAsync(id);

            if (userBill == null)
            {
                return NotFound();
            }

            return userBill;
        }

        // PUT: api/UserBills/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserBill(int id, UserBill userBill)
        {
            if (id != userBill.id)
            {
                return BadRequest();
            }

            _context.Entry(userBill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserBillExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
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
            string token;
            try
            {
                token = userBill.token;
            }
            catch (Exception)
            {
                token = null;
            }
            var username = _requestVerification.VerifyRequest(token);
            if(username is null)
            {
                return StatusCode(403);
            }
            //存储
            var user = _context.Users.Single(u => u.Name == username);
            UserBill userBill1 = new UserBill();
            userBill1.amount = userBill.amount;
            userBill1.category = userBill.category;
            userBill1.date = userBill.date;
            userBill1.type = userBill.type;
            userBill1.remark = userBill.remark;
            userBill1.userid = user.id;
            _context.UserBill.Add(userBill1);
            return StatusCode(201);
        }
        // DELETE: api/UserBills/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserBill>> DeleteUserBill(int id)
        {
            var userBill = await _context.UserBill.FindAsync(id);
            if (userBill == null)
            {
                return NotFound();
            }

            _context.UserBill.Remove(userBill);
            await _context.SaveChangesAsync();

            return userBill;
        }

        private bool UserBillExists(int id)
        {
            return _context.UserBill.Any(e => e.id == id);
        }
    }
}
