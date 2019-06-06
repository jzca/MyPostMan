using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyPostMan.Models.ViewModel
{
    public class BankAccountHouseholdViewModel
    {
        public string Name { get; set; }
        public decimal TotalBalance { get; set; }
        public List<BigEaBankAccDetailViewModel> BigEaBankAccDetail { get; set; }
    }
}