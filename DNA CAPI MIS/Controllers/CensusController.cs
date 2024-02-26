using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DNA_CAPI_MIS.Models;
using DNA_CAPI_MIS.DAL;
using System.Data.Entity.SqlServer;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.IO;
using SharpKml.Engine;
using SharpKml.Dom;

namespace DNA_CAPI_MIS.Controllers
{
    public class CensusController : Controller
    {
        private ProjectContext db = new ProjectContext();

        // GET: /Census/
        [Authorize(Roles = "Admin,KSAManager,GCCManager,CanEdit,Field Supervisor,Project Manager,QC,Field Observer")]
        public ActionResult Index()
        {
            int reportId_1371 = 0;
            int reportId_3390 = 0;
            int reportId_3392 = 0;
            if (User.IsInRole("KSAManager") || User.IsInRole("Admin"))
            {
                reportId_1371 = 1;
                reportId_3390 = 5;
                reportId_3392 = 5;
            }
            else if (User.IsInRole("GCCManager"))
            {
                reportId_1371 = 1;
                reportId_3390 = 0;
                reportId_3392 = 5;

            }
            ViewBag.reportId_1371 = reportId_1371;
            ViewBag.reportId_3390 = reportId_3390;
            ViewBag.reportId_3392 = reportId_3392;

            var query = from c in db.ProjectFieldSample
                        join pf in db.ProjectField on c.FieldID equals pf.ID
                       where pf.ParentFieldID == 1 && c.IsActive == true && c.ParentSampleID != 0
                       select new 
                          {
                              SampleID = c.ID,
                              Title = c.Title
                          };
            var cats = query.ToList()
                .Select(x => new CensusCategoriesView
                    {
                        SampleID = x.SampleID,
                        Title = x.Title
                    });
            return View(cats.ToList<CensusCategoriesView>());
        }

