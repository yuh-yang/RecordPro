using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecordPRO.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;

namespace RecordPRO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RecordPROContext _context;

        public UsersController(RecordPROContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 用户注册，若成功返回用户信息携带token，若用户名重复，返回409
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost("/register")]
        public IActionResult Register(string username, string password)
        {
            if (UserExists(username)){
                return Conflict();
            }else
            {
                //先存，拿到自增的id
                User user = new User();
                user.Name = username;
                user.Password = password;
                _context.Users.Add(user);
                _context.SaveChanges();
                var user_new = _context.Users.Single(u=>u.Name==username);
                //将id写到JWT中，二次存储
                var payload = new Dictionary<string, object>
               {
                    {"iss", "RecordProAPI" },
                    {"iat", DateTime.Now.ToString() },
                    {"name", username },
                    {"id", user_new.id}
               };
                const string secret = "ezio0124";
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
                var token = encoder.Encode(payload, secret);
                //刷新token
                user_new.Token = token;
                _context.Update<User>(user_new);
                _context.SaveChanges();
                return CreatedAtAction("Register", new { id = user.id}, user);
            }
        }

        /// <summary>
        /// 用户登录，成功返回用户信息，用户不存在返回404。密码错误返回417
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("/login")]
        public ActionResult<User> Login(string username, string password)
        {
            if (!UserExists(username))
            {
                return NotFound();
            }
            else
            {
                var user = _context.Users.Single(u => u.Name == username);
                if (user.Password.Equals(password))
                {
                    return user;
                }
                else
                {
                    return StatusCode(417);
                }
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.id == id);
        }

        private bool UserExists(string username)
        {
            return _context.Users.Any(e => e.Name == username);
        }
    }
}
