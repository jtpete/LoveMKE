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

        // GET: SettingsModels/Edit/5
        [Route("Settings")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            SettingsModel settingsModel = new SettingsModel();
            if (id == null)
            {
                var settings = await db.SettingsModels.ToListAsync();
                if (settings.Count() == 0)
                {
                    settingsModel = CreateSettings();
                }
                else
                {
                    settingsModel = settings.First<SettingsModel>();
                }
            }
            else
            {
                settingsModel = await db.SettingsModels.FindAsync(id);
            }
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
        [Route("Settings")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,HasTShirts,Logo,LogoName,Background,BackgroundName")] SettingsModel settingsModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(settingsModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "AdminDashboard");
            }
            return View(settingsModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        protected SettingsModel CreateSettings()
        {
            SettingsModel settingsModel = new SettingsModel();
            settingsModel.HasTShirts = false;
            settingsModel.LogoName = "";
            settingsModel.BackgroundName = "";
            db.SettingsModels.Add(settingsModel);
            db.SaveChanges();
            return db.SettingsModels.ToList().FirstOrDefault<SettingsModel>();
            

        }
    }
}
