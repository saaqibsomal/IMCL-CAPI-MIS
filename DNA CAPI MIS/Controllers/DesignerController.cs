using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using DNA_CAPI_MIS.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Ajax.Utilities;

namespace DNA_CAPI_MIS.Controllers
{
    public class DesignerController : Controller
    {
        DNA_CAPI_MIS.DAL.ProjectContext db = new DNA_CAPI_MIS.DAL.ProjectContext();

        protected class DimensionSQL
        {
            public string mdsql = "";
            public string mdsql_wo_order = "";
            public string qsrcjoin = "";
            public string qsmpjoin = "";
            public string qfilter = "";
            public string qgroupby = "";
            public string qorderby = "";
            public string qfields = "";

            public void Clear()
            {
                  qsrcjoin = "";
                  qsmpjoin = "";
                  qfilter = "";
                  qgroupby = "";
                  qorderby = "";
                  qfields = "";
            }
        }
        //public class ProjectFieldAndSamples : DNA_CAPI_MIS.Models.ProjectField
        //{
        //    public List<ProjectFieldSample> ProjectFieldSamples { get; set; }
        //}

        // GET: /Designer/
        [Authorize]
        public ActionResult SelectProject(string name, string status)
        {
            return OpenProject(name, status, "SelectProject");
        }

        [Authorize]
        public ActionResult Dashboard(string name, string status)
        {
            Monitoring();
            OpenClose();

            var All = "SELECT SurveyorName, COUNT(*) AS SurveyCount FROM Survey WHERE ProjectID in( 7120,7121,7122) GROUP BY SurveyorName ORDER BY COUNT(*) DESC";
            var RHS = "SELECT SurveyorName, COUNT(*) AS SurveyCount FROM Survey WHERE ProjectID = 7120 GROUP BY SurveyorName ORDER BY COUNT(*) DESC";
            var MSU = "SELECT SurveyorName, COUNT(*) AS SurveyCount FROM Survey WHERE ProjectID = 7121 GROUP BY SurveyorName ORDER BY COUNT(*) DESC";
            var FWC = "SELECT SurveyorName, COUNT(*) AS SurveyCount FROM Survey WHERE ProjectID = 7122 GROUP BY SurveyorName ORDER BY COUNT(*) DESC";

            var queryAll = db.Database.SqlQuery<SurveyorStats>(All);
            var queryRHS = db.Database.SqlQuery<SurveyorStats>(RHS);
            var queryMSU = db.Database.SqlQuery<SurveyorStats>(MSU);
            var queryFWC = db.Database.SqlQuery<SurveyorStats>(FWC);

            if (queryAll.Count() > 0)
            {
                ViewBag.All = queryAll.Sum(x=>x.SurveyCount);
            }
            if (queryRHS.Count() > 0)
            {
                ViewBag.RHS = queryRHS.Sum(x => x.SurveyCount);
            }
            if (queryMSU.Count() > 0)
            {
                ViewBag.MSU = queryMSU.Sum(x => x.SurveyCount);
            }
            if (queryFWC.Count() > 0)
            {
                ViewBag.FWC = queryFWC.Sum(x => x.SurveyCount);
            }

            return View();
        }




        

        public void GetDistict()
        {
            List<DNA_CAPI_MIS.Models.ProjectFieldSample> Distict = db.ProjectFieldSample
                                .Where(x => x.ParentSampleID != 0 && x.IsActive && x.FieldID.Equals(50435))
                                .OrderBy(x => x.DisplayOrder)
                                .ToList<ProjectFieldSample>();

            var distinctItems = Distict.Select(x => new SelectListItem
            {
                Text = x.Title, 
                Value = x.Title
            }).ToList();


            ViewBag.Distict = distinctItems;
        
        }
        [HttpPost]
        public JsonResult GetCentral(string id)
        {
            var val = id.Split(',');
            string CheckList = val[1].ToString().Trim();
            string Disctrict = val[3].ToString();
            List<DNA_CAPI_MIS.Models.ProjectFieldSample> Central = db.ProjectFieldSample
                                .Where(x =>  x.IsActive && x.Title.Contains(CheckList) && x.Title.Contains(Disctrict))
                                .OrderBy(x => x.DisplayOrder)
                                .ToList<ProjectFieldSample>();
            var distinctItems = Central.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.Title
            }).ToList();
            return Json(distinctItems);

        }


        [HttpPost]
        public JsonResult OpenClose(string id)
        {


            var Break = id.Split('|');
            var OC = Break[1];
            var pro = Break[0];

            string Query = $@"SELECT 
    CASE 
        WHEN p.id = 7120 THEN 50577--50446 
        WHEN p.id = 7121 THEN 50484--50486 
        WHEN p.id = 7122 THEN 55587--50517 
        ELSE 0 
    END AS Id, 
    [Name] ,
	RIGHT(p.Name, CHARINDEX(' ', REVERSE(p.Name) + ' ') - 1) as shortName
INTO #Project
FROM 
    Project p
WHERE 
    p.id IN ({pro}) 
ORDER BY 
    [Name];

 
	SELECT   
    
    pf.Title,
 
    (SELECT COUNT(*) 
     FROM ProjectFieldSample 
     WHERE IsActive in ({OC})  and  Title LIKE '%' + p.shortName + '%' AND Title LIKE '% ' + pf.Title + '%'
    ) AS OpenCenter 
FROM 
    #Project p 
INNER JOIN 
    ProjectFieldSample pf ON p.ID = pf.FieldID; ";
            var Pie = db.Database.SqlQuery<BarChart>(Query);

            var distinctItems = Pie.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.OpenCenter.ToString()
            }).ToList();
            return Json(distinctItems);

        }

        [HttpPost]
        public JsonResult BarChart(int id)
        {


            string Query = $@"SELECT 
    CASE 
        WHEN p.id = 7120 THEN 50577--50446 
        WHEN p.id = 7121 THEN 50484--50486 
        WHEN p.id = 7122 THEN 55587--50517 
        ELSE 0 
    END AS Id, 
    [Name] ,
	RIGHT(p.Name, CHARINDEX(' ', REVERSE(p.Name) + ' ') - 1) as shortName
INTO #Project
FROM 
    Project p
WHERE 
    p.id IN ({id}) 
ORDER BY 
    [Name];

 
	SELECT   
    
    pf.Title,
 
    (SELECT COUNT(*) 
     FROM ProjectFieldSample 
     WHERE Title LIKE '%' + p.shortName + '%' AND Title LIKE '% ' + pf.Title + '%'
    ) AS OpenCenter 
FROM 
    #Project p 
INNER JOIN 
    ProjectFieldSample pf ON p.ID = pf.FieldID; ";
            var Pie = db.Database.SqlQuery<BarChart>(Query);
          
            var distinctItems = Pie.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.OpenCenter.ToString()
            }).ToList();
            return Json(distinctItems);

        }
           
        
        
        [HttpPost]
        public JsonResult DistrictBarChart(string id)
        {
            
            if(id == "NaN")
            {
                id = "";
            }
           
            string where = string.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                where = $"and g.DistrictId in ({id})";
                
                 
            }
            else
            {
                where = "";
            }


            string Query = $@"
IF OBJECT_ID('tempdb..#Graph') IS NOT NULL
BEGIN
    DROP TABLE #Graph;
END

;with cte as (
	select s.sbjnum, s.Created, 
		sd1.fieldId as FieldId1, sd1.fieldValue as FieldValue1, 
		sd2.fieldId as FieldId2, sd2.fieldValue as FieldValue2,
		sd3.fieldId as FieldId3, sd3.fieldValue as FieldValue3,
	row_number() over (partition by sd1.fieldId, sd1.fieldValue, sd2.fieldId, sd2.fieldValue order by s.created desc) as RowNum
	from survey s
		inner join SurveyData sd1 on s.sbjnum = sd1.sbjnum and sd1.FieldId in (50446, 50486, 55588)--Center,Center,Center Ids
		inner join SurveyData sd2 on s.sbjnum = sd2.sbjnum and sd2.FieldId in (50435, 50484, 55587)--District,District,District Ids 
		inner join SurveyData sd3 on s.sbjnum = sd3.sbjnum and sd3.FieldId in (55570, 50482, 55585)--Open,Open,open close Survey Ids
)
select fs1.FieldID as CenterId,fs2.FieldID DistrictId,  fs2.Title as District,fs1.Title as Center,  

fs2.Title as DiscrtictGroup,
fs3.Title as OpenClose, case when fs3.Title = 'Open' then 1 else  0 end IsOpen,fs3.Title
 

    into #Graph
	from cte
	inner join ProjectFieldSample fs1 on cte.FieldId1 = fs1.FieldID and fs1.Code IN (cte.FieldValue1)
	inner join ProjectFieldSample fs2 on cte.FieldId2 = fs2.FieldID and fs2.Code IN (cte.FieldValue2)
	inner join ProjectFieldSample fs3 on cte.FieldId3 = fs3.FieldID and fs3.Code IN (cte.FieldValue3)
    where RowNum = 1 

	select  Count(g.IsOpen) OpenClose,g.Title     from  #Graph as g where g.IsOpen in (1,0) {where} 
	group by  g.IsOpen ,g.Title  

";
            var Pie = db.Database.SqlQuery<PieChartOC>(Query);
          
            var distinctItems = Pie.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.OpenClose.ToString()
            }).ToList();
            return Json(distinctItems);

        }       
        
        
        [HttpPost]
        public JsonResult CenterPieChart(string id)
        {
            
            if(id == "NaN")
            {
                id = "";
            }

            var Text = id.Split(',')[0];
            var val = id.Split(',')[1];


            string where = string.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                where = $"and g.District like '%{Text}%'";
                
                 
            }
            else
            {
                where = "";
            }


            string Query = $@"

IF OBJECT_ID('tempdb..#Graph') IS NOT NULL
BEGIN
    DROP TABLE #Graph;
END

