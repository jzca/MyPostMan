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
    public class TransactionController : Controller
    {
        private readonly RequestHelper RequestHelper;
        private string MyToken { get { return Request.Cookies["PostManCookie"]?.Value; } }

        public TransactionController()
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
        public ActionResult IndexBa()
        {
            var response = RequestHelper.SendGetRequestAuthGetDel("BankAccount"
                , "GetAllByUserId", null, MyToken, CusHttpMethod.Get);


            if (response.IsSuccessStatusCode)
            {
                var data = RequestHelper.ReadResponse(response);
                var model = JsonConvert.DeserializeObject<List<BankAccountViewModel>>(data);
                return View(model);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var model = new List<BankAccountViewModel>();

                return View(model);
            }

            return View();
        }

        [HttpGet]
        public ActionResult ShowMine(int? id)
        {

            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TransactionController.IndexBa));
            }

            var response = RequestHelper.SendGetRequestAuthGetDel("Transaction"
              , "GetAllByBaId", id, MyToken, CusHttpMethod.Get);


            if (response.IsSuccessStatusCode)
            {
                var data = RequestHelper.ReadResponse(response);
                var model = JsonConvert.DeserializeObject<List<TranscationDetailViewModel>>(data);
                return View(model);
            }


            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var model = new List<TranscationDetailViewModel>();

                return View(model);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Nf = false;
                return View("NofoundAuth");
            }



            return View("Error");
        }


        [HttpGet]
        public ActionResult Create(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TransactionController.IndexHh));
            }

            var model = new TranscationBindingModel();

            var responseBa = RequestHelper.SendGetRequestAuthGetDel("BankAccount"
                , "GetAllByHhId", id, MyToken, CusHttpMethod.Get);

            if (responseBa.IsSuccessStatusCode)
            {
                var data = RequestHelper.ReadResponse(responseBa);
                var bankAccounts = JsonConvert
                    .DeserializeObject<List<DropDownListTranscationBindingModel>>(data);
                model.BankAccounts = new SelectList(bankAccounts, "Id", "Name");
            }
            else if (responseBa.StatusCode == HttpStatusCode.NotFound)
            {
                TempData["Message"] =
                "There are no Bank Accounts, therefore you cannot create a transcation";
                return View("Error");
            }

            var responseCat = RequestHelper.SendGetRequestAuthGetDel("Category"
                , "GetAllByHhId", id, MyToken, CusHttpMethod.Get);

            if (responseCat.IsSuccessStatusCode)
            {
                var data = RequestHelper.ReadResponse(responseCat);
                var categories = JsonConvert
                    .DeserializeObject<List<DropDownListTranscationBindingModel>>(data);
                model.Categories = new SelectList(categories, "Id", "Name");
                return View(model);
            }
            else if (responseCat.StatusCode == HttpStatusCode.NotFound)
            {
                TempData["Message"] =
                "There are no Categories, therefore you cannot create a transcation";
                return View("Error");
            }

            return View("Error");
        }

        [HttpPost]
        public ActionResult Create(int id, TranscationBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            //if(formData.DateTransacted.ToString().Contains("0001-01-01"))
            //{
            //    TempData["Message"] = "Date Transacted is not valid";
            //    return View("Error");
            //}

            var title = formData.Title;
            var description = formData.Description;
            int categoryId = formData.CategoryId;
            int bankAccountId = formData.BankAccountId;
            var dateTransacted = formData.DateTransacted;
            decimal amount = formData.Amount;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("title", title));
            parameters.Add(new KeyValuePair<string, string>("description", description));
            parameters.Add(new KeyValuePair<string, string>("amount", amount.ToString()));
            parameters.Add(new KeyValuePair<string, string>("categoryId", categoryId.ToString()));
            parameters.Add(new KeyValuePair<string, string>("bankAccountId", bankAccountId.ToString()));
            parameters.Add(new KeyValuePair<string, string>("dateTransacted", dateTransacted.ToString()));

            var response = RequestHelper.SendGetRequestAuth(parameters, "Transaction"
                , "Create", null, MyToken, CusHttpMethod.Post);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(TransactionController.ShowMine), new { bankAccountId });
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Nf = true;
                return View("NofoundAuth");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Nf = false;
                return View("NofoundAuth");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                TempData["Message"] = "Date Transacted maybe not valid";
                return View("Error");
            }
            else if (response.StatusCode != HttpStatusCode.BadRequest)
            {
                return View("Error");
            }

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

                var model = new TranscationBindingModel();

                var responseBa = RequestHelper.SendGetRequestAuthGetDel("BankAccount"
                    , "GetAllByHhId", id, MyToken, CusHttpMethod.Get);

                if (responseBa.IsSuccessStatusCode)
                {
                    var dataBa = RequestHelper.ReadResponse(responseBa);
                    var bankAccounts = JsonConvert
                        .DeserializeObject<List<DropDownListTranscationBindingModel>>(dataBa);
                    model.BankAccounts = new SelectList(bankAccounts, "Id", "Name");
                }
                else if (responseBa.StatusCode == HttpStatusCode.NotFound)
                {
                    TempData["Message"] =
                   "There are no Bank Accounts, therefore you cannot create a transcation";
                    return View("Error");
                }


                var responseCat = RequestHelper.SendGetRequestAuthGetDel("Category"
                    , "GetAllByHhId", id, MyToken, CusHttpMethod.Get);

                if (responseCat.IsSuccessStatusCode)
                {
                    var dataCat = RequestHelper.ReadResponse(responseCat);
                    var categories = JsonConvert
                        .DeserializeObject<List<DropDownListTranscationBindingModel>>(dataCat);
                    model.Categories = new SelectList(categories, "Id", "Name");
                    return View(model);
                }
                else if (responseCat.StatusCode == HttpStatusCode.NotFound)
                {
                    TempData["Message"] =
                    "There are no Categories, therefore you cannot create a transcation";
                    return View("Error");
                }

                return View("Error");

            }

            return View("Error");
        }

        [HttpGet]
        public ActionResult Edit(int? id, int? baId)
        {
            if (!id.HasValue || !baId.HasValue)
            {
                return RedirectToAction(nameof(TransactionController.IndexBa));
            }

            var response = RequestHelper.SendGetRequestAuthGetDel("Transaction"
                , "GetById", id, MyToken, CusHttpMethod.Get);

            var data2 = RequestHelper.ReadResponse(response);

            if (response.IsSuccessStatusCode)
            {
                var dataTrans = RequestHelper.ReadResponse(response);
                var transactionModel = JsonConvert
                    .DeserializeObject<EditTranscationBindingModel>(dataTrans);

                var responseCat = RequestHelper.SendGetRequestAuthGetDel("Category"
                , "GetAllByHhBaId", baId, MyToken, CusHttpMethod.Get);

                if (responseCat.IsSuccessStatusCode)
                {
                    var data = RequestHelper.ReadResponse(responseCat);
                    var categories = JsonConvert
                        .DeserializeObject<List<DropDownListTranscationBindingModel>>(data);
                    transactionModel.Categories = new SelectList(categories, "Id", "Name");
                    return View(transactionModel);
                }
                else if (responseCat.StatusCode == HttpStatusCode.NotFound)
                {
                    TempData["Message"] =
                        "There are no Categories, therefore you cannot edit a transcation";
                    return View("Error");
                }

            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var model = new EditTranscationBindingModel();
                return View(model);
            }


            return View("Error");
        }

        [HttpPost]
        public ActionResult Edit(int id, int baId, EditTranscationBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Date Transacted maybe not valid";
                return View("Error");
            }

            var title = formData.Title;
            var description = formData.Description;
            int categoryId = formData.CategoryId;
            var dateTransacted = formData.DateTransacted;
            decimal amount = formData.Amount;

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("title", title));
            parameters.Add(new KeyValuePair<string, string>("description", description));
            parameters.Add(new KeyValuePair<string, string>("amount", amount.ToString()));
            parameters.Add(new KeyValuePair<string, string>("categoryId", categoryId.ToString()));
            parameters.Add(new KeyValuePair<string, string>("dateTransacted", dateTransacted.ToString()));

            var response = RequestHelper.SendGetRequestAuth(parameters, "Transaction"
                , "Edit", id, MyToken, CusHttpMethod.Put);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(TransactionController.ShowMine), new { id = baId });
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Nf = true;
                return View("NofoundAuth");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Nf = false;
                return View("NofoundAuth");
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                TempData["Message"] = "Date Transacted maybe not valid";
                return View("Error");
            }
            else if (response.StatusCode != HttpStatusCode.BadRequest)
            {
                return View("Error");
            }

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

                var model = new EditTranscationBindingModel();

                var responseCat = RequestHelper.SendGetRequestAuthGetDel("Category"
                , "GetAllByHhBaId", baId, MyToken, CusHttpMethod.Get);

                if (responseCat.IsSuccessStatusCode)
                {
                    var dataCat = RequestHelper.ReadResponse(responseCat);
                    var categories = JsonConvert
                        .DeserializeObject<List<DropDownListTranscationBindingModel>>(dataCat);
                    model.Categories = new SelectList(categories, "Id", "Name");
                    return View(model);
                }
                else if (responseCat.StatusCode == HttpStatusCode.NotFound)
                {
                    return View(model);
                }

                return View("Error");

            }

            return View("Error");


        }

        [HttpPost]
        public ActionResult Delete(int id, int baId)
        {

            var response = RequestHelper.SendGetRequestAuthGetDel("Transaction"
                , "Delete", id, MyToken, CusHttpMethod.Delete);


            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(TransactionController.ShowMine), new { id = baId });
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

            return View("Error");
        }

        [HttpPost]
        public ActionResult Void(int id, int baId)
        {

            var response = RequestHelper.SendGetRequestAuthGetDel("Transaction"
                , "Void", id, MyToken, CusHttpMethod.Post);


            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(TransactionController.ShowMine), new { id = baId });
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

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                TempData["Message"] = " The Transcation is already void.";
                return View("Error");
            }

            return View("Error");
        }


    }
}