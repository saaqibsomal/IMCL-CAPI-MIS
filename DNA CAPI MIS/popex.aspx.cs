using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CensusRMS
{
    public partial class popex : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string geom = Request.Form["geom"];
            string resultContent = "";

            if (geom == null)
            {
                Response.Clear();
                Response.Write(resultContent);
                return;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://populationexplorer.com");
                var content = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("geom", Request.Form["geom"]),
                new KeyValuePair<string, string>("loader", "analyse"),
                new KeyValuePair<string, string>("data", "1"),
                new KeyValuePair<string, string>("geometryID", ""),
                new KeyValuePair<string, string>("dataSource", ""),
                //new KeyValuePair<string, string>("featureID", "SPAMPB957UVJAZS"),
                new KeyValuePair<string, string>("featureID", "SPA1GE5019M1W56"),
                new KeyValuePair<string, string>("minx", Request.Form["minx"]),
                new KeyValuePair<string, string>("miny", Request.Form["miny"]),
                new KeyValuePair<string, string>("maxx", Request.Form["maxx"]),
                new KeyValuePair<string, string>("maxy", Request.Form["maxy"])
            });
                try
                {
                    var result = client.PostAsync("/Handlers/Data/Analysis.ashx", content).Result;
                    resultContent = result.Content.ReadAsStringAsync().Result;
                }
                catch {
                }

            }

            Response.Clear();
            Response.Write(resultContent);


        }
    }
}