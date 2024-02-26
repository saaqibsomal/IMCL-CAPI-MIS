using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNA_CAPI_MIS.Models
{
    public class ReportsModel
    {
    }
    public class Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Display(Name = "Report Name")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [StringLength(3)]
        [Display(Name = "Report Type")]
        public string Type { get; set; }
        //LFS = LocationProjectFieldSample => Cross Tab of Location and ProjectFieldSample values

        [ForeignKey("Project")]
        [Display(Name = "Project")]
        public int? ProjectID { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public virtual Project Project { get; set; }
    }

    //Report-Dimension (RDX)
    public class RDXLocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [ForeignKey("Report")]
        [Display(Name = "Report")]
        public int? ReportID { get; set; }

        [StringLength(10)]
        [Display(Name = "Region Name")]
        public string RegionName { get; set; }

        [DisplayFormat(NullDisplayText = "KML not defined")]
        [Column(TypeName = "ntext")]
        public string RegionKML { get; set; }

        [ForeignKey("Country")]
        [Display(Name = "Country")]
        public int? CountryID { get; set; }

        [ForeignKey("City")]
        [Display(Name = "City")]
        public int? CityID { get; set; }

        [ForeignKey("District")]
        [Display(Name = "District")]
        public int? DistrictID { get; set; }

        [Display(Name = "Display Order")]
        public int? DisplayOrder { get; set; }

        public virtual Report Report { get; set; }
        public virtual Country Country { get; set; }
        public virtual City City { get; set; }
        public virtual District District { get; set; }
    }

    //Report-Dimension (RDX)
    public class RDXProjectFieldSample
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Report")]
        [Display(Name = "Report")]
        public int? ReportID { get; set; }

        [Key]
        [Column(Order = 1)]
        [Display(Name = "Project Field Sample")]
        public int ProjectFieldSampleID { get; set; }

        [Display(Name = "Display Order")]
        public int? DisplayOrder { get; set; }

        public virtual Report Report { get; set; }
        public virtual ProjectFieldSample ProjectFieldSample { get; set; }
    }

    //Report-Dimension (RDX)
    public class ReportRecipient
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Report")]
        [Display(Name = "Report")]
        public int? ReportID { get; set; }

        [Key]
        [Column(Order = 1)]
        [Display(Name = "User")]
        public int UserID { get; set; }
        public virtual Report Report { get; set; }
    }


    public class RDXCustomerRegion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [ForeignKey("Report")]
        public int ReportID { get; set; }

        [ForeignKey("CustomerRegion")]
        public int CustomerRegionID { get; set; }

        [ForeignKey("City")]
        [Display(Name = "City")]
        public int? CityID { get; set; }

        [ForeignKey("District")]
        [Display(Name = "District")]
        public int? DistrictID { get; set; }

        [Display(Name = "Display Order")]
        public int? DisplayOrder { get; set; }

        public virtual Report Report { get; set; }
        public virtual CustomerRegion CustomerRegion { get; set; }
        public virtual City City { get; set; }
        public virtual District District { get; set; }
    }
}