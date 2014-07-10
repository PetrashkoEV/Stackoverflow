using System.Web;
using System.Web.Optimization;

namespace Stackoverflow
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Script
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/question").Include(
                       "~/Scripts/questionsScript.js"));

            bundles.Add(new ScriptBundle("~/bundles/dateformat").Include(
                        "~/Scripts/dateformat.js"));

            bundles.Add(new ScriptBundle("~/bundles/voute").Include(
                        "~/Scripts/voute.js"));

            bundles.Add(new ScriptBundle("~/bundles/cleditor").Include(
                       "~/Scripts/cleditor-min.js",
                       "~/Scripts/cleditor-ready-min.js"));
            
            // Style           
            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/Site.css",
                        "~/Content/Menu.css",
                        "~/Content/Button.css"));

            bundles.Add(new StyleBundle("~/Content/problems").Include("~/Content/Problems.css"));

            bundles.Add(new StyleBundle("~/Content/question").Include("~/Content/Question.css"));

            bundles.Add(new StyleBundle("~/Content/home").Include("~/Content/home.css"));

            bundles.Add(new StyleBundle("~/Content/cleditor").Include("~/Content/Cleditor.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}