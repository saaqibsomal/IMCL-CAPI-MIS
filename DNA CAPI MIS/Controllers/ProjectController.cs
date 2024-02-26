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
using System.Web.Script.Serialization;
using DNA_CAPI_MIS.Controllers;
using Microsoft.AspNet.Identity;
using System.Data.SqlTypes;

namespace DNA_CAPI_MIS
{
    public class ProjectController : Controller
    {
        private ProjectContext db = new ProjectContext();
        public List<SelectListItem> status = new List<SelectListItem>();
        public List<SelectListItem> type = new List<SelectListItem>();
        public List<SelectListItem> visibility = new List<SelectListItem>();

        public ProjectController() 
        {
            status.Add(new SelectListItem() { Text = "Draft", Value = "D", Selected = true });
            status.Add(new SelectListItem() { Text = "Test", Value = "T" });
            status.Add(new SelectListItem() { Text = "Published", Value = "P" });
            status.Add(new SelectListItem() { Text = "Closed", Value = "C" });
            ViewBag.Status = status;

            type.Add(new SelectListItem() { Text = "Survey", Value = "S", Selected = true });
            type.Add(new SelectListItem() { Text = "Census", Value = "C" });
            ViewBag.Type = type;

            visibility.Add(new SelectListItem() { Text = "Only Interviewer can view", Value = "I", Selected = true });
            visibility.Add(new SelectListItem() { Text = "Only Respondent can view", Value = "R" });
            visibility.Add(new SelectListItem() { Text = "Both can view", Value = "B" });
            ViewBag.ProjectVisibility = visibility;
        }

        // GET: /Project/
        public ActionResult Index()
        {
            string sql = "";
            if (User.IsInRole("Admin"))
            {
                sql = @"SELECT * FROM Project ORDER BY name";
            }
            else
            {
                string userFilter = "";
                string uid = User.Identity.GetUserId();
                userFilter = " AND pur.UserId = " + uid;
                sql = @"SELECT * FROM project p WHERE id IN (SELECT ObjectValue FROM DNAShared2.dbo.UserRights pur WHERE pur.ObjectName = 'PROJECT'" + userFilter + ")";
            }

            var query = db.Database.SqlQuery<Project>(sql);
            var data = query.ToList<Project>();

            return View(data);
        }

        // GET: /Project/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: /Project/Create
        public ActionResult Create()
        {
            Project project = new Project();
            project.Guid = System.Guid.NewGuid().ToString();

            return View(project);
        }

        // POST: /Project/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Guid,Name,questions_list,fld_respondent_name,fld_respondent_phone1,fld_respondent_phone2,QCRequirement,CreatedByUserId,CreatedOn,ModifiedByUserId,ModifiedOn,STGSync,Status,StatusChangedByUserId,StatusChangedOn,Type,CategoryPFId,TitlePFId,RespondentNamePFId,RespondentMobilePFId,NeedMapInput,NeedBuildingSearch,Visibility")] Project project)
        {
            if (ModelState.IsValid)
            {
                int uid = Convert.ToInt32(User.Identity.GetUserId());

                if (project.Status == null || project.Status == "Draft" || project.Status.Length == 0)
                {
                    project.Status = "D";
                }
                else
                {
                    project.Status = project.Status.Substring(0, 1);
                }
                if (project.Type == null || project.Type == "Survey" || project.Type.Length == 0)
                {
                    project.Type = "S";
                }
                else
                {
                    project.Type = project.Type.Substring(0, 1);
                }
                //if (project.Visibility == null || project.Visibility == "Only Interviewer can view" || project.Visibility.Length == 0)
                //{
                //    project.Visibility = "I";
                //}
                //else
                //{
                //    project.Visibility = project.Visibility.Substring(0, 1);
                //}
                project.StatusChangedByUserId = uid;
                project.StatusChangedOn = DateTime.Now;
                project.QCRequirement = 0;
                project.CreatedOn = DateTime.Now;
                project.CreatedByUserId = uid;
                project.ModifiedOn = DateTime.Now;
                project.ModifiedByUserId = uid;
                try
                {
                    db.Projects.Add(project);
                    db.SaveChanges();
                    return RedirectToAction("ProjectOverview", "Designer", new { id = project.ID });
                }
                catch (Exception e)
                {
                    return View(project);
                }
            }

            return View(project);
        }

