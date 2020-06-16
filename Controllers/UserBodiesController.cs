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
    public class UserBodiesController : ControllerBase
    {
        private readonly RecordPROContext _context;
        private readonly IUtils _utils;
        public UserBodiesController(RecordPROContext context, IUtils utils)
        {
            _context = context;
            _utils = utils;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserBody>>> GetUserBody(string token)
        {
            //鉴权
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }

            var userBody = await _context.UserBody.Where(s=>s.userid==int.Parse(userid)).ToListAsync();

            if (userBody == null)
            {
                return NotFound();
            }

            return userBody;
        }

        [HttpPut]
        public async Task<IActionResult> PutUserBody(int id, UserBodyDTO dto)
        {
            //鉴权
            var userid = _utils.VerifyRequest(dto.token);
            if (userid is null)
            {
                return StatusCode(403);
            }

            UserBody userBody = dto.ToEntity(int.Parse(userid));
            userBody.id = id;
            _context.Entry(userBody).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserBodyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(200);
        }

        [HttpPost]
        public async Task<ActionResult<UserBody>> PostUserBody(UserBodyDTO dto)
        {
            //鉴权
            var userid = _utils.VerifyRequest(dto.token);
            if (userid is null)
            {
                return StatusCode(403);
            }

            UserBody userBody = dto.ToEntity(int.Parse(userid));
            _context.UserBody.Add(userBody);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserBody", new { id = userBody.id }, userBody);
        }


        [HttpDelete]
        public async Task<ActionResult<UserBody>> DeleteUserBody(int id, string token)
        {
            //鉴权
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }

            var userBody = await _context.UserBody.FindAsync(id);
            if (userBody == null)
            {
                return NotFound();
            }

            _context.UserBody.Remove(userBody);
            await _context.SaveChangesAsync();

            return StatusCode(200);
        }

        private bool UserBodyExists(int id)
        {
            return _context.UserBody.Any(e => e.id == id);
        }
    }
}
