using MyPostMan.Models.BindingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyPostMan.Models
{
    public class ResDealer
    {
        public ActionResult RedirectedView { get; set; }
        public bool VBagNf { get; set; }
        public HouseholdBindingModel HouseholdBinding { get; set; }
    }
}