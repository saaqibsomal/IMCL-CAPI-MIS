using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DNA_CAPI_MIS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{file}.jpg");
            routes.IgnoreRoute("{file}.png");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");
            routes.IgnoreRoute("{resource}.kml/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "PQSettings",
                url: "Designer/PQSettings/{id}/{fieldId}/{fieldType}",
                defaults: new
                {
                    controller = "Designer",
                    action = "PQSettings",
                    id = UrlParameter.Optional,
                    fieldId = UrlParameter.Optional,
                    fieldType = UrlParameter.Optional
                }
            );
            routes.MapRoute(
                name: "PQSettingsEdit",
                url: "Designer/PQSettingsEdit/{id}/{fieldId}/{fieldType}",
                defaults: new
                {
                    controller = "Designer",
                    action = "PQSettingsEdit",
                    id = UrlParameter.Optional,
                    fieldId = UrlParameter.Optional,
                    fieldType = UrlParameter.Optional
                }
            );
            routes.MapRoute(
                name: "PQSettingsDelete",
                url: "Designer/PQSettingsDelete/{id}/{fieldId}/{fieldType}",
                defaults: new
                {
                    controller = "Designer",
                    action = "PQSettingsDelete",
                    id = UrlParameter.Optional,
                    fieldId = UrlParameter.Optional,
                    fieldType = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "GetProjectJSON",
                url: "Designer/GetProjectJSON/{id}/{language}",
                defaults: new
                {
                    controller = "Designer",
                    action = "GetProjectJSON",
                    id = UrlParameter.Optional,
                    language = UrlParameter.Optional
                }
            );
            routes.MapRoute(
                name: "GetProjectSectionFieldsJSON",
                url: "Designer/GetProjectSectionFieldsJSON/{id}/{language}",
                defaults: new
                {
                    controller = "Designer",
                    action = "GetProjectSectionFieldsJSON",
                    id = UrlParameter.Optional,
                    language = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "SurveyLocationWise",
                url: "{controller}/{action}/{id}/{language}/{country}",
                defaults: new { 
                    controller = "Census", 
                    action = "SurveyLocationWise", 
                    id = UrlParameter.Optional, 
                    language = UrlParameter.Optional, 
                    country = UrlParameter.Optional 
                }
            );

            routes.MapRoute(
                name: "GetDistrictsFilterByCities",
                url: "{controller}/{action}/{CityIDs}",
                defaults: new
                {
                    controller = "District",
                    action = "GetDistrictsFilterByCities",
                    CityIDs = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "SendData",
                url: "Designer/SendData",
                defaults: new
                {
                    controller = "Designer",
                    action = "SendData",
                }
            );

        }
    }
}
