﻿// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments and CLS compliance
// 0108: suppress "Foo hides inherited member Foo. Use the new keyword if hiding was intended." when a controller and its abstract parent are both processed
#pragma warning disable 1591, 3008, 3009, 0108
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;

[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
public static partial class MVC
{
    public static Sfw.Sabp.Mca.Web.Controllers.AssessmentController Assessment = new Sfw.Sabp.Mca.Web.Controllers.T4MVC_AssessmentController();
    public static Sfw.Sabp.Mca.Web.Controllers.Base.LayoutController Layout = new Sfw.Sabp.Mca.Web.Controllers.Base.T4MVC_LayoutController();
    public static Sfw.Sabp.Mca.Web.Controllers.BreakPageController BreakPage = new Sfw.Sabp.Mca.Web.Controllers.T4MVC_BreakPageController();
    public static Sfw.Sabp.Mca.Web.Controllers.HomeController Home = new Sfw.Sabp.Mca.Web.Controllers.T4MVC_HomeController();
    public static Sfw.Sabp.Mca.Web.Controllers.PersonController Person = new Sfw.Sabp.Mca.Web.Controllers.T4MVC_PersonController();
    public static Sfw.Sabp.Mca.Web.Controllers.QuestionController Question = new Sfw.Sabp.Mca.Web.Controllers.T4MVC_QuestionController();
    public static T4MVC.SharedController Shared = new T4MVC.SharedController();
}

namespace T4MVC
{
}

namespace T4MVC
{
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public class Dummy
    {
        private Dummy() { }
        public static Dummy Instance = new Dummy();
    }
}

[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
internal partial class T4MVC_System_Web_Mvc_ActionResult : System.Web.Mvc.ActionResult, IT4MVCActionResult
{
    public T4MVC_System_Web_Mvc_ActionResult(string area, string controller, string action, string protocol = null): base()
    {
        this.InitMVCT4Result(area, controller, action, protocol);
    }
     
    public override void ExecuteResult(System.Web.Mvc.ControllerContext context) { }
    
    public string Controller { get; set; }
    public string Action { get; set; }
    public string Protocol { get; set; }
    public RouteValueDictionary RouteValueDictionary { get; set; }
}



namespace Links
{
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public static class Scripts {
        private const string URLPATH = "~/Scripts";
        public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
        public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
        public static readonly string _references_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/_references.min.js") ? Url("_references.min.js") : Url("_references.js");
        public static readonly string assessment_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/assessment.min.js") ? Url("assessment.min.js") : Url("assessment.js");
        public static readonly string assessment_min_js = Url("assessment.min.js");
        public static readonly string assessment_min_js_map = Url("assessment.min.js.map");
        public static readonly string bootbox_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/bootbox.min.js") ? Url("bootbox.min.js") : Url("bootbox.js");
        public static readonly string bootbox_min_js = Url("bootbox.min.js");
        public static readonly string bootstrap_datepicker_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/bootstrap-datepicker.min.js") ? Url("bootstrap-datepicker.min.js") : Url("bootstrap-datepicker.js");
        public static readonly string bootstrap_datepicker_min_js = Url("bootstrap-datepicker.min.js");
        public static readonly string bootstrap_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/bootstrap.min.js") ? Url("bootstrap.min.js") : Url("bootstrap.js");
        public static readonly string bootstrap_min_js = Url("bootstrap.min.js");
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static class globalize {
            private const string URLPATH = "~/Scripts/globalize";
            public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
            public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
            [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
            public static class cultures {
                private const string URLPATH = "~/Scripts/globalize/cultures";
                public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
                public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
                public static readonly string globalize_culture_en_GB_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/globalize.culture.en-GB.min.js") ? Url("globalize.culture.en-GB.min.js") : Url("globalize.culture.en-GB.js");
                public static readonly string globalize_cultures_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/globalize.cultures.min.js") ? Url("globalize.cultures.min.js") : Url("globalize.cultures.js");
            }
        
            public static readonly string globalize_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/globalize.min.js") ? Url("globalize.min.js") : Url("globalize.js");
        }
    
        public static readonly string gridmvc_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/gridmvc.min.js") ? Url("gridmvc.min.js") : Url("gridmvc.js");
        public static readonly string gridmvc_lang_ru_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/gridmvc.lang.ru.min.js") ? Url("gridmvc.lang.ru.min.js") : Url("gridmvc.lang.ru.js");
        public static readonly string gridmvc_min_js = Url("gridmvc.min.js");
        public static readonly string jquery_1_10_2_intellisense_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery-1.10.2.intellisense.min.js") ? Url("jquery-1.10.2.intellisense.min.js") : Url("jquery-1.10.2.intellisense.js");
        public static readonly string jquery_1_10_2_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery-1.10.2.min.js") ? Url("jquery-1.10.2.min.js") : Url("jquery-1.10.2.js");
        public static readonly string jquery_1_10_2_min_js = Url("jquery-1.10.2.min.js");
        public static readonly string jquery_1_10_2_min_map = Url("jquery-1.10.2.min.map");
        public static readonly string jquery_validate_vsdoc_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.validate-vsdoc.min.js") ? Url("jquery.validate-vsdoc.min.js") : Url("jquery.validate-vsdoc.js");
        public static readonly string jquery_validate_globalize_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.validate.globalize.min.js") ? Url("jquery.validate.globalize.min.js") : Url("jquery.validate.globalize.js");
        public static readonly string jquery_validate_globalize_min_js = Url("jquery.validate.globalize.min.js");
        public static readonly string jquery_validate_globalize_min_js_map = Url("jquery.validate.globalize.min.js.map");
        public static readonly string jquery_validate_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.validate.min.js") ? Url("jquery.validate.min.js") : Url("jquery.validate.js");
        public static readonly string jquery_validate_min_js = Url("jquery.validate.min.js");
        public static readonly string jquery_validate_unobtrusive_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/jquery.validate.unobtrusive.min.js") ? Url("jquery.validate.unobtrusive.min.js") : Url("jquery.validate.unobtrusive.js");
        public static readonly string jquery_validate_unobtrusive_min_js = Url("jquery.validate.unobtrusive.min.js");
        public static readonly string modernizr_2_6_2_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/modernizr-2.6.2.min.js") ? Url("modernizr-2.6.2.min.js") : Url("modernizr-2.6.2.js");
        public static readonly string question_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/question.min.js") ? Url("question.min.js") : Url("question.js");
        public static readonly string question_min_js = Url("question.min.js");
        public static readonly string question_min_js_map = Url("question.min.js.map");
        public static readonly string respond_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/respond.min.js") ? Url("respond.min.js") : Url("respond.js");
        public static readonly string respond_min_js = Url("respond.min.js");
        public static readonly string sabp_mca_web_app_js = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/sabp-mca-web-app.min.js") ? Url("sabp-mca-web-app.min.js") : Url("sabp-mca-web-app.js");
        public static readonly string sabp_mca_web_app_min_js = Url("sabp-mca-web-app.min.js");
        public static readonly string sabp_mca_web_app_min_js_map = Url("sabp-mca-web-app.min.js.map");
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public static class Content {
        private const string URLPATH = "~/Content";
        public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
        public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
        public static readonly string bootstrap_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/bootstrap.min.css") ? Url("bootstrap.min.css") : Url("bootstrap.css");
             
        public static readonly string bootstrap_min_css = Url("bootstrap.min.css");
        public static readonly string datepicker3_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/datepicker3.min.css") ? Url("datepicker3.min.css") : Url("datepicker3.css");
             
        public static readonly string Gridmvc_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/Gridmvc.min.css") ? Url("Gridmvc.min.css") : Url("Gridmvc.css");
             
        public static readonly string Gridmvc_min_css = Url("Gridmvc.min.css");
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public static class Images {
            private const string URLPATH = "~/Content/Images";
            public static string Url() { return T4MVCHelpers.ProcessVirtualPath(URLPATH); }
            public static string Url(string fileName) { return T4MVCHelpers.ProcessVirtualPath(URLPATH + "/" + fileName); }
            public static readonly string favicon_ico = Url("favicon.ico");
            public static readonly string info_group_png = Url("info-group.png");
            public static readonly string sabp_chevrons_2_png = Url("sabp-chevrons-2.png");
            public static readonly string sabp_chevrons_png = Url("sabp-chevrons.png");
            public static readonly string sabp_png = Url("sabp.png");
        }
    
        public static readonly string InformationLogo_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/InformationLogo.min.css") ? Url("InformationLogo.min.css") : Url("InformationLogo.css");
             
        public static readonly string Site_css = T4MVCHelpers.IsProduction() && T4Extensions.FileExists(URLPATH + "/Site.min.css") ? Url("Site.min.css") : Url("Site.css");
             
        public static readonly string Site_min_css = Url("Site.min.css");
    }

    
    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public static partial class Bundles
    {
        public static partial class Scripts 
        {
            public static partial class globalize 
            {
                public static partial class cultures 
                {
                    public static class Assets
                    {
                        public const string globalize_culture_en_GB_js = "~/Scripts/globalize/cultures/globalize.culture.en-GB.js"; 
                        public const string globalize_cultures_js = "~/Scripts/globalize/cultures/globalize.cultures.js"; 
                    }
                }
                public static class Assets
                {
                    public const string globalize_js = "~/Scripts/globalize/globalize.js"; 
                }
            }
            public static class Assets
            {
                public const string _references_js = "~/Scripts/_references.js"; 
                public const string assessment_js = "~/Scripts/assessment.js"; 
                public const string bootbox_js = "~/Scripts/bootbox.js"; 
                public const string bootbox_min_js = "~/Scripts/bootbox.min.js"; 
                public const string bootstrap_datepicker_js = "~/Scripts/bootstrap-datepicker.js"; 
                public const string bootstrap_datepicker_min_js = "~/Scripts/bootstrap-datepicker.min.js"; 
                public const string bootstrap_js = "~/Scripts/bootstrap.js"; 
                public const string bootstrap_min_js = "~/Scripts/bootstrap.min.js"; 
                public const string gridmvc_js = "~/Scripts/gridmvc.js"; 
                public const string gridmvc_lang_ru_js = "~/Scripts/gridmvc.lang.ru.js"; 
                public const string gridmvc_min_js = "~/Scripts/gridmvc.min.js"; 
                public const string jquery_1_10_2_intellisense_js = "~/Scripts/jquery-1.10.2.intellisense.js"; 
                public const string jquery_1_10_2_js = "~/Scripts/jquery-1.10.2.js"; 
                public const string jquery_1_10_2_min_js = "~/Scripts/jquery-1.10.2.min.js"; 
                public const string jquery_validate_globalize_js = "~/Scripts/jquery.validate.globalize.js"; 
                public const string jquery_validate_globalize_min_js = "~/Scripts/jquery.validate.globalize.min.js"; 
                public const string jquery_validate_js = "~/Scripts/jquery.validate.js"; 
                public const string jquery_validate_min_js = "~/Scripts/jquery.validate.min.js"; 
                public const string jquery_validate_unobtrusive_js = "~/Scripts/jquery.validate.unobtrusive.js"; 
                public const string jquery_validate_unobtrusive_min_js = "~/Scripts/jquery.validate.unobtrusive.min.js"; 
                public const string modernizr_2_6_2_js = "~/Scripts/modernizr-2.6.2.js"; 
                public const string question_js = "~/Scripts/question.js"; 
                public const string respond_js = "~/Scripts/respond.js"; 
                public const string respond_min_js = "~/Scripts/respond.min.js"; 
                public const string sabp_mca_web_app_js = "~/Scripts/sabp-mca-web-app.js"; 
            }
        }
        public static partial class Content 
        {
            public static partial class Images 
            {
                public static class Assets
                {
                }
            }
            public static class Assets
            {
                public const string bootstrap_css = "~/Content/bootstrap.css";
                public const string bootstrap_min_css = "~/Content/bootstrap.min.css";
                public const string datepicker3_css = "~/Content/datepicker3.css";
                public const string Gridmvc_css = "~/Content/Gridmvc.css";
                public const string InformationLogo_css = "~/Content/InformationLogo.css";
                public const string Site_css = "~/Content/Site.css";
            }
        }
    }
}

[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
internal static class T4MVCHelpers {
    // You can change the ProcessVirtualPath method to modify the path that gets returned to the client.
    // e.g. you can prepend a domain, or append a query string:
    //      return "http://localhost" + path + "?foo=bar";
    private static string ProcessVirtualPathDefault(string virtualPath) {
        // The path that comes in starts with ~/ and must first be made absolute
        string path = VirtualPathUtility.ToAbsolute(virtualPath);
        
        // Add your own modifications here before returning the path
        return path;
    }

    // Calling ProcessVirtualPath through delegate to allow it to be replaced for unit testing
    public static Func<string, string> ProcessVirtualPath = ProcessVirtualPathDefault;

    // Calling T4Extension.TimestampString through delegate to allow it to be replaced for unit testing and other purposes
    public static Func<string, string> TimestampString = System.Web.Mvc.T4Extensions.TimestampString;

    // Logic to determine if the app is running in production or dev environment
    public static bool IsProduction() { 
        return (HttpContext.Current != null && !HttpContext.Current.IsDebuggingEnabled); 
    }
}





#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108