;with cte as (
	select s.sbjnum, s.Created, 
		sd1.fieldId as FieldId1, sd1.fieldValue as FieldValue1, 
		sd2.fieldId as FieldId2, sd2.fieldValue as FieldValue2,
		sd3.fieldId as FieldId3, sd3.fieldValue as FieldValue3,
	row_number() over (partition by sd1.fieldId, sd1.fieldValue, sd2.fieldId, sd2.fieldValue order by s.created desc) as RowNum
	from survey s
		inner join SurveyData sd1 on s.sbjnum = sd1.sbjnum and sd1.FieldId in (50446, 50486, 55588)--Center,Center,Center Ids
		inner join SurveyData sd2 on s.sbjnum = sd2.sbjnum and sd2.FieldId in (50435, 50484, 55587)--District,District,District Ids 
		inner join SurveyData sd3 on s.sbjnum = sd3.sbjnum and sd3.FieldId in (55570, 50482, 55585)--Open,Open,open close Survey Ids
)
select fs1.FieldID as CenterId,fs2.FieldID DistrictId,  fs2.Title as District,fs1.Title as Center,  

fs2.Title as DiscrtictGroup,
fs3.Title as OpenClose, case when fs3.Title = 'Open' then 1 else  0 end IsOpen
 

    into #Graph
	from cte
	inner join ProjectFieldSample fs1 on cte.FieldId1 = fs1.FieldID and fs1.Code IN (cte.FieldValue1)
	inner join ProjectFieldSample fs2 on cte.FieldId2 = fs2.FieldID and fs2.Code IN (cte.FieldValue2)
	inner join ProjectFieldSample fs3 on cte.FieldId3 = fs3.FieldID and fs3.Code IN (cte.FieldValue3)
    where RowNum = 1 

	select count(g.DiscrtictGroup) DiscrtictGroup ,case when g.IsOpen = 0 then 'Close' else 'Open' end Title, g.IsOpen OpenClose  from  #Graph as g where g.IsOpen in (1,0) and g.DistrictId in ({val})
	{where}
	group by g.District , g.IsOpen


 

