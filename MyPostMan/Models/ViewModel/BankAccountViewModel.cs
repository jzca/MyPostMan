﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyPostMan.Models.ViewModel
{
    public class BankAccountViewModel
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public decimal Balance { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}