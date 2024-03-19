using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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


    public class SurveyReport
    {
        public int sbjnum { get; set; }
        public string SurveyorName { get; set; }
        public int FieldId { get; set; }
        public string Title { get; set; }
        public string ReportTitle { get; set; }
        public string FieldValue { get; set; }
    }

    public class SurveyTitle
    {

        public string FieldValue { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public int FieldID { get; set; }
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

    public class BarChart
    {
        public string Title { get; set; }
        public int OpenCenter { get; set; }
    }

    public class PieChartOC
    {
        public string Title { get; set; }
        public int OpenClose { get; set; }
    }
    public class TotalSurveyDetail
    {
        public List<SelectListItem> selectListItems { get; set; }
        public string Name { get; set; }
        public int All { get; set; }
        public int RHS { get; set; }
        public int MSU { get; set; }
        public int FWC { get; set; }
        public int Open { get; set; }
        public int Close { get; set; }
        public string OpenTitle { get; set; }
        public string CloseTitle { get; set; }

        public int UnBrandedCnt{ get; set; }
        public int BrandedCnt { get; set; }


        public int Absent { get; set; }
        public int Leave { get; set; }
        public int Present { get; set; }
        public int Vacant { get; set; }
    }

   public class OpenCloseResponse
    {
        public int OpenClose { get; set; }
        public string Title { get; set; }
    }

    public class EmpStatus
    {
        public int cnt { get; set; }
        public string Status { get; set; }
    }  
    
    public class Branded
    {
        public int BrandedCnt { get; set; }
        public string Name { get; set; }
    }

    public class MonitoringOfficerDto
    {
        public string District { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string OpenClose { get; set; }
        public string Remarks { get; set; }
    }  
    
    
    public class Contraceptive
    {
        public string District { get; set; }
        public string contraceptive { get; set; }
        public string FieldValue1 { get; set; }

    }

}