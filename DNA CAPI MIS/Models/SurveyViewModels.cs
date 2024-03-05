#region Using
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DNA_CAPI_MIS.Models;

using System.Linq;
using System.Net;
using DNA_CAPI_MIS.DAL;

using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System;

#endregion

namespace DNA_CAPI_MIS.Models
{


    public class SurveyDropDown
    {
        public int Value { get; set; }
        public string Name { get; set; }

    }


    public class GraphData
    {
        public int y { get; set; }
        public string name { get; set; }
    }

    public class NationalRegionalSummaryData
    {
        public double score { get; set; }
        public double totalScore { get; set; }
    }

    public class GraphDataPercentage
    {
        public double y { get; set; }
        public string name { get; set; }
    }

    public class GraphDataWithId
    {
        public int y { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }


    public class GraphDataWithIdPercentage
    {
        public double y { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public bool intenal { get; set; }
    }

    public class PieData
    {
        public double y { get; set; }
        public string name { get; set; }
    }

    public class PieDataTotal
    {
        public double y { get; set; }
        public string name { get; set; }
        public double totl { get; set; }
        public double MarkedTotal { get; set; }
        public int Surveycount { get; set; }
    }

    public class GetData
    {
        public int id { get; set; }

        public string name { get; set; }

        public double? Points { get; set; }

        public int PointsFrom { get; set; }
    }


    public class SurveyDetails
    {
        public double? Score { get; set; }
        public int id { get; set; }
        public int Pid { get; set; }
        public int? ParentFieldID { get; set; }
        public string FieldType { get; set; }
        public string name { get; set; }
        public string Section_name { get; set; }
        public string Answer { get; set; }
        public string options { get; set; }
        public int? Marked { get; set; }
        public int fsDisplayOrder { get; set; }
        public int pfDisplayOrder { get; set; }
        public string Dealername { get; set; }
        public int? AnswerId { get; set; }
    }

    public class SurveyDetailsAllDealer
    {
        public int id { get; set; }
        public int Pid { get; set; }
        public int? ParentFieldID { get; set; }
        public string FieldType { get; set; }
        public string Section_name { get; set; }
        public string Answer { get; set; }
        public List<DetailsDealersSurvey> DealerLst { get; set; }
    }

    public class DetailsDealersSurvey
    {
        public string dealername { get; set; }
        public string name { get; set; }
        public double? Score { get; set; }
        public string options { get; set; }
        public int Marked { get; set; }

    }

    public class SqlQuriesDashboard
    {
        #region UnusedQueries
        public readonly string NationalStatusPie = @"Select CR.Name name,
    		  Convert(float,(SELECT SUM(SubSum.BaseCount) y from 
            	(SELECT mp.FieldID AS FieldID, 
            		(SELECT COUNT(*) FROM surveydata msd 
            		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            		AND msd.sbjnum IN 
            			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            			WHERE ISNULL(s.Test, 0) = 0 
						and sd.FieldId = 49756 AND sd.FieldValue in 
						(Select CB.ID from RDXCustomerRegion RCR
						join City C on C.ID=RCR.CityID
						join CustomerBranch CB on CB.CityID=C.ID
						where RCR.CustomerRegionID=CR.ID
						) 
						GROUP BY s.sbjnum ) 
						
									ANd msd.sbjnum IN	--Quarter
            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            				WHERE ISNULL(s.Test, 0) = 0 
							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
							GROUP BY s.sbjnum ) 
						) BaseCount 
            	FROM projectfieldsample mp
				inner join ProjectField pf on pf.ID=mp.FieldID
				 WHERE pf.ProjectID= {0} ) AS SubSum  )) y
			from 
			CustomerRegion CR 
			";


        public readonly string MetroCitySummary = @"Select C.name name,
    		  (SELECT SUM(SubSum.BaseCount) y from 
            	(SELECT mp.FieldID AS FieldID, 
            		(SELECT COUNT(*) FROM surveydata msd 
            		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            		AND msd.sbjnum IN 
            			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            			WHERE ISNULL(s.Test, 0) = 0 
						and sd.FieldId = 49756 
						AND sd.FieldValue in (select CB.ID from CustomerBranch CB where CB.CityID=C.ID) 
						GROUP BY s.sbjnum )
									ANd msd.sbjnum IN	--Quarter
            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            				WHERE ISNULL(s.Test, 0) = 0 
							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
							GROUP BY s.sbjnum ) 
						
						
						 ) BaseCount 
            	FROM projectfieldsample mp
									inner join ProjectField pf on pf.ID=mp.FieldID
				 WHERE pf.ProjectID={0} ) AS SubSum  ) y,C.ID id
			from City C 
			where C.ID in (1,5558,2071)

			union

Select total.name,SUM( total.y) y ,0 id from
(Select 'Others' name,
    		  (SELECT SUM(SubSum.BaseCount) y from 
            	(SELECT mp.FieldID AS FieldID, 
            		(SELECT COUNT(*) FROM surveydata msd 
            		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            		AND msd.sbjnum IN 
            			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            			WHERE ISNULL(s.Test, 0) = 0 
						and sd.FieldId = 49756 
						AND sd.FieldValue in (select CB.ID from CustomerBranch CB where CB.CityID=C.ID) 
						GROUP BY s.sbjnum )
						
									ANd msd.sbjnum IN	--Quarter
            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            				WHERE ISNULL(s.Test, 0) = 0 
							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
							GROUP BY s.sbjnum ) 
						
						
						 ) BaseCount 
            	FROM projectfieldsample mp
				inner join ProjectField pf on pf.ID=mp.FieldID
				 WHERE pf.ProjectID={0}) AS SubSum  ) y
			from City C 
			where C.ID not in (1,5558,2071))total		
			group by name


";




        public readonly string CitiesDetail = @"	Select Cb.Name name,
    		  (SELECT SUM(SubSum.BaseCount) y from 
            	(SELECT mp.FieldID AS FieldID, 
            		(SELECT COUNT(*) FROM surveydata msd 
            		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            		AND msd.sbjnum IN 
            			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            			WHERE ISNULL(s.Test, 0) = 0 
						and sd.FieldId = 49756 AND sd.FieldValue = Convert(nvarchar, Cb.ID) 
						GROUP BY s.sbjnum ) 

									ANd msd.sbjnum IN	--Quarter
            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            				WHERE ISNULL(s.Test, 0) = 0 
							and sd.FieldId = 49757 AND sd.FieldValue IN ({2})
							GROUP BY s.sbjnum ) 
						) BaseCount 
            	FROM projectfieldsample mp
				inner join ProjectField pf on pf.ID=mp.FieldID
				 WHERE pf.ProjectID={0}  
				) AS SubSum            
			) y
			from CustomerBranch Cb where Cb.CityID= {1}";

        public readonly string OtherCitiesDetail = @"			Select Cb.Name name,
    		  (SELECT SUM(SubSum.BaseCount) y from 
            	(SELECT mp.FieldID AS FieldID, 
            		(SELECT COUNT(*) FROM surveydata msd 
            		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            		AND msd.sbjnum IN 
            			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            			WHERE ISNULL(s.Test, 0) = 0 
						and sd.FieldId = 49756 AND sd.FieldValue = Convert(nvarchar, Cb.ID) 

									ANd msd.sbjnum IN	--Quarter
            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            				WHERE ISNULL(s.Test, 0) = 0 
							and sd.FieldId = 49757 AND sd.FieldValue IN ({2})
							GROUP BY s.sbjnum ) 
						
						GROUP BY s.sbjnum ) ) BaseCount 
            	FROM projectfieldsample mp
					inner join ProjectField pf on pf.ID=mp.FieldID
				 WHERE pf.ProjectID={0}  ) AS SubSum  
            ) y
			from CustomerBranch Cb where Cb.CityID not in ( {1} )";



        public readonly string nationalCustomerSectionPie = @"
                   with cte (y,name) as ( SELECT SUM(SubSum.BaseCount) as y, pfs.Name as name from 
            	(SELECT mp.FieldID AS FieldID, 
            		(SELECT COUNT(*) FROM surveydata msd 
            		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            		AND 
	AND msd.sbjnum IN	--Quarter
            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            				WHERE ISNULL(s.Test, 0) = 0 
							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
							GROUP BY s.sbjnum )
) BaseCount 
            	FROM projectfieldsample mp) AS SubSum  
            INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
            INNER JOIN ProjectFieldSection pfs on pfs.ID = pf.SectionID
            WHERE pf.ProjectID={0} and pfs.ID not in (1208,1209)
            GROUP BY pfs.ID,pfs.Name)   

			select (CAST(cte.y AS float) / CAST((select SUM(cte.y) from cte) as FLOAT))*100 as y,cte.name as name from cte
            ";



        public readonly string Top5NationalCustomerSection = @"SELECT Top 5 SUM(SubSum.BaseCount) as y, pf.Title as name from 
            	(SELECT mp.FieldID AS FieldID, 
            		(SELECT COUNT(*) FROM surveydata msd 
            		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            		 
	AND msd.sbjnum IN	--Quarter
            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            				WHERE ISNULL(s.Test, 0) = 0 
							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
							GROUP BY s.sbjnum ) 
) BaseCount 
            	FROM projectfieldsample mp) AS SubSum  
            INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
            WHERE pf.ProjectID={0} 
			group by pf.id,pf.Title
			order by y {2}";


        public string DealerCustomerSectionPie = @"		  SELECT  Cast( SUM(SubSum.BaseCount) as float) 'y', pfs.name as name,pfs.ID from 
            	(SELECT mp.FieldID AS FieldID, 
            		(SELECT COUNT(*) FROM surveydata msd 
            		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            		AND msd.sbjnum IN 
            			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            			WHERE ISNULL(s.Test, 0) = 0 
						and sd.FieldId = 49756 AND sd.FieldValue = '{2}'
						GROUP BY s.sbjnum ) 
AND msd.sbjnum IN	--Quarter
            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            				WHERE ISNULL(s.Test, 0) = 0 
							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
							GROUP BY s.sbjnum ) 
) BaseCount 
            	FROM projectfieldsample mp) AS SubSum  
            INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
            INNER JOIN ProjectFieldSection pfs on pfs.ID = pf.SectionID
            WHERE pf.ProjectID={0} and pfs.ID not in (1208,1209)
			GROUP BY pfs.name ,pfs.ID";


        public readonly string Top5DealerCustomerSection = @"		  SELECT Top 5 SUM(SubSum.BaseCount) 'y', pf.title as name,pf.ID from 
            	(SELECT mp.FieldID AS FieldID, 
            		(SELECT COUNT(*) FROM surveydata msd 
            		WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            		AND msd.sbjnum IN 
            			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            			WHERE ISNULL(s.Test, 0) = 0 
						and sd.FieldId = 49756 AND sd.FieldValue = '{2}'
						GROUP BY s.sbjnum )
AND msd.sbjnum IN	--Quarter
            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            				WHERE ISNULL(s.Test, 0) = 0 
							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
							GROUP BY s.sbjnum ) 
) BaseCount 
            	FROM projectfieldsample mp) AS SubSum  
            INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
            INNER JOIN ProjectFieldSection pfs on pfs.ID = pf.SectionID
            WHERE pf.ProjectID={0} 
			GROUP BY pf.title ,pf.ID
            Order by y {3}";

        public readonly string NationalBrachComp = @"
