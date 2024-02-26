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
    public class AnalyzerQuery
    {
        [Key]
        [Display(Name = "Query Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(200)]
        [Display(Name = "Query Title")]
        public string Title { get; set; }

        [ForeignKey("Project")]
        [Display(Name = "Project Id")]
        public int ProjectID { get; set; }

        [Display(Name = "Primary Base Distribution On")]
        public int? BaseProjectFieldID { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public virtual Project Project { get; set; }
    }

    public class AnalyzerQueryDimension
    {
        [Key]
        [Column("QueryID", Order = 0)]
        [ForeignKey("AnalyzerQuery")]
        [Display(Name = "Query Id")]
        public int QueryID { get; set; }

        [Key]
        [Column("DisplayOrder", Order = 1)]
        [Display(Name = "Display Order")]
        public int? DisplayOrder { get; set; }

        [ForeignKey("ProjectField")]
        [Display(Name = "Project Field Id")]
        public int ProjectFieldID { get; set; }

        [StringLength(127)]
        [Display(Name = "Project Field Title")]
        public string ProjectFieldTitle { get; set; }

        public bool HasChild { get; set; }

        public int? ParentPFID { get; set; }

        [Display(Name = "Position of Dimension [Side/Top]")]
        public string Position { get; set; }            //T, S

        [StringLength(10)]
        [Display(Name = "Field Type")]
        /*
         * SLT = SINGLE LINE TEXT
         * MLT = MULTI LINE TEXT
         * RDO = RADIO BUTTONS
         * CHK = CHECK BOXES
         * NUM = INTEGER NUMBER
         * CUR = CURRENCY / FLOATING POINT
         * DDN = DROP DOWN
         * LVW = LIST VIEW
         * DAT = DATE ONLY
         * TIM = TIME ONLY
         * DTM = DATE AND TIME
         * PIC = PICTURE
         */
        public string FieldType { get; set; }

        [StringLength(20)]
        public string SourceTable { get; set; }

        [StringLength(20)]
        public string SourceTableIDField { get; set; }

        [StringLength(20)]
        public string SourceTableTitleField { get; set; }

        [StringLength(20)]
        public string SourceTableKeyField { get; set; }

        [StringLength(20)]
        public string JoinTable { get; set; }

        [StringLength(20)]
        public string JoinTableKeyField { get; set; }

        [StringLength(20)]
        public string JoinTableValueField { get; set; }

        public virtual List<AnalyzerQueryDimension> ChildrenAQD { get; set; }
        public virtual AnalyzerQueryDimension ParentAQD { get; set; }
        public virtual AnalyzerQuery AnalyzerQuery { get; set; }
        public virtual ProjectField ProjectField { get; set; }
        public List<SimpleListWithAggregates> Items { get; set; }

    }

    public class AnalyzerQueryData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [ForeignKey("AnalyzerQuery")]
        [Display(Name = "Query Id")]
        public int QueryID { get; set; }
        public int TopIndex { get; set; }
        public int TopDisplayOrder { get; set; }
        public string TopGroupId { get; set; }
        public string TopGroupTitle { get; set; }
        
        [Index("AQD_TopId")]
        public int TopId { get; set; }
        public string TopTitle { get; set; }
        public int SideIndex { get; set; }
        public int SideDisplayOrder { get; set; }
        public string SideGroupId { get; set; }
        public string SideGroupTitle { get; set; }

        [Index("AQD_SideId")]
        public int SideId { get; set; }
        public string SideTitle { get; set; }
        public int AggregateValue { get; set; }
        public int GroupBaseCount { get; set; }
        public virtual AnalyzerQuery AnalyzerQuery { get; set; }
    }

    //public class ProjectField : DNA_CAPI_MIS.Models.ProjectField
    //{
    //    public List<ProjectFieldSample> ProjectFieldSamples { get; set; }
    //}

}