        // GET: /Project/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            foreach (var ps in status)
            {
                if (project.Status == ps.Value)
                {
                    ps.Selected = true;
                    break;
                }
            }
            ViewBag.Status = status;

            foreach (var pt in type)
            {
                if (project.Type == pt.Value)
                {
                    pt.Selected = true;
                    break;
                }
            }
            ViewBag.Type = type;

            //foreach (var v in visibility)
            //{
            //    if (project.Visibility == v.Value)
            //    {
            //        v.Selected = true;
            //        break;
            //    }
            //}
            //ViewBag.ProjectVisibility = visibility;


            List<SelectListItem> pfCategory = new List<SelectListItem>();
            List<SelectListItem> pfTitle = new List<SelectListItem>();

            List<ProjectField> pfList = db.ProjectField
                .Where<ProjectField>(x => x.ProjectID == id && x.IsActive)
                .OrderBy(o => o.DisplayOrder)
                .ToList();
            pfList.Insert(0, new ProjectField { ID = 0, Title = "", ReportTitle = "" });

            var pfCategoryListItem = pfList.Select(x => new SelectListItem()
                {
                    Selected = project.CategoryPFId.Equals(x.ID),
                    Text = (x.Title.Length > 100 ? x.Title.Substring(0, 100) : x.Title),     //Hiding ReportTitle as some titles were imported from STG without Unicode
                    Value = x.ID.ToString()
                });

            var pfTitleListItem = pfList.Select(x => new SelectListItem()
            {
                Selected = project.TitlePFId.Equals(x.ID),
                Text = (x.Title.Length > 100 ? x.Title.Substring(0, 100) : x.Title),
                Value = x.ID.ToString()
            });

            var pfRNameListItem = pfList.Select(x => new SelectListItem()
            {
                Selected = project.RespondentNamePFId.Equals(x.ID),
                Text = (x.Title.Length > 100 ? x.Title.Substring(0, 100) : x.Title),
                Value = x.ID.ToString()
            });

            var pfRMobileListItem = pfList.Select(x => new SelectListItem()
            {
                Selected = project.RespondentMobilePFId.Equals(x.ID),
                Text = (x.Title.Length > 100 ? x.Title.Substring(0, 100) : x.Title),
                Value = x.ID.ToString()
            });

            ViewBag.CategoryPFId = pfCategoryListItem;
            ViewBag.TitlePFId = pfTitleListItem;
            ViewBag.RespondentNamePFId = pfRNameListItem;
            ViewBag.RespondentMobilePFId = pfRMobileListItem;

            return View(project);
        }

        // POST: /Project/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Guid,QCRequirement,questions_list,fld_respondent_name,fld_respondent_phone1,fld_respondent_phone2,CreatedByUserId,CreatedOn,ModifiedByUserId,ModifiedOn,STGSync,Status,StatusChangedByUserId,StatusChangedOn,Type,CategoryPFId,TitlePFId,RespondentNamePFId,RespondentMobilePFId,NeedMapInput,NeedBuildingSearch,ActualStartDate,ActualEndDate,Visibility")] Project project)
        {
            if (ModelState.IsValid)
            {
                int uid = Convert.ToInt32(User.Identity.GetUserId());
                if (project.Status == null || project.Status == "Draft" || project.Status.Length == 0)
                {
                    project.Status = "D";
                }
                else
                {
                    project.Status = project.Status.Substring(0, 1);
                }
                if (project.Type == null || project.Type == "Survey" || project.Type.Length == 0)
                {
                    project.Type = "S";
                }
                else
                {
                    project.Type = project.Type.Substring(0, 1);
                }
                //if (project.Visibility == null || project.Visibility == "Only Interviewer can view" || project.Visibility.Length == 0)
                //{
                //    project.Visibility = "I";
                //}
                //else
                //{
                //    project.Visibility = project.Visibility.Substring(0, 1);
                //}
                project.StatusChangedByUserId = uid;
                project.StatusChangedOn = DateTime.Now;
                project.CreatedOn = (project.CreatedOn >= (DateTime)SqlDateTime.MinValue) ? project.CreatedOn : DateTime.Now;
                project.ModifiedOn = DateTime.Now;
                project.ModifiedByUserId = uid;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProjectOverview", "Designer", new { id = project.ID });
            }
            return View(project);
        }

