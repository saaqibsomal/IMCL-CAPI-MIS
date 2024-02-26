using System.Web;
using System.Web.Optimization;

namespace DNA_CAPI_MIS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            System.Web.Optimization.BundleTable.EnableOptimizations = false;            //FOR DEBUGGING ONLY

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                                    "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/styles").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            var scriptBundle = new Bundle("~/Content/styles").Include(
                      "~/Content/AdminLTE/css/bootstrap.min.css",
                      "~/Content/AdminLTE/css/font-awesome.min.css",
                      "~/Content/AdminLTE/css/morris/morris.css",
                      "~/Content/AdminLTE/css/jvectormap/jquery-jvectormap-1.2.2.css",
                      "~/Content/AdminLTE/css/fullcalendar/fullcalendar.css",
                      "~/Content/AdminLTE/css/daterangepicker/daterangepicker-bs3.css",
                      "~/Content/AdminLTE/css/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css",
                      "~/Content/AdminLTE/css/AdminLTE.css");
            scriptBundle.Transforms.Clear();
            bundles.Add(scriptBundle);

            bundles.Add(new ScriptBundle("~/bundles/bootstrap/AdminLTE").Include(
                      "~/Content/AdminLTE/js/jquery-ui-1.10.3.min.js",
                      "~/Content/AdminLTE/js/bootstrap.min.js",
                      "~/Content/AdminLTE/js/plugins/sparkline/jquery.sparkline.min.js",
                      "~/Content/AdminLTE/js/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                      "~/Content/AdminLTE/js/plugins/jvectormap/jquery-jvectormap-world-mill-en.js",
                      "~/Content/AdminLTE/js/plugins/fullcalendar/fullcalendar.min.js",
                      "~/Content/AdminLTE/js/plugins/jqueryKnob/jquery.knob.js",
                      "~/Content/AdminLTE/js/plugins/daterangepicker/daterangepicker.js",
                      "~/Content/AdminLTE/js/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js",
                      "~/Content/AdminLTE/js/plugins/jqueryKnob/jquery.knob.js",
                      "~/Content/AdminLTE/js/plugins/iCheck/icheck.min.js",
                      "~/Content/AdminLTE/js/AdminLTE/app.js",
                      "~/Content/DSDS/jquery.nestable.js",
                      "~/Content/DSDS/dsds.js",
                      "~/Content/Survey/FunctionsLibrary.js"
                      ));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap/AdminLTE/morris").Include(
                      "~/Content/AdminLTE/js/plugins/morris/morris.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap/jqwidgets").Include(
                  "~/Content/jqwidgets/jqwidgets/jqxcore.js",
                  "~/Content/jqwidgets/jqwidgets/jqxdata.js",
                  "~/Content/jqwidgets/jqwidgets/jqxbuttons.js",
                  "~/Content/jqwidgets/jqwidgets/jqxscrollbar.js",
                  "~/Content/jqwidgets/jqwidgets/jqxmenu.js",
                  "~/Content/jqwidgets/jqwidgets/jqxgrid.js",
                  "~/Content/jqwidgets/jqwidgets/jqxgrid.edit.js",
                  "~/Content/jqwidgets/jqwidgets/jqxgrid.sort.js",
                  "~/Content/jqwidgets/jqwidgets/jqxgrid.filter.js",
                  "~/Content/jqwidgets/jqwidgets/jqxgrid.selection.js",
                  "~/Content/jqwidgets/jqwidgets/jqxgrid.grouping.js",
                  "~/Content/jqwidgets/jqwidgets/jqxgrid.aggregates.js",
                  "~/Content/jqwidgets/jqwidgets/jqxpanel.js",
                  "~/Content/jqwidgets/jqwidgets/jqxcheckbox.js",
                  "~/Content/jqwidgets/jqwidgets/jqxlistbox.js",
                  "~/Content/jqwidgets/jqwidgets/jqxcombobox.js",
                  "~/Content/jqwidgets/jqwidgets/jqxdropdownlist.js",
                  "~/Content/jqwidgets/jqwidgets/jqxcalendar.js",
                  "~/Content/jqwidgets/jqwidgets/jqxdatetimeinput.js",
                  "~/Content/jqwidgets/jqwidgets/jqxgrid.columnsresize.js",
                  "~/Content/jqwidgets/jqwidgets/jqxgrid.columnsreorder.js",
                  "~/Content/jqwidgets/jqwidgets/jqxdata.export.js",
                  "~/Content/jqwidgets/jqwidgets/jqxgrid.export.js",
                  "~/Content/jqwidgets/jqwidgets/globalization/globalize.js",
                  "~/Content/jqwidgets/scripts/demos.js"
            ));

            bundles.Add(new StyleBundle("~/Content/styles/jqwidgets").Include(
                  "~/Content/jqwidgets/jqwidgets/styles/jqx.base.css",
                  "~/Content/jqwidgets/jqwidgets/styles/jqx.metro.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap/filters").Include(
                  "~/Content/js/bootstrap-datepicker.js",
                  "~/Content/js/select2.min.js"
            ));
            bundles.Add(new StyleBundle("~/Content/styles/filters").Include(
                  "~/Content/css/datepicker.css",
                  "~/Content/css/select2.css"));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap/dialog").Include(
                  "~/Content/bootstrap3-dialog-master/dist/js/bootstrap-dialog.js"
            ));
            bundles.Add(new StyleBundle("~/Content/styles/bootstrap/dialog").Include(
                  "~/Content/bootstrap3-dialog-master/dist/css/bootstrap-dialog.css"));
        }
    }
}
