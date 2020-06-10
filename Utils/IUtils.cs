using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.Utils
{
    public interface IUtils
    {
        public string VerifyRequest(string token);
        public Baidu.Aip.Nlp.Nlp GetBaiduClient();

        public float Median(float[] arr);
    }
}
