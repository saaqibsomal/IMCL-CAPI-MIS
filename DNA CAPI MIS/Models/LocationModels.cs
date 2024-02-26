using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNA_CAPI_MIS.Models
{
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "Country Name")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Country name cannot be longer than 100 characters.")]
        public string Name { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
    }

    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "City Name")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "City name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Display(Name = "Country Id")]
        public int? CountryID { get; set; }

        [StringLength(100)]
        public string Region { get; set; }

        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string KMLPath { get; set; }

        public virtual Country Country { get; set; }
    }

    public class District
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "District Name")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "District name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Display(Name = "City Id")]
        public int? CityID { get; set; }

        [Display(Name = "DistanceMax")]
        public double? DistanceMax { get; set; }

        public virtual City City { get; set; }
    }

    public class UserDistrict
    {
        [Key]
        [Column(Order = 0)]
        [Display(Name = "Surveyor")]
        [StringLength(50, MinimumLength = 1)]
        public string UserName { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Project")]
        [Display(Name = "Project")]
        public int? ProjectID { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("District")]
        [Display(Name = "District")]
        public int? DistrictID { get; set; }

        [Display(Name = "Is Signed Off?")]
        public bool IsSignedOff { get; set; }

        public virtual Project Project { get; set; }
        public virtual District District { get; set; }
    }

    public class TrackUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime OnDateTime { get; set; }

        [Display(Name = "User Name")]
        [StringLength(50, MinimumLength = 1)]
        public string UserName { get; set; }

        [Display(Name = "Device Datetime")]
        public DateTime? DeviceDateTime { get; set; }

        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }

        [Display(Name = "Accuracy (meters)")]
        public double? Accuracy { get; set; }

        public int? RouteID { get; set; }
        public int? MatchingTrackID { get; set; }
    }


    public class CityMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int CityID { get; set; }
        public int? ProjectID { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }
        public string KMLPath { get; set; }
    }

    public class DistrictMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int DistrictID { get; set; }
        public int? ProjectID { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }
    }

    public class DistrictsView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string CityName { get; set; }
    }
}