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
        private readonly IUtils _utils;
        private readonly IServices _services;
        
        public UserNotesController(RecordPROContext context, IUtils requestVerification, IServices services)
        {
            _context = context;
            _utils = requestVerification;
            _services = services;
        }

        /// <summary>
        /// 获取某用户的小记
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="days">几天内的小记，-1表示全部小记</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<UserNote>> GetUserNote(string token, int days)
        {
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }
            if (days == -1)
            {
                return _context.UserNote.FromSqlInterpolated($"SELECT * FROM usernote WHERE userid={userid}").ToList();
            }
            else
            {
                var requiredDateTime = DateTime.Now.Date.AddDays(-days);
                return _context.UserNote.FromSqlInterpolated($"SELECT * FROM usernote WHERE datetime > {requiredDateTime} AND userid = {userid}").ToList();
            }
        }

        /// <summary>
        /// 按id更新一条小记
        /// </summary>
        /// <param name="id">小记id</param>
        /// <param name="userNote">更新body</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult PutUserNote(int id, UserNoteDTO userNote, string token)
        {
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }
            var note = _context.UserNote.Single(u => u.id == id);
            //调用百度API进行自然语言处理
            var client = _utils.GetBaiduClient();
            //情感分析
            var sentiment_result = client.SentimentClassify(userNote.content);
            //关键词提取
            var keywords = _services.ExtractKeywords(userNote.content);
            //更新
            note.userid = int.Parse(userid);
            note.content = userNote.content;
            note.wordcount = userNote.wordcount;
            note.dateTime = userNote.dateTime;
            note.sentiment = sentiment_result["items"][0].Value<int>("sentiment");
            note.tags = keywords;
            _context.Update<UserNote>(note);
            _context.SaveChanges();
            return StatusCode(200);
        }

        /// <summary>
        /// 增加一条小记，需要token
        /// </summary>
        /// <param name="userNote"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostUserNote(UserNoteDTO userNote)
        {
            var userid = _utils.VerifyRequest(userNote.token);
            if (userid is null)
            {
                return StatusCode(403);
            }
            //调用百度API进行自然语言处理
            var client = _utils.GetBaiduClient();
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

        /// <summary>
        /// 删除一条小记
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult DeleteUserNote(int id, string token)
        {
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }
            var userNote = _context.UserNote.Find(id);
            if (userNote == null)
            {
                return NotFound();
            }

            _context.UserNote.Remove(userNote);
            _context.SaveChanges();
            return StatusCode(200);
        }

        private bool UserNoteExists(int id)
        {
            return _context.UserNote.Any(e => e.id == id);
        }
    }
}
