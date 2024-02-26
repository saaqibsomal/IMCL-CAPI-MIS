using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNA_CAPI_MIS.Models
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int ID { get; set; }

        [Index("ProjectName")]
        [Display(Name = "Project Name")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Project name cannot be longer than 255 characters.")]
        public string Name { get; set; }
        
        [Index("Guid")]
        [Required]
        [StringLength(100, ErrorMessage = "GUID cannot be longer than 100 characters.")]
        public string Guid { get; set; }

        [Required]
        [Display(Name = "QC Requirement (Surveyor wise in %)")]
        public int QCRequirement { get; set; }

        [Column(TypeName = "ntext")]
        [MaxLength]
        public string questions_list { get; set; }

        [StringLength(10)]
        public string fld_respondent_name { get; set; }
        [StringLength(10)]
        public string fld_respondent_phone1 { get; set; }
        [StringLength(10)]
        public string fld_respondent_phone2 { get; set; }

        [Display(Name = "Actual Start Date")]
        public DateTime? ActualStartDate { get; set; }
        
        [Display(Name = "Actual End Date")]
        public DateTime? ActualEndDate { get; set; }

        [Display(Name = "Created By")]
        public int CreatedByUserId { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Last Modified By")]
        public int ModifiedByUserId { get; set; }

        [Display(Name = "Last Modified On")]
        public DateTime ModifiedOn { get; set; }

        [Index("STGSync")]
        [Display(Name = "Sync with Dooblo SurveyToGo")]
        public bool STGSync { get; set; }

        [Index("ProjectType")]
        [StringLength(1)]
        public string Type { get; set; }            //C=Census, S=Survey

        [StringLength(1)]
        public string Visibility { get; set; }            //I=Only Interviewer, R=Only Respondent (Clink), B=Both

        [Display(Name = "Category Field")]
        public int? CategoryPFId { get; set; }
        
        [Display(Name = "Title Field")]
        public int? TitlePFId { get; set; }

        [Display(Name = "Respondent Name Field")]
        public int? RespondentNamePFId { get; set; }

        [Display(Name = "Respondent Mobile")]
        public int? RespondentMobilePFId { get; set; }

        [StringLength(1)]
        public string Status { get; set; }

        [Display(Name = "Status Changed By")]
        public int? StatusChangedByUserId { get; set; }

        [Display(Name = "Status Changed On")]
        public DateTime? StatusChangedOn { get; set; }

        [Display(Name = "Require Map Position Input")]
        public bool NeedMapInput { get; set; }

        [Display(Name = "Require KSA Building Search")]
        public bool NeedBuildingSearch { get; set; }


        //[Display(Name = "Require KSA Building Search")]
        //[StringLength(1)]
        //public string Type { get; set; }        //C=Census, S=Survey

        //[Display(Name = "Require KSA Building Search")]
        //[StringLength(1)]
        //public string Accessibility { get; set; }        //M=Mobile,W=CATI Web
        
        public virtual ICollection<Survey> ProjectSurvey { get; set; }
        //public virtual ICollection<ProjectVariable> ProjectFieldMap { get; set; }
        //public virtual ICollection<ProjectCriteria> ProjectCriteriaMap { get; set; }
        //public virtual ICollection<ProjectData> ProjectDataValues { get; set; }
    }

    public class ProjectField
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Field Id")]
        public int ID { get; set; }

        [Display(Name = "Parent Field Id")]
        public int? ParentFieldID { get; set; }
        public string MappedFieldID { get; set; }

        [StringLength(2048)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [StringLength(512)]
        [Display(Name = "Instructions")]
        public string Instructions { get; set; }

        [Display(Name = "Project Id")]
        public int? ProjectID { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Is Visible")]
        public bool IsVisible { get; set; }
        
        [Display(Name = "Is Mandatory")]
        public bool IsMandatory { get; set; }

        [Display(Name = "Display Order")]
        public int? DisplayOrder { get; set; }

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

        [StringLength(4)]
        [Display(Name = "ViewStyle")]
        public string ViewStyle { get; set; }       //Radio, Button, Dropdown

        [StringLength(1)]
        public string Visibility { get; set; }            //I=Only Interviewer, R=Only Respondent (Clink), B=Both

        public string VariableName { get; set; }

        [StringLength(127)]
        [Display(Name = "Report Title")]
        public string ReportTitle { get; set; }

        [StringLength(40)]
        [Display(Name = "DefaultValue")]
        public string DefaultValue { get; set; }
        
        [Display(Name = "Section")]
        public int? SectionID { get; set; }

        public string ScriptOnEntry { get; set; }
        public string ScriptOnValidate { get; set; }
        public string ScriptOnExit { get; set; }

        public string OptionsJSON { get; set; }     //Options encoded in JSON

        public bool? IsMarked { get; set; }     //When true, Points should be calculated

        public virtual Project Project { get; set; }
    }

    public class ProjectFieldMediaFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "File Id")]
        public int ID { get; set; }

        [Display(Name = "Field Id")]
        public int FieldID { get; set; }

        [StringLength(100)]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [StringLength(20)]
        [Display(Name = "File Code")]
        public string FileCode { get; set; }        //Purpose of the file

        [StringLength(10)]
        [Display(Name = "File Type")]
        public string FileType { get; set; }        //Image, Audio, Video, PDF

        [Display(Name = "Options JSON")]
        public string OptionsJSON { get; set; }

        [Display(Name = "Uploaded On DateTime")]
        public DateTime? UploadedOn { get; set; }

        [Display(Name = "Uploaded By")]
        public int? UploadedBy { get; set; }
    }
    
    public class ProjectFieldSample
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Sample Id")]
        public int ID { get; set; }

        [Display(Name = "Parent Sample Id")]
        public int? ParentSampleID { get; set; }

        [Display(Name = "Field Id")]
        public int FieldID { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [StringLength(255)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Min Sample %age")]
        public int? MinSample { get; set; }

        [Display(Name = "Max Sample %age")]
        public int? MaxSample { get; set; }

        [StringLength(20)]
        [Column(TypeName = "varchar")]
        [Index("PFS_VariableName")]
        public string VariableName { get; set; }

        [StringLength(20)]
        [Column(TypeName = "varchar")]
        [Index("PFS_Code")]
        public string Code { get; set; }

        [Display(Name = "Display Order")]
        public int? DisplayOrder { get; set; }

        [Display(Name = "Options JSON")]
        public string OptionsJSON { get; set; }

        [Display(Name = "Points")]
        public int? Points { get; set; }
    }

    public class ProjectFieldSampleMediaFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "File Id")]
        public int ID { get; set; }

        [Display(Name = "FieldSample Id")]
        public int FieldSampleID { get; set; }

        [StringLength(100)]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [StringLength(20)]
        [Display(Name = "File Code")]
        public string FileCode { get; set; }        //Purpose of the file

        [StringLength(10)]
        [Display(Name = "File Type")]
        public string FileType { get; set; }        //Image, Audio, Video, PDF

        [Display(Name = "Options JSON")]
        public string OptionsJSON { get; set; }

        [Display(Name = "Uploaded On DateTime")]
        public DateTime? UploadedOn { get; set; }

        [Display(Name = "Uploaded By")]
        public int? UploadedBy { get; set; }
    }

    public class ProjectFieldSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Section Id")]
        public int ID { get; set; }

        [Display(Name = "Section Name")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Section name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Display(Name = "Project Id")]
        public int ProjectID { get; set; }

        [Display(Name = "Display Order")]
        public int? DisplayOrder { get; set; }

    }

    public class ProjectView
    {
        public int ID { get; set; }
        public string Name { get; set; }

    }
    public class ProjectFieldView
    {
        public int ID { get; set; }
        public int? ParentFieldID { get; set; }
        public string MappedFieldID { get; set; }
        public string Title { get; set; }
        public int? ProjectID { get; set; }
        public bool IsActive { get; set; }
        public bool IsMandatory { get; set; }
        public int? DisplayOrder { get; set; }
        public string FieldType { get; set; }
        public string VariableName { get; set; }
        public string ReportTitle { get; set; }
        public string DefaultValue { get; set; }
        public int? SectionID { get; set; }
        public string Visibility { get; set; }
    }

    public class ProjectFieldSampleView
    {
        public int ID { get; set; }
        public int FieldID { get; set; }
        public string Title { get; set; }
        public string VariableName { get; set; }
        public string Code { get; set; }
        public int? DisplayOrder { get; set; }
    }

    public class ProjectUsers
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int IsSelected { get; set; }
    }

}