with cte as (SELECT s.sbjnum,sd.FieldId,sd.FieldValue FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            				WHERE ISNULL(s.Test, 0) = 0 
							and sd.FieldId = 49756 OR  sd.FieldId = 49757
							GROUP BY s.sbjnum,sd.FieldId ,sd.FieldValue
							)

	Select Cb.Name name,Cb.id id,
    		(SELECT round((CONVERT(float, SUM(SubSum.BaseCount))/Count(SubSum.BaseCount))*100,1)y from 
            	(SELECT mp.FieldID AS FieldID, 
            		(SELECT COUNT(*) FROM surveydata msd 
            			WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            		and 
					msd.Sbjnum IN (select cte.Sbjnum from cte where (cte.FieldId=49757 and cte.FieldValue in ( {1}))    )
					and 
					msd.Sbjnum IN (select cte.Sbjnum from cte where (cte.FieldId=49756 and cte.FieldValue = Convert(nvarchar, Cb.ID) ))
					) BaseCount 
            	FROM projectfieldsample mp
					inner join ProjectField pf on pf.ID=mp.FieldID
				 WHERE pf.ProjectID={0}  ) AS SubSum  
            ) y
			from CustomerBranch Cb			
			 where CB.IsActive=1

";

        public readonly string RegionBranchComp = @"
with cte as (SELECT s.sbjnum,sd.FieldId,sd.FieldValue FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            				WHERE ISNULL(s.Test, 0) = 0 
							and sd.FieldId = 49756 OR  sd.FieldId = 49757
							GROUP BY s.sbjnum,sd.FieldId ,sd.FieldValue
							)

	Select Cb.Name name,Cb.id id,
    		(SELECT round((CONVERT(float, SUM(SubSum.BaseCount))/Count(SubSum.BaseCount))*100,1)y from 
            	(SELECT mp.FieldID AS FieldID, 
            		(SELECT COUNT(*) FROM surveydata msd 
            			WHERE msd.fieldid = mp.fieldid AND mp.code IN (SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
            		and 
					msd.Sbjnum IN (select cte.Sbjnum from cte where (cte.FieldId=49757 and cte.FieldValue in ( {1}))    )
					and 
					msd.Sbjnum IN (select cte.Sbjnum from cte where (cte.FieldId=49756 and cte.FieldValue = Convert(nvarchar, Cb.ID) ))
					) BaseCount 
            	FROM projectfieldsample mp
					inner join ProjectField pf on pf.ID=mp.FieldID
				 WHERE pf.ProjectID={0}  ) AS SubSum  
            ) y
			from CustomerBranch Cb
			join City C on Cb.CityID=C.ID
			join RDXCustomerRegion RCR on RCR.CityID=C.ID
			
			 where CB.IsActive=1 and RCR.CustomerRegionID={2}";

        public readonly string intenalDealerDetail = @"			   SELECT  round((CONVERT(float, SUM(SubSum.BaseCount))/Count(SubSum.BaseCount))*100,1) 'y', pfs.Name as name,pfs.ID from 
                         (SELECT mp.FieldID AS FieldID,
                             (SELECT COUNT(*) FROM surveydata msd
                             WHERE msd.fieldid = mp.fieldid AND mp.code IN(SELECT ListMember FROM fnSplitCSV(msd.FieldValue))
                             AND msd.sbjnum IN
                                 (SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum
                                 WHERE ISNULL(s.Test, 0) = 0
                        and sd.FieldId = 49756 AND sd.FieldValue = '{1}'
                        GROUP BY s.sbjnum)
                        ANd msd.sbjnum IN--Quarter
                                     (SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum
                                     WHERE ISNULL(s.Test, 0) = 0
                            and sd.FieldId = 49757 AND sd.FieldValue IN({2})
                            GROUP BY s.sbjnum)
                         ) BaseCount
                         FROM projectfieldsample mp) AS SubSum
                     INNER JOIN ProjectField pf on pf.ID = SubSum.FieldID
                     INNER JOIN ProjectFieldSection pfs on pfs.ID = pf.SectionID
                     WHERE pf.ProjectID= {0} and pfs.ID not in (1208,1209)
        GROUP BY pfs.name ,pfs.ID";

        #endregion



        public static string sqlBranchScoreNational = $@"DECLARE @myTable SurveyIDType

 insert into  @myTable  SELECT distinct msd.sbjnum FROM surveydata msd 
                join ProjectField pf on pf.ID=msd.FieldId
                            		WHERE
                					msd.sbjnum IN	--Quarter
                            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                            				WHERE ISNULL(s.Test, 0) = 0 
                							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})   and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
                							GROUP BY s.sbjnum ) 
                 AND msd.sbjnum IN	--Dealer
                            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                            				WHERE ISNULL(s.Test, 0) = 0 
                							and sd.FieldId = 49756 AND sd.FieldValue IN ( 
select Cb.ID from CustomerBranch Cb
			join City C on Cb.CityID=C.ID
			join RDXCustomerRegion RCR on RCR.CityID=C.ID
			 where CB.IsActive=1 and RCR.CustomerRegionID={2} ) 
  and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
                							GROUP BY s.sbjnum )
                and pf.ProjectID={0}
                GROUP BY msd.sbjnum
				
				SELECT 
   SUM( Convert(float, ISNULL(  CASE WHEN sd.Marked =1 THEN sd.Score ELSE 0 END,0))/10) score,
 Convert(float, case  when SUM(ISNULL(sd.Marked,0)) > 0 then SUM(ISNULL(sd.Marked,0))  else 1 end) totalScore
FROM ProjectField pf 
 INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
 LEFT OUTER JOIN fnMultipleSurveyValues(@myTable ) sd ON
sd.FieldID = pf.ID
 LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
  CASE 
  WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE 
	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
  END) 
  WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' OR (pf.FieldType = 'MLT' AND SampleAnswers.ParentSampleID IS NULL) THEN sd.OptionCode 
  ELSE
  	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
 END
 LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'

WHERE fs.ID not in (1209,1208) and pf.ProjectID={0}";


        public static string sqlBranchScore = $@"DECLARE @myTable SurveyIDType

 insert into  @myTable  SELECT distinct msd.sbjnum FROM surveydata msd 
                join ProjectField pf on pf.ID=msd.FieldId
                            		WHERE
                					msd.sbjnum IN	--Quarter
                            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                            				WHERE ISNULL(s.Test, 0) = 0 
                							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})   and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
                							GROUP BY s.sbjnum )
                 AND msd.sbjnum IN	--Dealer
                            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                            				WHERE ISNULL(s.Test, 0) = 0 
                							and sd.FieldId = 49756 AND sd.FieldValue IN ( {2} )   and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
                							GROUP BY s.sbjnum )
                and pf.ProjectID={0}
                GROUP BY msd.sbjnum
				
				SELECT 
  Round((SUM( Convert(float, ISNULL( sd.Score,0))/10) /
  case  when SUM(ISNULL(sd.Marked,0)) > 0 then SUM(ISNULL(sd.Marked,0))  else 1 end
   )*100,2) y
