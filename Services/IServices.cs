using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordPRO.Services
{
    public interface IServices
    {
        public string ExtractKeywords(string content);

        public JObject DetectFace(string filepath);
    }
}
