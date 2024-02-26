using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DNA_CAPI_MIS.Reports.Viewer
{
    public partial class ReportViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Set the processing mode for the ReportViewer to Local
                CAPIReportViewer.ProcessingMode = ProcessingMode.Local;

                LocalReport localReport = CAPIReportViewer.LocalReport;

                localReport.ReportPath = ".\\Reports\\LocationSampleProgress.rdlc";

                int reportId = string.IsNullOrEmpty(Request.QueryString["r"]) ? 5 : Convert.ToInt32(Request.QueryString["r"]);
                string strFrom = Request.QueryString["dtFrom"];
                string strTo = Request.QueryString["dtTo"];
                if (string.IsNullOrEmpty(strFrom) || string.IsNullOrEmpty(strTo))
                {
                    strFrom = "01/01/2010";
                    strTo = "12/31/2099";
                }
                strFrom = strFrom.Substring(6, 4) + "-" + strFrom.Substring(0, 2) + "-" + strFrom.Substring(3, 2) + " 00:00:00";
                strTo = strTo.Substring(6, 4) + "-" + strTo.Substring(0, 2) + "-" + strTo.Substring(3, 2) + " 23:59:59";

                DataSet ds = GetDataSet(reportId, strFrom, strTo);
                ReportDataSource rds = new ReportDataSource("ds", ds.Tables[0]);

                localReport.DataSources.Clear();
                localReport.DataSources.Add(rds);

                //Parameters
                //var parametersCollection = new List<ReportParameter>();

                //parametersCollection.Add(new ReportParameter("dtFrom", dtFrom, false));
                //parametersCollection.Add(new ReportParameter("dtTo", dtTo, false));
                //localReport.SetParameters(parametersCollection);

                localReport.Refresh();    
            }
        }

        private System.Data.DataSet GetDataSet(int reportId, string dtFrom, string dtTo)
        {
            System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection("Data Source=192.168.1.9;Initial Catalog=CAPIMIS;Persist Security Info=True;User ID=capimis_admin;Password=CAPI!966");
            sqlConn.Open();
            string sql = "exec GetLocationPFSCount " + reportId + ", '" + dtFrom + "', '" + dtTo + "'";

            System.Data.SqlClient.SqlDataAdapter ad = new System.Data.SqlClient.SqlDataAdapter(sql, sqlConn);
            System.Data.DataSet ds = new System.Data.DataSet();
            ad.Fill(ds);
            sqlConn.Close();
            return ds;
        }
    }
}