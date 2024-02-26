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
        public int? RoleId { get; set; }
    }
    public class ProjectFieldResultSet
    {
        public int PF_Id { get; set; }
        public int? PF_ParentFieldID { get; set; }
        public string PF_FieldType { get; set; }
        public string PF_Title { get; set; }
        public bool PF_IsMandatory { get; set; }
        public int? PF_DisplayOrder { get; set; }
        public int? PFS_Id { get; set; }
        public int? PFS_ParentSampleID { get; set; }
        public int? PFS_FieldId { get; set; }
        public string PFS_Title { get; set; }
        public string PFS_VariableName { get; set; }
        public int? PFS_DisplayOrder { get; set; }
        public string PFS_Code { get; set; }
    }

    public class QueryDesignerFields
    {
        public int id { get; set; }
        public List<QueryDesignerFields> children { get; set; }

        public QueryDesignerFields()
        {
            children = new List<QueryDesignerFields>();
        }
    }

    public class SimpleList
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    public class SimpleListWithAggregates
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int GroupBaseCount { get; set; }
    }
    public class GroupInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Field { get; set; }
        public int ParentId { get; set; }
        public List<SimpleListWithAggregates> Items { get; set; }
    }
    public class BasicSingleCrosstab
    {
        public int TopIndex { get; set; }
        public int TopDisplayOrder { get; set; }
        public int TopGroupId { get; set; }
        public string TopGroupTitle { get; set; }
        public int TopId { get; set; }
        public string TopTitle { get; set; }
        public int SideIndex { get; set; }
        public int SideDisplayOrder { get; set; }
        public int SideGroupId { get; set; }
        public string SideGroupTitle { get; set; }
        public int SideId { get; set; }
        public string SideTitle { get; set; }
        public int AggregateValue { get; set; }
        public int GroupBaseCount { get; set; }
    }
    public class BasicSingleCrosstabBase
    {
        public int SideIndex { get; set; }
        public int TopId { get; set; }
        public int GroupBaseCount { get; set; }
    }

}