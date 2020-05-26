using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecordPRO.Models;
using RecordPRO.Services;
using RecordPRO.Utils;

namespace RecordPRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserFacesController : ControllerBase
    {
        private readonly RecordPROContext _context;
        private readonly IUtils _utils;
        private readonly IServices _services;

        public UserFacesController(RecordPROContext context, IUtils requestVerification, IServices services)
        {
            _context = context;
            _utils = requestVerification;
            _services = services;
        }

        /// <summary>
        /// 获取用户照片，days=-1表示全部照片
        /// </summary>
        /// <param name="token"></param>
        /// <param name="days">几天内的照片</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<UserFace>> GetUserFace(string token, int days)
        {
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }
            if (days == -1)
            {
                return _context.UserFace.FromSqlInterpolated($"SELECT * FROM userface WHERE userid = {userid}").ToList();
            }
            else
            {
                var requiredDateTime = DateTime.Now.Date.AddDays(-days);
                return _context.UserFace.FromSqlInterpolated($"SELECT * FROM userface WHERE datetime > {requiredDateTime} AND userid = {userid}").ToList();
            }

        }


        /// <summary>
        /// 上传一张图片，注意请求形式为 multipart/form-data
        /// </summary>
        /// <param name="file">人脸图片，小于1MB</param>
        /// <param name="token"></param>
        /// <param name="datetime">日期时间</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostUserFace(IFormFile file, string token, DateTime datetime)
        {
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }
            //保存图片
            var filePath = Path.Combine("Photos",
            file.FileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                file.CopyTo(stream);
            }
            //解析返回结果
            var res = _services.DetectFace(filePath);
            var data = res["faces"][0];
            //判断笑脸
            int smile;
            if (float.Parse(data["attributes"]["smile"]["threshold"].ToString()) < float.Parse(data["attributes"]["smile"]["threshold"].ToString()))
            {
                smile = 1;
            }
            else
            {
                smile = 0;
            }
            //判断性别
            int gender;
            if (data["attributes"]["gender"]["value"].ToString().Equals("Female"))
            {
                gender = 0;
            }
            else
            {
                gender = 1;
            }
            //组合颜值分数
            int MaleScore = data["attributes"]["beauty"].Value<int>("male_score");
            int FemaleScore = data["attributes"]["beauty"].Value<int>("female_score");
            string beauty = MaleScore + "/" + FemaleScore;
            //情绪排序，找出前二
            var EmotionDict = data["attributes"]["emotion"].ToObject<Dictionary<string, float>>();
            string emotion = String.Empty;
            var Emotion1 = EmotionDict.OrderByDescending(x => x.Value).First().Key +":"+ EmotionDict.OrderByDescending(x => x.Value).First().Value;
            var Emotion2 = EmotionDict.OrderByDescending(x => x.Value).Skip(1).First().Key + ":" + EmotionDict.OrderByDescending(x => x.Value).Skip(1).First().Value;
            emotion = Emotion1 + "/" + Emotion2;
            //组合皮肤状态
            var heath = data["attributes"]["skinstatus"].Value<string>("health");
            var stain = data["attributes"]["skinstatus"].Value<string>("stain");
            var acne = data["attributes"]["skinstatus"].Value<string>("acne");
            var dark_circle = data["attributes"]["skinstatus"].Value<string>("dark_circle");
            var skinstatus = heath + "/" + stain + "/" + acne + "/" + dark_circle;
            //存储
            var face = new UserFace();
            face.age = data["attributes"]["age"].Value<int>("value");
            face.emotion = emotion;
            face.smile = smile;
            face.gender = gender;
            face.filepath = filePath;
            face.beauty = beauty;
            face.facetoken = data.Value<string>("face_token");
            face.skinstatus = skinstatus;
            face.datetime = datetime;
            face.userid = int.Parse(userid);
            _context.UserFace.Add(face);
            _context.SaveChanges();

            return StatusCode(201);

        }

        /// <summary>
        /// 按照片id删除一张照片
        /// </summary>
        /// <param name="id">照片id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult<UserFace>> DeleteUserFace(int id, string token)
        {
            //鉴权
            var userid = _utils.VerifyRequest(token);
            if (userid is null)
            {
                return StatusCode(403);
            }
            //删除
            var userFace = await _context.UserFace.FindAsync(id);
            if (userFace == null)
            {
                return NotFound();
            }

            _context.UserFace.Remove(userFace);
            await _context.SaveChangesAsync();

            return userFace;
        }

        private bool UserFaceExists(int id)
        {
            return _context.UserFace.Any(e => e.id == id);
        }
    }
}
