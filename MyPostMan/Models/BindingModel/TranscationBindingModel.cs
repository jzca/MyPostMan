using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyPostMan.Models.BindingModel
{
    public class TranscationBindingModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTransacted { get; set; }
        public int CategoryId { get; set; }
    }
}