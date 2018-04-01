using LoveMKERegistration.API;
using LoveMKERegistration.Models;
using LoveMKERegistration.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace LoveMKERegistration.Controllers
{
    [RoutePrefix("signup")]
    public class SignupViewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private bool hasTshirtSignup
        {
            get
            {
                var settings = db.SettingsModels.ToList().First<SettingsModel>();
                return settings.HasTShirts;
            }

            set { }
        }

        [Route("{groupTypeName}")]
        // GET: /signup/LoveMKE
        public async Task<ActionResult> Signup(string groupTypeName)
        {
            string typeId = await CCBchurchAPI.GetTypeID(groupTypeName);
            var groupIdList = await CCBchurchAPI.GetGroupIdList(typeId);
            var modelList = await CCBchurchAPI.GetGroups(groupIdList);

            SignupViewModel thisSignup = new SignupViewModel()
            {
                Family = (IndividualViewModel)TempData["person"],
                Groups = modelList
            };
            return View(thisSignup);
        }

        //POST: signup/LoveMKE
        [HttpPost]
        public async Task<ActionResult> AddIndividuals(SignupViewModel model, string[] addPerson)
        {
            try
            {
                foreach(string s in addPerson)
                {
                    string[] splitString = s.Split('-');
                    string userId = splitString[0];
                    string groupId = splitString[1];

                    await CCBchurchAPI.APIcall(CCBchurchAPI.GetAddToGroupService(groupId, userId));
                }
                IndividualViewModel individual = (IndividualViewModel)TempData["person"];
                TempData["individual"] = individual;
                SignupViewModel signup = (SignupViewModel)TempData["signup"];
                SendEmailConfirmation(signup, addPerson);
                TempData["individual"] = signup.Family;

                if (hasTshirtSignup)
                {
                    return RedirectToAction("Tshirts", "TShirtSizeModels");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return View(model);
            }
        }

        // GET: SignupView/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SignupView/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SignupView/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SignupView/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SignupView/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SignupView/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SignupView/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        private void SendEmailConfirmation(SignupViewModel signup, string[] signupList)
        {
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(signup.Family.Email));
            message.Subject = "LoveMKE Sign-up Confirmation";
            string registrationDetails = "Sign-up details:\n";
            string personName;
            string groupName;
            foreach (string pair in signupList)
            {
                string[] splitString = pair.Split('-');
                string userId = splitString[0];
                string groupId = splitString[1];
                if (signup.Family.IndividualId == userId)
                    personName = signup.Family.DisplayName;
                else
                    personName = signup.Family.Family.Where(m => m.IndividualId == userId).Select(m => m.DisplayName).First();
                groupName = signup.Groups.Where(g => g.GroupId == groupId).Select(g => g.Name).First();
                registrationDetails += $"{personName} signed up for: {groupName} \n";
            }
            string confirmRegistration = Email.GetConfirmationRegistration();
            string website = Email.GetWebsiteDetails();

            message.Body = confirmRegistration + website + registrationDetails;
            Email.Send(message);
        }
    }
}