FROM ProjectField pf 
 INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
 LEFT OUTER JOIN fnMultipleSurveyValues(@myTable ) sd ON
sd.FieldID = pf.ID
 LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
  CASE 
  WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE 
	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
  END) 
  WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' OR (pf.FieldType = 'MLT' AND SampleAnswers.ParentSampleID IS NULL) THEN sd.OptionCode 
  ELSE
  	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
 END
 LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'

WHERE fs.ID not in (1209,1208) and pf.ProjectID={0}";


        public static string sqlFieldSectionScoreTotal = @"DECLARE @myTable SurveyIDType

 insert into  @myTable  SELECT distinct msd.sbjnum FROM surveydata msd 
                join ProjectField pf on pf.ID=msd.FieldId
                            		WHERE
                					msd.sbjnum IN	--Quarter
                            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                            				WHERE ISNULL(s.Test, 0) = 0 
                							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})  and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
                							GROUP BY s.sbjnum ) 
                 AND msd.sbjnum IN	--Dealer
                            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                            				WHERE ISNULL(s.Test, 0) = 0 
                							and sd.FieldId = 49756 AND sd.FieldValue IN ( {2} )  and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
                							GROUP BY s.sbjnum )
                and pf.ProjectID={0}
                GROUP BY msd.sbjnum
				
				SELECT fs.ID,fs.name name,
 Round((SUM( Convert(float, ISNULL(  CASE WHEN sd.Marked =1 THEN sd.Score ELSE 0 END,0))/10) /
  case  when SUM(ISNULL(sd.Marked,0)) > 0 then SUM(ISNULL(sd.Marked,0))  else 1 end
   )*100,1) y,
round( SUM( Convert(float, ISNULL(  CASE WHEN sd.Marked =1 THEN sd.Score ELSE 0 END,0))/10)/ISNULL(NULLIF( (SELECT COUNT(*) FROM @myTable),0),1), 2) totl,
(SELECT COUNT(*) FROM @myTable) Surveycount
FROM ProjectField pf 
 INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
 LEFT OUTER JOIN fnMultipleSurveyValues(@myTable ) sd ON
sd.FieldID = pf.ID
 LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
  CASE 
  WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE 
	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
  END) 
  WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' OR (pf.FieldType = 'MLT' AND SampleAnswers.ParentSampleID IS NULL) THEN sd.OptionCode 
  ELSE
  	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
 END
 LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
WHERE  fs.ID not in (1209,1208) and pf.ProjectID={0}
group by fs.ID,fs.name ";

        public static string sqlFieldSectionScore = @"DECLARE @myTable SurveyIDType

 insert into  @myTable  SELECT distinct msd.sbjnum FROM surveydata msd 
                join ProjectField pf on pf.ID=msd.FieldId
                            		WHERE
                					msd.sbjnum IN	--Quarter
                            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                            				WHERE ISNULL(s.Test, 0) = 0 
                							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})   and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
                							GROUP BY s.sbjnum ) 
                 AND msd.sbjnum IN	--Dealer
                            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                            				WHERE ISNULL(s.Test, 0) = 0 
                							and sd.FieldId = 49756 AND sd.FieldValue IN ( {2} )  and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
                							GROUP BY s.sbjnum )
                and pf.ProjectID={0}
                GROUP BY msd.sbjnum
				
				SELECT fs.ID,fs.name name,
 Round((SUM( Convert(float, ISNULL(  CASE WHEN sd.Marked =1 THEN sd.Score ELSE 0 END,0))/10) /
  case  when SUM(ISNULL(sd.Marked,0)) > 0 then SUM(ISNULL(sd.Marked,0))  else 1 end
   )*100,1) y
FROM ProjectField pf 
 INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
 LEFT OUTER JOIN fnMultipleSurveyValues(@myTable ) sd ON
sd.FieldID = pf.ID
 LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
  CASE 
  WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE 
	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
  END) 
  WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' OR (pf.FieldType = 'MLT' AND SampleAnswers.ParentSampleID IS NULL) THEN sd.OptionCode 
  ELSE
  	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
 END
 LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
WHERE  fs.ID not in (1209,1208) and pf.ProjectID={0}
group by fs.ID,fs.name ";

        string NationalTop5 = $@"DECLARE @myTable SurveyIDType

 insert into  @myTable  SELECT distinct msd.sbjnum FROM surveydata msd 
                join ProjectField pf on pf.ID=msd.FieldId
                            		WHERE
                					msd.sbjnum IN	--Quarter
                            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                            				WHERE ISNULL(s.Test, 0) = 0 
                							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})   and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
                							GROUP BY s.sbjnum ) 
                 AND msd.sbjnum IN	--Dealer
                            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                            				WHERE ISNULL(s.Test, 0) = 0 
                							and sd.FieldId = 49756 AND sd.FieldValue IN ( {2} )    and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
                							GROUP BY s.sbjnum )
                and pf.ProjectID={0}
                GROUP BY msd.sbjnum
				
				SELECT Top 5 pf.id id,fs.Name+  '/' + pf.Title name,
 Round((SUM( Convert(float, ISNULL(  CASE WHEN sd.Marked =1 THEN sd.Score ELSE 0 END ,0))/10) /
  case  when SUM(ISNULL(sd.Marked,0)) > 0 then SUM(ISNULL(sd.Marked,0))  else 1 end
   )*100,1) y
FROM ProjectField pf 
 INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
INNER JOIN ProjectField ppf on ppf.ID=pf.ParentFieldID
 LEFT OUTER JOIN fnMultipleSurveyValues(@myTable ) sd ON
sd.FieldID = pf.ID
 LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
  CASE 
  WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE 
	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
  END) 
  WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' OR (pf.FieldType = 'MLT' AND SampleAnswers.ParentSampleID IS NULL) THEN sd.OptionCode 
  ELSE
  	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
 END
 LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
