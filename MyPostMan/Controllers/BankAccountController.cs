using MyPostMan.Models;
using MyPostMan.Models.BindingModel;
using MyPostMan.Models.ViewModel;
using MyPostMan.Models.Helper;
using MyPostMan.Models.Filter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MyPostMan.Controllers
{
    [AuthorizeFilter]
    public class BankAccountController : Controller
    {
        private readonly RequestHelper RequestHelper;
        private string MyToken { get { return Request.Cookies["PostManCookie"]?.Value; } }

        public BankAccountController()
        {
            RequestHelper = new RequestHelper();
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

        private enum MyResDealer
        {
            regSuccess, dealBaViewModel ,aList, notFound, returnBaViewModel, noAuth, badResquest, empty, single
        }

        private ActionResult GeneralResDealer(HttpResponseMessage response, MyResDealer regSuccess, MyResDealer dealBaViewModel, MyResDealer isList,  int? hhId, MyResDealer notFound, MyResDealer returnBaViewModel, MyResDealer noAuth, MyResDealer badResquest)
        {
            if (regSuccess== MyResDealer.regSuccess && response.IsSuccessStatusCode)
            {
                if (dealBaViewModel == MyResDealer.dealBaViewModel)
                {
                    var data = RequestHelper.ReadResponse(response);

                    if(isList == MyResDealer.aList)
                    {
                        var listModel = JsonConvert.DeserializeObject<List<BankAccountViewModel>>(data);
                        return View(listModel);
                    }
                    else
                    {
                        var oneModel = JsonConvert.DeserializeObject<BankAccountBindingModel>(data);
                        return View(oneModel);
                    }

                }
                else
                {
                    if (hhId.HasValue)
                    {
                    return RedirectToAction(nameof(BankAccountController.ShowMine), "BankAccount", new { id = hhId.Value });
                    }
                    else
                    {
                        return View("Error");
                    }
                }
            }
            else if (notFound== MyResDealer.notFound && response.StatusCode == HttpStatusCode.NotFound)
            {
                if (returnBaViewModel == MyResDealer.returnBaViewModel)
                {
                    var model = new List<BankAccountViewModel>();

                    return View(model);
                }
                else
                {
                    ViewBag.Nf = true;
                    return View("NofoundAuth");
                }
            }
            else if (noAuth == MyResDealer.noAuth && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Nf = false;
                return View("NofoundAuth");
            }
            else if (badResquest == MyResDealer.badResquest)
            {
                DealBadRequest(response);
                return View();
            }
            else
            {
                return View("Erro");
            }
        }

        [HttpGet]
        public ActionResult IndexHh()
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

            return View();
        }

        [HttpGet]
        public ActionResult ShowMine(int? id)
        {

            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BankAccountController.IndexHh));
            }

            var response = RequestHelper.SendGetRequestAuthGetDel("BankAccount"
            , "GetCreatedByHhId", id, MyToken, CusHttpMethod.Get);

            return GeneralResDealer(response, MyResDealer.regSuccess, MyResDealer.dealBaViewModel, MyResDealer.aList, null,
                MyResDealer.notFound, MyResDealer.returnBaViewModel, MyResDealer.empty, MyResDealer.empty);
        }

        [HttpGet]
        public ActionResult ShowAll(int? id)
        {

            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BankAccountController.IndexHh));
            }

            var response = RequestHelper.SendGetRequestAuthGetDel("BankAccount"
              , "GetAllByHhId", id, MyToken, CusHttpMethod.Get);

            return GeneralResDealer(response, MyResDealer.regSuccess, MyResDealer.dealBaViewModel, MyResDealer.aList, null,
                MyResDealer.notFound, MyResDealer.returnBaViewModel, MyResDealer.empty, MyResDealer.empty);

        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(int id, BankAccountBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var name = formData.Name;
            var description = formData.Description;
            int householdId = id;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("name", name));
            parameters.Add(new KeyValuePair<string, string>("description", description));
            parameters.Add(new KeyValuePair<string, string>("householdId", householdId.ToString()));

            var response = RequestHelper.SendGetRequestAuth(parameters, "BankAccount"
                , "Create", null, MyToken, CusHttpMethod.Post);

            return GeneralResDealer(response, MyResDealer.regSuccess, MyResDealer.empty, MyResDealer.single, id,
                MyResDealer.notFound, MyResDealer.empty, MyResDealer.noAuth, MyResDealer.badResquest);

        }

        [HttpGet]
        public ActionResult Edit(int? id, int? hhId)
        {
            if (!id.HasValue || !hhId.HasValue)
            {
                return RedirectToAction(nameof(BankAccountController.IndexHh));
            }

            var response = RequestHelper.SendGetRequestAuthGetDel("BankAccount"
            , "GetByBaId", id, MyToken, CusHttpMethod.Get);


            return GeneralResDealer(response, MyResDealer.regSuccess, MyResDealer.dealBaViewModel, MyResDealer.single, null,
                MyResDealer.notFound, MyResDealer.empty, MyResDealer.empty, MyResDealer.empty);
        }

        [HttpPost]
        public ActionResult Edit(int id, int hhId, BankAccountBindingModel formData)
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

            var response = RequestHelper.SendGetRequestAuth(parameters, "BankAccount"
                , "Edit", id, MyToken, CusHttpMethod.Put);

            return GeneralResDealer(response, MyResDealer.regSuccess, MyResDealer.empty, MyResDealer.single, hhId,
                MyResDealer.notFound, MyResDealer.empty, MyResDealer.noAuth, MyResDealer.badResquest);


        }

        [HttpPost]
        public ActionResult Delete(int id, int hhId)
        {

            var response = RequestHelper.SendGetRequestAuthGetDel("BankAccount"
                , "Delete", id, MyToken, CusHttpMethod.Delete);

            return GeneralResDealer(response, MyResDealer.regSuccess, MyResDealer.empty, MyResDealer.single, hhId,
                MyResDealer.notFound, MyResDealer.empty, MyResDealer.noAuth, MyResDealer.empty);
        }

        [HttpPost]
        public ActionResult UpdateBalance(int id, int hhId)
        {

            var response = RequestHelper.SendGetRequestAuthGetDel("BankAccount"
                , "UpdateBalance", id, MyToken, CusHttpMethod.Put);

            return GeneralResDealer(response, MyResDealer.regSuccess, MyResDealer.empty, MyResDealer.empty, hhId,
                MyResDealer.notFound, MyResDealer.empty, MyResDealer.noAuth, MyResDealer.empty);

        }


    }
}