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


namespace LoveMKERegistration.Controllers
{
    public class TShirtSizeModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TShirtSizeModels
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> Tshirts()
        {
            IndividualViewModel individual = (IndividualViewModel)TempData["individual"];
            List<TShirtSizeModel> tShirts = db.TShirtSizeModel.ToList();
            string typeId = await CCBchurchAPI.GetTypeID("LoveMKE");
            var groupIdList = await CCBchurchAPI.GetGroupIdList(typeId);
            var groupList = await CCBchurchAPI.GetGroups(groupIdList);

            var sizes = GetAllSizes();
            List<TShirtSizeModel> familyShirts = new List<TShirtSizeModel>();
            TShirtSizeModel person = new TShirtSizeModel()
            {
                FirstName = individual.FirstName,
                LastName = individual.LastName,
                IndividualId = individual.IndividualId
            };
            if (tShirts.Where(t => t.IndividualId == individual.IndividualId).Count() > 0)
            {
                person.Size = tShirts.Where(t => t.IndividualId == individual.IndividualId).Select(s => s.Size).First();
                person.TShirtSizes = GetSelectListItems(sizes, person.Size);
            }
            else if (groupList.Count(g => g.CurrentMembers.Where(m => m.IndividualId == individual.IndividualId).Count() > 0) > 0)
            {
                person.Size = "None";
                person.TShirtSizes = GetSelectListItems(sizes, person.Size);
            }
            else if(groupList.Where(l => l.LeaderID == individual.IndividualId).Count() > 0)
            {
                person.Size = "None";
                person.TShirtSizes = GetSelectListItems(sizes, person.Size);
            }
            else
                person.Size = "Not signed up for project";

            familyShirts.Add(person);

            for (int i = 0; i < individual.Family?.Count(); i++)
            {
                person = new TShirtSizeModel()
                {
                    FirstName = individual.Family[i].FirstName,
                    LastName = individual.Family[i].LastName,
                    IndividualId = individual.Family[i].IndividualId,

                };
                if (tShirts.Where(t => t.IndividualId == individual.Family[i].IndividualId).Count() > 0)
                {
                    person.Size = tShirts.Where(t => t.IndividualId == individual.Family[i].IndividualId).Select(s => s.Size).First();
                    person.TShirtSizes = GetSelectListItems(sizes, person.Size);
                }
                else if (groupList.Count(g => g.CurrentMembers.Where(m => m.IndividualId == individual.Family[i].IndividualId).Count() > 0) > 0)
                {
                    person.Size = "None";
                    person.TShirtSizes = GetSelectListItems(sizes, person.Size);
                }
                else
                    person.Size = "Not signed up for project";
                familyShirts.Add(person);
            }
            return View(familyShirts);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Tshirts(List<TShirtSizeModel> tShirts)
        {
            if (ModelState.IsValid)
            {
                var sizes = GetAllSizes();
                List<TShirtSizeModel> allTShirts = db.TShirtSizeModel.ToList();
                foreach (var person in tShirts)
                {
                    if (person.Size != "Not signed up for project")
                    {

                        if(allTShirts.Where(p => p.IndividualId == person.IndividualId && p.Size != person.Size).Count() > 0)
                        {
                            db.Entry(allTShirts.Where(p => p.IndividualId == person.IndividualId && p.Size != person.Size).Select(t => t).First()).CurrentValues.SetValues(person);
                            db.SaveChanges();
                            ViewBag.Message = "Updated";
                        }
                        else if(allTShirts.Where(p => p.IndividualId == person.IndividualId).Count() == 0)
                        {
                            db.TShirtSizeModel.Add(person);
                            db.SaveChanges();
                            ViewBag.Message = "Updated";

                        }
                    }
                    person.TShirtSizes = GetSelectListItems(sizes, person.Size);
                }
                return View(tShirts);
            }
            return View(tShirts);
        }
        public ActionResult TshirtTotals()
        {
            List<TShirtSizeModel> tShirts = db.TShirtSizeModel.ToList();
            List<TshritSizeTotalViewModel> sizeTotals = new List<TshritSizeTotalViewModel>();
            TshritSizeTotalViewModel sizeTotal;
            IEnumerable<string> sizes = GetAllSizes();
            foreach (var size in sizes)
            {
                sizeTotal = new TshritSizeTotalViewModel();
                sizeTotal.Size = size;
                sizeTotal.Total = tShirts.Where(s => s.Size == size).Count();
                sizeTotals.Add(sizeTotal);
                
            }
            return View(sizeTotals);
        }
        public ActionResult TshirtList()
        {

            List<TShirtSizeModel> tShirts = db.TShirtSizeModel.ToList();
            
            return View(tShirts);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TshirtList(List<TShirtSizeModel> tShirts)
        {
            if (ModelState.IsValid)
            {
                
                return View(tShirts);
            }
            return View(tShirts);
        }

        // GET: TShirtSizeModels/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TShirtSizeModel tShirtSizeModel = db.TShirtSizeModel.Find(id);
            if (tShirtSizeModel == null)
            {
                return HttpNotFound();
            }
            return View(tShirtSizeModel);
        }

        // GET: TShirtSizeModels/Create
        public ActionResult GetPerson()
        {
            return View();
        }

        // POST: TShirtSizeModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> GetPerson(string email)
        {
            IndividualViewModel individual = new IndividualViewModel()
            {
                Email = email.Trim(),
            };
            string apiRequest = CCBchurchAPI.IndividualSearchService("", "", email.Trim(), 'E');
            XDocument results = await CCBchurchAPI.APIcall(apiRequest);
            string searchCount = IndividualSearchCount(results);
            int inactiveSearch = IndividualSearchInactiveUser(results);
            if (searchCount == "1" && inactiveSearch != 0)
            {
                IndividualViewModel thisIndividual = await CCBchurchAPI.GetIndividualViewModelById(GetIndividualId(results));
                TempData["individual"] = thisIndividual;
                return RedirectToAction("Tshirts");
            }
            else
            {
                return RedirectToAction("AdvanceSearch", individual);
            }
        }
        public ActionResult AdvanceSearch(IndividualViewModel individual)
        {
            return View(individual);
        }

        // POST: TShirtSizeModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> AdvanceSearch([Bind(Include = "FirstName,LastName,Phone,Email")] IndividualViewModel individualViewModel, string fullName)
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
                    TempData["individual"] = thisIndividual;
                    return RedirectToAction("Tshirts");
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
                        TempData["individual"] = thisIndividual;
                        return RedirectToAction("Tshirts");
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: TShirtSizeModels/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TShirtSizeModel tShirtSizeModel = db.TShirtSizeModel.Find(id);
            if (tShirtSizeModel == null)
            {
                return HttpNotFound();
            }
            var sizes = GetAllSizes();
            tShirtSizeModel.TShirtSizes = GetSelectListItems(sizes, tShirtSizeModel.Size);
            return View(tShirtSizeModel);
        }

