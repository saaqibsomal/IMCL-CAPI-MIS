using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//Package Manager Console - Add Database Migration
//add-migration <TagName>
//update-database

namespace DNA_CAPI_MIS.Models
{
    public class Census
    {
    }

    public class CensusCategoriesView
    {
        [Key]
        [Display(Name = "Category Id")]
        public int SampleID { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }
    }
    public class CityView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public string KMLPath { get; set; }
    }

    public class FieldProgress
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string CityKML { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int SurveyCount { get; set; }
        public double DistanceMade { get; set; }
        public double DistanceTotal { get; set; }
    }

}