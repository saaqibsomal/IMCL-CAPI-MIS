using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DNA_CAPI_MIS.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DNA_CAPI_MIS.DAL
{
    public class ProjectContext : DbContext
    {
        public ProjectContext()
            : base("DNASurvey")
        {
            //Disable initializer
            Database.SetInitializer<ProjectContext>(null);
            //AddUserAndRoles();
        }

        //public bool AddUserAndRoles()
        //{
        //    bool success = false;

        //    var idManager = new IdentityManager();
        //    success = idManager.CreateRole("KSAManager");
        //    if (!success == true) return success;

        //    success = idManager.CreateRole("GCCManager");
        //    if (!success == true) return success;

        //    success = idManager.CreateRole("CanEdit");
        //    if (!success == true) return success;

        //    success = idManager.CreateRole("User");
        //    if (!success) return success;


        //    var newUser = new ApplicationUser()
        //    {
        //        UserName = "admin",
        //        FirstName = "Admin",
        //        LastName = "DNA",
        //        Email = "admin@dnaworkspace.com"
        //    };

        //    success = idManager.CreateUser(newUser, "DNA1admin");
        //    if (!success) return success;

        //    success = idManager.AddUserToRole(newUser.Id, "Admin");
        //    if (!success) return success;

        //    success = idManager.AddUserToRole(newUser.Id, "CanEdit");
        //    if (!success) return success;

        //    success = idManager.AddUserToRole(newUser.Id, "User");
        //    if (!success) return success;

        //    return success;
        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<DNA_CAPI_MIS.Models.Project> Projects { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.Survey> Surveys { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.SurveyData> SurveyData { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.SurveyLocation> SurveyLocation { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.ProjectField> ProjectField { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.ProjectFieldSample> ProjectFieldSample { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.ProjectFieldMediaFile> ProjectFieldMediaFile { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.ProjectFieldSampleMediaFile> ProjectFieldSampleMediaFile { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.ProjectFieldSection> ProjectFieldSection { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.Translation> Translation { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.Country> Country { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.City> City { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.District> District { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.CityMapping> CityMapping { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.DistrictMapping> DistrictMapping { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.UserDistrict> UserDistrict { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.TrackUser> TrackUser { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.Report> Report { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.ReportRecipient> ReportRecipient { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.RDXLocation> RDXLocation { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.RDXProjectFieldSample> RDXProjectFieldSample { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.RDXCustomerRegion> RDXCustomerRegion { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.AnalyzerQuery> AnalyzerQuery { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.AnalyzerQueryDimension> AnalyzerQueryDimension { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.AnalyzerQueryData> AnalyzerQueryData { get; set; }

        public DbSet<DNA_CAPI_MIS.Models.Customer> Customer { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.CustomerBranch> CustomerBranch { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.CustomerUser> CustomerUser { get; set; }
        public DbSet<DNA_CAPI_MIS.Models.CustomerRegion> CustomerRegion { get; set; }

        //public System.Data.Entity.DbSet<DNA_CAPI_MIS.Models.CensusCategoriesView> CensusCategoriesViews { get; set; }
    }

}
