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

namespace DNA_CAPI_MIS.DAL
{
    public class ProjectInitializer: System.Data.Entity. DropCreateDatabaseIfModelChanges<ProjectContext>
    {
        protected override void Seed(ProjectContext context)
        {
        }
    }
}