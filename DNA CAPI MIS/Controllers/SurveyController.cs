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
using System.Data.SqlClient;
using System.Configuration;
using System.Device.Location;
using System.Collections;
using System.Data.Common;
using System.IO;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.Owin;
using ClosedXML;
using ClosedXML.Excel;

namespace DNA_CAPI_MIS.Controllers
{
    public class SurveyController : Controller
    {
        private ProjectContext db = new ProjectContext();

        private static string username = "";
        IdentityDbContext idenDB = new IdentityDbContext();
        private static List<SurveyDetailsAllDealer> SurveyDealerAll = new List<SurveyDetailsAllDealer>();


        public SurveyController()
        {
        }

        public SurveyController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;


        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        // GET: /Survey/Index/{project_id}
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //db.Database.ExecuteSqlCommand("sp_CAPI_Import");

            string sql = "SELECT Id, Name FROM Project WHERE Id = " + id;
            var project = db.Database.SqlQuery<ProjectView>(sql);
            ViewBag.ProjectName = project.FirstOrDefault<ProjectView>().Name;
            ViewBag.ProjectId = id;

            string language = "en-US";
            if (Request.QueryString.Get("lang") != null)
            {
                language = Request.QueryString.Get("lang");
            }
            else
            {
                language = "en-US";
            }

            sql = @"SELECT s.sbjnum, s.Created, s.Longitude, s.Latitude, s.SurveyorName, sd.FieldId, pf.FieldType, 
                               CASE WHEN t.Text IS NULL 
                                    THEN CASE WHEN pf.ReportTitle IS NULL THEN pf.Title ELSE pf.ReportTitle END 
                                    ELSE t.Text END AS Title, sd.FieldValue
                           FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum 
                           INNER JOIN ProjectField pf ON sd.FieldId = pf.ID 
                           LEFT OUTER JOIN Translation t ON t.EntityName = 'PROJECTFIELD' AND t.FieldName = 'TITLE' AND t.Language = '" + language + "' " +
                           "    AND pf.ID = (CASE WHEN ISNUMERIC(t.KeyValue) = 1 THEN CAST(t.KeyValue as int) ELSE 0 END) " +
                           "WHERE ISNULL(s.Version, 0) = 0 AND ISNULL(s.Test, 0) = 0 AND s.ProjectID = " + id + (id.Value == 7113 ? "" : " AND s.OpStatus = 1 ") +
                           " ORDER BY s.Created desc, s.sbjnum, pf.DisplayOrder";

            var surveys = db.Database.SqlQuery<SurveyDataView>(sql);

            return View(surveys.ToList<SurveyDataView>());
        }

        public ActionResult RepCensusProgressSW(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string sql = "SELECT Id, Name FROM Project WHERE Id = " + id;
            var project = db.Database.SqlQuery<ProjectView>(sql);
            ViewBag.ProjectName = project.FirstOrDefault<ProjectView>().Name;
            ViewBag.ProjectId = id;

            string language = "en-US";
            if (Request.QueryString.Get("lang") != null)
            {
                language = Request.QueryString.Get("lang");
            }
            else
            {
                language = "en-US";
            }

            sql = string.Format(@"SELECT POICount, SurveyorName, DistrictId, DistrictName, CityName, DistanceMax,
                ROUND(ISNULL((SELECT SUM(distance) FROM TrackUserRoute tur WHERE tur.DistrictId = poi.DistrictId
	                AND tur.ForDate >= ISNULL(p.ActualStartDate, p.CreatedOn) AND tur.ForDate <= ISNULL(p.ActualEndDate, '20990101')
	                AND tur.username = poi.SurveyorName
                    )/1000,0), 0) AS DistanceMade,
                (SELECT COUNT(*) FROM Survey s1 WHERE ISNULL(s1.Version, 0) = 0 AND ISNULL(s1.Test, 0) = 0 AND poi.SurveyorName = s1.SurveyorName AND p.Id = s1.ProjectId AND poi.DistrictId = s1.DistrictId AND s1.OpStatus = 0) AS OpCancelled,
                (SELECT COUNT(*) FROM Survey s2 WHERE ISNULL(s2.Version, 0) = 0 AND ISNULL(s2.Test, 0) = 0 AND poi.SurveyorName = s2.SurveyorName AND p.Id = s2.ProjectId AND poi.DistrictId = s2.DistrictId AND s2.OpStatus = 1 AND s2.QcStatus = 0) AS QCCancelled
            FROM (
	            SELECT s.ProjectID, COUNT(s.sbjnum) AS POICount, s.SurveyorName, s.DistrictId, d.Name AS DistrictName, c.Name AS CityName, ISNULL(d.DistanceMax, 0) AS DistanceMax
	            FROM Survey s 
                INNER JOIN District d ON d.Id = s.DistrictId
	            INNER JOIN City c ON c.Id = s.CityId
	            WHERE ISNULL(s.Version, 0) = 0 AND ISNULL(s.Test, 0) = 0 AND s.ProjectID = {0}
	            GROUP BY s.ProjectID, s.SurveyorName, s.DistrictId, d.Name, c.Name, d.DistanceMax) AS poi
            INNER JOIN Project p ON p.ID = poi.ProjectId
            ORDER BY SurveyorName, DistrictName
            ", id);

            var surveys = db.Database.SqlQuery<RepCensusProgressSW>(sql);

            return View(surveys.ToList<RepCensusProgressSW>());
        }

        // GET: /Survey/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Survey survey = db.Surveys.Find(id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            return View(survey);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = "Admin,CanEdit,Field Supervisor,Project Manager")]
        public ActionResult SurveyApproval(int? id, string language)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            db.Database.CommandTimeout = 0;

            //db.Database.ExecuteSqlCommand("sp_CAPI_Import");

            //string language = "en-US";
            if (language != null && language.Length == 0)
            {
                if (Request.QueryString.Get("lang") != null)
                {
                    language = Request.QueryString.Get("lang");
                }
                else
                {
                    language = "en-US";
                }
            }

            string fromDate = "", toDate = "";
            if (Request.QueryString.Get("fromdate") != null)
            {
                fromDate = Request.QueryString.Get("fromdate");
            }
            if (Request.QueryString.Get("todate") != null)
            {
                toDate = Request.QueryString.Get("todate");
            }
            string surveyors = "";
            if (Request.QueryString.Get("surveyors") != null)
            {
                surveyors = Request.QueryString.Get("surveyors");
            }
            string cities = "";
            if (Request.QueryString.Get("city") != null)
            {
                cities = Request.QueryString.Get("city");
            }
            string districts = "";
            if (Request.QueryString.Get("district") != null)
            {
                districts = Request.QueryString.Get("district");
            }

            string reportTitle = "0";
            if (Request.QueryString.Get("rt") != null)
            {
                reportTitle = Request.QueryString.Get("rt");
            }
            ViewBag.ReportTitleAs = reportTitle;

            string fieldValue = "0";
            if (Request.QueryString.Get("fv") != null)
            {
                fieldValue = Request.QueryString.Get("fv");
            }
            ViewBag.FieldValueAs = fieldValue;

            string reportId = "1";
            if (Request.QueryString.Get("r") != null)
            {
                reportId = Request.QueryString.Get("r");
            }

            string astatus = "";
            string OPStatus = "";                                       //Unfiltered by default
            if (Request.QueryString.Get("astatus") != null)
            {
                astatus = Request.QueryString.Get("astatus");
            }
            switch (astatus)
            {
                case "0": OPStatus = " AND s.OpStatus IS NULL "; break; //Unchecked
                case "1": OPStatus = " AND s.OpStatus = 1 "; break;     //Approved
                case "2": OPStatus = " AND s.OpStatus = 0 "; break;     //Rejected
            }

            string sql = "SELECT * FROM Project WHERE Id = " + id;
            var project = db.Database.SqlQuery<Project>(sql);
            Project p = project.FirstOrDefault<Project>();
            ViewBag.Project = p;
            ViewBag.CategoryPFId = p.CategoryPFId;
            ViewBag.TitlePFId = p.TitlePFId;

            string sysPFsOnly = "";
            if (reportId == "2")
            {
                if (p.CategoryPFId != null && p.CategoryPFId > 0) sysPFsOnly += "sd.FieldId = " + p.CategoryPFId;
                if (p.TitlePFId != null && p.TitlePFId > 0) sysPFsOnly += (sysPFsOnly.Length > 0 ? " OR " : "") + "sd.FieldId = " + p.TitlePFId;

                if (sysPFsOnly.Length > 0)
                {
                    sysPFsOnly = " AND (" + sysPFsOnly + ") ";
                }
            }
            sql = @"SELECT CASE WHEN s.OpStatus IS NULL THEN 'Unchecked' ELSE CASE WHEN s.OpStatus = 1 THEN 'Yes' ELSE 'No' END END AS OpStatus, 
                                  CASE WHEN s.QCStatus IS NULL THEN 'Unchecked' ELSE CASE WHEN s.QCStatus = 1 THEN 'Yes' ELSE 'No' END END AS QCStatus, 
                                  ISNULL(cy.Name, '') AS CountryName, ISNULL(ct.Name, '') AS CityName, ISNULL(dt.Name, '') AS DistrictName, s.sbjnum, s.Created, s.Longitude, s.Latitude, s.SurveyorName, sd.FieldId, isnull(pf.FieldType,'') FieldType, 
                                  CASE WHEN t.Text IS NULL THEN pf.Title ELSE t.Text END AS Title, pf.ReportTitle, " + (fieldValue == "0" ? "isnull(sd.FieldValue,'')" : "isnull(STR(sd.ResponseTime),'')") + @" AS FieldValue,
                                  s.DeviceSurveyId, s.DeviceId, s.AppVersion,isnull(sl.CITY_GPS,0) CITY_GPS, sl.*, ISNULL(STGSurvey.sbjnum, 0) AS STG_sbjnum
                           FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum " + (reportId == "2" ? sysPFsOnly : "") + @"
                           LEFT OUTER JOIN STGSurvey ON s.sbjnum = STGSurvey.SurveyId
                           LEFT OUTER JOIN SurveyLocation sl ON sl.sbjnum = s.sbjnum
                           LEFT OUTER JOIN ProjectField pf ON sd.FieldId = pf.ID 
                           LEFT OUTER JOIN ProjectFieldSection pfn ON pf.SectionId = pfn.id
                           LEFT OUTER JOIN Translation t ON t.EntityName = 'PROJECTFIELD' AND t.FieldName = 'TITLE' AND t.Language = '" + language + @"' 
                               AND pf.ID = (CASE WHEN ISNUMERIC(t.KeyValue) = 1 THEN CAST(t.KeyValue as int) ELSE 0 END) 
	                       LEFT OUTER JOIN City ct ON ct.ID = s.CityID
	                       LEFT OUTER JOIN District dt on DT.ID = s.DistrictID
	                       LEFT OUTER JOIN Country cy ON ct.CountryID = cy.ID 
                           WHERE ISNULL(s.Version, 0) = 0 AND ISNULL(s.Test, 0) = 0 AND s.ProjectID = " + id + OPStatus +
                           (fromDate.Length > 0 && toDate.Length > 0 ? " AND s.Created >= '" + fromDate + "' AND s.Created <= '" + toDate + "'" : "") +
                           (surveyors.Length > 0 ? " AND s.SurveyorName IN (" + surveyors + ")" : "") +
                           (cities.Length > 0 ? " AND s.CityId IN (" + cities + ")" : "") +
                           (districts.Length > 0 ? " AND s.DistrictId IN (" + districts + ")" : "") +
                           (id == 4902 ? " AND s.sbjnum NOT IN (SELECT sbjnum FROM SurveyData WHERE FieldId = 7106 AND FieldValue NOT IN ('1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30'))" : "") +
                           " ORDER BY s.sbjnum, cy.Name, ct.Name, dt.Name, s.Created desc, ISNULL(pfn.DisplayOrder, 0), pf.DisplayOrder";
            //            string sql = @"SELECT top 1000 CASE WHEN s.OpStatus IS NULL THEN 'Unchecked' ELSE CASE WHEN s.OpStatus = 1 THEN 'Yes' ELSE 'No' END END AS OpStatus, 
            //                                  CASE WHEN s.QCStatus IS NULL THEN 'Unchecked' ELSE CASE WHEN s.QCStatus = 1 THEN 'Yes' ELSE 'No' END END AS QCStatus, 
            //                                  cy.Name AS CountryName, ct.Name AS CityName, dt.Name AS DistrictName, s.sbjnum, s.Created, s.Longitude, s.Latitude, s.SurveyorName, sd.FieldId, pf.FieldType, 
            //                                  CASE WHEN pf.VariableName IS NULL THEN CASE WHEN t.Text IS NULL THEN pf.Title ELSE t.Text END ELSE pf.VariableName END AS Title, sd.FieldValue,
            //                                  s.DeviceSurveyId, s.DeviceId, s.AppVersion, sl.*
            //                           FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum 
            //                           LEFT OUTER JOIN SurveyLocation sl ON sl.sbjnum = s.sbjnum
            //                           INNER JOIN ProjectField pf ON sd.FieldId = pf.ID 
            //                           LEFT OUTER JOIN Translation t ON t.EntityName = 'PROJECTFIELD' AND t.FieldName = 'TITLE' AND t.Language = '" + language + @"' 
            //                               AND pf.ID = (CASE WHEN ISNUMERIC(t.KeyValue) = 1 THEN CAST(t.KeyValue as int) ELSE 0 END) 
            //	                       INNER JOIN City ct ON ct.ID = s.CityID
            //	                       LEFT OUTER JOIN District dt on DT.ID = s.DistrictID
            //	                       INNER JOIN Country cy ON ct.CountryID = cy.ID 
            //                           WHERE ISNULL(s.Test, 0) = 0 AND s.ProjectID = " + id + OPStatus +
            //                           (fromDate.Length > 0 && toDate.Length > 0 ? " AND s.Created >= '" + fromDate + "' AND s.Created <= '" + toDate + "'" : "") +
            //                           (surveyors.Length > 0 ? " AND s.SurveyorName IN (" + surveyors + ")" : "") +
            //                           " ORDER BY s.sbjnum, cy.Name, ct.Name, dt.Name, s.Created desc, pf.DisplayOrder";

            var surveys = db.Database.SqlQuery<SurveyDataView>(sql);
            List<SurveyDataView> data = surveys.ToList<SurveyDataView>();
            foreach (var item in data)
            {
                if(item.ReportTitle == null)
                {
                    if(item.Title != null)
                    {
                        item.ReportTitle = item.Title;
                    }
            
                }

                if(item.ReportTitle == null || item.ReportTitle == "")
                {
                    item.ReportTitle = "N/A";
                }
            }
            string viewName = "SurveyApproval";
            if (reportId == "2") viewName = "IdentifyPOIList";


            sql = @"select top 1 sbjnum from surveydata where sbjnum in (select sbjnum from survey s 
                    WHERE ISNULL(s.Version, 0) = 0 AND ISNULL(s.Test, 0) = 0 AND s.ProjectID = " + id + OPStatus +
                    (fromDate.Length > 0 && toDate.Length > 0 ? " AND s.Created >= '" + fromDate + "' AND s.Created <= '" + toDate + "'" : "") +
                    (surveyors.Length > 0 ? " AND s.SurveyorName IN (" + surveyors + ")" : "") +
                    (cities.Length > 0 ? " AND s.CityId IN (" + cities + ")" : "") +
                    (districts.Length > 0 ? " AND s.DistrictId IN (" + districts + ")" : "") +
                    (id == 4902 ? " AND s.sbjnum NOT IN (SELECT sbjnum FROM SurveyData WHERE FieldId = 7106 AND FieldValue NOT IN ('1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23', '24', '25', '26', '27', '28', '29', '30'))" : "") +
                    ") group by sbjnum order by count(*) desc";
            var queryMaxSurveyFields = db.Database.SqlQuery<int>(sql);
            int maxSurveyFieldsSurveyId = queryMaxSurveyFields.FirstOrDefault<int>();
            ViewBag.DefaultSurveyId = maxSurveyFieldsSurveyId;

            return View(viewName, data);
        }

        [Authorize(Roles = "Admin,CanEdit,QC,Project Manager")]
        public ActionResult SurveyQC(int? id, string language)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //string language = "en-US";
            if (language != null && language.Length == 0)
            {
                if (Request.QueryString.Get("lang") != null)
                {
                    language = Request.QueryString.Get("lang");
                }
                else
                {
                    language = "en-US";
                }
            }
            string surveyors = "";
            if (Request.QueryString.Get("surveyors") != null)
            {
                surveyors = Request.QueryString.Get("surveyors");
            }
            string astatus = "";
            string QCStatus = "";                                       //Unfiltered by default
            if (Request.QueryString.Get("astatus") != null)
            {
                astatus = Request.QueryString.Get("astatus");
            }
            switch (astatus)
            {
                case "0": QCStatus = " AND s.QCStatus IS NULL "; break; //Unchecked
                case "1": QCStatus = " AND s.QCStatus = 1 "; break;     //Approved
                case "2": QCStatus = " AND s.QCStatus = 0 "; break;     //Rejected
            }
            string sql = @"SELECT CASE WHEN s.OpStatus IS NULL THEN 'Unchecked' ELSE CASE WHEN s.OpStatus = 1 THEN 'Yes' ELSE 'No' END END AS OpStatus, 
                                  CASE WHEN s.QCStatus IS NULL THEN 'Unchecked' ELSE CASE WHEN s.QCStatus = 1 THEN 'Yes' ELSE 'No' END END AS QCStatus, 
                                  ISNULL(s.QCRejectReasonId, 0) AS QCRejectReasonId, s.QCRemarks, REPLACE(u.UserName, '@dna.com.sa', '') AS LastQCByUserName, s.LastQCOn,
                                  cy.Name AS CountryName, ct.Name AS CityName, dt.Name AS DistrictName, s.sbjnum, s.Created, s.Longitude, s.Latitude, s.SurveyorName, sd.FieldId, pf.FieldType, 
                                  CASE WHEN pf.VariableName IS NULL THEN CASE WHEN t.Text IS NULL THEN pf.Title ELSE t.Text END ELSE pf.VariableName END AS Title, sd.FieldValue,
                                  s.DeviceSurveyId, s.DeviceId, s.AppVersion, sl.*
                           FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum 
                           LEFT OUTER JOIN SurveyLocation sl ON sl.sbjnum = s.sbjnum
                           INNER JOIN ProjectField pf ON sd.FieldId = pf.ID 
                           LEFT OUTER JOIN Translation t ON t.EntityName = 'PROJECTFIELD' AND t.FieldName = 'TITLE' AND t.Language = '" + language + @"' 
                               AND pf.ID = (CASE WHEN ISNUMERIC(t.KeyValue) = 1 THEN CAST(t.KeyValue as int) ELSE 0 END) 
	                       LEFT OUTER JOIN City ct ON ct.ID = s.CityID
	                       LEFT OUTER JOIN District dt on DT.ID = s.DistrictID
	                       LEFT OUTER JOIN Country cy ON ct.CountryID = cy.ID 
                           LEFT OUTER JOIN [DNAShared2].[dbo].[AspNetUsers] u ON u.Id = s.LastQCByUserId
                           WHERE ISNULL(s.Version, 0) = 0 AND ISNULL(s.Test, 0) = 0 AND s.ProjectID = " + id + " AND s.OpStatus = 1 " + QCStatus +
                           (surveyors.Length > 0 ? " AND s.SurveyorName IN (" + surveyors + ")" : "") +
                           " ORDER BY s.sbjnum, cy.Name, ct.Name, dt.Name, s.Created desc, pf.DisplayOrder";

            var surveys = db.Database.SqlQuery<SurveyDataView>(sql);
            List<SurveyDataView> data = surveys.ToList<SurveyDataView>();

            sql = "SELECT Id, Name FROM Project WHERE Id = " + id;
            var project = db.Database.SqlQuery<ProjectView>(sql);
            ViewBag.ProjectName = project.FirstOrDefault<ProjectView>().Name;
            ViewBag.ProjectId = id;

