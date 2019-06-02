using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MyPostMan.Models;
using MyPostMan.Controllers;
using MyPostMan.Models.Helper;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyPostMan.Controllers
{
    public class AccountController : Controller
    {
        private readonly RequestHelper RequestHelper;

        public AccountController()
        {
            RequestHelper = new RequestHelper();
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var email = formData.Email;
            var password = formData.Password;
            var confirmPassword = formData.ConfirmPassword;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("email", email));
            parameters.Add(new KeyValuePair<string, string>("password", password));
            parameters.Add(new KeyValuePair<string, string>("confirmPassword", confirmPassword));

            var response = RequestHelper.SendGetRequest(parameters, "Account"
                , nameof(AccountController.Register), null);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Account");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }

            DealBadRequest(response);

            return View();
        }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userName = formData.Username;
            var password = formData.Password;
            var grantType = "password";

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("username", userName));
            parameters.Add(new KeyValuePair<string, string>("password", password));
            parameters.Add(new KeyValuePair<string, string>("grant_type", grantType));

            var response = RequestHelper.SendGetRequest(parameters, "token"
             , "token", null);

            if (response.IsSuccessStatusCode)
            {
                var data = RequestHelper.ReadResponse(response);
                var result = JsonConvert.DeserializeObject<LogInData>(data);
                var cookie = new HttpCookie("PostManCookie",
                    result.AccessToken);

                Response.Cookies.Add(cookie);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = RequestHelper.ReadResponse(response);
                var erroMsg = JsonConvert.DeserializeObject<BadError>(data);
                ModelState.AddModelError(erroMsg.Name, erroMsg.Message);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }

            return View();

        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            IsAuthenticated(nameof(AccountController.Login));

            return View();

        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordBindingModel formData)
        {
            IsAuthenticated(nameof(AccountController.Login));

            var myToken = GetToken();

            if (!ModelState.IsValid)
            {
                return View();
            }

            var oldPassword = formData.OldPassword;
            var newPassword = formData.NewPassword;
            var confirmPassword = formData.ConfirmPassword;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("oldPassword", oldPassword));
            parameters.Add(new KeyValuePair<string, string>("newPassword", newPassword));
            parameters.Add(new KeyValuePair<string, string>("confirmPassword", confirmPassword));

            var response = RequestHelper.SendGetRequestAuth(parameters, "Account"
             , nameof(AccountController.ChangePassword), null, myToken, CusHttpMethod.Post);

            if (response.IsSuccessStatusCode)
            {
                return View("ChangePasswordConfirmation");
            }

            DealBadRequest(response);

            return View();
        }


        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var email = formData.Email;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("email", email));
            var response = RequestHelper.SendGetRequest(parameters, "Account"
             , nameof(AccountController.ForgotPassword), null);

            if (response.IsSuccessStatusCode)
            {
                return View("ForgotPasswordConfirmation");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = RequestHelper.ReadResponse(response);
                var erroMsg = JsonConvert.DeserializeObject<BadError>(data);
                ModelState.AddModelError(erroMsg.Name, erroMsg.Message);
            }

            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(ResetPasswordBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var email = formData.Email;
            var password = formData.Password;
            var confirmPassword = formData.ConfirmPassword;
            var code = formData.Code;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("email", email));
            parameters.Add(new KeyValuePair<string, string>("password", password));
            parameters.Add(new KeyValuePair<string, string>("confirmPassword", confirmPassword));
            parameters.Add(new KeyValuePair<string, string>("code", code));
            var response = RequestHelper.SendGetRequest(parameters, "Account"
             , nameof(AccountController.ResetPassword), null);

            if (response.IsSuccessStatusCode)
            {
                return View("ResetPasswordConfirmation");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return View("Error");
            }

            DealBadRequest(response);

            return View();
        }

        [HttpPost]
        public ActionResult LogOut()
        {
            if (RequestHelper.HasCookie(Request))
            {
                DateTime yesterday = DateTime.Today.AddDays(-1);
                Response.Cookies["PostManCookie"]
                     .Expires = yesterday;
            }

            return RedirectToAction(nameof(AccountController.Login));

        }

        private ActionResult IsAuthenticated(string where)
        {
            if (RequestHelper.HasCookie(Request))
            {
                return View();
            }
            else
            {
                return RedirectToAction(where);
            }

        }

        private string GetToken()
        {
            return Request.Cookies["PostManCookie"].Value;
        }

        private void DealBadRequest(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var data = RequestHelper.ReadResponse(response);
                var erroMsg = JsonConvert.DeserializeObject<MsgModelState>(data);
                erroMsg.ModelState.ToList().ForEach(p =>
                {
                    p.Value.ToList().ForEach(b =>
                    {
                        ModelState.AddModelError(string.Empty, $"{b}");
                    });

                });
            }
        }





    }
}