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
using DNA_CAPI_MIS.DAL;
using Microsoft.AspNet.Identity;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DNA_CAPI_MIS.Controllers
{
    public class ProjectQuotaController : Controller
    {
        private ProjectContext db = new ProjectContext();
        
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

        public class UpdateRowRequest
        {
            int rowid { get; set; }
            string rowdata { get; set; }
        }
        [HttpPost]
        public HttpStatusCodeResult UpdateRow(int? id, int? rowid, string rowdata)
        {
            if (id > 0)
            {
                var settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.Objects;
                settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                JObject dxData = JsonConvert.DeserializeObject<JObject>(rowdata, settings);

                //Check if a tabulation exists
                string sql = "";

                sql = @"SELECT * FROM AnalyzerQuery WHERE ProjectID = " + id;
                var query = db.Database.SqlQuery<AnalyzerQuery>(sql);
                var aqlist = query.ToList<AnalyzerQuery>();
                var aq = aqlist.FirstOrDefault<AnalyzerQuery>();
                if (aqlist.Count > 0)
                {
                    sql = "";
                    string SideGroupId = "";
                    string SideId = "";
                    string TopGroupId = "";
                    string TopId = "";

                    foreach (JProperty item in dxData.Properties())
                    {
                        if (item.Name == "SideGroupId") { SideGroupId = item.Value.ToString(); continue; }
                        if (item.Name == "SideId") { SideId = item.Value.ToString(); continue; }

                        if (item.Name.Substring(0, 1) == "_")
                        {
                            TopGroupId = item.Name.Substring(0, item.Name.IndexOf("__"));
                            TopId = item.Name.Substring(TopGroupId.Length + 2);

                            //We must include SideGroupId in TopGroupId to allow Multi-level Side Groups in future
                            string whereClause = " WHERE QueryID = " + aq.ID + //" AND SideGroupId = '" + SideGroupId + "' " +
                                " AND SideId = " + SideId + " AND TopGroupId = '" + TopGroupId + "' " +
                                " AND TopId = " + TopId + ";";
                            string subsql = @"SELECT * FROM AnalyzerQueryData " + whereClause;
                            var subquery = db.Database.SqlQuery<AnalyzerQueryData>(subsql);
                            var aqdlist = subquery.ToList<AnalyzerQueryData>();
                            if (aqdlist.Count > 0)
                            {
                                var aqd = aqdlist.FirstOrDefault<AnalyzerQueryData>();
                                if (aqd.AggregateValue.ToString() != item.Value.ToString())
                                {
                                    sql += "UPDATE AnalyzerQueryData SET AggregateValue = " + item.Value.ToString() + whereClause;
                                }
                            }
                            else
                            {
                                sql += string.Format(@"INSERT INTO AnalyzerQueryData (QueryID, 
                                        TopId, TopGroupId, TopIndex, TopDisplayOrder,
                                        SideId, SideGroupId, SideIndex, SideDisplayOrder,
                                        AggregateValue, GroupBaseCount) 
                                        VALUES ({0}, {1}, '{2}', 0, 0, {3}, '{4}', 0, 0, {5}, {6});",
                                        aq.ID, TopId, TopGroupId, SideId, SideGroupId, item.Value.ToString(), 0);
                            }

                        }
                    }
                    if (sql.Length > 0)
                    {
                        db.Database.ExecuteSqlCommand(sql);
                    }
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        // GET: /Home/OpenProject
        public ActionResult OpenProject()
        {
            string sql = "";

            sql = @"SELECT p.Id, p.Name FROM Project p INNER JOIN DNAShared2.dbo.UserRights ur ON ur.ObjectValue = p.ID AND ur.ObjectName = 'PROJECT'
                INNER JOIN DNAShared2.dbo.AspNetRoles r ON ur.RoleId = r.Id
                WHERE r.Name = 'CanEdit'";

            var query = db.Database.SqlQuery<ProjectsList>(sql);
            List<ProjectsList> projects = query.ToList<ProjectsList>();

            return View(projects);
        }

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
                ViewBag.MaxSurveysByASurveyor = stats.FirstOrDefault<SurveyorStats>().SurveyCount;

                return View(project);
            }
            else
            {
                return View();
            }
        }

        protected bool AddPFInQDF(QueryDesignerFields qf, int parentId, int pfid)
        {
            foreach (var q in qf.children)
            {
                if (q.id == parentId)
                {
                    q.children.Add(new QueryDesignerFields { id = pfid });
                    return true;
                }
                else if (q.children.Count > 0)
                {
                    if (AddPFInQDF(q, parentId, pfid))
                    {
                        break;
                    }
                }
            }
            return false;
        }

        [HttpGet]   
        public ActionResult Index(int? id)
        {
            if (id > 0)
            {
                string sql = "";
                sql = "SELECT Id, Name FROM Project WHERE Id = " + id;
                var project = db.Database.SqlQuery<ProjectView>(sql);
                if (project.FirstOrDefault<ProjectView>() != null)
                {

                    ViewBag.ProjectName = project.FirstOrDefault<ProjectView>().Name;
                    ViewBag.ProjectId = id;

                    List<ProjectField> pfields = GetProjectFields((int)id, true);

                    //Check if a tabulation exists
                    sql = @"SELECT * FROM AnalyzerQuery WHERE ProjectID = " + id;
                    var query = db.Database.SqlQuery<AnalyzerQuery>(sql);
                    var aqlist = query.ToList<AnalyzerQuery>();
                    var aq = aqlist.FirstOrDefault<AnalyzerQuery>();
                    if (aqlist.Count > 0)
                    {
                        sql = @"SELECT * FROM AnalyzerQueryDimension WHERE QueryID = " + aq.ID;
                        var aquery = db.Database.SqlQuery<AnalyzerQueryDimension>(sql);
                        var aqd = aquery.ToList<AnalyzerQueryDimension>();
                        ProjectField pf = null;
                        List<AnalyzerQueryDimension> dxTop = new List<AnalyzerQueryDimension>();
                        List<AnalyzerQueryDimension> dxSide = new List<AnalyzerQueryDimension>();
                        QueryDesignerFields qfTop = new QueryDesignerFields();
                        List<QueryDesignerFields> qfSide = new List<QueryDesignerFields>();
                        QueryDesignerFields qf;

                        foreach (var d in aqd)
                        {
                            pf = pfields.Find(x => x.ID.Equals(d.ProjectFieldID));
                            d.ProjectFieldTitle = (pf.ReportTitle.Length > 0 ? pf.ReportTitle : pf.Title);

                            if (d.Position == "Top")
                            {
                                dxTop.Add(d);
                                qf = qfTop;
                                if (qf.id == 0)
                                {
                                    qf.id = d.ProjectFieldID;
                                }
                                else
                                {
                                    if (qf.id == d.ParentPFID)
                                    {
                                        qf.children.Add(new QueryDesignerFields { id = d.ProjectFieldID });
                                    }
                                    else
                                    {
                                        if (d.ParentPFID != null)
                                        {
                                            AddPFInQDF(qf, (int)d.ParentPFID, d.ProjectFieldID);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                dxSide.Add(d);
                                qf = new QueryDesignerFields();
                                qf.id = d.ProjectFieldID;
                                qfSide.Add(qf);
                                if (qf.id == d.ParentPFID)
                                {
                                    qf.children.Add(new QueryDesignerFields { id = d.ProjectFieldID });
                                }
                                else
                                {
                                    if (d.ParentPFID != null)
                                    {
                                        AddPFInQDF(qf, (int)d.ParentPFID, d.ProjectFieldID);
                                    }
                                }
                            }
                        }

                        pf = pfields.Find(x => x.ID.Equals(aq.BaseProjectFieldID));

                        ViewBag.QueryID = aq.ID;
                        ViewBag.BasePrimaryFieldTitle = (pf.ReportTitle.Length > 0 ? pf.ReportTitle : pf.Title);
                        ViewBag.AQDTop = dxTop;
                        ViewBag.AQDSide = dxSide;

                        sql = @"SELECT * FROM AnalyzerQueryData WHERE QueryID = " + aq.ID;
                        var daquery = db.Database.SqlQuery<BasicSingleCrosstab>(sql);
                        var tabularData = daquery.ToList<BasicSingleCrosstab>();

                        //ViewBag.Tabulation = RenderRazorViewToString("Tabulation", tabularData);
                        string renderedView = BuildQuotaTabulation((int)id, new string[0], 0,
                                new QueryDesignerFields { id = qfTop.id },
                                new List<QueryDesignerFields> { qfTop }, qfSide, aq.ID);
                        ViewBag.Tabulation = renderedView;
                    }
                    else
                    {
                        ViewBag.QueryID = 0;
                    }

                    return View(pfields);
                }
            }
            return View();
        }

        
        [HttpPost]
        public string Index(FormCollection form)
        {
            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Objects;
            settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

            int pid = Convert.ToInt32(form["ProjectID"].ToString());
            string[] baseOfPrimaryField = form["primaryFieldSample"].ToString().Split(',');
            int totalBase = 0;
            if (baseOfPrimaryField.Length > 0)
            {
                for (int p = 0; p < baseOfPrimaryField.Length; p++)
                {
                    if (baseOfPrimaryField[p].Length > 0 && Convert.ToInt32(baseOfPrimaryField[p]) > 0)
                    {
                        totalBase += Convert.ToInt32(baseOfPrimaryField[p]);
                    }
                }
            }

            string fieldsTop = form["FieldsTop"].ToString();
            //List<QueryDesignerFields> qfTop = JsonConvert.DeserializeObject<List<QueryDesignerFields>>(fieldsTop, settings);
            List<QueryDesignerFields> qfTop = new List<QueryDesignerFields>();

            //Add the primary field at the top level in the 'Top' dimension.
            QueryDesignerFields qdfPrimaryField = new QueryDesignerFields();
            qdfPrimaryField.id = Convert.ToInt32(form["primaryField"].ToString());
            qdfPrimaryField.children = JsonConvert.DeserializeObject<List<QueryDesignerFields>>(fieldsTop, settings);
            qfTop.Add(qdfPrimaryField);

            string fieldsSide = form["FieldsSide"].ToString();
            List<QueryDesignerFields> qfSide = JsonConvert.DeserializeObject<List<QueryDesignerFields>>(fieldsSide, settings);

            return BuildQuotaTabulation(pid, baseOfPrimaryField, totalBase, qdfPrimaryField, qfTop, qfSide);
        }

        protected string BuildQuotaTabulation(int pid, string[] baseOfPrimaryField, int totalBase, QueryDesignerFields qdfPrimaryField,
                List<QueryDesignerFields> qfTop, List<QueryDesignerFields> qfSide, int QueryId = 0) 
        {

            //Generate Tabulation
            string sql = "";

            //sql = "SELECT aqd.* FROM AnalyzerQueryDimension aqd WHERE aqd.QueryID = 1 AND aqd.Position = 'Side' ORDER BY aqd.DisplayOrder";

            //var aQuery = db.Database.SqlQuery<AnalyzerQueryDimension>(sql);
            //List<AnalyzerQueryDimension> rdxSide = aQuery.ToList<AnalyzerQueryDimension>();

            //sql = "SELECT aqd.* FROM AnalyzerQueryDimension aqd WHERE aqd.QueryID = 1 AND aqd.Position = 'Top' ORDER BY aqd.DisplayOrder";

            sql = "SELECT Id, ParentFieldId, FieldType, Title, DisplayOrder FROM ProjectField WHERE ProjectID = " + pid;
            var pfQuery = db.Database.SqlQuery<ProjectFieldView>(sql);
            List<ProjectFieldView> projectFields = pfQuery.ToList<ProjectFieldView>();
            
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

//            //Dictionary<int, GroupInfo> TopGroups = new Dictionary<int, GroupInfo>();
//            foreach (AnalyzerQueryDimension d in rdxTop)
//            {
//                bool rootLevel = (d.ParentAQD == null);
//                //if (d.HasChild) { continue; }
//                if (d.FieldType == "RDO" || d.FieldType == "CHK" || d.FieldType == "DDN" || d.FieldType == "LVW")
//                {
//                    sql = @"SELECT pfs.Id, pfs.Title, '' AS GroupId, count(*) AS GroupBaseCount 
//                            FROM Survey s INNER JOIN SurveyData sd ON s.sbjnum = sd.sbjnum 
//                            INNER JOIN ProjectFieldSample pfs ON sd.FieldId = pfs.FieldId 
//                            LEFT OUTER JOIN ProjectFieldSample pfsParent ON pfs.ParentSampleID = pfsParent.ID 
//                            LEFT OUTER JOIN ProjectField pf ON pfs.FieldID = pf.ID 
//                            WHERE sd.FieldId = " + d.ProjectFieldID + @" AND (pfs.Code IN (SELECT ListMember FROM fnSplitCSV(sd.FieldValue)) OR sd.FieldValue = ''+pfs.Code+'')
//                            GROUP BY pf.ID, pfsParent.ID, pf.Title, pfsParent.Title, pfs.Id, pfs.Title";
//                }
//                else if (d.FieldType == "CTY")
//                {
//                    sql = @"SELECT pfs.Id, pfs.Name AS Title, count(*) AS GroupBaseCount 
//                            FROM Survey s INNER JOIN City pfs ON s.CityId = pfs.Id 
//                            WHERE pfs.Id IN (2,6,7,10,11,12,13,14) AND s.ProjectId = " + pid + " GROUP BY pfs.ID, pfs.Name ORDER BY pfs.Name";
//                }
//                var GroupTitles = db.Database.SqlQuery<SimpleListWithAggregates>(sql);
//                d.Items = GroupTitles.ToList<SimpleListWithAggregates>();

//            }

            //Add SIDE Row Groups with Base
            Dictionary<int, GroupInfo> SideGroups = new Dictionary<int, GroupInfo>();
            foreach (AnalyzerQueryDimension d in rdxSide)
            {
                bool rootLevel = (d.ParentAQD == null);
                //if (d.HasChild) { continue; }
                if (d.FieldType == "RDO" || d.FieldType == "CHK" || d.FieldType == "DDN" || d.FieldType == "LVW" || d.FieldType == "SCD" || d.FieldType == "SCG" || d.FieldType == "MCG" || d.FieldType == "MLT" || d.FieldType == "ONG")
                {
                    sql = @"SELECT Id, Title FROM ProjectField WHERE Id = " + d.ProjectFieldID;

                    var GroupTitles = db.Database.SqlQuery<SimpleListWithAggregates>(sql);
                    SideGroups.Add(SideGroups.Count, new GroupInfo { Id = d.ProjectFieldID, Title = d.ProjectFieldTitle, ParentId = d.QueryID, HasChild = d.HasChild, TopLevel = rootLevel, Field = "SideGroupTitle" });
                }
                else if (d.FieldType == "CTY")
                {
                    SideGroups.Add(SideGroups.Count, new GroupInfo { Id = d.ProjectFieldID, Title = d.ProjectFieldTitle, ParentId = d.QueryID, HasChild = d.HasChild, TopLevel = rootLevel, Field = "SideId" });
                }

                //Multiple groups not supported yet
                //break;
            }

            //-------------------------------------------------------------------------------------------------------
            //Generate SQL 
            List<AnalyzerQueryDimension> rdxRoot = new List<AnalyzerQueryDimension>();
            AnalyzerQueryDimension dRootNode = null;
            foreach (var root in rdxTop)
            {
                dRootNode = root;
                break;
            }
            rdxRoot.Add(dRootNode);

            //List of columns being created

            //Setup TopColumn groups for the Grid
            string jqxDefColumnGroup = GenerateJQXColumnGroupDef(rdxRoot);
            ViewBag.jqxDefColumnGroup = jqxDefColumnGroup;

            //Build 2 dimensional queries
            if (qfTop.First().children.Count == 0)
            {
                BuildTabulation(ref rdxTop, ref rdxRoot, ref rdxSide, ref TopIndex, ref SideIndex, ref colSQL, baseOfPrimaryField, "_0");
            }
            else
            {
                BuildTabulation(ref rdxTop, ref rdxRoot, ref rdxSide, ref TopIndex, ref SideIndex, ref colSQL, baseOfPrimaryField);
            }

            //Merge SQL of all groups
            string sqlTabularData = "";
            if (colSQL.Count == 1)
            {
                sqlTabularData = colSQL[0].mdsql;
            }
            else if (colSQL.Count > 1)
            {
                for (int q = 0; q < colSQL.Count; q++)
                {
                    sqlTabularData += colSQL[q].mdsql_wo_order;
                }
            }
            if (qfTop.First().children.Count > 0)
            {
                sqlTabularData += " ORDER BY 5, 3";
            }
            var singleCrosstab = db.Database.SqlQuery<BasicSingleCrosstab>(sqlTabularData);
            List<BasicSingleCrosstab> tabularData = singleCrosstab.ToList<BasicSingleCrosstab>();

            if (QueryId == 0)
            {
                //Zeroise all values.... temporary... here we should be generating the base distribution
                foreach (var item in tabularData)
                {
                    item.AggregateValue = 0;
                }

                
                
                //Save Tabular data
                AnalyzerQuery aq = new AnalyzerQuery();
                aq.ProjectID = pid;
                aq.Title = qdfPrimaryField.id.ToString() + ":" + totalBase;
                aq.BaseProjectFieldID = qdfPrimaryField.id;
                aq.CreatedOn = DateTime.Now;
                aq.CreatedBy = User.Identity.GetUserId();
                db.AnalyzerQuery.Add(aq);
                db.SaveChanges();
                if (aq.ID > 0)
                {
                    int i = 1;
                    foreach (var d in rdxTop)
                    {
                        AnalyzerQueryDimension aqd = new AnalyzerQueryDimension();
                        aqd.QueryID = aq.ID;
                        aqd.DisplayOrder = i++;
                        aqd.ProjectFieldID = d.ProjectFieldID;
                        aqd.HasChild = d.HasChild;
                        if (d.ParentAQD != null)
                            aqd.ParentPFID = d.ParentAQD.ProjectFieldID;
                        aqd.Position = "Top";
                        db.AnalyzerQueryDimension.Add(aqd);
                    }
                    foreach (var d in rdxSide)
                    {
                        AnalyzerQueryDimension aqd = new AnalyzerQueryDimension();
                        aqd.QueryID = aq.ID;
                        aqd.DisplayOrder = i++;
                        aqd.ProjectFieldID = d.ProjectFieldID;
                        aqd.HasChild = d.HasChild;
                        if (d.ParentAQD != null)
                            aqd.ParentPFID = d.ParentAQD.ProjectFieldID;
                        aqd.Position = "Side";
                        db.AnalyzerQueryDimension.Add(aqd);
                    }

                    foreach (var rec in tabularData)
                    {
                        AnalyzerQueryData aqd = new AnalyzerQueryData();
                        aqd.QueryID = aq.ID;
                        aqd.TopIndex = rec.TopIndex;
                        aqd.TopDisplayOrder = rec.TopDisplayOrder;
                        aqd.TopGroupId = rec.TopGroupId;
                        aqd.TopGroupTitle = rec.TopGroupTitle;
                        aqd.TopId = rec.TopId;
                        aqd.TopTitle = rec.TopTitle;
                        aqd.SideIndex = rec.SideIndex;
                        aqd.SideDisplayOrder = rec.SideDisplayOrder;
                        aqd.SideGroupId = rec.SideGroupId;
                        aqd.SideGroupTitle = rec.SideGroupTitle;
                        aqd.SideId = rec.SideId;
                        aqd.SideTitle = rec.SideTitle;
                        aqd.AggregateValue = rec.AggregateValue;
                        aqd.GroupBaseCount = rec.GroupBaseCount;
                        db.AnalyzerQueryData.Add(aqd);
                    }
                    db.SaveChanges();
                }
            }
            else
            {
                FillProjectQuotaTabulationWithExistingValues(QueryId, ref tabularData);
                FillProjectActualBase(pid, ref tabularData);
            }

            //baseOfPrimaryField
            //Get user defined base for the TOP Primary Field
            //sql = @"SELECT pfs.* FROM ProjectFieldSample pfs WHERE pfs.FieldID = " + qdfPrimaryField.id + " ORDER BY DisplayOrder";
            //var pfsQuery = db.Database.SqlQuery<ProjectFieldSample>(sql);
            //List<ProjectFieldSample> pfsData = pfsQuery.ToList<ProjectFieldSample>();
            //SideIndex = 0;
            //GroupInfo samples = TopGroups[0];

            //foreach (ProjectFieldSample pfs in pfsData)
            //{
            //    if (baseOfPrimaryField.Length > SideIndex)
            //    {
            //        foreach (BasicSingleCrosstab ctData in tabularData.Where(x => x.TopId == pfs.ID))
            //        {
            //            ctData.GroupBaseCount = Convert.ToInt32(baseOfPrimaryField[SideIndex]);
            //        }
            //        samples.Items.Where(x => x.Id == pfs.ID)
            //            .First<SimpleListWithAggregates>()
            //            .GroupBaseCount = Convert.ToInt32(baseOfPrimaryField[SideIndex]);
            //    }
            //    SideIndex++;
            //}


            ViewBag.ColumnGroup = rdxTop;            //Set value for view
            ViewBag.RowGroup = SideGroups;            //Set value for view
            ViewBag.ProjectId = pid;
            ViewBag.QueryId = QueryId;

            return RenderRazorViewToString("Tabulation", tabularData);
        }

        protected void FillProjectQuotaTabulationWithExistingValues(int QueryId, ref List<BasicSingleCrosstab> data)
        {
            string sql = @"SELECT * FROM AnalyzerQueryData WHERE QueryID = " + QueryId;
            var query = db.Database.SqlQuery<AnalyzerQueryData>(sql);
            var aqlist = query.ToList<AnalyzerQueryData>();
            
            foreach (var item in data)
            {
                try
                {
                    var xd = aqlist.Single(x => x.SideGroupId == item.SideGroupId && x.SideId == item.SideId && x.TopGroupId == item.TopGroupId && x.TopId == item.TopId);
                    if (xd.AggregateValue > 0)
                    {
                        Console.Write("Non zero value");
                    }
                    item.AggregateValue = xd.AggregateValue;
                }
                catch (Exception e)
                {
                }
            }
        }

        protected void FillProjectActualBase(int ProjectId, ref List<BasicSingleCrosstab> data)
        {
            //In the database in SurveyData table we are saving ProjectField.ID in FieldId and ProjectFieldSample.Code in FieldValue for all non-openended questions
            //therefore, we need to find Ids for each Code
            string sql = "";

            //sql = "SELECT Id, ParentFieldId, FieldType, Title, ReportTitle, DisplayOrder, VariableName FROM ProjectField WHERE ProjectID = " + ProjectId;
            //var pfQuery = db.Database.SqlQuery<ProjectFieldView>(sql);
            //List<ProjectFieldView> projectFields = pfQuery.ToList<ProjectFieldView>();

            sql = @"SELECT pfs.Id, pfs.FieldId, pfs.Title, pfs.DisplayOrder, pfs.VariableName, pfs.Code 
                    FROM ProjectFieldSample pfs INNER JOIN ProjectField pf ON pf.ID = pfs.FieldID 
                        AND (pfs.ParentSampleID <> 0 OR pfs.ParentSampleID IS NULL) AND pfs.IsActive = 1 
                    WHERE pf.ProjectID = " + ProjectId;
            var pfsQuery = db.Database.SqlQuery<ProjectFieldSampleView>(sql);
            List<ProjectFieldSampleView> projectFieldSamples = pfsQuery.ToList<ProjectFieldSampleView>();
            ProjectFieldSampleView pfs = null;
            string[] allids;
            string filterTop = "", filterSide = "";
            foreach (var item in data)
            {
                filterTop = "";
                filterSide = "";
                allids = item.TopGroupId.Trim('_').Split('_');
                for (int i = 0; i < allids.Length; i+=2)
                {
                    if (allids[i].Trim().Length > 0)
                    {
                        if (i + 1 < allids.Length && allids[i + 1].Trim().Length > 0)
                        {
                            //ProjectFieldView pf = projectField.Find(x => x.ID.Equals(item.id));
                            if (Convert.ToInt32(allids[i]) == 0)
                            {
                                pfs = projectFieldSamples.Find(x => x.FieldID == Convert.ToInt32(allids[i + 1]) && x.ID == Convert.ToInt32(item.TopId));
                            }
                            else
                            {
                                pfs = projectFieldSamples.Find(x => x.FieldID == Convert.ToInt32(allids[i]) && x.ID == Convert.ToInt32(allids[i + 1]));
                            }
                        }
                        else if (i + 1 == allids.Length)
                        {
                            pfs = projectFieldSamples.Find(x => x.FieldID == Convert.ToInt32(allids[i]) && x.ID == Convert.ToInt32(item.TopId));
                        }
                        if (pfs != null && pfs.ID > 0)
                        {
                            filterTop += string.Format(" INNER JOIN SurveyData sdt{0} ON s.sbjnum = sdt{0}.sbjnum AND sdt{0}.FieldId = {1} AND ('{2}' IN (SELECT ListMember FROM fnSplitCSV(sdt{0}.FieldValue)) OR sdt{0}.FieldValue = '{2}')", i, pfs.FieldID, pfs.Code);
                        }
                    }
                }
                allids = item.SideGroupId.Trim('_').Split('_');
                for (int i = 0; i < allids.Length; i+=2)
                {
                    if (allids[i].Trim().Length > 0)
                    {
                        if (i + 1 < allids.Length && allids[i + 1].Trim().Length > 0)
                        {
                            if (Convert.ToInt32(allids[i]) == 0)
                            {
                                pfs = projectFieldSamples.Find(x => x.FieldID == Convert.ToInt32(allids[i + 1]) && x.ID == Convert.ToInt32(item.SideId));
                            }
                            else
                            {
                                pfs = projectFieldSamples.Find(x => x.FieldID == Convert.ToInt32(allids[i]) && x.ID == Convert.ToInt32(allids[i + 1]));
                            }
                        }
                        else if (i + 1 == allids.Length)
                        {
                            pfs = projectFieldSamples.Find(x => x.FieldID == Convert.ToInt32(allids[i]) && x.ID == Convert.ToInt32(item.SideId));
                        }
                        if (pfs != null && pfs.ID > 0)
                        {
                            filterSide += string.Format(" INNER JOIN SurveyData sds{0} ON s.sbjnum = sds{0}.sbjnum AND sds{0}.FieldId = {1} AND ('{2}' IN (SELECT ListMember FROM fnSplitCSV(sds{0}.FieldValue)) OR sds{0}.FieldValue = '{2}')", i, pfs.FieldID, pfs.Code);
                        }
                    }
                }               
     
                try
                {
                    if (filterTop.Length > 0 && filterSide.Length > 0)
                    {
                        //Now get aggregates from actual data
                        sql = string.Format(@"SELECT COUNT(*) FROM Survey s {0} {1} WHERE s.OpStatus = 1 AND s.QcStatus = 1 AND s.ProjectID = {2}", filterTop, filterSide, ProjectId);
                        var query = db.Database.SqlQuery<int>(sql);
                        item.ActualCount = query.FirstOrDefault<int>();
                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        protected string GenerateJQXColumnGroupDef(List<AnalyzerQueryDimension> dxTop, string parentFieldSampleId = "")
        {
            string cg = "";
            string parentId = "";
            foreach (var colg in dxTop)
            {
                if (parentFieldSampleId.Length > 0 || colg.ChildrenAQD != null)
                {
                    //Make ParentId
                    if (parentFieldSampleId.Length == 0)
                    {
                        if (colg.ParentAQD != null)
                        {
                            parentId = colg.ParentAQD.ProjectFieldID.ToString();
                        }
                    }
                    else
                    {
                        parentId = parentFieldSampleId;
                    }
                    if (parentId.Length > 0)
                    {
                        parentId = parentId.Trim('_') + "_";
                    }
                    parentId = "_" + parentId;

                    //Make column group definition
                    if (colg.ParentAQD == null)
                    {
                        cg += "{ text: urldecode('" + Url.Encode(colg.ProjectFieldTitle) + "'), align: 'center', name: '_" + colg.ProjectFieldID + "' },\n";
                    }
                    else
                    {
                        cg += "{ text: urldecode('" + Url.Encode(colg.ProjectFieldTitle) + "'), align: 'center', name: '" + parentId + colg.ProjectFieldID + "', parentgroup: '_" + parentId.Trim('_') + "' },\n";
                    }

                    if (colg.Items != null && colg.ChildrenAQD != null)     //If Has Sample Items and Have Child Dimensions
                    {
                        foreach (var gi in colg.Items)
                        {
                            cg += "{ text: urldecode('" + Url.Encode(gi.Title) + "'), align: 'center', name: '" + parentId + colg.ProjectFieldID + "_" + gi.Id + "', parentgroup: '" + parentId + colg.ProjectFieldID + "' },\n";
                            cg += GenerateJQXColumnGroupDef(colg.ChildrenAQD, parentId + colg.ProjectFieldID + "_" + gi.Id);
                        }
                    }
                }
            }

            return cg;
        }

        

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        public string RenderRazorViewToString(ViewResult viewResult)
        {
            //ViewData.Model = viewResult.Model;
            //TempData = viewResult.TempData;
            using (var sw = new StringWriter())
            {
                //var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                //viewResult.View.Render(viewContext, sw);
                foreach (var v in viewResult.ViewEngineCollection)
                {
                    v.ReleaseView(ControllerContext, viewResult.View);
                }
                return sw.GetStringBuilder().ToString();
            }
        }

        
        private List<ProjectField> GetProjectFields(int id, bool ContainsSample = false)
        {
            string sql = "";
            sql = @"SELECT pf.* FROM ProjectField pf WHERE FieldType IN ('CTY', 'RDO', 'CHK', 'DDN', 'LVW', 'SCD', 'SCG', 'MCG', 'MLT', 'ONG') AND pf.ProjectID = " + id;
            if (ContainsSample)
            {
                sql += " AND pf.ID IN (SELECT FieldId FROM ProjectFieldSample WHERE FieldId = pf.ID AND (ProjectFieldSample.ParentSampleID <> 0 OR ProjectFieldSample.ParentSampleID IS NULL) AND ProjectFieldSample.IsActive = 1)";
            }
            sql += " ORDER BY pf.DisplayOrder, pf.ID";
            var query = db.Database.SqlQuery<ProjectField>(sql);
            return query.ToList<ProjectField>();
        }

        private List<SimpleListWithAggregates> GetProjectFieldSamples(int id)
        {
            string sql = "";

            sql = @"SELECT Id, Title, '' AS GroupId, 0 AS GroupBaseCount FROM ProjectFieldSample WHERE FieldID = " + id + " AND (ParentSampleID <> 0 OR ParentSampleID IS NULL) AND IsActive = 1 ORDER BY DisplayOrder";
            var query = db.Database.SqlQuery<SimpleListWithAggregates>(sql);
            return query.ToList<SimpleListWithAggregates>();
        }

        private int getProjectFieldSampleCount(int id)
        {
            string sql = "";

            sql = @"SELECT COUNT(*) FROM ProjectFieldSample WHERE FieldId = " + id + " AND (ParentSampleID <> 0 OR ParentSampleID IS NULL) AND IsActive = 1";
            return db.Database.SqlQuery<int>(sql).First();
        }

        public ActionResult Tabulation(FormCollection form)
        {            
            return View();
        }

        protected void GenerateSQL(int SideIndex, int TopIndex, AnalyzerQueryDimension d, ref DimensionSQL sql) 
        {
            if (d.FieldType == "RDO" || d.FieldType == "CHK" || d.FieldType == "DDN" || d.FieldType == "LVW" || d.FieldType == "SCD" || d.FieldType == "SCG" || d.FieldType == "MCG" || d.FieldType == "MLT" || d.FieldType == "ONG")
            {
                sql.qfields += (sql.qfields.Length > 0 ? ", " : "") + (d.Position == "Side" ? SideIndex + " AS SideIndex, " + TopIndex + " AS TopIndex, " : "") + 
                    @"'<TopGroupID>' AS <pos>GroupID, 
                    N'<ProjectFieldTitle>' AS <pos>GroupTitle, 
                    <pos>SampleSource.Id AS <pos>Id, <pos>SampleSource.Title AS <pos>Title ";
                sql.qsmpjoin += "INNER JOIN ProjectField <pos>SampleSourceRoot ON p.ID = <pos>SampleSourceRoot.ProjectID ";
                sql.qsmpjoin += "  INNER JOIN ProjectFieldSample <pos>SampleSource ON <pos>SampleSource.FieldID = <pos>SampleSourceRoot.ID AND (<pos>SampleSource.ParentSampleID <> 0 OR <pos>SampleSource.ParentSampleID IS NULL) AND <pos>SampleSource.IsActive = 1 ";

                sql.qfilter += (sql.qfilter.Length > 0 ? "AND " : "") + "<pos>SampleSourceRoot.Id = <ProjectFieldID> ";
                sql.qorderby += (sql.qorderby.Length > 0 ? ", " : "") + "<pos>SampleSource.Title ";
                sql.qgroupby += (sql.qgroupby.Length > 0 ? ", " : "") + "<pos>SampleSourceRoot.ID, <pos>SampleSourceRoot.Title, <pos>SampleSource.Id, <pos>SampleSource.Title ";
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

        protected void ReplacePlaceholders(AnalyzerQueryDimension d, ref DimensionSQL sql, string TopGroupID) 
        {
            sql.qfields = sql.qfields.Replace("<TopGroupID>", TopGroupID + "_" + d.ProjectFieldID.ToString());
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

        protected void BuildTabulation(ref List<AnalyzerQueryDimension> gdxTop, ref List<AnalyzerQueryDimension> rdxTop, ref List<AnalyzerQueryDimension> rdxSide, ref int TopIndex, ref int SideIndex, ref Dictionary<int, DimensionSQL> colSQL, string[] baseOfPrimaryField, string TopGroupID = "")
        {
            foreach (AnalyzerQueryDimension dTop in rdxTop)
            {
                string parentId = "";
                if (dTop.HasChild)
                {
                    //string sql = @"SELECT pfs.* FROM ProjectFieldSample pfs WHERE pfs.FieldID = " + dTop.ProjectFieldID + " ORDER BY DisplayOrder";
                    //var pfsQuery = db.Database.SqlQuery<ProjectFieldSample>(sql);
                    //List<ProjectFieldSample> pfsData = pfsQuery.ToList<ProjectFieldSample>();
                    foreach (var pfsTop in dTop.Items)
                    {
                        AnalyzerQueryDimension dTopChildf = null;
                        //Find the child of the current dimension
                        foreach (AnalyzerQueryDimension dTopChild in gdxTop)
                        {
                            if (dTopChild.ParentAQD != null && dTopChild.ParentAQD.ProjectFieldID == dTop.ProjectFieldID)
                            {
                                dTopChildf = dTopChild;
                                break;
                            }
                        }
                        if (dTopChildf != null)
                        {
                            //Make ParentId
                            parentId = TopGroupID;
                            if (dTopChildf.ParentAQD != null)
                            {
                                parentId += "_" + dTopChildf.ParentAQD.ProjectFieldID.ToString();
                            }
                            if (parentId.Length > 0)
                            {
                                parentId = parentId.Trim('_') + "_";
                            }
                            if (parentId.Length == 0)
                            {
                                parentId = dTop.ProjectFieldID.ToString() + "_";
                            }
                            parentId = "_" + parentId;

                            List<AnalyzerQueryDimension> rdxTopChild = new List<AnalyzerQueryDimension>();
                            rdxTopChild.Add(dTopChildf);
                            BuildTabulation(ref gdxTop, ref rdxTopChild, ref rdxSide, ref TopIndex, ref SideIndex, ref colSQL, baseOfPrimaryField, parentId + pfsTop.Id);
                        }
                    }
                }
                else
                {
                    if (TopGroupID.Length > 0)
                    {
                        DimensionSQL tSQL = new DimensionSQL();

                        //Loop two times to create SQL joins between top and side
                        foreach (AnalyzerQueryDimension dSide in rdxSide)
                        {
                            if (dSide.HasChild) { continue; }
                            GenerateDimensionSQL(ref tSQL, dTop, dSide, TopIndex, SideIndex, rdxSide.Count, TopGroupID);

                            SideIndex++;
                        }

                        colSQL.Add(TopIndex++, tSQL);

                        //break; //Testing single Top dimension only at this time
                    }
                }
            }


        }

        protected void GenerateDimensionSQL(ref DimensionSQL tSQL, AnalyzerQueryDimension dTop, AnalyzerQueryDimension dSide, int TopIndex, int SideIndex, int SideCount, string TopGroupID)
        {
            AnalyzerQueryDimension d;
            int countOfSideSample = 0, countOfTopSample = 0;

            for (short dx = 0; dx < 2; dx++)
            {
                if (dx == 0)
                {
                    d = dSide;
                    countOfSideSample = getProjectFieldSampleCount(d.ProjectFieldID);
                }
                else
                {
                    d = dTop;
                    countOfTopSample = getProjectFieldSampleCount(d.ProjectFieldID);
                }

                GenerateSQL(SideIndex, TopIndex, d, ref tSQL);

                ReplacePlaceholders(d, ref tSQL, TopGroupID);
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
                tSQL.qorderby = "ORDER BY 4, 2, " + tSQL.qorderby + " ";
            }
            else
            {
                tSQL.qorderby = "ORDER BY 4, 2";
            }

            if (SideIndex > 0)
            {   //Remove order by clause
                if (SideIndex == 1)
                {
                    int pos = tSQL.mdsql.IndexOf("ORDER BY");
                    if (pos > 0)
                    {
                        tSQL.mdsql = tSQL.mdsql.Substring(0, pos - 1);
                    }
                }

                //Mid
                //tSQL.mdsql += "UNION (SELECT " + tSQL.qfields + ", " + countOfSideSample + " AS AggregateValue, " + countOfTopSample + " AS GroupBaseCount FROM Project p " + tSQL.qsmpjoin + tSQL.qfilter + tSQL.qgroupby + ")";
                tSQL.mdsql += "UNION (SELECT " + tSQL.qfields + ", 0 AS AggregateValue, " + countOfTopSample + " AS GroupBaseCount FROM Project p " + tSQL.qsmpjoin + tSQL.qfilter + tSQL.qgroupby + ")";

                //Last
                if (SideIndex >= SideCount - 1)
                {
                    tSQL.mdsql += " " + tSQL.qorderby;
                }
            }
            else
            {   //First
                tSQL.mdsql = "SELECT " + tSQL.qfields + ", 0 AS AggregateValue, " + countOfTopSample + " AS GroupBaseCount FROM Project p " + tSQL.qsmpjoin + tSQL.qfilter + tSQL.qgroupby + tSQL.qorderby;
            }
            tSQL.mdsql_wo_order = tSQL.mdsql.Replace(tSQL.qorderby, "");
            tSQL.Clear();
        }
        private void PopulateDimension(ref List<AnalyzerQueryDimension> rdx, List<QueryDesignerFields> qf, List<ProjectFieldView> projectFields, string pos, AnalyzerQueryDimension parentAQD = null)
        {
            int i = 1;
            foreach (QueryDesignerFields item in qf)
            {
                ProjectFieldView pf = projectFields.Find(x => x.ID.Equals(item.id));

                AnalyzerQueryDimension aqd = new AnalyzerQueryDimension();
                if (parentAQD == null)
                {
                    aqd.QueryID = i;
                }
                else
                {
                    aqd.QueryID = parentAQD.QueryID;
                    aqd.ParentAQD = parentAQD;
                    if (parentAQD.ChildrenAQD == null)
                    {
                        parentAQD.ChildrenAQD = new List<AnalyzerQueryDimension>();
                    }
                    parentAQD.ChildrenAQD.Add(aqd);
                }
                aqd.DisplayOrder = i;
                aqd.ProjectFieldID = item.id;
                aqd.ProjectFieldTitle = pf.Title;
                aqd.HasChild = item.children.Count() > 0;
                aqd.ParentPFID = pf.ParentFieldID;
                aqd.Position = pos;
                aqd.FieldType = pf.FieldType;
                aqd.Items = GetProjectFieldSamples(item.id);
                rdx.Add(aqd);
                i++;

                if (item.children.Count() > 0)
                {
                    PopulateDimension(ref rdx, item.children, projectFields, pos, aqd);
                }
            }
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            string sql = @"SELECT * FROM AnalyzerQuery WHERE ProjectID = " + id;
            var query = db.Database.SqlQuery<AnalyzerQuery>(sql);
            AnalyzerQuery aq = query.FirstOrDefault<AnalyzerQuery>();
            if (aq != null && aq.ID > 0)
            {
                sql = @"DELETE FROM AnalyzerQueryData WHERE QueryId = " + aq.ID;
                db.Database.ExecuteSqlCommand(sql);

                sql = @"DELETE FROM AnalyzerQueryDimension WHERE QueryId = " + aq.ID;
                db.Database.ExecuteSqlCommand(sql);

                sql = @"DELETE FROM AnalyzerQuery WHERE ProjectId = " + id;
                db.Database.ExecuteSqlCommand(sql);
            }

            return View();
        }
        [HttpGet]
        public ActionResult DeleteData(int? id)
        {
            string sql = @"SELECT * FROM AnalyzerQuery WHERE ProjectID = " + id;
            var query = db.Database.SqlQuery<AnalyzerQuery>(sql);
            AnalyzerQuery aq = query.FirstOrDefault<AnalyzerQuery>();
            if (aq != null && aq.ID > 0)
            {
                sql = @"UPDATE AnalyzerQueryData SET AggregateValue = 0 WHERE QueryId = " + aq.ID;
                db.Database.ExecuteSqlCommand(sql);
            }
            return View();
        }
    }
}