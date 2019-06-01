using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MyPostMan.Models.Helper
{
    public enum CusHttpMethod
    {
        Post,
        Put,
        Get,
        Delete
    }


    public class CusUrlHelper
    {
        //public string Make(string controller, string action)
        //{
        //    return $"http://localhost:44333/api/{controller}/{action}";

        //}
        //public string MakeById(string controller, string action, int id)
        //{

        //    return $"http://localhost:44333/api/{controller}/{action}/{id}";
        //}

        public string MakeForToken()
        {

            return $"http://localhost:44333/token";
        }

        public string Make(string controller, string action, int? id)
        {
            if (!id.HasValue)
            {
                return $"http://localhost:44333/api/{controller}/{action}";
            }
            else
            {
                return $"http://localhost:44333/api/{controller}/{action}/{id}";
            }

        }

    }


    public class RequestHelper
    {
        private readonly CusUrlHelper CusUrlHelper;
        public RequestHelper()
        {
            CusUrlHelper = new CusUrlHelper();
        }

        public bool HasCookie(HttpRequestBase request)
        {
            return request.Cookies["PostManCookie"] != null;
        }

        public string ReadResponse(HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result;
        }

        public HttpResponseMessage SendGetRequest(List<KeyValuePair<string, string>> parameters, string controller, string action, int? id)
        {
            string url;

            if (controller.ToLower() == "token" && action.ToLower() == "token")
            {
                url = CusUrlHelper.MakeForToken();
            }
            else
            {
                url = CusUrlHelper.Make(controller, action, id);
            }


            var httpClient = new HttpClient();

            var encodedValues = new FormUrlEncodedContent(parameters);

            return httpClient.PostAsync(url, encodedValues).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    return "WentThrough";
            //}
        }

        public HttpResponseMessage SendGetRequestAuth(List<KeyValuePair<string, string>> parameters, string controller, string action, int? id, string token, CusHttpMethod method)
        {
            string url;

            if (controller.ToLower() == "token" && action.ToLower() == "token")
            {
                url = CusUrlHelper.MakeForToken();
            }
            else
            {
                url = CusUrlHelper.Make(controller, action, id);
            }


            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
           $"Bearer {token}");


            var encodedValues = new FormUrlEncodedContent(parameters);

            if (method == CusHttpMethod.Put)
            {
                return httpClient.PutAsync(url, encodedValues).Result;
            }
           
            return httpClient.PostAsync(url, encodedValues).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    return "WentThrough";
            //}
        }

        public string GetToken(List<KeyValuePair<string, string>> parameters, string controller, string action, int? id)
        {
            var url = CusUrlHelper.Make(controller, action, id);

            var httpClient = new HttpClient();

            var encodedValues = new FormUrlEncodedContent(parameters);

            var response = httpClient.PostAsync(url, encodedValues).Result;

            if (response.IsSuccessStatusCode)
            {
                return "WentThrough";
            }

            return response.Content.ReadAsStringAsync().Result;
        }

        public HttpResponseMessage SendGetRequestAuthGetDel(string controller, string action, int? id, string token, CusHttpMethod method)
        {
            var url = CusUrlHelper.Make(controller, action, id);
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization",
            $"Bearer {token}");

            if (method == CusHttpMethod.Delete)
            {
                return httpClient.DeleteAsync(url).Result;
            }

            return httpClient.GetAsync(url).Result;
        }

    }


    public class ResponseHelper
    {
        //public ActionResult IfSusccess(HttpResponseMessage response)
        //{
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        //    {
        //        return View("Error");
        //    }

        //    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        //    {
        //        var data = RequestHelper.ReadResponse(response);
        //        var erroMsg = JsonConvert.DeserializeObject<BadUrl>(data);
        //        ModelState.AddModelError("Url", erroMsg.Message);
        //    }

        //    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        //    {
        //        var data = RequestHelper.ReadResponse(response);
        //        var erroMsg = JsonConvert.DeserializeObject<MsgModelState>(data);
        //        erroMsg.ModelState.ToList().ForEach(p =>
        //        {
        //            p.Value.ToList().ForEach(b =>
        //            {
        //                ModelState.AddModelError($"{p.Key.ToString()}", $"{b}");
        //            });

        //        });
        //    }
        //}
    }




}

//public string Make(string controller, string action, int? id)
//{
//    if (!id.HasValue)
//    {
//        return $"http://localhost:44333/api/{controller}/{action}";
//    }
//    else
//    {
//        return $"http://localhost:44333/api/{controller}/{action}/{id}";
//    }

//}