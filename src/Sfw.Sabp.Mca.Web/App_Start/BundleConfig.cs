using System.Web.Optimization;

namespace Sfw.Sabp.Mca.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.validate.unobtrusive.js",
                        "~/Scripts/jquery.validate.globalize.js"));

            bundles.Add(new ScriptBundle("~/bundles/globalize").Include(
                        "~/Scripts/globalize/globalize.js",
                        "~/Scripts/globalize/cultures/globalize.cultures.js",
                        "~/Scripts/globalize/cultures/globalize.culture.en-GB.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-datepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                        "~/Scripts/gridmvc.js",
                        "~/Scripts/sabp-mca-web-app.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/bootbox.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/datepicker3.css",
                      "~/Content/gridmvc.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/question").Include(
                        "~/Scripts/question.js"));

            bundles.Add(new ScriptBundle("~/bundles/assessment").Include(
                        "~/Scripts/assessment.js"));
        }
    }
}