WHERE pf.ProjectID={7120} and pf.IsMarked is not null
group by pf.id,pf.Title,fs.Name
 Order by y ";


        public List<GraphDataPercentage> SectionIdGrph(int ProjectId, string Quarterids, int? dealerid, string username)
        {
            List<GraphDataPercentage> GrphData = new List<GraphDataPercentage>();
            List<GetData> Datalst = new List<GetData>();
            ProjectContext db = new ProjectContext();

            List<int> sbjIds = new List<int>();
            string sql = "";


            if (dealerid == null)
            {
                string dealers = "'" + string.Join("','", db.CustomerBranch.Where(x => x.IsActive == true).Select(x => x.ID).ToList()) + "'";
                sql = sqlFieldSectionScore;

                sql = string.Format(sql, ProjectId.ToString(), Quarterids, dealers, username);
            }
            else
            {
                sql = sqlFieldSectionScore;


                sql = string.Format(sql, ProjectId.ToString(), Quarterids, "'" + dealerid.ToString() + "'", username);

            }
            GrphData = db.Database.SqlQuery<GraphDataPercentage>(sql).ToList();

            #region
            //            if (dealerid == null)
            //            {
            //                string dealers = "'" + string.Join("','", db.CustomerBranch.Where(x => x.IsActive == true).Select(x => x.ID).ToList()) + "'";

            //                sql = @"
            //SELECT msd.sbjnum FROM surveydata msd 
            //join ProjectField pf on pf.ID=msd.FieldId
            //            		WHERE
            //					msd.sbjnum IN	--Quarter
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
            //							GROUP BY s.sbjnum ) 
            // AND msd.sbjnum IN	--Dealer
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49756 AND sd.FieldValue IN ( {2} )
            //							GROUP BY s.sbjnum )
            //and pf.ProjectID={0}
            //GROUP BY msd.sbjnum";
            //                sbjIds = db.Database.SqlQuery<int>(string.Format(sql, ProjectId.ToString(), Quarterids,dealers)).ToList();

            //            }
            //            else
            //            {
            //                sql = @"
            //SELECT msd.sbjnum FROM surveydata msd 
            //join ProjectField pf on pf.ID=msd.FieldId
            //            		WHERE
            //					msd.sbjnum IN	--Quarter
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
            //							GROUP BY s.sbjnum ) 
            //		AND msd.sbjnum IN	--Dealer
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49756 AND sd.FieldValue = {2}
            //							GROUP BY s.sbjnum )
            //and pf.ProjectID={0}
            //GROUP BY msd.sbjnum";
            //                sbjIds = db.Database.SqlQuery<int>(string.Format(sql, ProjectId.ToString(), Quarterids, dealerid)).ToList();

            //            }
            //            foreach (var item in sbjIds)
            //            {
            //                List<GetData> DatalstTemp = new List<GetData>();
            //                sql = @"SELECT  fs.id id,fs.name name, SUM(Convert(float, SampleAnswers.Points * ISNULL(sd.Score, 0)) /10) Points , SUM(CASE WHEN sd.Score IS NULL THEN 0 ELSE 1 END) PointsFrom 
            //FROM
            // ProjectField pf 
            //    INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
            //LEFT OUTER JOIN fnSurveyValues({0}) sd ON sd.FieldID = pf.ID
            // LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
            //  CASE 
            //   WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE sd.Answer END) 
            //   WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' THEN sd.OptionCode 
            //   ELSE sd.Answer END
            // LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
            //    WHERE 
            //pf.ProjectID={1} and
            //fs.id not in (1208,1209)
            //group by fs.id,fs.name
            //";
            //                DatalstTemp = db.Database.SqlQuery<GetData>(string.Format(sql, item.ToString(), ProjectId.ToString())).ToList();

            //                Datalst = Datalst.Concat(DatalstTemp).ToList();

            //            }

            //            var result = Datalst.GroupBy(x => x.id).Select(i => i.ToList()).ToList();
            //            Datalst = Datalst.GroupBy(x => new { x.id, x.name }).Select(c => new GetData
            //            {
            //                id = c.First().id,
            //                name = c.First().name,
            //                Points = c.Sum(s => s.Points),
            //                PointsFrom = c.Sum(s => s.PointsFrom)
            //            }).ToList();

            //            foreach (var item in Datalst)
            //            {
            //                GraphDataPercentage GrphdataP = new Models.GraphDataPercentage();
            //                GrphdataP.name = item.name;
            //                GrphdataP.y = Math.Round(Convert.ToDouble(item.Points / (item.PointsFrom * 10)) * 100);
            //                GrphData.Add(GrphdataP);
            //            }
            #endregion


            return GrphData;

        }

        public List<GraphDataPercentage> NationalRegionalLevelPie(int Projectid, string Quarterids, string username)
        {
            string sql = "", dealers = "";
            double totalScore = 0, Score = 0;
            List<SurveyData> sd = new List<SurveyData>();
            List<GetData> Datalst = new List<GetData>();
            ProjectContext db = new ProjectContext();
            List<CustomerBranch> custBranh;
            List<NationalRegionalSummaryData> NatRegSumLst = new List<NationalRegionalSummaryData>();

            List<GraphDataPercentage> BarDatalst = new List<GraphDataPercentage>();
            GraphDataPercentage gdatap = new GraphDataPercentage();
            List<int> Cityids = new List<int> { 1, 5558, 2071 };
            //List<City> Cities = db.City.Where(x => Cityids.Contains(x.ID)).TSmoList();
            List<CustomerRegion> CustRegion = db.CustomerRegion.ToList();
            foreach (var Region in CustRegion)
            {
                //                sql = @"select Cb.* from CustomerBranch Cb
                //			join City C on Cb.CityID=C.ID
                //			join RDXCustomerRegion RCR on RCR.CityID=C.ID
                //			 where CB.IsActive=1 and RCR.CustomerRegionID={0}
                //";

                //                dealers = "'" + string.Join("','", db.Database.SqlQuery<CustomerBranch>(string.Format(sql, Region.ID)).Select(x => x.ID).ToList()) + "'";

                sql = sqlBranchScoreNational;

                sql = string.Format(sql, Projectid.ToString(), Quarterids, Region.ID, username);
                NationalRegionalSummaryData NatRegSum = new NationalRegionalSummaryData();

                //PieData piData = new PieData();
                gdatap = new GraphDataPercentage();
                NatRegSum = db.Database.SqlQuery<NationalRegionalSummaryData>(sql).ToList()[0];
                gdatap.name = Region.Name;
                //gdatap.y = (NatRegSum.score / NatRegSum.totalScore) * 100;
                totalScore += NatRegSum.totalScore;
                Score += NatRegSum.score;

                NatRegSumLst.Add(NatRegSum);
                BarDatalst.Add(gdatap);

                //piData = db.Database.SqlQuery<PieData>(sql).ToList()[0];
                //piData.name = Region.Name;
                //PiDatalst.Add(piData);
            }
            int ind = 0;
            foreach (var item in NatRegSumLst)
            {
                BarDatalst[ind].y = (item.score / totalScore) * 100;
                ind++;
            }

            gdatap = new GraphDataPercentage();
            gdatap.name = "National";
            gdatap.y = (Score / totalScore) * 100;
            BarDatalst.Add(gdatap);
            return BarDatalst;



        }


        public List<PieDataTotal> SectionIdPie(int ProjectId, string Quarterids, int? dealerid, string username)
        {
            List<PieDataTotal> PieData = new List<PieDataTotal>();
            List<GetData> Datalst = new List<GetData>();
            ProjectContext db = new ProjectContext();

            List<int> sbjIds = new List<int>();
            string sql = "";

            sql = sqlFieldSectionScoreTotal;



            if (dealerid == null)
            {
                string dealers = "'" + string.Join("','", db.CustomerBranch.Where(x => x.IsActive == true).Select(x => x.ID).ToList()) + "'";
                //PieData = db.Database.SqlQuery<PieDataTotal>(string.Format(sql, ProjectId.ToString(), Quarterids, dealers, username)).ToList();
            }
            else
            {
                PieData = db.Database.SqlQuery<PieDataTotal>(string.Format(sql, ProjectId.ToString(), Quarterids, "'" + dealerid.ToString() + "'", username)).ToList();
            }

            #region
            //            if (dealerid == null)
            //            {
            //                string dealers = "'" + string.Join("','", db.CustomerBranch.Where(x => x.IsActive == true).Select(x => x.ID).ToList()) + "'";

            //                sql = @"
            //SELECT msd.sbjnum FROM surveydata msd 
            //join ProjectField pf on pf.ID=msd.FieldId
            //            		WHERE
            //					msd.sbjnum IN	--Quarter
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
            //							GROUP BY s.sbjnum ) 
            // AND msd.sbjnum IN	--Dealer
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49756 AND sd.FieldValue IN ( {2} )
            //							GROUP BY s.sbjnum )
            //and pf.ProjectID={0}
            //GROUP BY msd.sbjnum";
            //                sbjIds = db.Database.SqlQuery<int>(string.Format(sql, ProjectId.ToString(), Quarterids,dealers)).ToList();

            //            }
            //            else
            //            {
            //                sql = @"
            //SELECT msd.sbjnum FROM surveydata msd 
            //join ProjectField pf on pf.ID=msd.FieldId
            //            		WHERE
            //					msd.sbjnum IN	--Quarter
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
            //							GROUP BY s.sbjnum ) 
            //		AND msd.sbjnum IN	--Dealer
            //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //            				WHERE ISNULL(s.Test, 0) = 0 
            //							and sd.FieldId = 49756 AND sd.FieldValue = {2}
            //							GROUP BY s.sbjnum )
            //and pf.ProjectID={0}
            //GROUP BY msd.sbjnum";
            //                sbjIds = db.Database.SqlQuery<int>(string.Format(sql, ProjectId.ToString(), Quarterids, dealerid)).ToList();

            //            }
            //            foreach (var item in sbjIds)
            //            {
            //                List<GetData> DatalstTemp = new List<GetData>();
            //                sql = @"SELECT  fs.id id,fs.name name, SUM(Convert(float, SampleAnswers.Points * ISNULL(sd.Score, 0)) /10) Points , SUM(CASE WHEN sd.Score IS NULL THEN 0 ELSE 1 END) PointsFrom 
            //FROM
            // ProjectField pf 
            //    INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
            //LEFT OUTER JOIN fnSurveyValues({0}) sd ON sd.FieldID = pf.ID
            // LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
            //  CASE 
            //   WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE sd.Answer END) 
            //   WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' THEN sd.OptionCode 
            //   ELSE sd.Answer END
            // LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
            //    WHERE 
            //pf.ProjectID={1} and
            //fs.id not in (1208,1209)
            //group by fs.id,fs.name
            //";
            //                DatalstTemp = db.Database.SqlQuery<GetData>(string.Format(sql, item.ToString(), ProjectId.ToString())).ToList();

            //                Datalst = Datalst.Concat(DatalstTemp).ToList();

            //            }

            //            var result = Datalst.GroupBy(x => x.id).Select(i => i.ToList()).ToList();
            //            Datalst = Datalst.GroupBy(x => new { x.id, x.name }).Select(c => new GetData
            //            {
            //                id = c.First().id,
            //                name = c.First().name,
            //                Points = c.Sum(s => s.Points),
            //                PointsFrom = c.Sum(s => s.PointsFrom)
            //            }).ToList();

            //            foreach (var item in Datalst)
            //            {
            //                PieData Pidata = new Models.PieData();
            //                Pidata.name = item.name;
            //                if (item.PointsFrom != 0)
            //                {
            //                    Pidata.y = Convert.ToDouble(item.Points / (item.PointsFrom * 10)) * 100;

            //                }
            //                PieData.Add(Pidata);
            //            }
            #endregion

            return PieData;

        }


        public List<GraphDataPercentage> NationTop5(int ProjectId, string Quarterids, string orderby, int? dealerid, string username)
        {
            List<GraphDataPercentage> GraphData = new List<GraphDataPercentage>();
            List<GetData> Datalst = new List<GetData>();
            ProjectContext db = new ProjectContext();

            List<int> sbjIds = new List<int>();
            string sql = "";
            sql = NationalTop5;

            //            sql = @"DECLARE @myTable SurveyIDType
            //
            // insert into  @myTable  SELECT distinct msd.sbjnum FROM surveydata msd 
            //                join ProjectField pf on pf.ID=msd.FieldId
            //                            		WHERE
            //                					msd.sbjnum IN	--Quarter
            //                            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //                            				WHERE ISNULL(s.Test, 0) = 0 
            //                							and sd.FieldId = 49757 AND sd.FieldValue IN ({1}) and s.SurveyorName in ( {4} ) and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
            //                							GROUP BY s.sbjnum ) 
            //                 AND msd.sbjnum IN	--Dealer
            //                            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            //                            				WHERE ISNULL(s.Test, 0) = 0 
            //                							and sd.FieldId = 49756 AND sd.FieldValue IN ( {2} ) and s.SurveyorName in ({4})  and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
            //                							GROUP BY s.sbjnum )
            //                and pf.ProjectID={0}
            //                GROUP BY msd.sbjnum
            //				
            //				SELECT Top 5 pf.id id,fs.Name+  '/' + pf.Title name,
            // Round((SUM( Convert(float, ISNULL(  CASE WHEN sd.Marked =1 THEN sd.Score ELSE 0 END ,0))/10) /
            //  case  when SUM(ISNULL(sd.Marked,0)) > 0 then SUM(ISNULL(sd.Marked,0))  else 1 end
            //   )*100,1) y
            //FROM ProjectField pf 
            // INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
            //INNER JOIN ProjectField ppf on ppf.ID=pf.ParentFieldID
            // LEFT OUTER JOIN fnMultipleSurveyValues(@myTable ) sd ON
            //sd.FieldID = pf.ID
            // LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
            //  CASE 
            //  WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE 
            //	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
            //  END) 
            //  WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' OR (pf.FieldType = 'MLT' AND SampleAnswers.ParentSampleID IS NULL) THEN sd.OptionCode 
            //  ELSE
            //  	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
            // END
            // LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
            //WHERE pf.ProjectID={0} and pf.IsMarked is not null
            //group by pf.id,pf.Title,fs.Name
            // Order by y {3}";



            if (dealerid == null)
            {
                string dealers = "'" + string.Join("','", db.CustomerBranch.Where(x => x.IsActive == true).Select(x => x.ID).ToList()) + "'";

                GraphData = db.Database.SqlQuery<GraphDataPercentage>(string.Format(sql, ProjectId.ToString(), Quarterids, dealers, orderby, username)).ToList();

            }
            else
            {
                GraphData = db.Database.SqlQuery<GraphDataPercentage>(string.Format(sql, ProjectId.ToString(), Quarterids, "'" + dealerid.ToString() + "'", orderby, username)).ToList();

            }
            return GraphData;

        }

        public List<GraphDataWithIdPercentage> CityLevel(int Projectid, string Quarterids, int? CitySearch, string username)
        {
            string sql = "";
            List<SurveyData> sd = new List<SurveyData>();
            List<GetData> Datalst = new List<GetData>();
            ProjectContext db = new ProjectContext();
            List<CustomerBranch> custBranh;
            string dealers = "";
            List<GraphDataWithIdPercentage> gDataWp = new List<GraphDataWithIdPercentage>();




            if (CitySearch == null)
            {
                List<int> Cityids = new List<int> { 1, 5558, 2071 };
                List<City> Cities = db.City.Where(x => Cityids.Contains(x.ID)).ToList();
                foreach (var City in Cities)
                {
                    custBranh = db.CustomerBranch.Where(x => x.CityID == City.ID && x.IsActive == true).ToList();
                    //dealers

                    dealers = "'" + string.Join("','", db.CustomerBranch.Where(x => x.CityID == City.ID && x.IsActive == true).Select(x => x.ID).ToList()) + "'";

                    sql = sqlBranchScore;
                    sql = string.Format(sql, Projectid.ToString(), Quarterids, dealers, username);

                    GraphDataWithIdPercentage GdatawithIP = new GraphDataWithIdPercentage();
                    GdatawithIP = db.Database.SqlQuery<GraphDataWithIdPercentage>(sql).ToList()[0];
                    GdatawithIP.id = City.ID;
                    GdatawithIP.name = City.Name;
                    gDataWp.Add(GdatawithIP);
                }


                //custBranh = db.CustomerBranch.Where(x => !Cityids.Contains((int)x.CityID) && x.IsActive == true).ToList();
                dealers = "'" + string.Join("','", db.CustomerBranch.Where(x => !Cityids.Contains((int)x.CityID) && x.IsActive == true).Select(x => x.ID).ToList()) + "'";
                sql = sqlBranchScore;
                sql = string.Format(sql, Projectid.ToString(), Quarterids, dealers, username);

                GraphDataWithIdPercentage GdatawithIP1 = db.Database.SqlQuery<GraphDataWithIdPercentage>(sql).ToList()[0];
                GdatawithIP1.id = 0;
                GdatawithIP1.name = "Other Cities";
                gDataWp.Add(GdatawithIP1);


                #region
                //double? value = 0, Score = 0;
                //foreach (var item in custBranh)
                //{

                //List<int> sbjIds = new List<int>();


                //                    sql = @"SELECT msd.sbjnum FROM surveydata msd 
                //join ProjectField pf on pf.ID=msd.FieldId
                //            		WHERE
                //					msd.sbjnum IN	--Quarter
                //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                //            				WHERE ISNULL(s.Test, 0) = 0 
                //							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
                //							GROUP BY s.sbjnum ) 
                //		AND msd.sbjnum IN	--Dealer
                //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                //            				WHERE ISNULL(s.Test, 0) = 0 
                //							and sd.FieldId = 49756 AND sd.FieldValue = {2} 
                //							GROUP BY s.sbjnum  )
                //and pf.ProjectID={0}
                //GROUP BY msd.sbjnum";
                //                    sbjIds = db.Database.SqlQuery<int>(string.Format(sql, Projectid.ToString(), Quarterids, item.ID)).ToList();


                //                    foreach (var item1 in sbjIds)
                //                    {
                //                        List<GetData> DatalstTemp = new List<GetData>();

                //                        sql = @"SELECT   SUM(Convert(float, SampleAnswers.Points * ISNULL(sd.Score, 0)) /10) Points , SUM(CASE WHEN sd.Score IS NULL THEN 0 ELSE 1 END) PointsFrom 
                //FROM
                // ProjectField pf 
                //    INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
                //LEFT OUTER JOIN fnSurveyValues({0}) sd ON sd.FieldID = pf.ID
                // LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
                //  CASE 
                //   WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE sd.Answer END) 
                //   WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' THEN sd.OptionCode 
                //   ELSE sd.Answer END
                // LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
                //    WHERE 
                //pf.ProjectID={1} and
                //fs.id not in (1208,1209)";
                //                        DatalstTemp = db.Database.SqlQuery<GetData>(string.Format(sql, item1.ToString(), Projectid.ToString())).ToList();
                //                        value += DatalstTemp[0].Points ?? 0;
                //                        Score += DatalstTemp[0].PointsFrom * 10;
                //                    }


                //                }
                //                if (Score != 0)
                //                {
                //                    value = Math.Round(Convert.ToDouble(value / Score) * 100, 2);

                //                }
                //                else
                //                {
                //                    value = 0;

                //                }

                //    sql = string.Format(sql, Projectid.ToString(), Quarterids, dealers);

                //    GraphDataWithIdPercentage GdatawithIP1 = new GraphDataWithIdPercentage();
                //GdatawithIP1.y = Convert.ToDouble(value);
                //GdatawithIP1.id = 0;
                //GdatawithIP1.name = "Metro";
                //gDataWp.Add(GdatawithIP1);
                #endregion
            }
            else
            {
                if (CitySearch > 0)
                {


                    sql = sqlBranchScore;

                    List<CustomerBranch> CustomerBrnch = db.CustomerBranch.Where(x => x.CityID == CitySearch && x.IsActive == true).ToList();
                    foreach (var CustB in CustomerBrnch)
                    {
                        //dealers

                        #region
                        //                        double? value1 = 0, Score1 = 0;
                        //                        custBranh = db.CustomerBranch.Where(x => x.CityID == City.ID && x.IsActive == true).ToList();

                        //                        foreach (var item in custBranh)
                        //                        {
                        //                            List<int> sbjIds = new List<int>();


                        //                            sql = @"SELECT msd.sbjnum FROM surveydata msd 
                        //join ProjectField pf on pf.ID=msd.FieldId
                        //            		WHERE
                        //					msd.sbjnum IN	--Quarter
                        //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                        //            				WHERE ISNULL(s.Test, 0) = 0 
                        //							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
                        //							GROUP BY s.sbjnum ) 
                        //		AND msd.sbjnum IN	--Dealer
                        //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                        //            				WHERE ISNULL(s.Test, 0) = 0 
                        //							and sd.FieldId = 49756 AND sd.FieldValue = {2} 
                        //							GROUP BY s.sbjnum  )
                        //and pf.ProjectID={0}
                        //GROUP BY msd.sbjnum";
                        //                            sbjIds = db.Database.SqlQuery<int>(string.Format(sql, Projectid.ToString(), Quarterids, item.ID)).ToList();


                        //                            foreach (var item1 in sbjIds)
                        //                            {
                        //                                List<GetData> DatalstTemp = new List<GetData>();

                        //                                sql = @"SELECT   SUM(Convert(float, SampleAnswers.Points * ISNULL(sd.Score, 0)) /10) Points , SUM(CASE WHEN sd.Score IS NULL THEN 0 ELSE 1 END) PointsFrom 
                        //FROM
                        // ProjectField pf 
                        //    INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
                        //LEFT OUTER JOIN fnSurveyValues({0}) sd ON sd.FieldID = pf.ID
                        // LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
                        //  CASE 
                        //   WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE sd.Answer END) 
                        //   WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' THEN sd.OptionCode 
                        //   ELSE sd.Answer END
                        // LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
                        //    WHERE 
                        //pf.ProjectID={1} and
                        //fs.id not in (1208,1209)";
                        //                                DatalstTemp = db.Database.SqlQuery<GetData>(string.Format(sql, item1.ToString(), Projectid.ToString())).ToList();
                        //                                value1 += DatalstTemp[0].Points ?? 0;
                        //                                Score1 += DatalstTemp[0].PointsFrom * 10;
                        //                            }


                        //                        }
                        //                        if (Score1 != 0)
                        //                        {
                        //                            value1 = Math.Round(Convert.ToDouble(value1 / Score1) * 100, 2);

                        //                        }
                        //                        else
                        //                        {
                        //                            value1 = 0;

                        //                        }
                        #endregion
                        string sqltoRun = "";

                        sqltoRun = string.Format(sql, Projectid.ToString(), Quarterids, "'" + CustB.ID.ToString() + "'", username);

                        GraphDataWithIdPercentage GdatawithIP = new GraphDataWithIdPercentage();
                        GdatawithIP = db.Database.SqlQuery<GraphDataWithIdPercentage>(sqltoRun).ToList()[0];
                        GdatawithIP.id = CustB.ID;
                        GdatawithIP.name = CustB.Name;
                        gDataWp.Add(GdatawithIP);
                    }
                }
                else
                {
                    List<int> Cityids = new List<int> { 1, 5558, 2071 };
                    custBranh = db.CustomerBranch.Where(x => !Cityids.Contains((int)x.CityID) && x.IsActive == true).ToList();
                    sql = sqlBranchScore;
                    //double? value = 0, Score = 0;
                    foreach (var item in custBranh)
                    {
                        #region
                        //                        List<int> sbjIds = new List<int>();


                        //                        sql = @"SELECT msd.sbjnum FROM surveydata msd 
                        //join ProjectField pf on pf.ID=msd.FieldId
                        //            		WHERE
                        //					msd.sbjnum IN	--Quarter
                        //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                        //            				WHERE ISNULL(s.Test, 0) = 0 
                        //							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
                        //							GROUP BY s.sbjnum ) 
                        //		AND msd.sbjnum IN	--Dealer
                        //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                        //            				WHERE ISNULL(s.Test, 0) = 0 
                        //							and sd.FieldId = 49756 AND sd.FieldValue = {2} 
                        //							GROUP BY s.sbjnum  )
                        //and pf.ProjectID={0}
                        //GROUP BY msd.sbjnum";
                        //                        sbjIds = db.Database.SqlQuery<int>(string.Format(sql, Projectid.ToString(), Quarterids, item.ID)).ToList();


                        //                        foreach (var item1 in sbjIds)
                        //                        {
                        //                            List<GetData> DatalstTemp = new List<GetData>();

                        //                            sql = @"SELECT   SUM(Convert(float, SampleAnswers.Points * ISNULL(sd.Score, 0)) /10) Points , SUM(CASE WHEN sd.Score IS NULL THEN 0 ELSE 1 END) PointsFrom 
                        //FROM
                        // ProjectField pf 
                        //    INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
                        //LEFT OUTER JOIN fnSurveyValues({0}) sd ON sd.FieldID = pf.ID
                        // LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
                        //  CASE 
                        //   WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE sd.Answer END) 
                        //   WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' THEN sd.OptionCode 
                        //   ELSE sd.Answer END
                        // LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
                        //    WHERE 
                        //pf.ProjectID={1} and
                        //fs.id not in (1208,1209)";
                        //                            DatalstTemp = db.Database.SqlQuery<GetData>(string.Format(sql, item1.ToString(), Projectid.ToString())).ToList();
                        //                            value += DatalstTemp[0].Points ?? 0;
                        //                            Score += DatalstTemp[0].PointsFrom * 10;
                        //                        }

                        //                        if (Score != 0)
                        //                        {
                        //                            value = Math.Round(Convert.ToDouble(value / Score) * 100, 2);

                        //                        }
                        //                        else
                        //                        {
                        //                            value = 0;

                        //                        }
                        #endregion

                        string sqltoRun = string.Format(sql, Projectid.ToString(), Quarterids, "'" + item.ID.ToString() + "'", username);

                        GraphDataWithIdPercentage GdatawithIP1 = db.Database.SqlQuery<GraphDataWithIdPercentage>(sqltoRun).ToList()[0];
                        //GdatawithIP1.y = Convert.ToDouble(value);
                        GdatawithIP1.id = item.ID;
                        GdatawithIP1.name = item.Name;
                        gDataWp.Add(GdatawithIP1);
                    }

                }
            }
            return gDataWp;



        }


        public List<GraphDataWithIdPercentage> CityLevelDealer(int Projectid, string Quarterids, int? CitySearch, int? dealerid, string username)
        {
            string sql = "";
            List<SurveyData> sd = new List<SurveyData>();
            List<GetData> Datalst = new List<GetData>();
            ProjectContext db = new ProjectContext();
            List<CustomerBranch> custBranh;
            string dealers = "";
            List<GraphDataWithIdPercentage> gDataWp = new List<GraphDataWithIdPercentage>();

            if (CitySearch == null)
            {
                List<int> Cityidsother = new List<int> { 1, 5558, 2071 };

                List<int> Cityids = new List<int>();
                int Cityid = (int)db.CustomerBranch.Where(x => x.ID == dealerid).First().CityID;
                if (Cityidsother.Contains(Cityid))
                {
                    Cityids.Add(Cityid);
                    List<City> Cities = db.City.Where(x => Cityids.Contains(x.ID)).ToList();
                    foreach (var City in Cities)
                    {
                        custBranh = db.CustomerBranch.Where(x => x.CityID == City.ID && x.IsActive == true).ToList();
                        dealers = "'" + string.Join("','", db.CustomerBranch.Where(x => x.CityID == City.ID && x.IsActive == true).Select(x => x.ID).ToList()) + "'";
                        sql = sqlBranchScore;
                        sql = string.Format(sql, Projectid.ToString(), Quarterids, dealers, username);
                        GraphDataWithIdPercentage GdatawithIP = new GraphDataWithIdPercentage();
                        GdatawithIP = db.Database.SqlQuery<GraphDataWithIdPercentage>(sql).ToList()[0];
                        GdatawithIP.id = City.ID;
                        GdatawithIP.name = City.Name;
                        gDataWp.Add(GdatawithIP);
                    }
                    custBranh = db.CustomerBranch.Where(x => x.ID == dealerid && x.IsActive == true).ToList();
                    dealers = "'" + string.Join("','", db.CustomerBranch.Where(x => x.ID == dealerid && x.IsActive == true).Select(x => x.ID).ToList()) + "'";
                    sql = sqlBranchScore;

                    sql = string.Format(sql, Projectid.ToString(), Quarterids, dealers, username);
                    GraphDataWithIdPercentage GdatawithIP1 = new GraphDataWithIdPercentage();
                    GdatawithIP1 = db.Database.SqlQuery<GraphDataWithIdPercentage>(sql).ToList()[0];
                    GdatawithIP1.id = custBranh[0].ID;
                    GdatawithIP1.name = custBranh[0].Name;
                    GdatawithIP1.intenal = true;
                    gDataWp.Add(GdatawithIP1);
                }
                else
                {

                    dealers = "'" + string.Join("','", db.CustomerBranch.Where(x => !Cityidsother.Contains((int)x.CityID) && x.IsActive == true).Select(x => x.ID).ToList()) + "'";
                    sql = sqlBranchScore;
                    sql = string.Format(sql, Projectid.ToString(), Quarterids, dealers, username);

                    GraphDataWithIdPercentage GdatawithIP1 = db.Database.SqlQuery<GraphDataWithIdPercentage>(sql).ToList()[0];
                    GdatawithIP1.id = 0;
                    GdatawithIP1.intenal = true;
                    GdatawithIP1.name = "Other Cities";
                    gDataWp.Add(GdatawithIP1);


                    custBranh = db.CustomerBranch.Where(x => x.ID == dealerid && x.IsActive == true).ToList();
                    dealers = "'" + string.Join("','", db.CustomerBranch.Where(x => x.ID == dealerid && x.IsActive == true).Select(x => x.ID).ToList()) + "'";
                    sql = sqlBranchScore;
                    sql = string.Format(sql, Projectid.ToString(), Quarterids, dealers, username);
                    GdatawithIP1 = new GraphDataWithIdPercentage();
                    GdatawithIP1 = db.Database.SqlQuery<GraphDataWithIdPercentage>(sql).ToList()[0];
                    GdatawithIP1.id = custBranh[0].ID;
                    GdatawithIP1.intenal = true;
                    GdatawithIP1.name = custBranh[0].Name;
                    gDataWp.Add(GdatawithIP1);
                }
                #region
                //List<City> Cities = db.City.Where(x => Cityids.Contains(x.ID)).ToList();
                //foreach (var City in Cities)
                //{
                //    custBranh = db.CustomerBranch.Where(x => x.CityID == City.ID && x.IsActive == true).ToList();
                //    dealers = "'" + string.Join("','", db.CustomerBranch.Where(x => x.CityID == City.ID && x.IsActive == true).Select(x => x.ID).ToList()) + "'";
                //    sql = sqlBranchScore;
                //    sql = string.Format(sql, Projectid.ToString(), Quarterids, dealers);
                //    GraphDataWithIdPercentage GdatawithIP = new GraphDataWithIdPercentage();
                //    GdatawithIP = db.Database.SqlQuery<GraphDataWithIdPercentage>(sql).ToList()[0];
                //    GdatawithIP.id = City.ID;
                //    GdatawithIP.name = City.Name;
                //    gDataWp.Add(GdatawithIP);
                //}

                //dealers = "'" + string.Join("','", db.CustomerBranch.Where(x => !Cityids.Contains((int)x.CityID) && x.IsActive == true).Select(x => x.ID).ToList()) + "'";
                //sql = sqlBranchScore;
                //sql = string.Format(sql, Projectid.ToString(), Quarterids, dealers);

                //GraphDataWithIdPercentage GdatawithIP1 = db.Database.SqlQuery<GraphDataWithIdPercentage>(sql).ToList()[0];
                //GdatawithIP1.id = 0;
                //GdatawithIP1.name = "Metro";
                //gDataWp.Add(GdatawithIP1);
                #endregion
            }
            else
            {
                if (CitySearch > 0)
                {
                    sql = sqlBranchScore;
                    List<CustomerBranch> CustomerBrnch = db.CustomerBranch.Where(x => x.ID == dealerid && x.IsActive == true).ToList();
                    foreach (var CustB in CustomerBrnch)
                    {
                        string sqltoRun = "";
                        sqltoRun = string.Format(sql, Projectid.ToString(), Quarterids, "'" + CustB.ID.ToString() + "'", username);
                        GraphDataWithIdPercentage GdatawithIP = new GraphDataWithIdPercentage();
                        GdatawithIP = db.Database.SqlQuery<GraphDataWithIdPercentage>(sqltoRun).ToList()[0];
                        GdatawithIP.id = CustB.ID;
                        GdatawithIP.name = CustB.Name;
                        gDataWp.Add(GdatawithIP);
                    }
                }
                else
                {
                    List<int> Cityids = new List<int> { 1, 5558, 2071 };
                    custBranh = db.CustomerBranch.Where(x => x.ID == dealerid && x.IsActive == true).ToList();
                    sql = sqlBranchScore;
                    foreach (var item in custBranh)
                    {
                        string sqltoRun = string.Format(sql, Projectid.ToString(), Quarterids, "'" + item.ID.ToString() + "'", username);
                        GraphDataWithIdPercentage GdatawithIP1 = db.Database.SqlQuery<GraphDataWithIdPercentage>(sqltoRun).ToList()[0];
                        GdatawithIP1.id = item.ID;
                        GdatawithIP1.name = item.Name;
                        gDataWp.Add(GdatawithIP1);
                    }
                }
            }
            return gDataWp;
        }


        public List<GraphDataWithIdPercentage> DealerLevelSurvey(int Projectid, string Quarterids, int dealer, string username)
        {
            string sql = "";
            List<SurveyData> sd = new List<SurveyData>();
            List<GetData> Datalst = new List<GetData>();
            ProjectContext db = new ProjectContext();
            List<CustomerBranch> custBranh;

            custBranh = db.CustomerBranch.Where(x => x.ID == dealer && x.IsActive == true).ToList();



            List<GraphDataWithIdPercentage> gDataWp = new List<GraphDataWithIdPercentage>();
            foreach (var item in custBranh)
            {
                #region
                //                List<int> sbjIds = new List<int>();
                //                double? value = 0, Score = 0;

                //                sql = @"SELECT msd.sbjnum FROM surveydata msd 
                //join ProjectField pf on pf.ID=msd.FieldId
                //            		WHERE
                //					msd.sbjnum IN	--Quarter
                //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                //            				WHERE ISNULL(s.Test, 0) = 0 
                //							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
                //							GROUP BY s.sbjnum ) 
                //		AND msd.sbjnum IN	--Dealer
                //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                //            				WHERE ISNULL(s.Test, 0) = 0 
                //							and sd.FieldId = 49756 AND sd.FieldValue = {2} 
                //							GROUP BY s.sbjnum  )
                //and pf.ProjectID={0}
                //GROUP BY msd.sbjnum";
                //                sbjIds = db.Database.SqlQuery<int>(string.Format(sql, Projectid.ToString(), Quarterids, item.ID)).ToList();


                //                foreach (var item1 in sbjIds)
                //                {
                //                    List<GetData> DatalstTemp = new List<GetData>();

                //                    sql = @"SELECT   SUM(Convert(float, SampleAnswers.Points * ISNULL(sd.Score, 0)) /10) Points , SUM(CASE WHEN sd.Score IS NULL THEN 0 ELSE 1 END) PointsFrom 
                //FROM
                // ProjectField pf 
                //    INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
                //LEFT OUTER JOIN fnSurveyValues({0}) sd ON sd.FieldID = pf.ID
                // LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
                //  CASE 
                //   WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE sd.Answer END) 
                //   WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' THEN sd.OptionCode 
                //   ELSE sd.Answer END
                // LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
                //    WHERE 
                //pf.ProjectID={1} and
                //fs.id not in (1208,1209)";
                //                    DatalstTemp = db.Database.SqlQuery<GetData>(string.Format(sql, item1.ToString(), Projectid.ToString())).ToList();
                //                    value += DatalstTemp[0].Points ?? 0;
                //                    Score += DatalstTemp[0].PointsFrom * 10;
                //                }
                //                if (Score != 0)
                //                {
                //                    value = Math.Round(Convert.ToDouble(value / Score) * 100, 2);

                //                }
                //                else
                //                {
                //                    value = 0;

                //                }
                #endregion
                sql = string.Format(sqlBranchScore, Projectid.ToString(), Quarterids, "'" + item.ID.ToString() + "'", username);

                GraphDataWithIdPercentage GdatawithIP = db.Database.SqlQuery<GraphDataWithIdPercentage>(sql).ToList()[0];
                GdatawithIP.y = GdatawithIP.y;
                GdatawithIP.id = item.ID;
                GdatawithIP.name = item.Name;
                gDataWp.Add(GdatawithIP);

            }

            return gDataWp;
        }

        public List<GraphDataWithIdPercentage> NationalLevelDealer(int Projectid, string Quarterids, int dealerId, string username, string adminusername)
        {
            string sql = "";
            List<SurveyData> sd = new List<SurveyData>();
            List<GetData> Datalst = new List<GetData>();
            ProjectContext db = new ProjectContext();
            List<CustomerBranch> custBranh = db.CustomerBranch.Where(x => x.ID == dealerId).ToList();
            List<GraphDataWithIdPercentage> gDataWp = new List<GraphDataWithIdPercentage>();
            sql = string.Format(sqlBranchScore, Projectid.ToString(), Quarterids, "'" + dealerId.ToString() + "'", username);

            GraphDataWithIdPercentage GdatawithIP = db.Database.SqlQuery<GraphDataWithIdPercentage>(sql).ToList()[0];
            GdatawithIP.y = GdatawithIP.y;
            GdatawithIP.id = dealerId;
            GdatawithIP.name = GdatawithIP.name;
            gDataWp.Add(GdatawithIP);

            return gDataWp;
        }
        public List<GraphDataWithIdPercentage> NationalLevelAndRegionalLevelBranch(int Projectid, string Quarterids, int? dealerid, string usernameinternal, string usernameextrnal)
        {
            string sql = "";
            List<SurveyData> sd = new List<SurveyData>();
            List<GetData> Datalst = new List<GetData>();
            ProjectContext db = new ProjectContext();
            CustomerBranch custBranh = db.CustomerBranch.Where(x => x.ID == dealerid).Single();
            List<GraphDataWithIdPercentage> gDataWp = new List<GraphDataWithIdPercentage>();

            sql = string.Format(sqlBranchScore, Projectid.ToString(), Quarterids, "'" + custBranh.ID.ToString() + "'", usernameinternal);

            GraphDataWithIdPercentage GdatawithIP = db.Database.SqlQuery<GraphDataWithIdPercentage>(sql).ToList()[0];
            GdatawithIP.y = GdatawithIP.y;
            GdatawithIP.id = custBranh.ID;
            GdatawithIP.name = "Internal Audit " + custBranh.Name;
            GdatawithIP.intenal = true;
            gDataWp.Add(GdatawithIP);

            sql = string.Format(sqlBranchScore, Projectid.ToString(), Quarterids, "'" + custBranh.ID.ToString() + "'", usernameextrnal);

            GdatawithIP = db.Database.SqlQuery<GraphDataWithIdPercentage>(sql).ToList()[0];
            GdatawithIP.y = GdatawithIP.y;
            GdatawithIP.id = custBranh.ID;
            GdatawithIP.name = "External Audit " + custBranh.Name;
            GdatawithIP.intenal = false;
            gDataWp.Add(GdatawithIP);



            return gDataWp;
        }


        public List<GraphDataWithIdPercentage> NationalLevelAndRegionalLevel(int Projectid, string Quarterids, int? regionid, string username)
        {
            string sql = "";
            List<SurveyData> sd = new List<SurveyData>();
            List<GetData> Datalst = new List<GetData>();
            ProjectContext db = new ProjectContext();
            List<CustomerBranch> custBranh;

            if (regionid != null)
            {
                sql = @"select Cb.* from CustomerBranch Cb
			join City C on Cb.CityID=C.ID
			join RDXCustomerRegion RCR on RCR.CityID=C.ID
			
			 where CB.IsActive=1 and RCR.CustomerRegionID={0}
";
                custBranh = db.Database.SqlQuery<CustomerBranch>(string.Format(sql, regionid.ToString())).ToList();
            }
            else
            {
                custBranh = db.CustomerBranch.Where(x => x.IsActive == true).ToList();
            }

            List<GraphDataWithIdPercentage> gDataWp = new List<GraphDataWithIdPercentage>();
            foreach (var item in custBranh)
            {
                #region
                //                List<int> sbjIds = new List<int>();
                //                double? value = 0, Score = 0;

                //                sql = @"SELECT msd.sbjnum FROM surveydata msd 
                //join ProjectField pf on pf.ID=msd.FieldId
                //            		WHERE
                //					msd.sbjnum IN	--Quarter
                //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                //            				WHERE ISNULL(s.Test, 0) = 0 
                //							and sd.FieldId = 49757 AND sd.FieldValue IN ({1})
                //							GROUP BY s.sbjnum ) 
                //		AND msd.sbjnum IN	--Dealer
                //            				(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
                //            				WHERE ISNULL(s.Test, 0) = 0 
                //							and sd.FieldId = 49756 AND sd.FieldValue = {2} 
                //							GROUP BY s.sbjnum  )
                //and pf.ProjectID={0}
                //GROUP BY msd.sbjnum";
                //                sbjIds = db.Database.SqlQuery<int>(string.Format(sql, Projectid.ToString(), Quarterids, item.ID)).ToList();


                //                foreach (var item1 in sbjIds)
                //                {
                //                    List<GetData> DatalstTemp = new List<GetData>();

                //                    sql = @"SELECT   SUM(Convert(float, SampleAnswers.Points * ISNULL(sd.Score, 0)) /10) Points , SUM(CASE WHEN sd.Score IS NULL THEN 0 ELSE 1 END) PointsFrom 
                //FROM
                // ProjectField pf 
                //    INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
                //LEFT OUTER JOIN fnSurveyValues({0}) sd ON sd.FieldID = pf.ID
                // LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
                //  CASE 
                //   WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE sd.Answer END) 
                //   WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' THEN sd.OptionCode 
                //   ELSE sd.Answer END
                // LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
                //    WHERE 
                //pf.ProjectID={1} and
                //fs.id not in (1208,1209)";
                //                    DatalstTemp = db.Database.SqlQuery<GetData>(string.Format(sql, item1.ToString(), Projectid.ToString())).ToList();
                //                    value += DatalstTemp[0].Points ?? 0;
                //                    Score += DatalstTemp[0].PointsFrom * 10;
                //                }
                //                if (Score != 0)
                //                {
                //                    value = Math.Round(Convert.ToDouble(value / Score) * 100, 2);

                //                }
                //                else
                //                {
                //                    value = 0;

                //                }
                #endregion
                sql = string.Format(sqlBranchScore, Projectid.ToString(), Quarterids, "'" + item.ID.ToString() + "'", username);

                GraphDataWithIdPercentage GdatawithIP = db.Database.SqlQuery<GraphDataWithIdPercentage>(sql).ToList()[0];
                GdatawithIP.y = GdatawithIP.y;
                GdatawithIP.id = item.ID;
                GdatawithIP.name = item.Name;
                gDataWp.Add(GdatawithIP);

            }
            return gDataWp;
        }

        public List<SurveyDetails> GetLastSurveyDetail(string username, int projectid, int dealerid, int Quarterid, string torFilter = "", string torFunc = "")
        {
            ProjectContext db = new ProjectContext();
            string sql = "";
            sql = @"DECLARE @myTable SurveyIDType

insert into  @myTable
select Top 1 S.sbjnum as value

from Survey S  where S.sbjnum in (
select msd.sbjnum FROM SurveyData MSD
join ProjectField pf on pf.ID=MSD.FieldId

WHERE 
msd.sbjnum in (SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            		WHERE ISNULL(s.Test, 0) = 0
					and sd.FieldId = 49756 AND sd.FieldValue = '{2}'  and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
					  and pf.Projectid={1}
					GROUP BY s.sbjnum )
								ANd
									msd.sbjnum IN	--Quarter
            			(SELECT s.sbjnum FROM survey s INNER JOIN surveydata sd ON s.sbjnum = sd.sbjnum 
            			WHERE ISNULL(s.Test, 0) = 0 
						and sd.FieldId = 49757 AND sd.FieldValue ='{3}' 
						  and (s.OpStatus is null or s.OpStatus = 1) and (s.QCStatus is null or s.QCStatus = 1)
						GROUP BY s.sbjnum )
						and pf.Projectid={1}
group by msd.sbjnum)
order by
Created desc

SELECT fs.ID, pf.id Pid, pf.ParentFieldID, pf.FieldType, 
 pf.title as Section_name,
 SampleAnswers.Title name, 
 SampleAnswersOption.Title options,
 Convert(float, sd.Score)/10 AS Score,
 Convert(int, sd.Marked) Marked,
 SampleAnswersOption.FieldID,
 Convert(int, SampleAnswers.ID) AnswerId,
 fs.DisplayOrder fsDisplayOrder, pf.DisplayOrder pfDisplayOrder

FROM ProjectField pf 
 INNER JOIN ProjectFieldSection fs ON fs.ID = pf.SectionID
 LEFT OUTER JOIN {5}((select Top 1 sbjnum from @myTable)) sd ON sd.FieldID = pf.ID
 LEFT OUTER JOIN ProjectFieldSample SampleAnswers on sd.FieldId = SampleAnswers.FieldID and SampleAnswers.Code = 
  CASE 
  WHEN pf.FieldType = 'SCG' THEN (CASE WHEN SampleAnswers.ParentSampleID = 0 THEN -1 ELSE 
	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
  END) 
  WHEN pf.FieldType = 'DDN' OR pf.FieldType = 'RDO' OR (pf.FieldType = 'MLT' AND SampleAnswers.ParentSampleID IS NULL) THEN sd.OptionCode 
  ELSE
  	CASE WHEN IsNumeric(sd.Answer) = 1 THEN CONVERT(int, sd.Answer) ELSE -1 END
 END
 LEFT OUTER JOIN ProjectFieldSample SampleAnswersOption on sd.FieldId = SampleAnswersOption.FieldID and sd.OptionCode = SampleAnswersOption.Code and SampleAnswersOption.ParentSampleID = 0 AND pf.FieldType = 'SCG'
WHERE pf.ProjectID={1} and fs.ID not in (1209,1208) and (pf.FieldType not in ( 'MLT' ,'PIC'))
and pf.title != 'Topics' {4}
Order by fs.DisplayOrder, pf.DisplayOrder,fs.ID
";
            List<SurveyDetails> SurDetail = db.Database.SqlQuery<SurveyDetails>(string.Format(sql, username, projectid, dealerid, Quarterid, torFilter, torFunc)).ToList();
            return SurDetail;
        }
    }
}