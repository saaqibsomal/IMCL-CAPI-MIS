using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CensusRMS
{
    public partial class surveyproxy : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["r"] == "data")
            {
                GetSurveyData(Request.QueryString["sbjnum"]);
            }
            else if (Request.QueryString["r"] == "category")
            {
                GetCategoryForm(Request.QueryString["catId"]);
            }
            else if (Request.QueryString["r"] == "surveyview")
            {
                GetSurveyView(Request.QueryString["sbjnum"]);
               
            }
        }

        private void GetSurveyView(string sbjnum)
        {
            string resultContent = "";
            Response.Clear();
            using (var client = new HttpClient())
            {
                ///http://117.20.29.237:3001/api/getsurveyitem/7903301/all
                //client.BaseAddress = new Uri("http://apps.dna.com.sa:5312");
                client.BaseAddress = new Uri("http://demo.kcompute.com:8065");
                try
                {
                    var result = client.GetAsync("/api/getsurveyitem/"+sbjnum+"/all").Result;
                    resultContent = result.Content.ReadAsStringAsync().Result;
                    resultContent = resultContent.Replace("\\\"", "\"");
                    resultContent = resultContent.TrimStart(new char[] { '"' });
                    resultContent = resultContent.TrimEnd(new char[] { '"' });
                }
                catch
                {
                    return;
                }
            }
            Response.Write(resultContent);

        }

        private void GetSurveyData(string sbjnum)
        {
            string resultContent = "";
            Response.Clear();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.kcompute.com:8065");
                try
                {
                    var result = client.GetAsync("/api/getsurvey/" + sbjnum).Result;
                    resultContent = result.Content.ReadAsStringAsync().Result;
                    resultContent = resultContent.Replace("\\\"", "\"");
                    resultContent = resultContent.TrimStart(new char[] { '"' });
                    resultContent = resultContent.TrimEnd(new char[] { '"' });
                }
                catch
                {
                    return;
                }
            }
            Response.Write(resultContent);
        }
        private void GetCategoryForm(string catId)
        {
            string resultContent;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://demo.kcompute.com:8065");
                try
                {
                    var result = client.GetAsync("/api/getcategoryform"
                        + "/" + "1100" + "/" + "en" + "/" + catId).Result;
                    resultContent = result.Content.ReadAsStringAsync().Result;
                    resultContent = resultContent.Replace("\\\"", "\"");
                    resultContent = resultContent.TrimStart(new char[] { '"' });
                    resultContent = resultContent.TrimEnd(new char[] { '"' });
                }
                catch
                {
                    return;
                }
            }
            Response.Write(resultContent);
        }

    }
}