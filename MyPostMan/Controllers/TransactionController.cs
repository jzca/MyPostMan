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
            var response = SendReqGetRes(false, null, "Household", "GetByUserId");

            return ResDealerIndexHhBa(response, true);
        }

        [HttpGet]
        public ActionResult IndexBa()
        {
            var response = SendReqGetRes(false, null, "BankAccount", "GetAllByUserId");

            return ResDealerIndexHhBa(response, false);
        }

        [HttpGet]
        public ActionResult ShowMine(int? id)
        {

            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TransactionController.IndexBa));
            }

            var response = SendReqGetRes(true, id, "Transaction", "GetAllByBaId");

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
                return NotFoundOrUnauthorized(false);
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

            return FillDropDownListOnCreation(id.Value);

        }

        [HttpPost]
        public ActionResult Create(int id, TranscationBindingModel formData)
        {
            if (!ModelState.IsValid)
            {
                return FillDropDownListOnCreation(id);
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
                return NotFoundOrUnauthorized(true);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return NotFoundOrUnauthorized(false);
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                CusAddTempDataMsg("Date Transacted maybe not valid");
                return View("Error");
            }
            else if (response.StatusCode != HttpStatusCode.BadRequest)
            {
                return View("Error");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                BadRequestSmall(response);

                var responseBa = SendReqGetResDropDownList(true, id);

                if (responseBa.IsSuccessStatusCode)
                {
                    formData.BankAccounts = MakeNewDropDownList(ReadOutputDropDownList(responseBa));
                }
                else if (responseBa.StatusCode == HttpStatusCode.NotFound)
                {
                    AddTempDataMsg(true);
                    return View("Error");
                }


                var responseCat = SendReqGetResDropDownList(false, id);

                if (responseCat.IsSuccessStatusCode)
                {
                    formData.Categories = MakeNewDropDownList(ReadOutputDropDownList(responseCat));
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

            var response = SendReqGetRes(true, id, "Transaction", "GetById");

            if (response.IsSuccessStatusCode)
            {
                var dataTrans = RequestHelper.ReadResponse(response);
                var transactionModel = JsonConvert
                    .DeserializeObject<EditTranscationBindingModel>(dataTrans);

                var responseCat = SendReqGetRes(true, baId, "Category", "GetAllByHhBaId");

                if (responseCat.IsSuccessStatusCode)
                {
                    transactionModel.Categories = MakeNewDropDownList(ReadOutputDropDownList(responseCat));
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
                var responseCat = SendReqGetResCatDropDownListByBa(baId);

                if (responseCat.IsSuccessStatusCode)
                {
                    formData.Categories = MakeNewDropDownList(ReadOutputDropDownList(responseCat));
                    formData.DateTransacted = DateTime.Today;
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
                CusAddTempDataMsg("Date Transacted maybe not valid");
                return View("Error");
            }
            else if (response.StatusCode != HttpStatusCode.BadRequest)
            {
                return View("Error");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                BadRequestSmall(response);

                var responseCat = SendReqGetResCatDropDownListByBa(baId);

                if (responseCat.IsSuccessStatusCode)
                {
                    formData.Categories = MakeNewDropDownList(ReadOutputDropDownList(responseCat));
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


        private ActionResult ResDealerDelVoid(HttpResponseMessage response, int baId, bool forVoid)
        {
            if (response.IsSuccessStatusCode)
            {
                return ToShowMe(baId);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFoundOrUnauthorized(true);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return NotFoundOrUnauthorized(false);
            }
            else if (forVoid && response.StatusCode == HttpStatusCode.BadRequest)
            {
                CusAddTempDataMsg("The Transcation is already void.");
                return View("Error");
            }
            else
            {
                return View("Error");
            }
        }

        private ActionResult NotFoundOrUnauthorized(bool isNf)
        {
            ViewBag.Nf = isNf;
            return View("NofoundAuth");
        }


        private ActionResult ToShowMe(int baId)
        {
            return RedirectToAction(nameof(TransactionController.ShowMine), new { id = baId });
        }

        private List<DropDownListTranscationBindingModel> ReadOutputDropDownList(HttpResponseMessage resp)
        {
            var data = RequestHelper.ReadResponse(resp);
            return JsonConvert
                .DeserializeObject<List<DropDownListTranscationBindingModel>>(data);
        }

        private void CusAddTempDataMsg(string msg)
        {
            TempData["Message"] = msg;
        }

        private void AddTempDataMsg(bool isBa)
        {
            if (isBa)
            {
                CusAddTempDataMsg(
               "There are no Bank Accounts, therefore you cannot create/edit a transcation");
            }
            else
            {
                CusAddTempDataMsg(
                "There are no Categories, therefore you cannot create/edit a transcation");
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

        private HttpResponseMessage SendReqGetRes(bool hasId, int? inputId, string controller, string action)
        {
            if (hasId && inputId.HasValue)
            {
                return RequestHelper.SendGetRequestAuthGetDel(controller
                , action, inputId.Value, MyToken, CusHttpMethod.Get);

            }
            else
            {
                return RequestHelper.SendGetRequestAuthGetDel(controller
                , action, null, MyToken, CusHttpMethod.Get);
            }
        }

        private HttpResponseMessage SendReqGetResDropDownList(bool isBa, int inputId)
        {
            if (isBa)
            {
                return RequestHelper.SendGetRequestAuthGetDel("BankAccount"
                    , "GetAllByHhId", inputId, MyToken, CusHttpMethod.Get);

            }
            else
            {
                return RequestHelper.SendGetRequestAuthGetDel("Category"
                    , "GetAllByHhId", inputId, MyToken, CusHttpMethod.Get);
            }
        }

        private HttpResponseMessage SendReqGetResCatDropDownListByBa(int baId)
        {
            return RequestHelper.SendGetRequestAuthGetDel("Category"
                , "GetAllByHhBaId", baId, MyToken, CusHttpMethod.Get);
        }

        private ActionResult FillDropDownListOnCreation(int id)
        {
            var model = new TranscationBindingModel();

            var responseBa = SendReqGetRes(true, id, "BankAccount", "GetAllByHhId");

            if (responseBa.IsSuccessStatusCode)
            {
                model.BankAccounts = MakeNewDropDownList(ReadOutputDropDownList(responseBa));
            }
            else if (responseBa.StatusCode == HttpStatusCode.NotFound)
            {
                AddTempDataMsg(true);
                return View("Error");
            }

            var responseCat = SendReqGetRes(true, id, "Category", "GetAllByHhId");

            if (responseCat.IsSuccessStatusCode)
            {
                model.Categories = MakeNewDropDownList(ReadOutputDropDownList(responseCat));

                return View(model);
            }
            else if (responseCat.StatusCode == HttpStatusCode.NotFound)
            {
                AddTempDataMsg(false);
                return View("Error");
            }

            return View("Error");
        }



    }
}