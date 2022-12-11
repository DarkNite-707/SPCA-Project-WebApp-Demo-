using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;


namespace WebApplication2.Controllers
{
    public class ContactController : Controller
    {


        
      

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "dp85WjX2Q3s8FscsO5Sc78QUQRncLkcBw5L6kBsc",
            
            BasePath = "https://mvc-spca-demo-default-rtdb.firebaseio.com/"

        };

        IFirebaseClient client;



        Potential_Adopter model = new Potential_Adopter();

        // GET: Contact
        //[HttpGet]
        public ActionResult Index()
        {
            return View(model);
        }




        // This Method was Adapted from Youtube
        //https://www.youtube.com/watch?v=pg-MKEWEM5U&list=PLsCCbqYZfKoyuIOA2RCO1aqtPVlwnDXg2&index=2
        // IT Traveler
        //https://www.youtube.com/@ittraveler8261

        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Index(Potential_Adopter P_A_Obj)
        {

            try
            {
                AddPotentialAdopterToFirebase(P_A_Obj);
                ModelState.AddModelError(string.Empty, "Added Successfully!");
                return RedirectToAction("Index");



            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex);
            }
           
            return RedirectToAction("Index");
            //if (ModelState.IsValid)
            //{
            //    AddPotentialAdopterToFirebase(P_A_Obj);
            //    return RedirectToAction("Index");
            //}

            //return View(P_A_Obj);

        }


        // This Method was Adapted from Youtube
        //https://www.youtube.com/watch?v=pg-MKEWEM5U&list=PLsCCbqYZfKoyuIOA2RCO1aqtPVlwnDXg2&index=2
        // IT Traveler
        //https://www.youtube.com/@ittraveler8261
        private void AddPotentialAdopterToFirebase(Potential_Adopter Ptl_Adtr_Obj)
        {

            client = new FireSharp.FirebaseClient(config);
            var data = Ptl_Adtr_Obj;
            PushResponse response = client.Push("Potential Adopters/", data);
            data.Ptl_Adopter_id = response.Result.name;
            SetResponse setResponse = client.Set("Potential Adopters/" + data.Ptl_Adopter_id, data);
        }

    }
}