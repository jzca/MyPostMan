using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyPostMan.Models
{
    public class BadError
    {
        [JsonProperty("error")]
        public string Name { get; set; }

        [JsonProperty("error_description")]
        public string Message { get; set; }
    }
}