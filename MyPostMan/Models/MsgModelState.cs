using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyPostMan.Models
{
    public class MsgModelState
    {
        public string Message { get; set; }
        public Dictionary<string, string[]> ModelState { get; set; }
    }
}