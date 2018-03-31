using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LoveMKERegistration.Models;

namespace LoveMKERegistration.Controllers
{
    public class SettingsModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SettingsModels
        public async Task<ActionResult> Index()
        {
            return View(await db.SettingsModels.ToListAsync());
        }

        // GET: SettingsModels/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SettingsModel settingsModel = await db.SettingsModels.FindAsync(id);
            if (settingsModel == null)
            {
                return HttpNotFound();
            }
            return View(settingsModel);
        }

        // GET: SettingsModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SettingsModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,HasTShirts,Logo,Background")] SettingsModel settingsModel)
        {
            if (ModelState.IsValid)
            {
                db.SettingsModels.Add(settingsModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(settingsModel);
        }

        // GET: SettingsModels/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SettingsModel settingsModel = await db.SettingsModels.FindAsync(id);
            if (settingsModel == null)
            {
                return HttpNotFound();
            }
            return View(settingsModel);
        }

        // POST: SettingsModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,HasTShirts,Logo,Background")] SettingsModel settingsModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(settingsModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(settingsModel);
        }

        // GET: SettingsModels/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SettingsModel settingsModel = await db.SettingsModels.FindAsync(id);
            if (settingsModel == null)
            {
                return HttpNotFound();
            }
            return View(settingsModel);
        }

        // POST: SettingsModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            SettingsModel settingsModel = await db.SettingsModels.FindAsync(id);
            db.SettingsModels.Remove(settingsModel);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
