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
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace RecordPRO.Utils
{
    public class UtilsImpl : IUtils
    {

        public string VerifyRequest(string token)
        {
            //检查token是否存在
            if (token is null)
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

        public float Median(float[] arr)
        {
            //为了不修改arr值，对数组的计算和修改在tempArr数组中进行
            float[] tempArr = new float[arr.Length];
            arr.CopyTo(tempArr, 0);

            //对数组进行排序
            float temp;
            for (int i = 0; i < tempArr.Length; i++)
            {
                for (int j = i; j < tempArr.Length; j++)
                {
                    if (tempArr[i] > tempArr[j])
                    {
                        temp = tempArr[i];
                        tempArr[i] = tempArr[j];
                        tempArr[j] = temp;
                    }
                }
            }
            //针对数组元素的奇偶分类讨论
            if (tempArr.Length % 2 != 0)
            {
                return (float)Math.Round(tempArr[arr.Length / 2 + 1], 3);
            }
            else
            {
                return (float)Math.Round((tempArr[tempArr.Length / 2] +
                    tempArr[tempArr.Length / 2 + 1]) / 2, 3);
            }

        }
    }
}
