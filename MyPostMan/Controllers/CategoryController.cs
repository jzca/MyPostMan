using MyPostMan.Models;
using MyPostMan.Models.BindingModel;
using MyPostMan.Models.ViewModel;
using MyPostMan.Models.Helper;
using MyPostMan.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

namespace MyPostMan.Controllers
{
    [AuthorizeFilter]
    public class CategoryController : Controller
    {

        private readonly RequestHelper RequestHelper;
        private string MyToken { get { return Request.Cookies["PostManCookie"]?.Value; } }

        public CategoryController()
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
        public ActionResult ShowMine(int? id)
        {

            if (!id.HasValue)
            {
                return RedirectToAction(nameof(CategoryController.IndexHh));
            }

            var response = RequestHelper.SendGetRequestAuthGetDel("Category"
            , "GetCreatedByHhId", id, MyToken, CusHttpMethod.Get);

            if (response.IsSuccessStatusCode)
            {
                var data = RequestHelper.ReadResponse(response);
                var model = JsonConvert.DeserializeObject<List<CategoryViewModel>>(data);
                return View(model);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var model = new List<CategoryViewModel>();

                return View(model);
            }

            return View();
        }

        [HttpGet]
        public ActionResult ShowAll(int? id)
        {

            if (!id.HasValue)
            {
                return RedirectToAction(nameof(CategoryController.IndexHh));
            }

            var response = RequestHelper.SendGetRequestAuthGetDel("Category"
              , "GetAllByHhId", id, MyToken, CusHttpMethod.Get);


            if (response.IsSuccessStatusCode)
            {
                var data = RequestHelper.ReadResponse(response);
                var model = JsonConvert.DeserializeObject<List<CategoryViewModel>>(data);
                return View(model);
            }


            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var model = new List<CategoryViewModel>();

                return View(model);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Nf = false;
                return View("NofoundAuth");
            }



            return View();
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(int id, CategoryBindingModel formData)
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

            var response = RequestHelper.SendGetRequestAuth(parameters, "Category"
                , "Create", null, MyToken, CusHttpMethod.Post);

            //var data = RequestHelper.ReadResponse(response);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(CategoryController.ShowMine), new { id });
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
            else if (response.StatusCode != HttpStatusCode.BadRequest)
            {
                return View("Error");
            }

            DealBadRequest(response);

            return View();


        }

        [HttpGet]
        public ActionResult Edit(int? id, int? hhId)
        {
            if (!id.HasValue || !hhId.HasValue)
            {
                return RedirectToAction(nameof(CategoryController.IndexHh));
            }

            var response = RequestHelper.SendGetRequestAuthGetDel("Category"
            , "GetByCatId", id, MyToken, CusHttpMethod.Get);


            if (response.IsSuccessStatusCode)
            {
                var data = RequestHelper.ReadResponse(response);
                var model = JsonConvert.DeserializeObject<CategoryBindingModel>(data);
                return View(model);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Nf = true;
                return View("NofoundAuth");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, int hhId, CategoryBindingModel formData)
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

            var response = RequestHelper.SendGetRequestAuth(parameters, "Category"
                , "Edit", id, MyToken, CusHttpMethod.Put);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(CategoryController.ShowMine), new { id = hhId });
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

        [HttpPost]
        public ActionResult Delete(int id, int hhId)
        {

            var response = RequestHelper.SendGetRequestAuthGetDel("Category"
                , "Delete", id, MyToken, CusHttpMethod.Delete);


            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(CategoryController.ShowMine), new { id = hhId });
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

    }
}