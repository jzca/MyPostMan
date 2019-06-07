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

        [HttpGet]
        public ActionResult IndexHh()
        {
            var response = RequestHelper.SendGetRequestAuthGetDel("Household"
                , "GetByUserId", null, MyToken, CusHttpMethod.Get);


            return ResDealerIndexHhBa(response, true);
        }

        [HttpGet]
        public ActionResult IndexBa()
        {
            var response = RequestHelper.SendGetRequestAuthGetDel("BankAccount"
                , "GetAllByUserId", null, MyToken, CusHttpMethod.Get);

            return ResDealerIndexHhBa(response, false);

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
                model.BankAccounts = MakeNewDropDownList(ReadOutputBankAccDropDownList(responseBa));
            }
            else if (responseBa.StatusCode == HttpStatusCode.NotFound)
            {
                AddTempDataMsg(true);
                return View("Error");
            }

            var responseCat = RequestHelper.SendGetRequestAuthGetDel("Category"
                , "GetAllByHhId", id, MyToken, CusHttpMethod.Get);

            if (responseCat.IsSuccessStatusCode)
            {
                model.Categories = MakeNewDropDownList(ReadOutputCategoryDropDownList(responseCat));
                model.DateTransacted = DateTime.Today;
                return View(model);
            }
            else if (responseCat.StatusCode == HttpStatusCode.NotFound)
            {
                AddTempDataMsg(false);
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
                return ToShowMe(bankAccountId);
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
                BadRequestSmall(response);

                var responseBa = RequestHelper.SendGetRequestAuthGetDel("BankAccount"
                    , "GetAllByHhId", id, MyToken, CusHttpMethod.Get);

                if (responseBa.IsSuccessStatusCode)
                {
                    formData.BankAccounts = MakeNewDropDownList(ReadOutputBankAccDropDownList(responseBa));
                }
                else if (responseBa.StatusCode == HttpStatusCode.NotFound)
                {
                    AddTempDataMsg(true);
                    return View("Error");
                }


                var responseCat = RequestHelper.SendGetRequestAuthGetDel("Category"
                    , "GetAllByHhId", id, MyToken, CusHttpMethod.Get);

                if (responseCat.IsSuccessStatusCode)
                {
                    formData.Categories = MakeNewDropDownList(ReadOutputCategoryDropDownList(responseCat));
                    return View(formData);
                }
                else if (responseCat.StatusCode == HttpStatusCode.NotFound)
                {
                    AddTempDataMsg(false);
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
                    transactionModel.Categories = MakeNewDropDownList(ReadOutputCategoryDropDownList(responseCat));
                    return View(transactionModel);
                }
                else if (responseCat.StatusCode == HttpStatusCode.NotFound)
                {
                    AddTempDataMsg(false);
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
                var responseCat = RequestHelper.SendGetRequestAuthGetDel("Category"
                    , "GetAllByHhId", id, MyToken, CusHttpMethod.Get);

                if (responseCat.IsSuccessStatusCode)
                {
                    formData.Categories = MakeNewDropDownList(ReadOutputCategoryDropDownList(responseCat));
                    return View(formData);
                }
                else if (responseCat.StatusCode == HttpStatusCode.NotFound)
                {
                    AddTempDataMsg(false);
                    return View("Error");
                }
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
                return ToShowMe(baId);
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
                BadRequestSmall(response);


                var responseCat = RequestHelper.SendGetRequestAuthGetDel("Category"
                , "GetAllByHhBaId", baId, MyToken, CusHttpMethod.Get);

                if (responseCat.IsSuccessStatusCode)
                {
                    formData.Categories = MakeNewDropDownList(ReadOutputCategoryDropDownList(responseCat));
                    return View(formData);
                }
                else if (responseCat.StatusCode == HttpStatusCode.NotFound)
                {
                    AddTempDataMsg(false);
                    return View("Error");
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

            return ResDealerDelVoid(response, baId, false);

        }

        [HttpPost]
        public ActionResult Void(int id, int baId)
        {

            var response = RequestHelper.SendGetRequestAuthGetDel("Transaction"
                , "Void", id, MyToken, CusHttpMethod.Post);

            return ResDealerDelVoid(response, baId, true);

        }

        private enum MyResDealer
        {
            regSuccess, dealBaViewModel, aList, notFound, returnBaViewModel, noAuth, badResquest, empty, single
        }

        private ActionResult ResDealerDelVoid(HttpResponseMessage response, int baId, bool forVoid)
        {
            if (response.IsSuccessStatusCode)
            {
                return ToShowMe(baId);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Nf = true;
                return View("NofoundAuth");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Nf = false;
                return View("NofoundAuth");
            }
            else if (forVoid && response.StatusCode == HttpStatusCode.BadRequest)
            {
                TempData["Message"] = " The Transcation is already void.";
                return View("Error");
            }
            else
            {
                return View("Error");
            }
        }

        private ActionResult ToShowMe(int baId)
        {
            return RedirectToAction(nameof(TransactionController.ShowMine), new { id = baId });
        }

        private List<DropDownListTranscationBindingModel> ReadOutputBankAccDropDownList(HttpResponseMessage responseBa)
        {
            var data = RequestHelper.ReadResponse(responseBa);
            return JsonConvert
                .DeserializeObject<List<DropDownListTranscationBindingModel>>(data);
        }

        private List<DropDownListTranscationBindingModel> ReadOutputCategoryDropDownList(HttpResponseMessage responseCat)
        {
            var data = RequestHelper.ReadResponse(responseCat);
            return JsonConvert
                .DeserializeObject<List<DropDownListTranscationBindingModel>>(data);
        }

        private void AddTempDataMsg(bool isBa)
        {
            if (isBa)
            {
                TempData["Message"] =
               "There are no Bank Accounts, therefore you cannot create/edit a transcation";
            }
            else
            {
                TempData["Message"] =
                "There are no Categories, therefore you cannot create/edit a transcation";
            }

        }

        private void BadRequestSmall(HttpResponseMessage response)
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

        private SelectList MakeNewDropDownList(List<DropDownListTranscationBindingModel> dropDownLists)
        {
            return new SelectList(dropDownLists, "Id", "Name");
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

        private ActionResult ResDealerIndexHhBa(HttpResponseMessage response, bool isHh)
        {
            if (response.IsSuccessStatusCode)
            {
                var data = RequestHelper.ReadResponse(response);

                if (isHh)
                {
                    var model = JsonConvert.DeserializeObject<List<MyHouseholdViewModel>>(data);
                    return View(model);
                }
                else
                {
                    var model = JsonConvert.DeserializeObject<List<BankAccountViewModel>>(data);
                    return View(model);
                }

            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                if (isHh)
                {
                    var model = new List<MyHouseholdViewModel>();
                    return View(model);
                }
                else
                {
                    var model = new List<BankAccountViewModel>();
                    return View(model);
                }

            }
            else
            {
                return View("Error");
            }
        }



    }
}