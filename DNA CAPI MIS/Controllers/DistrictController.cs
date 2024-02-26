using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DNA_CAPI_MIS.Models;
using DNA_CAPI_MIS.DAL;
using Newtonsoft.Json;

namespace DNA_CAPI_MIS.Controllers
{
    public class DistrictController : Controller
    {
        private ProjectContext db = new ProjectContext();

        // GET: /District/
        public ActionResult Index()
        {
            var District = db.District.Include(d => d.City);
            return View(District.ToList());
        }

        // GET: /District/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            District district = db.District.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            return View(district);
        }

        // GET: /District/Create
        public ActionResult Create()
        {
            ViewBag.CityID = new SelectList(db.City, "ID", "Name");
            return View();
        }

        // POST: /District/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Name,CityID")] District district)
        {
            if (ModelState.IsValid)
            {
                db.District.Add(district);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CityID = new SelectList(db.City, "ID", "Name", district.CityID);
            return View(district);
        }

        // GET: /District/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            District district = db.District.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.City, "ID", "Name", district.CityID);
            return View(district);
        }

        // POST: /District/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Name,CityID")] District district)
        {
            if (ModelState.IsValid)
            {
                db.Entry(district).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityID = new SelectList(db.City, "ID", "Name", district.CityID);
            return View(district);
        }

        // GET: /District/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            District district = db.District.Find(id);
            if (district == null)
            {
                return HttpNotFound();
            }
            return View(district);
        }

        // POST: /District/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            District district = db.District.Find(id);
            db.District.Remove(district);
            db.SaveChanges();
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

        [HttpPost]
        public string GetDistrictsFilterByCities(string CityIDs)
        {
            if (CityIDs != null)
            {
                string[] IDs = CityIDs.Split(',');
                if (IDs.Length > 0)
                {
                    string sql = string.Format(@"SELECT District.Id, District.Name, City.Name AS CityName FROM District INNER JOIN City ON District.CityId = City.Id 
                        WHERE City.Id IN ({0}) ORDER BY City.Name, District.Name", CityIDs);
                    var query = db.Database.SqlQuery<DistrictsView>(sql);
                    List<DistrictsView> Districts = query.ToList<DistrictsView>();

                    //var settings = new JsonSerializerSettings();
                    //settings.TypeNameHandling = TypeNameHandling.Objects;
                    //settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

                    return JsonConvert.SerializeObject(Districts);
                }
            }
            return "[]";
        }

        [HttpPost]
        public string MapToForProject(int id, string mapToDistName, int projectId)
        {
            if (id > 0 && projectId > 0)
            {
                if (mapToDistName.Length == 0)
                {
                    //Delete previous mapping
                    string sql = string.Format("DELETE FROM DistrictMapping WHERE DistrictId = {0} AND ProjectId = {1} AND Source = 'PROJECT'", id, projectId);
                    db.Database.ExecuteSqlCommand(sql);
                    return "OK";
                }
                else
                {
                    //Create a new mapping record
                    string sql = string.Format(@"SELECT count(*) FROM DistrictMapping WHERE DistrictId = {0} AND ProjectId = {1} AND Source = 'PROJECT'", id, projectId);
                    var query = db.Database.SqlQuery<int>(sql);
                    int mapFound = query.FirstOrDefault<int>();
                    if (mapFound > 0)
                    {
                        sql = string.Format("UPDATE DistrictMapping SET Name = '{0}' WHERE DistrictId = {0} AND ProjectId = {1} AND Source = 'PROJECT'", mapToDistName, id, projectId);
                        db.Database.ExecuteSqlCommand(sql);
                    }
                    else
                    {
                        sql = string.Format("INSERT INTO DistrictMapping (DistrictId, ProjectID, Source, Name) VALUES ({0}, {1}, 'PROJECT', '{2}')", id, projectId, mapToDistName);
                        db.Database.ExecuteSqlCommand(sql);
                    }
                    return "OK";
                }
            }
            return "Not Found";
        }


    }
}
