using System.Globalization;
using DNA_CAPI_MIS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace DNA_CAPI_MIS.Controllers
{
    [Authorize]
    public class ExcelController : Controller
    {
        public ExcelController()
        {
        }

        DNA_CAPI_MIS.DAL.ProjectContext db = new DNA_CAPI_MIS.DAL.ProjectContext();

        [HttpPost]
        

        public ActionResult RHS(string name, string status)
        {
            string sql = @"SELECT case 
 
when id = 7120 then 50435--50446 
when id = 7121 then 50484--50486 
when id = 7122 then 55587--50517 
else 0 end Id , Name,id as RoleId      FROM Project WHERE id in (7120,7121,7122) ORDER BY name"; //7114 ,


            int FieldID = Convert.ToInt32(50435);
            List<DNA_CAPI_MIS.Models.ProjectFieldSample> Distict = db.ProjectFieldSample
                                .Where(x => x.ParentSampleID != 0 && x.IsActive && x.FieldID.Equals(FieldID))
                                .OrderBy(x => x.DisplayOrder)
                                .ToList<ProjectFieldSample>();

            var RHS_A = Distict.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.FieldID.ToString().Trim()
            }).ToList();

            var CheckFor = db.Database.SqlQuery<ProjectsList>(sql);

            ViewBag.District = RHS_A;

            return View();
        }



    }
}