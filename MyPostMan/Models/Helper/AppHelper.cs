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




}