        // GET: /Project/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: /Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("ProjectOverview", "Designer", new { id = id });
        }

        // GET: /Project/SurveyResults/5
        public ActionResult SurveyResults(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(db.Surveys.ToList());
        }

        public ActionResult jsProjectFieldSample(int? id)
        {
            string sql = "";

            sql = @"SELECT pfs.* FROM ProjectFieldSample pfs WHERE pfs.FieldID = " + id + " AND (pfs.ParentSampleID <> 0 OR pfs.ParentSampleID IS NULL) AND pfs.IsActive = 1 ORDER BY DisplayOrder";
            
            var query = db.Database.SqlQuery<ProjectFieldSample>(sql);
            List<ProjectFieldSample> data = query.ToList<ProjectFieldSample>();
            
            JavaScriptSerializer jss = new JavaScriptSerializer();
            ViewBag.jsOutput = jss.Serialize(data);
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = "Admin,CanEdit,Project Manager")]
        [HttpGet]
        public ActionResult Permissions(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            
            //Get Users
            string sql = @"SELECT r.Id AS RoleId, r.Name AS RoleName, u.Id AS UserId, u.username AS UserName, ISNULL(pur.id, 0) AS IsSelected
                        FROM DNAShared2.dbo.AppRoles ar INNER JOIN DNAShared2.dbo.AspNetRoles r ON ar.RoleId = r.Id 
                            INNER JOIN DNAShared2.dbo.AspNetUserRoles ur ON ur.RoleId = r.Id 
                            INNER JOIN DNAShared2.dbo.AspNetUsers u ON ur.UserId = u.Id 
							LEFT OUTER JOIN DNAShared2.dbo.UserRights pur ON pur.ObjectName = 'PROJECT' AND pur.ObjectValue = " + id + " AND pur.UserId = u.Id AND pur.RoleId = r.Id " + 
                        " WHERE ar.AppCode = 'DSO' ORDER BY u.username ";

            var dsUsers = db.Database.SqlQuery<ProjectUsers>(sql);
            ViewBag.ProjectUsers = dsUsers.ToList<ProjectUsers>();

            sql = @"SELECT r.Id, r.Name AS Title FROM DNAShared2.dbo.AppRoles ar INNER JOIN DNAShared2.dbo.AspNetRoles r ON ar.RoleId = r.Id 
                        WHERE ar.AppCode = 'DSO' ORDER BY ar.DisplayOrder ";
            var roles = db.Database.SqlQuery<SimpleList>(sql);
            ViewBag.Roles = roles.ToList<SimpleList>();

            return View(project);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Permissions(FormCollection form)
        {
            int id = Convert.ToInt32(form["ProjectID"].ToString());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Delete previous permissions
            string sql = @"DELETE FROM DNAShared2.dbo.UserRights WHERE ObjectName = 'PROJECT' AND ObjectValue = " + id;
            db.Database.ExecuteSqlCommand(sql);

            //Set new permissions
            sql = @"SELECT r.Id, r.Name AS Title FROM DNAShared2.dbo.AppRoles ar INNER JOIN DNAShared2.dbo.AspNetRoles r ON ar.RoleId = r.Id 
                        WHERE ar.AppCode = 'DSO' ORDER BY ar.DisplayOrder ";
            var roles = db.Database.SqlQuery<SimpleList>(sql);

            foreach (SimpleList r in roles.ToList<SimpleList>())
            {
                try
                {
                    if (form["role_" + r.Id] != null)
                    {
                        string selectedUsers = form["role_" + r.Id].ToString();
                        if (selectedUsers.Length > 0)
                        {
                            string[] UserRoles = selectedUsers.Split(',');
                            for (int i = 0; i < UserRoles.Length; i++)
                            {
                                sql = @"INSERT INTO DNAShared2.dbo.UserRights (ObjectName, ObjectValue, UserId, RoleId) " +
                                      " VALUES ('PROJECT', " + id + ", " + UserRoles[i] + ", " + r.Id + ")";
                                db.Database.ExecuteSqlCommand(sql);
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                }
            }

            return Permissions(id);
        }


        public ActionResult Copy(int id, string newProjectName)
        {
            Project sourcep = db.Projects.AsNoTracking()
                             .FirstOrDefault(e => e.ID == id);
            sourcep.Guid = System.Guid.NewGuid().ToString();
            sourcep.Status = "D";
            sourcep.Name = newProjectName;
            Project destp = db.Projects.Add(sourcep);
            db.SaveChanges();

            int newpid = destp.ID;
            Dictionary<int, int> prjSections = new Dictionary<int, int>();
            Dictionary<int, int> prjFields = new Dictionary<int, int>();
            Dictionary<int, int> prjFieldSamples = new Dictionary<int, int>();
            Dictionary<int, int> prjFieldMedia = new Dictionary<int, int>();
            Dictionary<int, int> prjSampleMedia = new Dictionary<int, int>();

            List<ProjectFieldSection> pfn = db.ProjectFieldSection.Where(x => x.ProjectID == id).ToList<ProjectFieldSection>();
            foreach (var newPfn in pfn)
            {
                int oid = newPfn.ID;
                newPfn.ProjectID = newpid;
                db.ProjectFieldSection.Add(newPfn);
                db.SaveChanges();
                prjSections.Add(oid, newPfn.ID);
            }
            List<ProjectField> opf = db.ProjectField.Where(x => x.ProjectID == id).ToList<ProjectField>();
            foreach (var newPf in opf)
            {
                int oid = newPf.ID;
                newPf.ProjectID = newpid;
                if (prjSections.ContainsKey((int)newPf.SectionID))
                {
                    newPf.SectionID = prjSections[(int)newPf.SectionID];
                }
                db.ProjectField.Add(newPf);
                db.SaveChanges();
                prjFields.Add(oid, newPf.ID);
            }

            opf = db.ProjectField.Where(x => x.ProjectID == id).ToList<ProjectField>();
            List<ProjectField> pf = db.ProjectField.Where(x => x.ProjectID == newpid && (!String.IsNullOrEmpty(x.ScriptOnEntry) || !String.IsNullOrEmpty(x.ScriptOnValidate) || !String.IsNullOrEmpty(x.ScriptOnExit))).ToList<ProjectField>();
            foreach (var newPf in pf)
            {
                //var oid = prjFields.Where(x => x.Value == (int)newPf.ID).FirstOrDefault().Key;

                foreach (var oldPf in opf)
                {
                    if (newPf.ScriptOnEntry.IndexOf("'" + oldPf.ID + "'") > 0)
                    {
                        newPf.ScriptOnEntry = newPf.ScriptOnEntry.Replace("'" + oldPf.ID + "'", "'" + prjFields[(int)oldPf.ID] + "'");
                    }
                    if (newPf.ScriptOnEntry.IndexOf("\"" + oldPf.ID + "\"") > 0)
                    {
                        newPf.ScriptOnEntry = newPf.ScriptOnEntry.Replace("\"" + oldPf.ID + "\"", "\"" + prjFields[(int)oldPf.ID] + "\"");
                    }
                    if (newPf.ScriptOnValidate.IndexOf("'" + oldPf.ID + "'") > 0)
                    {
                        newPf.ScriptOnValidate = newPf.ScriptOnValidate.Replace("'" + oldPf.ID + "'", "'" + prjFields[(int)oldPf.ID] + "'");
                    }
                    if (newPf.ScriptOnValidate.IndexOf("\"" + oldPf.ID + "\"") > 0)
                    {
                        newPf.ScriptOnValidate = newPf.ScriptOnValidate.Replace("\"" + oldPf.ID + "\"", "\"" + prjFields[(int)oldPf.ID] + "\"");
                    }
                    if (newPf.ScriptOnExit.IndexOf("'" + oldPf.ID + "'") > 0)
                    {
                        newPf.ScriptOnExit = newPf.ScriptOnExit.Replace("'" + oldPf.ID + "'", "'" + prjFields[(int)oldPf.ID] + "'");
                    }
                    if (newPf.ScriptOnExit.IndexOf("\"" + oldPf.ID + "\"") > 0)
                    {
                        newPf.ScriptOnExit = newPf.ScriptOnExit.Replace("\"" + oldPf.ID + "\"", "\"" + prjFields[(int)oldPf.ID] + "\"");
                    }
                }
            }
            db.SaveChanges();

            List<ProjectFieldMediaFile> pfmf = db.ProjectFieldMediaFile
                .SqlQuery("SELECT * FROM ProjectFieldMediaFile WHERE FieldID IN (SELECT ID FROM ProjectField WHERE ProjectID = " + id + ")")
                .ToList<ProjectFieldMediaFile>();
            foreach (var newPfmf in pfmf)
            {
                int oid = newPfmf.ID;
                if (prjFields.ContainsKey((int)newPfmf.FieldID))
                {
                    newPfmf.FieldID = prjFields[(int)newPfmf.FieldID];
                }
                db.ProjectFieldMediaFile.Add(newPfmf);
                db.SaveChanges();
                prjFieldMedia.Add(oid, newPfmf.ID);
            }
            List<ProjectFieldSample> pfs = db.ProjectFieldSample
                .SqlQuery("SELECT * FROM ProjectFieldSample WHERE FieldID IN (SELECT ID FROM ProjectField WHERE ProjectID = " + id + ")")
                .ToList<ProjectFieldSample>();
            foreach (var newPfs in pfs)
            {
                int oid = newPfs.ID;
                if (prjFields.ContainsKey((int)newPfs.FieldID))
                {
                    newPfs.FieldID = prjFields[(int)newPfs.FieldID];
                }
                db.ProjectFieldSample.Add(newPfs);
                db.SaveChanges();
                prjFieldSamples.Add(oid, newPfs.ID);
            }
            List<ProjectFieldSampleMediaFile> pfsmf = db.ProjectFieldSampleMediaFile
                .SqlQuery("SELECT * FROM ProjectFieldSampleMediaFile WHERE FieldSampleID IN (SELECT ID FROM ProjectFieldSample WHERE FieldID IN (SELECT ID FROM ProjectField WHERE ProjectID = " + id + "))")
                .ToList<ProjectFieldSampleMediaFile>();
            foreach (var newPfsmf in pfsmf)
            {
                int oid = newPfsmf.ID;
                if (prjFieldSamples.ContainsKey((int)newPfsmf.FieldSampleID))
                {
                    newPfsmf.FieldSampleID = prjFieldSamples[(int)newPfsmf.FieldSampleID];
                }
                db.ProjectFieldSampleMediaFile.Add(newPfsmf);
                db.SaveChanges();
                prjSampleMedia.Add(oid, newPfsmf.ID);
            }

            return View(destp);
        }

    }
}
