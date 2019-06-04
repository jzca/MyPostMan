using MyPostMan.Models;
using MyPostMan.Models.BindingModel;
using MyPostMan.Models.ViewModel;
using MyPostMan.Models.Helper;
using MyPostMan.Models.Filter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net;

namespace MyPostMan.Controllers
{
    [AuthorizeFilter]
    public class HouseholdController : Controller
    {
        private readonly RequestHelper RequestHelper;
        private string MyToken { get { return Request.Cookies["PostManCookie"]?.Value; } }

        public HouseholdController()
        {
            RequestHelper = new RequestHelper();
        }

        [HttpGet]
        public ActionResult Index()
        {
            var response = RequestHelper.SendGetRequestAuthGetDel("Household"
                , "GetByUserId", null, MyToken, CusHttpMethod.Get);


            if (response.IsSuccessStatusCode)
            {
                var data = RequestHelper.ReadResponse(response);
                var model = JsonConvert.DeserializeObject<List<MyHouseholdViewModel>>(data);
                return View(model);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var model = new List<MyHouseholdViewModel>();

                return View(model);
            }


            return View("Erro");


        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(HouseholdBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var name = formData.Name;
            var description = formData.Description;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("name", name));
            parameters.Add(new KeyValuePair<string, string>("description", description));

            var response = RequestHelper.SendGetRequestAuth(parameters, "Household"
                , "Create", null, MyToken, CusHttpMethod.Post);

            //var data = RequestHelper.ReadResponse(response);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(HouseholdController.Index));
            }

            DealBadRequest(response);

            return View();


        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HouseholdController.Index));
            }

            var response = RequestHelper.SendGetRequestAuthGetDel("Household"
            , "GetOwnedById", id, MyToken, CusHttpMethod.Get);


            if (response.IsSuccessStatusCode)
            {
                var data = RequestHelper.ReadResponse(response);
                var model = JsonConvert.DeserializeObject<HouseholdBindingModel>(data);
                return View(model);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Nf = true;
                return View("NofoundAuth");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Nf = false;
                return View("NofoundAuth");
            }

            return View("Erro");
        }

        [HttpPost]
        public ActionResult Edit(int id, HouseholdBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var name = formData.Name;
            var description = formData.Description;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("name", name));
            parameters.Add(new KeyValuePair<string, string>("description", description));

            var response = RequestHelper.SendGetRequestAuth(parameters, "Household"
                , "Edit", id, MyToken, CusHttpMethod.Put);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(HouseholdController.Index));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Nf = true;
                return View("NofoundAuth");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Nf = false;
                return View("NofoundAuth");
            }

            DealBadRequest(response);

            return View();


        }

        [HttpGet]
        public ActionResult Invite(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HouseholdController.Index));
            }

            var model = new InviteUserBindingModel();

            return View();
        }

        [HttpPost]
        public ActionResult Invite(int id, InviteUserBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var email = formData.Email;
            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("email", email));
            var response = RequestHelper.SendGetRequestAuth(parameters, "Household"
                , "InviteUserByHhIdEmail", id, MyToken, CusHttpMethod.Post);


            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(HouseholdController.Index));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Nf = true;
                return View("NofoundAuth");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Nf = false;
                return View("NofoundAuth");
            }

            DealBadRequest(response);

            return View();


        }

        [HttpGet]
        public ActionResult Join()
        {
            var response = RequestHelper.SendGetRequestAuthGetDel("Household"
                , "GetByInvitedUser", null, MyToken, CusHttpMethod.Get);


            if (response.IsSuccessStatusCode)
            {
                var data = RequestHelper.ReadResponse(response);
                var model = JsonConvert.DeserializeObject<List<HouseholdViewModel>>(data);
                return View(model);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var model = new List<HouseholdViewModel>();

                return View(model);
            }

            return View("Erro");
        }

        [HttpPost]
        public ActionResult Join(int id)
        {

            var parameters = new List<KeyValuePair<string, string>>();
            var response = RequestHelper.SendGetRequestAuth(parameters, "Household"
                , "JoinHouseholdById", id, MyToken, CusHttpMethod.Post);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(HouseholdController.Index));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Nf = true;
                return View("NofoundAuth");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Nf = false;
                return View("NofoundAuth");
            }

            DealBadRequest(response);

            return View();


        }

        [HttpGet]
        public ActionResult ShowUsers(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HouseholdController.Index));
            }

            var response = RequestHelper.SendGetRequestAuthGetDel("Household"
                , "GetUsersByHhId", id, MyToken, CusHttpMethod.Get);

            if (response.IsSuccessStatusCode)
            {
                var data = RequestHelper.ReadResponse(response);
                var model = JsonConvert.DeserializeObject<List<ShowUsersViewModel>>(data);
                return View(model);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Nf = true;
                return View("NofoundAuth");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Nf = false;
                return View("NofoundAuth");
            }

            return View();


        }

        [HttpPost]
        public ActionResult Leave(int id)
        {

            var parameters = new List<KeyValuePair<string, string>>();
            var response = RequestHelper.SendGetRequestAuth(parameters, "Household"
                , "Leave", id, MyToken, CusHttpMethod.Post);


            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(HouseholdController.Index));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Nf = true;
                return View("NofoundAuth");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Nf = false;
                return View("NofoundAuth");
            }

            return View("Erro");
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {

            var response = RequestHelper.SendGetRequestAuthGetDel("Household"
                , "Delete", id, MyToken, CusHttpMethod.Delete);


            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(HouseholdController.Index));
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Nf = true;
                return View("NofoundAuth");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Nf = false;
                return View("NofoundAuth");
            }

            return View("Erro");
        }

        private void DealBadRequest(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
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