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


        /// <summary>
        /// 用户群体统计数据
        /// </summary>
        /// <returns>情感比例/平均情感/情感众数/平均字数/最少字数及其内容/3个高频词</returns>
        /// <param name="token"></param>
        /// <param name="period">统计周期，week或month</param>
        [HttpGet("statistics")]
        public ActionResult<NoteStatisticsDTO> Analyze(string token, string period)
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
            //基数据
            var BaseData = _context.UserNote
                .Where(b => b.dateTime > TimePeriod);
            float BaseDataCount = BaseData.Count();
            //计算情感比例
            var negative_ratio = (float)Math.Round(BaseData.Where(b => b.sentiment == 0).Count() / BaseDataCount, 2);
            var neutral_ratio = (float)Math.Round(BaseData.Where(b => b.sentiment == 1).Count() / BaseDataCount, 2);
            var positive_ratio = (float)Math.Round(BaseData.Where(b => b.sentiment == 2).Count() / BaseDataCount, 2);
            //计算平均情感
            int[] SentimentArray = BaseData.Select(s => s.sentiment).ToArray();
            int SentimentSum = 0;
            foreach (int i in SentimentArray)
            {
                SentimentSum += i;
            }
            float sentiment_mean = (float)Math.Round(SentimentSum / (float)SentimentArray.Length, 2);
            //计算情感众数
            var MostPresentTimes = SentimentArray.Distinct().Max(i => SentimentArray.Count(j => j == i));
            var MostPresent = SentimentArray.Distinct().Where(i => SentimentArray.Count(j => j == i) == MostPresentTimes).First();
            var sentiment_mode = new { sentiment = MostPresent, times = MostPresentTimes };
            //计算平均字数
            int[] WordCountArray = BaseData.Select(s => s.wordcount).ToArray();
            int WordSum = 0;
            foreach (int i in WordCountArray)
            {
                WordSum += i;
            }
            int wordcount_mean = WordSum / WordCountArray.Length;
            //求出最小字数及其内容
            var NoteWithLeastWord = BaseData.Where(s=>s.wordcount==BaseData.Min(s=>s.wordcount)).First();
            var wordcount_least = new { content = NoteWithLeastWord.content, wordcount = NoteWithLeastWord.wordcount };
            //找出3个高频关键词
            var keywords_list_orig = BaseData.Select(s => s.tags).ToList();
            var keyword_list = new List<string>();
            //字符串解包
            foreach(string s in keywords_list_orig)
            {
                var temps = s.Split('/');
                foreach(string st in temps)
                {
                    if (!(st.Equals("")))
                    {
                        keyword_list.Add(st);
                    }
                }
            }
            //使用LINQ查询
            //按次数分组
            var query = keyword_list.GroupBy(x => x)
                        .OrderByDescending(g=>g.Count())
                        .Select(g => new { g.Key,Count=g.Count()})
                        .ToList();
            var topthree = query.Take(3).ToList();
            return new NoteStatisticsDTO(negative_ratio, positive_ratio, neutral_ratio, sentiment_mean, sentiment_mode, wordcount_mean, wordcount_least, topthree);
        }

        private bool UserNoteExists(int id)
        {
            return _context.UserNote.Any(e => e.id == id);
        }
    }
}
