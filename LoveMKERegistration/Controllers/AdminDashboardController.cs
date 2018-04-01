using LoveMKERegistration.API;
using LoveMKERegistration.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LoveMKERegistration.Controllers
{
    //[Authorize (Roles = "Admin")] //Uncomment the preceeding to truly lock down access only to Admin.
    public class AdminDashboardController : Controller
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

        // GET: AdminDashboard
        public async Task<ActionResult> Index()
        {
            try
            {
                string typeId = await CCBchurchAPI.GetTypeID("LoveMKE");
                var groupIdList = await CCBchurchAPI.GetGroupIdList(typeId);
                var groupList = await CCBchurchAPI.GetGroups(groupIdList);
                List<string> participantNames = new List<string>();
                groupList.ForEach(g =>
                {
                    participantNames.Add(g.Leader.DisplayName);
                    participantNames.AddRange(g.CurrentMembers.Select(m => m.DisplayName));
                });
                var totalCount = participantNames.Select(n => n).Distinct().Count();
                ViewBag.Count = totalCount;
            }
            catch
            {
                ViewBag.Count = "Unavailable";

            }

            if (hasTshirtSignup)
            {
                ViewBag.Tshirts = true;
            }
            else
            {
                ViewBag.Tshirts = false;

            }

            return View();
        }
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> GetParticipants()
        {
            string typeId = await CCBchurchAPI.GetTypeID("LoveMKE");
            var groupIdList = await CCBchurchAPI.GetGroupIdList(typeId);
            var groupList = await CCBchurchAPI.GetGroups(groupIdList);
            List<ParticipantViewModel> participants = new List<ParticipantViewModel>();
            ParticipantViewModel participant;
            foreach (var group in groupList)
            {
                participant = new ParticipantViewModel();
                participant.FirstName = group.Leader.FirstName;
                participant.LastName = group.Leader.LastName;
                participant.Phone = group.Leader.Phone;
                participant.Email = group.Leader.Email;
                participant.GroupName = group.Name;
                participants.Add(participant);

                foreach (var member in group.CurrentMembers)
                {
                    participant = new ParticipantViewModel();
                    participant.FirstName = member.FirstName;
                    participant.LastName = member.LastName;
                    participant.Phone = member.Phone;
                    participant.Email = member.Email;
                    participant.GroupName = group.Name;
                    participants.Add(participant);
                }
            }
            TempData["participants"] = participants;
            return RedirectToAction("GroupParticipants", participants);
        }

        public ActionResult GroupParticipants(List<ParticipantViewModel> participants)
        {
            participants = (List<ParticipantViewModel>)TempData["participants"];
            if (TempData["group"] != null)
                ViewBag.GroupName = TempData["group"];
            ViewBag.Groups = participants.Select(p => p.GroupName).Distinct().ToList();
            return View(participants);
        }
        [HttpPost]
        public ActionResult GroupParticipants([Bind(Include = "FirstName,LastName,Phone,Email,GroupName")]List<ParticipantViewModel> participants, string group)
        {
            if(ModelState.IsValid)
            {
                ViewBag.DisplayGroup = group;
                ViewBag.Groups = participants.Select(p => p.GroupName).Distinct().ToList();
                return View(participants);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        // GET: AdminDashboard/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminDashboard/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminDashboard/Create
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

        // GET: AdminDashboard/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminDashboard/Edit/5
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

        // GET: AdminDashboard/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminDashboard/Delete/5
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
    }
}
