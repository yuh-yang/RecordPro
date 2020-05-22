using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RecordPRO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.Utils
{
    public class UtilsImpl : IUtils
    {

        public string VerifyRequest(string token)
        {
            //检查token是否存在
            if(token is null)
            {
                return null;
            }
            string json;
            try
            {
                json = new JwtBuilder()
                  .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                  .WithSecret("ezio0124")
                  .MustVerifySignature()
                  .Decode(token);
            }
            catch (Exception)
            {
                return null;
            }

            Dictionary<string, string> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return jsonDict["id"];
        }

        public Baidu.Aip.Nlp.Nlp GetBaiduClient()
        {
            // 设置APPID/AK/SK
            var APP_ID = "20015213";
            var API_KEY = "gWdrA6pI4XlGZ1wvDLZtmo8l";
            var SECRET_KEY = "h3ehwAL57zAV6egQGYfr6qLG6LVdgvRa";

            var client = new Baidu.Aip.Nlp.Nlp(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间

            return client;
        }
    }
}
