﻿using System.Web;
using System.Web.Optimization;

namespace MyWebsiteEntity
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

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

            BundleTable.EnableOptimizations = true;


            bundles.UseCdn = true;

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/StyleSheet.css",
                "~/Content/font-awesome.css",
                "~/Content/font-awesome.min.css"));

            //bundles.Add(new StyleBundle("~/Content/profile").Include("~/Content/StyleSheet - Profile.css"));

            //bundles.Add(new StyleBundle("~/Content/settings").Include("~/Content/StyleSheet - Settings.css"));

            bundles.Add(new StyleBundle("~/Content/PandS").Include(
                "~/Content/StyleSheet - Profile.css",
                "~/Content/StyleSheet - Settings.css"
                ));

            bundles.Add(new ScriptBundle("~/Scripts/jquery").Include(
            "~/Scripts/cappy_jquery.js",
            "~/Scripts/masonry.pkgd.min.js",
            "~/Scripts/jquery-1.11.1.min.js",
            "~/Scripts/jquery.unobstrusive-ajax.min.js"
            ));

        }
    }
}