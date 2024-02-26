using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DNA_CAPI_MIS.Models
{
    public class ProjectsInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SurveyCount { get; set; }
        public int SurveyorsCount { get; set; }
    }
    public class SurveyorStats
    {
        public string SurveyorName { get; set; }
        public int SurveyCount { get; set; }
    }
    
    public class ProjectsList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int RoleId { get; set; }
        public string Status { get; set; }

        public string GetStatus()
        {
            switch (Status)
            {
                case "D":
                    return "Draft";
                case "T":
                    return "Test";
                case "P":
                    return "Published";
                case "C":
                    return "Closed";
                default:
                    return "";
            }
        }

        public string GetStatusClass()
        {
            switch (Status)
            {
                case "D":
                    return "label-warning";
                case "T":
                    return "label-info";
                case "P":
                    return "label-success";
                case "C":
                    return "label-primary";
                default:
                    return "";
            }
        }
    }
    public class ProjectFieldResultSet
    {
        public int PF_Id { get; set; }
        public int? PF_ParentFieldID { get; set; }
        public string PF_FieldType { get; set; }
        public string PF_ReportTitle { get; set; }
        public string PF_Title { get; set; }
        public string PF_Instructions { get; set; }
        public bool PF_IsMandatory { get; set; }
        public int? PF_DisplayOrder { get; set; }
        public int? PF_SectionID { get; set; }
        public int? PFS_Id { get; set; }
        public int? PFS_ParentSampleID { get; set; }
        public int? PFS_FieldId { get; set; }
        public string PFS_Title { get; set; }
        public string PFS_VariableName { get; set; }
        public int? PFS_DisplayOrder { get; set; }
        public string PFS_Code { get; set; }
        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public Dictionary<string, Object> OptionsJSON { get; set; }

    }

    public class QueryDesignerFields
    {
        public int id { get; set; }
        public List<QueryDesignerFields> children { get; set; }
        public List<SimpleListWithAggregates> Items { get; set; }

        public QueryDesignerFields()
        {
            children = new List<QueryDesignerFields>();
        }
    }

    public class PieChart
    {
        public string Title { get; set; }
        public string OpenCenter { get; set; }
    }
}