using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNA_CAPI_MIS.Models
{
    public class Survey
    {
        [Key]
        [Display(Name = "Survey Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sbjnum { get; set; }

        [ForeignKey("Project")]
        [Display(Name = "Project Id")]
        public int ProjectID { get; set; }

        [Display(Name = "Source Survey Id")]
        public int SourceSurveyId { get; set; }

        [StringLength(15)]
        [Display(Name = "Device Survey Id")]
        public string DeviceSurveyId { get; set; }

        [StringLength(10)]
        [Display(Name = "App Version")]
        public string AppVersion { get; set; }

        [Display(Name = "Country")]
        public int? CountryID { get; set; }

        [Display(Name = "City")]
        public int? CityID { get; set; }

        [Display(Name = "District")]
        public int? DistrictID { get; set; }

        public DateTime Created { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        [Display(Name = "Device UDID")]
        [StringLength(60)]
        public string DeviceId { get; set; }

        [Display(Name = "Device Timestamp on Start")]
        public DateTime? DeviceTimestampStart { get; set; }

        [Display(Name = "Device Timestamp on Submit")]
        public DateTime? DeviceTimestamp { get; set; }

        [Display(Name = "Device Timestamp on Upload")]
        public DateTime? DeviceTimestampUpload { get; set; }

        //[Display(Name = "Surveyor ID")]
        //public int SurveyorId { get; set; }
        
        [Display(Name = "Surveyor Username")]
        public string SurveyorName { get; set; }

        [Display(Name = "Test Survey?")]
        public bool? Test { get; set; }

        [Display(Name = "Field Operations Status")]
        public int? OpStatus { get; set; }

        [Display(Name = "QC Status")]
        public int? QCStatus { get; set; }

        [Display(Name = "QC Rejection Reason")]
        public int? QCRejectReasonId { get; set; }

        [Display(Name = "QC Remarks")]
        [StringLength(60)]
        public string QCRemarks { get; set; }

        [Display(Name = "Version number")]          //The latest record will have NULL here
        public int? Version { get; set; }

        [Display(Name = "Version for Survey ID")]   //Original (first) Survey Id, so that we can group all versions, if required
        public int? VersionForId { get; set; }

        [Display(Name = "Last QC By")]
        public int? LastQCByUserId { get; set; }

        [Display(Name = "Last QC On")]
        public DateTime? LastQCOn { get; set; }

        public virtual Project Project { get; set; }
    }

    public class SurveyData
    {
        [Key]
        [ForeignKey("Survey")]
        [Column("sbjnum", Order = 0)]
        [Display(Name = "Survey Id")]
        public int sbjnum { get; set; }

        [Key]
        [Column("FieldId", Order = 1)]
        [Display(Name = "Field Id")]
        public int FieldId { get; set; }

        [StringLength(1024)]
        [Display(Name = "Field Value")]
        public string FieldValue { get; set; }

        [Display(Name = "Response Time")]
        public int? ResponseTime { get; set; }      //In milliseconds
        
        public virtual Survey Survey { get; set; }

    }

    public class SurveyLocation
    {
        [Key]
        [ForeignKey("Survey")]
        [Display(Name = "Survey Id")]
        public int sbjnum { get; set; }

        public string time_spent_for_location { get; set; }
        public string cellid { get; set; }
        public string CITY_GPS { get; set; }
        public string DISTRICT_GPS { get; set; }
        public string DISTRICT_AUDITOR { get; set; }
        public string BUILDING_NUMBER_AUDITOR { get; set; }
        public string BUILDING_TYPE_DROPDOWN { get; set; }
        public string STREET_NAME_GPS { get; set; }
        public string STREET_NAME_DROPDOWN { get; set; }
        public string STREET_NAME_AUDITOR { get; set; }
        public string ZIPCODE_GPS { get; set; }
        public string ADDITIONAL_CODE_GPS { get; set; }
        public string LANDMARK_GPS { get; set; }
        public string LANDMARK_AUDITOR { get; set; }
        public string STC_SHEET_AUDITOR { get; set; }
        public string MOBILY_SHEET_AUDITOR { get; set; }

        public virtual Survey Survey { get; set; }
    }

    public class SurveyDataView
    {
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }

        public int sbjnum { get; set; }
        public int? STG_sbjnum { get; set; }
        public string OpStatus { get; set; }
        public string QCStatus { get; set; }
        public int? QCRejectReasonId { get; set; }
        public string QCRemarks { get; set; }
        public string LastQCByUserName { get; set; }
        public DateTime? LastQCOn { get; set; }

        public DateTime? Created { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string DeviceId { get; set; }
        public DateTime? DeviceTimestamp { get; set; }
        public string SurveyorName { get; set; }

        public int FieldId { get; set; }

        public string FieldType { get; set; }

        public string Title { get; set; }
        public string ReportTitle { get; set; }
        public string VariableName { get; set; }

        public string FieldValue { get; set; }
        public int SubCategoryID { get; set; }

        public string SubCategoryName { get; set; }
        public int Multiples { get; set; }
        public double distanceFromLastPoint { get; set; }
        public int yy { get; set; }
        public int mm { get; set; }
        public int dd { get; set; }
        public int hh { get; set; }
        public int mins { get; set; }
        public int ss { get; set; }
    
        public string time_spent_for_location { get; set; }
        public string cellid { get; set; }
        public string CITY_GPS { get; set; }
        public string DISTRICT_GPS { get; set; }
        public string DISTRICT_AUDITOR { get; set; }
        public string BUILDING_NUMBER_AUDITOR { get; set; }
        public string BUILDING_TYPE_DROPDOWN { get; set; }
        public string STREET_NAME_GPS { get; set; }
        public string STREET_NAME_DROPDOWN { get; set; }
        public string STREET_NAME_AUDITOR { get; set; }
        public string ZIPCODE_GPS { get; set; }
        public string ADDITIONAL_CODE_GPS { get; set; }
        public string LANDMARK_GPS { get; set; }
        public string LANDMARK_AUDITOR { get; set; }
        public string STC_SHEET_AUDITOR { get; set; }
        public string MOBILY_SHEET_AUDITOR { get; set; }

        public string DeviceSurveyId { get; set; }
        public string AppVersion { get; set; }
    }

    public class RepCensusProgressSW
    {
        public int POICount { get; set; }
        public string SurveyorName { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string CityName { get; set; }
        public double? DistanceMax { get; set; }
        public double? DistanceMade { get; set; }
        public int? OpCancelled { get; set; }
        public int? QCCancelled { get; set; }
    }

    public class TrackUserRouteDataView
    {
        public int id { get; set; }
        public string SurveyorName { get; set; }
        public DateTime ForDate { get; set; }
        public double distance { get; set; }
        public string SnappedPath { get; set; }
    }

    public class ProjectFieldSampleLookup
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public string Title { get; set; }
    }
    public class SummaryView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int OutletCount { get; set; }
        public int Color_Red { get; set; }
        public int Color_Green { get; set; }
        public int Color_Blue { get; set; }
    }

    public class Surveyorname {
       
        public string Name { get; set; }
        public int count { get; set; }
    
    }
    public class SurveyDetail { 
    
        public DateTime Created{get;set;}
        public double Latitude{get;set;}
        public double Longitude { get; set; }
    
    }

   

}