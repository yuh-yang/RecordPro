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
    public class RequestVerification : IUtils
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
    }
}
