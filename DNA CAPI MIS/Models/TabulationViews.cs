using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DNA_CAPI_MIS.Models
{
    public class SimpleList
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
    public class SimpleListWithAggregates
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string GroupId { get; set; }
        public int GroupBaseCount { get; set; }
    }
    public class GroupInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Field { get; set; }
        public int ParentId { get; set; }
        public bool HasChild { get; set; }
        public bool TopLevel { get; set; }
        public List<SimpleListWithAggregates> Items { get; set; }
    }
    public class BasicSingleCrosstab
    {
        public int TopIndex { get; set; }
        public int TopDisplayOrder { get; set; }
        public string TopGroupId { get; set; }
        public string TopGroupTitle { get; set; }
        public int TopId { get; set; }
        public string TopTitle { get; set; }
        public int SideIndex { get; set; }
        public int SideDisplayOrder { get; set; }
        public string SideGroupId { get; set; }
        public string SideGroupTitle { get; set; }
        public int SideId { get; set; }
        public string SideTitle { get; set; }
        public int AggregateValue { get; set; }
        public int GroupBaseCount { get; set; }
        public int ActualCount { get; set; }
    }
    public class BasicSingleCrosstabBase
    {
        public int SideIndex { get; set; }
        public int TopId { get; set; }
        public int GroupBaseCount { get; set; }
    }

    public class FieldAnalytics
    {
        public int SampleId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public int BaseCount { get; set; }
    }
    public class FieldAnalyticsReport
    {
        public int FieldId { get; set; }
        public int? SectionID { get; set; }
        public string FieldTitle { get; set; }
        public List<FieldAnalytics> FieldAnalytics { get; set; }
    }
}