        // GET: /Census/
        [Authorize(Roles = "Admin,KSAManager,GCCManager,CanEdit,Field Supervisor,Project Manager,QC")]
        public ActionResult SurveyLocations(int? id)
        {
            string reportId = "";
            if (User.IsInRole("KSAManager"))
            {
                reportId = "AND rdx.ReportID = 1";
            }
            else if (User.IsInRole("GCCManager"))
            {
                reportId = "AND rdx.ReportID = 3";
            }
            //Generate city names view for Menu
            var allCities = db.Database.SqlQuery<CityView>(@"
                SELECT DISTINCT Country.Id CountryId, Country.Name CountryName FROM Country  
                INNER JOIN rdxlocation rdx ON Country.id = rdx.CountryID " + reportId + " ORDER BY Country.Name");

            CensusCategoriesView selectedCategory = GetCategory((int)id);
            ViewBag.CensusCategoryName = selectedCategory.Title;
            ViewBag.CategoryId = id;

            return View(allCities.ToList<CityView>());
        }

        [Authorize(Roles = "Admin,KSAManager,GCCManager,CanEdit,Field Supervisor,Project Manager,QC")]
        // GET: /Survey/Index/{project_id}
        public ActionResult Survey(int? id, int? projectId, string language)
        {
            if (id == null || projectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //db.Database.ExecuteSqlCommand("sp_CAPI_Import");

            //string language = "en-US";
            if (Request.QueryString.Get("lang") != null)
            {
                language = Request.QueryString.Get("lang");
            }
            else
            {
                language = "en-US";
            }

            string catId = id.ToString();
            CensusCategoriesView selectedCategory = GetCategory((int)id);
            ViewBag.CensusCategoryName = selectedCategory.Title;

            string sql = "SELECT s.sbjnum, s.Created, s.Longitude, s.Latitude, s.SurveyorName, sd.FieldId, pf.FieldType, " +
                           "    CASE WHEN t.Text IS NULL THEN pf.Title ELSE t.Text END AS Title, sd.FieldValue " +
                           "FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum " +
                           "INNER JOIN ProjectField pf ON sd.FieldId = pf.ID " +
                           "LEFT OUTER JOIN Translation t ON t.EntityName = 'PROJECTFIELD' AND t.FieldName = 'TITLE' AND t.Language = '" + language + "' " +
                           "    AND pf.ID = (CASE WHEN ISNUMERIC(t.KeyValue) = 1 THEN CAST(t.KeyValue as int) ELSE 0 END) " +
                           "WHERE s.ProjectID = " + projectId + 
                           "    AND s.sbjnum IN (SELECT sd2.sbjnum FROM SurveyData sd2 WHERE sd2.FieldId = 1 AND sd2.FieldValue = '" + catId + "') " +
                           "ORDER BY s.Created desc, s.sbjnum, pf.DisplayOrder";

            var surveys = db.Database.SqlQuery<SurveyDataView>(sql);

            return View(surveys.ToList<SurveyDataView>());
        }


        // GET: /Survey/Index/{project_id}
        [Authorize(Roles = "Admin,KSAManager,GCCManager,CanEdit,Field Supervisor,Project Manager,QC")]
        public ActionResult SurveyLocationWise(int? id, string language, int? country, int? projectId)
        {
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
            if (country == 0 || country == null)
            {
                country = 2;
                if (Request.QueryString.Get("cy") != null)
                {
                    country = Convert.ToInt32(Request.QueryString.Get("cy"));
                }
            }
            if (projectId == 0 || projectId == null)
            {
                projectId = 3390;
            }
            //CHECK USER ROLE according to region
            //string UserId = User.Identity.GetUserId();
            
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

            ViewBag.CountryId = country;

            string catId = id.ToString();
            CensusCategoriesView selectedCategory = GetCategory((int)id);
            ViewBag.CensusCategoryName = selectedCategory.Title;
            ViewBag.CategoryId = catId;

            string sql = @"SELECT cy.Name AS CountryName, ct.Name AS CityName, dt.Name AS DistrictName, s.sbjnum, s.Created, s.Longitude, s.Latitude, s.SurveyorName, sd.FieldId, pf.FieldType, 
                               CASE WHEN t.Text IS NULL THEN pf.Title ELSE t.Text END AS Title, sd.FieldValue 
                           FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum 
                           INNER JOIN ProjectField pf ON sd.FieldId = pf.ID 
                           LEFT OUTER JOIN Translation t ON t.EntityName = 'PROJECTFIELD' AND t.FieldName = 'TITLE' AND t.Language = '" + language + @"' 
                               AND pf.ID = (CASE WHEN ISNUMERIC(t.KeyValue) = 1 THEN CAST(t.KeyValue as int) ELSE 0 END) 
	                       INNER JOIN City ct ON ct.ID = s.CityID
	                       LEFT OUTER JOIN District dt on DT.ID = s.DistrictID
	                       INNER JOIN Country cy ON ct.CountryID = cy.ID AND cy.ID = " + country + @"
                           WHERE s.ProjectID = " + projectId + @" 
                               AND s.sbjnum IN (SELECT sd2.sbjnum FROM SurveyData sd2 WHERE sd2.FieldId = 1 AND sd2.FieldValue = '" + catId + @"') 
                           ORDER BY s.sbjnum, cy.Name, ct.Name, dt.Name, s.Created desc, pf.DisplayOrder";

            var surveys = db.Database.SqlQuery<SurveyDataView>(sql);
            Dictionary<int, ProjectFieldSampleLookup> pfsLookup = new Dictionary<int, ProjectFieldSampleLookup>();

            pfsLookup.Add(1010, new ProjectFieldSampleLookup(){Id = 1010, FieldId = 2, Title = "Neighbourhood Stationary / Bookshop"});
            pfsLookup.Add(1011, new ProjectFieldSampleLookup(){Id = 1011, FieldId = 2, Title = "Large Stationary / Bookshop"});
            pfsLookup.Add(1012, new ProjectFieldSampleLookup(){Id = 1012, FieldId = 2, Title = "Stationary Wholesaler"});
            pfsLookup.Add(1013, new ProjectFieldSampleLookup(){Id = 1013, FieldId = 2, Title = "Copy Center"});
            pfsLookup.Add(1014, new ProjectFieldSampleLookup(){Id = 1014, FieldId = 3, Title = "University"});
            pfsLookup.Add(1015, new ProjectFieldSampleLookup(){Id = 1015, FieldId = 3, Title = "College"});
            pfsLookup.Add(1016, new ProjectFieldSampleLookup(){Id = 1016, FieldId = 3, Title = "Secondary School"});
            pfsLookup.Add(1017, new ProjectFieldSampleLookup(){Id = 1017, FieldId = 3, Title = "Intermediate School"});
            pfsLookup.Add(1018, new ProjectFieldSampleLookup(){Id = 1018, FieldId = 3, Title = "Primary School"});
            pfsLookup.Add(1019, new ProjectFieldSampleLookup(){Id = 1019, FieldId = 3, Title = "Pre Primary School"});

            pfsLookup.Add(3922, new ProjectFieldSampleLookup() { Id = 3922, FieldId = 2, Title = "Neighbourhood Stationary / Bookshop" });
            pfsLookup.Add(3923, new ProjectFieldSampleLookup() { Id = 3923, FieldId = 2, Title = "Large Stationary / Bookshop" });
            pfsLookup.Add(3924, new ProjectFieldSampleLookup() { Id = 3924, FieldId = 2, Title = "Stationary Wholesaler" });
            pfsLookup.Add(3925, new ProjectFieldSampleLookup() { Id = 3925, FieldId = 2, Title = "Copy Center" });
            pfsLookup.Add(3944, new ProjectFieldSampleLookup() { Id = 3944, FieldId = 2, Title = "Office Supplier (Selling Office Equipment + Stationery)" });
            pfsLookup.Add(3926, new ProjectFieldSampleLookup() { Id = 3926, FieldId = 3, Title = "University" });
            pfsLookup.Add(3927, new ProjectFieldSampleLookup() { Id = 3927, FieldId = 3, Title = "College" });
            pfsLookup.Add(3928, new ProjectFieldSampleLookup() { Id = 3928, FieldId = 3, Title = "Secondary School" });
            pfsLookup.Add(3929, new ProjectFieldSampleLookup() { Id = 3929, FieldId = 3, Title = "Intermediate School" });
            pfsLookup.Add(3930, new ProjectFieldSampleLookup() { Id = 3930, FieldId = 3, Title = "Primary School" });
            pfsLookup.Add(3931, new ProjectFieldSampleLookup() { Id = 3931, FieldId = 3, Title = "Pre Primary School" });

            List<SurveyDataView> data = surveys.ToList<SurveyDataView>();
            foreach (SurveyDataView sd in data)
            {
                if (sd.FieldId == 37 || sd.FieldId == 52 || sd.FieldId == 1297 || sd.FieldId == 1307) 
                {
                    var scat = sd.FieldValue.Split(',');
                    string scatNames = "";
                    foreach (string sample in scat)
                    {
                        if (sample.Length > 0)
                        {
                            if (scatNames.Length > 0) { scatNames += ", "; }
                            scatNames += pfsLookup[Convert.ToInt32(sample)].Title;
                        }
                    }
                    if (scatNames.Length > 0)
                    {
                        sd.FieldValue = scatNames;
                    }
                }
            }
            return View(data);
        }

        private CensusCategoriesView GetCategory(int catId) {
            //var query = from c in db.ProjectFieldSample where c.ID == catId
            //        select new
            //        {
            //            SampleID = c.ID,
            //            Title = c.Title
            //        };
            //var cats = query.ToList()
            //    .Select(x => new CensusCategoriesView
            //    {
            //        SampleID = x.SampleID,
            //        Title = x.Title
            //    });

            string sql = @"SELECT TOP 1 id, Category_en AS Title FROM Categories WHERE IsActive = 1 AND id = " + catId;

            var cats = db.Database.SqlQuery<CensusCategoriesView>(sql);
            return cats.ToList<CensusCategoriesView>().SingleOrDefault();
        }

        [Authorize(Roles = "Admin,KSAManager,GCCManager,CanEdit,Field Supervisor,Project Manager,QC")]
        public ActionResult SurveyQCFilter(int? id, string language, int? projectId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string sql = @"select surveyorname as Name, count(*) as 'count' from survey where projectid = " + projectId + " group by surveyorname";

            var categories = db.Database.SqlQuery<Surveyorname>(sql);
            ViewBag.surveyorname = categories.ToList<Surveyorname>();

            ViewBag.ProjectId = projectId;
            ViewBag.language = language;
            ViewBag.id = id;
            List<SurveyDataView> data = new List<SurveyDataView>();
            return View(data);
        
        }

        [Authorize(Roles = "Admin,KSAManager,GCCManager,CanEdit,Field Supervisor,Project Manager,QC")]
        public ActionResult SurveyQC(FormCollection form, int? id, string language, int? projectId, string fromdate, string todate, string surveyors)
        {
            string whereclause = "";
            string fdate = "";
            string tdate = "";
            if (fromdate != null) {
                fdate = fromdate.ToString();
                fdate = fdate.Replace(" ", "");
            }
            if (todate != null) {
                tdate = todate.ToString();
                tdate = tdate.Replace(" ", "");
            }
            
            string surveyor = "";
            if (surveyors != null && surveyors != "")
            {
                surveyor = surveyors.ToString();
                surveyor = surveyor.Replace(",", "','");
                surveyor = "'" + surveyor + "'";
                whereclause += "and SurveyorName in(" + surveyor + ") ";
            }
            string datewhere = "";
            if (fdate != null && fdate != "" && tdate != null && tdate != "") 
            {
                datewhere = " and CONVERT(VARCHAR(4),DATEPART(YEAR,  Created))   + '-'+ CONVERT(VARCHAR(2),DATEPART(MONTH,  Created))+ '-' +" +
                         "CONVERT(VARCHAR(2),DATEPART(DAY,  Created)) >= '" + fdate + "'" +
                         " and CONVERT(VARCHAR(4),DATEPART(YEAR,  Created))   + '-'+ CONVERT(VARCHAR(2),DATEPART(MONTH,  Created))+ '-' +" +
                         "CONVERT(VARCHAR(2),DATEPART(DAY,  Created)) <= '" + tdate + "' ";
                whereclause += datewhere;
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.ProjectId = projectId;

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
            string sql = @"SELECT CASE WHEN s.OpStatus = 1 THEN 'Yes' ELSE 'No' END AS OpStatus, CASE WHEN s.QCStatus = 1 THEN 'Yes' ELSE 'No' END AS QCStatus, cy.Name AS CountryName, ct.Name AS CityName, dt.Name AS DistrictName, s.sbjnum, s.Created, s.Longitude, s.Latitude, s.SurveyorName, sd.FieldId, pf.FieldType, 
                               CASE WHEN pf.VariableName IS NULL THEN CASE WHEN t.Text IS NULL THEN pf.Title ELSE t.Text END ELSE pf.VariableName END AS Title, sd.FieldValue 
                           FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum 
                           INNER JOIN ProjectField pf ON sd.FieldId = pf.ID 
                           LEFT OUTER JOIN Translation t ON t.EntityName = 'PROJECTFIELD' AND t.FieldName = 'TITLE' AND t.Language = '" + language + @"' 
                               AND pf.ID = (CASE WHEN ISNUMERIC(t.KeyValue) = 1 THEN CAST(t.KeyValue as int) ELSE 0 END) 
	                       INNER JOIN City ct ON ct.ID = s.CityID
	                       LEFT OUTER JOIN District dt on DT.ID = s.DistrictID
	                       INNER JOIN Country cy ON ct.CountryID = cy.ID 
                           WHERE ISNULL(s.Test, 0) = 0 AND s.ProjectID = " + projectId + " AND s.OpStatus = 1" +
                           " AND s.sbjnum IN (SELECT sd2.sbjnum FROM SurveyData sd2 WHERE sd2.FieldId = 1 "+whereclause+" AND sd2.FieldValue = '" + id + "')" +  
                           " ORDER BY s.sbjnum, cy.Name, ct.Name, dt.Name, s.Created desc, pf.DisplayOrder";

            var surveys = db.Database.SqlQuery<SurveyDataView>(sql);
            List<SurveyDataView> data = surveys.ToList<SurveyDataView>();
            return View(data);
        }

        // GET: /Census/
        [Authorize(Roles = "Admin,KSAManager,GCCManager,CanEdit,Field Supervisor,Project Manager,QC,Field Observer")]
        public ActionResult FieldProgress(int? id, int? cid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string reportId = "";
            if (User.IsInRole("KSAManager"))
            {
                reportId = "AND rdx.ReportID = 1";
            }
            else if (User.IsInRole("GCCManager"))
            {
                reportId = "AND rdx.ReportID = 3";
            }

            string sql = "SELECT Id, Name FROM Project WHERE Id = " + id;
            var project = db.Database.SqlQuery<ProjectView>(sql);
            ViewBag.ProjectName = project.FirstOrDefault<ProjectView>().Name;
            ViewBag.ProjectId = id;

            List<FieldProgress> data;
            DbRawSqlQuery<FieldProgress> allCities;

            if (!cid.HasValue)
            {
                //Generate city names view for Menu
                allCities = db.Database.SqlQuery<FieldProgress>(string.Format(@"
                SELECT Country.Id CountryId, Country.Name CountryName, rdx.CityId, City.Name AS CityName,
                0 AS SurveyCount,
                ROUND(ISNULL(SUM(tdd.distance)/1000,0), 0) AS DistanceMade,
                ROUND(ISNULL(SUM(d.DistanceMax),0), 0) AS DistanceTotal
                FROM Country  
                INNER JOIN rdxlocation rdx ON Country.id = rdx.CountryID {0}
                INNER JOIN City ON City.Id = rdx.CityId 
                INNER JOIN CityMapping ctm ON City.Id = ctm.CityId AND ctm.ProjectId = {1} AND ctm.Source = 'PROJECT'
                INNER JOIN District d ON d.CityId = City.Id
                LEFT OUTER JOIN TrackDistrictDistance tdd ON tdd.DistrictId = d.Id AND tdd.ProjectId = {1}
				GROUP BY Country.Id, Country.Name, rdx.CityId, City.Name
                ORDER BY Country.Name, 6 DESC, City.Name", reportId, id));

                ViewBag.ViewTitle = "Cities";

                data = allCities.ToList<FieldProgress>();
            }
            else
            {
                allCities = GetFieldProgressData(id, cid);

                ViewBag.CityName = allCities.FirstOrDefault<FieldProgress>().CityName;
                ViewBag.ViewTitle = "Districts - " + ViewBag.CityName;

                List<District> cityDistricts = db.District.Where(d => d.CityID == cid).OrderBy(d => d.Name).ToList();
                ViewBag.CityDistricts = cityDistricts;
                List<District> kmlDistricts = GetDistrictsFromKML((int)cid, allCities.FirstOrDefault<FieldProgress>().CityKML);
                ViewBag.KMLDistricts = kmlDistricts;

                data = allCities.ToList<FieldProgress>();
            }
            return View(data);
        }

        DbRawSqlQuery<FieldProgress> GetFieldProgressData(int? ProjectId, int? cid, int did = 0)
        {
            string reportId = "";
            if (User.IsInRole("KSAManager"))
            {
                reportId = "AND rdx.ReportID = 1";
            }
            else if (User.IsInRole("GCCManager"))
            {
                reportId = "AND rdx.ReportID = 3";
            }

            string districtFilter = "";
            if (did > 0)
            {
                districtFilter = " AND District.Id = " + did;
            }
            //Generate city names view for Menu
            return db.Database.SqlQuery<FieldProgress>(string.Format(@"
                SELECT Country.Id CountryId, Country.Name CountryName, rdx.CityId, City.Name AS CityName, 
                CASE WHEN cm.KMLPath IS NULL THEN ISNULL(City.KMLPath, '') ELSE cm.KMLPath END AS CityKML,
                District.Id AS DistrictId, District.Name AS DistrictName,
                0 AS SurveyCount,
                ROUND(ISNULL(SUM(tdd.distance)/1000,0), 0) AS DistanceMade,
                ROUND(ISNULL(SUM(District.DistanceMax),0), 0) AS DistanceTotal
                FROM Country  
                INNER JOIN rdxlocation rdx ON Country.id = rdx.CountryID {0}
                INNER JOIN City ON City.Id = rdx.CityId
                INNER JOIN District ON District.CityId = City.Id 
                LEFT OUTER JOIN CityMapping cm ON cm.CityId = City.Id AND cm.Source = 'PROJECT' AND cm.ProjectID = {1}
                LEFT OUTER JOIN TrackDistrictDistance tdd ON tdd.DistrictId = District.Id AND tdd.ProjectId = {1}
                WHERE City.Id = {2} {3}
                GROUP BY Country.Id, Country.Name, rdx.CityId, City.Name, CASE WHEN cm.KMLPath IS NULL THEN ISNULL(City.KMLPath, '') ELSE cm.KMLPath END, District.Id, District.Name
                ORDER BY 9 DESC, District.Name", 
                reportId, ProjectId, cid, districtFilter));
        }

        [Authorize(Roles = "Admin,KSAManager,GCCManager,CanEdit,Field Supervisor,Project Manager,QC,Field Observer")]
        public ActionResult FieldView(int? id, string language, int? c, int? district, string surveyor, string fromdate, string todate)
        {
            double citylat = 0.0;
            double citylong = 0.0;

            if (surveyor == null)
            {
                surveyor = "";
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
            if (fromdate != null)
                fromdate = fromdate.Replace(" ", "");
            if (todate != null)
            {
                todate = todate.Replace(" ", "");
                DateTime todate2 = new DateTime(Convert.ToInt32(todate.Substring(0, 4)), Convert.ToInt32(todate.Substring(4, 2)), Convert.ToInt32(todate.Substring(6, 2)));
                todate2 = todate2.AddDays(1);
                todate = todate2.Year.ToString() + todate2.Month.ToString().PadLeft(2, '0') + todate2.Day.ToString().PadLeft(2, '0');
            }

            //Generate default view
            string sql = "";
            if (c == null)
                c = 0;

            if (district == null)
                district = 0;

            sql = "SELECT * FROM Project WHERE Id = " + id;
            var project = db.Database.SqlQuery<Project>(sql);
            Project p = project.FirstOrDefault<Project>();
            ViewBag.ProjectName = p.Name;
            ViewBag.ProjectId = id;

            ViewBag.CityId = c;
            ViewBag.DistrictId = district;
            ViewBag.CurrentFilterText = "";
            ViewBag.City_Longitude = 0;
            ViewBag.City_Latitude = 0;

            db.Database.CommandTimeout = 300;

            List<CityView> cityData;
            string userList = "";
            if (c > 0)
            {
                var cityName = db.Database.SqlQuery<CityView>("SELECT c.id, c.name, c.Longitude, c.Latitude, c.CountryID, CASE WHEN mc.KMLPath IS NULL THEN c.KMLPath ELSE mc.KMLPath END AS KMLPath FROM City c LEFT OUTER JOIN CityMapping mc ON mc.CityID = c.ID AND mc.ProjectID = " + id + " AND mc.Source = 'PROJECT'");
                cityData = cityName.ToList<CityView>();
                ViewBag.AllCitiesList = cityData;
                var selCity = cityData.Find(x => x.ID == c);
                if (cityData.Count > 0 && selCity != null)
                {
                    ViewBag.CurrentFilterText += " » City: " + selCity.Name;
                    ViewBag.City_Longitude = selCity.Longitude;
                    ViewBag.City_Latitude = selCity.Latitude;
                    ViewBag.CountryID = selCity.CountryID;
                    ViewBag.KMLUrl = "http://censusrms.apps.dna.com.sa/kml/" + selCity.KMLPath;
                    //ViewBag.KMLUrl = "http://85.25.211.247:5303/kml/kml_" + city + "_nonsec.kml";
                    citylat = Convert.ToDouble(selCity.Latitude);
                    citylong = Convert.ToDouble(selCity.Longitude);

                    string surveyorSql = "";
                    if (district > 0)
                    {
                        surveyorSql = string.Format(@"select distinct tur.username as Name from trackuserRoute tur, Project p 
                            Where districtid = {0} AND p.id = {1} AND tur.ForDate >= ISNULL(p.ActualStartDate, p.CreatedOn) AND tur.ForDate <= ISNULL(p.ActualEndDate, '20990101')
	                        AND tur.username IN (SELECT DISTINCT substring(u.UserName, 0, charindex('@', u.UserName)) FROM DNAShared2.dbo.UserRights pur INNER JOIN DNAShared2.dbo.AspNetUsers u ON u.Id = pur.UserId WHERE pur.ObjectName = 'PROJECT' AND ObjectValue = {1})
                            order by username", district, id);
                    }
                    else
                    {
                        surveyorSql = string.Format(@"select distinct tur.username as Name from trackuserRoute tur, Project p 
                            Where cityid = {0} AND p.id = {1} AND tur.ForDate >= ISNULL(p.ActualStartDate, p.CreatedOn) AND tur.ForDate <= ISNULL(p.ActualEndDate, '20990101')
	                        AND tur.username IN (SELECT DISTINCT substring(u.UserName, 0, charindex('@', u.UserName)) FROM DNAShared2.dbo.UserRights pur INNER JOIN DNAShared2.dbo.AspNetUsers u ON u.Id = pur.UserId WHERE pur.ObjectName = 'PROJECT' AND ObjectValue = {1})
                            order by username", c, id);
                    }
                    var surveyornames = db.Database.SqlQuery<string>(surveyorSql);
                    ViewBag.surveyorname = surveyornames.ToList<string>();
                    userList = "'" + String.Join("','", surveyornames.ToArray<string>()) + "'";
                }
            }
            List<District> districtData;
            ViewBag.DistrictName = "";
            if (district > 0)
            {
                var districtName = db.Database.SqlQuery<District>("SELECT id, name, CityId, DistanceMax from District where ID = " + district);
                districtData = districtName.ToList<District>();
                if (districtData.Count > 0)
                {
                    District seldistrict = districtName.ToList<District>().FirstOrDefault();
                    ViewBag.CurrentFilterText += " » District: " + seldistrict.Name;
                    ViewBag.DistrictName = seldistrict.Name;
                }
            }

            //Including TrackPointID in the query would render the DISTINCT clause useless
            //Storing DeviceDateTime in place of TrackPointID to mark points ID
            DateTime prjDate = (DateTime)(p.ActualStartDate.HasValue ? p.ActualStartDate : p.CreatedOn);
            string prjStartDate = prjDate.Year.ToString() + prjDate.Month.ToString().PadLeft(2, '0') + prjDate.Day.ToString().PadLeft(2, '0');
            prjDate = (DateTime)(p.ActualEndDate.HasValue ? p.ActualEndDate : new DateTime(2099, 01, 01));
            string prjEndDate = prjDate.Year.ToString() + prjDate.Month.ToString().PadLeft(2, '0') + prjDate.Day.ToString().PadLeft(2, '0');

            sql = @"SELECT DISTINCT TOP 500000 tur.username AS SurveyorName, datepart(year, DeviceDateTime) yy, datepart(month, DeviceDateTime) mm, datepart(day, DeviceDateTime) dd, 
                    datepart(hour, DeviceDateTime) hh, datepart(minute, DeviceDateTime) mins, datepart(second, DeviceDateTime) ss,
                    Latitude, Longitude, tur.distance
                    FROM TrackUserRoute tur INNER JOIN TrackUser tu ON tur.ID = tu.RouteId
                    WHERE ISNULL(tu.accuracy, 0) < 40 AND tur.CityId = " + c + " AND tur.username IN (" + userList + ") " +
                    " AND tur.ForDate >= '" + prjStartDate + "' AND tur.ForDate <= '" + prjEndDate + "' ";

            if (district > 0)
            {
                sql += " AND tur.DistrictId = " + district;
            }
            if (surveyor.Length > 0)
            {
                sql += " AND tur.username = '" + surveyor + "' ";
            }

            if (fromdate != null && todate != null)
            {
                sql += " and DeviceDateTime >= '" + fromdate + "' and DeviceDateTime <= '" + todate + "' ";
            }
            sql = sql + " order by SurveyorName, yy, mm, dd, hh, mins, ss";

            var surveySql = db.Database.SqlQuery<SurveyDataView>(sql);
            List<SurveyDataView> surveyData = surveySql.ToList<SurveyDataView>();


            ////
//            sql = @"SELECT id, username AS SurveyorName, ForDate, distance, SnappedPath FROM TrackUserRoute 
//                    WHERE SnappedPath IS NOT NULL AND CityId = " + c + " AND username IN (" + userList + ") " +
//                    " AND ForDate >= '" + prjStartDate + "' AND ForDate <= '" + prjEndDate + "' ";

//            if (district > 0)
//            {
//                sql += " AND DistrictId = " + district;
//            }
//            if (surveyor.Length > 0)
//            {
//                sql += " AND username = '" + surveyor + "' ";
//            }

//            if (fromdate != null && todate != null)
//            {
//                sql += " and ForDate >= '" + fromdate + "' and ForDate <= '" + todate + "' ";
//            }
//            sql = sql + " order by SurveyorName, ForDate";

//            var pathSql = db.Database.SqlQuery<TrackUserRouteDataView>(sql);
//            List<TrackUserRouteDataView> pathData = pathSql.ToList<TrackUserRouteDataView>();
//            ViewBag.SnappedPaths = pathData;


            ////
            var allCities = GetFieldProgressData(id, c, district.HasValue ? (int)district : 0);
            var data = allCities.ToList<FieldProgress>();
            ViewBag.FieldProgress = data;

            return View(surveyData);

        }


        public bool UserAllowedInProject(int ProjectId)
        {
            string sql = "";

            if (User.IsInRole("Admin"))
            {
                return true;
            }
            else
            {
                string userFilter = "";
                string uid = User.Identity.GetUserId();
                userFilter = " AND pur.UserId = " + uid;
                sql = "SELECT * FROM project p WHERE id = " + ProjectId + " AND id IN (SELECT ObjectValue FROM DNAShared2.dbo.UserRights pur WHERE pur.ObjectName = 'PROJECT'" + userFilter + ")";
            }

            var query = db.Database.SqlQuery<ProjectsList>(sql);
            List<ProjectsList> projects = query.ToList<ProjectsList>();
            if (projects.Count > 0)
            {
                return true;
            }
            return false;
        }


        //////////////////////////////////////////////////////////////////
        // Census RMS View
        public ActionResult RMS()
        {
            //Generate city names view for Menu
            var allCities = db.Database.SqlQuery<CityView>(@"
                SELECT City.id, City.name, Country.Name CountryName, Country.Id AS CountryId FROM City INNER JOIN Country ON City.CountryId = Country.Id 
                INNER JOIN rdxlocation rdx ON City.id = rdx.cityid AND rdx.ReportID IN (1,3)
                ORDER BY City.CountryId, City.Name");
            List<CityView> allCityData = allCities.ToList<CityView>();
            ViewBag.AllCitiesList = allCityData;

            //Generate default view
            string filter = "";
            string category = Request.QueryString.Get("cat");
            string parentcat = Request.QueryString.Get("pcat");

            string sql = "";
            string city = Request.QueryString.Get("c");
            string country = Request.QueryString.Get("cn");

            if (string.IsNullOrEmpty(city)) city = "-1";

            //if (filter.Length == 0)
            //{
            //    if(city.Length == 0) 
            //        city = "-1";
            //}

            string countryFilter = " ct.ID = " + city;

            if (!string.IsNullOrEmpty(country))
            {
                countryFilter = " ct.CountryID = " + country;
            }

            ViewBag.RootCategory = 0;
            if (parentcat != null && parentcat.Length > 0)
            {
                ViewBag.RootCategory = parentcat;
                if (country == "2")
                {
                    filter = " AND (SubCategoryID IN (3922, 3923, 3924, 3925, 3944) OR SubCategoryID IN (3926, 3927, 3928, 3929, 3930, 3931)) ";
                }
                else
                {
                    filter = " AND (SubCategoryID IN (1010, 1011, 1012, 1013) OR SubCategoryID IN (1014, 1015, 1016, 1017, 1018, 1019)) ";
                }
            }
            else
            {
                if (category != null && category.Length > 0)
                {
                    filter = " AND SubCategoryID IN (" + category + ")"; //Categories may come in multiples
                }
            }

            ViewBag.country = country;
            ViewBag.city = city;
            ViewBag.category = category;
            ViewBag.CurrentFilterText = "";

            if ((city != "-1" && city.Length > 0) || (country != null && country.Length > 0))
            {
                if (country != null && country.Length > 0)
                {
                    var cnName = db.Database.SqlQuery<CityView>("SELECT id, name, Longitude, Latitude from Country where ID = " + country);
                    List<CityView> cnData = cnName.ToList<CityView>();
                    if (cnData.Count > 0)
                    {
                        CityView selCountry = cnName.ToList<CityView>().FirstOrDefault();
                        ViewBag.CurrentFilterText += " » Country: " + selCountry.Name;
                        ViewBag.Default_Longitude = selCountry.Longitude;
                        ViewBag.Default_Latitude = selCountry.Latitude;
                    }
                }
                if (city.Length > 0)
                {
                    var cityName = db.Database.SqlQuery<CityView>("SELECT id, name, Longitude, Latitude from City where ID = " + city);
                    List<CityView> cityData = cityName.ToList<CityView>();
                    if (cityData.Count > 0)
                    {
                        CityView selCity = cityName.ToList<CityView>().FirstOrDefault();
                        ViewBag.CurrentFilterText += " » City: " + selCity.Name;
                        ViewBag.Default_Longitude = selCity.Longitude;
                        ViewBag.Default_Latitude = selCity.Latitude;
                    }
                }

                string findCatName = "select pfs2.ID AS ID, CONCAT(CASE WHEN parentCat.Title = 'Default' THEN '' ELSE CONCAT(parentCat.Title, ' > ') END, pfs2.Title) AS NAME FROM " +
                  "  (select pf.ProjectID, ct.ID AS CityID, pfs.* from ProjectField pf " +
                  "      INNER JOIN ProjectFieldSample pfs ON pf.ID = pfs.FieldID " +
                  "      INNER JOIN Categories cat ON pf.CategoryId = cat.Id " +
                  "      INNER JOIN Country cn ON cat.CountryId = cn.ID INNER JOIN City ct ON cn.ID = ct.CountryID " +
                  "  WHERE " + countryFilter + " AND ParentFieldID = 1) AS parentCat " +
                  " INNER JOIN ProjectFieldSample pfs2 ON parentCat.ID = pfs2.ParentSampleID";


                if (category != null && category.Length > 0)
                {
                    var categoryName = db.Database.SqlQuery<SummaryView>(findCatName + " WHERE pfs2.ID IN (" + category + ")");
                    //ViewBag.CurrentFilterText += " » Category: " + categoryName.ToList<SummaryView>().FirstOrDefault().Name;
                }

                //                sql = @"select pfs2.ID AS ID, CONCAT(CASE WHEN parentCat.Title = 'Default' THEN '' ELSE CONCAT(parentCat.Title, ' > ') END, pfs2.Title) AS NAME,
                //                  COUNT(sd.sbjnum) AS OutletCount,
                //                  CAST(ABS(CHECKSUM(NewId())) % 255 AS INT) AS Color_Red,
                //                  CAST(ABS(CHECKSUM(NewId())) % 255 AS INT) AS Color_Green,
                //                  CAST(ABS(CHECKSUM(NewId())) % 255 AS INT) AS Color_Blue
                //                  FROM
                //                    (select pf.ProjectID, ct.ID AS CityID, pfs.* from ProjectField pf 
                //                        INNER JOIN ProjectFieldSample pfs ON pf.ID = pfs.FieldID 
                //                        INNER JOIN Categories cat ON pf.CategoryId = cat.Id 
                //                        INNER JOIN Country cn ON cat.CountryId = cn.ID INNER JOIN City ct ON cn.ID = ct.CountryID 
                //                    WHERE ct.ID = " + city + @" AND ParentFieldID = 1) AS parentCat 
                //                   INNER JOIN ProjectFieldSample pfs2 ON parentCat.ID = pfs2.ParentSampleID
                //                   INNER JOIN SurveyData sd ON pfs2.FieldID = sd.FieldId AND pfs2.ID IN (select ListMember from fnSplitCSV(sd.FieldValue)) 
                //                   INNER JOIN Survey s ON s.sbjnum = sd.sbjnum AND ISNULL(s.Latitude, 0) > 0 and ISNULL(s.Longitude, 0) > 0 
                //                   WHERE pfs2.IsActive = 1 AND s.CityID = " + city +
                //                   " GROUP BY pfs2.ID, parentCat.Title, pfs2.Title ORDER BY parentCat.Title, pfs2.Title";

                int projectId = 1371;
                if (country == "2" || city == "2" || city == "6" || city == "7" || city == "8" || city == "9" || city == "15" || city == "1015" || city == "2017" || city == "2026" || city == "2067" || city == "2070")
                {
                    projectId = 3390;
                }
                sql = @"SELECT SubCategoryID AS Id, SubCategoryName as Name, SUM(OutletCount) AS OutletCount, Color_Red, Color_Green, Color_Blue
                    FROM viewCensusCategoryCount v 
                    INNER JOIN City ct ON ct.ID = v.CityID
                    WHERE " + countryFilter + " AND v.ProjectID = " + projectId + " GROUP BY SubCategoryID, SubCategoryName, Color_Red, Color_Green, Color_Blue ORDER BY SubCategoryName";

                var categories = db.Database.SqlQuery<SummaryView>(sql);
                db.Database.CommandTimeout = 300;
                List<SummaryView> categoryDetail = categories.ToList<SummaryView>();
                List<SummaryView> categorySummary = new List<SummaryView>();
                int outletCount = 0;
                SummaryView lastCat = null;
                foreach (SummaryView cs in categoryDetail)
                {
                    if (lastCat != null && lastCat.ID != cs.ID)
                    {
                        lastCat.OutletCount = outletCount;
                        categorySummary.Add(lastCat);
                        outletCount = 0;
                    }
                    lastCat = cs;
                    outletCount += cs.OutletCount;
                }
                if (lastCat != null)    //Last cat will not be counted in the loop above
                {
                    lastCat.OutletCount = outletCount;
                    categorySummary.Add(lastCat);
                }
                ViewBag.CategorySummary = categorySummary;
            }


            //Outlet Sizes            
            ViewBag.ShowOutlets = false;
            //string sizeFilter = "";

            //if (city == "2" || int.Parse(city) >= 6)
            //{
            //    if (category != null && int.Parse(category) > 0 && (category.Equals("1010") || category.Equals("1011") || category.Equals("1012") || category.Equals("1013")))
            //    {
            //        string selSize = Request.QueryString.Get("os");
            //        if (selSize != null && selSize.Length > 0)
            //        {
            //            if (selSize.Equals("1"))
            //            {
            //                sizeFilter = " AND CONVERT(float, sd.fieldValue) < 100 ";
            //                ViewBag.CurrentFilterText += " » Size: < 100m";
            //            }
            //            else if (selSize.Equals("2"))
            //            {
            //                sizeFilter = " AND CONVERT(float, sd.fieldValue) >= 100 AND CONVERT(float, sd.fieldValue) <= 200 ";
            //                ViewBag.CurrentFilterText += " Size: 100m - 200m";
            //            }
            //            else if (selSize.Equals("3"))
            //            {
            //                sizeFilter = " AND CONVERT(float, sd.fieldValue) > 200 AND CONVERT(float, sd.fieldValue) <= 300 ";
            //                ViewBag.CurrentFilterText += " Size: 201m - 300m";
            //            }
            //            else if (selSize.Equals("4"))
            //            {
            //                sizeFilter = " AND CONVERT(float, sd.fieldValue) > 300 ";
            //                ViewBag.CurrentFilterText += " Size: > 300m";
            //            }
            //        }

            //        //Outlet size distribution
            //        sql = "SELECT s.sbjnum as ID, sd.fieldValue as Name, 0 AS OutletCount " +
            //               " FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum " +
            //               " INNER JOIN ProjectField pf ON sd.FieldId = pf.ID " +
            //               " WHERE s.ProjectID = 1371  AND s.CityID = " + city +
            //               " AND pf.ID = 35 AND s.sbjnum IN (SELECT sd2.sbjnum FROM SurveyData sd2 INNER JOIN ProjectField pf2 ON sd2.FieldId = pf2.ID AND pf2.ParentFieldID = 2 WHERE sd.sbjnum = sd2.sbjnum AND '" + category + "' like '%'+sd2.FieldValue+'%') " +
            //               " ORDER BY s.Created desc, s.sbjnum, pf.DisplayOrder";

            //        var outlets = db.Database.SqlQuery<SummaryView>(sql);
            //        var OutletsSizes = outlets.ToList<SummaryView>();
            //        List<SummaryView> outletSizes = new List<SummaryView>();
            //        SummaryView s1 = new SummaryView();
            //        s1.ID = 1;
            //        s1.Name = "< 100m";
            //        s1.OutletCount = 0;
            //        SummaryView s2 = new SummaryView();
            //        s2.ID = 2;
            //        s2.Name = "100m - 200m";
            //        s2.OutletCount = 0;
            //        SummaryView s3 = new SummaryView();
            //        s3.ID = 3;
            //        s3.Name = "201m - 300m";
            //        s3.OutletCount = 0;
            //        SummaryView s4 = new SummaryView();
            //        s4.ID = 4;
            //        s4.Name = "> 301m";
            //        s4.OutletCount = 0;

            //        outletSizes.Add(s1);
            //        outletSizes.Add(s2);
            //        outletSizes.Add(s3);
            //        outletSizes.Add(s4);

            //        //< 100m
            //        //100 - 200m
            //        //201 - 300m
            //        //> 300m
            //        double size = 0;

            //        foreach (SummaryView rec in OutletsSizes)
            //        {
            //            if (rec.Name.Length > 0 && double.TryParse(rec.Name.ToString(), out size))
            //            {
            //                if (size < 100)
            //                {
            //                    foreach (SummaryView os in outletSizes)
            //                    {
            //                        if (os.ID == 1) { os.OutletCount++; break; }
            //                    }
            //                }
            //                else if (size >= 100 && size <= 200)
            //                {
            //                    foreach (SummaryView os in outletSizes)
            //                    {
            //                        if (os.ID == 2) { os.OutletCount++; break; }
            //                    }
            //                }
            //                else if (size > 200 && size <= 300)
            //                {
            //                    foreach (SummaryView os in outletSizes)
            //                    {
            //                        if (os.ID == 3) { os.OutletCount++; break; }
            //                    }
            //                }
            //                else if (size > 300)
            //                {
            //                    foreach (SummaryView os in outletSizes)
            //                    {
            //                        if (os.ID == 4) { os.OutletCount++; break; }
            //                    }
            //                }
            //            }
            //        }
            //        ViewBag.OutletSizeSummary = outletSizes;
            //        ViewBag.ShowOutlets = true;
            //    }
            //}

            //if (sizeFilter.Length > 0)
            //{
            //    sizeFilter = " AND s.sbjnum IN (SELECT s.sbjnum as ID" +
            //               " FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum " +
            //               " WHERE s.ProjectID = 1371  AND s.CityID = " + city + sizeFilter +
            //               " AND sd.FieldId = 35 AND s.sbjnum IN (SELECT sd2.sbjnum FROM SurveyData sd2 INNER JOIN ProjectField pf2 ON sd2.FieldId = pf2.ID AND pf2.ParentFieldID = 2 WHERE sd.sbjnum = sd2.sbjnum AND '" + category + "' like '%'+sd2.FieldValue+'%') " +
            //               ") ";
            //}

            ////Survey data points - to be displayed on the Map with outlet name
            ///* sql = "SELECT s.sbjnum, s.Created, s.Longitude, s.Latitude, s.SurveyorName, sd.FieldId, pf.FieldType, pf.Title, sd.FieldValue " +
            //   " FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum " +
            //   " INNER JOIN ProjectField pf ON sd.FieldId = pf.ID " +
            //   " WHERE s.ProjectID = 1371 " + cityFilter +
            //   " AND pf.ParentFieldID = 5 " + filter + sizeFilter +
            //   " ORDER BY s.Created desc, s.sbjnum, pf.DisplayOrder"; */



            if (((country != null && country.Length > 0) || city != "-1") && ((category != null && category.Length > 0) || (parentcat != null && parentcat.Length > 0)))
            {
                //                sql = @"select s.sbjnum, s.Created, s.Longitude, s.Latitude, s.SurveyorName, sd2.FieldId, sd2.FieldValue ,
                //                pfs2.ID AS ID, CONCAT(CASE WHEN parentCat.Title = 'Default' THEN '' ELSE CONCAT(parentCat.Title, ' > ') END, pfs2.Title) AS NAME
                //                FROM   (select pf.ProjectID, ct.ID AS CityID, pfs.* from ProjectField pf       
                //	                INNER JOIN ProjectFieldSample pfs ON pf.ID = pfs.FieldID       
                //	                INNER JOIN Categories cat ON pf.CategoryId = cat.Id       
                //	                INNER JOIN Country cn ON cat.CountryId = cn.ID INNER JOIN City ct ON cn.ID = ct.CountryID   WHERE ct.ID = " + city + @" AND ParentFieldID = 1) 
                //	                AS parentCat  
                //                INNER JOIN ProjectFieldSample pfs2 ON parentCat.ID = pfs2.ParentSampleID 
                //                INNER JOIN SurveyData sd ON pfs2.FieldID = sd.FieldId AND pfs2.ID IN (select ListMember from fnSplitCSV(sd.FieldValue)) 
                //                INNER JOIN Survey s ON s.sbjnum = sd.sbjnum AND ISNULL(s.Latitude, 0) > 0 and ISNULL(s.Longitude, 0) > 0 
                //
                //                INNER JOIN SurveyData sd2 ON s.sbjnum = sd2.sbjnum  INNER JOIN ProjectField pf ON sd2.FieldId = pf.ID  WHERE s.ProjectID = 1371  AND pf.ParentFieldID = 5  
                //                AND pfs2.IsActive = 1 
                //
                //                AND s.CityID = " + city + filter + @" 
                //                ORDER BY parentCat.Title, pfs2.Title";

                sql = @"SELECT v.*, ISNULL((SELECT TOP 1 COUNT(*) FROM viewCensusPOI m WHERE m.sbjnum = v.sbjnum GROUP BY m.sbjnum HAVING COUNT(*) > 1), 0) AS Multiples
                    FROM viewCensusPOI v INNER JOIN City ct ON ct.ID = v.CityID WHERE " + countryFilter + filter + " ORDER BY SubCategoryName";
                //sql = @"select * FROM viewCensusPOI WHERE CityID = " + city + filter + " ORDER BY SubCategoryName";

            }
            else
            {
                sql = @"select 0 sbjnum, 0 CityID, '' Created, 0 Longitude, 0 Latitude, '' SurveyorName, '' FieldId, '' FieldValue ,
                0 AS SubCategoryID, '' AS SubCategoryName, 0 AS Multiples WHERE 1=2";
            }

            var surveys = db.Database.SqlQuery<SurveyDataView>(sql);
            db.Database.CommandTimeout = 300;
            List<SurveyDataView> surveyData = surveys.ToList<SurveyDataView>();
            //surveyData.RemoveAll(x => x.ID != Convert.ToInt32(category));

            if ((country != null && country.Length > 0) || city != "-1")
            {
                return View(surveyData);
            }
            else
            {
                return View("Index");
            }

        }

        /// /////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<District> GetDistrictsFromKML(int cityId, string cityKMLPath)
        {
            List<District> KmlDistricts = new List<District>();

            string path = Server.MapPath("~/Content/kml/");
            string kmlFileName = "kml_" + cityId + "_nonsec.kml";

            if (!System.IO.File.Exists(path + kmlFileName) && cityKMLPath.Length > 0)
            {
                kmlFileName = cityKMLPath;
            }

            if (System.IO.File.Exists(path + kmlFileName))
            {
                // This will read a Kml file into memory.
                FileStream fileStream = new FileStream(path + kmlFileName, FileMode.Open, FileAccess.Read);
                KmlFile file = KmlFile.Load(fileStream);
                Kml kml = file.Root as Kml;
                if (kml != null)
                {
                    foreach (var placemark in kml.Flatten().OfType<Placemark>())
                    {
                        //Console.WriteLine(placemark.Name);
                        string sql = string.Format(@"SELECT d.ID FROM District d LEFT OUTER JOIN DistrictMapping dm ON d.ID = dm.DistrictId AND dm.Source = 'PROJECT' 
                        WHERE (CASE WHEN dm.Name IS NULL THEN d.Name ELSE dm.Name END) = '{0}' AND d.CityId = {1}",
                        placemark.Name.Trim().Replace("'", "''"), cityId);

                        var qDist = db.Database.SqlQuery<int>(sql);
                        int districtId = qDist.FirstOrDefault<int>();

                        District newDistrict = new District { ID = districtId, CityID = cityId, Name = placemark.Name };

                        //Add districts' placemark name in List
                        KmlDistricts.Add(newDistrict);
                    }
                }
            }
            return KmlDistricts.OrderBy(d => d.Name).ToList();
        }



    }
}
