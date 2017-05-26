using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LoveMKERegistration.Models;
using LoveMKERegistration.API;
using System.Xml.Linq;
using System.Web.Routing;
using System.Threading.Tasks;

namespace LoveMKERegistration.Controllers
{
    public class IndividualViewModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: IndividualViewModels
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register([Bind(Include = "Email")] IndividualViewModel individualViewModel)
        {
            if (ModelState.IsValid)
            {
                string apiRequest = CCBchurchAPI.IndividualSearchService(individualViewModel.FirstName?.Trim(), individualViewModel.LastName?.Trim(), individualViewModel.Email?.Trim(), 'E');
                XDocument results = await CCBchurchAPI.APIcall(apiRequest);
                string searchCount = IndividualSearchCount(results);
                int inactiveSearch = IndividualSearchInactiveUser(results);
                if (searchCount == "1" && inactiveSearch != 0)
                {
                    IndividualViewModel thisIndividual = await CCBchurchAPI.GetIndividualViewModelById(GetIndividualId(results));
                    TempData["person"] = thisIndividual;
                    return RedirectToAction("LoveMKE", "signup");
                }
                else
                {
                    return RedirectToAction("AdvanceSearch", individualViewModel);
                }


                return RedirectToAction("Index");
            }

            return View(individualViewModel);

        }
        // GET: IndividualViewModels
        public ActionResult AdvanceSearch(IndividualViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdvanceSearch([Bind(Include = "FirstName,LastName,Phone,Email")] IndividualViewModel individualViewModel, string fullName)
        {
            if (ModelState.IsValid)
            {
                string fullSearchCount;
                string phoneSearchCount;
                string apiRequest = CCBchurchAPI.IndividualSearchService(individualViewModel.FirstName?.Trim(), individualViewModel.LastName?.Trim(), individualViewModel.Phone?.Trim(), individualViewModel.Email?.Trim());
                XDocument results = await CCBchurchAPI.APIcall(apiRequest);
                fullSearchCount = IndividualSearchCount(results);
                int inactiveSearch = IndividualSearchInactiveUser(results);
                if (fullSearchCount == "1" && inactiveSearch != 0)
                {
                    IndividualViewModel thisIndividual = await CCBchurchAPI.GetIndividualViewModelById(GetIndividualId(results));
                    TempData["person"] = thisIndividual;
                    return RedirectToAction("LoveMKE", "signup");
                }
                else
                {
                    apiRequest = CCBchurchAPI.IndividualSearchService(individualViewModel.FirstName?.Trim(), individualViewModel.LastName?.Trim(), individualViewModel.Phone?.Trim(), 'P');
                    results = await CCBchurchAPI.APIcall(apiRequest);
                    phoneSearchCount = IndividualSearchCount(results);
                    inactiveSearch = IndividualSearchInactiveUser(results);
                    if (phoneSearchCount == "1" && inactiveSearch != 0)
                    {
                        IndividualViewModel thisIndividual = await CCBchurchAPI.GetIndividualViewModelById(GetIndividualId(results));
                        TempData["person"] = thisIndividual;
                        return RedirectToAction("LoveMKE", "signup");
                    }
                    else if (inactiveSearch == 0)
                    {
                        return RedirectToAction("AddIndividualToCCB", individualViewModel);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");

        }
        [HttpGet]
        public ActionResult AddIndividualToCCB(IndividualViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddIndividualToCCB([Bind(Include = "FirstName,LastName,Phone,Email,Family")] IndividualViewModel individualViewModel, string radioResponse, string familyNumber)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int numberOfFamilyMemebers = Int32.Parse(familyNumber.Trim());
                    if (radioResponse == "No")
                    {
                        await CCBchurchAPI.PostIndividualFamilyToCCB(individualViewModel);
                        IndividualViewModel thisPerson = await GetAllIndividualIds(individualViewModel);
                        if (thisPerson != null)
                        {
                            TempData["person"] = thisPerson;
                            return RedirectToAction("LoveMKE", "signup");
                        }
                        else
                            return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        if (individualViewModel.Family == null)
                            individualViewModel.Family = new List<FamilyViewModel>();
                        for (int i = 0; i < numberOfFamilyMemebers; i++)
                        {
                            FamilyViewModel familyMember = new FamilyViewModel()
                            {
                                FirstName = "",
                                LastName = individualViewModel.LastName?.Trim(),
                                IndividualId = "",
                                Position = ""
                            };
                            individualViewModel.Family.Add(familyMember);
                        }
                        var individual = new IndividualViewModel()
                        {
                            FirstName = individualViewModel.FirstName,
                            LastName = individualViewModel.LastName,
                            Email = individualViewModel.Email,
                            Phone = individualViewModel.Phone,
                            Family = individualViewModel.Family

                        };
                        return GetFamilyInfo(individual);
                    }
                }
                catch (Exception e)
                {
                    return View(individualViewModel);
                }
            }
            return RedirectToAction("Index", "Home");

        }
        [HttpGet]
        public ActionResult GetFamilyInfo(IndividualViewModel individual)
        {
            return View("GetFamilyInfo", individual);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetFamilyInfo([Bind(Include = "FirstName,LastName,Phone,Email,Family")] IndividualViewModel individualViewModel, string familyNumber)
        {
            if (ModelState.IsValid)
            {
                await CCBchurchAPI.PostIndividualFamilyToCCB(individualViewModel);
                IndividualViewModel thisPerson = await GetAllIndividualIds(individualViewModel);
                if (thisPerson != null)
                {
                    TempData["person"] = thisPerson;
                    return RedirectToAction("LoveMKE", "signup");
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");

        }
        private string IndividualSearchCount(XDocument document)
        {
            var individualSearch = document.Descendants("response").Select(s => s);
            string countString = individualSearch.Select(s => s.Element("individuals").Attribute("count").Value).First();
            return countString;
        }
        private string GetIndividualId(XDocument document)
        {
            var individualSearch = document.Descendants("individuals").Select(s => s);
            string individualId = individualSearch.Select(s => s.Element("individual").Attribute("id").Value).First();
            return individualId;
        }
        private int IndividualSearchInactiveUser(XDocument document)
        {
            var inactiveCount = document.Descendants("individual").Select(s => s).Count();
            return inactiveCount;
        }
        private async Task<IndividualViewModel> GetAllIndividualIds(IndividualViewModel person)
        {
            IndividualViewModel thisIndividual;
            string apiRequest = CCBchurchAPI.IndividualSearchService(person.FirstName, person.LastName, person.Phone, person.Email);
            XDocument results = await CCBchurchAPI.APIcall(apiRequest);
            string count = IndividualSearchCount(results);
            if (count == "1")
                thisIndividual = await CCBchurchAPI.GetIndividualViewModelById(GetIndividualId(results));
            else
                thisIndividual = null;
            return thisIndividual;
        }

    }
}
