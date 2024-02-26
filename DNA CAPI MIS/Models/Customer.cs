using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DNA_CAPI_MIS.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int ID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }

    public class CustomerBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Name { get; set; }

        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        [ForeignKey("City")]
        [Display(Name = "City")]
        public int? CityID { get; set; }

        [Display(Name = "Display Order")]
        public int? DisplayOrder { get; set; }

        public bool? IsActive { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual City City { get; set; }
    }

    public class CustomerUser
    {
        [Key]
        [ForeignKey("CustomerBranch")]
        [Column(Order = 0)]
        public int CustomerBranchID { get; set; }

        [Key]
        [Column(Order = 1)]
        public int UserID { get; set; }

        public virtual CustomerBranch CustomerBranch { get; set; }
    }

    public class CustomerRegion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        public virtual Customer Customer { get; set; }
    }

}