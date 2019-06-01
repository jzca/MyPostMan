using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyPostMan.Models
{
    public class LogInData
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiredCode { get; set; }
        //[JsonProperty("issued")]
        //public DateTime IssuedDate { get; set; }
        //[JsonProperty("expires")]
        //public DateTime ExpiredDate { get;set; }
    }
}