using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Firebase.Database;
using Firebase.Database.Query;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //public async Task<ActionResult> About()
        //{
        //    //Simulate test user data and login timestamp
        //    var userId = "12345";
        //    var currentLoginTime = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");

        //    //Save non identifying data to Firebase
        //    var currentUserLogin = new LoginData() { TimestampUtc = currentLoginTime };
        //    var firebaseClient = new FirebaseClient("yourFirebaseProjectUrl");
        //    var result = await firebaseClient.Child("Users/" + userId + "/Logins").PostAsync(currentUserLogin);

        //    //Retrieve data from Firebase
        //    var dbLogins = await firebaseClient.Child("Users").Child(userId).Child("Logins").OnceAsync<LoginData>();

        //    var timestampList = new List<DateTime>();

        //    //Convert JSON data to original datatype
        //    foreach (var login in dbLogins)
        //    {
        //        timestampList.Add(Convert.ToDateTime(login.Object.TimestampUtc).ToLocalTime());
        //    }

        //    //Pass data to the view
        //    ViewBag.CurrentUser = userId;
        //    ViewBag.Logins = timestampList.OrderByDescending(x => x);
        //    return View();
        //} 
        
        public async Task<ActionResult> Details()
        {
            //Simulate test user data and login timestamp
            var userId = "12345";
            var AnimalCde = "ANL_xabnmabmmm$%#&*****,ajlkkjkljklkj";
            var Animalnme = "Liger";
            var AnimalPrc = "1 million";

            //Save non identifying data to Firebase
            var My_Animal_Data = new Animal() 
            {  Animal_Code = AnimalCde,
               Animal_name  = Animalnme,
               Animal_Price = AnimalPrc
            };

            var firebaseClient = new FirebaseClient("https://mvc-spca-demo-default-rtdb.firebaseio.com/");
            var result = await firebaseClient.Child("Animals/").PostAsync(My_Animal_Data);

            //Retrieve data from Firebase
            var dbLogins = await firebaseClient.Child("Animals").Child(userId).Child("Logins").OnceAsync<Animal>();

            var Animal_list = new List<string>();

            //Convert JSON data to original datatype
            foreach (var login in dbLogins)
            {
                Animal_list.Add(Convert.ToString(login.Object).ToString());
            }

            //Pass data to the view
            ViewBag.CurrentUser = userId;
            ViewBag.Logins = Animal_list.OrderByDescending(x => x);
            return View();
        }
    }
}