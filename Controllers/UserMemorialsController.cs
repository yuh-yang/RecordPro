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
    public class UserMemorialsController : ControllerBase
    {
        private readonly RecordPROContext _context;
        private readonly IUtils _utils;

        public UserMemorialsController(RecordPROContext context, IUtils utils)
        {
            _context = context;
            _utils = utils;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserMemorial>>> GetUserMemorial(string token)
        {
            //鉴权
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }

            var userMemorial = await _context.UserMemorial.Where(s => s.userid == int.Parse(userid)).ToListAsync();

            if (userMemorial == null)
            {
                return NotFound();
            }

            return userMemorial;
        }

        [HttpPut]
        public async Task<IActionResult> PutUserMemorial(string token, UserMemorial userMemorial)
        {
            //鉴权
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }

            _context.Entry(userMemorial).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserMemorialExists(userMemorial.id))
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
        public async Task<ActionResult<UserMemorial>> PostUserMemorial(UserMemorialDTO userMemorialDTO)
        {
            //鉴权
            var userid = _utils.VerifyRequest(userMemorialDTO.token);
            if (userid is null)
            {
                return StatusCode(403);
            }

            var userMemorial = new UserMemorial();
            userMemorial.userid = int.Parse(userid);
            userMemorial.type = userMemorialDTO.type;
            userMemorial.date = userMemorialDTO.date;
            userMemorial.desc = userMemorialDTO.desc;
            _context.UserMemorial.Add(userMemorial);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserMemorial", new { id = userMemorial.id }, userMemorial);
        }

        [HttpDelete]
        public async Task<ActionResult<UserMemorial>> DeleteUserMemorial(int id,string token)
        {
            //鉴权
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }

            var userMemorial = await _context.UserMemorial.FindAsync(id);
            if (userMemorial == null)
            {
                return NotFound();
            }

            _context.UserMemorial.Remove(userMemorial);
            await _context.SaveChangesAsync();

            return StatusCode(200);
        }

        private bool UserMemorialExists(int id)
        {
            return _context.UserMemorial.Any(e => e.id == id);
        }
    }
}
