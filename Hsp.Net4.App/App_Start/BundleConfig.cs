using System.Web;
using System.Web.Optimization;

namespace Hsp.Net4.App
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/WebFormsJs").Include(
                            "~/Scripts/WebForms/WebForms.js",
                            "~/Scripts/WebForms/WebUIValidation.js",
                            "~/Scripts/WebForms/MenuStandards.js",
                            "~/Scripts/WebForms/Focus.js",
                            "~/Scripts/WebForms/GridView.js",
                            "~/Scripts/WebForms/DetailsView.js",
                            "~/Scripts/WebForms/TreeView.js",
                            "~/Scripts/WebForms/WebParts.js"));

            // 若要使这些文件正常工作，采用适当的顺序是非常重要的；这些文件具有显式依赖关系
            bundles.Add(new ScriptBundle("~/bundles/MsAjaxJs").Include(
                            "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
                            "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
                            "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
                            "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备就绪，请使用 https://modernizr.com 上的生成工具仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        //"~/Scripts/plugin/popover.js",
                        "~/Scripts/bootstrap.bundle.js",
                        "~/Scripts/bootstrap-table.js",
                        "~/Scripts/locales/bootstrap-table-zh-CN.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-table.css",
                      "~/Content/Site.css"));

        }
    }
}