";
            var Pie = db.Database.SqlQuery<PieChartOC>(Query);
          
            var distinctItems = Pie.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.OpenClose.ToString()
            }).ToList();
            return Json(distinctItems);

        }

        [HttpPost]
        public JsonResult GetDistictById(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return Json(null);
            }
            
            int FieldID = Convert.ToInt32(id.Split(',')[0]);
            List<DNA_CAPI_MIS.Models.ProjectFieldSample> Distict = db.ProjectFieldSample
                                .Where(x => x.ParentSampleID != 0 && x.IsActive && x.FieldID.Equals(FieldID)  )
                                .OrderBy(x => x.DisplayOrder)
                                .ToList<ProjectFieldSample>();

            var distinctItems = Distict.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.FieldID.ToString().Trim()
            }).ToList();
     
            return Json(distinctItems);

        }

        [HttpPost]
        public JsonResult ForAllMonitoring()
        {
            var All = "SELECT SurveyorName, COUNT(*) AS SurveyCount FROM Survey WHERE ProjectID in( 7120,7121,7122) GROUP BY SurveyorName ORDER BY COUNT(*) DESC";
            var RHS = "SELECT SurveyorName, COUNT(*) AS SurveyCount FROM Survey WHERE ProjectID = 7120 GROUP BY SurveyorName ORDER BY COUNT(*) DESC";
            var MSU = "SELECT SurveyorName, COUNT(*) AS SurveyCount FROM Survey WHERE ProjectID = 7121 GROUP BY SurveyorName ORDER BY COUNT(*) DESC";
            var FWC = "SELECT SurveyorName, COUNT(*) AS SurveyCount FROM Survey WHERE ProjectID = 7122 GROUP BY SurveyorName ORDER BY COUNT(*) DESC";

            var queryAll = db.Database.SqlQuery<SurveyorStats>(All);
            var queryRHS = db.Database.SqlQuery<SurveyorStats>(RHS);
            var queryMSU = db.Database.SqlQuery<SurveyorStats>(MSU);
            var queryFWC = db.Database.SqlQuery<SurveyorStats>(FWC);
            TotalSurveyDetail res = new TotalSurveyDetail();
            if (queryAll.Count() > 0)
            {
                res.All = queryAll.Sum(x => x.SurveyCount);
            }
            if (queryRHS.Count() > 0)
            {
                res.RHS = queryRHS.Sum(x => x.SurveyCount);
            }
            if (queryMSU.Count() > 0)
            {
                res.MSU = queryMSU.Sum(x => x.SurveyCount);
            }
            if (queryFWC.Count() > 0)
            {
                res.FWC = queryFWC.Sum(x => x.SurveyCount);
            }

            return Json(res);
        }
        [HttpPost]
        public JsonResult ForSelectedMonitoring(string id)
        {
           
            //7120 7121 7122
            if (string.IsNullOrEmpty(id))
            {
                return Json(null);
            }
            int CenterOpenCloseID = 0;
            if (id.Split(',')[1].Trim() == "RHS")
            {
                CenterOpenCloseID = 55570 ;
            }
            else if (id.Split(',')[1].Trim() == "MSU")
            {
                CenterOpenCloseID = 50482 ;
            }
            else if (id.Split(',')[1].Trim() == "FWC")
            {
                CenterOpenCloseID = 55585;
            }
            var all = $"SELECT SurveyorName, COUNT(*) AS SurveyCount FROM Survey WHERE ProjectID in ({id.Split(',')[2]}) GROUP BY SurveyorName ORDER BY COUNT(*) DESC";

            var OpenCloseSql = $@"
IF OBJECT_ID('tempdb..#Graph') IS NOT NULL
BEGIN
    DROP TABLE #Graph;
END

;with cte as (
	select s.sbjnum, s.Created, 
		sd1.fieldId as FieldId1, sd1.fieldValue as FieldValue1, 
		sd2.fieldId as FieldId2, sd2.fieldValue as FieldValue2,
		sd3.fieldId as FieldId3, sd3.fieldValue as FieldValue3,
	row_number() over (partition by sd1.fieldId, sd1.fieldValue, sd2.fieldId, sd2.fieldValue order by s.created desc) as RowNum
	from survey s
		inner join SurveyData sd1 on s.sbjnum = sd1.sbjnum and sd1.FieldId in (50446, 50486, 55588)--Center,Center,Center Ids
		inner join SurveyData sd2 on s.sbjnum = sd2.sbjnum and sd2.FieldId in (50435, 50484, 55587)--District,District,District Ids 
		inner join SurveyData sd3 on s.sbjnum = sd3.sbjnum and sd3.FieldId in ({CenterOpenCloseID})--Open,Open,open close Survey Ids
)
select fs1.FieldID as CenterId,fs2.FieldID DistrictId,  fs2.Title as District,fs1.Title as Center,  

fs2.Title as DiscrtictGroup,
fs3.Title as OpenClose, case when fs3.Title = 'Open' then 1 else  0 end IsOpen,fs3.Title
 

    into #Graph
	from cte
	inner join ProjectFieldSample fs1 on cte.FieldId1 = fs1.FieldID and fs1.Code IN (cte.FieldValue1)
	inner join ProjectFieldSample fs2 on cte.FieldId2 = fs2.FieldID and fs2.Code IN (cte.FieldValue2)
	inner join ProjectFieldSample fs3 on cte.FieldId3 = fs3.FieldID and fs3.Code IN (cte.FieldValue3)
    where RowNum = 1 

	select  Count(g.IsOpen) OpenClose,g.Title     from  #Graph as g where g.IsOpen in (1,0) and g.DistrictId in (50435)
	group by  g.IsOpen ,g.Title  


";


            var Openclose = db.Database.SqlQuery<OpenCloseResponse>(OpenCloseSql);
            var queryFWC = db.Database.SqlQuery<SurveyorStats>(all);
            TotalSurveyDetail res = new TotalSurveyDetail();
            if (queryFWC.Count() > 0)
            {
                res.All = queryFWC.Sum(x => x.SurveyCount);
                res.Name = id.Split(',')[1];
            }
            if(Openclose.Count() > 0)
            {
                foreach (var item in Openclose.ToList())
                {
                    if(item.Title == "Close")
                    {
                        res.Close = item.OpenClose;
                        res.CloseTitle = item.Title;
                    }
                    else
                    {
                        res.Open = item.OpenClose;
                        res.OpenTitle = item.Title;
                    }
                }
            }
            else
            {
                res.Close = 0 ;
                res.CloseTitle = "Close";
                res.Open = 0;
                res.OpenTitle = "Open";
            }
            return Json(res);
        }

        public void OpenClose()
        {


            var distinctItems = new List<SelectListItem>();

            distinctItems.Add(new SelectListItem
            {
                Text = "Select both",
                Value = "1,0"
            });
            // Hardcode "Open" option with value true
            distinctItems.Add(new SelectListItem
            {
                Text = "Open",
                Value = "1"
            });

            // Hardcode "Close" option with value false
            distinctItems.Add(new SelectListItem
            {
                Text = "Close",
                Value = "0"
            });

 

            // Convert the list to a List<SelectListItem>
            distinctItems = distinctItems.ToList();
            ViewBag.OpenClose = distinctItems;
           

        }

        public void Central()
        {
            var dummyData = new List<ProjectFieldSample> { new ProjectFieldSample { Title = "select", Code = "1" }, };
            var Centrals = dummyData.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.Code.ToString()
            }).ToList();
            ViewBag.Centrals = Centrals; ;
        }
    
        public ActionResult OpenProject(string name, string status, string viewName = "OpenProject")
        {
            if (string.IsNullOrEmpty(status))
            {
                //status = "'D','T','P','C'";
                status = "'D','T','P'";                 //By default show only these projects
            }
            string sql = "";
            if (User.IsInRole("Admin"))
            {
                sql = @"SELECT * FROM Project WHERE (name like '%" + name + "%' OR guid like '%" + name + "%') and isnull(status, 'D') IN (" + status + ") ORDER BY name";
            }
            else
            {
                string userFilter = "";
                string uid = User.Identity.GetUserId();
                userFilter = " AND pur.UserId = " + uid;
                sql = @"SELECT * FROM project p WHERE (name like '%" + name + "%' OR guid like '%" + name + "%') AND isnull(status, 'D') IN (" + status + ")  AND id IN (SELECT ObjectValue FROM DNAShared2.dbo.UserRights pur WHERE pur.ObjectName = 'PROJECT'" + userFilter + ")";
            } 

            var query = db.Database.SqlQuery<ProjectsList>(sql);
            List<ProjectsList> projects = query.ToList<ProjectsList>();

            return View(viewName, projects);
        }

        
        public void Monitoring()
        {
           string sql = @"SELECT case 
 
when id = 7120 then 50435--50446 
when id = 7121 then 50484--50486 
when id = 7122 then 55587--50517 
else 0 end Id , Name,id as RoleId      FROM Project WHERE id in (7120,7121,7122) ORDER BY name"; //7114 ,
            var CheckFor = db.Database.SqlQuery<ProjectsList>(sql);
             
            
            var Checklist = CheckFor.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString() + "," + x.Name.Split('-')[1] +","+ x.RoleId.ToString(),
            }).ToList();

            var dummyData = new List<ProjectFieldSample> { new ProjectFieldSample { Title = "Select District", Code = "0" }, };
            var District = dummyData.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.Code.ToString()
            }).ToList();


            var dummyData2 = new List<ProjectFieldSample> { new ProjectFieldSample { Title = "Select Center", Code = "0" }, };
            var Center = dummyData2.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.Code.ToString()
            }).ToList();


            ViewBag.Center = Center;
            ViewBag.District = District;
            ViewBag.Checklist = Checklist;
        }


        [Authorize(Roles = "Admin")]
        //[Authorize(Roles = "Admin,CanEdit,Field Supervisor,Project Manager")]
        public ActionResult ProjectOverview(int? id)
        {
            if (id > 0)
            {
                string sql = "";

                sql = "SELECT p.id, p.name, COUNT(s.sbjnum) AS SurveyCount FROM Project p LEFT OUTER JOIN Survey s ON s.ProjectID = p.id WHERE p.id = " + id + 
                      " GROUP BY p.id, p.name";

                var query1 = db.Database.SqlQuery<ProjectsInfo>(sql);
                ProjectsInfo project = query1.FirstOrDefault<ProjectsInfo>();

                sql = "SELECT SurveyorName, COUNT(*) AS SurveyCount FROM Survey WHERE ProjectID = " + id +
                      " GROUP BY SurveyorName ORDER BY COUNT(*) DESC";

                var query2 = db.Database.SqlQuery<SurveyorStats>(sql);
                List<SurveyorStats> stats = query2.ToList<SurveyorStats>();
                ViewBag.SurveyorStats = stats;
                if (stats.Count > 0)
                {
                    ViewBag.MaxSurveysByASurveyor = stats.FirstOrDefault<SurveyorStats>().SurveyCount;
                }
                Central();
                GetDistict();

               
                return View(project);
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult DesignReport(FormCollection form)
        {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Objects;
            settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

            int pid = Convert.ToInt32(form["ProjectID"].ToString());

            string fieldsTop = form["FieldsTop"].ToString();
            List<QueryDesignerFields> qfTop = JsonConvert.DeserializeObject<List<QueryDesignerFields>>(fieldsTop, settings);

            string fieldsSide = form["FieldsSide"].ToString();
            List<QueryDesignerFields> qfSide = JsonConvert.DeserializeObject<List<QueryDesignerFields>>(fieldsSide, settings);

            //Generate Tabulation
            string sql = "";

            //sql = "SELECT aqd.* FROM AnalyzerQueryDimension aqd WHERE aqd.QueryID = 1 AND aqd.Position = 'Side' ORDER BY aqd.DisplayOrder";

            //var aQuery = db.Database.SqlQuery<AnalyzerQueryDimension>(sql);
            //List<AnalyzerQueryDimension> rdxSide = aQuery.ToList<AnalyzerQueryDimension>();

            //sql = "SELECT aqd.* FROM AnalyzerQueryDimension aqd WHERE aqd.QueryID = 1 AND aqd.Position = 'Top' ORDER BY aqd.DisplayOrder";

            sql = "SELECT Id, ParentFieldId, FieldType, Title, DisplayOrder FROM ProjectField WHERE ProjectID = " + pid;
            var pfQuery = db.Database.SqlQuery<ProjectField>(sql);
            List<ProjectField> projectFields = pfQuery.ToList<ProjectField>();
            
            sql = "SELECT aqd.* FROM AnalyzerQueryDimension aqd WHERE 1 = 2";

            var aQuery = db.Database.SqlQuery<AnalyzerQueryDimension>(sql);

            //Populate Top area
            List<AnalyzerQueryDimension> rdxTop = aQuery.ToList<AnalyzerQueryDimension>();
            PopulateDimension(ref rdxTop, qfTop, projectFields, "Top");

            //Populate Side area
            List<AnalyzerQueryDimension> rdxSide = aQuery.ToList<AnalyzerQueryDimension>();
            PopulateDimension(ref rdxSide, qfSide, projectFields, "Side");

            Dictionary<int, DimensionSQL> colSQL = new Dictionary<int, DimensionSQL>();
            int TopIndex = 0;
            int SideIndex = 0;

            //-------------------------------------------------------------------------------------------------------
            //Add TOP Groups with Base
            //Set Row/Column Group Titles
            //List of all values is required for Column Groups because we need to create columns for all the items.

            Dictionary<int, GroupInfo> TopGroups = new Dictionary<int, GroupInfo>();
            foreach (AnalyzerQueryDimension d in rdxTop)
            {
                if (d.HasChild) { continue; }
                if (d.FieldType == "RDO" || d.FieldType == "CHK" || d.FieldType == "DDN" || d.FieldType == "LVW")
                {
                    sql = @"SELECT pfs.Id, pfs.Title, count(*) AS GroupBaseCount 
                            FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum 
                            INNER JOIN ProjectFieldSample pfs ON sd.FieldId = pfs.FieldId AND (pfs.ParentSampleID <> 0 OR pfs.ParentSampleID IS NULL) AND pfs.IsActive = 1
                            LEFT OUTER JOIN ProjectFieldSample pfsParent ON pfs.ParentSampleID = pfsParent.ID AND pfsParent.IsActive = 1
                            LEFT OUTER JOIN ProjectField pf ON pfs.FieldID = pf.ID 
                            WHERE sd.FieldId = " + d.ProjectFieldID + @" AND (pfs.Coding IN (SELECT ListMember FROM fnSplitCSV(sd.FieldValue)) OR sd.FieldValue = ''+pfs.Coding+'')
                            GROUP BY pf.ID, pfsParent.ID, pf.Title, pfsParent.Title, pfs.Id, pfs.Title";

                    var GroupTitles = db.Database.SqlQuery<SimpleListWithAggregates>(sql);

                    TopGroups.Add(TopGroups.Count, new GroupInfo { Id = d.ProjectFieldID, Title = d.ProjectFieldTitle, ParentId = 0, Items = GroupTitles.ToList<SimpleListWithAggregates>() });
                }
                else if (d.FieldType == "CTY")
                {
                    sql = @"SELECT pfs.Id, pfs.Name AS Title, count(*) AS GroupBaseCount 
                            FROM Survey s INNER JOIN City pfs ON s.CityId = pfs.Id 
                            WHERE pfs.Id IN (2,6,7,10,11,12,13,14) AND s.ProjectId = " + pid + " GROUP BY pfs.ID, pfs.Name ORDER BY pfs.Name";

                    var GroupTitles = db.Database.SqlQuery<SimpleListWithAggregates>(sql);

                    TopGroups.Add(TopGroups.Count, new GroupInfo { Id = d.ProjectFieldID, Title = d.ProjectFieldTitle, ParentId = 0, Items = GroupTitles.ToList<SimpleListWithAggregates>() });
                }
            }
            ViewBag.ColumnGroup = TopGroups;            //Set value for view

            //Add SIDE Row Groups with Base
            Dictionary<int, GroupInfo> SideGroups = new Dictionary<int, GroupInfo>();
            foreach (AnalyzerQueryDimension d in rdxSide)
            {
                if (d.HasChild) { continue; }
                if (d.FieldType == "RDO" || d.FieldType == "CHK" || d.FieldType == "DDN" || d.FieldType == "LVW")
                {
                    sql = @"SELECT Id, Title FROM ProjectField WHERE Id = " + d.ProjectFieldID;

                    var GroupTitles = db.Database.SqlQuery<SimpleListWithAggregates>(sql);
                    SideGroups.Add(SideGroups.Count, new GroupInfo { Id = d.ProjectFieldID, Title = d.ProjectFieldTitle, ParentId = 0, Field = "SideGroupTitle" });
                }
                else if (d.FieldType == "CTY")
                {
                    SideGroups.Add(SideGroups.Count, new GroupInfo { Id = d.ProjectFieldID, Title = d.ProjectFieldTitle, ParentId = 0, Field = "SideId" });
                }

                //Multiple groups not supported yet
                break;
            }
            ViewBag.RowGroup = SideGroups;            //Set value for view

            //-------------------------------------------------------------------------------------------------------
            //Generate SQL 
            foreach (AnalyzerQueryDimension dTop in rdxTop)
            {
                if (dTop.HasChild) { continue; }
                DimensionSQL tSQL = new DimensionSQL();
                AnalyzerQueryDimension d;

                //Loop two times to create SQL joins between top and side
                foreach (AnalyzerQueryDimension dSide in rdxSide)
                {
                    if (dSide.HasChild) { continue; }

                    for (short dx = 0; dx < 2; dx++)
                    {
                        if (dx == 0)
                        {
                            d = dSide;
                        }
                        else
                        {
                            d = dTop;
                        }

                        GenerateSQL(SideIndex, TopIndex, d, ref tSQL);

                        ReplacePlaceholders(d, ref tSQL);
                    }

                    //Merge SQL fragments
                    if (tSQL.qfilter.Length > 0)
                    {
                        tSQL.qfilter = "WHERE " + tSQL.qfilter + " ";
                    }
                    if (tSQL.qgroupby.Length > 0)
                    {
                        tSQL.qgroupby = "GROUP BY " + tSQL.qgroupby + " ";
                    }
                    if (tSQL.qorderby.Length > 0)
                    {
                        //SideIndex, TopIndex
                        tSQL.qorderby = "ORDER BY 1, 2, " + tSQL.qorderby + " ";
                    }

                    if (SideIndex > 0)
                    {   //Mid
                        tSQL.mdsql += "UNION (SELECT " + tSQL.qfields + ", count(*) AS AggregateValue, 0 AS GroupBaseCount FROM Survey SurveySource " + tSQL.qsrcjoin + tSQL.qsmpjoin + tSQL.qfilter + tSQL.qgroupby + ")";

                        //Last
                        if (SideIndex >= rdxSide.Count - 1)
                        {
                            tSQL.mdsql += " " + tSQL.qorderby;
                        }
                    }
                    else
                    {   //First
                        tSQL.mdsql = "SELECT " + tSQL.qfields + ", count(*) AS AggregateValue, 0 AS GroupBaseCount FROM Survey SurveySource " + tSQL.qsrcjoin + tSQL.qsmpjoin + tSQL.qfilter + tSQL.qgroupby;
                    }
                    tSQL.mdsql_wo_order = tSQL.mdsql.Replace(tSQL.qorderby, "");
                    tSQL.Clear();

                    SideIndex++;
                }

                colSQL.Add(TopIndex++, tSQL);

                break; //Testing single Top dimension only at this time
            }

            var singleCrosstab = db.Database.SqlQuery<BasicSingleCrosstab>(colSQL[0].mdsql);
            List<BasicSingleCrosstab> tabularData = singleCrosstab.ToList<BasicSingleCrosstab>();

            //Get Group Base; Count
            sql = "SELECT SideIndex, TopId, sum(AggregateValue) AS GroupBaseCount FROM (" + colSQL[0].mdsql_wo_order + ") AS Tabulation GROUP BY SideIndex, TopId";
            var singleCrosstabBase = db.Database.SqlQuery<BasicSingleCrosstabBase>(sql);
            List<BasicSingleCrosstabBase> tabularBaseData = singleCrosstabBase.ToList<BasicSingleCrosstabBase>();
            foreach (BasicSingleCrosstabBase ctBase in tabularBaseData)
            {
                foreach (BasicSingleCrosstab ctData in tabularData)
                {
                    if (ctData.SideIndex == ctBase.SideIndex && ctData.TopId == ctBase.TopId)
                    {
                        ctData.GroupBaseCount = ctBase.GroupBaseCount;
                    }
                }
            }

            return View("Tabulation", tabularData);
        }

        [Authorize]
        [HttpGet]
        public ActionResult DesignReport(int? id)
        {
            if (id > 0)
            {
                List<ProjectField> pfields = GetProjectFields((int)id);

                ViewBag.ProjectId = id;

                return View(pfields);
            }
            else
            {
                return View();
            }
        }

        private List<ProjectField> GetProjectFields(int id)
        {
            string sql = "";

            sql = @"SELECT pf.Id, ISNULL(pf.ParentFieldID, 0), pf.FieldType, pf.Title, pf.DisplayOrder FROM ProjectField pf WHERE FieldType IN ('CTY', 'RDO', 'CHK', 'DDN', 'LVW') AND pf.ProjectID = " + id;

            var query = db.Database.SqlQuery<ProjectField>(sql);
            return query.ToList<ProjectField>();
        }

        [Authorize]
        [HttpPost]
        public ActionResult PQSettingsEdit(int? id, int? fieldId)
        {
            return PQSettings(id, fieldId, "");
        }

        [HttpGet]
        public ActionResult ImportOptions()
        {
            return View();
        }

        public class ProjectFieldOrder
        {
            public int id { get; set; }
            public int order { get; set; }
        }

        [Authorize]
        [HttpPost]
        public ActionResult PQSettingsSetOrder(int? id, List<ProjectFieldOrder> fldOrder)
        {
            string sql = "";
            ViewBag.ResponseCode = 200;
            ViewBag.ResponseMessage = "";

            if (fldOrder != null)
            {
                foreach (ProjectFieldOrder pfo in fldOrder)
                {
                    sql += string.Format("UPDATE ProjectField SET DisplayOrder = {0} WHERE ID = {1} AND ProjectId = {2}; ", pfo.order, pfo.id, id);
                }
                if (sql.Length > 0) db.Database.ExecuteSqlCommand(sql);
            }
            return View("PQSettingsDelete");
        }

        [Authorize]
        [HttpPost]
        public ActionResult PQSettingsDelete(int? id, int? fieldId)
        {
            string sql = "";
            ViewBag.ResponseCode = 200;
            ViewBag.ResponseMessage = "";

            sql = @"SELECT count(*) cnt FROM SurveyData sd INNER JOIN Survey s ON s.sbjnum = sd.sbjnum WHERE ISNULL(s.OpStatus, 2) = 1 AND ISNULL(s.QcStatus, 1) = 1 AND sd.FieldID = " + fieldId;

            var query = db.Database.SqlQuery<int>(sql);
            var data = query.FirstOrDefault<int>();
            if (data > 0)
            {
                ViewBag.ResponseCode = 400;
                ViewBag.ResponseMessage = "Question is used in Surveys. To delete a question, first Reject all records from Field Approval and QC.";
            }
            else
            {
                sql = "DELETE FROM ProjectField WHERE id = " + fieldId;
                db.Database.ExecuteSqlCommand(sql);
            }
            return View();
        }

        //[Authorize]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult PQSettings(FormCollection form)
        {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Objects;
            settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

            int pid = Convert.ToInt32(form["ProjectID"].ToString());
            int fid = string.IsNullOrEmpty(form["FieldID"].ToString()) ? 0 : Convert.ToInt32(form["FieldID"].ToString());
            int parentfid = string.IsNullOrEmpty(form["ParentFieldID"].ToString()) ? 0 : Convert.ToInt32(form["ParentFieldID"].ToString());

            string questionType = form["questionType"].ToString();

            if (fid == 0 && questionType.Length == 0)
            {
                return null;
            }
            string reportTitle = form["reportTitle"].ToString();
            string questionTitle_en = HttpUtility.UrlDecode(form["questionTitle-en"].ToString());
            string questionTitle_ar = HttpUtility.UrlDecode(form["questionTitle-ar"].ToString());
            string instructions_en = HttpUtility.UrlDecode(form["instructions-en"].ToString());
            string instructions_ar = HttpUtility.UrlDecode(form["instructions-ar"].ToString());

            string fieldSection = form["fieldSection"].ToString();
            string variableName = form["variableName"].ToString();
            string dataCoding = form["FieldCodingData"].ToString();
            string dataCodingQ = form["FieldCodingDataQ"].ToString();

            string scriptOnEntry = form["scriptOnEntry"].ToString().Equals("Script ...") ? "" : form["scriptOnEntry"].ToString();
            string scriptOnValidate = form["scriptOnValidate"].ToString().Equals("Script ...") ? "" : form["scriptOnValidate"].ToString();
            string scriptOnExit = form["scriptOnExit"].ToString().Equals("Script ...") ? "" : form["scriptOnExit"].ToString();

            string IsMandatory = form.AllKeys.Contains("IsMandatory") && form["IsMandatory"].ToString() == "on" ? "1" : "0";
            string IsVisible = form.AllKeys.Contains("IsVisible") && form["IsVisible"].ToString() == "on" ? "1" : "0";
            string MinValueLength = form["MinValueLength"].ToString();
            string MaxValueLength = form["MaxValueLength"].ToString();
            string InputLines = form["InputLines"].ToString();
            string Orientation = (form["rdoOrientation"] == null ? "" : form["rdoOrientation"].ToString());

            //Save in Database
            DNA_CAPI_MIS.Models.ProjectField pf = new DNA_CAPI_MIS.Models.ProjectField();
            DNA_CAPI_MIS.Models.Translation lang = new DNA_CAPI_MIS.Models.Translation();
            List<DNA_CAPI_MIS.Models.ProjectFieldSample> pfsList = JsonConvert.DeserializeObject<List<DNA_CAPI_MIS.Models.ProjectFieldSample>>(dataCoding, settings);
            List<DNA_CAPI_MIS.Models.ProjectFieldSample> pfsListQ = JsonConvert.DeserializeObject<List<DNA_CAPI_MIS.Models.ProjectFieldSample>>(dataCodingQ, settings);
            List<Dictionary<string, string>> pfsAllFields = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(dataCoding, settings);
            List<Dictionary<string, string>> pfsAllFieldsQ = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(dataCodingQ, settings);

            int fieldSectionId = 0;
            if (fieldSection.Length > 0)
            {
                var pfs = db.ProjectFieldSection.Where(x => x.ProjectID == pid && x.Name.Equals(fieldSection)).FirstOrDefault<ProjectFieldSection>();
                if (pfs != null)
                {
                    fieldSectionId = pfs.ID;
                }
            }

            
            if (fid > 0)
            {
                pf = db.ProjectField.Find(fid);
                if (pf == null)
                {
                    pf = new DNA_CAPI_MIS.Models.ProjectField();
                }
            }

            if (parentfid != 0) pf.ParentFieldID = parentfid;
            pf.ProjectID = pid;
            pf.IsActive = true;
            pf.IsMandatory = IsMandatory.Equals("1") ? true : false;
            pf.IsVisible = IsVisible.Equals("1") ? true : false;
            pf.ScriptOnEntry = scriptOnEntry;
            pf.ScriptOnValidate = scriptOnValidate;
            pf.ScriptOnExit = scriptOnExit;
            pf.FieldType = questionType;
            pf.Title = questionTitle_en;
            pf.Instructions = instructions_en;
            pf.ReportTitle = reportTitle;
            pf.VariableName = variableName;
            pf.SectionID = fieldSectionId;

            var pfOptionJSON = new Dictionary<string, Object>();
            if (pf.OptionsJSON != null && pf.OptionsJSON.Length > 0)
            {
                //DeSerialize JSON into Dictionary object
                var optDeSerializer = new JsonFx.Json.JsonReader();
                dynamic output = optDeSerializer.Read(pf.OptionsJSON);
                if (output != null)
                {
                    PopulateAttributeList(pfOptionJSON, output);
                }
            }
            if (MinValueLength.Length > 0)
            {
                if (pfOptionJSON.ContainsKey("MinLength"))
                {
                    pfOptionJSON["MinLength"] = Convert.ToInt32(MinValueLength);
                }
                else
                {
                    pfOptionJSON.Add("MinLength", Convert.ToInt32(MinValueLength));
                }
            }
            if (MaxValueLength.Length > 0)
            {
                if (pfOptionJSON.ContainsKey("MaxLength"))
                {
                    pfOptionJSON["MaxLength"] = Convert.ToInt32(MaxValueLength);
                }
                else
                {
                    pfOptionJSON.Add("MaxLength", Convert.ToInt32(MaxValueLength));
                }
            }
            if (InputLines.Length > 0)
            {
                if (pfOptionJSON.ContainsKey("NoOfLines"))
                {
                    pfOptionJSON["NoOfLines"] = Convert.ToInt32(InputLines);
                }
                else
                {
                    pfOptionJSON.Add("NoOfLines", Convert.ToInt32(InputLines));
                }
            }
            if (Orientation.Length > 0)
            {
                if (pfOptionJSON.ContainsKey("Orientation"))
                {
                    pfOptionJSON["Orientation"] = Orientation;
                }
                else
                {
                    pfOptionJSON.Add("Orientation", Orientation);
                }
            }
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            pf.OptionsJSON = serializer.Serialize(pfOptionJSON);

            if (fid == 0)
            {
                string maxsql = "SELECT ISNULL(MAX(DisplayOrder), 0) FROM ProjectField WHERE ProjectId = " + pid;
                pf.DisplayOrder = db.Database.SqlQuery<int>(maxsql).First<int>() + 1;

                db.ProjectField.Add(pf);
            }
            db.SaveChanges();

            bool new_title_ar = (fid == 0);
            if (fid > 0)
            {
                var query = db.Translation.SingleOrDefault(t => t.EntityName == "PROJECTFIELD" && t.FieldName == "TITLE" && t.KeyValue == pf.ID.ToString() && t.Language == "ar-SA");
                if (query != null)
                {
                    if (questionTitle_ar.Length == 0)
                    {
                        db.Translation.Attach(query);                       //Only generate DELETE statement without Quering
                        db.Translation.Remove(query);
                    }
                    else
                    {
                        query.Text = questionTitle_ar;
                        db.Translation.Attach(query);                       //Only generate an UPDATE statement without Quering
                        var entry = db.Entry(query);
                        entry.Property(e => e.Text).IsModified = true;
                    }
                    db.SaveChanges();
                }
                else
                {
                    new_title_ar = true;
                }
            }
            if (new_title_ar && questionTitle_ar.Length > 0)
            {
                lang.EntityName = "PROJECTFIELD";
                lang.FieldName = "TITLE";
                lang.KeyValue = pf.ID.ToString();
                lang.Language = "ar-SA";
                lang.Text = questionTitle_ar;
                db.Translation.Add(lang);
                db.SaveChanges();
            }

            bool new_instructions_ar = (fid == 0);
            if (fid > 0)
            {
                var query = db.Translation.SingleOrDefault(t => t.EntityName == "PROJECTFIELD" && t.FieldName == "INSTRUCTIONS" && t.KeyValue == pf.ID.ToString() && t.Language == "ar-SA");
                if (query != null)
                {
                    if (instructions_ar.Length == 0)
                    {
                        db.Translation.Attach(query);                       //Only generate DELETE statement without Quering
                        db.Translation.Remove(query);
                    }
                    else
                    {
                        query.Text = instructions_ar;
                        db.Translation.Attach(query);                       //Only generate an UPDATE statement without Quering
                        var entry = db.Entry(query);
                        entry.Property(e => e.Text).IsModified = true;
                    }
                    db.SaveChanges();
                }
                else
                {
                    new_instructions_ar = true;
                }
            }
            if (new_instructions_ar && instructions_ar.Length > 0)
            {
                lang.EntityName = "PROJECTFIELD";
                lang.FieldName = "INSTRUCTIONS";
                lang.KeyValue = pf.ID.ToString();
                lang.Language = "ar-SA";
                lang.Text = instructions_ar;
                db.Translation.Add(lang);
                db.SaveChanges();
            }

            //First delete all existing records <-- DON'T DO THIS, FIND AND UPDATE RECORDS INSTEAD
            //var queryPFS = db.Database.ExecuteSqlCommand("DELETE FROM ProjectFieldSample WHERE FieldId = " + pf.ID.ToString());

            //Then add the records found in the pfsList collection
            // *
            // * ParentSampleID with NULL value are Options (questions that are displayed on the left side in a grid)
            // *
            // * ParentSampleID with 0 value are Questions (questions that are displayed on the top side in a grid)
            // *
            ProjectFieldSample existingPFS;
            ProjectFieldSample existingPFSQ;
            if (fid > 0)
            {
                List<DNA_CAPI_MIS.Models.ProjectFieldSample> ProjectFieldSamples = db.ProjectFieldSample.Where(x => x.FieldID.Equals(fid) && x.ParentSampleID != 0 && x.IsActive).ToList<ProjectFieldSample>();
                List<DNA_CAPI_MIS.Models.ProjectFieldSample> PFSToDelete = new List<ProjectFieldSample>();
                if (ProjectFieldSamples != null && ProjectFieldSamples.Count > 0)
                {
                    foreach (ProjectFieldSample pfs in ProjectFieldSamples)
                    {
                        existingPFS = pfsList.Find(x => x.ID == pfs.ID);
                        if (existingPFS == null)
                        {
                            PFSToDelete.Add(pfs);
                        }
                        else
                        {
                            string title = Regex.Replace(existingPFS.Title, @"\t|\n|\r", "");
                            pfs.Code = existingPFS.Code;
                            pfs.VariableName = existingPFS.VariableName;
                            pfs.DisplayOrder = existingPFS.DisplayOrder;
                            pfs.Title = title;
                            pfsList.Remove(existingPFS);        //So that we know which recs are updated
                        }
                    }

                    if (PFSToDelete.Count > 0)
                    {
                        foreach (ProjectFieldSample pfs in PFSToDelete)
                        {
                            db.ProjectFieldSample.Remove(pfs);
                        }
                    }
                    db.SaveChanges();
                }
                List<DNA_CAPI_MIS.Models.ProjectFieldSample> ProjectFieldSamplesQ = db.ProjectFieldSample.Where(x => x.FieldID.Equals(fid) && x.ParentSampleID == 0 && x.IsActive).ToList<ProjectFieldSample>();
                List<DNA_CAPI_MIS.Models.ProjectFieldSample> PFSToDeleteQ = new List<ProjectFieldSample>();
                if (ProjectFieldSamplesQ != null && ProjectFieldSamplesQ.Count > 0)
                {
                    foreach (ProjectFieldSample pfs in ProjectFieldSamplesQ)
                    {
                        existingPFSQ = pfsListQ.Find(x => x.ID == pfs.ID);
                        if (existingPFSQ == null)
                        {
                            PFSToDeleteQ.Add(pfs);
                        }
                        else
                        {
                            string title = Regex.Replace(existingPFSQ.Title, @"\t|\n|\r", "");
                            pfs.Code = existingPFSQ.Code;
                            pfs.VariableName = existingPFSQ.VariableName;
                            pfs.DisplayOrder = existingPFSQ.DisplayOrder;
                            pfs.Title = title;
                            pfsListQ.Remove(existingPFSQ);        //So that we know which recs are updated
                        }
                    }

                    if (PFSToDeleteQ.Count > 0)
                    {
                        foreach (ProjectFieldSample pfs in PFSToDeleteQ)
                        {
                            db.ProjectFieldSample.Remove(pfs);
                        }
                    }
                    db.SaveChanges();
                }
            }
            if (pfsList != null && pfsList.Count > 0)
            {
                //Now these items are not in database and we just need to add them
                foreach (ProjectFieldSample pfs in pfsList)
                {
                    pfs.FieldID = pf.ID;
                    pfs.IsActive = true;
                    pfs.ParentSampleID = null;          //This will indicate that this is a top level Option/Sample
                    db.ProjectFieldSample.Add(pfs);
                }
                db.SaveChanges();
            }
            if (pfsListQ != null && pfsListQ.Count > 0)
            {
                //Now these items are not in database and we just need to add them
                foreach (ProjectFieldSample pfs in pfsListQ)
                {
                    pfs.FieldID = pf.ID;
                    pfs.IsActive = true;
                    pfs.ParentSampleID = 0;             //This will indicate that this is an Option breakup (Question)
                    db.ProjectFieldSample.Add(pfs);
                }
                db.SaveChanges();
            }


            //Save ProjectFieldSample translations
            if (pfsAllFields != null && pfsAllFields.Count > 0)
            {
                string title_lang;
                string keyId = "";

                //Now these items are not in database and we just need to add them
                foreach (var t in pfsAllFields)
                {
                    foreach (var item in t)
                    {
                        if (item.Key.Contains("Title_") && item.Value.Length > 0)
                        {
                            title_lang = item.Key.Substring(item.Key.IndexOf('_') + 1);
                            keyId = t["ID"];
                            if (Convert.ToInt32(keyId) == 0)
                            {
                                string x1 = t["VariableName"];
                                string x2 = t["Code"];
                                existingPFS = db.ProjectFieldSample.Where(x => x.FieldID == fid && x.VariableName.Equals(x1) && x.Code.Equals(x2) && x.ParentSampleID != 0 && x.IsActive).FirstOrDefault();
                                if (existingPFS != null)
                                {
                                    keyId = existingPFS.ID.ToString();
                                }
                                else
                                {
                                    continue;       //Nothing else we can do here
                                }
                            }
                            DNA_CAPI_MIS.Models.Translation trans = db.Translation.Where(
                                x => x.EntityName == "PROJECTFIELDSAMPLE" && x.FieldName == "TITLE"
                                && x.KeyValue == keyId && x.Language == title_lang).FirstOrDefault();
                            if (trans == null)
                            {
                                string title = Regex.Replace(item.Value, @"\t|\n|\r", "");

                                trans = new Translation();
                                trans.EntityName = "PROJECTFIELDSAMPLE";
                                trans.FieldName = "TITLE";
                                trans.KeyValue = keyId;
                                trans.Language = title_lang;
                                trans.Text = title;
                                db.Translation.Add(trans);
                            }
                            else
                            {
                                trans.Text = item.Value;
                            }
                        }
                    }
                }
                if (keyId.Length > 0)
                {
                    db.SaveChanges();
                }
            }
            if (pfsAllFieldsQ != null && pfsAllFieldsQ.Count > 0)
            {
                string title_lang;
                string keyId = "";

                //Now these items are not in database and we just need to add them
                foreach (var t in pfsAllFieldsQ)
                {
                    foreach (var item in t)
                    {
                        if (item.Key.Contains("Title_") && item.Value.Length > 0)
                        {
                            title_lang = item.Key.Substring(item.Key.IndexOf('_') + 1);
                            keyId = t["ID"];
                            if (Convert.ToInt32(keyId) == 0)
                            {
                                string x1 = t["VariableName"];
                                string x2 = t["Code"];
                                existingPFSQ = db.ProjectFieldSample.Where(x => x.FieldID == fid && x.VariableName.Equals(x1) && x.Code.Equals(x2) && x.ParentSampleID == 0 && x.IsActive).FirstOrDefault();
                                if (existingPFSQ != null)
                                {
                                    keyId = existingPFSQ.ID.ToString();
                                }
                                else
                                {
                                    continue;       //Nothing else we can do here
                                }
                            }
                            DNA_CAPI_MIS.Models.Translation trans = db.Translation.Where(
                                x => x.EntityName == "PROJECTFIELDSAMPLE" && x.FieldName == "TITLE"
                                && x.KeyValue == keyId && x.Language == title_lang).FirstOrDefault();
                            if (trans == null)
                            {
                                string title = Regex.Replace(item.Value, @"\t|\n|\r", "");

                                trans = new Translation();
                                trans.EntityName = "PROJECTFIELDSAMPLE";
                                trans.FieldName = "TITLE";
                                trans.KeyValue = keyId;
                                trans.Language = title_lang;
                                trans.Text = title;
                                db.Translation.Add(trans);
                            }
                            else
                            {
                                trans.Text = item.Value;
                            }
                        }
                    }
                }
                if (keyId.Length > 0)
                {
                    db.SaveChanges();
                }
            }

            //Get Project Id and Name
            string sql = "SELECT Id, Name FROM Project WHERE Id = " + pid;
            var project = db.Database.SqlQuery<ProjectView>(sql);
            ViewBag.ProjectName = project.FirstOrDefault<ProjectView>().Name;
            ViewBag.ProjectId = pid;

            //Get Project Field Sections
            List<ProjectFieldSection> ProjectFieldSection = db.ProjectFieldSection
                .Where(x => x.ProjectID.Equals(pid))
                .OrderBy(x => x.DisplayOrder)
                .ToList<ProjectFieldSection>();
            if (ProjectFieldSection.Count == 0)
            {
                ProjectFieldSection.Add(new ProjectFieldSection { ProjectID = pid, ID = 0, Name = "Questionnaire" });
            }
            ViewBag.ProjectFieldSection = ProjectFieldSection;
            // Don't send this, so that view shows new form
            //ViewBag.FieldId = pf.ID;
            var fieldType = pf.FieldType;
            pf = new ProjectField();
            pf.FieldType = fieldType;
            pf.IsMandatory = true;
            pf.IsVisible = true;
            ViewBag.ProjectField = pf;
            ViewBag.ProjectFieldSample = new List<DNA_CAPI_MIS.Models.ProjectFieldSample>();
            ViewBag.ProjectFieldSampleQ = new List<DNA_CAPI_MIS.Models.ProjectFieldSample>();

            return View("PQSettings", GetProjectFieldsWithValues((int)pid));
        }

        [Authorize]
        [HttpPost]
        public ActionResult PQSettingsUploadMedia(int ProjectID, int ProjectFieldID, string PFMediaType, HttpPostedFileBase PFMediaFile)
        {
            ProjectField pf = db.ProjectField.Find(ProjectFieldID);
            ProjectFieldMediaFile pfMediaFile = db.ProjectFieldMediaFile
                                                .Where(x => x.FieldID == ProjectFieldID)
                                                .FirstOrDefault<ProjectFieldMediaFile>();

            if (pfMediaFile == null)
            {
                pfMediaFile = new ProjectFieldMediaFile();
                pfMediaFile.FieldID = ProjectFieldID;
                pfMediaFile.FileCode = "Default";
                db.ProjectFieldMediaFile.Add(pfMediaFile);
            }
            pfMediaFile.FileType = PFMediaType;

            if (PFMediaFile != null && PFMediaFile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(PFMediaFile.FileName);
                fileName = '_' + ProjectFieldID + "_" + fileName;
                var path = Path.Combine(Server.MapPath("~/Pictures/SurveyMedia"), fileName);
                pfMediaFile.FileName = fileName;
                PFMediaFile.SaveAs(path);
            }

            db.SaveChanges();

            return View("~/Views/Shared/Blank.cshtml");
        }


        [Authorize]
        [HttpGet]
        public ActionResult PQSettings(int? id, int? fieldId, string fieldType)
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

                    //
                    if (fieldId != null && fieldId > 0)
                    {
                        int fid = (int)fieldId;
                        ViewBag.FieldId = fid;

                        //Get fields of ProjectField 
                        DNA_CAPI_MIS.Models.ProjectField pf = db.ProjectField.Find(fid);
                        ViewBag.ProjectField = pf;

                        if (pf.OptionsJSON != null && pf.OptionsJSON.Length > 0)
                        {
                            //DeSerialize JSON into Dictionary object
                            var pfOptionJSON = new Dictionary<string, Object>();
                            var optDeSerializer = new JsonFx.Json.JsonReader();
                            dynamic output = optDeSerializer.Read(pf.OptionsJSON);
                            if (output != null)
                            {
                                PopulateAttributeList(pfOptionJSON, output);
                                ViewBag.OptionsJSON = pfOptionJSON;
                            }
                        }

                        //Get records of ProjectFieldSample Options
                        List<DNA_CAPI_MIS.Models.ProjectFieldSample> ProjectFieldSamples = db.ProjectFieldSample
                            .Where(x => x.ParentSampleID != 0 && x.IsActive && x.FieldID.Equals(fid))
                            .OrderBy(x => x.DisplayOrder)
                            .ToList<ProjectFieldSample>();
                        ViewBag.ProjectFieldSample = ProjectFieldSamples;

                        //Get records of ProjectFieldSample Questions
                        List<DNA_CAPI_MIS.Models.ProjectFieldSample> ProjectFieldSamplesQ = db.ProjectFieldSample
                            .Where(x => x.ParentSampleID == 0 && x.IsActive && x.FieldID.Equals(fid))
                            .OrderBy(x => x.DisplayOrder)
                            .ToList<ProjectFieldSample>();
                        ViewBag.ProjectFieldSampleQ = ProjectFieldSamplesQ;

                        sql = string.Format(@"SELECT t.* FROM Translation t INNER JOIN ProjectFieldSample pfs 
                        ON pfs.ID = t.KeyValue AND t.EntityName = 'PROJECTFIELDSAMPLE' AND t.FieldName = 'TITLE' AND pfs.FieldId = {0}", fid);

                        List<DNA_CAPI_MIS.Models.Translation> SampleTitleTranslations = db.Database.SqlQuery<Translation>(sql).ToList<Translation>();
                        ViewBag.SampleTitleTranslations = SampleTitleTranslations;

                        ViewBag.questionTitle_ar = "";
                        var query = db.Translation.SingleOrDefault(t => t.EntityName == "PROJECTFIELD" && t.FieldName == "TITLE" && t.KeyValue == pf.ID.ToString() && t.Language == "ar-SA");
                        if (query != null)
                        {
                            ViewBag.questionTitle_ar = query.Text;
                        }
                        ViewBag.instructions_ar = "";
                        var query2 = db.Translation.SingleOrDefault(t => t.EntityName == "PROJECTFIELD" && t.FieldName == "INSTRUCTIONS" && t.KeyValue == pf.ID.ToString() && t.Language == "ar-SA");
                        if (query2 != null)
                        {
                            ViewBag.instructions_ar = query2.Text;
                        }

                        ProjectFieldMediaFile pfMediaFile = db.ProjectFieldMediaFile
                            .Where(x => x.FieldID == fid)
                            .FirstOrDefault<ProjectFieldMediaFile>();
                        if (pfMediaFile != null)
                        {
                            ViewBag.MediaFile = "/Pictures/SurveyMedia/" + pfMediaFile.FileName;
                            ViewBag.MediaFileType = pfMediaFile.FileType;
                        }

                    }
                    else
                    {
                        ProjectField pf = new ProjectField();
                        pf.FieldType = fieldType;
                        pf.IsMandatory = true;
                        pf.IsVisible = true;
                        ViewBag.ProjectField = pf;
                        ViewBag.ProjectFieldSample = new List<DNA_CAPI_MIS.Models.ProjectFieldSample>();
                        ViewBag.ProjectFieldSampleQ = new List<DNA_CAPI_MIS.Models.ProjectFieldSample>();
                    }
                    return View("PQSettings", GetProjectFieldsWithValues((int)id));
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
        [HttpGet]
        public ActionResult PQAnswer(int? id, int? count, string varName)
        {
            if (id > 0)
            {
                ProjectFieldSample pfs = db.ProjectFieldSample.Find(id);
                ProjectField pf = db.ProjectField.Find(pfs.FieldID);
                
                ProjectFieldSampleMediaFile pfsMediaFile = db.ProjectFieldSampleMediaFile
                                            .Where(x => x.FieldSampleID == pfs.ID)
                                            .FirstOrDefault<ProjectFieldSampleMediaFile>();

                List<ProjectField> otherPFs = db.ProjectField
                                            .Where(x => (x.FieldType.Equals("SLT") || x.FieldType.Equals("NUM")) && x.IsActive && x.ProjectID == pf.ProjectID)
                                            .OrderBy(o => o.DisplayOrder)
                                            .ToList<ProjectField>();

                if (pfs.OptionsJSON != null && pfs.OptionsJSON.Length > 0)
                {
                    //DeSerialize JSON into Dictionary object
                    var pfsOptionJSON = new Dictionary<string, Object>();
                    var optDeSerializer = new JsonFx.Json.JsonReader();
                    dynamic output = optDeSerializer.Read(pfs.OptionsJSON);
                    if (output != null)
                    {
                        PopulateAttributeList(pfsOptionJSON, output);
                        ViewBag.OptionsJSON = pfsOptionJSON;
                    }
                }

                if (pfsMediaFile != null)
                {
                    ViewBag.MediaFile = "/Pictures/SurveyMedia/" + pfsMediaFile.FileName;
                    ViewBag.MediaFileType = pfsMediaFile.FileType;
                }
                
                ViewBag.ProjectFieldName = pf.Title;
                ViewBag.ProjectFieldId = pf.ID;
                ViewBag.ProjectFieldSampleID = pfs.ID;
                ViewBag.OtherProjectFields = otherPFs;

                return View(pfs);
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult PQAnswer(int ProjectFieldID, int ProjectFieldSampleID, int? PFSDisplayOrder,
            string PFSCode, string PFSVariableName, string PFSType, string PFSTypeOther, string PFSMediaType,
            string PFSTitle, string PFSTitle_ar, HttpPostedFileBase PFSMediaFile)
        {
            ProjectFieldSample pfs = (ProjectFieldSampleID > 0 ? db.ProjectFieldSample.Find(ProjectFieldSampleID) : null);
            ProjectFieldSampleMediaFile pfsMediaFile = db.ProjectFieldSampleMediaFile
                                                        .Where(x => x.FieldSampleID == ProjectFieldSampleID)
                                                        .FirstOrDefault<ProjectFieldSampleMediaFile>();

            if (pfs == null)
            {
                pfs = new ProjectFieldSample();
                pfs.FieldID = ProjectFieldID;
                db.ProjectFieldSample.Add(pfs);
            }
            pfs.Code = PFSCode;
            pfs.VariableName = PFSVariableName;
            pfs.DisplayOrder = PFSDisplayOrder;
            pfs.Title = PFSTitle;

            var pfsOptionJSON = new Dictionary<string, Object>();
            if (pfs.OptionsJSON != null && pfs.OptionsJSON.Length > 0)
            {
                //DeSerialize JSON into Dictionary object
                var optDeSerializer = new JsonFx.Json.JsonReader();
                dynamic output = optDeSerializer.Read(pfs.OptionsJSON);
                if (output != null)
                {
                    PopulateAttributeList(pfsOptionJSON, output);
                }
            }

            if (PFSType != "0")
            {
                if (pfsOptionJSON.ContainsKey("OptionType"))
                {
                    pfsOptionJSON["OptionType"] = PFSType;
                }
                else
                {
                    pfsOptionJSON.Add("OptionType", PFSType);
                }
            }
            if (PFSType == "O")
            {
                if (pfsOptionJSON.ContainsKey("OptionTypeOtherPF"))
                {
                    pfsOptionJSON["OptionTypeOtherPF"] = PFSTypeOther;
                }
                else
                {
                    pfsOptionJSON.Add("OptionTypeOtherPF", PFSTypeOther);
                }
            }
            else
            {
                if (pfsOptionJSON.ContainsKey("OptionTypeOtherPF"))
                {
                    pfsOptionJSON.Remove("OptionTypeOtherPF");
                }
            }
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            pfs.OptionsJSON = serializer.Serialize(pfsOptionJSON);

            db.SaveChanges();
            ProjectFieldSampleID = pfs.ID;

            // ProjectFieldSampleMediaFile
            if (pfsMediaFile == null)
            {
                pfsMediaFile = new ProjectFieldSampleMediaFile();
                pfsMediaFile.FieldSampleID = ProjectFieldSampleID;
                pfsMediaFile.FileCode = "Default";
                db.ProjectFieldSampleMediaFile.Add(pfsMediaFile);
            }
            pfsMediaFile.FileType = PFSMediaType;

            if (PFSMediaFile != null && PFSMediaFile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(PFSMediaFile.FileName);
                fileName = "_" + ProjectFieldID + '_' + ProjectFieldSampleID + "_" + fileName;
                var path = Path.Combine(Server.MapPath("~/Pictures/SurveyMedia"), fileName);
                pfsMediaFile.FileName = fileName;
                PFSMediaFile.SaveAs(path);
            }
            
            db.SaveChanges();

            return View("~/Views/Shared/Blank.cshtml");
        }

        [Authorize]
        [HttpGet]
        public ActionResult MobilePreview(int? id, int? fieldId)
        {
            if (id > 0)
            {
                string sql = "SELECT Id, Name FROM Project WHERE Id = " + id;
                var project = db.Database.SqlQuery<ProjectView>(sql);
                ViewBag.ProjectName = project.FirstOrDefault<ProjectView>().Name;
                ViewBag.ProjectId = id;
                
                return View(GetProjectFieldsWithValues((int)id));
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult CATI(int? id)
        {
            if (id > 0)
            {
                string sql = "SELECT Id, Name FROM Project WHERE Id = " + id;
                var project = db.Database.SqlQuery<ProjectView>(sql);
                ViewBag.ProjectName = project.FirstOrDefault<ProjectView>().Name;
                ViewBag.ProjectId = id;

                return View(GetProjectFieldsWithValues((int)id));
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult Tabulation(FormCollection form)
        {            
            return View();
        }


        protected void GenerateSQL(int SideIndex, int TopIndex, AnalyzerQueryDimension d, ref DimensionSQL sql) 
        {
            if (d.FieldType == "RDO" || d.FieldType == "CHK" || d.FieldType == "DDN" || d.FieldType == "LVW")
            {
                sql.qfields += (sql.qfields.Length > 0 ? ", " : "") + (d.Position == "Side" ? SideIndex + " AS SideIndex, " + TopIndex + " AS TopIndex, " : "") + 
                    @"CASE WHEN <pos>SampleSourceParent.ID IS NULL THEN 0 ELSE <pos>SampleSourceParent.ID END AS <pos>GroupID, 
                    N'<ProjectFieldTitle>' + CASE WHEN <pos>SampleSourceParent.ID IS NULL THEN '' ELSE ' > ' + <pos>SampleSourceParent.Title END AS <pos>GroupTitle, 
                    <pos>SampleSource.Id AS <pos>Id, <pos>SampleSource.Title AS <pos>Title ";
                sql.qsrcjoin += "INNER JOIN SurveyData <pos>SurveyLink ON SurveySource.sbjnum = <pos>SurveyLink.sbjnum ";
                sql.qsmpjoin += "INNER JOIN ProjectFieldSample <pos>SampleSource ON <pos>SurveyLink.FieldId = <pos>SampleSource.FieldId AND (<pos>SampleSource.ParentSampleId <> 0 OR <pos>SampleSource.ParentSampleId IS NULL) AND <pos>SampleSource.IsActive = 1 ";
                sql.qsmpjoin += "LEFT OUTER JOIN ProjectFieldSample <pos>SampleSourceParent ON <pos>SampleSource.ParentSampleID = <pos>SampleSourceParent.ID AND <pos>SampleSourceParent.IsActive = 1 ";
                sql.qsmpjoin += "LEFT OUTER JOIN ProjectField <pos>SampleSourceRoot ON <pos>SampleSource.FieldID = <pos>SampleSourceRoot.ID ";

                if (d.FieldType == "RDO")
                {
                    sql.qfilter += (sql.qfilter.Length > 0 ? "AND " : "") + "<pos>SurveyLink.FieldId = <ProjectFieldID> AND <pos>SurveyLink.FieldValue = ''+<pos>SampleSource.Coding+'' ";
                }
                else if (d.FieldType == "LVW" || d.FieldType == "CHK")
                {
                    sql.qfilter += (sql.qfilter.Length > 0 ? "AND " : "") + "<pos>SurveyLink.FieldId = <ProjectFieldID> AND <pos>SampleSource.Coding IN (SELECT ListMember FROM fnSplitCSV(<pos>SurveyLink.FieldValue)) ";
                }
                sql.qorderby += (sql.qorderby.Length > 0 ? ", " : "") + "<pos>SampleSource.Title ";
                sql.qgroupby += (sql.qgroupby.Length > 0 ? ", " : "") + "<pos>SampleSourceRoot.ID, <pos>SampleSourceParent.ID, <pos>SampleSourceRoot.Title, <pos>SampleSourceParent.Title, <pos>SampleSource.Id, <pos>SampleSource.Title ";
            }
            else if (d.FieldType == "CTY")
            {
                sql.qfields += (sql.qfields.Length > 0 ? ", " : "") + (d.Position == "Side" ? SideIndex + " AS SideIndex, " + TopIndex + " AS TopIndex, " : "") +
                    @"0 AS <pos>GroupID, N'<ProjectFieldTitle>' AS <pos>GroupTitle,       
                    <pos>SampleSource.Id AS <pos>Id, <pos>SampleSource.Name AS <pos>Title ";
                sql.qsrcjoin += "";
                sql.qsmpjoin += "INNER JOIN City <pos>SampleSource ON SurveySource.CityID = <pos>SampleSource.ID ";
                //if (d.JoinTableValueField.Contains(',') && d.JoinTableValueField.Contains('(') && d.JoinTableValueField.Contains(')'))
                //{
                //    sql.qfilter += (sql.qfilter.Length > 0 ? "AND " : "") + "SurveySource.CityID IN <JoinValue>";
                //    fieldsTree = "AND SampleSource.Id IN " + d.JoinTableValueField;
                //}
                //else
                //{
                //    sql.qfilter += (sql.qfilter.Length > 0 ? "AND " : "") + "SurveySource.CityID = <JoinValue>";
                //    fieldsTree = "";
                //}
                sql.qfilter += (sql.qfilter.Length > 0 ? "AND " : "") + "SurveySource.CityID IN (2,6,7,10,11,12,13,14) ";

                sql.qorderby += (sql.qorderby.Length > 0 ? ", " : "") + "<pos>SampleSource.Name ";
                sql.qgroupby += (sql.qgroupby.Length > 0 ? ", " : "") + "<pos>SampleSource.Id, <pos>SampleSource.Name ";


            }
        }

        protected void ReplacePlaceholders(AnalyzerQueryDimension d, ref DimensionSQL sql) 
        {
            sql.qfields = sql.qfields.Replace("<pos>", d.Position);
            sql.qfields = sql.qfields.Replace("<ProjectFieldTitle>", d.ProjectFieldTitle);
            sql.qsrcjoin = sql.qsrcjoin.Replace("<pos>", d.Position);
            sql.qsrcjoin = sql.qsrcjoin.Replace("<ProjectFieldID>", d.ProjectFieldID.ToString());
            sql.qsmpjoin = sql.qsmpjoin.Replace("<pos>", d.Position);
            sql.qsmpjoin = sql.qsmpjoin.Replace("<ProjectFieldID>", d.ProjectFieldID.ToString());
            sql.qfilter = sql.qfilter.Replace("<pos>", d.Position);
            sql.qfilter = sql.qfilter.Replace("<ProjectFieldID>", d.ProjectFieldID.ToString());
            sql.qorderby = sql.qorderby.Replace("<pos>", d.Position);
            sql.qgroupby = sql.qgroupby.Replace("<pos>", d.Position);
        }


        private void PopulateDimension(ref List<AnalyzerQueryDimension> rdx, List<QueryDesignerFields> qf, List<ProjectField> projectFields, string pos)
        {
            int i = 0;
            foreach (QueryDesignerFields item in qf)
            {
                ProjectField pf = projectFields.Find(x => x.ID.Equals(item.id));

                AnalyzerQueryDimension aqd = new AnalyzerQueryDimension();
                aqd.QueryID = 0;
                aqd.DisplayOrder = i++;
                aqd.ProjectFieldID = item.id;
                aqd.ProjectFieldTitle = pf.Title;
                aqd.HasChild = item.children.Count() > 0;
                aqd.ParentPFID = pf.ParentFieldID;
                aqd.Position = pos;
                aqd.FieldType = pf.FieldType;

                rdx.Add(aqd);
            }
        }


        [Authorize]
        public ActionResult GraphType(int? id)
        {
            return View();
        }

        [Authorize]
        public ActionResult CostCalculator()
        {
            return View();
        }
        [Authorize]
        public ActionResult CostCalculatorQL()
        {
            return View();
        }

        public List<ProjectField> GetProjectFieldsWithValues(int SurveyId)
        {
            string sql = @"SELECT pf.Id PF_Id, pf.Title PF_Title, pf.ReportTitle PF_ReportTitle, pf.FieldType PF_FieldType, pf.IsMandatory PF_Mandatory, pf.DisplayOrder PF_DisplayOrder, pf.SectionID AS PF_SectionID,
                pfs.Id PFS_Id, pfs.ParentSampleID PFS_ParentSampleID, pfs.FieldId PFS_FieldId, pfs.Title PFS_Title, pfs.VariableName PFS_VariableName, pfs.DisplayOrder PFS_DisplayOrder, pfs.Code PFS_Code,
                ISNULL(pfn.Id, 0) SectionId, pfn.Name SectionName
                FROM ProjectField pf LEFT OUTER JOIN ProjectFieldSample pfs ON pf.Id = pfs.FieldId AND (pfs.ParentSampleID <> 0 OR pfs.ParentSampleID IS NULL) AND pfs.IsActive = 1
                LEFT OUTER JOIN ProjectFieldSection pfn ON pf.SectionId = pfn.id
                WHERE pf.IsActive = 1 AND pf.ProjectId = " + SurveyId +
                " ORDER BY ISNULL(pfn.DisplayOrder, 0), pf.DisplayOrder, pfs.FieldId";

            var query = db.Database.SqlQuery<ProjectFieldResultSet>(sql);

            List<ProjectField> pfList = new List<ProjectField>();
            ProjectField pf = new ProjectField();
            
            int fid = 0;
            foreach (ProjectFieldResultSet dr in query.ToList<ProjectFieldResultSet>())
            {
                if (fid != dr.PF_Id)
                {
                    if (fid != 0)
                    {
                        pfList.Add(pf);
                    }
                    fid = dr.PF_Id;
                    pf = new ProjectField();
                    pf.ID = fid;
                    pf.ReportTitle = dr.PF_ReportTitle;
                    pf.Title = dr.PF_Title;
                    pf.FieldType = dr.PF_FieldType;
                    pf.IsMandatory = dr.PF_IsMandatory;
                    pf.SectionID = dr.PF_SectionID;

                    if (dr.PFS_Id != null & dr.PFS_Id > 0)
                    {
                        //pf.ProjectFieldSamples = new List<ProjectFieldSample>();
                    }
                }
                //if (dr.PFS_Id != null & dr.PFS_Id > 0)
                //{
                //    pfs = new ProjectFieldSample();
                //    pfs.ID = (int)dr.PFS_Id;
                //    pfs.ParentSampleID = dr.PFS_ParentSampleID;
                //    pfs.FieldID = (int)dr.PFS_FieldId;
                //    pfs.Title = dr.PFS_Title;
                //    pfs.VariableName = dr.PFS_VariableName;
                //    pfs.DisplayOrder = dr.PFS_DisplayOrder;
                //    pfs.Code = dr.PFS_Code;
                //    pf.ProjectFieldSamples.Add(pfs);
                //}
            }
            if (fid != 0)
                pfList.Add(pf);

            return pfList;
        }

        internal void PopulateAttributeList(Dictionary<string, Object> list, dynamic jsonObject)
        {
            foreach (var opt in jsonObject)
            {
                if (opt.Value != null && opt.Value.GetType().Name == "ExpandoObject[]")
                {
                    List<Object> listP = new List<Object>();
                    foreach (var child in opt.Value)
                    {
                        if (opt.Value.GetType().Name == "ExpandoObject[]")
                        {
                            Dictionary<string, Object> listC = new Dictionary<string, Object>();
                            foreach (var childO in child)
                            {
                                listC.Add(childO.Key, childO.Value);
                            }
                            listP.Add(listC);
                        }
                        else if (opt.GetType().Name == "ExpandoObject")
                        {
                            //TODO: COMPLETE ITS IMPLEMENTATION
                            //PopulateAttributeList(listP, child.Value);
                        }
                    }
                    list.Add(opt.Key, listP);
                }
                else
                {
                    list.Add(opt.Key, opt.Value);
                }
            }
        }

        //public string GetProjectJSON(int id, string language)
        //{
        //    DSDS_WebAPI.Controllers.SurveyController survey = new DSDS_WebAPI.Controllers.SurveyController();
        //    string json = survey.GetProject(id, language);
        //    return json;
        //}
        //public string GetProjectSectionFieldsJSON(int id, string language)
        //{
        //    DSDS_WebAPI.Controllers.SurveyController survey = new DSDS_WebAPI.Controllers.SurveyController();
        //    string json = survey.GetProjectSectionFields(id, language);
        //    return json;
        //}
        //public string GetProjectFieldSamplesJSON(int id)
        //{
        //    DSDS_WebAPI.Controllers.ProjectController api = new DSDS_WebAPI.Controllers.ProjectController();
        //    string json = api.GetProjectFieldSamples(id.ToString());
        //    return json;
        //}
        //public string GetProjectFieldSamplesQJSON(int id)
        //{
        //    DSDS_WebAPI.Controllers.ProjectController api = new DSDS_WebAPI.Controllers.ProjectController();
        //    string json = api.GetProjectFieldSamplesQ(id.ToString());
        //    return json;
        //}
        //public string SendData(DSDS_WebAPI.Controllers.CategoryController.Survey survey)
        //{
        //    DSDS_WebAPI.Controllers.CategoryController controller = new DSDS_WebAPI.Controllers.CategoryController();
        //    string result = controller.SaveSurveyData(survey);
        //    string surveyId = "ERROR";
        //    if (result != null && result.Length > 0)
        //    {
        //        var settings = new JsonSerializerSettings();
        //        settings.TypeNameHandling = TypeNameHandling.Objects;
        //        settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
        //        survey = JsonConvert.DeserializeObject<DSDS_WebAPI.Controllers.CategoryController.Survey>(result, settings);
        //        surveyId = survey.sbjnum;
        //    }
        //    return surveyId;
        //}
    }
}


//--Query where link is between Location data and Questionnaire	
//select SideSampleSource.Name, TopSampleSource.Title, count(*) 
//FROM Survey SurveySource 
//  INNER JOIN SurveyData TopSurveyLink ON SurveySource.sbjnum = TopSurveyLink.sbjnum 
//  INNER JOIN City SideSampleSource ON SurveySource.CityID = SideSampleSource.ID
//  INNER JOIN ProjectFieldSample TopSampleSource ON TopSurveyLink.FieldId = TopSampleSource.FieldID
//WHERE SurveySource.ProjectID = 1371
//AND TopSurveyLink.FieldId = 1 AND TopSampleSource.ID IN (SELECT ListMember FROM fnSplitCSV(TopSurveyLink.FieldValue))
//AND SurveySource.CityID IN (2,6,7)
//GROUP BY TopSampleSource.Title, SideSampleSource.CountryID, SideSampleSource.Name
//ORDER BY TopSampleSource.Title, SideSampleSource.CountryID, SideSampleSource.Name

//--Query where link is between Questionnaire and Questionnaire	
//select SideSampleSource.Title, TopSampleSource.Title, count(*) 
//FROM Survey SurveySource 
//  INNER JOIN SurveyData SideSurveyLink ON SurveySource.sbjnum = SideSurveyLink.sbjnum 
//  INNER JOIN SurveyData TopSurveyLink ON SurveySource.sbjnum = TopSurveyLink.sbjnum 
//  INNER JOIN ProjectFieldSample SideSampleSource ON SideSurveyLink.FieldId = SideSampleSource.FieldID
//  INNER JOIN ProjectFieldSample TopSampleSource ON TopSurveyLink.FieldId = TopSampleSource.FieldID
//WHERE SurveySource.ProjectID = 1371
//AND SideSurveyLink.FieldId = 1 AND SideSampleSource.ID IN (SELECT ListMember FROM fnSplitCSV(SideSurveyLink.FieldValue))
//AND TopSurveyLink.FieldId = 43 AND TopSampleSource.Title = TopSurveyLink.FieldValue
//GROUP BY TopSampleSource.Title, SideSampleSource.Title
//ORDER BY TopSampleSource.Title, SideSampleSource.Title


