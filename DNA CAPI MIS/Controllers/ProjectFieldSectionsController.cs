using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DNA_CAPI_MIS.DAL;
using DNA_CAPI_MIS.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace DNA_CAPI_MIS.Controllers
{
    public class ProjectFieldSectionsController : Controller
    {
        private ProjectContext db = new ProjectContext();

        // GET: ProjectFieldSections
        public async Task<ActionResult> Index(int? id)
        {
            List<ProjectFieldSection> pfs = await db.ProjectFieldSection
                                            .Where(x => x.ProjectID == (int)id)
                                            .OrderBy(o => o.DisplayOrder)
                                            .ToListAsync();
            ViewBag.ProjectId = id;
            return View(pfs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FormCollection form)
        {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Objects;
            settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

            int id = Convert.ToInt32(form["ProjectId"].ToString());
            string dataSection = form["FieldSectionData"].ToString();

            List<DNA_CAPI_MIS.Models.ProjectFieldSection> pfsList = JsonConvert.DeserializeObject<List<DNA_CAPI_MIS.Models.ProjectFieldSection>>(dataSection, settings);

            ProjectFieldSection existingPFS;
            List<DNA_CAPI_MIS.Models.ProjectFieldSection> ProjectFieldSections = db.ProjectFieldSection.Where(x => x.ProjectID.Equals(id)).ToList<ProjectFieldSection>();
            List<DNA_CAPI_MIS.Models.ProjectFieldSection> PFSToDelete = new List<ProjectFieldSection>();
            if (ProjectFieldSections != null && ProjectFieldSections.Count > 0)
            {
                foreach (ProjectFieldSection pfs in ProjectFieldSections)
                {
                    existingPFS = pfsList.Find(x => x.ID == pfs.ID);
                    if (existingPFS == null)
                    {
                        PFSToDelete.Add(pfs);
                    }
                    else
                    {
                        string title = Regex.Replace(existingPFS.Name, @"\t|\n|\r", "");
                        pfs.DisplayOrder = existingPFS.DisplayOrder;
                        pfs.Name = title;
                        pfsList.Remove(existingPFS);        //So that we know which recs are updated
                    }
                }

                if (PFSToDelete.Count > 0)
                {
                    foreach (ProjectFieldSection pfs in PFSToDelete)
                    {
                        db.ProjectFieldSection.Remove(pfs);
                    }
                }
                db.SaveChanges();
            }
            if (pfsList != null && pfsList.Count > 0)
            {
                //Now these items are not in database and we just need to add them
                foreach (ProjectFieldSection pfs in pfsList)
                {
                    pfs.ProjectID = id;
                    db.ProjectFieldSection.Add(pfs);
                }
                db.SaveChanges();
            }

            List<ProjectFieldSection> pfsl = db.ProjectFieldSection.Where(x => x.ProjectID == (int)id).ToList();
            ViewBag.ProjectId = id;
            return View(pfsl);
        }

        // GET: ProjectFieldSections/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectFieldSection projectFieldSection = await db.ProjectFieldSection.FindAsync(id);
            if (projectFieldSection == null)
            {
                return HttpNotFound();
            }
            return View(projectFieldSection);
        }

        // GET: ProjectFieldSections/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var maxPFS = db.ProjectFieldSection.Where(s => s.ProjectID == (int)id).Max(x => x.DisplayOrder);
            ViewBag.DisplayOrder = maxPFS.GetValueOrDefault(0) + 1;
            ViewBag.ProjectId = id;
            return View();
        }

        // POST: ProjectFieldSections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,ProjectID,DisplayOrder")] ProjectFieldSection projectFieldSection)
        {
            if (ModelState.IsValid)
            {
                db.ProjectFieldSection.Add(projectFieldSection);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(projectFieldSection);
        }

        // GET: ProjectFieldSections/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectFieldSection projectFieldSection = await db.ProjectFieldSection.FindAsync(id);
            if (projectFieldSection == null)
            {
                return HttpNotFound();
            }
            return View(projectFieldSection);
        }

        // POST: ProjectFieldSections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name,ProjectID,DisplayOrder")] ProjectFieldSection projectFieldSection)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectFieldSection).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(projectFieldSection);
        }

        // GET: ProjectFieldSections/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectFieldSection projectFieldSection = await db.ProjectFieldSection.FindAsync(id);
            if (projectFieldSection == null)
            {
                return HttpNotFound();
            }
            return View(projectFieldSection);
        }

        // POST: ProjectFieldSections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ProjectFieldSection projectFieldSection = await db.ProjectFieldSection.FindAsync(id);
            db.ProjectFieldSection.Remove(projectFieldSection);
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
