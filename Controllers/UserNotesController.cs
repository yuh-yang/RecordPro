using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecordPRO.DTO;
using RecordPRO.Models;
using RecordPRO.Services;
using RecordPRO.Utils;

namespace RecordPRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserNotesController : ControllerBase
    {
        private readonly RecordPROContext _context;
        private readonly IUtils Utils;
        private readonly IServices _services;

        public UserNotesController(RecordPROContext context, IUtils requestVerification, IServices services)
        {
            _context = context;
            Utils = requestVerification;
            _services = services;
        }

        // GET: api/UserNotes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserNote>>> GetUserNote()
        {
            return await _context.UserNote.ToListAsync();
        }

        // GET: api/UserNotes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserNote>> GetUserNote(int id)
        {
            var userNote = await _context.UserNote.FindAsync(id);

            if (userNote == null)
            {
                return NotFound();
            }

            return userNote;
        }

        // PUT: api/UserNotes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserNote(int id, UserNote userNote)
        {
            if (id != userNote.id)
            {
                return BadRequest();
            }

            _context.Entry(userNote).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserNoteExists(id))
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
        /// 增加一条小记，需要token
        /// </summary>
        /// <param name="userNote"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostUserNote(UserNoteDTO userNote)
        {
            var userid = Utils.VerifyRequest(userNote.token);
            if (userid is null)
            {
                return StatusCode(403);
            }
            //调用百度API进行自然语言处理
            var client = Utils.GetBaiduClient();
            //情感分析
            var sentiment_result = client.SentimentClassify(userNote.content);
            //关键词提取
            var keywords = _services.ExtractKeywords(userNote.content);
            //存储
            var note = new UserNote();
            note.userid = int.Parse(userid);
            note.content = userNote.content;
            note.wordcount = userNote.wordcount;
            note.dateTime = userNote.dateTime;
            note.sentiment = sentiment_result["items"][0].Value<int>("sentiment");
            note.tags = keywords;
            _context.UserNote.Add(note);
            _context.SaveChanges();

            return StatusCode(201);
        }

        // DELETE: api/UserNotes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserNote>> DeleteUserNote(int id)
        {
            var userNote = await _context.UserNote.FindAsync(id);
            if (userNote == null)
            {
                return NotFound();
            }

            _context.UserNote.Remove(userNote);
            await _context.SaveChangesAsync();

            return userNote;
        }

        private bool UserNoteExists(int id)
        {
            return _context.UserNote.Any(e => e.id == id);
        }
    }
}
