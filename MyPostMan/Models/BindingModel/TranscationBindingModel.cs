using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyPostMan.Models.BindingModel
{
    public class TranscationBindingModel
    {
        public TranscationBindingModel()
        {
            DateTransacted = DateTime.Today;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTransacted { get; set; }
        public SelectList Categories { get; set; }
        public int CategoryId { get; set; }
        public SelectList BankAccounts { get; set; }
        public int BankAccountId { get; set; }

    }
}