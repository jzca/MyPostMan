using MyPostMan.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MyPostMan.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var url = "https://jsonplaceholder.typicode.com/todos/1";

            //var httpClient = new HttpClient();

            //var data =httpClient.GetStringAsync(url).Result;

            //var result = JsonConvert.DeserializeObject<Todo>(data);

            //ViewBag.Todo = result;

            return View();
        }

        //public ActionResult PutIt()
        //{
        //    //Url and parameters to post
        //    var url = "https://jsonplaceholder.typicode.com/posts/1";
        //    var title = "my title";
        //    var body = "This is the body of my post Title 2";
        //    var completed = true;
        //    var userId = "999";

        //    //HttpClient object to handle the comunication
        //    var httpClient = new HttpClient();

        //    //Parameters list with KeyValue pair
        //    var parameters = new List<KeyValuePair<string, string>>();
        //    parameters.Add(new KeyValuePair<string, string>("title", title));
        //    parameters.Add(new KeyValuePair<string, string>("body", body));
        //    parameters.Add(new KeyValuePair<string, string>("completed", completed.ToString()));
        //    parameters.Add(new KeyValuePair<string, string>("userId", userId));

        //    //Encoding the parameters before sending to the API
        //    var encodedParameters = new FormUrlEncodedContent(parameters);

        //    //Calling the API and storing the response
        //    var response = httpClient.PutAsync(url, encodedParameters).Result;

        //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        //Read the response
        //        var data = response.Content.ReadAsStringAsync().Result;

        //        //Convert the data back into an object
        //        var result = JsonConvert.DeserializeObject<Todo>(data);
        //        ViewBag.Todo = result;
        //    }

        //    return View("Index");
        //}

        //public ActionResult RmIt()
        //{
        //    var url = "https://jsonplaceholder.typicode.com/posts/1";

        //    //HttpClient object to handle the comunication
        //    var httpClient = new HttpClient();

        //    //Calling the API and storing the response
        //    var response = httpClient.DeleteAsync(url).Result;

        //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        //Read the response
        //        var data = response.Content.ReadAsStringAsync().Result;

        //        //Convert the data back into an object
        //        //var result = JsonConvert.DeserializeObject<Todo>(data);
        //        //ViewBag.Del = result;
        //    }

        //    return View("About");
        //}





        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}