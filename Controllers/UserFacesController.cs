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
using RecordPRO.DTO;

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
            if (float.Parse(data["attributes"]["smile"]["threshold"].ToString()) <= float.Parse(data["attributes"]["smile"]["value"].ToString()))
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


        /// <summary>
        /// 用户群体统计数据
        /// </summary>
        /// <param name="token"></param>
        /// <param name="period">时间周期，week或month</param>
        /// <param name="gender">性别，男1女0</param>
        /// <returns>颜值所处比例/记录频率/笑容比率/出现最多的三个情绪/黑眼圈比率/皮肤健康比率</returns>
        [HttpGet("statistics")]
        public ActionResult<FaceStatisticsDTO> Analyze(string token, string period, int gender)
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
            //实例化DTO
            var DTO = new FaceStatisticsDTO();
            //基数据
            var BaseData = _context.UserFace
                .Where(b => b.datetime > TimePeriod && b.gender==gender);
            float BaseDataCount = BaseData.Count();
            var CurrentUser = BaseData
                .Where(s => s.userid == int.Parse(userid))
                .OrderByDescending(s=>s.datetime)
                .First();
            //颜值所占比例
            var users_beauty_orig = BaseData.Select(s => s.beauty).ToList();
            List<float> users_beauty = new List<float>();
            foreach(string s in users_beauty_orig)
            {
                float beauty_sum=0;
                foreach(string k in s.Split('/'))
                {
                    beauty_sum += float.Parse(k);
                }
                users_beauty.Add(beauty_sum / 2);
            }
            //降序排序
            users_beauty.Sort((x, y) => -x.CompareTo(y));
            var current_user_beauty_orig = CurrentUser.beauty;
            float current_user_beauty = 0;
            foreach(string f in current_user_beauty_orig.Split('/'))
            {
                current_user_beauty += float.Parse(f) / 2 ;
            }
            //计算处于该用户前方的颜值个数
            int user_index = 0;
            foreach(var item in users_beauty)
            {
                if (item <= current_user_beauty)
                {
                    user_index = users_beauty.IndexOf(item);
                    break;
                }
            }
            DTO.beauty_ratio = user_index / BaseDataCount;
            //记录频率
            if (period.Equals("month"))
            {
                DTO.record_frequency = BaseDataCount / 30;
            }
            else
            {
                DTO.record_frequency = BaseDataCount / 7;
            }
            //笑容比率
            DTO.smile_ratio = BaseData.Where(s => s.smile == 1).Count() / BaseDataCount;
            //最常出现的三种情绪
            var emotion_list_orig = BaseData.Select(s => s.emotion).ToList();
            var emotion_list = new List<string>();
            //字符串解包，格式化
            foreach(string s in emotion_list_orig)
            {
                var temps = s.Split('/');
                foreach(string v in temps)
                {
                    emotion_list.Add(v.Split(":")[0]);
                }
            }
            //使用LINQ查询
            //按次数分组
            var query = emotion_list.GroupBy(x => x)
            .OrderByDescending(g => g.Count())
            .Select(g => new { g.Key, Count = g.Count() })
            .ToList();
            DTO.emotion_mode = query.Take(3).ToList();
            //黑眼圈与皮肤健康比率
            var skin_status_orig = BaseData.Select(s => s.skinstatus).ToList();
            var black_eye_list = new List<float>();
            var health_list = new List<float>();
            foreach(string s in skin_status_orig)
            {
                black_eye_list.Add(float.Parse(s.Split("/")[3]));
                health_list.Add(float.Parse(s.Split("/")[0]));
            }
            //线性归一化处理
            for(int i=0;i<black_eye_list.Count();i++)
            {
                black_eye_list[i] = (black_eye_list[i] - black_eye_list.Min()) / (black_eye_list.Max() - black_eye_list.Min());
            }
            for(int i = 0; i < health_list.Count(); i++)
            {
                health_list[i] = (health_list[i] - health_list.Min()) / (health_list.Max() - health_list.Min());
            }
            //健康度分级
            DTO.skin_health_ratio = new
            {
                healthy = new
                {
                    ratio = (float)health_list.Count(x => x >= 0.7) / health_list.Count()
                },
                basically_healthy = new
                {
                    ratio = (float)health_list.Count(x => x >= 0.3 && x < 0.7) / health_list.Count()
                },
                unhealthy = new
                {
                    ratio = (float)health_list.Count(x => x < 0.3) / health_list.Count()
                }
            };
            DTO.black_eye_ratio = new
            {
                healthy = new
                {
                    ratio = (float)black_eye_list.Count(x => x >= 0.7) / black_eye_list.Count()
                },
                basically_healthy = new
                {
                    ratio = (float)black_eye_list.Count(x => x >= 0.3 && x < 0.7) / black_eye_list.Count()
                },
                unhealthy = new
                {
                    ratio = (float)black_eye_list.Count(x => x < 0.3) / black_eye_list.Count()
                }
            };

            return DTO;
        }
        private bool UserFaceExists(int id)
        {
            return _context.UserFace.Any(e => e.id == id);
        }
    }
}
