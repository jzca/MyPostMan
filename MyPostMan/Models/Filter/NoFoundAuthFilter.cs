using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyPostMan.Models.Filter
{
    public class NoFoundAuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var resCode = filterContext.HttpContext.Response.StatusCode;

            if (resCode == Convert.ToInt32(HttpStatusCode.NotFound)
                || resCode == Convert.ToInt32(HttpStatusCode.Unauthorized))
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "NofoundAuthGeneral"
                };
            }
        }
    }
}