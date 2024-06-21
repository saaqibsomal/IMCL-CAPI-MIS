using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
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
        public string Longitude { get; set; }
        public string Latitude { get; set; }
       
        public string FieldValue { get; set; }
    }


    public class SurveyResponse
    {
        public List<SurveyReport> RawData  { get; set; }
        public List<TitleValue> DataTitle { get; set; }
        public ReportField Field { get; set; }
    }

    public class ReportField
    {
        public string NameOfDistrict { get; set; }
        public string NameOfCenter { get; set; }
        public string DateOfVisit { get; set; }
        public string TimeOfVisit { get; set; }
        public string OpenCloseStatus { get; set; }
        public string StaffPosition { get; set; }

        public string Indication { get; set; }
        public string Cleanliness { get; set; }
        public string COUNSELING { get; set; }
        public string StockofMedicines { get; set; }
        public string StockofContraceptives { get; set; }
        public string StatusofBuilding { get; set; }
        public string DailyClientRegister { get; set; }
        public string MonthlyBreakup { get; set; }
        public string MedicineStockRegister { get; set; }
        public string ContraceptiveStockRegister { get; set; }
        public string LogBook { get; set; }
        public string DeadStockRegister { get; set; }
        public string IECMaterial { get; set; }
        public string MECWheel { get; set; }
        public string EquipmentPosition { get; set; }
        public string Furnitureposition { get; set; }
        public string TechnicalMonitoringChecklist { get; set; }
        public string SERVICEDELIVERY { get; set; }
        public string EquipmentCondition { get; set; }
        public string FurniturePositionCondition { get; set; }
        public string StaffPositionNames { get; set; }
        public string ClientsPresent { get; set; }
        public string PerformanceOfService { get; set; }
        public string NoVisitor { get; set; }
        public string StockOfMed { get; set; }
        public string StockOfCon { get; set; }
        public string Last3Contraceptive { get; set; }
        public string NoOfSup { get; set; }
        public string DCIT { get; set; }
        public string LastThreeMonth { get; set; }
        public string HospitalManagement { get; set; }
        public string RemarksofMonitoringOfficer { get; set; }
        public string Last6Field { get; set; }
        public string NameofProj { get; set; }
    
        
    }
    public class SurveyTitle
    {

        public string FieldValue { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public int FieldID { get; set; }
    }

    public class TitleValue
    {
       public int sbjnum        { get; set; }
       public string SurveyorName  { get; set; }
       public int FieldId       { get; set; }
       public string Title         { get; set; }
        public string FieldValue { get; set; }
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


    public class StuffPosition
    {
        public string District { get; set; }
        public string Center { get; set; }
        public string Remarks { get; set; }

    }

    public class Grid2
    {
        public string ProjectName { get; set; }
        public string District { get; set; }
        public string Center { get; set; }
        public string Premises { get; set; }
        public string OpenClose { get; set; }
        public string Status { get; set; }

    }

    public class Grid13
    {
        public string asDate { get; set; }
        public string ProjectName { get; set; }
        public string District { get; set; }
        public string Center { get; set; }
        public string Monitoring { get; set; }
    }   
    
    public class Grid8
    {
        public string asDate { get; set; }
        public string ProjectName { get; set; }
        public string District { get; set; }
        public string Center { get; set; }
        public string CTP { get; set; }
    }

    public class Grid3
    {
        public string asDate { get; set; }
        public string ProjectName { get; set; }
        public string District { get; set; }
        public string Center { get; set; }
        public string IndicateSign { get; set; }
        public string StatusOfBuilding { get; set; }
        public string Cleanliness { get; set; }

    }
    public class Grid4
    {
        public string asDate { get; set; }
        public string ProjectName { get; set; }
        public string District { get; set; }
        public string Center { get; set; }
        public string GC { get; set; }
        public string FB { get; set; }
        public string MCH { get; set; }
        public string CS { get; set; }

    }

    public class Grid5
    {
        public string asDate { get; set; }
        public string ProjectName { get; set; }
        public string District { get; set; }
        public string Center { get; set; }
        public string CS { get; set; }

    }

    public class Grid7
    {
        public string asDate { get; set; }
        public string ProjectName { get; set; }
        public string District { get; set; }
        public string Center { get; set; }
        public string ConStockPosition { get; set; }

    }


    public class Grid6
    {
        public string asDate { get; set; }
        public string ProjectName { get; set; }
        public string District { get; set; }
        public string Center { get; set; }
        public string IECMatrial { get; set; }
        public string MECWheel { get; set; }


    }

    public class IECStock
    {
        public string asDate { get; set; }
        public string ProjectName { get; set; }
        public string District { get; set; }
        public string Center { get; set; }
        public string dailyClient { get; set; }
        public string MediStock { get; set; }
        public string BreakUp { get; set; }
        public string StockReg { get; set; }
        public string LogBook { get; set; }
        public string DeadStock { get; set; }


    }

    public class Grid11
    {
        public string asDate { get; set; }
        public string ProjectName { get; set; }
        public string District { get; set; }
        public string Center { get; set; }
        public string Technical { get; set; }



    }  
    
    public class Grid12
    {
        public string asDate { get; set; }
        public string ProjectName { get; set; }
        public string District { get; set; }
        public string Center { get; set; }
        public string HMC { get; set; }



    }
    public class ContraceptivePie
    {

        public string Contraceptive { get; set; }
        public int  Qty { get; set; }
    }

}