            return View(data);
        }

        [Authorize(Roles = "Admin,CanEdit,Field Supervisor,Project Manager")]
        [HttpPost]
        public ActionResult SurveyApprovalApprove(int? id, int OpStatus, string SurveyIDs)
        {
            if (id != null && id > 0 && OpStatus >= 0 && SurveyIDs != null)
            {
                string[] IDs = SurveyIDs.Split(',');
                if (IDs.Length > 0)
                {
                    string sql = "";
                    if (OpStatus == 2) //Test
                    {
                        sql = @"UPDATE Survey SET ProjectID = " + id + ", Test = 1, OpStatus = 0 WHERE sbjnum In (" + SurveyIDs + ")";
                    }
                    else
                    {
                        sql = @"UPDATE Survey SET ProjectID = " + id + ", OpStatus = " + OpStatus + " WHERE sbjnum In (" + SurveyIDs + ")";
                    }

                    int recordsUpdated = db.Database.ExecuteSqlCommand(sql);
                    if (recordsUpdated == 0)
                    {
                        return HttpNotFound();
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        [Authorize(Roles = "Admin,CanEdit,Field Supervisor,Project Manager")]
        [HttpPost]
        public ActionResult CopySurveys(int? id, int TargetProject, string SurveyIDs)
        {
            if (id != null && id > 0 && TargetProject >= 0 && SurveyIDs != null)
            {
                var sp = db.Projects.Find(id);
                var tp = db.Projects.Find(TargetProject);

                string[] IDs = SurveyIDs.Split(',');
                foreach (string sid in IDs)
                {
                    string sql = string.Format(@"INSERT INTO Survey (SourceSurveyId, ProjectId, Created, Test, Longitude, Latitude, SurveyorName, OpStatus, QCStatus, DeviceId, DeviceTimestamp, DeviceTimestampUpload, VersionForId, DeviceSurveyId, AppVersion, CityId, CountryId, DistrictId)
                    OUTPUT INSERTED.sbjnum 
                    SELECT sbjnum AS SourceSurveyId, {0} AS ProjectId, Created, Test, Longitude, Latitude, SurveyorName, OpStatus, QCStatus, DeviceId, DeviceTimestamp, DeviceTimestampUpload, VersionForId, DeviceSurveyId, AppVersion, CityId, CountryId, DistrictId 
                    FROM Survey WHERE sbjnum = {1} AND sbjnum NOT IN 
                    (SELECT DISTINCT SourceSurveyId FROM Survey where ProjectId = {0} AND SourceSurveyId IS NOT NULL)",
                    TargetProject, sid);

                    db.Database.CommandTimeout = 300;
                    int new_sid = db.Database.SqlQuery<int>(sql).FirstOrDefault();

                    if (new_sid > 0)
                    {
                        sql = string.Format(@"INSERT INTO SurveyLocation (sbjnum, time_spent_for_location, cellid, CITY_GPS, DISTRICT_GPS, DISTRICT_AUDITOR, BUILDING_NUMBER_AUDITOR, BUILDING_TYPE_DROPDOWN, STREET_NAME_GPS, STREET_NAME_DROPDOWN, STREET_NAME_AUDITOR, ZIPCODE_GPS, ADDITIONAL_CODE_GPS, LANDMARK_GPS, LANDMARK_AUDITOR, STC_SHEET_AUDITOR, MOBILY_SHEET_AUDITOR)
                        SELECT {0} as sbjnum, time_spent_for_location, cellid, CITY_GPS, DISTRICT_GPS, DISTRICT_AUDITOR, BUILDING_NUMBER_AUDITOR, BUILDING_TYPE_DROPDOWN, STREET_NAME_GPS, STREET_NAME_DROPDOWN, STREET_NAME_AUDITOR, ZIPCODE_GPS, ADDITIONAL_CODE_GPS, LANDMARK_GPS, LANDMARK_AUDITOR, STC_SHEET_AUDITOR, MOBILY_SHEET_AUDITOR FROM SurveyLocation 
                        WHERE sbjnum = {1}",
                        new_sid, sid);
                        db.Database.ExecuteSqlCommand(sql);

                        AddMappedField(sp.CategoryPFId, tp.CategoryPFId, new_sid, sid, db);
                        AddMappedField(sp.TitlePFId, tp.TitlePFId, new_sid, sid, db);
                        AddMappedField(sp.RespondentNamePFId, tp.RespondentNamePFId, new_sid, sid, db);
                        AddMappedField(sp.RespondentMobilePFId, tp.RespondentMobilePFId, new_sid, sid, db);
                    }
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        private void AddMappedField(int? srcField, int? dstField, int new_sid, string sid, ProjectContext ctx)
        {
            if (srcField != null && srcField > 0)
            {
                if (dstField != null && dstField > 0)
                {
                    string sql = string.Format(@"INSERT INTO SurveyData (sbjnum, FieldId, FieldValue, ResponseTime)
                                SELECT {0} AS sbjnum, {1} AS FieldId, FieldValue, ResponseTime FROM SurveyData
                                WHERE sbjnum = {2} AND FieldId = {3}", new_sid, dstField, sid, srcField);
                    ctx.Database.ExecuteSqlCommand(sql);
                }
            }
        }

        public class QCApprovedSurveys
        {
            public int QCStatus { get; set; }
            public int SurveyId { get; set; }
            public int QCRejectReasonId { get; set; }
            public string QCRemarks { get; set; }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,CanEdit,Field Supervisor,Project Manager")]
        public ActionResult SurveyQCApprove(int? id, List<QCApprovedSurveys> QCSurveys)
        {
            //Stream req = Request.InputStream;
            //req.Seek(0, System.IO.SeekOrigin.Begin);
            //string json = new StreamReader(req).ReadToEnd();
            //return View();
            if (id != null && id > 0)
            {
                if (QCSurveys.Count() > 0)
                {
                    //MembershipUserCollection CurrentUser = Membership.FindUsersByName(User.Identity.Name);

                    foreach (QCApprovedSurveys s in QCSurveys)
                    {
                        string sql = @"UPDATE Survey SET ProjectID = " + id + ", QCStatus = " + s.QCStatus +
                            ", QCRejectReasonId = " + s.QCRejectReasonId + ", QCRemarks = '" + s.QCRemarks + "'" +
                            ", LastQCByUserId = " + User.Identity.GetUserId() + ", LastQCOn='" + DateTime.Now.ToString() + "'" +
                            " WHERE sbjnum = " + s.SurveyId + "";

                        int recordsUpdated = db.Database.ExecuteSqlCommand(sql);
                        if (recordsUpdated == 0)
                        {
                            return HttpNotFound();
                        }
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        [Authorize(Roles = "Admin,KSAManager,GCCManager,CanEdit,Field Supervisor,Project Manager,QC,Field Observer")]
        public ActionResult POIMap(int? id, string language, int? country, int? sid, string surveyor, string fromdate, string todate)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Survey CurrentSIdData = null;
            if (sid != null && sid > 0)
            {
                //--------------------------------------------------------------
                //Get record for Survey Id
                var CurrentSId = db.Database.SqlQuery<Survey>("SELECT s.* FROM Survey s WHERE s.sbjnum = " + sid);
                CurrentSIdData = CurrentSId.FirstOrDefault<Survey>();
            }
            ViewBag.CurrentSIdData = CurrentSIdData;
            ViewBag.ProjectId = id;
            return View(CurrentSIdData);
        }

        //[Authorize(Roles = "Admin,KSAManager,GCCManager,CanEdit,Field Supervisor,Project Manager,QC,Field Observer")]
        [AllowAnonymous]
        public ActionResult TrafficMap(int? id, string language, int? country, int? sid, string surveyor, string fromdate, string todate)
        {
            ViewBag.ProjectId = id;
            return View();
        }

        [Authorize(Roles = "Admin,CanEdit,Field Supervisor,Project Manager")]
        [HttpPost]
        public ActionResult SurveyApprovalModify(int? id)
        {
            if (id != null && id > 0)
            {
                string setValues = "";
                if (Request.QueryString.Get("longitude") != null)
                {
                    setValues += ", longitude = " + Request.QueryString.Get("longitude");
                }
                if (Request.QueryString.Get("latitude") != null)
                {
                    setValues += ", latitude = " + Request.QueryString.Get("latitude");
                }

                if (setValues.Length > 0)
                {
                    ////Save existing version of Survey, if user is editing record
                    //int OldSurveyId = 0;
                    //var sq = db.Database.SqlQuery<Survey>("SELECT * FROM Survey WHERE sbjnum = " + id);
                    //Survey s = sq.FirstOrDefault<Survey>();

                    //if (s != null)
                    //{
                    //                        if (Convert.ToInt32(s.VersionForId) == 0)
                    //                        {
                    //                            OldSurveyId = Convert.ToInt32(id);
                    //                        }
                    //                        else
                    //                        {
                    //                            OldSurveyId = Convert.ToInt32(s.VersionForId);
                    //                        }
                    //                        var sqc = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM Survey WHERE VersionForId = " + OldSurveyId);
                    //                        int vno = sqc.FirstOrDefault<int>();
                    //                        if (vno == 0) vno = 1;

                    //                        db.Database.ExecuteSqlCommand("UPDATE Survey SET Version = " + vno + ", VersionForId = " + OldSurveyId + " WHERE sbjnum = " + id);

                    //                        var qSid = db.Database.SqlQuery<int>(String.Format(@"INSERT INTO Survey (ProjectID,Created,Longitude,Latitude,SurveyorName,DistrictID,CountryID,CityID,Test,OpStatus,QCStatus,Version,DeviceId,DeviceTimestamp,VersionForId,DeviceTimestampUpload,QCRejectReasonId,QCRemarks,LastQCByUserId,LastQCOn,DeviceSurveyId,AppVersion,DeviceTimestampStart) 
                    //                            OUTPUT INSERTED.sbjnum
                    //                            SELECT ProjectID,getdate(),Longitude,Latitude,SurveyorName,DistrictID,CountryID,CityID,Test,OpStatus,QCStatus,NULL,DeviceId,DeviceTimestamp,NULL,DeviceTimestampUpload,QCRejectReasonId,QCRemarks,LastQCByUserId,LastQCOn,DeviceSurveyId,AppVersion,DeviceTimestampStart FROM Survey WHERE sbjnum = {0}", id));

                    //                        int newSId = qSid.FirstOrDefault<int>();

                    db.Database.ExecuteSqlCommand("UPDATE Survey SET " + setValues.Trim(',') + " WHERE sbjnum = " + id);
                    //                        db.Database.ExecuteSqlCommand(String.Format(@"INSERT INTO SurveyData (ProjectID,Created,Longitude,Latitude,SurveyorName,DistrictID,CountryID,CityID,Test,OpStatus,QCStatus,Version,DeviceId,DeviceTimestamp,VersionForId,DeviceTimestampUpload,QCRejectReasonId,QCRemarks,LastQCByUserId,LastQCOn,DeviceSurveyId,AppVersion,DeviceTimestampStart) 
                    //                            OUTPUT INSERTED.sbjnum
                    //                            SELECT ProjectID,getdate(),Longitude,Latitude,SurveyorName,DistrictID,CountryID,CityID,Test,OpStatus,QCStatus,NULL,DeviceId,DeviceTimestamp,NULL,DeviceTimestampUpload,QCRejectReasonId,QCRemarks,LastQCByUserId,LastQCOn,DeviceSurveyId,AppVersion,DeviceTimestampStart FROM Survey WHERE sbjnum = {0}", id));
                    //}

                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        [Authorize(Roles = "Admin,KSAManager,GCCManager,CanEdit,Field Supervisor,Project Manager,QC,Field Observer")]
        public ActionResult Map(int? id, string language, int? country, int? sid, string surveyor, string fromdate, string todate)
        {
            double citylat = 0.0;
            double citylong = 0.0;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (language != null && language.Length == 0)
            {
                if (Request.QueryString.Get("lang") != null)
                {
                    language = Request.QueryString.Get("lang");
                }
                else
                {
                    language = "en-US";
                }
            }

            Survey CurrentSIdData = null; ;
            if (sid != null && sid > 0)
            {
                //--------------------------------------------------------------
                //Get record for Survey Id
                var CurrentSId = db.Database.SqlQuery<Survey>("SELECT s.* FROM Survey s INNER JOIN City ON s.CityId = City.Id WHERE s.sbjnum = " + sid);
                CurrentSIdData = CurrentSId.FirstOrDefault<Survey>();
                country = CurrentSIdData.CountryID;
            }
            ViewBag.CurrentSIdData = CurrentSIdData;

            if (country == 0 || country == null)
            {
                country = 2;
                if (Request.QueryString.Get("cy") != null)
                {
                    country = Convert.ToInt32(Request.QueryString.Get("cy"));
                }
            }
            if (id == 0 || id == null)
            {
                id = 3390;
            }
            ViewBag.ProjectId = id;
            ViewBag.CountryId = country;

            //CHECK USER ROLE according to region
            //string UserId = User.Identity.GetUserId();

            if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {
                if (country == 2 && User.IsInRole("GCCManager"))
                {
                    ViewBag.ErrorMessage = "You are not authorized to view this page";
                    return View();
                }
                else if (country != 2 && User.IsInRole("KSAManager"))
                {
                    ViewBag.ErrorMessage = "You are not authorized to view this page";
                    return View();
                }
            }

            //--------------------------------------------------------------
            //Generate city names view for Menu
            var allCities = db.Database.SqlQuery<CityView>(@"
                SELECT City.id, City.name, Country.Id AS CountryId, Country.Name CountryName FROM City INNER JOIN Country ON City.CountryId = Country.Id 
                INNER JOIN rdxlocation rdx ON City.id = rdx.cityid AND rdx.ReportID IN (1,3) 
                WHERE Country.Id = " + country + " ORDER BY City.CountryId, City.Name");
            List<CityView> allCityData = allCities.ToList<CityView>();
            ViewBag.AllCitiesList = allCityData;

            //Generate default view
            string filter = "";
            string category = Request.QueryString.Get("cat");
            if (category != null && category.Length > 0)
            {
                filter = " AND SubCategoryID = " + category;
            }
            string surveyId = Request.QueryString.Get("sid");
            if (sid != null && sid > 0)
            {
                surveyId = sid.ToString();
            }
            if (surveyId != null && surveyId.Length > 0)
            {
                filter += " AND s.sbjnum = " + surveyId;
            }

            string sql = "";
            string city = Request.QueryString.Get("c");
            if (sid != null && sid > 0)
            {
                city = CurrentSIdData.CityID.ToString();
            }
            if (filter.Length == 0)
            {
                if (city == null || city.Length == 0)
                    city = "-1";
            }


            ViewBag.city = city;
            ViewBag.category = category;
            ViewBag.CurrentFilterText = "";
            ViewBag.CategorySummary = null;
            ViewBag.City_Longitude = 0;
            ViewBag.City_Latitude = 0;

            List<CityView> cityData;
            if (city != "-1" && city.Length > 0)
            {
                var cityName = db.Database.SqlQuery<CityView>("SELECT id, name, Longitude, Latitude, CountryID from City where ID = " + city);
                cityData = cityName.ToList<CityView>();
                if (cityData.Count > 0)
                {
                    CityView selCity = cityName.ToList<CityView>().FirstOrDefault();
                    ViewBag.CurrentFilterText += " » City: " + selCity.Name;
                    ViewBag.City_Longitude = selCity.Longitude;
                    ViewBag.City_Latitude = selCity.Latitude;
                    ViewBag.CountryID = selCity.CountryID;
                    citylat = Convert.ToDouble(selCity.Latitude);
                    citylong = Convert.ToDouble(selCity.Longitude);
                }

                //                string findCatName = "select pfs2.ID AS ID, CONCAT(CASE WHEN parentCat.Title = 'Default' THEN '' ELSE CONCAT(parentCat.Title, ' > ') END, pfs2.Title) AS NAME FROM " +
                //                  "  (select pf.ProjectID, ct.ID AS CityID, pfs.* from ProjectField pf " +
                //                  "      INNER JOIN ProjectFieldSample pfs ON pf.ID = pfs.FieldID " +
                //                  "      INNER JOIN Categories cat ON pf.CategoryId = cat.Id " +
                //                  "      INNER JOIN Country cn ON cat.CountryId = cn.ID INNER JOIN City ct ON cn.ID = ct.CountryID " +
                //                  "  WHERE ct.ID = " + city + " AND ParentFieldID = 1) AS parentCat " +
                //                  " INNER JOIN ProjectFieldSample pfs2 ON parentCat.ID = pfs2.ParentSampleID";


                //                if (category != null && category.Length > 0)
                //                {
                //                    var categoryName = db.Database.SqlQuery<SummaryView>(findCatName + " WHERE pfs2.ID = " + category);
                //                    ViewBag.CurrentCateogryName = categoryName.ToList<SummaryView>().FirstOrDefault().Name;
                //                    ViewBag.CurrentFilterText += " » Category: " + ViewBag.CurrentCateogryName;
                //                }
                //                sql = @"select SubCategoryID AS Id, SubCategoryName as Name, OutletCount, Color_Red, Color_Green, Color_Blue
                //                    FROM viewCensusCategoryCount WHERE CityID = " + city + " AND ProjectId = " + id + " ORDER BY SubCategoryName";

                //                var categories = db.Database.SqlQuery<SummaryView>(sql);
                //                db.Database.CommandTimeout = 300;
                //                ViewBag.CategorySummary = categories.ToList<SummaryView>();
            }


            if (city != "-1" && city.Length > 0)
            {

                sql = @"select surveyorname as Name, count(*) as [count] from survey where projectid = " + id + " and cityid = " + city + " group by surveyorname";

                var categories = db.Database.SqlQuery<Surveyorname>(sql);
                ViewBag.surveyorname = categories.ToList<Surveyorname>();
            }

            if (surveyor != null)
            {
                //sql = @"select * from viewCensusPOI where projectid = " + id + " and cityid = " + city + " and surveyorname = '" + surveyor + "'";
                sql = @"SELECT
                        CASE WHEN s.OpStatus IS NULL THEN 'Unchecked' ELSE CASE WHEN s.OpStatus = 1 THEN 'Yes' ELSE 'No' END END AS OpStatus, 
                        CASE WHEN s.QCStatus IS NULL THEN 'Unchecked' ELSE CASE WHEN s.QCStatus = 1 THEN 'Yes' ELSE 'No' END END AS QCStatus, 
                        cy.Name AS CountryName, ct.Name AS CityName, dt.Name AS DistrictName, s.*, sd.FieldId, pf.FieldType, 
                        CASE WHEN pf.VariableName IS NULL THEN CASE WHEN t.Text IS NULL THEN pf.Title ELSE t.Text END ELSE pf.VariableName END AS Title, sd.FieldValue
                        FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum 
                           INNER JOIN ProjectField pf ON sd.FieldId = pf.ID 
                           LEFT OUTER JOIN Translation t ON t.EntityName = 'PROJECTFIELD' AND t.FieldName = 'TITLE' AND t.Language = 'en' 
                               AND pf.ID = (CASE WHEN ISNUMERIC(t.KeyValue) = 1 THEN CAST(t.KeyValue as int) ELSE 0 END) 
	                       INNER JOIN City ct ON ct.ID = s.CityID
	                       LEFT OUTER JOIN District dt on DT.ID = s.DistrictID
	                       INNER JOIN Country cy ON ct.CountryID = cy.ID 
                    WHERE s.CityID = " + city + filter + " AND s.ProjectId = " + id +
                    " and surveyorname = '" + surveyor + "'" +
                    " ORDER BY s.sbjnum";

                var categories = db.Database.SqlQuery<SurveyDataView>(sql);
                ViewBag.SurveyorOwnOutlets = categories.ToList<SurveyDataView>();
            }

            //Outlet Sizes 
            //string datewhere = "CONVERT(VARCHAR(2),DATEPART(MONTH, ondatetime))   + '/'+ CONVERT(VARCHAR(2),DATEPART(DAY, ondatetime))" +
            //                  "+ '/' + (CONVERT(VARCHAR(4),DATEPART(YEAR, ondatetime)))";
            if (fromdate != null)
                fromdate = fromdate.Replace(" ", "");
            if (todate != null)
                todate = todate.Replace(" ", "");

            string datewhere = "CONVERT(VARCHAR(4),DATEPART(YEAR, DeviceDateTime))   + '-'+ CONVERT(VARCHAR(2),DATEPART(MONTH, DeviceDateTime))+ '-' + " +
                    "(CONVERT(VARCHAR(2),DATEPART(DAY, DeviceDateTime))) >= '" + fromdate + "' and  CONVERT(VARCHAR(4),DATEPART(YEAR, DeviceDateTime))   + '-'+ CONVERT(VARCHAR(2),DATEPART(MONTH, DeviceDateTime))+ '-' + " +
                    "(CONVERT(VARCHAR(2),DATEPART(DAY, DeviceDateTime))) <= '" + todate + "' ";

            ViewBag.ShowOutlets = false;
            if ((sid != null && sid > 0) || (city != "-1" && category != null && category.Length > 0))
            {
                sql = @"SELECT
                        CASE WHEN s.OpStatus IS NULL THEN 'Unchecked' ELSE CASE WHEN s.OpStatus = 1 THEN 'Yes' ELSE 'No' END END AS OpStatus, 
                        CASE WHEN s.QCStatus IS NULL THEN 'Unchecked' ELSE CASE WHEN s.QCStatus = 1 THEN 'Yes' ELSE 'No' END END AS QCStatus, 
                        cy.Name AS CountryName, ct.Name AS CityName, dt.Name AS DistrictName, s.*, sd.FieldId, pf.FieldType, 
                        CASE WHEN pf.VariableName IS NULL THEN CASE WHEN t.Text IS NULL THEN pf.Title ELSE t.Text END ELSE pf.VariableName END AS Title, sd.FieldValue
                        FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum 
                           INNER JOIN ProjectField pf ON sd.FieldId = pf.ID 
                           LEFT OUTER JOIN Translation t ON t.EntityName = 'PROJECTFIELD' AND t.FieldName = 'TITLE' AND t.Language = 'en' 
                               AND pf.ID = (CASE WHEN ISNUMERIC(t.KeyValue) = 1 THEN CAST(t.KeyValue as int) ELSE 0 END) 
	                       INNER JOIN City ct ON ct.ID = s.CityID
	                       LEFT OUTER JOIN District dt on DT.ID = s.DistrictID
	                       INNER JOIN Country cy ON ct.CountryID = cy.ID 
                    WHERE s.CityID = " + city + filter + " AND s.ProjectId = " + id + " ORDER BY s.sbjnum";

            }
            else if (surveyor != null && fromdate == null || todate == null)
            {
                sql = @"select distinct datepart(year, DeviceDateTime) yy, datepart(month, DeviceDateTime) mm, datepart(day, DeviceDateTime) dd, datepart(hour, DeviceDateTime) hh, datepart(minute, DeviceDateTime) mins, datepart(second, DeviceDateTime) ss," +
                      "Latitude,Longitude, 0 sbjnum, CONVERT(float, 0.00) distanceFromLastPoint from trackuser where isnull(accuracy, 0) < 40 and username = '" + surveyor + "' order by yy, mm, dd, hh, mins, ss";
                //sql = @"select distinct Latitude,Longitude, 0 sbjnum, CONVERT(float, 0.00) distanceFromLastPoint from trackuser where username = '" + surveyor + "' --order by devicedatetime";


            }
            else if (surveyor != null && fromdate != null && todate != null)
            {
                sql = @" select distinct  DeviceDateTime,datepart(year, DeviceDateTime) yy, datepart(month, DeviceDateTime) mm, datepart(day, DeviceDateTime) dd, " +
                    "datepart(hour, DeviceDateTime) hh, datepart(minute, DeviceDateTime) mins, datepart(second, DeviceDateTime) ss, Latitude,Longitude, 0 sbjnum, CONVERT(float, 0.00) distanceFromLastPoint from " +
                    "trackuser where username = '" + surveyor + "' and  " + datewhere +
                    " order by yy, mm, dd, hh, mins, ss";
            }
            else if (surveyor == null && fromdate != null && todate != null)
            {
                sql = @" select distinct  DeviceDateTime,datepart(year, DeviceDateTime) yy, datepart(month, DeviceDateTime) mm, datepart(day, DeviceDateTime) dd, " +
                   "datepart(hour, DeviceDateTime) hh, datepart(minute, DeviceDateTime) mins, datepart(second, DeviceDateTime) ss, Latitude,Longitude, 0 sbjnum, CONVERT(float, 0.00) distanceFromLastPoint from trackuser where isnull(accuracy, 0) < 40 and " + datewhere +
                   " order by yy, mm, dd, hh, mins, ss";
            }
            else
            {
                sql = @"select 0 sbjnum, 0 CityID, '' Created, 0 Longitude, 0 Latitude, '' SurveyorName, '' FieldId, '' FieldValue ,
                0 AS SubCategoryID, '' AS SubCategoryName WHERE 1=2";
            }

            var surveys = db.Database.SqlQuery<SurveyDataView>(sql);
            List<SurveyDataView> surveyData = surveys.ToList<SurveyDataView>();

            //Remove points outside of city
            FilterOutofCityPoints(surveyData, citylat, citylong);

            //Remove points that are near with each other
            //FilterOutofNearPoints(surveyData);

            db.Database.CommandTimeout = 300;

            return View(surveyData);

        }

        [Authorize(Roles = "Admin,KSAManager,GCCManager,CanEdit,Field Supervisor,Project Manager,QC,Field Observer")]
        public ActionResult Track(int? id, string language, int? c, int? country, string surveyor, string fromdate, string todate, string trackpointid, string noofpoints)
        {
            double citylat = 0.0;
            double citylong = 0.0;

            if (language != null && language.Length == 0)
            {
                if (Request.QueryString.Get("lang") != null)
                {
                    language = Request.QueryString.Get("lang");
                }
                else
                {
                    language = "en-US";
                }
            }

            if (country == null || country == 0)
            {
                country = 2;
            }
            ViewBag.CountryId = country;

            //--------------------------------------------------------------
            //Generate city names view for Menu
            var timeout = db.Database.CommandTimeout;
            db.Database.CommandTimeout = 600;

            var allCities = db.Database.SqlQuery<CityView>(@"
                SELECT City.id, City.name, Country.Id AS CountryId, Country.Name CountryName FROM City INNER JOIN Country ON City.CountryId = Country.Id 
                INNER JOIN rdxlocation rdx ON Country.id = rdx.CountryID AND City.Id = rdx.CityId AND rdx.ReportID = 1 
                ORDER BY City.CountryId, City.Name");
            List<CityView> allCityData = allCities.ToList<CityView>();
            ViewBag.AllCitiesList = allCityData;

            //Generate default view
            string sql = "";
            string city = Request.QueryString.Get("c");
            if (city == null || city.Length == 0)
                city = "-1";


            ViewBag.city = city;
            ViewBag.CurrentFilterText = "";
            ViewBag.City_Longitude = 0;
            ViewBag.City_Latitude = 0;

            List<CityView> cityData;
            if (city != "-1" && city.Length > 0)
            {
                var cityName = db.Database.SqlQuery<CityView>("SELECT id, name, Longitude, Latitude, CountryID from City where ID = " + city);
                cityData = cityName.ToList<CityView>();
                if (cityData.Count > 0)
                {
                    CityView selCity = cityName.ToList<CityView>().FirstOrDefault();
                    ViewBag.CurrentFilterText += " » City: " + selCity.Name;
                    ViewBag.City_Longitude = selCity.Longitude;
                    ViewBag.City_Latitude = selCity.Latitude;
                    ViewBag.CountryID = selCity.CountryID;
                    citylat = Convert.ToDouble(selCity.Latitude);
                    citylong = Convert.ToDouble(selCity.Longitude);
                }

                sql = "select distinct username as Name from trackuser order by username";

                var surveyornames = db.Database.SqlQuery<Surveyorname>(sql);
                ViewBag.surveyorname = surveyornames.ToList<Surveyorname>();
            }

            if (fromdate != null)
                fromdate = fromdate.Replace(" ", "");
            if (todate != null)
            {
                todate = todate.Replace(" ", "");
                DateTime todate2 = new DateTime(Convert.ToInt32(todate.Substring(0, 4)), Convert.ToInt32(todate.Substring(4, 2)), Convert.ToInt32(todate.Substring(6, 2)));
                todate2 = todate2.AddDays(1);
                todate = todate2.Year.ToString() + todate2.Month.ToString().PadLeft(2, '0') + todate2.Day.ToString().PadLeft(2, '0');
            }

            string trackpointidFilter = "";
            if (trackpointid != null && trackpointid.Length > 0)
            {
                trackpointidFilter = " AND (ID > " + (Convert.ToInt32(trackpointid) - Convert.ToInt32(noofpoints)) + " AND ID < " + (Convert.ToInt32(trackpointid) + Convert.ToInt32(noofpoints)) + ") ";
            }
            if (surveyor != null && fromdate == null || todate == null)
            {
                sql = @"select distinct TOP 500000 username AS SurveyorName, datepart(year, DeviceDateTime) yy, datepart(month, DeviceDateTime) mm, datepart(day, DeviceDateTime) dd, datepart(hour, DeviceDateTime) hh, datepart(minute, DeviceDateTime) mins, datepart(second, DeviceDateTime) ss," +
                      "Latitude,Longitude, 0 sbjnum, CONVERT(float, 0.00) distanceFromLastPoint from trackuser where username = '" + surveyor + "' and isnull(accuracy, 0) < 40 " +
                      trackpointidFilter +
                    //                      "  and DeviceDateTime >= '20150223' " +
                      "order by SurveyorName, yy, mm, dd, hh, mins, ss";
            }
            else if (surveyor != null && fromdate != null && todate != null)
            {
                //Including TrackPointID in the query would render the DISTINCT clause useless
                //Storing DeviceDateTime in place of TrackPointID to mark points ID
                sql = @" select distinct TOP 500000 username AS SurveyorName, DeviceDateTime,datepart(year, DeviceDateTime) yy, datepart(month, DeviceDateTime) mm, datepart(day, DeviceDateTime) dd, " +
                    "datepart(hour, DeviceDateTime) hh, datepart(minute, DeviceDateTime) mins, datepart(second, DeviceDateTime) ss, Latitude,Longitude, 0 sbjnum, CONVERT(float, 0.00) distanceFromLastPoint from " +
                    "trackuser where username = '" + surveyor + "' and DeviceDateTime >= '" + fromdate + "' and DeviceDateTime < '" + todate + "'" +
                    " and isnull(accuracy, 0) < 40 " + trackpointidFilter +
                    " order by SurveyorName, yy, mm, dd, hh, mins, ss";
            }

            var surveyors = db.Database.SqlQuery<SurveyDataView>(sql);
            List<SurveyDataView> surveyData = surveyors.ToList<SurveyDataView>();
            db.Database.CommandTimeout = timeout;

            //Remove points outside of city
            FilterOutofCityPoints(surveyData, citylat, citylong);

            //Remove points that are near with each other
            //FilterOutofNearPoints(surveyData);

            return View(surveyData);

        }

        [HttpPost]
        [Authorize(Roles = "Admin,KSAManager,GCCManager")]
        public ActionResult TrackData(int? id, int? c, int? country, string surveyor, string fordate)
        {
            string datewhere = "CONVERT(VARCHAR(2),DATEPART(DAY, DeviceDateTime)) + '-'+ CONVERT(VARCHAR(2),DATEPART(MONTH, DeviceDateTime)) + '-' + CONVERT(VARCHAR(4),DATEPART(YEAR, DeviceDateTime)) = '" + fordate + "'";
            string sql = @"SELECT DISTINCT DeviceDateTime,datepart(year, DeviceDateTime) yy, datepart(month, DeviceDateTime) mm, datepart(day, DeviceDateTime) dd, " +
                "datepart(hour, DeviceDateTime) hh, datepart(minute, DeviceDateTime) mins, datepart(second, DeviceDateTime) ss, Latitude,Longitude, 0 sbjnum, CONVERT(float, 0.00) distanceFromLastPoint from " +
                "trackuser where username = '" + surveyor + "' and  " + datewhere +
                " and isnull(accuracy, 0) < 40 " +
                " order by yy, mm, dd, hh, mins, ss";
            var surveyors = db.Database.SqlQuery<SurveyDataView>(sql);
            List<SurveyDataView> surveyData = surveyors.ToList<SurveyDataView>();

            //Remove points outside of city
            var cityName = db.Database.SqlQuery<CityView>("SELECT id, name, Longitude, Latitude, CountryID from City where ID = " + c);
            var cityData = cityName.ToList<CityView>();
            if (cityData.Count > 0)
            {
                double citylat = 0.0;
                double citylong = 0.0;
                CityView selCity = cityName.ToList<CityView>().FirstOrDefault();
                citylat = Convert.ToDouble(selCity.Latitude);
                citylong = Convert.ToDouble(selCity.Longitude);
                FilterOutofCityPoints(surveyData, citylat, citylong);
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            var result = new ContentResult
            {
                Content = serializer.Serialize(surveyData),
                ContentType = "application/json"
            };
            return result;
        }

        private void FilterOutofCityPoints(List<SurveyDataView> surveyData, double cityLat, double cityLong)
        {
            //return;
            int prox = 50000;

            GeoCoordinate curr = new GeoCoordinate(cityLat, cityLong);
            GeoCoordinate tar = new GeoCoordinate();

            List<SurveyDataView> scopy = surveyData.GetRange(0, surveyData.Count);
            int i = 0;
            foreach (var r in scopy)
            {
                tar.Latitude = r.Latitude;
                tar.Longitude = r.Longitude;

                double dist = curr.GetDistanceTo(tar);
                if (dist > prox)
                {
                    surveyData.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

        }

        private void FilterOutofNearPoints(List<SurveyDataView> surveyData)
        {
            GeoCoordinate lastPos = new GeoCoordinate();
            bool isfar = true;
            int prox = 10;

            List<SurveyDataView> scopy = surveyData.GetRange(0, surveyData.Count);
            int i = 0;

            foreach (var r in scopy)
            {
                if (isfar)
                {
                    lastPos.Latitude = r.Latitude;
                    lastPos.Longitude = r.Longitude;
                    isfar = false;
                }
                else
                {
                    GeoCoordinate curr = new GeoCoordinate(r.Latitude, r.Longitude);

                    double dist = curr.GetDistanceTo(lastPos);
                    r.distanceFromLastPoint = dist;
                    isfar = (dist > prox);
                }
            }

            foreach (var r in scopy)
            {
                if (r.distanceFromLastPoint > prox)
                {
                    surveyData.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }


        private void GetArea(double currentLatitude, double currentLongitude, int milesProximity, out double startLatitude, out double endLatitude, out double startLongitude, out double endLongitude)
        {
            startLatitude = currentLatitude - 1;
            endLatitude = currentLatitude + 1;
            startLongitude = currentLongitude - 1;
            endLongitude = currentLongitude + 1;

            GeoCoordinate geo = new GeoCoordinate();

        }
        public Boolean IgnoreNearestPoints(double latitude, double longitude, double latitude2, double longitude2)
        {

            double lat = latitude; //latitude
            double lat2 = latitude2; //longitude
            double lng = longitude; //latitude
            double lng2 = longitude2; //longitude

            string[] latstr = lat.ToString().Split('.');
            string[] lat2str = lat2.ToString().Split('.');

            string[] lngstr = lng.ToString().Split('.');
            string[] lng2str = lng2.ToString().Split('.');

            Boolean result = false;

            if (latstr[0].ToString() == lat2str[0].ToString())
            {
                double a = Convert.ToDouble(Convert.ToDouble(latstr[1]) - Convert.ToDouble(lat2str[1]));
                if (a < 0)
                {
                    a = Convert.ToDouble(Convert.ToDouble(lat2str[1]) - Convert.ToDouble(latstr[1]));

                }
                if (a > 4000000)
                {
                    result = true;
                }
            }
            else if (lngstr[0].ToString() == lng2str[0].ToString())
            {
                double a = Convert.ToDouble(Convert.ToDouble(lngstr[1]) - Convert.ToDouble(lng2str[1]));
                if (a < 0)
                {
                    a = Convert.ToDouble(Convert.ToDouble(lng2str[1]) - Convert.ToDouble(lngstr[1]));
                }
                if (a > 4000000)
                {
                    result = true;
                }
            }
            else
            {

                double c = Convert.ToDouble(Convert.ToDouble(lngstr[0]) - Convert.ToDouble(lng2str[0]));
                if (c < 0)
                {
                    c = Convert.ToDouble(Convert.ToDouble(lng2str[0]) - Convert.ToDouble(lngstr[0]));
                }
                if (c > 4)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }

                double b = Convert.ToDouble(Convert.ToDouble(latstr[0]) - Convert.ToDouble(lat2str[0]));
                if (b < 0)
                {
                    b = Convert.ToDouble(Convert.ToDouble(lat2str[0]) - Convert.ToDouble(latstr[0]));
                }
                if (b > 4)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }

            }
            return result;

        }


        private string GetJson(DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new

            System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows =
              new List<Dictionary<string, object>>();
            Dictionary<string, object> row = null;

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName.Trim(), dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult SurveyApprovalFilter(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string sql = @"select surveyorname as Name, count(*) as 'count' from survey where projectid = " + id + " group by surveyorname";

            var surveyors = db.Database.SqlQuery<Surveyorname>(sql);
            ViewBag.Surveyors = surveyors.ToList<Surveyorname>();

            List<SurveyDataView> data = new List<SurveyDataView>();

            sql = "SELECT Id, Name FROM Project WHERE Id = " + id;
            var project = db.Database.SqlQuery<ProjectView>(sql);
            ViewBag.ProjectName = project.FirstOrDefault<ProjectView>().Name;
            ViewBag.ProjectId = id;

            //--------------------------------------------------------------
            //Generate city names view for Menu
            var country = 2;        //KSA
            var allCities = db.Database.SqlQuery<CityView>(@"
                SELECT City.id, City.name, Country.Id AS CountryId, Country.Name CountryName FROM City INNER JOIN Country ON City.CountryId = Country.Id 
                INNER JOIN rdxlocation rdx ON City.id = rdx.cityid AND rdx.ReportID IN (1,3) 
                WHERE Country.Id = " + country + " ORDER BY City.CountryId, City.Name");
            List<CityView> allCityData = allCities.ToList<CityView>();
            ViewBag.AllCitiesList = allCityData;

            return View(data);
        }
        [Authorize(Roles = "Admin,CanEdit,Field Supervisor,Project Manager")]
        public ActionResult OpsReports(int? id, string rc, string language)
        {
            //id = projectid, rc = report code

            if (id == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string sql = "";

            switch (rc)
            {
                case "Coding":
                    sql = @"select surveyorname as Name, count(*) as 'count' from survey where projectid = " + id + " group by surveyorname";

                    var surveyors = db.Database.SqlQuery<Surveyorname>(sql);
                    ViewBag.Surveyors = surveyors.ToList<Surveyorname>();

                    List<SurveyDataView> data = new List<SurveyDataView>();

                    sql = @"select * from ProjectField where fieldtype = 'SLT' AND projectid = " + id + " ORDER BY DisplayOrder";

                    var openended = db.Database.SqlQuery<ProjectField>(sql);
                    return View(openended.ToList<ProjectField>());
            }

            sql = "SELECT Id, Name FROM Project WHERE Id = " + id;
            var project = db.Database.SqlQuery<ProjectView>(sql);
            ViewBag.ProjectName = project.FirstOrDefault<ProjectView>().Name;
            ViewBag.ProjectId = id;

            return View();
        }
        [Authorize(Roles = "Admin,CanEdit,Field Supervisor,Project Manager")]
        public ActionResult OpsReportsView(int? id, string rc, string language)
        {
            //id = projectid, rc = report code

            if (id == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string sql = "";

            switch (rc)
            {
                case "Coding":
                    //string language = "en-US";
                    if (language != null && language.Length == 0)
                    {
                        if (Request.QueryString.Get("lang") != null)
                        {
                            language = Request.QueryString.Get("lang");
                        }
                        else
                        {
                            language = "en-US";
                        }
                    }

                    string fromDate = "", toDate = "";
                    if (Request.QueryString.Get("fromdate") != null)
                    {
                        fromDate = Request.QueryString.Get("fromdate");
                    }
                    if (Request.QueryString.Get("todate") != null)
                    {
                        toDate = Request.QueryString.Get("todate");
                    }
                    string surveyors = "";
                    if (Request.QueryString.Get("surveyors") != null)
                    {
                        surveyors = Request.QueryString.Get("surveyors");
                    }
                    string openendedFields = "";
                    string _openendedFields = "";
                    if (Request.QueryString.Get("oef") != null)
                    {
                        openendedFields = Request.QueryString.Get("oef");
                        _openendedFields = '_' + openendedFields.Replace(",", ",_"); //Add _ before each Id
                    }

                    sql = "SELECT " + _openendedFields + " FROM " +
                        @"(SELECT sd.FieldValue, CONCAT('_', sd.FieldId) as FieldId, row_number() over(partition by sd.FieldId order by sd.FieldValue) rn
                            FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum 
                            INNER JOIN ProjectField pf ON sd.FieldId = pf.ID 
                            WHERE ISNULL(s.Test, 0) = 0 AND s.ProjectID = " + id +
                           (fromDate.Length > 0 && toDate.Length > 0 ? " AND s.Created >= '" + fromDate + "' AND s.Created <= '" + toDate + "'" : "") +
                           (surveyors.Length > 0 ? " AND s.SurveyorName IN (" + surveyors + ")" : "") +
                           (openendedFields.Length > 0 ? " AND sd.FieldId IN (" + openendedFields + ")" : "") +
                           @" GROUP BY sd.FieldValue, sd.FieldId
                           ) as orderedTable
                           PIVOT
                           (
                              max(FieldValue)
                              FOR FieldId in (" + _openendedFields + ")" +
                           ") AS pivottable";

                    using (var ctx = new ProjectContext())
                    using (var cmd = ctx.Database.Connection.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        ctx.Database.Connection.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            var model = Read(reader).ToList();
                            sql = "SELECT * FROM ProjectField WHERE ProjectId = " + id + (openendedFields.Length > 0 ? " AND Id IN (" + openendedFields + ")" : "");

                            var projectFields = db.Database.SqlQuery<ProjectField>(sql);
                            List<ProjectField> pfdata = projectFields.ToList<ProjectField>();
                            ViewBag.ProjectFields = pfdata;

                            ViewBag.ProjectId = id;
                            return View("OpsReport_Coding", model);
                        }
                    }
            }
            sql = "SELECT Id, Name FROM Project WHERE Id = " + id;
            var project = db.Database.SqlQuery<ProjectView>(sql);
            ViewBag.ProjectName = project.FirstOrDefault<ProjectView>().Name;
            ViewBag.ProjectId = id;

            return View();
        }

        private static IEnumerable<object[]> Read(DbDataReader reader)
        {
            while (reader.Read())
            {
                var values = new List<object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    values.Add(reader.GetValue(i));
                }
                yield return values.ToArray();
            }
        }

        [Authorize(Roles = "Admin,CanEdit,QC,Project Manager")]
        public ActionResult SurveyQCFilter(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string sql = @"select surveyorname as Name, count(*) as 'count' from survey where projectid = " + id + " group by surveyorname";

            var surveyors = db.Database.SqlQuery<Surveyorname>(sql);
            ViewBag.Surveyors = surveyors.ToList<Surveyorname>();

            List<SurveyDataView> data = new List<SurveyDataView>();

            sql = "SELECT Id, Name FROM Project WHERE Id = " + id;
            var project = db.Database.SqlQuery<ProjectView>(sql);
            ViewBag.ProjectName = project.FirstOrDefault<ProjectView>().Name;
            ViewBag.ProjectId = id;

            return View(data);
        }

        [Authorize(Roles = "Admin,CanEdit,QC,Project Manager")]
        public ActionResult Reports(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string sql = "SELECT Id, Name FROM Project WHERE Id = " + id;
            var project = db.Database.SqlQuery<ProjectView>(sql);
            ViewBag.ProjectName = project.FirstOrDefault<ProjectView>().Name;
            ViewBag.ProjectId = id;

            return View();
        }


        [Authorize]
        [HttpGet]
        public ActionResult AnalyticReports(int? id)
        {
            if (id > 0)
            {
                //Get Project Id and Name
                string sql = "SELECT Id, Name FROM Project WHERE Id = " + id;
                var project = db.Database.SqlQuery<ProjectView>(sql).FirstOrDefault<ProjectView>();
                if (project != null)
                {
                    ViewBag.ProjectName = project.Name;
                    ViewBag.ProjectId = id;
                    var designController = new DesignerController();

                    //Get Project Field Sections
                    List<ProjectFieldSection> ProjectFieldSection = db.ProjectFieldSection
                        .Where(x => x.ProjectID.Equals((int)id))
                        .OrderBy(x => x.DisplayOrder)
                        .ToList<ProjectFieldSection>();
                    if (ProjectFieldSection.Count == 0)
                    {
                        ProjectFieldSection.Add(new ProjectFieldSection { ProjectID = id.GetValueOrDefault(0), ID = 0, Name = "Questionnaire" });
                    }
                    ViewBag.ProjectFieldSection = ProjectFieldSection;

                    //Get records of ProjectFieldSample Options
                    sql = "SELECT pfs.* FROM ProjectFieldSample pfs INNER JOIN ProjectField pf ON pfs.FieldID = pf.ID " +
                          " WHERE (pf.FieldType <> 'SCD' OR pf.FieldType <> 'SCG' OR pf.FieldType <> 'MCG') AND pfs.ParentSampleID IS NULL AND pfs.IsActive = 1 AND pf.ProjectId = " + id.Value +
                          " ORDER BY pf.SectionID, pfs.DisplayOrder";
                    List<DNA_CAPI_MIS.Models.ProjectFieldSample> ProjectFieldSamples = db.Database.SqlQuery<ProjectFieldSample>(sql).ToList();
                    ViewBag.ProjectFieldSample = ProjectFieldSamples;

                    //Get records of ProjectFieldSample Questions
                    sql = "SELECT pfs.* FROM ProjectFieldSample pfs INNER JOIN ProjectField pf ON pfs.FieldID = pf.ID " +
                          " WHERE pfs.ParentSampleID = 0 AND pfs.IsActive = 1 AND pf.ProjectId = " + id.Value +
                          " ORDER BY pfs.DisplayOrder";
                    List<DNA_CAPI_MIS.Models.ProjectFieldSample> ProjectFieldSamplesQ = db.Database.SqlQuery<ProjectFieldSample>(sql).ToList();
                    ViewBag.ProjectFieldSampleQ = ProjectFieldSamplesQ;

                    sql = string.Format(@"SELECT t.* FROM Translation t INNER JOIN ProjectFieldSample pfs 
                            ON pfs.ID = t.KeyValue AND t.EntityName = 'PROJECTFIELDSAMPLE' AND t.FieldName = 'TITLE'
                        INNER JOIN ProjectField pf ON pfs.FieldID = pf.ID AND pf.ProjectId = {0}", id.Value);

                    List<DNA_CAPI_MIS.Models.Translation> SampleTitleTranslations = db.Database.SqlQuery<Translation>(sql).ToList<Translation>();
                    ViewBag.SampleTitleTranslations = SampleTitleTranslations;

                    //Project Fields
                    List<ProjectField> pfList = db.ProjectField.Where<ProjectField>(f => f.ProjectID == id.Value).ToList();
                    foreach (var pf in pfList)
                    {
                        int fid = pf.ID;

                        if (pf.OptionsJSON != null && pf.OptionsJSON.Length > 0)
                        {
                            //DeSerialize JSON into Dictionary object
                            var pfOptionJSON = new Dictionary<string, Object>();
                            var optDeSerializer = new JsonFx.Json.JsonReader();
                            dynamic output = optDeSerializer.Read(pf.OptionsJSON);
                            if (output != null)
                            {
                                designController.PopulateAttributeList(pfOptionJSON, output);
                                ViewBag.OptionsJSON = pfOptionJSON;
                            }
                        }
                    }
                    return View("AnalyticReports", designController.GetProjectFieldsWithValues((int)id));
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }


        [Authorize]
        public ActionResult _SurveyDetailRow(int projectid, int sectionId, int sbjnum)
        {

            string sql;
            sql = @"SELECT pf.ID,pf.title as Section_name, SampleAnswers.Title name, SampleAnswersOption.Title options
FROM
 ProjectField pf 
    INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
 LEFT OUTER JOIN fnSurveyValues({2}) sd ON sd.FieldID = pf.ID
 LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
  CASE 
   WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE sd.Answer END) 
   WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' THEN sd.OptionCode 
   ELSE sd.Answer END
 LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
    WHERE pf.ProjectID={0} 

	and fs.ID={1}
    Order by fs.DisplayOrder, pf.DisplayOrder";

            ViewBag.id = sectionId;


            List<SurveyDetails> SurDetail = db.Database.SqlQuery<SurveyDetails>(string.Format(sql, projectid.ToString(), sectionId.ToString(), sbjnum.ToString())).ToList<SurveyDetails>();


            ViewBag.SurDetail = SurDetail;
            return View();
        }


        [Authorize]
        public ActionResult SummaryData(int id)
        {
            ViewBag.id = id;
            List<GraphDataWithId> GdataWid = new List<GraphDataWithId>();
            List<PieData> piDataLst = new List<PieData>();
            List<GraphData> GraphDataLst = new List<GraphData>();
            string sql = "";
            int userid = Convert.ToInt32(User.Identity.GetUserId());
            var userRoles = UserManager.GetRoles(userid);
            List<CustomerUser> custUser = db.CustomerUser.Where(x => x.UserID == userid).ToList();
            string userids = "";

            if (custUser.Count > 0)
            {
                if (custUser[0].CustomerBranchID != 1)
                {
                    ViewBag.BranchName = "Internal  Audit " + custUser[0].CustomerBranch.Name;
                }
                int cusBrnchid = custUser[0].CustomerBranchID;
                //.Database
                List<string> useridlst = db.CustomerUser.Where(x => x.CustomerBranchID == cusBrnchid).Select(x => x.UserID.ToString()).ToList();
                userids = string.Join(",", useridlst);
                List<string> tempstrlst = idenDB.Database.SqlQuery<string>("select Replace( username,'@pwd.com','') from AspNetUsers where id in (" + userids + ") ").ToList<string>();
                username = "'" + string.Join("','", tempstrlst) + "'";

            }

            #region
            //             sql = @"
            //                with cte (y,name) as ( SELECT SUM(SubSum.BaseCount) as y, pfs.Name as name from 
            //         	(SELECT mp.FieldID AS FieldID, 
            //         		(SELECT COUNT(*) FROM surveydata msd 
            //         		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            //         		AND msd.sbjnum IN 
            //         			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //         			WHERE ISNULL(s.Test, 0) = 0 GROUP BY s.sbjnum ) ) BaseCount 
            //         	FROM projectfieldsample mp) AS SubSum  
            //         INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
            //         INNER JOIN ProjectFieldSection pfs on pfs.ID = pf.SectionID
            //         WHERE pf.ProjectID={0} 
            //         GROUP BY pfs.ID,pfs.Name)   

            //select (CAST(cte.y AS float) / CAST((select SUM(cte.y) from cte) as FLOAT))*100 as y,cte.name as name from cte
            //         ";
            //         piDataLst = db.Database.SqlQuery<PieData>(string.Format(sql, id.ToString())).ToList<PieData>();
            //         ViewBag.OverAllStatusPie = JsonConvert.SerializeObject(piDataLst);

            //         sql = @"SELECT Top 5 SUM(SubSum.BaseCount) as y, pf.Title as name from 
            //         	(SELECT mp.FieldID AS FieldID, 
            //         		(SELECT COUNT(*) FROM surveydata msd 
            //         		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            //         		AND msd.sbjnum IN 
            //         			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //         			WHERE ISNULL(s.Test, 0) = 0 GROUP BY s.sbjnum ) ) BaseCount 
            //         	FROM projectfieldsample mp) AS SubSum  
            //         INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
            //         WHERE pf.ProjectID={0} 
            //group by pf.id,pf.Title
            //order by y desc";
            //         List<GraphData> GraphDataLst = db.Database.SqlQuery<GraphData>(string.Format(sql, id.ToString())).ToList<GraphData>();
            //         ViewBag.DealerHighScoreDetailX = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.y).ToArray());
            //         ViewBag.DealerHighScoreDetailName = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.name).ToArray());

            //         sql = @"SELECT Top 5 SUM(SubSum.BaseCount) as y, pf.Title as name from 
            //         	(SELECT mp.FieldID AS FieldID, 
            //         		(SELECT COUNT(*) FROM surveydata msd 
            //         		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            //         		AND msd.sbjnum IN 
            //         			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //         			WHERE ISNULL(s.Test, 0) = 0 GROUP BY s.sbjnum ) ) BaseCount 
            //         	FROM projectfieldsample mp) AS SubSum  
            //         INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
            //         WHERE pf.ProjectID={0} 
            //group by pf.id,pf.Title
            //order by y asc";
            //         GraphDataLst = db.Database.SqlQuery<GraphData>(string.Format(sql, id.ToString())).ToList<GraphData>();
            //         ViewBag.DealerLowScoreDetailX = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.y).ToArray());
            //         ViewBag.DealerLowScoreDetailName = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.name).ToArray());

            //            sql = @"Select C.name name,
            //    		  (SELECT SUM(SubSum.BaseCount) y from 
            //            	(SELECT mp.FieldID AS FieldID, 
            //            		(SELECT COUNT(*) FROM surveydata msd 
            //            		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            //            		AND msd.sbjnum IN 
            //            			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            			WHERE ISNULL(s.Test, 0) = 0 
            //						and sd.FieldId = 49756 
            //						AND sd.FieldValue in (select CB.ID from CustomerBranch CB where CB.CityID=C.ID) 
            //						GROUP BY s.sbjnum ) ) BaseCount 
            //            	FROM projectfieldsample mp
            //									inner join ProjectField pf on pf.ID=mp.FieldID
            //				 WHERE pf.ProjectID={0} ) AS SubSum  ) y,C.ID id
            //			from City C 
            //			where C.ID in (1,5558,2071)

            //			union

            //Select total.name,SUM( total.y) y ,0 id from
            //(Select 'Others' name,
            //    		  (SELECT SUM(SubSum.BaseCount) y from 
            //            	(SELECT mp.FieldID AS FieldID, 
            //            		(SELECT COUNT(*) FROM surveydata msd 
            //            		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            //            		AND msd.sbjnum IN 
            //            			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            			WHERE ISNULL(s.Test, 0) = 0 
            //						and sd.FieldId = 49756 
            //						AND sd.FieldValue in (select CB.ID from CustomerBranch CB where CB.CityID=C.ID) 
            //						GROUP BY s.sbjnum ) ) BaseCount 
            //            	FROM projectfieldsample mp
            //				inner join ProjectField pf on pf.ID=mp.FieldID
            //				 WHERE pf.ProjectID={0}) AS SubSum  ) y
            //			from City C 
            //			where C.ID not in (1,5558,2071))total		
            //			group by name
            //";
            //            GdataWid = db.Database.SqlQuery<GraphDataWithId>(string.Format(sql, id.ToString())).ToList<GraphDataWithId>();
            //            ViewBag.MetroCitySummaryData = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());
            //            ViewBag.MetroCitySummaryname = JsonConvert.SerializeObject(GdataWid.Select(x =>  x.name ).ToArray());

            //            sql = @"
            //Select CR.Name name,
            //    		  Convert(float,(SELECT SUM(SubSum.BaseCount) y from 
            //            	(SELECT mp.FieldID AS FieldID, 
            //            		(SELECT COUNT(*) FROM surveydata msd 
            //            		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            //            		AND msd.sbjnum IN 
            //            			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            			WHERE ISNULL(s.Test, 0) = 0 
            //						and sd.FieldId = 49756 AND sd.FieldValue in 
            //						(Select CB.ID from RDXCustomerRegion RCR
            //						join City C on C.ID=RCR.CityID
            //						join CustomerBranch CB on CB.CityID=C.ID
            //						where RCR.CustomerRegionID=CR.ID
            //						) 
            //						GROUP BY s.sbjnum ) ) BaseCount 
            //            	FROM projectfieldsample mp
            //				inner join ProjectField pf on pf.ID=mp.FieldID
            //				 WHERE pf.ProjectID={0} ) AS SubSum  )) y
            //			from 
            //			CustomerRegion CR 
            //			";
            //            piDataLst = db.Database.SqlQuery<PieData>(string.Format(sql, id.ToString())).ToList<PieData>();
            //            ViewBag.NationalStatusPie = JsonConvert.SerializeObject(piDataLst);





            //            sql = @"	Select Cb.Name name,Cb.id id,
            //    		(SELECT SUM(SubSum.BaseCount) y from 
            //            	(SELECT mp.FieldID AS FieldID, 
            //            		(SELECT COUNT(*) FROM surveydata msd 
            //            			WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))

            //						AND msd.sbjnum IN	--Dealer
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49756 AND sd.FieldValue = Convert(nvarchar, Cb.ID) 
            //							GROUP BY s.sbjnum )

            //						ANd msd.sbjnum IN	--Quarter
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49757 AND sd.FieldValue IN ('1','2','3','4')
            //							GROUP BY s.sbjnum ) 
            //					) BaseCount 
            //            	FROM projectfieldsample mp
            //					inner join ProjectField pf on pf.ID=mp.FieldID
            //				 WHERE pf.ProjectID={0}  ) AS SubSum  
            //            ) y
            //			from CustomerBranch Cb			
            //			 where CB.IsActive=1
            //";

            //            GdataWid = db.Database.SqlQuery<GraphDataWithId>(string.Format(sql, id.ToString())).ToList<GraphDataWithId>();
            //            ViewBag.NationalAVGdataQ0 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());
            //            ViewBag.NationalAVGname = JsonConvert.SerializeObject(GdataWid.Select(x => x.name).ToArray());

            //            sql = @"	Select Cb.Name name,Cb.id id,
            //    		(SELECT SUM(SubSum.BaseCount) y from 
            //            	(SELECT mp.FieldID AS FieldID, 
            //            		(SELECT COUNT(*) FROM surveydata msd 
            //            			WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))

            //						AND msd.sbjnum IN	--Dealer
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49756 AND sd.FieldValue = Convert(nvarchar, Cb.ID) 
            //							GROUP BY s.sbjnum )

            //						ANd msd.sbjnum IN	--Quarter
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49757 AND sd.FieldValue IN ('1','2','3','4')
            //							GROUP BY s.sbjnum ) 
            //					) BaseCount 
            //            	FROM projectfieldsample mp
            //					inner join ProjectField pf on pf.ID=mp.FieldID
            //				 WHERE pf.ProjectID={0}  ) AS SubSum  
            //            ) y
            //			from CustomerBranch Cb			
            //			 where CB.IsActive=1
            //";
            //            GdataWid = db.Database.SqlQuery<GraphDataWithId>(string.Format(sql, id.ToString())).ToList<GraphDataWithId>();

            //            ViewBag.NationalAVGdataQ1 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());


            //            sql = @"
            //	Select Cb.Name name,Cb.id id,
            //    		(SELECT SUM(SubSum.BaseCount) y from 
            //            	(SELECT mp.FieldID AS FieldID, 
            //            		(SELECT COUNT(*) FROM surveydata msd 
            //            			WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))

            //						AND msd.sbjnum IN	--Dealer
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49756 AND sd.FieldValue = Convert(nvarchar, Cb.ID) 
            //							GROUP BY s.sbjnum )

            //						ANd msd.sbjnum IN	--Quarter
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49757 AND sd.FieldValue IN ('1','2','3','4')
            //							GROUP BY s.sbjnum ) 
            //					) BaseCount 
            //            	FROM projectfieldsample mp
            //					inner join ProjectField pf on pf.ID=mp.FieldID
            //				 WHERE pf.ProjectID={0}  ) AS SubSum  
            //            ) y
            //			from CustomerBranch Cb
            //			join City C on Cb.CityID=C.ID
            //			join RDXCustomerRegion RCR on RCR.CityID=C.ID

            //			 where CB.IsActive=1 and RCR.CustomerRegionID=1";

            //            GdataWid = db.Database.SqlQuery<GraphDataWithId>(string.Format(sql, id.ToString())).ToList<GraphDataWithId>();
            //            ViewBag.SouthAVGdataQ0 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());
            //            ViewBag.SouthAVGname = JsonConvert.SerializeObject(GdataWid.Select(x => x.name).ToArray());



            //            sql = @"
            //	Select Cb.Name name,Cb.id id,
            //    		(SELECT SUM(SubSum.BaseCount) y from 
            //            	(SELECT mp.FieldID AS FieldID, 
            //            		(SELECT COUNT(*) FROM surveydata msd 
            //            			WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))

            //						AND msd.sbjnum IN	--Dealer
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49756 AND sd.FieldValue = Convert(nvarchar, Cb.ID) 
            //							GROUP BY s.sbjnum )

            //						ANd msd.sbjnum IN	--Quarter
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49757 AND sd.FieldValue IN ('1','4')
            //							GROUP BY s.sbjnum ) 
            //					) BaseCount 
            //            	FROM projectfieldsample mp
            //					inner join ProjectField pf on pf.ID=mp.FieldID
            //				 WHERE pf.ProjectID={0}  ) AS SubSum  
            //            ) y
            //			from CustomerBranch Cb
            //			join City C on Cb.CityID=C.ID
            //			join RDXCustomerRegion RCR on RCR.CityID=C.ID

            //			 where CB.IsActive=1 and RCR.CustomerRegionID=1";

            //            GdataWid = db.Database.SqlQuery<GraphDataWithId>(string.Format(sql, id.ToString())).ToList<GraphDataWithId>();
            //            ViewBag.SouthAVGdataQ1 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());

            //            sql = @"
            //	Select Cb.Name name,Cb.id id,
            //    		(SELECT SUM(SubSum.BaseCount) y from 
            //            	(SELECT mp.FieldID AS FieldID, 
            //            		(SELECT COUNT(*) FROM surveydata msd 
            //            			WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))

            //						AND msd.sbjnum IN	--Dealer
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49756 AND sd.FieldValue = Convert(nvarchar, Cb.ID) 
            //							GROUP BY s.sbjnum )

            //						ANd msd.sbjnum IN	--Quarter
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49757 AND sd.FieldValue IN ('1','2','3','4')
            //							GROUP BY s.sbjnum ) 
            //					) BaseCount 
            //            	FROM projectfieldsample mp
            //					inner join ProjectField pf on pf.ID=mp.FieldID
            //				 WHERE pf.ProjectID={0}  ) AS SubSum  
            //            ) y
            //			from CustomerBranch Cb
            //			join City C on Cb.CityID=C.ID
            //			join RDXCustomerRegion RCR on RCR.CityID=C.ID

            //			 where CB.IsActive=1 and RCR.CustomerRegionID=3";

            //            GdataWid = db.Database.SqlQuery<GraphDataWithId>(string.Format(sql, id.ToString())).ToList<GraphDataWithId>();
            //            ViewBag.NorthAVGdataQ0 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());
            //            ViewBag.NorthAVGname = JsonConvert.SerializeObject(GdataWid.Select(x => x.name).ToArray());


            //            sql = @"
            //	Select Cb.Name name,Cb.id id,
            //    		(SELECT SUM(SubSum.BaseCount) y from 
            //            	(SELECT mp.FieldID AS FieldID, 
            //            		(SELECT COUNT(*) FROM surveydata msd 
            //            			WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))

            //						AND msd.sbjnum IN	--Dealer
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49756 AND sd.FieldValue = Convert(nvarchar, Cb.ID) 
            //							GROUP BY s.sbjnum )

            //						ANd msd.sbjnum IN	--Quarter
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49757 AND sd.FieldValue IN ('1','4')
            //							GROUP BY s.sbjnum ) 
            //					) BaseCount 
            //            	FROM projectfieldsample mp
            //					inner join ProjectField pf on pf.ID=mp.FieldID
            //				 WHERE pf.ProjectID={0}  ) AS SubSum  
            //            ) y
            //			from CustomerBranch Cb
            //			join City C on Cb.CityID=C.ID
            //			join RDXCustomerRegion RCR on RCR.CityID=C.ID

            //			 where CB.IsActive=1 and RCR.CustomerRegionID=3";

            //            GdataWid = db.Database.SqlQuery<GraphDataWithId>(string.Format(sql, id.ToString())).ToList<GraphDataWithId>();
            //            ViewBag.NorthAVGdataQ1 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());
            //            sql = @"
            //	Select Cb.Name name,Cb.id id,
            //    		(SELECT SUM(SubSum.BaseCount) y from 
            //            	(SELECT mp.FieldID AS FieldID, 
            //            		(SELECT COUNT(*) FROM surveydata msd 
            //            			WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))

            //						AND msd.sbjnum IN	--Dealer
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49756 AND sd.FieldValue = Convert(nvarchar, Cb.ID) 
            //							GROUP BY s.sbjnum )

            //						ANd msd.sbjnum IN	--Quarter
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49757 AND sd.FieldValue IN ('1','2','3','4')
            //							GROUP BY s.sbjnum ) 
            //					) BaseCount 
            //            	FROM projectfieldsample mp
            //					inner join ProjectField pf on pf.ID=mp.FieldID
            //				 WHERE pf.ProjectID={0}  ) AS SubSum  
            //            ) y
            //			from CustomerBranch Cb
            //			join City C on Cb.CityID=C.ID
            //			join RDXCustomerRegion RCR on RCR.CityID=C.ID

            //			 where CB.IsActive=1 and RCR.CustomerRegionID = 2";

            //            GdataWid = db.Database.SqlQuery<GraphDataWithId>(string.Format(sql, id.ToString())).ToList<GraphDataWithId>();
            //            ViewBag.CentralAVGdataQ0 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());
            //            ViewBag.CentralAVGname = JsonConvert.SerializeObject(GdataWid.Select(x => x.name).ToArray());
            //            sql = @"
            //	Select Cb.Name name,Cb.id id,
            //    		(SELECT SUM(SubSum.BaseCount) y from 
            //            	(SELECT mp.FieldID AS FieldID, 
            //            		(SELECT COUNT(*) FROM surveydata msd 
            //            			WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))

            //						AND msd.sbjnum IN	--Dealer
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49756 AND sd.FieldValue = Convert(nvarchar, Cb.ID) 
            //							GROUP BY s.sbjnum )

            //						ANd msd.sbjnum IN	--Quarter
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49757 AND sd.FieldValue IN ('1','2','3','4')
            //							GROUP BY s.sbjnum ) 
            //					) BaseCount 
            //            	FROM projectfieldsample mp
            //					inner join ProjectField pf on pf.ID=mp.FieldID
            //				 WHERE pf.ProjectID= {0}  ) AS SubSum  
            //            ) y
            //			from CustomerBranch Cb
            //			join City C on Cb.CityID=C.ID
            //			join RDXCustomerRegion RCR on RCR.CityID=C.ID

            //			 where CB.IsActive=1 and RCR.CustomerRegionID=2";

            //            GdataWid = db.Database.SqlQuery<GraphDataWithId>(string.Format(sql, id.ToString())).ToList<GraphDataWithId>();
            #endregion
            //GdataWid = db.Database.SqlQuery<GraphDataWithId>(string.Format(sql, id.ToString())).ToList<GraphDataWithId>();

            ViewBag.DealerHighScoreDetailX = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.y).ToArray());
            ViewBag.DealerHighScoreDetailName = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.name).ToArray());

            ViewBag.DealerLowScoreDetailX = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.y).ToArray());
            ViewBag.DealerLowScoreDetailName = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.name).ToArray());


            ViewBag.OverAllStatusPie = JsonConvert.SerializeObject(piDataLst);

            ViewBag.NationalAVGdataQ0 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());
            ViewBag.NationalAVGname = JsonConvert.SerializeObject(GdataWid.Select(x => x.name).ToArray());
            ViewBag.NationalAVGdataQ1 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());

            ViewBag.SouthAVGdataQ0 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());
            ViewBag.SouthAVGname = JsonConvert.SerializeObject(GdataWid.Select(x => x.name).ToArray());
            ViewBag.SouthAVGdataQ1 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());


            ViewBag.NorthAVGdataQ0 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());
            ViewBag.NorthAVGname = JsonConvert.SerializeObject(GdataWid.Select(x => x.name).ToArray());
            ViewBag.NorthAVGdataQ1 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());


            ViewBag.CentralAVGdataQ0 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());
            ViewBag.CentralAVGname = JsonConvert.SerializeObject(GdataWid.Select(x => x.name).ToArray());
            ViewBag.CentralAVGdataQ1 = JsonConvert.SerializeObject(GdataWid.Select(x => new { x.y, x.id }).ToArray());
            return View();
        }

        [Authorize]
        public JsonResult FilterFirstRow(string CompareQuarter, string dealerid = "", string Cityid = "", string Projectid = "")
        {
            string CompareQuarters = CompareQuarter;
            List<PieData> piDataLst = new List<PieData>();
            string nationalSummaryName = "", nationalSummaryData = "", MetroCitySummaryData = "", MetroCitySummaryname = "", cityValue = "", cityname = "";
            List<GraphData> GraphDataLst = new List<GraphData>();
            List<GraphDataPercentage> GrphDataPer = new List<GraphDataPercentage>();
            List<GraphDataWithIdPercentage> GraphDataWithP = new List<GraphDataWithIdPercentage>();
            SqlQuriesDashboard sqlQ = new SqlQuriesDashboard();
            List<GraphDataWithId> GdataWid;
            string sql = "";
            int userid = Convert.ToInt32(User.Identity.GetUserId());
            var userRoles = UserManager.GetRoles(userid);
            List<CustomerUser> custUser = db.CustomerUser.Where(x => x.UserID == userid).ToList();

            sql = sqlQ.NationalStatusPie;

            //piDataLst = db.Database.SqlQuery<PieData>(string.Format(sql, Projectid, CompareQuarters)).ToList<PieData>();
            GrphDataPer = sqlQ.NationalRegionalLevelPie(Convert.ToInt32(Projectid), CompareQuarters, GetAllAdminUser());
            nationalSummaryData = JsonConvert.SerializeObject(GrphDataPer.Select(x => x.y).ToList());
            nationalSummaryName = JsonConvert.SerializeObject(GrphDataPer.Select(x => x.name).ToList());
            //sql = sqlQ.MetroCitySummary;
            //GdataWid = db.Database.SqlQuery<GraphDataWithId>(string.Format(sql, Projectid, CompareQuarters)).ToList<GraphDataWithId>();
            if (custUser.Count == 0 || custUser[0].CustomerBranchID == 1)
            {
                GraphDataWithP = sqlQ.CityLevel(Convert.ToInt32(Projectid), CompareQuarters, null, username);
            }
            else
            {
                GraphDataWithP = sqlQ.CityLevelDealer(Convert.ToInt32(Projectid), CompareQuarters, null, Convert.ToInt32(custUser[0].CustomerBranchID), GetAllAdminUser());
            }



            MetroCitySummaryData = JsonConvert.SerializeObject(GraphDataWithP.Select(x => new { x.y, x.id, x.intenal }).ToArray());
            MetroCitySummaryname = JsonConvert.SerializeObject(GraphDataWithP.Select(x => x.name).ToArray());

            if (Cityid.ToLower() != "")
            {
                if (Cityid != "0")
                {
                    int cityID = Convert.ToInt32(Cityid);
                    //sql = sqlQ.CitiesDetail;
                    //GraphDataLst = db.Database.SqlQuery<GraphData>(string.Format(sql, Projectid, Cityid.ToString(), CompareQuarters)).ToList<GraphData>();
                    if (custUser[0].CustomerBranchID == 1)
                    {

                        GraphDataWithP = sqlQ.CityLevel(Convert.ToInt32(Projectid), CompareQuarters, cityID, username);
                    }
                    else
                    {
                        GraphDataWithP = sqlQ.CityLevelDealer(Convert.ToInt32(Projectid), CompareQuarters, cityID, Convert.ToInt32(custUser[0].CustomerBranchID), GetAllAdminUser());
                    }

                }
                else
                {
                    //sql = sqlQ.OtherCitiesDetail;
                    //GraphDataLst = db.Database.SqlQuery<GraphData>(string.Format(sql, Projectid, "1,2071,5558", CompareQuarters)).ToList<GraphData>();
                    if (custUser[0].CustomerBranchID == 1)
                    {

                        GraphDataWithP = sqlQ.CityLevel(Convert.ToInt32(Projectid), CompareQuarters, -1, username);
                    }
                    else
                    {
                        GraphDataWithP = sqlQ.CityLevelDealer(Convert.ToInt32(Projectid), CompareQuarters, -1, Convert.ToInt32(custUser[0].CustomerBranchID), GetAllAdminUser());
                    }
                }

                cityValue = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.y).ToArray());
                cityname = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.name.Trim()).ToArray());
            }
            return Json(
                new
                {
                    nationalSummaryData = nationalSummaryData,
                    nationalSummaryName = nationalSummaryName,
                    MetroCitySummaryData = MetroCitySummaryData,
                    MetroCitySummaryname = MetroCitySummaryname,
                    cityname = cityname,
                    cityValue = cityValue
                }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public List<GraphDataWithIdPercentage> fillGraphDatawithPercentage(List<GraphDataWithId> gdata)
        {
            List<GraphDataWithIdPercentage> GDatawithIdP = new List<GraphDataWithIdPercentage>();
            int total = gdata.Sum(x => x.y);


            foreach (var item in gdata)
            {
                GraphDataWithIdPercentage Gdata = new GraphDataWithIdPercentage();

                Gdata.y = Math.Round(Convert.ToDouble(item.y) / total * 100, 2);
                Gdata.name = item.name;
                Gdata.id = item.id;
                GDatawithIdP.Add(Gdata);
            }

            return GDatawithIdP;

        }


        [Authorize]
        public JsonResult FilterSecondRow(string CompareQuarter, string CompareQuarterLocal, string Projectid)
        {
            string NationalQ0 = "[]", NationalQ1 = "[]", NationalName = "[]";
            string SouthQ0 = "[]", SouthQ1 = "[]", SouthName = "[]";
            string NorthQ0 = "[]", NorthQ1 = "[]", NorthName = "[]";
            string CentralQ0 = "[]", CentralQ1 = "[]", CentralName = "[]";
            int total = 0;
            int userid = Convert.ToInt32(User.Identity.GetUserId());
            var userRoles = UserManager.GetRoles(userid);
            List<CustomerUser> custUser = db.CustomerUser.Where(x => x.UserID == userid).ToList();


            List<GraphDataWithId> GDatawithId = new List<GraphDataWithId>();
            List<GraphDataWithIdPercentage> GDatawithIdP = new List<GraphDataWithIdPercentage>();

            SqlQuriesDashboard sqlQ = new SqlQuriesDashboard();

            if (custUser[0].CustomerBranchID == 1)
            {
                //GDatawithId = db.Database.SqlQuery<GraphDataWithId>(string.Format(sqlQ.NationalBrachComp, Projectid, CompareQuarter)).ToList<GraphDataWithId>();
                //GDatawithIdP = db.Database.SqlQuery<GraphDataWithIdPercentage>(string.Format(sqlQ.NationalBrachComp, Projectid, CompareQuarter)).ToList();

                GDatawithIdP = sqlQ.NationalLevelAndRegionalLevel(Convert.ToInt32(Projectid), CompareQuarter, null, username);
                NationalQ0 = JsonConvert.SerializeObject(GDatawithIdP.Select(x => new { x.y, x.id }));
                NationalName = JsonConvert.SerializeObject(GDatawithIdP.Select(x => x.name));

                //GDatawithIdP = db.Database.SqlQuery<GraphDataWithIdPercentage>(string.Format(sqlQ.NationalBrachComp, Projectid, CompareQuarterLocal)).ToList();
                //GDatawithIdP = fillGraphDatawithPercentage(GDatawithId);
                if (CompareQuarter != CompareQuarterLocal)
                    GDatawithIdP = sqlQ.NationalLevelAndRegionalLevel(Convert.ToInt32(Projectid), CompareQuarterLocal, null, username);

                NationalQ1 = JsonConvert.SerializeObject(GDatawithIdP.Select(x => new { x.y, x.id }).ToArray());

                //GDatawithIdP = db.Database.SqlQuery<GraphDataWithIdPercentage>(string.Format(sqlQ.RegionBranchComp, Projectid, CompareQuarter, "3")).ToList();
                //GDatawithIdP = fillGraphDatawithPercentage(GDatawithId);
                GDatawithIdP = sqlQ.NationalLevelAndRegionalLevel(Convert.ToInt32(Projectid), CompareQuarter, 3, username);

                NorthQ0 = JsonConvert.SerializeObject(GDatawithIdP.Select(x => new { x.y, x.id }));
                NorthName = JsonConvert.SerializeObject(GDatawithIdP.Select(x => x.name));

                //GDatawithIdP = db.Database.SqlQuery<GraphDataWithIdPercentage>(string.Format(sqlQ.RegionBranchComp, Projectid, CompareQuarterLocal, "3")).ToList();
                //GDatawithIdP = fillGraphDatawithPercentage(GDatawithId);
                if (CompareQuarter != CompareQuarterLocal)
                    GDatawithIdP = sqlQ.NationalLevelAndRegionalLevel(Convert.ToInt32(Projectid), CompareQuarterLocal, 3, username); ;

                NorthQ1 = JsonConvert.SerializeObject(GDatawithIdP.Select(x => new { x.y, x.id }));

                //GDatawithIdP = db.Database.SqlQuery<GraphDataWithIdPercentage>(string.Format(sqlQ.RegionBranchComp, Projectid, CompareQuarter, "1")).ToList();
                //GDatawithIdP = fillGraphDatawithPercentage(GDatawithId);
                GDatawithIdP = sqlQ.NationalLevelAndRegionalLevel(Convert.ToInt32(Projectid), CompareQuarter, 1, username);

                SouthQ0 = JsonConvert.SerializeObject(GDatawithIdP.Select(x => new { x.y, x.id }));
                SouthName = JsonConvert.SerializeObject(GDatawithIdP.Select(x => x.name));

                //GDatawithIdP = db.Database.SqlQuery<GraphDataWithIdPercentage>(string.Format(sqlQ.RegionBranchComp, Projectid, CompareQuarterLocal, "1")).ToList();
                //GDatawithIdP = fillGraphDatawithPercentage(GDatawithId);
                if (CompareQuarter != CompareQuarterLocal)
                    GDatawithIdP = sqlQ.NationalLevelAndRegionalLevel(Convert.ToInt32(Projectid), CompareQuarterLocal, 1, username);

                SouthQ1 = JsonConvert.SerializeObject(GDatawithIdP.Select(x => new { x.y, x.id }));

                //GDatawithIdP = db.Database.SqlQuery<GraphDataWithIdPercentage>(string.Format(sqlQ.RegionBranchComp, Projectid, CompareQuarter, "2")).ToList();
                //GDatawithIdP = fillGraphDatawithPercentage(GDatawithId);
                GDatawithIdP = sqlQ.NationalLevelAndRegionalLevel(Convert.ToInt32(Projectid), CompareQuarter, 2, username);
                CentralQ0 = JsonConvert.SerializeObject(GDatawithIdP.Select(x => new { x.y, x.id }));
                CentralName = JsonConvert.SerializeObject(GDatawithIdP.Select(x => x.name));

                //GDatawithIdP = db.Database.SqlQuery<GraphDataWithIdPercentage>(string.Format(sqlQ.RegionBranchComp, Projectid, CompareQuarterLocal, "2")).ToList();
                //GDatawithIdP = fillGraphDatawithPercentage(GDatawithId);
                if (CompareQuarter != CompareQuarterLocal)
                    GDatawithIdP = sqlQ.NationalLevelAndRegionalLevel(Convert.ToInt32(Projectid), CompareQuarterLocal, 2, username);
                CentralQ1 = JsonConvert.SerializeObject(GDatawithIdP.Select(x => new { x.y, x.id }));
            }
            else
            {
                GDatawithIdP = sqlQ.NationalLevelAndRegionalLevelBranch(Convert.ToInt32(Projectid), CompareQuarter, custUser[0].CustomerBranchID, username, GetAllAdminUser());
                NationalQ0 = JsonConvert.SerializeObject(GDatawithIdP.Select(x => new { x.y, x.id, x.intenal }));
                NationalName = JsonConvert.SerializeObject(GDatawithIdP.Select(x => x.name));

                if (CompareQuarter != CompareQuarterLocal)
                    GDatawithIdP = sqlQ.NationalLevelAndRegionalLevelBranch(Convert.ToInt32(Projectid), CompareQuarterLocal, custUser[0].CustomerBranchID, username, GetAllAdminUser());

                NationalQ1 = JsonConvert.SerializeObject(GDatawithIdP.Select(x => new { x.y, x.id, x.intenal }).ToArray());

            }


            return Json(new
            {
                NationalQ0 = NationalQ0,
                NationalQ1 = NationalQ1,
                NationalName = NationalName,
                SouthQ0 = SouthQ0,
                SouthQ1 = SouthQ1,
                SouthName = SouthName,
                NorthQ0 = NorthQ0,
                NorthQ1 = NorthQ1,
                NorthName = NorthName,
                CentralQ0 = CentralQ0,
                CentralQ1 = CentralQ1,
                CentralName = CentralName
            },
                JsonRequestBehavior.AllowGet);

        }


        [Authorize]
        public JsonResult FilterThirdRow(string CompareQuarter, string Projectid, string dealerid)
        {
            string nationalCustomerSectionPie = "", lastValue = "", lastname = "", firstValue = "", firstname = "";
            List<GraphData> GraphDataLst = new List<GraphData>();
            List<GraphDataPercentage> GraphDataLstPer = new List<GraphDataPercentage>();

            List<PieData> piDataLst = new List<PieData>();
            List<PieDataTotal> piDataTotalLst = new List<PieDataTotal>();
            SqlQuriesDashboard sqlQ = new SqlQuriesDashboard();
            string sql = "";
            int userid = Convert.ToInt32(User.Identity.GetUserId());

            var userRoles = UserManager.GetRoles(userid);
            List<CustomerUser> custUser = db.CustomerUser.Where(x => x.UserID == userid).ToList();
            if (custUser.Count != 0 && dealerid == "-1" && custUser[0].CustomerBranchID != 1)
            {
                dealerid = custUser[0].CustomerBranchID.ToString();
            }

            if (dealerid == "-1")
            {
                //sql = sqlQ.nationalCustomerSectionPie;
                //sql = string.Format(sql, Projectid.ToString(), CompareQuarter);//To retrive National Customer Section Avg
                //piDataLst = db.Database.SqlQuery<PieData>(sql).ToList<PieData>();

                //piDataLst = sqlQ.SectionIdPie(Convert.ToInt32(Projectid), CompareQuarter, null);
                piDataTotalLst = sqlQ.SectionIdPie(Convert.ToInt32(Projectid), CompareQuarter, null, username);
                nationalCustomerSectionPie = JsonConvert.SerializeObject(piDataTotalLst);

                //sql = sqlQ.Top5NationalCustomerSection;
                //sql = string.Format(sql, Projectid.ToString(), CompareQuarter, "asc");//To retrive Top 5 Customer Section
                //GraphDataLst = db.Database.SqlQuery<GraphData>(sql).ToList<GraphData>();
                GraphDataLstPer = sqlQ.NationTop5(Convert.ToInt32(Projectid), CompareQuarter, "asc", null, username);
                lastname = JsonConvert.SerializeObject(GraphDataLstPer.Select(x => x.name));
                lastValue = JsonConvert.SerializeObject(GraphDataLstPer.Select(x => x.y));

                //sql = sqlQ.Top5NationalCustomerSection;
                //sql = string.Format(sql, Projectid.ToString(), CompareQuarter, "desc");//To retrive Last 5 Customer Section
                //GraphDataLst = db.Database.SqlQuery<GraphData>(sql).ToList<GraphData>();
                GraphDataLstPer = sqlQ.NationTop5(Convert.ToInt32(Projectid), CompareQuarter, "desc", null, username);
                firstname = JsonConvert.SerializeObject(GraphDataLstPer.Select(x => x.name));
                firstValue = JsonConvert.SerializeObject(GraphDataLstPer.Select(x => x.y));
            }
            else
            {
                //sql = sqlQ.DealerCustomerSectionPie;
                //sql = string.Format(sql, Projectid.ToString(), CompareQuarter, dealerid.ToString());//To retrive National Customer Section Avg
                //piDataLst = db.Database.SqlQuery<PieData>(sql).ToList<PieData>();
                piDataTotalLst = sqlQ.SectionIdPie(Convert.ToInt32(Projectid), CompareQuarter, Convert.ToInt32(dealerid), username);

                nationalCustomerSectionPie = JsonConvert.SerializeObject(piDataTotalLst);

                //sql = sqlQ.Top5DealerCustomerSection;
                //sql = string.Format(sql, Projectid.ToString(), CompareQuarter, dealerid.ToString(), "asc");//To retrive Top 5 Customer Section
                //GraphDataLst = db.Database.SqlQuery<GraphData>(sql).ToList<GraphData>();

                GraphDataLstPer = sqlQ.NationTop5(Convert.ToInt32(Projectid), CompareQuarter, "asc", Convert.ToInt32(dealerid), username);
                lastname = JsonConvert.SerializeObject(GraphDataLstPer.Select(x => x.name));
                lastValue = JsonConvert.SerializeObject(GraphDataLstPer.Select(x => x.y));

                //sql = sqlQ.Top5DealerCustomerSection;
                //sql = string.Format(sql, Projectid.ToString(), CompareQuarter, dealerid.ToString(), "desc");//To retrive Last 5 Customer Section
                //GraphDataLst = db.Database.SqlQuery<GraphData>(sql).ToList<GraphData>();
                GraphDataLstPer = sqlQ.NationTop5(Convert.ToInt32(Projectid), CompareQuarter, "desc", Convert.ToInt32(dealerid), username);
                firstname = JsonConvert.SerializeObject(GraphDataLstPer.Select(x => x.name));
                firstValue = JsonConvert.SerializeObject(GraphDataLstPer.Select(x => x.y));
            }

            return Json(new { nationalCustomerSectionPie = nationalCustomerSectionPie, lastname = lastname, lastValue = lastValue, firstname = firstname, firstValue = firstValue }, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        public JsonResult GetInternalCityDetails(int Cityid, int ProjectID, string CompareQuarter)
        {
            List<GraphDataWithIdPercentage> GraphDataLst = new List<GraphDataWithIdPercentage>();
            int userid = Convert.ToInt32(User.Identity.GetUserId());
            var userRoles = UserManager.GetRoles(userid);
            List<CustomerUser> custUser = db.CustomerUser.Where(x => x.UserID == userid).ToList();

            string name = "[]", y = "[]";
            string sql = "";
            if (userRoles.Contains("Survey Admin") || userRoles.Contains("Admin"))
            {
                SqlQuriesDashboard sqlQ = new SqlQuriesDashboard();
                if (Cityid != 0)
                {
                    //             sql = @"	Select Cb.Name name,
                    // 		  (SELECT round((CONVERT(float, SUM(SubSum.BaseCount))/Count(SubSum.BaseCount))*100,2) y from 
                    //         	(SELECT mp.FieldID AS FieldID, 
                    //         		(SELECT COUNT(*) FROM surveydata msd 
                    //         		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
                    //         		AND msd.sbjnum IN 
                    //         			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                    //         			WHERE ISNULL(s.Test, 0) = 0 
                    //			and sd.FieldId = 49756 AND sd.FieldValue = Convert(nvarchar, Cb.ID) 
                    //			GROUP BY s.sbjnum ) ) BaseCount 
                    //         	FROM projectfieldsample mp
                    //	inner join ProjectField pf on pf.ID=mp.FieldID
                    //	 WHERE pf.ProjectID={0}  
                    //	) AS SubSum            
                    //) y
                    //from CustomerBranch Cb where Cb.CityID= {1} and Cb.ID != 1";

                    //             GraphDataLst = db.Database.SqlQuery<GraphDataWithIdPercentage>(string.Format(sql, ProjectID.ToString(), Cityid.ToString())).ToList();
                    int cityID = Convert.ToInt32(Cityid);
                    //GraphDataLst = sqlQ.CityLevel(ProjectID, CompareQuarter, Cityid);
                    if (custUser.Count == 0 || custUser[0].CustomerBranchID == 1)
                    {
                        GraphDataLst = sqlQ.CityLevel(Convert.ToInt32(ProjectID), CompareQuarter, Cityid, username);
                    }
                    else
                    {
                        GraphDataLst = sqlQ.CityLevelDealer(Convert.ToInt32(ProjectID), CompareQuarter, Cityid, Convert.ToInt32(custUser[0].CustomerBranchID), GetAllAdminUser());
                    }

                }
                else
                {
                    //             sql = @"			Select Cb.Name name,
                    // 		  (SELECT SUM(SubSum.BaseCount) y from 
                    //         	(SELECT mp.FieldID AS FieldID, 
                    //         		(SELECT COUNT(*) FROM surveydata msd 
                    //         		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
                    //         		AND msd.sbjnum IN 
                    //         			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                    //         			WHERE ISNULL(s.Test, 0) = 0 
                    //			and sd.FieldId = 49756 AND sd.FieldValue = Convert(nvarchar, Cb.ID) 
                    //			GROUP BY s.sbjnum ) ) BaseCount 
                    //         	FROM projectfieldsample mp
                    //		inner join ProjectField pf on pf.ID=mp.FieldID
                    //	 WHERE pf.ProjectID={0}  ) AS SubSum  
                    //         ) y
                    //from CustomerBranch Cb where Cb.CityID not in ( {1} )";


                    //             GraphDataLst = db.Database.SqlQuery<GraphDataWithIdPercentage>(string.Format(sql, ProjectID.ToString(), "1,2071,5558")).ToList();

                    int cityID = Convert.ToInt32(Cityid);
                    //GraphDataLst = sqlQ.CityLevel(ProjectID, CompareQuarter, -1);
                    if (custUser[0].CustomerBranchID == 1)
                    {
                        GraphDataLst = sqlQ.CityLevel(Convert.ToInt32(ProjectID), CompareQuarter, -1, username);
                    }
                    else
                    {
                        GraphDataLst = sqlQ.CityLevelDealer(Convert.ToInt32(ProjectID), CompareQuarter, cityID, Convert.ToInt32(custUser[0].CustomerBranchID), GetAllAdminUser());
                    }
                }


                y = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.y).ToArray());
                name = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.name.Trim()).ToArray());
            }

            return Json(new { name = name, y = y }, JsonRequestBehavior.AllowGet);
        }


        //     public JsonResult GetOverAllDetailInternally(int id)
        //     {

        //         string DealerHighScoreDetailX, DealerHighScoreDetailName, DealerLowScoreDetailX, DealerLowScoreDetailName, OverAllStatusPie;

        //         string sql = @"
        //                with cte (y,name) as ( SELECT SUM(SubSum.BaseCount) as y, pfs.Name as name from 
        //         	(SELECT mp.FieldID AS FieldID, 
        //         		(SELECT COUNT(*) FROM surveydata msd 
        //         		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
        //         		AND msd.sbjnum IN 
        //         			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
        //         			WHERE ISNULL(s.Test, 0) = 0 GROUP BY s.sbjnum ) ) BaseCount 
        //         	FROM projectfieldsample mp) AS SubSum  
        //         INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
        //         INNER JOIN ProjectFieldSection pfs on pfs.ID = pf.SectionID
        //         WHERE pf.ProjectID={0} 
        //         GROUP BY pfs.ID,pfs.Name)   

        //select (CAST(cte.y AS float) / CAST((select SUM(cte.y) from cte) as FLOAT))*100 as y,cte.name as name from cte
        //         ";
        //         List<PieData> piDataLst = db.Database.SqlQuery<PieData>(string.Format(sql, id.ToString())).ToList<PieData>();
        //         OverAllStatusPie = JsonConvert.SerializeObject(piDataLst);

        //         sql = @"SELECT Top 5 SUM(SubSum.BaseCount) as y, pf.Title as name from 
        //         	(SELECT mp.FieldID AS FieldID, 
        //         		(SELECT COUNT(*) FROM surveydata msd 
        //         		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
        //         		AND msd.sbjnum IN 
        //         			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
        //         			WHERE ISNULL(s.Test, 0) = 0 GROUP BY s.sbjnum ) ) BaseCount 
        //         	FROM projectfieldsample mp) AS SubSum  
        //         INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
        //         WHERE pf.ProjectID={0} 
        //group by pf.id,pf.Title
        //order by y desc";
        //         List<GraphData> GraphDataLst = db.Database.SqlQuery<GraphData>(string.Format(sql, id.ToString())).ToList<GraphData>();
        //         DealerHighScoreDetailX = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.y).ToArray());
        //         DealerHighScoreDetailName = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.name).ToArray());

        //         sql = @"SELECT Top 5 SUM(SubSum.BaseCount) as y, pf.Title as name from 
        //         	(SELECT mp.FieldID AS FieldID, 
        //         		(SELECT COUNT(*) FROM surveydata msd 
        //         		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
        //         		AND msd.sbjnum IN 
        //         			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
        //         			WHERE ISNULL(s.Test, 0) = 0 GROUP BY s.sbjnum ) ) BaseCount 
        //         	FROM projectfieldsample mp) AS SubSum  
        //         INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
        //         WHERE pf.ProjectID={0} 
        //group by pf.id,pf.Title
        //order by y asc";
        //         GraphDataLst = db.Database.SqlQuery<GraphData>(string.Format(sql, id.ToString())).ToList<GraphData>();
        //         DealerLowScoreDetailX = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.y).ToArray());
        //         DealerLowScoreDetailName = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.name).ToArray());


        //         return Json(new { DealerHighScoreDetailvalue = DealerHighScoreDetailX, DealerHighScoreDetailName = DealerHighScoreDetailName, DealerLowScoreDetailValue = DealerLowScoreDetailX, DealerLowScoreDetailName = DealerLowScoreDetailName, OverAllStatusPie = OverAllStatusPie });
        //     }


        [Authorize]
        public JsonResult GetInternalDetailsBranch(int Projectid, string CompareQuarter, string CompareQuarter1, int dealerid, bool intenal)
        {
            string sql = "";
            int userid = Convert.ToInt32(User.Identity.GetUserId());
            var userRoles = UserManager.GetRoles(userid);
            List<CustomerUser> custUser = db.CustomerUser.Where(x => x.UserID == userid).ToList();
            if (custUser.Count > 0 || userRoles.Contains("Admin"))
            {

                int? CustomerDealer = (custUser.Count == 0 || custUser == null) ? null : (int?)custUser[0].CustomerBranchID;

                SqlQuriesDashboard sqlQ = new SqlQuriesDashboard();
                List<PieData> piDataLst = new List<PieData>();
                List<PieDataTotal> piDataTotalLst = new List<PieDataTotal>();
                List<GraphData> GraphDataLst = new List<GraphData>();
                List<GraphDataPercentage> GdataPer = new List<GraphDataPercentage>();
                string DealerCustomerSectionPie = "", lastname = "", lastValue = "", firstname = "", firstValue = "", internalname = "";
                string CompValue = "", CompValue1 = "";
                string usernameString = intenal ? username : GetAllAdminUser();

                piDataTotalLst = sqlQ.SectionIdPie(Convert.ToInt32(Projectid), CompareQuarter1, dealerid, usernameString);
                DealerCustomerSectionPie = JsonConvert.SerializeObject(piDataTotalLst);

                GdataPer = sqlQ.NationTop5(Convert.ToInt32(Projectid), CompareQuarter1, "asc", Convert.ToInt32(dealerid), usernameString);
                lastname = JsonConvert.SerializeObject(GdataPer.Select(x => x.name));
                lastValue = JsonConvert.SerializeObject(GdataPer.Select(x => x.y));

                GdataPer = sqlQ.NationTop5(Convert.ToInt32(Projectid), CompareQuarter1, "desc", Convert.ToInt32(dealerid), usernameString);
                firstname = JsonConvert.SerializeObject(GdataPer.Select(x => x.name));
                firstValue = JsonConvert.SerializeObject(GdataPer.Select(x => x.y));

                GdataPer = sqlQ.SectionIdGrph(Projectid, CompareQuarter, dealerid, usernameString);

                internalname = JsonConvert.SerializeObject(GdataPer.Select(x => x.name));
                CompValue = JsonConvert.SerializeObject(GdataPer.Select(x => x.y));

                GdataPer = sqlQ.SectionIdGrph(Projectid, CompareQuarter1, dealerid, usernameString);
                CompValue1 = JsonConvert.SerializeObject(GdataPer.Select(x => x.y));

                return Json(new { internalY1 = CompValue1, internalY = CompValue, internalname = internalname, piData = DealerCustomerSectionPie, LowxValue = lastValue.ToString(), LowyValue = lastname.ToString(), FirstxValue = firstValue.ToString(), FirstyValue = firstname.ToString() }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { internalY1 = "[]", internalY = "[]", internalname = "[]", piData = "[]", LowxValue = "[]", LowyValue = "[]", FirstxValue = "[]", FirstyValue = "[]" }, JsonRequestBehavior.AllowGet);

        }


        [Authorize]
        public JsonResult GetInternalDetails(int Projectid, string CompareQuarter, string CompareQuarter1, int dealerid)
        {
            string sql = "";
            int userid = Convert.ToInt32(User.Identity.GetUserId());
            var userRoles = UserManager.GetRoles(userid);
            List<CustomerUser> custUser = db.CustomerUser.Where(x => x.UserID == userid).ToList();
            if (custUser.Count > 0 || userRoles.Contains("Admin"))
            {

                int? CustomerDealer = (custUser.Count == 0 || custUser == null) ? null : (int?)custUser[0].CustomerBranchID;
                if (userRoles.Contains("Admin") || (CustomerDealer != null && dealerid == (int)CustomerDealer))
                {

                    #region
                    //         sql = @"		  SELECT  Cast( SUM(SubSum.BaseCount) as float) 'y', pfs.name as name,pfs.ID from 
                    //         	(SELECT mp.FieldID AS FieldID, 
                    //         		(SELECT COUNT(*) FROM surveydata msd 
                    //         		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
                    //         		AND msd.sbjnum IN 
                    //         			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                    //         			WHERE ISNULL(s.Test, 0) = 0 
                    //			and sd.FieldId = 49756 AND sd.FieldValue = '{0}'
                    //			GROUP BY s.sbjnum ) ) BaseCount 
                    //         	FROM projectfieldsample mp) AS SubSum  
                    //         INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
                    //         INNER JOIN ProjectFieldSection pfs on pfs.ID = pf.SectionID
                    //         WHERE pf.ProjectID={1} 
                    //GROUP BY pfs.name ,pfs.ID";

                    //         List<PieData> piDataLst = db.Database.SqlQuery<PieData>(string.Format(sql, Dealerid.ToString(), Projectid.ToString())).ToList<PieData>();
                    //         pi = JsonConvert.SerializeObject(piDataLst);


                    //         sql = @"		  SELECT Top 5 SUM(SubSum.BaseCount) 'y', pf.title as name,pf.ID from 
                    //         	(SELECT mp.FieldID AS FieldID, 
                    //         		(SELECT COUNT(*) FROM surveydata msd 
                    //         		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
                    //         		AND msd.sbjnum IN 
                    //         			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                    //         			WHERE ISNULL(s.Test, 0) = 0 
                    //			and sd.FieldId = 49756 AND sd.FieldValue = '{0}'
                    //			GROUP BY s.sbjnum ) ) BaseCount 
                    //         	FROM projectfieldsample mp) AS SubSum  
                    //         INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
                    //         INNER JOIN ProjectFieldSection pfs on pfs.ID = pf.SectionID
                    //         WHERE pf.ProjectID={1} 
                    //GROUP BY pf.title ,pf.ID
                    //         Order by y asc";



                    //         List<GraphData> GraphDataLst = new List<GraphData>();
                    //         GraphDataLst = db.Database.SqlQuery<GraphData>(string.Format(sql, Dealerid.ToString(), Projectid.ToString())).ToList<GraphData>();
                    //         Lastx = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.y).ToArray());
                    //         Lasty = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.name.Trim()).ToArray());


                    //         sql = @"		  SELECT Top 5 SUM(SubSum.BaseCount) 'y', pf.title as name,pf.ID from 
                    //         	(SELECT mp.FieldID AS FieldID, 
                    //         		(SELECT COUNT(*) FROM surveydata msd 
                    //         		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
                    //         		AND msd.sbjnum IN 
                    //         			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                    //         			WHERE ISNULL(s.Test, 0) = 0 
                    //			and sd.FieldId = 49756 AND sd.FieldValue = '{0}'
                    //			GROUP BY s.sbjnum ) ) BaseCount 
                    //         	FROM projectfieldsample mp) AS SubSum  
                    //         INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
                    //         INNER JOIN ProjectFieldSection pfs on pfs.ID = pf.SectionID
                    //         WHERE pf.ProjectID={1} 
                    //GROUP BY pf.title ,pf.ID
                    //         Order by y desc";

                    //         GraphDataLst = db.Database.SqlQuery<GraphData>(string.Format(sql, Dealerid.ToString(), Projectid.ToString())).ToList<GraphData>();
                    //         Firstx = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.y).ToArray());
                    //         Firsty = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.name.Trim()).ToArray());

                    //         sql = @"			   SELECT  SUM(SubSum.BaseCount) 'y', pfs.Name as name,pfs.ID from 
                    //         	(SELECT mp.FieldID AS FieldID, 
                    //         		(SELECT COUNT(*) FROM surveydata msd 
                    //         		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
                    //         		AND msd.sbjnum IN 
                    //         			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                    //         			WHERE ISNULL(s.Test, 0) = 0 
                    //			and sd.FieldId = 49756 AND sd.FieldValue = '{0}'
                    //			GROUP BY s.sbjnum )

                    //			ANd msd.sbjnum IN	--Quarter
                    //         				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                    //         				WHERE ISNULL(s.Test, 0) = 0 
                    //				and sd.FieldId = 49757 AND sd.FieldValue IN ('1','2','3','4')
                    //				GROUP BY s.sbjnum ) 

                    //			 ) BaseCount 
                    //         	FROM projectfieldsample mp) AS SubSum  
                    //         INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
                    //         INNER JOIN ProjectFieldSection pfs on pfs.ID = pf.SectionID
                    //         WHERE pf.ProjectID={1}
                    //GROUP BY pfs.name ,pfs.ID
                    //         ";

                    //         GraphDataLst = db.Database.SqlQuery<GraphData>(string.Format(sql, Dealerid.ToString(), Projectid.ToString())).ToList<GraphData>();
                    //         string internaly = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.y).ToArray());
                    //         string internalname = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.name.Trim()).ToArray());

                    //         sql = @"			   SELECT  SUM(SubSum.BaseCount) 'y', pfs.Name as name,pfs.ID from 
                    //         	(SELECT mp.FieldID AS FieldID, 
                    //         		(SELECT COUNT(*) FROM surveydata msd 
                    //         		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
                    //         		AND msd.sbjnum IN 
                    //         			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                    //         			WHERE ISNULL(s.Test, 0) = 0 
                    //			and sd.FieldId = 49756 AND sd.FieldValue = '{0}'
                    //			GROUP BY s.sbjnum )

                    //			ANd msd.sbjnum IN	--Quarter
                    //         				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                    //         				WHERE ISNULL(s.Test, 0) = 0 
                    //				and sd.FieldId = 49757 AND sd.FieldValue IN ('1','4')
                    //				GROUP BY s.sbjnum ) 

                    //			 ) BaseCount 
                    //         	FROM projectfieldsample mp) AS SubSum  
                    //         INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
                    //         INNER JOIN ProjectFieldSection pfs on pfs.ID = pf.SectionID
                    //         WHERE pf.ProjectID={1}
                    //GROUP BY pfs.name ,pfs.ID
                    //         ";

                    //         GraphDataLst = db.Database.SqlQuery<GraphData>(string.Format(sql, Dealerid.ToString(), Projectid.ToString())).ToList<GraphData>();
                    //         string internaly1 = JsonConvert.SerializeObject(GraphDataLst.Select(x => x.y).ToArray());
                    //return Json(new { internalY1 = internaly1, internalY = internaly, internalname = internalname, piData = pi, LowxValue = Lastx.ToString(), LowyValue = Lasty.ToString(), FirstxValue = Firstx.ToString(), FirstyValue = Firsty.ToString() }, JsonRequestBehavior.AllowGet);
                    #endregion
                    SqlQuriesDashboard sqlQ = new SqlQuriesDashboard();
                    List<PieData> piDataLst = new List<PieData>();
                    List<PieDataTotal> piDataTotalLst = new List<PieDataTotal>();
                    List<GraphData> GraphDataLst = new List<GraphData>();
                    List<GraphDataPercentage> GdataPer = new List<GraphDataPercentage>();
                    string DealerCustomerSectionPie = "", lastname = "", lastValue = "", firstname = "", firstValue = "", internalname = "";
                    string CompValue = "", CompValue1 = "";

                    //sql = sqlQ.DealerCustomerSectionPie;
                    //sql = string.Format(sql, Projectid.ToString(), CompareQuarter, dealerid.ToString());//To retrive National Customer Section Avg
                    //piDataLst = db.Database.SqlQuery<PieData>(sql).ToList<PieData>();
                    piDataTotalLst = sqlQ.SectionIdPie(Convert.ToInt32(Projectid), CompareQuarter1, dealerid, username);

                    DealerCustomerSectionPie = JsonConvert.SerializeObject(piDataTotalLst);

                    //sql = sqlQ.Top5DealerCustomerSection;
                    //sql = string.Format(sql, Projectid.ToString(), CompareQuarter, dealerid.ToString(), "asc");//To retrive Top 5 Customer Section
                    //GraphDataLst = db.Database.SqlQuery<GraphData>(sql).ToList<GraphData>();
                    GdataPer = sqlQ.NationTop5(Convert.ToInt32(Projectid), CompareQuarter1, "asc", Convert.ToInt32(dealerid), username);

                    lastname = JsonConvert.SerializeObject(GdataPer.Select(x => x.name));
                    lastValue = JsonConvert.SerializeObject(GdataPer.Select(x => x.y));

                    //sql = sqlQ.Top5DealerCustomerSection;
                    //sql = string.Format(sql, Projectid.ToString(), CompareQuarter, dealerid.ToString(), "desc");//To retrive Last 5 Customer Section
                    //GraphDataLst = db.Database.SqlQuery<GraphData>(sql).ToList<GraphData>();
                    GdataPer = sqlQ.NationTop5(Convert.ToInt32(Projectid), CompareQuarter1, "desc", Convert.ToInt32(dealerid), username);

                    firstname = JsonConvert.SerializeObject(GdataPer.Select(x => x.name));
                    firstValue = JsonConvert.SerializeObject(GdataPer.Select(x => x.y));

                    //sql = string.Format(sqlQ.intenalDealerDetail, Projectid.ToString(), dealerid.ToString(), CompareQuarter);
                    //GdataPer = db.Database.SqlQuery<GraphDataPercentage>(sql).ToList();
                    GdataPer = sqlQ.SectionIdGrph(Projectid, CompareQuarter, dealerid, username);

                    internalname = JsonConvert.SerializeObject(GdataPer.Select(x => x.name));
                    CompValue = JsonConvert.SerializeObject(GdataPer.Select(x => x.y));

                    //sql = string.Format(sqlQ.intenalDealerDetail, Projectid.ToString(), dealerid.ToString(), CompareQuarter1);
                    //GdataPer = db.Database.SqlQuery<GraphDataPercentage>(sql).ToList();
                    GdataPer = sqlQ.SectionIdGrph(Projectid, CompareQuarter1, dealerid, username);
                    CompValue1 = JsonConvert.SerializeObject(GdataPer.Select(x => x.y));

                    return Json(new { internalY1 = CompValue1, internalY = CompValue, internalname = internalname, piData = DealerCustomerSectionPie, LowxValue = lastValue.ToString(), LowyValue = lastname.ToString(), FirstxValue = firstValue.ToString(), FirstyValue = firstname.ToString() }, JsonRequestBehavior.AllowGet);

                }
            }
            return Json(new { internalY1 = "[]", internalY = "[]", internalname = "[]", piData = "[]", LowxValue = "[]", LowyValue = "[]", FirstxValue = "[]", FirstyValue = "[]" }, JsonRequestBehavior.AllowGet);

        }


        [Authorize(Roles = "Admin")]
        public ActionResult SurveyDetailAll(int ProjectId)
        {
            ViewBag.Projectid = ProjectId;
            int id = 0;
            string sql = "";
            int userid = Convert.ToInt32(User.Identity.GetUserId());
            var userRoles = UserManager.GetRoles(userid);
            List<CustomerUser> custUser = db.CustomerUser.Where(x => x.UserID == userid).ToList();
            string userids = "";

            if (custUser.Count > 0)
            {
                int cusBrnchid = custUser[0].CustomerBranchID;

                List<string> useridlst = db.CustomerUser.Where(x => x.CustomerBranchID == cusBrnchid).Select(x => x.UserID.ToString()).ToList();
                userids = string.Join(",", useridlst);
                List<string> tempstrlst = idenDB.Database.SqlQuery<string>("select Replace( username,'@pwd.com','') from AspNetUsers where id in (" + userids + ") ").ToList<string>();
                username = "'" + string.Join("','", tempstrlst) + "'";

            }


            if (User.IsInRole("Admin"))
            {
                //SelectList dealer = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.ID, x.Name }).OrderBy(f => f.Name), "ID", "Name", null);
                //ViewBag.dealer = dealer;
                //string sql = "";
                //List<SurveyDetails> SurDetail = new List<SurveyDetails>();

                //List<SurveyDropDown> surveyDropDown = new List<SurveyDropDown>();


                //SelectList surveyDropDown1 = new SelectList(surveyDropDown, "Value", "Name", null);
                //ViewBag.SurveyDropDown = surveyDropDown1;

                //ViewBag.SurDetail = SurDetail;
            }

            return View();
        }

        [Authorize]
        public ActionResult _SurveyDetailTableAll(int projectid, int quarterId, string tor = "", string internalOrExternal = "")
        {
            string sql = "";
            string torFunc = "fnSurveyValues";
            string torFilter = " AND fs.id <> 1210";
            //Type Of Report: C = COVID, F = Facility Audit
            if (tor == "C")
            {
                torFilter = " AND fs.id = 1210";
                torFunc = "fnSurveyValuesCOVID";
            }

            List<CustomerBranch> CustBrnchlst;
            
            //    string adminUsers = GetAllAdminUser();
            CustBrnchlst = db.CustomerBranch.Where(x => x.ID != 1).ToList();

            SqlQuriesDashboard sqlQ = new SqlQuriesDashboard();
            List<SurveyDetails> SurDetaillst = new List<SurveyDetails>(), mainSurDetaillst = new List<SurveyDetails>();

            foreach (var item in CustBrnchlst)
            {
                string surveyors = username;
                if (internalOrExternal.Equals("I"))
                {
                    List<string> useridlst = db.CustomerUser.Where(x => x.CustomerBranchID == item.ID).Select(x => x.UserID.ToString()).ToList();
                    var userids = string.Join(",", useridlst);
                    List<string> tempstrlst = idenDB.Database.SqlQuery<string>("select LEFT([Email], CHARINDEX('@', [Email])-1) from AspNetUsers where id in (" + userids + ") ").ToList<string>();
                    surveyors = "'" + string.Join("','", tempstrlst) + "'";
                }

                SurDetaillst = new List<SurveyDetails>();
                SurDetaillst = sqlQ.GetLastSurveyDetail(surveyors, projectid, item.ID, quarterId, torFilter, torFunc);

                SurDetaillst.ForEach(x =>
                    x.Dealername = item.Name
                    );
                mainSurDetaillst.AddRange(SurDetaillst);
            }
            mainSurDetaillst = mainSurDetaillst.OrderBy(x => x.fsDisplayOrder).ThenBy(x => x.pfDisplayOrder).ThenBy(x => x.id).ToList();
            List<int?> Answerids = mainSurDetaillst.Select(x => x.AnswerId).Distinct<int?>().ToList();
            for (int indx = 0; indx < Answerids.Count(); indx++)
            {
                List<SurveyDetails> tempSur = mainSurDetaillst.Where(x => x.AnswerId == Answerids[indx]).ToList();
                SurveyDetailsAllDealer SurveyDetailAllTemp = new SurveyDetailsAllDealer();

                SurveyDetailAllTemp.Answer = tempSur[0].Answer;
                SurveyDetailAllTemp.DealerLst = new List<DetailsDealersSurvey>();
                SurveyDetailAllTemp.FieldType = tempSur[0].FieldType;
                SurveyDetailAllTemp.id = tempSur[0].id;
                SurveyDetailAllTemp.ParentFieldID = tempSur[0].ParentFieldID;
                SurveyDetailAllTemp.Pid = tempSur[0].Pid;
                SurveyDetailAllTemp.Section_name = tempSur[0].Section_name;

                foreach (var item in CustBrnchlst)
                {
                    DetailsDealersSurvey detailTemp = new DetailsDealersSurvey();
                    SurveyDetails Surtemp = tempSur.Where(x => x.Dealername == item.Name).FirstOrDefault();

                    detailTemp.dealername = item.Name;
                    detailTemp.name = tempSur[0].name;
                    if (Surtemp != null)
                    {
                        detailTemp.options = Surtemp.options;
                        detailTemp.Score = Surtemp.Score;
                        detailTemp.Marked = (int)(Surtemp.Marked ?? 0);
                    }
                    SurveyDetailAllTemp.DealerLst.Add(detailTemp);
                }

                SurveyDealerAll.Add(SurveyDetailAllTemp);
            }


            sql = @"select fs.ID id, fs.name AS Section_name,fs.DisplayOrder 
from ProjectFieldSection fs 
where ProjectId={0} and  fs.ID not in (1209,1208) {1} 
order by fs.displayOrder";

            List<SurveyDetails> FieldSectionDetail = new List<SurveyDetails>();
            FieldSectionDetail = db.Database.SqlQuery<SurveyDetails>(string.Format(sql, projectid, torFilter)).ToList<SurveyDetails>();

            ViewBag.Dealerlst = CustBrnchlst;
            ViewBag.SurDetailChild = SurveyDealerAll;
            ViewBag.SurDetail = FieldSectionDetail;
            return View();
        }

        [Authorize]
        public FileResult ExportData(string projectid)
        {


            string sql = @"select fs.ID id, fs.name AS Section_name,fs.DisplayOrder 
from ProjectFieldSection fs 
where ProjectId={0} and  fs.ID not in (1209,1208)
order by fs.displayOrder";

            List<CustomerBranch> Dealerlst = db.CustomerBranch.Where(x => x.ID != 1).ToList();
            List<SurveyDetails> FieldSectionDetail = new List<SurveyDetails>();
            FieldSectionDetail = db.Database.SqlQuery<SurveyDetails>(string.Format(sql, projectid)).ToList<SurveyDetails>();

            using (XLWorkbook wb = new XLWorkbook())
            {
                int col = 0, row = 0;
                var worksheet = wb.Worksheets.Add("Audit Sheet");
                worksheet.Column(3).Width = 50;
                worksheet.Column(4).Width = 50;

                worksheet.Cell(1, 1).Value = "Audit Report";
                worksheet.Range(1, 1, 1, (SurveyDealerAll[0].DealerLst.Count * 2) + 4).Merge();
                worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                col = 1;
                row = 2;
                worksheet.Cell(row, col++).Value = "#";
                worksheet.Cell(row, col++).Value = "+";
                worksheet.Cell(row, col++).Value = "Audit Item";
                worksheet.Cell(row, col++).Value = "";

                foreach (var item in Dealerlst)
                {
                    worksheet.Cell(row, col++).Value = item.Name;

                    worksheet.Cell(row, col++).Value = "";
                    int startCol = 1;
                    worksheet.Range(row, (col - 2), row, col - 1).Merge();
                }

                worksheet.Range(row, 1, row, (SurveyDealerAll[0].DealerLst.Count * 2) + 4).Style.Font.FontColor = XLColor.White;
                worksheet.Range(row, 1, row, (SurveyDealerAll[0].DealerLst.Count * 2) + 4).Style.Fill.BackgroundColor= XLColor.Black;


                List<SurveyDetails> Sdetail = FieldSectionDetail;//For Parent
                int number = 1;
                foreach (var item in Sdetail)
                {
                    List<DNA_CAPI_MIS.Models.SurveyDetailsAllDealer> SchDetail = SurveyDealerAll;//For Child

                    SchDetail = SchDetail.Where(x => x.id == item.id).ToList();
                    col = 1;
                    row++;
                    worksheet.Cell(row, col++).Value = number;
                    //worksheet.Cell(row, col++).Value = "";
                    col++;
                    worksheet.Cell(row, col++).Value = "Selected Answer";
                    worksheet.Cell(row, col++).Value = item.Section_name;

                    for (int indx = 0; indx < Dealerlst.Count; indx++)
                    {
                        double sumDealer = SchDetail.Sum(x => (double)(x.DealerLst[indx].Score ?? 0));
                        worksheet.Cell(row, col++).Value = sumDealer;
                        //worksheet.Cell(row, col++).Value = "";
                        col++;
                        worksheet.Range(row, (col - 2), row, col - 1).Merge();
                    }

                    row++;
                    col = 1;

                    worksheet.Cell(row, col++).Value = "";
                    worksheet.Cell(row, col++).Value = "";
                    worksheet.Cell(row, col++).Value = "Question";
                    worksheet.Cell(row, col++).Value = "";

                    for (int indx = 0; indx < Dealerlst.Count; indx++)
                    {
                        worksheet.Cell(row, col++).Value = "Option";
                        worksheet.Cell(row, col++).Value = "Score";

                    }
                    worksheet.Range(row, 1, row, col).Style.Fill.BackgroundColor = XLColor.Gray;
                    worksheet.Range(row, 1, row, col).Style.Font.FontColor = XLColor.White;
                    row++;
                    col = 1;
                    string name = "";


                    for (int i = 0; i < SchDetail.Count; i++)
                    {
                        worksheet.Cell(row, col++).Value = "";
                        worksheet.Cell(row, col++).Value = "";
                        worksheet.Cell(row, col++).Value = name == SchDetail[i].Section_name ? "" : SchDetail[i].Section_name;
                        worksheet.Cell(row, col++).Value = SchDetail[i].DealerLst[0].name == null ? "" : SchDetail[i].DealerLst[0].name;

                        foreach (var Dealeritem in Dealerlst)
                        {
                            DetailsDealersSurvey DealerDetail = SchDetail[i].DealerLst.Where(x => x.dealername == Dealeritem.Name).FirstOrDefault();

                            worksheet.Cell(row, col++).Value = (DealerDetail == null ? "" : DealerDetail.options ?? "");
                            worksheet.Cell(row, col++).Value = (DealerDetail == null ? "" : DealerDetail.Score.ToString());

                        }
                        worksheet.Range(row, 1, row, col).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;

                        row++;
                        col = 1;
                        name = SchDetail[i].Section_name;
                    }

                    number++;
                }

                double[] TotalQuestion = new double[Dealerlst.Count];
                double[] TotalMarkedQuestion = new double[Dealerlst.Count], TotalScore = new double[Dealerlst.Count];

                for (int indx = 0; indx < Dealerlst.Count; indx++)
                {
                    TotalScore[indx] = SurveyDealerAll.Sum(x => x.DealerLst[indx].Score ?? 0);
                    TotalMarkedQuestion[indx] = SurveyDealerAll.Sum(x => x.DealerLst[indx].Marked);
                    TotalQuestion[indx] = SurveyDealerAll.Select(x => x.DealerLst[indx].Marked).Count();
                }


                row++;
                col = 1;
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "Grand Total";
                worksheet.Cell(row, col++).Value = "Total Score";

                for (int indx = 0; indx < Dealerlst.Count; indx++)
                {
                    worksheet.Cell(row, col++).Value = TotalScore[indx];
                    col++;
                    worksheet.Range(row, (col - 2), row, col - 1).Merge();
                }
                //worksheet.Range(row, 1, row, col).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;

                row++;
                col = 1;
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "Total Marked Question";

                for (int indx = 0; indx < Dealerlst.Count; indx++)
                {
                    worksheet.Cell(row, col++).Value = TotalMarkedQuestion[indx];
                    col++;
                    worksheet.Range(row, (col - 2), row, col - 1).Merge();
                }
                worksheet.Range(row, 1, row, col).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;

                row++;
                col = 1;
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "Total Question";

                for (int indx = 0; indx < Dealerlst.Count; indx++)
                {
                    worksheet.Cell(row, col++).Value = TotalQuestion[indx];
                    col++;
                    worksheet.Range(row, (col - 2), row, col - 1).Merge();

                }
                worksheet.Range(row, 1, row, col).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;

                row++;
                col = 1;
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "Total Score Available";

                for (int indx = 0; indx < Dealerlst.Count; indx++)
                {
                    worksheet.Cell(row, col++).Value = TotalQuestion[indx];
                    col++;
                    worksheet.Range(row, (col - 2), row, col - 1).Merge();
                }
                worksheet.Range(row, 1, row, col).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;


                row++;
                col = 1;
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "Total UnmarkedQuestion";

                for (int indx = 0; indx < Dealerlst.Count; indx++)
                {
                    worksheet.Cell(row, col++).Value = TotalQuestion[indx] - TotalMarkedQuestion[indx];
                    col++;
                    worksheet.Range(row, (col - 2), row, col - 1).Merge();
                }
                worksheet.Range(row, 1, row, col).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;


                row++;
                col = 1;
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "";
                worksheet.Cell(row, col++).Value = "Total Percentage";

                for (int indx = 0; indx < Dealerlst.Count; indx++)
                {
                    worksheet.Cell(row, col++).Value = (TotalScore[indx] / TotalMarkedQuestion[indx]) * 100;
                    col++;
                    worksheet.Range(row, (col - 2), row, col - 1).Merge();
                }
                worksheet.Range(row, 1, row, col).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;



                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Audit Report.xlsx");
                }
            }

        }

        [Authorize]
        public FileResult Export()
        {
            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Merge Cells");

            // Merge a row
            //ws.Cell("B2").Value = "Merged Row(1) of Range (B2:D3)";
            //ws.Range("B2:D3").Row(1).Merge();
            ws.Cell(1, 1).Value = "Merged Row(1) of Range (B2:D3)";
            ws.Cell(1, 2).Value = "";
            ws.Cell(1, 3).Value = "";
            ws.Range(1, 1, 1, 3).Row(1).Merge();



            // Merge a column
            ws.Cell("F2").Value = "Merged Column(1) of Range (F2:G8)";
            ws.Cell("F2").Style.Alignment.WrapText = true;
            ws.Range("F2:G8").Column(1).Merge();

            // Merge a range
            ws.Cell("B4").Value = "Merged Range (B4:D6)";
            ws.Cell("B4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Range("B4:D6").Merge();

            // Unmerging a range...
            ws.Cell("B8").Value = "Unmerged";
            ws.Range("B8:D8").Merge();
            ws.Range("B8:D8").Unmerge();

            //workbook.SaveAs("MergeCells.xlsx");

            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Audit Report.xlsx");
            }
        }

        public JsonResult GetSurveryor(int ProjectId, int Quarterid, int Dealerid)
        {
            string sql = "";
            sql = @"
select S.SurveyorName + ' '+ convert(nvarchar,Created) as name,S.sbjnum as value from Survey S  where S.sbjnum in (
select msd.sbjnum FROM SurveyData MSD
join ProjectField pf on pf.ID=MSD.FieldId

WHERE msd.sbjnum in (SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            			WHERE ISNULL(s.Test, 0) = 0
						and sd.FieldId = 49756 AND sd.FieldValue = '{2}'  and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
						and pf.Projectid=7113
						GROUP BY s.sbjnum )
									ANd msd.sbjnum IN	--Quarter
            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            				WHERE ISNULL(s.Test, 0) = 0 
							and sd.FieldId = 49757 AND sd.FieldValue ='{1}' and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
							GROUP BY s.sbjnum )
							and pf.Projectid={0}
group by msd.sbjnum)

";


            List<SurveyDropDown> surveyDropDown = db.Database.SqlQuery<SurveyDropDown>(string.Format(sql, ProjectId.ToString(), Quarterid.ToString(), Dealerid.ToString())).ToList();

            return Json(JsonConvert.SerializeObject(surveyDropDown), JsonRequestBehavior.AllowGet);


        }


        [Authorize]
        public ActionResult _SurveyDetailTable(int id, int sbjnum, string tor = "")
        {
            string torFunc = "fnSurveyValues";
            string torFilter = " AND fs.id <> 1210";
            //Type Of Report: C = COVID, F = Facility Audit
            if (tor == "C")
            {
                torFilter = " AND fs.id = 1210";
                torFunc = "fnSurveyValuesCOVID";
            }

            string sql = "";
            sql = @"SELECT fs.ID id, fs.name AS Section_name,fs.DisplayOrder
FROM ProjectField pf 
 INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
 LEFT OUTER JOIN {3}({1}) sd ON sd.FieldID = pf.ID
 LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
  CASE 
  WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE sd.Answer END) 
  WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' THEN sd.OptionCode 
  ELSE sd.Answer END
 LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
WHERE pf.ProjectID={0}
	and fs.ID not in (1209,1208) {2}
	group by fs.ID,fs.name,fs.DisplayOrder
    Order by fs.DisplayOrder";

            List<SurveyDetails> SurDetail = new List<SurveyDetails>();
            SurDetail = db.Database.SqlQuery<SurveyDetails>(string.Format(sql, id.ToString(), sbjnum.ToString(), torFilter, torFunc)).ToList<SurveyDetails>();


            List<SurveyDetails> SurDetailChild = new List<SurveyDetails>();
            sql = @"
SELECT fs.ID, pf.id Pid, pf.ParentFieldID, pf.FieldType, 
 pf.title as Section_name,SampleAnswers.Title name, SampleAnswersOption.Title options,
 Convert(float, sd.Score)/10 AS Score,sd.Answer ,
 CASE WHEN pf.FieldType IN ('PIC', 'MLT', 'DDN') THEN 0 ELSE sd.Marked END as Marked
FROM ProjectField pf 
 INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
 LEFT OUTER JOIN {3}({1}) sd ON sd.FieldID = pf.ID
 LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
  CASE 
  WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE 
	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
  END) 
  WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' OR (pf.FieldType = 'MLT' AND SampleAnswers.ParentSampleID IS NULL) THEN sd.OptionCode 
  ELSE
  	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
 END
 LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
WHERE pf.ProjectID={0} and fs.ID not in (1209,1208) {2}
Order by fs.DisplayOrder, pf.DisplayOrder,fs.ID
";
            SurDetailChild = db.Database.SqlQuery<SurveyDetails>(string.Format(sql, id.ToString(), sbjnum.ToString(), torFilter, torFunc)).ToList<SurveyDetails>();
            //SurDetailChild.ForEach(x =>
            //{
            //    int ParentID = x.ParentFieldID ?? -1;
            //    if (x.FieldType == "SCG" && SurDetailChild.Exists(y => y.Pid == ParentID && y.name == "No"))
            //    {
            //        x.Score = 0;
            //    }
            //});
            //int Count = 0;
            //foreach (var item in SurDetailChild)
            //{
            //    int ParentID = item.ParentFieldID ?? -1;
            //    if (item.FieldType == "SCG" && ParentID != -1)
            //    {

            //        SurveyDetails sdetail = SurDetailChild.Where(x => x.Pid == ParentID).Single();
            //        if (sdetail.name == "No")
            //        {
            //            //item.options = "";
            //            //item.Score = 0;
            //        }
            //        else if (sdetail.name == "N/A")
            //        {
            //            //item.options = "";
            //            //item.Score = null;
            //            Count += SurDetailChild.Where(x => x.ParentFieldID == ParentID && x.Score != null && x.FieldType != "PIC").Count();
            //        }
            //    }
            //    if (item.name == "Yes")
            //    {

            //        item.Score = 1;
            //    }
            //}

            int Marked = (int)SurDetailChild.Select(x => x.Marked).Where(x => x != null).Sum(), total = SurDetailChild.Count(), Unmarked = 0;
            Unmarked = total - Marked;
            ViewBag.MarkedQuestion = Marked;
            ViewBag.UnMarkedQuestion = Unmarked;
            ViewBag.TotalQuestion = total;

            //SurDetailChild = SurDetailChild.Where(x => x.Score != null ).ToList();
            ViewBag.SurDetailChild = SurDetailChild;
            //ViewBag.Count = SurDetailChild.Where(x => x.FieldType != "PIC" && x.Score != null).Count() - Count;
            //ViewBag.Count = DatalstTemp[0].PointsFrom;

            ViewBag.SurDetail = SurDetail;

            return View();
        }

        public string GetAllAdminUser()
        {
            string userids = "";
            string usersname = "";
            List<string> useridlst = db.CustomerUser.Where(x => x.CustomerBranchID == 1).Select(x => x.UserID.ToString()).ToList();
            userids = string.Join(",", useridlst);
            List<string> tempstrlst = idenDB.Database.SqlQuery<string>("select Replace( username,'@pwd.com','') from AspNetUsers where id in (" + userids + ") ").ToList<string>();
            usersname = "'" + string.Join("','", tempstrlst) + "'";
            return usersname;
        }

        [Authorize]
        public ActionResult SurveyDetail(int ProjectId)
        {
            ViewBag.Projectid = ProjectId;
            string sql = "";


            if (User.IsInRole("Admin"))
            {
                SelectList dealer = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true).Select(x => new { x.ID, x.Name }).OrderBy(f => f.Name), "ID", "Name", null);

                ViewBag.dealer = dealer;
                sql = "";
                #region
                //            sql = @"
                //					  SELECT  SUM(SubSum.BaseCount) Score, pfs.Name as Section_name,pfs.ID--,SubSum.Title, pf.title as name,SubSum.ParentSampleID ParentSampleID,SubSum.Fieldtype 
                //					   from 
                //            	(SELECT mp.FieldID AS FieldID, mp.Title as Title,mp.ParentSampleID ParentSampleID,pfch.fieldtype Fieldtype,
                //            		(SELECT COUNT(*) FROM surveydata msd 
                //            		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
                //            		AND msd.sbjnum IN        --Dealer
                //            			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                //            			WHERE ISNULL(s.Test, 0) = 0
                //						and sd.FieldId = 49756 AND sd.FieldValue = '1'
                //						GROUP BY s.sbjnum )
                //									ANd msd.sbjnum IN	--Quarter
                //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                //            				WHERE ISNULL(s.Test, 0) = 0 
                //							and sd.FieldId = 49757 AND sd.FieldValue ='1'
                //							GROUP BY s.sbjnum ) 					

                //						 ) BaseCount 
                //            	FROM projectfieldsample mp
                //				inner join ProjectField pfch on mp.FieldID=pfch.ID

                //				) AS SubSum  
                //            INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
                //            INNER JOIN ProjectFieldSection pfs on pfs.ID = pf.SectionID
                //            WHERE pf.ProjectID=7113 
                //			and pfs.Name not in ('End','General')
                //			and ((SubSum.fieldtype ='SCG' and SubSum.ParentSampleID is null) or SubSum.fieldtype !='SCG' )
                //			--GROUP BY  pfs.Name,pf.title ,pf.ID,SubSum.ParentSampleID,SubSum.Fieldtype,SubSum.Title  
                //			GROUP BY  pfs.id,pfs.Name
                //            Order by pfs.Name,Score desc
                //";
                #endregion
                List<SurveyDetails> SurDetail = new List<SurveyDetails>();
                //SurDetail = db.Database.SqlQuery<SurveyDetails>(sql).ToList<SurveyDetails>();

                //                sql = @"
                //select S.SurveyorName + ' '+ convert(nvarchar,Created) as Name,S.sbjnum as Value from Survey S  where S.sbjnum in (
                //select msd.sbjnum FROM SurveyData MSD
                //join ProjectField pf on pf.ID=MSD.FieldId

                //WHERE msd.sbjnum in (SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                //            			WHERE ISNULL(s.Test, 0) = 0
                //						and sd.FieldId = 49756 AND sd.FieldValue = '2'
                //						and pf.Projectid={0}
                //						GROUP BY s.sbjnum )
                //									ANd msd.sbjnum IN	--Quarter
                //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                //            				WHERE ISNULL(s.Test, 0) = 0 
                //							and sd.FieldId = 49757 AND sd.FieldValue ='1'
                //							GROUP BY s.sbjnum )
                //							and pf.Projectid={0}
                //group by msd.sbjnum)


                //";

                //                List<SurveyDropDown> surveyDropDown = db.Database.SqlQuery<SurveyDropDown>(string.Format(sql, ProjectId.ToString())).ToList();

                List<SurveyDropDown> surveyDropDown = new List<SurveyDropDown>();


                SelectList surveyDropDown1 = new SelectList(surveyDropDown, "Value", "Name", null);
                ViewBag.SurveyDropDown = surveyDropDown1;
                //ViewBag.SurDetail = SurDetail;
            }
            else
            {
                //if (int.TryParse(User.Identity.GetUserId(), out id))
                //{
                //    var user = await UserManager.FindByIdAsync(id);
                //    if (user == null)
                //    {
                //        return HttpNotFound();
                //    }
                //}
                int user = Convert.ToInt32(User.Identity.GetUserId());
                int customerBranch = db.CustomerUser.Where(x => x.UserID == user).Single().CustomerBranchID;
                SelectList dealer = new SelectList(db.CustomerBranch.Where(x => x.IsActive == true && x.ID == customerBranch).Select(x => new { x.ID, x.Name }).OrderBy(f => f.Name), "ID", "Name", null);

                ViewBag.dealer = dealer;
                sql = "";
                List<SurveyDetails> SurDetail = new List<SurveyDetails>();
                //SurDetail = db.Database.SqlQuery<SurveyDetails>(sql).ToList<SurveyDetails>();

                //sql = @"
                //select S.SurveyorName + ' '+ convert(nvarchar,Created) as Name,S.sbjnum as Value from Survey S  where S.sbjnum in (
                //select msd.sbjnum FROM SurveyData MSD
                //join ProjectField pf on pf.ID=MSD.FieldId

                //WHERE msd.sbjnum in (SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                //            			WHERE ISNULL(s.Test, 0) = 0
                //						and sd.FieldId = 49756 AND sd.FieldValue = '2'
                //						and pf.Projectid={0}
                //						GROUP BY s.sbjnum )
                //									ANd msd.sbjnum IN	--Quarter
                //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                //            				WHERE ISNULL(s.Test, 0) = 0 
                //							and sd.FieldId = 49757 AND sd.FieldValue ='1'
                //							GROUP BY s.sbjnum )
                //							and pf.Projectid={0}
                //group by msd.sbjnum)


                //";

                //List<SurveyDropDown> surveyDropDown = db.Database.SqlQuery<SurveyDropDown>(string.Format(sql, ProjectId.ToString())).ToList();
                List<SurveyDropDown> surveyDropDown = new List<SurveyDropDown>();

                SelectList surveyDropDown1 = new SelectList(surveyDropDown, "Value", "Name", null);
                ViewBag.SurveyDropDown = surveyDropDown1;
                //ViewBag.SurDetail = SurDetail;
            }

            return View();
        }





        [Authorize]
        [HttpPost]
        public ActionResult AnalyticReports(FormCollection form, int? ProjectID)
        {
            if (ProjectID > 0)
            {
                //Get Project Id and Name
                string sql = "SELECT Id, Name FROM Project WHERE Id = " + ProjectID;
                var project = db.Database.SqlQuery<ProjectView>(sql).FirstOrDefault<ProjectView>();
                if (project != null)
                {
                    ViewBag.ProjectName = project.Name;
                    ViewBag.ProjectId = ProjectID;
                    var designController = new DesignerController();

                    //Get Project Field Sections
                    List<ProjectFieldSection> ProjectFieldSection = db.ProjectFieldSection
                        .Where(x => x.ProjectID.Equals(ProjectID.Value))
                        .OrderBy(x => x.DisplayOrder)
                        .ToList<ProjectFieldSection>();
                    if (ProjectFieldSection.Count == 0)
                    {
                        ProjectFieldSection.Add(new ProjectFieldSection { ProjectID = ProjectID.GetValueOrDefault(0), ID = 0, Name = "Questionnaire" });
                    }
                    ViewBag.ProjectFieldSection = ProjectFieldSection;


                    //Get Project Field Sections
                    List<ProjectField> ProjectFields = db.ProjectField
                        .Where(x => x.ProjectID == ProjectID.Value && (x.FieldType.Equals("RDO") || x.FieldType.Equals("CHK") || x.FieldType.Equals("SCD") || x.FieldType.Equals("SCG") || x.FieldType.Equals("MCG")))
                        .OrderBy(x => x.DisplayOrder)
                        .ToList<ProjectField>();

                    sql = @"SELECT mp.ID SampleID, mp.code, mp.title, 
		(SELECT COUNT(*) FROM surveydata msd 
		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
		AND msd.sbjnum IN 
			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
			WHERE ISNULL(s.Test, 0) = 0 AND ";

                    // Get field counts
                    int fieldCount = 0;
                    foreach (var pf in ProjectFields)
                    {
                        string elementId = "_" + pf.ID;
                        if (form.AllKeys.Contains(elementId))
                        {
                            string fieldValue = form[elementId];
                            if (fieldValue.Length > 0)
                            {
                                fieldCount++;
                                string fieldId = (elementId.Substring(0, 1) == "_" ? elementId.Substring(1) : elementId);

                                if (pf.FieldType == "RDO")
                                {
                                    sql += " (sd.FieldId = " + fieldId + " AND sd.FieldValue = '" + fieldValue + "') OR";
                                }
                                else if (pf.FieldType == "CHK")
                                {
                                    sql += " (sd.FieldId = " + fieldId + " AND (sd.FieldValue = '" + fieldValue + "' OR '" + fieldValue + "' IN (SELECT ListMember FROM fnSplitCSV(sd.FieldValue)))) OR";
                                }
                                else if (pf.FieldType == "SCD" || pf.FieldType == "SCG")
                                {

                                }
                                else if (pf.FieldType == "MCG")
                                {

                                }
                            }
                        }
                    }
                    if (fieldCount == 0)
                    {
                        sql = sql.Substring(0, sql.Length - 5);     //remove AND
                    }
                    else
                    {
                        sql = sql.Substring(0, sql.Length - 3);     //remove OR
                    }
                    sql += @" GROUP BY s.sbjnum ) ) BaseCount 
	FROM projectfieldsample mp WHERE mp.FieldID = {0}
    ORDER BY mp.DisplayOrder";

                    List<FieldAnalyticsReport> faList = new List<FieldAnalyticsReport>();
                    int BaseTotal = 0;
                    foreach (var pf in ProjectFields)
                    {
                        string fSql = string.Format(sql, pf.ID);

                        var faQuery = db.Database.SqlQuery<FieldAnalytics>(fSql);
                        List<FieldAnalytics> fa = faQuery.ToList<FieldAnalytics>();

                        var max = fa.Max<FieldAnalytics>(x => x.BaseCount);
                        if (max > BaseTotal) BaseTotal = (int)max;

                        FieldAnalyticsReport far = new FieldAnalyticsReport();
                        far.FieldId = pf.ID;
                        far.SectionID = pf.SectionID;
                        far.FieldTitle = (pf.ReportTitle == null ? pf.Title : pf.ReportTitle);
                        far.FieldAnalytics = fa;
                        faList.Add(far);
                    }
                    //Get Base count
                    ViewBag.BaseTotal = BaseTotal;

                    return View("AnalyticReportsView", faList);
                }
            }
            return View();
        }

    }
}