        // POST: TShirtSizeModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IndividualId,Size,FirstName,LastName")] TShirtSizeModel tShirtSizeModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tShirtSizeModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("TshirtList");
            }
            return View(tShirtSizeModel);
        }

        // GET: TShirtSizeModels/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TShirtSizeModel tShirtSizeModel = db.TShirtSizeModel.Find(id);
            if (tShirtSizeModel == null)
            {
                return HttpNotFound();
            }
            return View(tShirtSizeModel);
        }

        // POST: TShirtSizeModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TShirtSizeModel tShirtSizeModel = db.TShirtSizeModel.Find(id);
            db.TShirtSizeModel.Remove(tShirtSizeModel);
            db.SaveChanges();
            return RedirectToAction("TshirtList");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private string IndividualSearchCount(XDocument document)
        {
            var individualSearch = document.Descendants("response").Select(s => s);
            string countString = individualSearch.Select(s => s.Element("individuals").Attribute("count").Value).First();
            return countString;
        }
        private int IndividualSearchInactiveUser(XDocument document)
        {
            var inactiveCount = document.Descendants("individual").Select(s => s).Count();
            return inactiveCount;
        }
        private string GetIndividualId(XDocument document)
        {
            var individualSearch = document.Descendants("individuals").Select(s => s);
            string individualId = individualSearch.Select(s => s.Element("individual").Attribute("id").Value).First();
            return individualId;
        }
        private IEnumerable<string> GetAllSizes()
        {
            return new List<string>
            {
                "None","XXXL","XXL","XL","L","M","S","XL (Child)","L (Child)", "M (Child)","S (Child)", "XS (Child)"
            };
        }
        private IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<string> elements, string selectedElement)
        {
            var selectList = new List<SelectListItem>();


            foreach (var element in elements)
            {
                SelectListItem item = new SelectListItem();
                item.Value = element;
                item.Text = element;
                if (element == selectedElement)
                    item.Selected = true;
                selectList.Add(item);
            }

            return selectList;
        }
    }
}
